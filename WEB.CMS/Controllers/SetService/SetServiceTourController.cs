using Entities.ViewModels;
using Entities.Models;
using Entities.ViewModels.Tour;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;
using WEB.CMS.Customize;
using WEB.CMS.Models;
using Caching.Elasticsearch;
using Entities.ViewModels.ElasticSearch;
using static Utilities.Contants.OrderConstants;
using MongoDB.Driver.Linq;
using GDHT.CMS.Service;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using static Utilities.DepositHistoryConstant;

namespace GDHT.CMS.Controllers.SetService.Tour
{
    [CustomAuthorize]
    public class SetServiceController : Controller
    {

        private readonly IConfiguration _configuration;
        private readonly ITourRepository _tourRepository;
        private readonly IAllCodeRepository _allCodeRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderRepositor _orderRepositor;
        private readonly IHotelBookingCodeRepository _hotelBookingCodeRepository;
        private readonly IContactClientRepository _contactClientRepository;
        private readonly TourESRepository _tourESRepository;
        private readonly IPaymentRequestRepository _paymentRequestRepository;
        private readonly ITourPackagesOptionalRepository _tourPackagesOptionalRepository;
        private ManagementUser _ManagementUser;
        private APIService apiService;
        private readonly IUserRepository _userRepository;
        private readonly IWebHostEnvironment _WebHostEnvironment;
        private readonly IContractPayRepository _contractPayRepository;
        public SetServiceController(IConfiguration configuration, ITourRepository tourRepository, IAllCodeRepository allCodeRepository, IOrderRepository orderRepository, IOrderRepositor orderRepositor, ManagementUser managementUser, IUserRepository userRepository,
            IHotelBookingCodeRepository hotelBookingCodeRepository, IContactClientRepository contactClientRepository, IPaymentRequestRepository paymentRequestRepository, ITourPackagesOptionalRepository tourPackagesOptionalRepository, IWebHostEnvironment WebHostEnvironment, IContractPayRepository contractPayRepository)
        {
            _configuration = configuration;
            _tourRepository = tourRepository;
            _allCodeRepository = allCodeRepository;
            _orderRepository = orderRepository;
            _orderRepositor = orderRepositor;
            _hotelBookingCodeRepository = hotelBookingCodeRepository;
            _contactClientRepository = contactClientRepository;
            _tourESRepository = new TourESRepository(_configuration["DataBaseConfig:Elastic:Host"]);
            _paymentRequestRepository = paymentRequestRepository;
            _tourPackagesOptionalRepository = tourPackagesOptionalRepository;
            _ManagementUser = managementUser;
            apiService = new APIService(configuration, userRepository);
            _WebHostEnvironment = WebHostEnvironment;
            _contractPayRepository = contractPayRepository;
        }
        public async Task<IActionResult> Tour()
        {

            var BOOKING_HOTEL = _allCodeRepository.GetListByType(AllCodeType.BOOKING_HOTEL_ROOM_STATUS);
            var BOOKING_HOTEL2 = _allCodeRepository.GetListByType(AllCodeType.BOOKING_HOTEL_ROOM_STATUS);
            var ORGANIZING_TYPE = _allCodeRepository.GetListByType(AllCodeType.ORGANIZING_TYPE);
            var TOUR_TYPE = _allCodeRepository.GetListByType(AllCodeType.TOUR_TYPE);
            ViewBag.ORGANIZING_TYPE = ORGANIZING_TYPE;

            ViewBag.TOUR_TYPE = TOUR_TYPE;
            var searchModel = new TourSearchViewModel();

            var current_user = _ManagementUser.GetCurrentUser();
            if (current_user != null)
            {
                if (current_user.Role != "")
                {
                    var list = current_user.Role.Split(',');
                    foreach (var item in list)
                    {
                        switch (Convert.ToInt32(item))
                        {


                            case (int)RoleType.TPTour:
                            case (int)RoleType.DHPQ:
                            case (int)RoleType.DHTour:
                                {
                                    if (searchModel.SalerPermission == null || searchModel.SalerPermission.Trim() == "")
                                    {
                                        searchModel.SalerPermission = current_user.UserUnderList;
                                    }
                                    else
                                    {
                                        searchModel.SalerPermission += "," + current_user.UserUnderList;

                                    }
                                }
                                break;
                            case (int)RoleType.Admin:
                            case (int)RoleType.KT:
                            case (int)RoleType.GDHN:
                            case (int)RoleType.GDHPQ:
                            case (int)RoleType.GD:
                            case (int)RoleType.TPDHTour:
                                {
                                    searchModel.SalerPermission = null;
                                }
                                break;
                        }
                    }

                    var CountTourByStatus = await _tourRepository.CountTourByStatus(searchModel);
                    ViewBag.CountTourByStatus = CountTourByStatus;
                }
            }


            var BOOKING_HOTEL_Data = BOOKING_HOTEL;
            var BOOKING_HOTEL_Data2 = BOOKING_HOTEL_Data;
            ViewBag.BOOKING_HOTEL = BOOKING_HOTEL_Data2;
            ViewBag.BOOKING_HOTEL2 = BOOKING_HOTEL2;

            return View();
        }
        public async Task<IActionResult> TourSearch(TourSearchViewModel searchModel)
        {
            var model = new GenericViewModel<TourGetListViewModel>();

            try
            {
                var current_user = _ManagementUser.GetCurrentUser();
                if (current_user != null)
                {
                    if (current_user.Role != "")
                    {
                        var list = current_user.Role.Split(',');
                        foreach (var item in list)
                        {
                            switch (Convert.ToInt32(item))
                            {

                                case (int)RoleType.TPDHTour:
                                case (int)RoleType.TPTour:
                                case (int)RoleType.DHTour:
                                    {

                                        if (searchModel.SalerPermission == null || searchModel.SalerPermission.Trim() == "")
                                        {
                                            searchModel.SalerPermission = current_user.UserUnderList;
                                        }
                                        else
                                        {
                                            searchModel.SalerPermission += "," + current_user.UserUnderList;

                                        }
                                    }
                                    break;
                                case (int)RoleType.Admin:
                                case (int)RoleType.KT:
                                case (int)RoleType.GDHN:
                                case (int)RoleType.GDHPQ:
                                case (int)RoleType.GD:
                                    {
                                        searchModel.SalerPermission = null;
                                    }
                                    break;
                            }
                        }

                        model = await _tourRepository.GetListTour(searchModel);
                    }
                }

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SearchTour - SetServiceTourController: " + ex);
            }

            return PartialView(model);
        }

        public async Task<IActionResult> TourDetail(int id)
        {

            try
            {
                ViewBag.ClientId = 0;

                var model = await _tourRepository.GetDetailTourByID(Convert.ToInt32(id));
                ViewBag.user = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                if (model.Price == null)
                {
                    model.Price = model.Amount;
                }
                ViewBag.AllowToFinishPayment = false;
                if (model != null && model.Id > 0)
                {
                    var max_date = model.EndDate;
                    if (max_date < DateTime.Now)
                    {
                        ViewBag.AllowToFinishPayment = true;

                    }
                }
                var order = await _orderRepository.GetOrderByID((long)model.OrderId);
                if (order != null && order.OrderId > 0)
                {
                    ViewBag.ClientId = order.ClientId ?? 0;
                }
                return View(model);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("TourDetail - SetServiceTourController: " + ex);
                return View();
            }
        }
        public async Task<IActionResult> TourServiceDetail(int id)
        {

            try
            {
                var model = await _tourRepository.GetDetailTourByID(Convert.ToInt32(id));
                var amount = _contractPayRepository.GetTotalAmountContractPayByServiceId(id.ToString(), (int)ServicesType.Tourist, (int)CONTRACT_PAY_TYPE.THU_TIEN_NCC_HOAN_TRA);
                ViewBag.amount = amount;
                if (model.Price == null)
                {
                    model.Price = model.Amount;
                }
                ViewBag.Profit = (model.Amount - model.Price) > 0 ? model.Amount - model.Price : 0;

                return PartialView(model);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("TourServiceDetail - SetServiceTourController: " + ex);
                return PartialView();
            }
        }

        public async Task<IActionResult> ListTourPackages(long id, long order)
        {

            try
            {
                var Order = await _orderRepositor.GetDetailOrderByOrderId(Convert.ToInt32(order));
                var tour_item = await _tourRepository.GetTourById(id);
                ViewBag.Tour = new Entities.Models.Tour();

                if (tour_item != null && tour_item.Id > 0)
                {
                    ViewBag.Tour = tour_item;

                }

                var model = await _tourRepository.ListTourPackagesByTourId(id);
                var ListTourGuests = await _tourRepository.GetListTourGuestsByTourId(id);
                if (model != null)
                    foreach (var item in model)
                    {
                        if (item.PackageCode == "adt_amount")
                        {
                            item.PackageName = "Người lớn";
                        }
                        if (item.PackageCode == "chd_amount")
                        {
                            item.PackageName = "Trẻ em(2 - 14 tuổi)";
                        }
                        if (item.PackageCode == "inf_amount")
                        {
                            item.PackageName = "Em bé(0 - 2 tuổi)";
                        }
                    }
                ViewBag.ListTourGuests = ListTourGuests;
                if (Order != null && Order.Count > 0)
                {
                    var tour = await _tourRepository.GetTourByOrderId(Convert.ToInt32(Order[0].OrderId));
                    ViewBag.Tourstatus = tour[0].Status;
                    ViewBag.ContactClient = _contactClientRepository.GetByContactClientId(Order[0].ContactClientId);
                }
                if (model != null)
                {
                    ViewBag.SumUnitPrice = model.Sum(s => s.Amount);
                }

                return PartialView(model);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ListTourPackages - SetServiceTourController: " + ex);
            }

            return PartialView();
        }
        public async Task<IActionResult> ListTourPackagesOrdered(long id, long order, long HotelBookingstatus)
        {

            try
            {

                var TourPackageOptional = _allCodeRepository.GetListByType(AllCodeType.TourPackageOptional);
                var Order = await _orderRepositor.GetDetailOrderByOrderId(Convert.ToInt32(order));
                var listTourPackagesOptional = await _tourPackagesOptionalRepository.GetTourPackagesOptional(id);
                var model = await _tourRepository.ListTourPackagesByTourId(id);
                var ListTourGuests = await _tourRepository.GetListTourGuestsByTourId(id);
                ViewBag.ListTourGuests = ListTourGuests;
                if (Order != null && Order.Count > 0)
                {
                    ViewBag.ContactClient = _contactClientRepository.GetByContactClientId(Order[0].ContactClientId);
                }
                if (model != null)
                {
                    ViewBag.SumUnitPrice = model.Sum(s => s.UnitPrice == null ? s.Amount : s.UnitPrice);
                }
                ViewBag.Tourstatus = HotelBookingstatus;
                ViewBag.listTourPackagesOptional = listTourPackagesOptional;
                ViewBag.TourPackageOptional = TourPackageOptional;
                ViewBag.Tour = new Entities.Models.Tour();
                var tour_item = await _tourRepository.GetTourById(id);
                if (tour_item != null && tour_item.Id > 0)
                {
                    ViewBag.Tour = tour_item;

                }

                return PartialView(model);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ListTourPackagesOrdered - SetServiceTourController: " + ex);
            }

            return PartialView();
        }
        [HttpPost]
        public async Task<IActionResult> UpdateTourStatus(long tourId, int tour_status, long OrderId, decimal amount)
        {

            var status = (int)ResponseType.SUCCESS;
            var msg = "Không thành công";
            try
            {
                if (tour_status != 0)
                {
                    var _UserId = 0;
                    _UserId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                    var order = _orderRepositor.GetByOrderId(OrderId);
                    var tour = await _tourRepository.GetDetailTourByID(Convert.ToInt32(tourId));
                    string link = "/Order/" + OrderId;
                    var current_user = _ManagementUser.GetCurrentUser();
                    int statusOder = 0;
                    if (tour_status == (int)ServiceStatus.Decline)
                    {
                        apiService.SendMessage(_UserId.ToString(), ((int)ModuleType.DICH_VU).ToString(), ((int)ActionType.TU_CHOI).ToString(), order.OrderNo, link, current_user.Role.ToString(), tour.ServiceCode);
                        msg = "Từ chối dịch vụ thành công";
                    }
                    if (tour_status == (int)ServiceStatus.OnExcution)
                    {
                        apiService.SendMessage(_UserId.ToString(), ((int)ModuleType.DICH_VU).ToString(), ((int)ActionType.NHAN_TRIEN_KHAI).ToString(), order.OrderNo, link, current_user.Role.ToString(), tour.ServiceCode);
                        msg = "Nhận đặt dịch vụ thành công";
                    }
                    if (tour_status == (int)ServiceStatus.Payment)
                    {
                        var data = _paymentRequestRepository.GetByServiceId(tourId, 5);
                        var sum = data.Where(n => ((n.Status == (int)(PAYMENT_REQUEST_STATUS.DA_CHI) || n.Status == (int)(PAYMENT_REQUEST_STATUS.CHO_CHI) || n.IsSupplierDebt == true))).Sum(n => n.Amount);
                        if (data != null && data.Count > 0 && sum >= amount)
                        {
                            apiService.SendMessage(_UserId.ToString(), ((int)ModuleType.DICH_VU).ToString(), ((int)ActionType.QUYET_TOAN).ToString(), order.OrderNo, link, current_user.Role.ToString(), tour.ServiceCode);
                            msg = "Quyết toán dịch vụ thành công";
                        }
                        else
                        {
                            return Ok(new
                            {
                                status = (int)ResponseType.ERROR,
                                msg = "Đơn hàng chưa được thanh toán đủ"
                            });
                        }
                    }
                    if (tour_status == (int)ServiceStatus.ServeCode)
                    {
                        int Type = 5;
                        var hotelBookingCode = await _hotelBookingCodeRepository.GetListlBookingCodeByHotelBookingId(tourId, Type);
                        if (hotelBookingCode != null && hotelBookingCode.Count > 0)
                        {
                            apiService.SendMessage(_UserId.ToString(), ((int)ModuleType.DICH_VU).ToString(), ((int)ActionType.TRA_CODE).ToString(), order.OrderNo, link, current_user.Role.ToString(), tour.ServiceCode);
                            msg = "Trả code dịch vụ thành công";
                        }
                        else
                        {
                            return Ok(new
                            {
                                status = (int)ResponseType.ERROR,
                                msg = "Chưa có Code dịch vụ"
                            });

                        }
                    }
                    var data2 = await _tourRepository.UpdateTourStatus(tourId, tour_status);
                    if (tour_status == (int)ServiceStatus.Decline || tour_status == (int)ServiceStatus.Payment)
                    {
                        var dataOrder2 = await _orderRepository.ProductServiceName(OrderId.ToString());
                        var count_dataOrder2 = dataOrder2.Where(s => s.Status != (int)ServiceStatus.Cancel).ToList().Count;
                        var data = dataOrder2.Where(s => s.Status == (int)ServiceStatus.Payment ? s.Status == (int)ServiceStatus.Payment : s.Status == (int)ServiceStatus.Decline).ToList();
                        if (data.Count == count_dataOrder2)
                        {
                            long UpdatedBy = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                            if (status == (int)ServiceStatus.Decline)
                            {
                                await _orderRepository.UpdateOrderStatus(OrderId, (int)OrderStatus.OPERATOR_DECLINE, UpdatedBy, 0);
                            }
                            else
                            {
                                await _orderRepository.UpdateOrderStatus(OrderId, (int)OrderStatus.WAITING_FOR_ACCOUNTANT, UpdatedBy, 0);
                            }

                        }
                    }
                    status = (int)ResponseType.SUCCESS;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateTourStatus - SetServiceController: " + ex);
                status = (int)ResponseType.ERROR;
                msg = "Đã xảy ra lỗi, vui lòng liên hệ IT";
            }
            return Ok(new
            {
                status = status,
                msg = msg
            });
        }
        //[HttpPost]
        //public async Task<IActionResult> UpdateTourPackagesUnitPrice(List<TourPackages> data, long tour_id)
        //{
        //    int status = (int)ResponseType.SUCCESS;
        //    string msg = "Cập nhật giá đặt dịch vụ không thành công";
        //    try
        //    {
        //        if (data != null)
        //        {
        //            var UserUpdate = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
        //            foreach (var item in data)
        //            {
        //                item.UpdatedBy = UserUpdate;
        //            }
        //            var x = _tourRepository.UpdateTourPackages(data);
        //            var y = _tourRepository.UpdateTourTotalPrice(tour_id);
        //            status = (int)ResponseType.SUCCESS;
        //            msg = "Cập nhật giá đặt dịch vụ thành công";
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        LogHelper.InsertLogTelegram("UpdateOrderStatus - SetServiceController: " + ex);
        //        status = (int)ResponseType.ERROR;
        //        msg = "Đã xảy ra lỗi, vui lòng liên hệ IT";
        //    }

        //    return Ok(new
        //    {
        //        status = status,
        //        msg = msg
        //    });
        //}
        [HttpPost]
        public async Task<IActionResult> TourSuggestion(string txt_search)
        {

            try
            {
                long _UserId = 0;
                if (HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) != null)
                {
                    _UserId = Convert.ToInt64(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                if (txt_search != null)
                {
                    var data = await _tourRepository.GetAllTour();
                    var listdata = data.Where(s => s.ServiceCode.ToLower().Contains(txt_search.ToLower())).ToList();
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        data = listdata,
                        selected = _UserId
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        data = new List<TourESViewModel>()
                    });
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("HotelBookingSuggestion - SetServiceController: " + ex.ToString());
                return Ok(new
                {
                    status = (int)ResponseType.SUCCESS,
                    data = new List<HotelBookingESViewModel>()
                });
            }

        }
        public async Task<IActionResult> TotalTour(TourSearchViewModel searchModel)
        {
            var model = new GenericViewModel<TourGetListViewModel>();
            try
            {


                var current_user = _ManagementUser.GetCurrentUser();
                if (current_user != null)
                {
                    if (current_user.Role != "")
                    {
                        var list = current_user.Role.Split(',');
                        foreach (var item in list)
                        {
                            switch (Convert.ToInt32(item))
                            {
                                case (int)RoleType.SaleOnl:
                                case (int)RoleType.SaleTour:
                                case (int)RoleType.SaleKd:
                                case (int)RoleType.TPDHKS:
                                case (int)RoleType.TPDHVe:
                                case (int)RoleType.TPDHTour:
                                case (int)RoleType.TPKS:
                                case (int)RoleType.TPTour:
                                case (int)RoleType.TPVe:
                                case (int)RoleType.DHPQ:
                                case (int)RoleType.DHTour:
                                case (int)RoleType.DHVe:
                                    {
                                        if (searchModel.SalerPermission == null || searchModel.SalerPermission.Trim() == "")
                                        {
                                            searchModel.SalerPermission = current_user.UserUnderList;
                                        }
                                        else
                                        {
                                            searchModel.SalerPermission += "," + current_user.UserUnderList;

                                        }
                                    }
                                    break;
                                case (int)RoleType.Admin:
                                case (int)RoleType.KT:
                                case (int)RoleType.GDHN:
                                case (int)RoleType.GDHPQ:
                                case (int)RoleType.GD:
                                    {
                                        searchModel.SalerPermission = null;
                                    }
                                    break;
                            }
                        }

                        model = await _tourRepository.GetListTour(searchModel);
                    }
                }

                if (model != null)
                    return Ok(new
                    {
                        isSuccess = true,
                        data = model.TotalRecord
                    });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("TotalTour - SetServiceController: " + ex);
            }
            return Ok(new
            {
                isSuccess = false,
                data = 0
            });

        }
        public async Task<IActionResult> SummitTourService(List<TourPackagesOptional> data)
        {
            int status = (int)ResponseType.SUCCESS;
            string msg = "Cập nhật giá đặt dịch vụ không thành công";
            try
            {
                foreach (var item in data)
                {
                    item.CreatedBy = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                    if (item.Id == 0)
                    {
                        var a = _tourPackagesOptionalRepository.InsertTourPackagesOptional(item);
                    }
                    else
                    {
                        var a = _tourPackagesOptionalRepository.UpdateTourPackagesOptional(item);
                    }

                }
                var y = _tourRepository.UpdateTourTotalPrice((long)data[0].TourId);

                #region Update Order Amount:
                var tour = await _tourRepository.GetTourById((long)data[0].TourId);
                int _UserId = 0;
                if (HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) != null)
                {
                    _UserId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                await _orderRepository.UpdateOrderDetail((long)tour.OrderId, _UserId);
                #endregion

                status = (int)ResponseType.SUCCESS;
                msg = "Cập nhật giá đặt dịch vụ thành công";
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("TotalTour - SetServiceController: " + ex);
                status = (int)ResponseType.ERROR;
                msg = "Đã xảy ra lỗi, vui lòng liên hệ IT";
            }
            return Ok(new
            {
                status = status,
                msg = msg
            });

        }

        public async Task<IActionResult> DeleteTourPackageOptional(int id, int tourId)
        {
            int status = (int)ResponseType.SUCCESS;
            string msg = "Xóa không thành công";
            try
            {
                if (id > 0)
                {
                    var delete = await _tourPackagesOptionalRepository.DeleteTourPackagesOptional(id);
                    if (delete > 0)
                    {
                        var y = _tourRepository.UpdateTourTotalPrice((long)tourId);
                        status = (int)ResponseType.SUCCESS;
                        msg = "Xóa thành công";
                    }

                }


            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("TotalTour - SetServiceController: " + ex);
                status = (int)ResponseType.ERROR;
                msg = "Đã xảy ra lỗi, vui lòng liên hệ IT";
            }
            return Ok(new
            {
                status = status,
                msg = msg
            });

        }
        [HttpPost]
        public async Task<IActionResult> TourPackageOptionalMultiple(string name)
        {

            try
            {
                long _UserId = 0;

                var TourPackageOptional = _allCodeRepository.GetListByType(AllCodeType.TourPackageOptional);
                if (HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) != null)
                {
                    _UserId = Convert.ToInt64(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                if (name == null) name = "";
                var data = TourPackageOptional.ToList();
                data = data.Where(s => s.Description.ToLower().Contains(name.ToLower())).ToList();
                if (data == null) data = new List<AllCode>();
                return Ok(new
                {
                    status = (int)ResponseType.SUCCESS,
                    data = data,
                    selected = _UserId
                });

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UserSuggestion - OrderManualController: " + ex.ToString());
                return Ok(new
                {
                    status = (int)ResponseType.SUCCESS,
                    data = new List<CustomerESViewModel>()
                });
            }

        }
        [HttpPost]
        public async Task<IActionResult> UpdateTourGuests(List<TourGuests> data)
        {

            var status = (int)ResponseType.SUCCESS;
            var msg = "Cập nhật không thành công";
            try
            {
                foreach (var item in data)
                {
                    item.UpdatedBy = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                    var update = _tourRepository.UpdateTourGuest(item);
                    if (update < 0)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.SUCCESS,
                            msg = "Không thành công"
                        });
                    }
                }
                msg = "Cập nhật danh sách đoàn thành công";
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateTourStatus - SetServiceController: " + ex);
                status = (int)ResponseType.ERROR;
                msg = "Đã xảy ra lỗi, vui lòng liên hệ IT";
            }
            return Ok(new
            {
                status = status,
                msg = msg
            });
        }
        [HttpPost]
        public async Task<IActionResult> TourExportExcel(TourSearchViewModel searchModel)
        {
            try
            {
                var current_user = _ManagementUser.GetCurrentUser();
                string _FileName = "Danh sách đặt dịch vụ tour(" + current_user.Id + ").xlsx";
                string _UploadFolder = @"Template\Export";
                string _UploadDirectory = Path.Combine(_WebHostEnvironment.WebRootPath, _UploadFolder);

                if (!Directory.Exists(_UploadDirectory))
                {
                    Directory.CreateDirectory(_UploadDirectory);
                }
                //delete all file in folder before export
                try
                {
                    System.IO.DirectoryInfo di = new DirectoryInfo(_UploadDirectory);
                    foreach (FileInfo file in di.GetFiles())
                    {
                        file.Delete();
                    }
                }
                catch
                {
                }


                string FilePath = Path.Combine(_UploadDirectory, _FileName);

                var rsPath = await _tourRepository.ExportDeposit(searchModel, FilePath);

                if (!string.IsNullOrEmpty(rsPath))
                {
                    return new JsonResult(new
                    {
                        isSuccess = true,
                        message = "Xuất dữ liệu thành công",
                        path = "/" + _UploadFolder + "/" + _FileName
                    });
                }
                else
                {
                    return new JsonResult(new
                    {
                        isSuccess = false,
                        message = "Xuất dữ liệu thất bại"
                    });
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ExportExcel - FundingController: " + ex);
                return new JsonResult(new
                {
                    isSuccess = false,
                    message = ex.Message.ToString()
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTourFundCustomer(int id,long OrderId, int type)
        {
            var sst_status = (int)ResponseType.SUCCESS;
            var msg = "Không thành công";
            try
            {

                var model = new Entities.Models.Tour();
                if (type == 1)
                {
                    var Tour_detail = await _tourRepository.GetDetailTourByID(Convert.ToInt32(id));
                    model.FundCustomerCare = (Tour_detail.Profit - (Tour_detail.Profit * 0.08)) * 0.03;
                }
                else
                {
                    model.FundCustomerCare = 0;
                }
                model.Id = id;
                model.UpdatedBy = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var data = await _tourRepository.UpdateTour(model);
                if (data != 0 && data > 0)
                {
                    _orderRepository.UpdateOrderAmountFundCustomer(OrderId);
                    sst_status = (int)ResponseType.SUCCESS;
                    msg = "Sửa thành công";
                }
                else
                {
                    sst_status = (int)ResponseType.ERROR;
                    msg = "Sửa không thành công";
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateHotelBooking - SetServiceController: " + ex);
                sst_status = (int)ResponseType.ERROR;
                msg = "Đã xảy ra lỗi, vui lòng liên hệ IT";
            }

            return Ok(new
            {
                status = sst_status,
                msg = msg
            });
        }
    }
}
