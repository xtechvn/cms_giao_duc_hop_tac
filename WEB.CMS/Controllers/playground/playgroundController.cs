using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Caching.Elasticsearch;
using Caching.RedisWorker;
using Entities.Models;
using Entities.ViewModels;
using Entities.ViewModels.ApiSever;
using Entities.ViewModels.Attachment;
using Entities.ViewModels.ElasticSearch;
using Entities.ViewModels.PlaygroundDetai;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Repositories.IRepositories;
using Utilities;
using Utilities.Contants;
using GDHT.CMS.Service;
using WEB.CMS.Customize;
using WEB.CMS.Models;

namespace GDHT.CMS.Controllers.playground
{
    [CustomAuthorize]

    public class playgroundController : Controller
    {
        private readonly IPlaygroundDetaiRepository _playgroundDetaiRepository;
        private readonly IAllCodeRepository _allCodeRepository;
        private readonly IConfiguration _configuration;
        private readonly IAttachFileRepository _AttachFileRepository;
        private readonly ICommonRepository _commonRepository;
        private readonly ArticleESRepository _articleESRepository;
        private IUserRepository _userRepository;
        private readonly RedisConn _redisService;
        public playgroundController(IPlaygroundDetaiRepository playgroundDetaiRepository, IAllCodeRepository allCodeRepository, IConfiguration configuration,
            IAttachFileRepository AttachFileRepository, IUserRepository userRepository, ICommonRepository commonRepository)
        {
            _playgroundDetaiRepository = playgroundDetaiRepository;
            _allCodeRepository = allCodeRepository;
            _configuration = configuration;
            _AttachFileRepository = AttachFileRepository;
            _articleESRepository= new ArticleESRepository(_configuration["DataBaseConfig:Elastic:Host"]);
            _userRepository = userRepository;
            _redisService = new RedisConn(configuration);
            _redisService.Connect();
            _commonRepository = commonRepository;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Search(PlaygroundDetaiSeachViewModel searchModel, int currentPage = 1, int pageSize = 20)
        {
            var model = new GenericViewModel<PlaygroundDetaiViewModel>();

            try
            {
                model = await _playgroundDetaiRepository.GetListPlayground(searchModel, searchModel.PageIndex, searchModel.PageSize);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Search - playgroundController: " + ex);
            }
            ViewBag.model = model;
            return PartialView(model);
        }
        public async Task<IActionResult> PopupAdd(long id)
        {
            try
            {
                APIService apiService = new APIService(_configuration, _userRepository);
                var data = await apiService.GetProductCategory();
                var supplierList = data.data;
                string domain = _configuration["DomainConfig:ImageStatic"];
                var suggestionlist = supplierList.Select(s => new ProductCategoryViewModel
                {
                    id = s.id,
                    code = s.code,
                    image = s.image,
                    link = s.link,
                    name = s.name,
                }).ToList();
                ViewBag.suggestionlist = suggestionlist;
                var key_token_api = _configuration["DataBaseConfig:key_api:api_manual"];
                string ApiPrefix = ReadFile.LoadConfig().API_URL + ReadFile.LoadConfig().API_ALLCODE;
                var SERVICE_TYPE =  _allCodeRepository.GetListByType(AllCodeType.SERVICE_TYPE);
                ViewBag.SERVICE_TYPE = SERVICE_TYPE;
                if (id != 0)
                {
                  var detail=await _playgroundDetaiRepository.GetDetailPlaygroundDetail(id);   
                  ViewBag.PlaygroundDetail= detail;
                    var attach_file =await _AttachFileRepository.GetListByType(id, (int)AttachmentType.AddService_VinWonder);
                    ViewBag.ListAttachFile = attach_file;
                }
                ViewBag.domain = domain;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("PopupAdd - playgroundController: " + ex);
            }

            return PartialView();
        }

        public async Task<string> GetProductCategorySuggest(string name)
        {
            try
            {
                APIService apiService = new APIService(_configuration,_userRepository);
                var data = await apiService.GetProductCategory();
                var supplierList = data.data;
                if(name!=null)
                supplierList = supplierList.Where(s => s.name.ToLower().Contains(name.Trim().ToLower())).ToList();
                var suggestionlist = supplierList.Select(s => new ProductCategoryViewModel
                {
                    id = s.id,
                    code = s.code,
                    image = s.image,
                    link = s.link,
                    name = s.name,
                }).ToList();
               
                return JsonConvert.SerializeObject(suggestionlist);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetProductCategorySuggest - playgroundController: " + ex);
                return null;
            }
        }
        public async Task<IActionResult> AddPlayGround(PlaygroundDetaiViewModel data, List<string> attach_file)
        {
            var status = (int)ResponseType.FAILED;
            var msg = "Thêm mới không thành công";
            var id = 0;
            try
            {
                int db_index = Convert.ToInt32(_configuration["Redis:Database:db_common"]);
                string cache_name = CacheName.PLAYGROUND_DETAIL+data.Code;
                int user_id = 0;
                if (HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) != null)
                {
                    user_id = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                if (data.Id == 0)
                {
                    var PlaygroundDetail = await _playgroundDetaiRepository.GetPlaygroundDetailbyCode(data.Code.ToString());
                    if (PlaygroundDetail.Count > 0)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.ERROR,
                            msg = data.LocationName+" đã tồn tại",
                           
                        });
                    }
                    else
                    {
                        var AddPlayGround = await _playgroundDetaiRepository.InsertPlaygroundDetail(data);
                        if (AddPlayGround > 0)
                        {

                            status = (int)ResponseType.SUCCESS;
                            msg = "Thêm mới thành công";
                            id = AddPlayGround;
                            _redisService.clear(cache_name, db_index);
                        }
                        if (attach_file != null && attach_file.Count > 0)
                        {
                            var AttachFiles = new List<AttachFile>();
                            foreach (var image in attach_file)
                            {
                                var str_path = UpLoadHelper.UploadBase64Src(image, _configuration["DomainConfig:ImageStatic"]).Result;
                                if (!string.IsNullOrEmpty(str_path))
                                {
                                    AttachFiles.Add(new AttachFile
                                    {
                                        DataId = AddPlayGround,
                                        Path = str_path,
                                        Type = (int)AttachmentType.AddService_VinWonder,
                                        UserId = user_id
                                    });
                                }
                                
                            }
                            var SaveAttachFile = await _AttachFileRepository.CreateMultiple(AttachFiles);
                        }
                        //if (attach_file != null && attach_file.Count > 0)
                        //{
                        //    var SaveAttachFile = await _AttachFileRepository.SaveAttachFileURL(attach_file, AddPlayGround, user_id, (int)ServicesType.VinWonder);
                        //}
                    }
                    
                }
                else
                {
                    var UpdatelayGround = await _playgroundDetaiRepository.UpdatePlaygroundDetail(data);
                     if (UpdatelayGround > 0)
                    {
                        status = (int)ResponseType.SUCCESS;
                        msg = "Cập nhật thành công";
                        id = UpdatelayGround;
                        _redisService.clear(cache_name, db_index);
                    }
                    var Delete = _AttachFileRepository.DeleteAttachFilesByDataId(data.Id, (int)AttachmentType.AddService_VinWonder);
                    if (attach_file != null && attach_file.Count > 0)
                    {
                        var AttachFiles = new List<AttachFile>();
                        foreach (var image in attach_file)
                        {
                            var str_path = UpLoadHelper.UploadBase64Src(image, _configuration["DomainConfig:ImageStatic"]).Result;
                            if (!string.IsNullOrEmpty(str_path))
                            {
                                AttachFiles.Add(new AttachFile
                                {
                                    DataId = data.Id,
                                    Path = str_path,
                                    Type = (int)AttachmentType.AddService_VinWonder,
                                    UserId = user_id
                                });
                            }

                        }
                        var SaveAttachFile = await _AttachFileRepository.CreateMultiple(AttachFiles);
                    }
                  
                }
               
              
              
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("AddPlayGround - playgroundController: " + ex);
                return null;
            }
            return Ok(new
            {
                status = status,
                msg = msg,
                id=id
            });
        }
        public async Task<IActionResult> DeletePlayGround(long id)
        {
            try
            {

                if (id != 0)
                {
                    var Delete = await _playgroundDetaiRepository.DeletePlaygroundDetail(id);
                    if(Delete>0)
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        msg = "Xóa thành công"
                    });
                }
             
               
                
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdatePlayGround - playgroundController: " + ex);
        
            }
            return Ok(new
            {
                status = (int)ResponseType.ERROR,
                msg = "Xóa không thành công"
            });
        }
        public async Task<IActionResult> ArticleSuggestion(string txt_search)
        {

            try
            {
               
                    var data = await _articleESRepository.GetArticleSuggesstion(txt_search);
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        data = data
                    });
                
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ArticleSuggestion - playgroundController: " + ex);
               
            }
            return Ok(new
            {
                status = (int)ResponseType.SUCCESS,
                data = new List<ArticleESViewModel>()
            });

        }
    }
}
