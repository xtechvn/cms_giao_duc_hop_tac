using Caching.Elasticsearch;
using Caching.RedisWorker;
using Entities.Models;
using Entities.ViewModels;
using Entities.ViewModels.Tour;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;
using WEB.CMS.Customize;

namespace GDHT.CMS.Controllers.SetService
{
    [CustomAuthorize]
    public class TourProductController : Controller
    {
        private readonly IAllCodeRepository _allCodeRepository;
        private readonly ICommonRepository _commonRepository;
        private readonly ITourRepository _TourRepository;
        private readonly RedisConn _redisService;
        private readonly IConfiguration _configuration;
        //private readonly IBrandRepository _brandRepository;
        private TourESRepository _tourESRepository;

        public TourProductController(IAllCodeRepository allCodeRepository, ICommonRepository commonRepository, IConfiguration configuration,
            ITourRepository tourRepository)
        {
            _allCodeRepository = allCodeRepository;
            _commonRepository = commonRepository;
            _TourRepository = tourRepository;
            _configuration = configuration;
            _redisService = new RedisConn(configuration);
            _redisService.Connect();
            _tourESRepository = new TourESRepository(_configuration["DataBaseConfig:Elastic:Host"]);
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.TourTypes = _allCodeRepository.GetListByType("TOUR_TYPE");
            ViewBag.OrganizingTypes = _allCodeRepository.GetListByType("ORGANIZING_TYPE");
            ViewBag.Provinces = await _commonRepository.GetProvinceList();
            return View();
        }

        [HttpPost]
        public IActionResult Search(TourProductSearchModel searchModel)
        {
            var model = new GenericViewModel<TourProductGridModel>();
            try
            {
                searchModel.IsSelfDesign = false;
                var datas = _TourRepository.GetPagingTourProduct(searchModel);
                model.CurrentPage = searchModel.PageIndex;
                model.ListData = datas.ToList();
                model.PageSize = searchModel.PageSize;
                model.TotalRecord = datas != null && datas.Any() ? datas.FirstOrDefault().TotalRow : 0;
                model.TotalPage = (int)Math.Ceiling((double)model.TotalRecord / searchModel.PageSize);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Search - TourProductController: " + ex);
            }
            return PartialView(model);
        }

        public async Task<IActionResult> AddOrUpdate(long id)
        {
            var model = new TourProductUpsertModel()
            {
                IsDisplayWeb = false
            };

            if (id > 0)
            {
                var tourProduct = await _TourRepository.GetTourProductById(id);
                var TourPosition = await _TourRepository.GetListTourPosition(id);
                var TourPositionB2B = TourPosition != null && TourPosition.Count > 0 ? TourPosition.Where(S => S.PositionType == Utilities.Contants.PositionType.B2B).ToList() : null;
                var TourPositionB2C = TourPosition != null && TourPosition.Count > 0 ? TourPosition.Where(S => S.PositionType == Utilities.Contants.PositionType.B2C).ToList() : null;

                model = new TourProductUpsertModel()
                {
                    Id = tourProduct.Id,
                    AdditionInfo = tourProduct.AdditionInfo,
                    Avatar = tourProduct.Avatar,
                    DateDeparture = tourProduct.DateDeparture,
                    Days = tourProduct.Days,
                    Description = tourProduct.Description,
                    Exclude = tourProduct.Exclude,
                    Include = tourProduct.Include,
                    IsDisplayWeb = tourProduct.IsDisplayWeb,
                    Note = tourProduct.Note,
                    OldPrice = tourProduct.OldPrice,
                    OrganizingType = tourProduct.OrganizingType,
                    Price = tourProduct.Price,
                    Refund = tourProduct.Refund,
                    Schedule = tourProduct.Schedule,
                    Star = tourProduct.Star,
                    StartPoint = tourProduct.StartPoint,
                    Status = tourProduct.Status,
                    SupplierId = tourProduct.SupplierId,
                    Surcharge = tourProduct.Surcharge,
                    TourName = tourProduct.TourName,
                    TourType = tourProduct.TourType,
                    Transportation = tourProduct.Transportation,
                    PositionB2B = TourPositionB2B != null && TourPositionB2B.Count > 0 ? (int)TourPositionB2B[0].Position : 0,
                    PositionB2C = TourPositionB2C != null && TourPositionB2C.Count>0 ? (int)TourPositionB2C[0].Position : 0,
                };

                if (!string.IsNullOrEmpty(model.Schedule))
                {
                    model.TourSchedule = JsonConvert.DeserializeObject<IEnumerable<TourProductScheduleModel>>(model.Schedule);
                }
                else
                {
                    if (model.Days.HasValue && model.Days.Value > 0)
                    {
                        var ListShedule = new List<TourProductScheduleModel>();
                        for (int i = 1; i <= model.Days; i++)
                        {
                            ListShedule.Add(new TourProductScheduleModel
                            {
                                day_num = i,
                                day_title = string.Empty,
                                day_description = string.Empty
                            });
                        }
                        model.TourSchedule = ListShedule;
                    }
                }

                var attachments = _commonRepository.GetAttachFilesByDataIdAndType(model.Id, (int)AttachmentType.TOUR_PRODUCT);
                if (attachments != null && attachments.Any())
                {
                    model.OtherImages = attachments.Select(s => s.Path);
                }

                if (model.SupplierId != null && model.SupplierId > 0)
                {
                    var supplier_model = await _commonRepository.GetSupplierById((int)model.SupplierId);
                    if (supplier_model != null) model.SupplierName = supplier_model.FullName;
                }

                var tourDestination = await _TourRepository.GetTourDestinationByTourProductId(id);
                if (tourDestination != null && tourDestination.Any())
                {
                    model.EndPoints = tourDestination.Select(s => (int)s.LocationId);
                }
            }

            ViewBag.TourTransports = _allCodeRepository.GetListByType("TOUR_PRODUCT_TRANSPORT");
            ViewBag.TourTypes = _allCodeRepository.GetListByType("TOUR_TYPE");
            ViewBag.OrganizingTypes = _allCodeRepository.GetListByType("ORGANIZING_TYPE");
            ViewBag.ClientTypes = _allCodeRepository.GetListByType("CLIENT_TYPE");

            var provinces = await _commonRepository.GetProvinceList();

            if (model.TourType == 3)
            {
                var Nationals = await _commonRepository.GetNationalList();
                ViewBag.EndPoints = Nationals.Select(s => new Province { Id = s.Id, Name = s.NameVn }).ToList();
            }
            else
            {
                ViewBag.EndPoints = provinces;
            }

            ViewBag.Provinces = provinces;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpsertTourProduct([FromForm] TourProductUpsertModel model)
        {
            try
            {
                model.IsSelfDesigned = false;
                var tour_id =await _TourRepository.UpsertTourProduct(model);

                if (tour_id > 0)
                {
                    if (model.TourType != null)
                    {
                        int db_index = Convert.ToInt32(_configuration["Redis:Database:db_common"]);
                        string cache_name = CacheName.B2C_TOUR_Search;
                         _redisService.DeleteCacheByKeyword(cache_name, db_index);
                        cache_name = CacheName.B2C_TOUR_ListTour;
                        _redisService.DeleteCacheByKeyword(cache_name, db_index);
                        cache_name = CacheName.B2C_TOUR_ListTour_POSITION;
                        _redisService.DeleteCacheByKeyword(cache_name, db_index);
                        cache_name = CacheName.LIST_TOUR_TYPE;
                        _redisService.DeleteCacheByKeyword(cache_name, db_index);    
                        cache_name = CacheName.B2C_TOUR_Favourites;
                        _redisService.DeleteCacheByKeyword(cache_name, db_index);
                    }

                    return new JsonResult(new
                    {
                        isSuccess = true,
                        message = "Cập nhật sản phẩm tour thành công",
                        ProductID = tour_id
                    });
                }
                else
                {
                    return new JsonResult(new
                    {
                        isSuccess = false,
                        message = "Cập nhật sản phẩm tour thất bại"
                    });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    isSuccess = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost]
        public IActionResult DeleteTourProduct(int Id)
        {
            try
            {
                var tour_id = _TourRepository.DeleteTourProduct(Id);

                if (tour_id > 0)
                {
                    return new JsonResult(new
                    {
                        isSuccess = true,
                        message = "Xóa sản phẩm tour thành công"
                    });
                }
                else
                {
                    return new JsonResult(new
                    {
                        isSuccess = false,
                        message = "Xóa sản phẩm tour thất bại"
                    });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    isSuccess = false,
                    message = ex.Message
                });
            }
        }

        public async Task<IActionResult> GetEnpointListByType(int type)
        {
            if (type == 3)
            {
                var data = await _commonRepository.GetNationalList();
                return new JsonResult(data.Select(s => new
                {
                    Id = s.Id,
                    Name = s.NameVn
                }));
            }
            else
            {
                var data = await _commonRepository.GetProvinceList();
                return new JsonResult(data.Select(s => new
                {
                    s.Id,
                    s.Name
                }));
            }
        }
        [HttpPost]
        public async Task<IActionResult> GetTourProgramPackages(long tour_product_id)
        {
            try
            {
                var tour_prices = await _TourRepository.GetListTourProgramPackagesByTourProductId(tour_product_id);
                var client_types = _allCodeRepository.GetListByType("CLIENT_TYPE");

                if (tour_prices != null && tour_prices.Count > 0)
                {
                    return new JsonResult(new
                    {
                        isSuccess = true,
                        data = tour_prices,
                        client_types = client_types
                    });
                }
                else
                {
                    return new JsonResult(new
                    {
                        isSuccess = false,
                        msg = "No Data",
                        client_types = client_types

                    });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    isSuccess = false,
                    msg = ex.Message
                });
            }
        }
        [HttpPost]
        public async Task<IActionResult> UpSertProductPrices(long tour_product_id, List<TourProgramPackages> model_upload)
        {
            try
            {
                int _UserId = 0;
                if (HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) != null)
                {
                    _UserId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                var success = await _TourRepository.UpsertTourProductPrices(tour_product_id, model_upload, _UserId);
                if (success)
                {
                    return new JsonResult(new
                    {
                        isSuccess = true,
                        message = "Cập nhật lịch trình tour thành công"
                    });
                }
                else
                {
                    return new JsonResult(new
                    {
                        isSuccess = false,
                        message = "Không thể cập nhật lịch trình Tour, vui lòng liên hệ bộ phận kỹ thuật"
                    });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    isSuccess = false,
                    message = ex.Message
                });
            }
        }
        [HttpPost]
        public async Task<IActionResult> DeleteProductPrice(long price_id)
        {
            try
            {
                int _UserId = 0;
                if (HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) != null)
                {
                    _UserId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                var success = _TourRepository.DeleteTourProgramPackages(price_id, _UserId);
                if (success > 0)
                {
                    return new JsonResult(new
                    {
                        isSuccess = true,
                        message = "Cập nhật lịch trình tour thành công"
                    });
                }
                else
                {
                    return new JsonResult(new
                    {
                        isSuccess = false,
                        message = "Không thể cập nhật lịch trình Tour, vui lòng liên hệ bộ phận kỹ thuật"
                    });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    isSuccess = false,
                    message = ex.Message
                });
            }
        }
    }
}
