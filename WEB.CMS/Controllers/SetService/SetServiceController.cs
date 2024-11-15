using Caching.Elasticsearch;
using Entities.Models;
using Entities.ViewModels;
using Entities.ViewModels.Attachment;
using Entities.ViewModels.ElasticSearch;
using Entities.ViewModels.HotelBooking;
using Entities.ViewModels.HotelBookingCode;
using Entities.ViewModels.HotelBookingRoom;
using Entities.ViewModels.SupplierConfig;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Repositories.IRepositories;
using Repositories.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;
using GDHT.CMS.Service;
using GDHT.CMS.Service.ServiceInterface;
using WEB.CMS.Customize;
using WEB.CMS.Models;
using static Utilities.Contants.OrderConstants;
using static Utilities.DepositHistoryConstant;

namespace GDHT.CMS.Controllers.SetService
{
    [CustomAuthorize]

    public class SetServiceController : Controller
    {
        private readonly IHotelBookingRepositories _hotelBookingRepositories;
        private readonly IConfiguration _configuration;
        private readonly IOrderRepositor _orderRepositor;
        private readonly IOrderRepository _orderRepository;
        private readonly IHotelBookingRoomRepository _hotelBookingRoomRepository;
        private readonly IHotelBookingGuestRepository _hotelBookingGuestRepository;
        private readonly IHotelBookingRoomExtraPackageRepository _hotelBookingRoomExtraPackageRepository;
        private readonly IContactClientRepository _contactClientRepository;
        private readonly HotelBookingESRepository _HotelBookingESRepository;
        private readonly IAllCodeRepository _allCodeRepository;
        private readonly IHotelBookingCodeRepository _hotelBookingCodeRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPaymentRequestRepository _paymentRequestRepository;
        private readonly IEmailService _emailService;
        private readonly ITourRepository _tourRepository;
        private readonly IFlyBookingDetailRepository _flyBookingDetailRepository;
        private readonly IAttachFileRepository _attachFileRepository;
        private readonly IContractPayRepository _contractPayRepository;
        private IHotelBookingRoomRatesRepository _hotelBookingRoomRatesRepository;
        private ISupplierRepository _supplierRepository;
        private APIService apiService;
        private ManagementUser _ManagementUser;
        private readonly IWebHostEnvironment _WebHostEnvironment;
        private IOtherBookingRepository _otherBookingRepository;
        private IVinWonderBookingRepository _vinWonderBookingRepository;
        private OrderESRepository _orderESRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IHotelRepository _HotelRepository;


        public SetServiceController(IConfiguration configuration, IHotelBookingRepositories hotelBookingRepositories, IOrderRepositor orderRepositor, IOrderRepository orderRepository, IWebHostEnvironment WebHostEnvironment
            , IHotelBookingRoomRepository hotelBookingRoomRepository, IHotelBookingGuestRepository hotelBookingGuestRepository, IHotelBookingRoomExtraPackageRepository hotelBookingRoomExtraPackageRepository,
            IContactClientRepository contactClientRepository, IAllCodeRepository allCodeRepository, ITourRepository tourRepository,
            IHotelBookingCodeRepository hotelBookingCodeRepository, IUserRepository userRepository, IEmailService emailService, IFlyBookingDetailRepository flyBookingDetailRepositor, ManagementUser managementUser,
            IPaymentRequestRepository paymentRequestRepository, IAttachFileRepository attachFileRepository, IHotelBookingRoomRatesRepository hotelBookingRoomRatesRepository, ISupplierRepository supplierRepository,
            IOtherBookingRepository otherBookingRepository, IVinWonderBookingRepository vinWonderBookingRepository, IContractPayRepository contractPayRepository, IClientRepository clientRepository, IHotelRepository HotelRepository)
        {
            _paymentRequestRepository = paymentRequestRepository;
            _contractPayRepository = contractPayRepository;
            _configuration = configuration;
            _hotelBookingRepositories = hotelBookingRepositories;
            _orderRepositor = orderRepositor;
            _hotelBookingRoomRepository = hotelBookingRoomRepository;
            _hotelBookingGuestRepository = hotelBookingGuestRepository;
            _hotelBookingRoomExtraPackageRepository = hotelBookingRoomExtraPackageRepository;
            _contactClientRepository = contactClientRepository;
            _orderRepository = orderRepository;
            _HotelBookingESRepository = new HotelBookingESRepository(_configuration["DataBaseConfig:Elastic:Host"]);
            _allCodeRepository = allCodeRepository;
            _hotelBookingCodeRepository = hotelBookingCodeRepository;
            _userRepository = userRepository;
            _emailService = emailService;
            _tourRepository = tourRepository;
            _flyBookingDetailRepository = flyBookingDetailRepositor;
            _attachFileRepository = attachFileRepository;
            _hotelBookingRoomRatesRepository = hotelBookingRoomRatesRepository;
            apiService = new APIService(configuration, userRepository);
            _ManagementUser = managementUser;
            _supplierRepository = supplierRepository;
            _WebHostEnvironment = WebHostEnvironment;
            _vinWonderBookingRepository = vinWonderBookingRepository;
            _otherBookingRepository = otherBookingRepository;
            _orderESRepository = new OrderESRepository(_configuration["DataBaseConfig:Elastic:Host"]);
            _clientRepository = clientRepository;
            _HotelRepository = HotelRepository;
        }
        public async Task<IActionResult> SetServiceHotel()
        {

            var BOOKING_HOTEL = _allCodeRepository.GetListByType(AllCodeType.BOOKING_HOTEL_ROOM_STATUS);
            ViewBag.BOOKING_HOTEL = BOOKING_HOTEL;
            var searchModel = new SearchHotelBookingViewModel();
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
                            case (int)RoleType.TPKS:
                            case (int)RoleType.DHKS:
                            case (int)RoleType.DHPQ:
                                {
                                    searchModel.SalerPermission += current_user.UserUnderList;
                                }
                                break;
                            case (int)RoleType.Admin:
                            case (int)RoleType.KT:
                            case (int)RoleType.GDHN:
                            case (int)RoleType.GDHPQ:
                            case (int)RoleType.GD:
                            case (int)RoleType.TPDHKS:
                            case (int)RoleType.KeToanTruong:
                            case (int)RoleType.PhoTPKeToan:
                                {
                                    searchModel.SalerPermission = null;
                                }
                                break;
                        }
                    }

                    var model = await _hotelBookingRepositories.TotalHotelBooking(searchModel);
                    ViewBag.TotalHotelBooking = model;
                }
            }

            return View();
        }
        public async Task<IActionResult> Search(SearchHotelBookingViewModel searchModel, int currentPage = 1, int pageSize = 20)
        {
            var model = new GenericViewModel<SearchHotelBookingModel>();

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
                                case (int)RoleType.TPKS:
                                case (int)RoleType.TPDHKS:
                                case (int)RoleType.DHPQ:
                                    {
                                        searchModel.SalerPermission += current_user.UserUnderList;
                                    }
                                    break;
                                case (int)RoleType.Admin:
                                case (int)RoleType.KT:
                                case (int)RoleType.GDHN:
                                case (int)RoleType.GDHPQ:
                                case (int)RoleType.GD:
                                case (int)RoleType.KeToanTruong:
                                case (int)RoleType.PhoTPKeToan:
                                    {
                                        searchModel.SalerPermission = null;
                                    }
                                    break;
                            }
                        }

                        model = await _hotelBookingRepositories.GetPagingList(searchModel, searchModel.PageIndex, searchModel.PageSize);
                    }
                }

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Search - SetServiceController: " + ex);
            }

            return PartialView(model);
        }
   
        public async Task<IActionResult> VerifyHotelServiceDetai(int id)
        {
            try
            {
                ViewBag.ClientId = 0;
                if (id != 0)
                {
                    var model = await _hotelBookingRepositories.GetHotelBookingById(id);
                    ViewBag.user = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                    ViewBag.AllowToFinishPayment = false;
                    if (model != null && model.Count > 0)
                    {
                        var max_date = model[0].DepartureDate;
                        if (max_date < DateTime.Now)
                        {
                            ViewBag.AllowToFinishPayment = true;

                        }
                        var order = await _orderRepository.GetOrderByID(model[0].OrderId);
                        if (order != null && order.OrderId > 0 && order.ClientId != null && order.ClientId > 0)
                        {
                            ViewBag.ClientId = (long)order.ClientId;
                        }
                    }

                    return View(model[0]);
                }
                return View();
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("VerifyHotelServiceDetai - SetServiceController: " + ex);
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> HotelServiceDetai(long HotelBookingID)
        {
            try
            {
                if (HotelBookingID != 0)
                {
                    var model = await _hotelBookingRepositories.GetHotelBookingById(HotelBookingID);
                    var amount = _contractPayRepository.GetTotalAmountContractPayByServiceId(HotelBookingID.ToString(), (int)ServicesType.VINHotelRent, (int)CONTRACT_PAY_TYPE.THU_TIEN_NCC_HOAN_TRA);
                    model[0].TotalAmountPaymentRequest = model.Sum(s => s.TotalAmountPaymentRequest);
                    ViewBag.amount = amount;
                    return PartialView(model[0]);
                }
                return View();
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("VerifyHotelServiceDetai - SetServiceController: " + ex);
            }
            return PartialView();
        }
        [HttpPost]
        public async Task<IActionResult> ListOrder(long id)
        {
            var data = new List<OrderDetailViewModel>();
            ViewBag.Client = new Client();
            try
            {
                data = await _orderRepositor.GetDetailOrderByOrderId(Convert.ToInt32(id));
                {
                    ViewBag.Client = await _clientRepository.GetClientDetailByClientId(data[0].ClientId);

                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ListOrder - SetServiceController: " + ex);
            }

            return PartialView(data);
        }
        [HttpPost]
        public async Task<IActionResult> ListHotelServicesOrder(long HotelBookingID, long status, long ContactClientId)
        {
            try
            {
                ViewBag.room_list = new List<HotelBookingRooms>();
                ViewBag.package_list = new List<HotelBookingRoomRates>();
                ViewBag.num_people = 0;
                ViewBag.HotelBooking = new HotelBooking()
                {
                    Note = ""
                };
                if (HotelBookingID > 0)
                {
                    var booking = await _hotelBookingRepositories.GetHotelBookingByID(HotelBookingID);
                    if (booking != null && booking.Id > 0)
                    {
                        ViewBag.HotelBooking = booking;
                        var rooms = await _hotelBookingRoomRepository.GetByHotelBookingID(HotelBookingID);
                        var packages = await _hotelBookingRoomRatesRepository.GetByHotelBookingID(HotelBookingID);
                        ViewBag.room_list = rooms == null || rooms.Count < 1 ? new List<HotelBookingRooms>() : rooms;
                        List<HotelBookingRoomRates> package_daterange = new List<HotelBookingRoomRates>();
                        int number_of_people = (int)rooms.Sum(x => x.NumberOfAdult) + (int)rooms.Sum(x => x.NumberOfChild) + (int)rooms.Sum(x => x.NumberOfInfant);
                        if (packages != null && packages.Count > 0)
                        {
                            foreach (var p in packages)
                            {
                                if (p.StartDate == null && p.EndDate == null)
                                {
                                    if (packages.Count < 1 || !package_daterange.Any(x => x.HotelBookingRoomId == p.HotelBookingRoomId && x.RatePlanId == p.RatePlanId))
                                    {
                                        var add_value = p;
                                        add_value.StartDate = add_value.StayDate;
                                        add_value.EndDate = add_value.StayDate.AddDays(1);
                                        p.StayDate = (DateTime)add_value.StartDate;
                                        p.SalePrice = p.TotalAmount;
                                        p.OperatorPrice = p.Price;
                                        package_daterange.Add(add_value);
                                    }
                                    else
                                    {
                                        var p_d = package_daterange.FirstOrDefault(x => x.HotelBookingRoomId == p.HotelBookingRoomId && x.RatePlanId == p.RatePlanId && ((DateTime)x.EndDate).Date == p.StayDate.Date);
                                        if (p_d != null)
                                        {

                                            if (p_d.StartDate == null || p_d.StartDate > p.StayDate)
                                                p_d.StartDate = p.StayDate;
                                            p_d.EndDate = p.StayDate.AddDays(1);
                                        }
                                        else
                                        {
                                            var add_value = p;
                                            add_value.StartDate = add_value.StayDate;
                                            add_value.EndDate = add_value.StayDate.AddDays(1);
                                            p.StayDate = (DateTime)add_value.StartDate;
                                            p.SalePrice = p.TotalAmount;
                                            p.OperatorPrice = p.Price;
                                            package_daterange.Add(add_value);
                                        }
                                    }
                                }
                                else
                                {
                                    package_daterange.Add(p);
                                }
                            }
                        }
                        ViewBag.package_list = package_daterange;
                        ViewBag.num_people = number_of_people;
                    }

                }
                ViewBag.hotelBookingGuest = await _hotelBookingGuestRepository.GetHotelGuestByHotelBookingId(HotelBookingID);
                var extra_package = await _hotelBookingRoomExtraPackageRepository.GetByBookingID(HotelBookingID);
                ViewBag.ExtraPackages = extra_package != null ? extra_package : new List<HotelBookingRoomExtraPackages>();

                ViewBag.ContactClient = _contactClientRepository.GetByContactClientId(ContactClientId);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ListOrder - SetServiceController: " + ex);
            }

            return PartialView();
        }

        [HttpPost]
        public async Task<IActionResult> ListHotelServicesbooked(long HotelBookingID, long status, long ContactClientId, long HotelBookingstatus)
        {
            try
            {
                ViewBag.room_list = new List<HotelBookingRoomOptional>();
                ViewBag.package_list = new List<HotelBookingRoomRates>();
                ViewBag.num_people = 0;
                ViewBag.HotelBookingStatus = 0;
                ViewBag.HotelBooking = new HotelBooking()
                {
                    Note = ""
                };
                ViewBag.hotelBookingGuest = new List<HotelGuestViewModel>();

                if (HotelBookingID > 0)
                {
                    var booking = await _hotelBookingRepositories.GetHotelBookingByID(HotelBookingID);
                    if (booking != null && booking.Id > 0)
                    {
                        ViewBag.HotelBooking = booking;
                        ViewBag.HotelBookingStatus = (short)booking.Status;
                        var packages = await _hotelBookingRoomRepository.GetHotelBookingRoomRatesOptionalByBookingId(HotelBookingID);
                        var rooms = await _hotelBookingRepositories.GetHotelBookingOptionalListByHotelBookingId(HotelBookingID);
                        ViewBag.room_list = (rooms == null || rooms.Count < 1) ? new List<HotelBookingsRoomOptionalViewModel>() : rooms;
                        List<HotelBookingRoomRatesOptionalViewModel> package_daterange = new List<HotelBookingRoomRatesOptionalViewModel>();
                        if (packages != null && packages.Count > 0)
                        {
                            package_daterange.AddRange(packages);
                        }
                        ViewBag.package_list = package_daterange;
                    }

                }
                ViewBag.hotelBookingGuest = await _hotelBookingGuestRepository.GetHotelGuestByHotelBookingId(HotelBookingID);
                var extra_package = await _hotelBookingRoomExtraPackageRepository.Gethotelbookingroomextrapackagebyhotelbookingid(HotelBookingID);
                ViewBag.ExtraPackages = extra_package != null ? extra_package : new List<HotelBookingRoomExtraPackagesViewModel>();

                ViewBag.ContactClient = _contactClientRepository.GetByContactClientId(ContactClientId);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ListHotelServicesbooked - SetServiceController: " + ex);
            }

            return PartialView();
        }

        [HttpPost]
        public async Task<IActionResult> ListHotelBookingCode(long HotelBookingstatus, long HotelBookingID, int type)
        {
            var data = new List<HotelBookingCodeModel>();
            int attachment_type = type;
            try
            {
                switch (type)
                {
                    case (int)ServiceType.BOOK_HOTEL_ROOM:
                    case (int)ServiceType.BOOK_HOTEL_ROOM_VIN:
                        {
                            attachment_type = (int)AttachmentType.ServiceCode_HotelRent;

                        }
                        break;
                    case (int)ServiceType.PRODUCT_FLY_TICKET:
                        {
                            attachment_type = (int)AttachmentType.ServiceCode_FlyingTicket;

                        }
                        break;
                    case (int)ServiceType.Other:
                        {
                            attachment_type = (int)AttachmentType.ServiceCode_Others;

                        }
                        break;
                    case (int)ServiceType.Tour:
                        {
                            attachment_type = (int)AttachmentType.ServiceCode_Tourist;

                        }
                        break;
                    case (int)ServiceType.VinWonder:
                        {
                            attachment_type = (int)AttachmentType.ServiceCode_VinWonder;

                        }
                        break;
                }
                var booking = await _hotelBookingRepositories.GetHotelBookingByID(HotelBookingID);
                ViewBag.IsVinHotel = false;
                if (booking != null && booking.Id > 0)
                {
                    var hotel = _HotelRepository.GetHotelByHotelID(booking.PropertyId);
                    ViewBag.IsVinHotel = hotel.IsVinHotel == null ? false : hotel.IsVinHotel;
                }
                data = await _hotelBookingCodeRepository.GetListlBookingCodeByHotelBookingId(HotelBookingID, type);
                if (data != null && data.Count > 0)
                {
                    foreach (var item in data)
                    {

                        var models = await _attachFileRepository.GetListByType(item.Id, attachment_type);
                        item.attachFiles = models;
                    }
                }

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ListHotelBookingCode - SetServiceController: " + ex);
            }
            ViewBag.HotelBookingID = HotelBookingID;
            ViewBag.HotelBookingstatus = HotelBookingstatus;
            ViewBag.Type = type;
            ViewBag.AttachmentType = attachment_type;
            return PartialView(data);
        }


        /// <summary>
        /// HotelBookingID = serviceId 
        /// </summary>
        /// <param name="HotelBookingID"></param>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        [HttpPost]

        public async Task<IActionResult> ListHotelBookingpayment(long HotelBookingID, int serviceType, int supplierId, decimal amount,
            long orderId, int HotelBookingstatus = 0, string serviceCode = "")
        {
            if (HotelBookingstatus != 0)
                TempData["status"] = HotelBookingstatus;
            if (!string.IsNullOrEmpty(serviceCode))
                TempData["serviceCode"] = serviceCode;
            TempData.Keep("status");
            TempData.Keep("serviceCode");
            if (TempData.ContainsKey("status"))
                HotelBookingstatus = (int)TempData["status"];
            if (TempData.ContainsKey("serviceCode"))
                serviceCode = (string)TempData["serviceCode"];
            ViewBag.serviceId = HotelBookingID;
            ViewBag.serviceType = serviceType;
            ViewBag.supplierId = supplierId;
            ViewBag.amount = amount;
            ViewBag.status = HotelBookingstatus;
            ViewBag.orderId = orderId;
            ViewBag.serviceCode = serviceCode;
            ViewBag.isAdmin = false;
            var current_user = _ManagementUser.GetCurrentUser();
            if (current_user != null && !string.IsNullOrEmpty(current_user.Role))
            {
                var listRole = Array.ConvertAll(current_user.Role.Split(','), int.Parse);
                bool isAdmin = _userRepository.IsAdmin(current_user.Id);
                ViewBag.isAdmin = isAdmin;
            }
            var data = _paymentRequestRepository.GetByServiceId(HotelBookingID, serviceType);
            var serviceTypeOptional = 0;
            switch (serviceType)
            {
                case (int)ServiceType.Tour:
                    serviceTypeOptional = (int)SubServiceType.Tour; break;
                case (int)ServiceType.PRODUCT_FLY_TICKET:
                    serviceTypeOptional = (int)SubServiceType.PRODUCT_FLY_TICKET; break;
                case (int)ServiceType.BOOK_HOTEL_ROOM_VIN:
                    serviceTypeOptional = (int)SubServiceType.BOOK_HOTEL_ROOM_VIN; break;
                case (int)ServiceType.Other:
                    serviceTypeOptional = (int)SubServiceType.Other; break;
                default:
                    serviceTypeOptional = 0;
                    break;
            }
            var listContractPay = _contractPayRepository.GetContractPayBySupplierId(orderId, HotelBookingID, serviceType);
            var listContractPaySub = _contractPayRepository.GetContractPayBySupplierId(orderId, HotelBookingID, serviceTypeOptional);
            foreach (var item in listContractPaySub)
            {
                listContractPay.Add(item);
            }
            ViewBag.listContractPay = listContractPay;
            return PartialView(data);
        }

        [HttpPost]

        public async Task<IActionResult> MarketingPaymentRequest(long serviceId, int serviceType, int supplierId, long orderId,
            int serviceStatus = 0, string serviceCode = "")
        {
            if (serviceStatus != 0)
                TempData["status"] = serviceStatus;
            if (serviceType != 0)
                TempData["serviceType"] = serviceType;
            if (orderId != 0)
                TempData["orderId"] = orderId;
            if (supplierId != 0)
                TempData["supplierId"] = supplierId;
            if (serviceId != 0)
                TempData["serviceId"] = serviceId;
            if (!string.IsNullOrEmpty(serviceCode))
                TempData["serviceCode"] = serviceCode;
            if (TempData.ContainsKey("status"))
                serviceStatus = (int)TempData["status"];
            if (TempData.ContainsKey("serviceCode"))
                serviceCode = (string)TempData["serviceCode"];
            if (TempData.ContainsKey("serviceId"))
                serviceId = Convert.ToInt64(TempData["serviceId"]);
            if (TempData.ContainsKey("serviceType"))
                serviceType = (int)TempData["serviceType"];
            if (TempData.ContainsKey("supplierId"))
                supplierId = (int)TempData["supplierId"];
            if (TempData.ContainsKey("orderId"))
                orderId = Convert.ToInt64(TempData["orderId"]);
            TempData.Keep("status");
            TempData.Keep("serviceType");
            TempData.Keep("orderId");
            TempData.Keep("supplierId");
            TempData.Keep("serviceId");
            TempData.Keep("serviceCode");
            ViewBag.serviceId = serviceId;
            ViewBag.serviceType = serviceType;
            ViewBag.supplierId = supplierId;
            ViewBag.status = serviceStatus;
            ViewBag.orderId = orderId;
            ViewBag.serviceCode = serviceCode;
            ViewBag.isAdmin = false;
            var current_user = _ManagementUser.GetCurrentUser();
            if (current_user != null && !string.IsNullOrEmpty(current_user.Role))
            {
                var listRole = Array.ConvertAll(current_user.Role.Split(','), int.Parse);
                bool isAdmin = _userRepository.IsAdmin(current_user.Id);
                ViewBag.isAdmin = isAdmin;
            }
            var data = _paymentRequestRepository.GetByServiceId(serviceId, serviceType, (int)PAYMENT_VOUCHER_TYPE.CHI_PHI_MARKETING);
            var serviceTypeOptional = 0;
            switch (serviceType)
            {
                case (int)ServiceType.Tour:
                    serviceTypeOptional = (int)SubServiceType.Tour; break;
                case (int)ServiceType.PRODUCT_FLY_TICKET:
                    serviceTypeOptional = (int)SubServiceType.PRODUCT_FLY_TICKET; break;
                case (int)ServiceType.BOOK_HOTEL_ROOM_VIN:
                    serviceTypeOptional = (int)SubServiceType.BOOK_HOTEL_ROOM_VIN; break;
                case (int)ServiceType.Other:
                    serviceTypeOptional = (int)SubServiceType.Other; break;
                default:
                    serviceTypeOptional = 0;
                    break;
            }

            return PartialView(data);
        }

        [HttpPost]

        public async Task<IActionResult> ListPaymentRequestByClient(long clientId, long bookingId, int serviceType, decimal amount,
            long orderId, int bookingstatus = 0, string serviceCode = "")
        {
            if (bookingstatus != 0)
                TempData["status"] = bookingstatus;
            if (!string.IsNullOrEmpty(serviceCode))
                TempData["serviceCode"] = serviceCode;
            if (bookingId != 0)
                TempData["serviceId"] = bookingId;
            if (amount != 0)
                TempData["amount"] = amount;
            if (serviceType != 0)
                TempData["serviceType"] = serviceType;
            if (TempData.ContainsKey("status"))
                bookingstatus = (int)TempData["status"];
            if (TempData.ContainsKey("serviceId"))
                bookingId = (long)TempData["serviceId"];
            if (TempData.ContainsKey("serviceType"))
                serviceType = (int)TempData["serviceType"];
            if (TempData.ContainsKey("amount"))
                amount = (decimal)TempData["amount"];
            if (TempData.ContainsKey("serviceCode"))
                serviceCode = (string)TempData["serviceCode"];
            TempData.Keep("status");
            TempData.Keep("serviceCode");
            TempData.Keep("serviceType");
            TempData.Keep("serviceId");
            TempData.Keep("amount");
            ViewBag.clientId = clientId;
            ViewBag.serviceId = bookingId;
            ViewBag.amount = amount;
            ViewBag.status = bookingstatus;
            ViewBag.serviceCode = serviceCode;
            ViewBag.orderId = orderId;
            ViewBag.isAdmin = false;
            ViewBag.serviceType = serviceType;
            var current_user = _ManagementUser.GetCurrentUser();
            if (current_user != null && !string.IsNullOrEmpty(current_user.Role))
            {
                var listRole = Array.ConvertAll(current_user.Role.Split(','), int.Parse);
                bool isAdmin = _userRepository.IsAdmin(current_user.Id);
                ViewBag.isAdmin = isAdmin;
            }
            var data = _paymentRequestRepository.GetRequestByClientId(clientId, orderId);
            return PartialView(data);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrderStatus(long id, int status, long OrderId, decimal amount)
        {
            var sst_status = (int)ResponseType.SUCCESS;
            var smg = "Không thành công";
            try
            {
                if (status != 0)
                {
                    var _UserId = 0;
                    var Hotel = await _hotelBookingRepositories.GetDetailHotelBookingByID(Convert.ToInt32(id));
                    _UserId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                    var order = _orderRepositor.GetByOrderId(OrderId);
                    string link = "/Order/" + OrderId;
                    var current_user = _ManagementUser.GetCurrentUser();
                    int statusOder = 0;
                    if (status == (int)ServiceStatus.Decline)
                    {


                        apiService.SendMessage(_UserId.ToString(), ((int)ModuleType.DICH_VU).ToString(), ((int)ActionType.QUYET_TOAN).ToString(), order.OrderNo, link, current_user.Role.ToString(), Hotel[0].ServiceCode);
                        smg = "Từ chối dịch vụ thành công";
                    }
                    if (status == (int)ServiceStatus.OnExcution)
                    {
                        //apiService.SendMessage(_UserId.ToString(), ((int)ModuleType.DICH_VU).ToString(), ((int)ActionType.NHAN_TRIEN_KHAI).ToString(), order.OrderNo, link, current_user.Role.ToString(), Hotel[0].ServiceCode);
                        smg = "Nhận đặt dịch vụ thành công";
                    }
                    if (status == (int)ServiceStatus.Payment)
                    {
                        var data = _paymentRequestRepository.GetByServiceId(id, (int)(ServicesType.VINHotelRent));

                        var sum = data.Where(n => (n.Status == (int)(PAYMENT_REQUEST_STATUS.DA_CHI) || n.Status == (int)(PAYMENT_REQUEST_STATUS.CHO_CHI) || n.IsSupplierDebt == true)).Sum(n => n.Amount);

                        if (data != null && data.Count > 0 && sum >= amount)
                        {
                            apiService.SendMessage(_UserId.ToString(), ((int)ModuleType.DICH_VU).ToString(), ((int)ActionType.QUYET_TOAN).ToString(), order.OrderNo, link, current_user.Role.ToString(), Hotel[0].ServiceCode);
                            smg = "Quyết toán dịch vụ thành công";
                        }
                        else
                        {
                            return Ok(new
                            {
                                sst_status = (int)ResponseType.ERROR,
                                smg = "Đơn hàng chưa được thanh toán đủ"
                            });
                        }

                    }
                    if (status == (int)ServiceStatus.ServeCode)
                    {
                        int Type = 1;
                        var hotelBookingCode = await _hotelBookingCodeRepository.GetListlBookingCodeByHotelBookingId(id, Type);
                        if (hotelBookingCode != null && hotelBookingCode.Count > 0)
                        {
                            apiService.SendMessage(_UserId.ToString(), ((int)ModuleType.DICH_VU).ToString(), ((int)ActionType.TRA_CODE).ToString(), order.OrderNo, link, current_user.Role.ToString(), Hotel[0].ServiceCode);
                            smg = "Trả code dịch vụ thành công";
                        }
                        else
                        {
                            return Ok(new
                            {
                                sst_status = (int)ResponseType.ERROR,
                                smg = "Chưa có Code dịch vụ"
                            });

                        }

                    }

                    var data2 = await _hotelBookingRepositories.UpdateHotelBookingStatus(id, status);
                    if (status == (int)ServiceStatus.Decline || status == (int)ServiceStatus.Payment)
                    {
                        var dataOrder2 = await _orderRepository.ProductServiceName(OrderId.ToString());
                        var count_dataOrder2 = dataOrder2.Where(s => s.Status != (int)ServiceStatus.Cancel).ToList().Count;
                        var data = dataOrder2.Where(s => s.Status == (int)ServiceStatus.Payment ? s.Status == (int)ServiceStatus.Payment : s.Status == (int)ServiceStatus.Decline).ToList();
                        if (data.Count == count_dataOrder2)
                        {
                            long UpdatedBy = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                            long UserVerify = 0;
                            if (status == (int)ServiceStatus.Decline)
                            {
                                await _orderRepository.UpdateOrderStatus(OrderId, (int)OrderStatus.OPERATOR_DECLINE, UpdatedBy, UserVerify);
                            }
                            else
                            {
                                await _orderRepository.UpdateOrderStatus(OrderId, (int)OrderStatus.WAITING_FOR_ACCOUNTANT, UpdatedBy, UserVerify);
                            }

                        }
                    }
                    sst_status = (int)ResponseType.SUCCESS;

                }

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateOrderStatus - SetServiceController: " + ex);
                sst_status = (int)ResponseType.ERROR;
                smg = "Đã xảy ra lỗi, vui lòng liên hệ IT";
            }

            return Ok(new
            {
                sst_status = sst_status,
                smg = smg
            });
        }
        [HttpPost]
        public async Task<IActionResult> UpdateHotelBookingUnitPrice(HotelBookingUnitPriceChangeSummitModel data, long hotel_booking_id)
        {
            try
            {
                //-- Validate
                if (hotel_booking_id <= 0 || data.rooms == null || data.rooms.Count <= 0)
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = "Dữ liệu gửi lên không chính xác, vui lòng kiểm tra lại"
                    });
                }
                //bool any_room_zero = data.rooms.Any(x => x.rates.Any(x => x.operator_price <= 0 || x.operator_amount < 0));
                //bool any_extra_zero = false;
                //if (data.extra_packages != null && data.extra_packages.Count > 0)
                //{
                //    any_extra_zero = data.extra_packages.Any(x => x.operator_price < 0);
                //}
                //if (any_room_zero || any_extra_zero)
                //{
                //    return Ok(new
                //    {
                //        status = (int)ResponseType.FAILED,
                //        msg = "Dữ liệu gửi lên không chính xác, vui lòng kiểm tra lại"
                //    });
                //}
                //-- Update UnitPrice
                int _UserId = 0;
                if (HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) != null)
                {
                    _UserId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                var success = await _hotelBookingRoomRepository.UpdateHotelBookingUnitPrice(data, hotel_booking_id, _UserId);

                if (success > 0)
                {

                    #region Update Order Amount:
                    var hotel_booking = await _hotelBookingRepositories.GetHotelBookingByID(hotel_booking_id);
                    await _orderRepository.UpdateOrderDetail(hotel_booking.OrderId, _UserId);
                    #endregion

                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        msg = "Cập nhật thành công"
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = "Cập nhật thất bại, vui lòng liên hệ IT"
                    });
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateHotelBookingUnitPrice - SetServiceController: " + ex);
                return Ok(new
                {
                    status = (int)ResponseType.ERROR,
                    msg = "Lỗi trong quá trình xử lý, vui lòng liên hệ IT"
                });
            }
        }
        [HttpPost]
        public async Task<IActionResult> HotelBookingSuggestion(string txt_search)
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
                    var data = await _HotelBookingESRepository.GetListProduct(txt_search.Trim());
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        data = data,
                        selected = _UserId
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        data = new List<HotelBookingESViewModel>()
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
        [HttpPost]
        public async Task<IActionResult> AddPopupCode(int id, int hotelbookingid, int type)
        {
            HotelBookingCode data = new HotelBookingCode();
            int attachment_type = type;
            int code_id = id;
            try
            {
                switch (type)
                {
                    case (int)ServiceType.BOOK_HOTEL_ROOM:
                    case (int)ServiceType.BOOK_HOTEL_ROOM_VIN:
                        {
                            attachment_type = (int)AttachmentType.ServiceCode_HotelRent;

                        }
                        break;
                    case (int)ServiceType.PRODUCT_FLY_TICKET:
                        {
                            attachment_type = (int)AttachmentType.ServiceCode_FlyingTicket;

                        }
                        break;
                    case (int)ServiceType.Other:
                        {
                            attachment_type = (int)AttachmentType.ServiceCode_Others;

                        }
                        break;
                    case (int)ServiceType.Tour:
                        {
                            attachment_type = (int)AttachmentType.ServiceCode_Tourist;

                        }
                        break;
                    case (int)ServiceType.VinWonder:
                        {
                            attachment_type = (int)AttachmentType.ServiceCode_VinWonder;

                        }
                        break;
                }

                if (id != 0)
                {
                    code_id = id;
                    data = await _hotelBookingCodeRepository.GetDetailBookingCodeById(id);
                    if (data == null) data = new HotelBookingCode();
                }
                data.Type = type;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("AddPopupCode - SetServiceController: " + ex);
            }
            ViewBag.HotelBookingId = hotelbookingid;
            ViewBag.AttachmentType = attachment_type;
            ViewBag.CodeID = code_id;

            return PartialView(data);
        }
        [HttpPost]
        public async Task<IActionResult> SetUpHotelBookingCode(HotelBookingCodeViewModel model)
        {
            var sst_status = (int)ResponseType.SUCCESS;
            var smg = "Không thành công";
            var type = model.Type;
            var id = 0;
            try
            {
                int user_id = 0;
                if (HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) != null)
                {
                    user_id = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                if (model.Id == 0)
                {
                    model.IsDelete = 0;
                    var file = await _attachFileRepository.GetListByType(model.HotelBookingId, type);
                    if (file != null && file.Count > 0)
                    {
                        model.AttactFile = file[0].Path;
                    }
                    model.CreatedBy = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                    var data = await _hotelBookingCodeRepository.InsertHotelBookingCode(model);
                    if (data != 0 && data > 0)
                    {

                        sst_status = (int)ResponseType.SUCCESS;
                        smg = "Thêm mới thành công";
                        id = data;
                    }
                    else
                    {
                        sst_status = (int)ResponseType.ERROR;
                        smg = "Thêm mới không thành công";
                    }

                }

                if (model.Id != 0)
                {
                    model.IsDelete = 0;
                    model.UpdatedBy = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                    var data = await _hotelBookingCodeRepository.UpdateHotelBookingCode(model);
                    if (data != 0 && data > 0)
                    {

                        sst_status = (int)ResponseType.SUCCESS;
                        smg = "Sửa thành công ";
                        id = data;
                    }
                    else
                    {
                        sst_status = (int)ResponseType.ERROR;
                        smg = "Sửa không thành công";
                    }
                }

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SetUpHotelBookingCode - SetServiceController: " + ex);
                sst_status = (int)ResponseType.ERROR;
                smg = "Đã xảy ra lỗi, vui lòng liên hệ IT";
            }

            return Ok(new
            {
                sst_status = sst_status,
                smg = smg,
                type = type,
                id = id
            });
        }
        [HttpPost]
        public async Task<IActionResult> DeleteHotelBookingCode(long id, int type)
        {
            var sst_status = (int)ResponseType.SUCCESS;
            var smg = "Không thành công";
            try
            {
                if (id != 0)
                {
                    var hotelBookingCode = await _hotelBookingCodeRepository.GetDetailBookingCodeById(id);
                    var model = new HotelBookingCodeViewModel();
                    model.Id = hotelBookingCode.Id;
                    model.BookingCode = hotelBookingCode.BookingCode;
                    model.HotelBookingId = Convert.ToInt32(hotelBookingCode.ServiceId);
                    model.Description = hotelBookingCode.Description;
                    model.AttactFile = hotelBookingCode.AttactFile;
                    model.Note = hotelBookingCode.Note;
                    model.IsDelete = 1;
                    model.UpdatedBy = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                    var data = await _hotelBookingCodeRepository.UpdateHotelBookingCode(model);
                    if (data != 0 && data > 0)
                    {
                        sst_status = (int)ResponseType.SUCCESS;
                        smg = "Xóa thành công";
                    }
                    else
                    {
                        sst_status = (int)ResponseType.ERROR;
                        smg = "Xóa không thành công";
                    }

                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("DeleteHotelBookingCode - SetServiceController: " + ex);
                sst_status = (int)ResponseType.ERROR;
                smg = "Đã xảy ra lỗi, vui lòng liên hệ IT";
            }

            return Ok(new
            {
                sst_status = sst_status,
                smg = smg,
                type = type
            });
        }
        public async Task<IActionResult> SendEmail(long id, long Orderid, int type, string group_booking_id = "")
        {

            try
            {
                var dataOrder = await _orderRepositor.GetDetailOrderByOrderId(Convert.ToInt32(Orderid));
                if (dataOrder != null && dataOrder[0].SalerId != 0)
                {
                    var user = await _userRepository.GetDetailUser(dataOrder[0].SalerId);
                    ViewBag.user = user.Entity;
                }
                if (dataOrder != null && dataOrder[0].ContactClientId != 0)
                {
                    var ContactClient = _contactClientRepository.GetByContactClientId(dataOrder[0].ContactClientId);
                    ViewBag.ContactClientEmail = ContactClient.Email;
                }
                ViewBag.Order = dataOrder[0];
                ViewBag.ServiceType = type;
                var model = new SendEmailViewModel();
                model = null;
                switch (type)
                {
                    case (int)ServiceType.BOOK_HOTEL_ROOM:
                    case (int)ServiceType.BOOK_HOTEL_ROOM_VIN:
                        {
                            ViewBag.EmailBody = await _emailService.GetTemplateinsertUser(model, id, "", "", true);
                        }
                        break;
                    case (int)ServiceType.PRODUCT_FLY_TICKET:
                        {
                            ViewBag.EmailBody = await _emailService.GetFlyBookingTemplateBody(model, group_booking_id, "", "", true);
                        }
                        break;
                    case (int)ServiceType.Tour:
                        {

                            ViewBag.EmailBody = await _emailService.TourTemplateBody(model, id, "", "", true);

                        }
                        break;
                    case (int)EmailType.Supplier:
                        {
                            int SupplierId = 2;
                            ViewBag.EmailBody = await _emailService.GetTemplateSupplier(model, id, SupplierId, model.type, "", "", true);

                        }
                        break;
                    case (int)EmailType.DON_HANG_Fly:
                        {
                            ViewBag.EmailBody = await _emailService.GetOrderFlyTemplateBody(Orderid, "", "", true);

                        }
                        break;

                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SendEmail - SetServiceController: " + ex);
            }
            ViewBag.Orderid = Orderid;
            ViewBag.ServiceId = id;
            ViewBag.GroupBookingId = group_booking_id;

            return PartialView();
        }
        public async Task<IActionResult> SendEmailSupplier(long id, long Orderid, int type, long SupplierId, long ServiceType, string group_booking_id = "")
        {

            try
            {
                var dataOrder = await _orderRepositor.GetDetailOrderByOrderId(Convert.ToInt32(Orderid));
                if (dataOrder != null && dataOrder[0].SalerId != 0)
                {
                    var user = await _userRepository.GetDetailUser(dataOrder[0].SalerId);
                    ViewBag.user = user.Entity;
                }
                if (dataOrder != null && dataOrder[0].ContactClientId != 0)
                {
                    var ContactClient = _contactClientRepository.GetByContactClientId(dataOrder[0].ContactClientId);
                    ViewBag.ContactClientEmail = ContactClient.Email;
                }
                ViewBag.Order = dataOrder[0];
                ViewBag.type = type;
                ViewBag.ServiceType = ServiceType;
                var model = new SendEmailViewModel();
                model = null;

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SendEmailSupplier - SetServiceController: " + ex);
            }
            ViewBag.Orderid = Orderid;
            ViewBag.ServiceId = id;
            ViewBag.GroupBookingId = group_booking_id;

            return PartialView();
        }
        public async Task<IActionResult> TemplateSupplier(long id, long Orderid, int type, long SupplierId, long ServiceType, string group_booking_id = "")
        {

            try
            {
                var dataOrder = await _orderRepositor.GetDetailOrderByOrderId(Convert.ToInt32(Orderid));
                if (dataOrder != null && dataOrder[0].SalerId != 0)
                {
                    var user = await _userRepository.GetDetailUser(dataOrder[0].SalerId);
                    ViewBag.user = user.Entity;
                }
                if (dataOrder != null && dataOrder[0].ContactClientId != 0)
                {
                    var ContactClient = _contactClientRepository.GetByContactClientId(dataOrder[0].ContactClientId);
                    ViewBag.ContactClientEmail = ContactClient.Email;
                }
                ViewBag.Order = dataOrder[0];
                ViewBag.Type = type;
                ViewBag.ServiceType = ServiceType;
                var model = new SendEmailViewModel();
                model = null;
                switch (type)
                {

                    case (int)EmailType.Supplier:
                        {

                            ViewBag.EmailBody = await _emailService.GetTemplateSupplier(model, id, SupplierId, ServiceType, "", "", true);

                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SendEmailSupplier - SetServiceController: " + ex);
            }
            ViewBag.Orderid = Orderid;
            ViewBag.ServiceId = id;
            ViewBag.GroupBookingId = group_booking_id;

            return PartialView();
        }
        public async Task<IActionResult> ConfirmSendEmail(SendEmailViewModel model, List<AttachfileViewModel> attach_file)
        {

            var status = (int)ResponseType.ERROR;
            var msg = "Không thành công";
            try
            {
                if (model != null)
                {
                    bool resulstSendMail = await _emailService.SendEmail(model, attach_file);
                    if (resulstSendMail)
                    {
                        status = (int)ResponseType.SUCCESS;
                        msg = "Gửi email thành công";
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ConfirmSendEmail - SetServiceController: " + ex);
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
        public async Task<IActionResult> UpdateHotelBooking(int id, long OrderId, int salerId, int type)
        {
            var sst_status = (int)ResponseType.SUCCESS;
            var smg = "Không thành công";
            try
            {
                if (id != 0)
                {
                    switch (type)
                    {
                        case (int)ServiceType.BOOK_HOTEL_ROOM:
                        case (int)ServiceType.BOOK_HOTEL_ROOM_VIN:
                            {
                                var model = new HotelBooking();
                                model.Id = id;
                                model.SalerId = salerId;
                                model.UpdatedBy = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                                var data = await _hotelBookingRepositories.UpdateHotelBooking(model);
                                if (data != 0 && data > 0)
                                {
                                    _orderRepository.UpdateOrderOperator(OrderId);
                                    sst_status = (int)ResponseType.SUCCESS;
                                    smg = "Sửa thành công";
                                }
                                else
                                {
                                    sst_status = (int)ResponseType.ERROR;
                                    smg = "Sửa không thành công";
                                }
                            }
                            break;

                        case (int)ServiceType.Tour:
                            {
                                var model = new Entities.Models.Tour();
                                model.Id = id;
                                model.SalerId = salerId;
                                model.UpdatedBy = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                                var data = await _tourRepository.UpdateTour(model);
                                if (data != 0 && data > 0)
                                {
                                    _orderRepository.UpdateOrderOperator(OrderId);
                                    sst_status = (int)ResponseType.SUCCESS;
                                    smg = "Sửa thành công";
                                }
                                else
                                {
                                    sst_status = (int)ResponseType.ERROR;
                                    smg = "Sửa không thành công";
                                }


                            }
                            break;

                    }


                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateHotelBooking - SetServiceController: " + ex);
                sst_status = (int)ResponseType.ERROR;
                smg = "Đã xảy ra lỗi, vui lòng liên hệ IT";
            }

            return Ok(new
            {
                sst_status = sst_status,
                smg = smg
            });
        }
        public async Task<IActionResult> DetailUserHotel(int id, long orderid, int type)
        {

            try
            {
                if (id != 0)
                {
                    ViewBag.id = id;
                    ViewBag.type = type;
                    switch (type)
                    {
                        case (int)ServiceType.BOOK_HOTEL_ROOM:
                        case (int)ServiceType.BOOK_HOTEL_ROOM_VIN:
                            {
                                var model = await _hotelBookingRepositories.GetHotelBookingById(id);
                                ViewBag.Name = model[0].SalerName;
                                _orderRepository.UpdateOrderOperator(orderid);
                            }
                            break;
                        case (int)ServiceType.Tour:
                            {
                                var model = await _tourRepository.GetDetailTourByID(id);
                                ViewBag.Name = model.SalerIdName;
                                _orderRepository.UpdateOrderOperator(orderid);
                            }
                            break;
                    }
                    return PartialView();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("DetailUserAgent - SetServiceController: " + ex);
            }
            return PartialView();
        }
        public async Task<IActionResult> HotelPopupDetail(int id, int type, int orderid, string groupbookingid)
        {

            try
            {
                ViewBag.id = id;
                ViewBag.type = type;
                ViewBag.orderid = orderid;
                ViewBag.group_booking_id = groupbookingid;
                return View();
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("HotelPopupDetail - ContractController: " + ex);
                return View();
            }
        }
        [HttpPost]
        public async Task<IActionResult> UpdateServiceStatus(int id, int orderid, int type, string Note, string groupbookingid)
        {
            var status = (int)ResponseType.SUCCESS;
            var msg = "Không thành công";
            try
            {
                if (type != 0)
                {
                    var order = _orderRepositor.GetByOrderId(orderid);
                    string link = "/Order/" + orderid;
                    var current_user = _ManagementUser.GetCurrentUser();
                    var CreatedatedBy = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                    var model = new ServiceDeclines();
                    model.OrderId = orderid;
                    model.CreatedBy = CreatedatedBy;
                    model.Type = type;
                    if (type == (int)ServiceType.PRODUCT_FLY_TICKET)
                    {
                        model.ServiceId = groupbookingid;
                    }
                    else
                    {
                        model.ServiceId = id.ToString();
                    }

                    model.Note = Note;
                    await _hotelBookingRepositories.InsertServiceDeclines(model);
                    long UpdatedBy = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                    long UserVerify = 0;
                    switch (type)
                    {
                        case (int)ServiceType.BOOK_HOTEL_ROOM:
                        case (int)ServiceType.BOOK_HOTEL_ROOM_VIN:
                            {
                                var data = await _hotelBookingRepositories.UpdateHotelBookingStatus(id, (int)ServiceStatus.Decline);
                                if (data != 0 && data > 0)
                                {
                                    var listHotel = await _hotelBookingRepositories.GetListByOrderId(orderid);
                                    var dataOrder2 = await _orderRepository.ProductServiceName(orderid.ToString());
                                    dataOrder2 = dataOrder2.Where(s => s.Status != (int)ServiceStatus.Cancel).ToList();
                                    var listdata = dataOrder2.Where(s => s.Status == (int)ServiceStatus.Payment ? s.Status == (int)ServiceStatus.Payment : s.Status == (int)ServiceStatus.Decline).ToList();
                                    var listdata2 = dataOrder2.Where(s => s.Status == (int)ServiceStatus.Decline).ToList();
                                    if (listdata2.Count == dataOrder2.Count && listdata.Count == dataOrder2.Count)
                                    {
                                        await _orderRepository.UpdateOrderStatus(orderid, (int)OrderStatus.OPERATOR_DECLINE, UpdatedBy, UserVerify);
                                    }
                                    apiService.SendMessage(CreatedatedBy.ToString(), ((int)ModuleType.DICH_VU).ToString(), ((int)ActionType.TU_CHOI).ToString(), order.OrderNo, link, current_user.Role, listHotel.Count > 0 ? listHotel[0].ServiceCode : "0");
                                    status = (int)ResponseType.SUCCESS;
                                    msg = "Từ chối thành công";
                                }
                                else
                                {
                                    status = (int)ResponseType.ERROR;
                                    msg = "Từ chối không thành công";
                                }

                            }
                            break;

                        case (int)ServiceType.PRODUCT_FLY_TICKET:
                            {
                                var data = await _flyBookingDetailRepository.UpdateServiceStatus((int)ServiceStatus.Decline, groupbookingid, CreatedatedBy);
                                if (data > 0)
                                {
                                    var fly_list = _flyBookingDetailRepository.GetListByOrderId(orderid).ToList();
                                    var dataOrder2 = await _orderRepository.ProductServiceName(orderid.ToString());
                                    dataOrder2 = dataOrder2.Where(s => s.Status != (int)ServiceStatus.Cancel).ToList();
                                    var listdata = dataOrder2.Where(s => s.Status == (int)ServiceStatus.Payment ? s.Status == (int)ServiceStatus.Payment : s.Status == (int)ServiceStatus.Decline).ToList();
                                    var listdata2 = dataOrder2.Where(s => s.Status == (int)ServiceStatus.Decline).ToList();
                                    if (listdata2.Count == dataOrder2.Count && listdata.Count == dataOrder2.Count)
                                    {
                                        await _orderRepository.UpdateOrderStatus(orderid, (int)OrderStatus.OPERATOR_DECLINE, UpdatedBy, UserVerify);
                                    }

                                    apiService.SendMessage(CreatedatedBy.ToString(), ((int)ModuleType.DICH_VU).ToString(), ((int)ActionType.TU_CHOI).ToString(), order.OrderNo, link, current_user.Role, fly_list[0].ServiceCode);
                                    status = (int)ResponseType.SUCCESS;
                                    msg = "Đổi trạng thái từ chối dịch vụ thành công";
                                }

                            }
                            break;
                        case (int)ServiceType.Tour:
                            {
                                var data = await _tourRepository.UpdateTourStatus(id, (int)ServiceStatus.Decline);
                                if (data != 0 && data > 0)
                                {
                                    var listtour = await _tourRepository.GetTourByOrderId(orderid);
                                    var dataOrder2 = await _orderRepository.ProductServiceName(orderid.ToString());
                                    dataOrder2 = dataOrder2.Where(s => s.Status != (int)ServiceStatus.Cancel).ToList();
                                    var listdata = dataOrder2.Where(s => s.Status == (int)ServiceStatus.Payment ? s.Status == (int)ServiceStatus.Payment : s.Status == (int)ServiceStatus.Decline).ToList();
                                    var listdata2 = dataOrder2.Where(s => s.Status == (int)ServiceStatus.Decline).ToList();
                                    if (listdata2.Count == dataOrder2.Count && listdata.Count == dataOrder2.Count)
                                    {
                                        await _orderRepository.UpdateOrderStatus(orderid, (int)OrderStatus.OPERATOR_DECLINE, UpdatedBy, UserVerify);
                                    }
                                    apiService.SendMessage(CreatedatedBy.ToString(), ((int)ModuleType.DICH_VU).ToString(), ((int)ActionType.TU_CHOI).ToString(), order.OrderNo, link, current_user.Role, listtour[0].ServiceCode);
                                    status = (int)ResponseType.SUCCESS;
                                    msg = "Từ chối thành công";
                                }
                                else
                                {
                                    status = (int)ResponseType.ERROR;
                                    msg = "Từ chối không thành công";
                                }


                            }
                            break;
                        case (int)ServiceType.Other:
                            {
                                var data = await _otherBookingRepository.UpdateServiceStatus((int)ServiceStatus.Decline, id, (int)UpdatedBy);
                                if (data != 0 && data > 0)
                                {
                                    var otherBookings = await _otherBookingRepository.GetOtherBookingById(id);
                                    var dataOrder2 = await _orderRepository.ProductServiceName(orderid.ToString());
                                    dataOrder2 = dataOrder2.Where(s => s.Status != (int)ServiceStatus.Cancel).ToList();
                                    var listdata = dataOrder2.Where(s => s.Status == (int)ServiceStatus.Payment ? s.Status == (int)ServiceStatus.Payment : s.Status == (int)ServiceStatus.Decline).ToList();
                                    var listdata2 = dataOrder2.Where(s => s.Status == (int)ServiceStatus.Decline).ToList();
                                    if (listdata2.Count == dataOrder2.Count && listdata.Count == dataOrder2.Count)
                                    {
                                        await _orderRepository.UpdateOrderStatus(orderid, (int)OrderStatus.OPERATOR_DECLINE, UpdatedBy, UserVerify);
                                    }
                                    apiService.SendMessage(CreatedatedBy.ToString(), ((int)ModuleType.DICH_VU).ToString(), ((int)ActionType.TU_CHOI).ToString(), order.OrderNo, link, current_user.Role, otherBookings.ServiceCode);
                                    status = (int)ResponseType.SUCCESS;
                                    msg = "Từ chối thành công";
                                }
                                else
                                {
                                    status = (int)ResponseType.ERROR;
                                    msg = "Từ chối không thành công";
                                }


                            }
                            break;
                        case (int)ServiceType.VinWonder:
                            {
                                var data = await _vinWonderBookingRepository.UpdateServiceStatus((int)ServiceStatus.Decline, id, (int)UpdatedBy);
                                if (data != 0 && data > 0)
                                {
                                    var vinwonderBooking = _vinWonderBookingRepository.GetVinWonderBookingById(id);
                                    var dataOrder2 = await _orderRepository.ProductServiceName(orderid.ToString());
                                    dataOrder2 = dataOrder2.Where(s => s.Status != (int)ServiceStatus.Cancel).ToList();
                                    var listdata = dataOrder2.Where(s => s.Status == (int)ServiceStatus.Payment ? s.Status == (int)ServiceStatus.Payment : s.Status == (int)ServiceStatus.Decline).ToList();
                                    var listdata2 = dataOrder2.Where(s => s.Status == (int)ServiceStatus.Decline).ToList();
                                    if (listdata2.Count == dataOrder2.Count && listdata.Count == dataOrder2.Count)
                                    {
                                        await _orderRepository.UpdateOrderStatus(orderid, (int)OrderStatus.OPERATOR_DECLINE, UpdatedBy, UserVerify);
                                    }
                                    apiService.SendMessage(CreatedatedBy.ToString(), ((int)ModuleType.DICH_VU).ToString(), ((int)ActionType.TU_CHOI).ToString(), order.OrderNo, link, current_user.Role, vinwonderBooking.ServiceCode);
                                    status = (int)ResponseType.SUCCESS;
                                    msg = "Từ chối thành công";
                                }
                                else
                                {
                                    status = (int)ResponseType.ERROR;
                                    msg = "Từ chối không thành công";
                                }


                            }
                            break;

                    }


                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateServiceStatus - SetServiceController: " + ex);
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
        public async Task<IActionResult> Updatesupplier(long id, long supplierid)
        {
            var sst_status = (int)ResponseType.SUCCESS;
            var smg = "Không thành công";
            try
            {
                if (id != 0)
                {
                    var hotelBookingCode = await _hotelBookingCodeRepository.GetDetailBookingCodeById(id);
                    var model = new HotelBooking();
                    model.SupplierId = (int?)supplierid;
                    model.Id = id;
                    var data = await _hotelBookingRepositories.UpdateHotelBooking(model);
                    if (data != 0 && data > 0)
                    {
                        sst_status = (int)ResponseType.SUCCESS;
                        smg = "Thay đổi nhà cung cấp thành công";
                    }
                    else
                    {
                        sst_status = (int)ResponseType.ERROR;
                        smg = "Thay đổi nhà cung cấp không thành công";
                    }

                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Updatesupplier - SetServiceController: " + ex);
                sst_status = (int)ResponseType.ERROR;
                smg = "Đã xảy ra lỗi, vui lòng liên hệ IT";
            }

            return Ok(new
            {
                status = sst_status,
                smg = smg,

            });
        }
        public async Task<IActionResult> DetailSupplier(int SupplierId)
        {

            try
            {
                if (SupplierId != 0)
                {
                    var model = _supplierRepository.GetDetailById(SupplierId);
                    return PartialView(model);
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("DetailUserAgent - PaymentAccountController: " + ex);
            }
            return PartialView();
        }
        [HttpPost]
        public async Task<IActionResult> ExportExcel(SearchHotelBookingViewModel searchModel)
        {
            try
            {
                var current_user = _ManagementUser.GetCurrentUser();
                string _FileName = "Danh sách đặt dịch vụ khách sạn(" + current_user.Id + ").xlsx";
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

                var rsPath = await _hotelBookingRepositories.ExportDeposit(searchModel, FilePath);

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

        public async Task<IActionResult> SendYCChi(long id, string profit, int type)
        {

            try
            {
                EmailYCChiViewModel Model = null;
                if (id > 0)
                {
                    ViewBag.EmailBody = await _emailService.TemplatePaymentRequest(id, profit, type, Model);
                }
                ViewBag.id = id;
                ViewBag.type = type;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SendYCChi - SetServiceController: " + ex);
            }

            return PartialView();
        }
        public async Task<IActionResult> ConfirmSendYCChi(long id, string profit, int type, EmailYCChiViewModel model)
        {

            try
            {

                if (id > 0)
                {
                    string html = await _emailService.TemplatePaymentRequest(id, profit, type, model);
                    // var result = SendToPrinterHelper.SendToPrinter(html);
                    var result = 0;
                    if (result == 0)
                    {
                        return new JsonResult(new
                        {
                            status = (int)ResponseType.SUCCESS,
                            msg = "In file thành công",
                            html = html
                        });
                    }
                    else
                    {
                        return new JsonResult(new
                        {
                            status = (int)ResponseType.ERROR,
                            msg = "In file không thành công"
                        });
                    }
                }
                else
                {
                    return new JsonResult(new
                    {
                        status = (int)ResponseType.ERROR,
                        msg = "In file không thành công"
                    });
                }

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ConfirmSendYCChi - SetServiceController: " + ex);
                return new JsonResult(new
                {
                    status = (int)ResponseType.ERROR,
                    msg = "In file không thành công"
                });
            }


        }
        public async Task<IActionResult> BookingCodeSuggestion(string txt_search, string type)
        {

            try
            {
                if (txt_search != null)
                {

                    if (Convert.ToInt32(type) > 0 || type != "")
                    {
                        var data = await _orderESRepository.GetHotelBookingCode(txt_search, Convert.ToInt32(type));
                        return Ok(new
                        {
                            status = (int)ResponseType.SUCCESS,
                            data = data,

                        });
                    }

                }

                return Ok(new
                {
                    status = (int)ResponseType.SUCCESS,
                    data = new List<ESHotelBookingCodeViewModel>()
                });

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("OrderNoSuggestion - OrderController: " + ex.ToString());
                return Ok(new
                {
                    status = (int)ResponseType.SUCCESS,
                    data = new List<ESHotelBookingCodeViewModel>()
                });
            }

        }
        public async Task<IActionResult> SupplierContactSuggestion(string txt_search, string Supplierid)
        {

            try
            {
                var datas = _supplierRepository.GetSupplierContactList(Convert.ToInt32(Supplierid));
                return Ok(new
                {
                    status = (int)ResponseType.SUCCESS,
                    data = datas,

                });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SupplierContactSuggestion - SetServiceController: " + ex.ToString());
                return Ok(new
                {
                    status = (int)ResponseType.SUCCESS,
                    data = new List<SupplierContactViewModel>()
                });
            }

        }

        [HttpPost]
        public async Task<IActionResult> ExportVinHotelCode(long booking_id)
        {
            try
            {
                if (booking_id <= 0)
                {
                    return new JsonResult(new
                    {
                        isSuccess = false,
                        message = "Dữ liệu không chính xác, vui lòng thử lại / liên hệ bộ phận IT ",
                    });
                }

                var current_user = _ManagementUser.GetCurrentUser();
                var hotel_booking = await _hotelBookingRepositories.GetHotelBookingByID(booking_id);
                var hotel_booking_room = await _hotelBookingRoomRepository.GetByHotelBookingID(booking_id);
                var hotel_booking_rates = await _hotelBookingRoomRatesRepository.GetByHotelBookingID(booking_id);
                var hotel_booking_guest = await _hotelBookingGuestRepository.GetByHotelBookingID(booking_id);
                HotelBookingCreateBookingModel commit_booking = new HotelBookingCreateBookingModel()
                {
                    arrivalDate = hotel_booking.ArrivalDate.ToString("yyyy-MM-dd"),
                    departureDate = hotel_booking.DepartureDate.ToString("yyyy-MM-dd"),
                    distributionChannel = _configuration["API_Vinpearl:Distribution_ID"],
                    propertyId = hotel_booking.PropertyId,
                    reservations = new List<HotelBookingCreateBookingModelreservations>()

                };
                foreach (var room in hotel_booking_room)
                {
                    var guest = hotel_booking_guest.Where(x => x.HotelBookingRoomsId == room.Id);
                    var number_of_adt = guest.Count() > 0 ? guest.Count(x => x.Type == 0) : 1;
                    var number_of_chd = guest.Count() > 0 ? guest.Count(x => x.Type == 1) : 0;
                    var number_of_inf = guest.Count() > 0 ? guest.Count(x => x.Type == 2) : 0;
                    var item = new HotelBookingCreateBookingModelreservations()
                    {
                        roomOccupancy = new HotelBookingCreateBookingModelroomOccupancy()
                        {
                            numberOfAdult = number_of_adt,
                            otherOccupancies = new List<HotelBookingCreateBookingModelroomOccupancyotherOccupancies>()
                        }

                    };
                }

                return new JsonResult(new
                {
                    isSuccess = true,
                    message = "Xuất dữ liệu thành công",
                });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ExportVinHotelCode - SetServiceController: " + ex);
                return new JsonResult(new
                {
                    isSuccess = false,
                    message = ex.Message.ToString()
                });
            }
        }

    }

}
