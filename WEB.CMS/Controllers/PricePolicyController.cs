using Caching.Elasticsearch;
using Caching.RedisWorker;
using Entities.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;
using WEB.CMS.Customize;

namespace GDHT.CMS.Controllers
{
    [CustomAuthorize]

    public partial class PricePolicyController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IAllCodeRepository _allCodeRepository;
        private readonly ICampaignRepository _campaignRepository;
        private readonly IPriceDetailRepository _priceDetailRepository;
        private readonly IGroupProductRepository _groupProductRepository;
        private readonly IProductFlyTicketServiceRepository _productFlyTicketServiceRepository;
        private readonly IProductRoomServiceRepository _productRoomServiceRepository;
        private readonly IServicePriceRoomRepository _servicePiceRoomRepository;
        private readonly RedisConn _redisService;
        private readonly IRoomFunRepository _roomFunRepository;
        private HotelESRepository _hotelESRepository;
        private readonly IHotelRepository _hotelRepository;
        private ManagementUser _ManagementUser;
        private readonly IWebHostEnvironment _WebHostEnvironment;

        public PricePolicyController(IConfiguration configuration, ICampaignRepository campaignRepository, IAllCodeRepository allCodeRepository,
            IPriceDetailRepository priceDetailRepository, IGroupProductRepository groupProductRepository, 
             IProductFlyTicketServiceRepository productFlyTicketServiceRepository, ManagementUser managementUser, IWebHostEnvironment WebHostEnvironment,
             IServicePriceRoomRepository servicePiceRoomRepository, IRoomFunRepository roomFunRepository, IHotelRepository hotelRepository, IProductRoomServiceRepository productRoomServiceRepository)
        {
            _configuration = configuration;
            _campaignRepository = campaignRepository;
            _priceDetailRepository = priceDetailRepository;
            _groupProductRepository = groupProductRepository;
            _productFlyTicketServiceRepository = productFlyTicketServiceRepository;
            _redisService = new RedisConn(_configuration);
            _redisService.Connect();
            _allCodeRepository = allCodeRepository;
            _servicePiceRoomRepository = servicePiceRoomRepository;
            _roomFunRepository = roomFunRepository;
            _hotelESRepository = new HotelESRepository(_configuration["DataBaseConfig:Elastic:Host"]);
            _hotelRepository = hotelRepository;
            _productRoomServiceRepository = productRoomServiceRepository;
            _ManagementUser = managementUser;
            _WebHostEnvironment = WebHostEnvironment;

        }
        /// <summary>
        /// View
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            ViewBag.ServiceType = _allCodeRepository.GetListByType(AllCodeType.SERVICE_TYPE);

            return View();
        }
        /// <summary>
        /// View
        /// </summary>
        /// <returns></returns>
        public IActionResult AddNew()
        {
            var service_list = _allCodeRepository.GetListByType(AllCodeType.SERVICE_TYPE);
            service_list.Remove(service_list.Where(x => x.CodeValue == (int)ServicesType.OthersHotelRent).FirstOrDefault());
            return View(service_list);
        }
        /// <summary>
        /// Hàm tìm kiếm để filter ngoài trang hiển thị tất cả chiến dịch
        /// </summary>
        /// <param name="searchModel"></param>
        /// <param name="currentPage"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Search(PricePolicySearchModel searchModel, int currentPage = 1, int pageSize = 20)
        {
            var model = new GenericViewModel<PricePolicyListingModel>();
            try
            {
                if (searchModel.ToDate == DateTime.MinValue) searchModel.ToDate = DateTime.MaxValue;
                var status_int = searchModel.CampaginStatus.Split(",");
                List<string> status_str = new List<string>();
                foreach(var status_num in status_int)
                {
                    if(CampaignStatus.CampaignStatusConstant.ContainsKey(status_num))
                    status_str.Add(CampaignStatus.CampaignStatusConstant[status_num]);
                }
                
                searchModel.CampaginStatus = string.Join(",", status_str);
                model = _campaignRepository.GetPagingList(searchModel, currentPage, pageSize);
              
            }
                
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Search - PricePolicyController: " + ex);
            }
            return PartialView(model);
        }
        [HttpPost]
        public async Task<IActionResult> ExportExcel(PricePolicySearchModel searchModel)
        {
            try
            {
                int _UserId = 0;
                if (HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) != null)
                {
                    _UserId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                }

               
                
                string folder = @"\wwwroot\Template\Export\" + _UserId;
                string full_path = Directory.GetCurrentDirectory() + folder;
                string file_name = StringHelpers.GenFileName("Danh sách chính sách giá", _UserId, "xlsx");

                string _UploadDirectory = Path.Combine(_WebHostEnvironment.WebRootPath, folder);
                string file_path_combine = Path.Combine(_UploadDirectory, file_name);
                try
                {
                    System.IO.DirectoryInfo di = new DirectoryInfo(full_path);
                    if (!di.Exists)
                    {
                        Directory.CreateDirectory(full_path);
                    }
                    else
                    {
                        foreach (FileInfo file in di.GetFiles())
                        {
                            file.Delete();
                        }
                    }
                }
                catch
                {

                }


                var file_path = await _campaignRepository.ExportCampaignExcel( _campaignRepository.GetPagingList(searchModel, 1, 100000), file_path_combine);

                return Ok(new
                {
                    status = (int)ResponseType.SUCCESS,
                    msg = "Xuất dữ liệu thành công",
                    path = file_path.Replace(@"\wwwroot", "")
                });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Search - OperatorReportController: " + ex);
            }
            return Ok(new
            {
                status = (int)ResponseType.FAILED,
                msg = "Xuất dữ liệu thất bại, vui lòng liên hệ IT",
                path = ""
            });
        }
    }
}
