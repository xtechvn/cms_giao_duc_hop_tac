using Caching.Elasticsearch;
using Caching.RedisWorker;
using Entities.Models;
using Entities.ViewModels;
using Entities.ViewModels.Attachment;
using Entities.ViewModels.ElasticSearch;
using Entities.ViewModels.HotelBookingCode;
using Entities.ViewModels.Mongo;
using Entities.ViewModels.OrderManual;
using ENTITIES.ViewModels.Articles;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OfficeOpenXml;
using Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;
using GDHT.CMS.Service;
using WEB.CMS.Customize;
using WEB.CMS.Models;

namespace GDHT.CMS.Controllers.Order
{
    [CustomAuthorize]

    public partial class OrderController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IOrderRepository _orderRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IAllCodeRepository _allCodeRepository;
        private readonly IContractRepository _contractRepository;
        private readonly IAccountClientRepository _accountClientRepository;
        private readonly IUserRepository _userRepository;
        private readonly IIdentifierServiceRepository _identifierServiceRepository;
        private ClientESRepository _clientESRepository;
        private UserESRepository _userESRepository;
        private NationalESRepository _nationalESRepository;
        private HotelESRepository _hotelESRepository;
        private IHotelBookingRepositories _hotelBookingRepository;
        private IHotelBookingRoomRepository _hotelBookingRoomRepository;
        private IHotelBookingRoomRatesRepository _hotelBookingRoomRatesRepository;
        private IHotelBookingRoomExtraPackageRepository _hotelBookingRoomExtraPackageRepository;
        private IHotelBookingGuestRepository _hotelBookingGuestRepository;
        private readonly ProvinceRedisService _provinceRedisService;
        private IFlyBookingDetailRepository _flyBookingDetailRepository;
        private IAirlinesRepository _airlinesRepository;
        private IPassengerRepository _passengerRepository;
        private ITourRepository _tourRepository;
        private IProvinceRepository _provinceRepository;
        private INationalRepository _nationalRepository;
        private ISupplierRepository _supplierRepository;
        private IGroupProductRepository _groupProductRepository;
        private IOtherBookingRepository _otherBookingRepository;
        private readonly IndentiferService indentiferService;
        private ManagementUser _ManagementUser;
        private readonly List<int> list_order_status_not_allow_to_edit = new List<int>() { (int)OrderStatus.FINISHED, (int)OrderStatus.CANCEL, (int)OrderStatus.WAITING_FOR_ACCOUNTANT,(int)OrderStatus.WAITING_FOR_OPERATOR };
        private readonly List<int> list_service_status_not_allow_to_edit = new List<int>() { (int)ServiceStatus.OnExcution, (int)ServiceStatus.ServeCode, (int)ServiceStatus.Payment };
        private readonly IAttachFileRepository _AttachFileRepository;
        private readonly IVinWonderBookingRepository _vinWonderBookingRepository;
        private APIService apiService;
        private readonly TourESRepository _tourESRepository;
        private readonly IHotelBookingCodeRepository _hotelBookingCodeRepository;
        private LogActionMongoService LogActionMongo;

        public OrderController(IConfiguration configuration, IOrderRepository orderRepository, IClientRepository clientRepository, IAllCodeRepository allcodeRepository, IUserRepository userRepository, IIdentifierServiceRepository identifierServiceRepository
                , IAccountClientRepository accountClientRepository, IHotelBookingRepositories hotelBookingRepository, IHotelBookingRoomRepository hotelBookingRoomRepository, IHotelBookingRoomRatesRepository hotelBookingRoomRatesRepository,
                IHotelBookingRoomExtraPackageRepository hotelBookingRoomExtraPackageRepository, IHotelBookingGuestRepository hotelBookingGuestRepository, IFlyBookingDetailRepository flyBookingDetailRepository, IAirlinesRepository airlinesRepository,
                 IPassengerRepository passengerRepository, IProvinceRepository provinceRepository, INationalRepository nationalRepository, ManagementUser managementUser, IOtherBookingRepository otherBookingRepository,
                 ITourRepository tourRepository, ISupplierRepository supplierRepository, IContractRepository contractRepository, IAttachFileRepository attachFileRepository, IVinWonderBookingRepository vinWonderBookingRepository,
                 IGroupProductRepository groupProductRepository, IHotelBookingCodeRepository hotelBookingCodeRepository, IDepartmentRepository departmentRepository)
        {
            _configuration = configuration;
            _orderRepository = orderRepository;
            _clientRepository = clientRepository;
            _allCodeRepository = allcodeRepository;
            _userRepository = userRepository;
            _identifierServiceRepository = identifierServiceRepository;
            _clientESRepository = new ClientESRepository(_configuration["DataBaseConfig:Elastic:Host"]);
            _userESRepository = new UserESRepository(_configuration["DataBaseConfig:Elastic:Host"]);
            _hotelESRepository = new HotelESRepository(_configuration["DataBaseConfig:Elastic:Host"]);
            _nationalESRepository = new NationalESRepository(_configuration["DataBaseConfig:Elastic:Host"]);
            _accountClientRepository = accountClientRepository;
            _provinceRedisService = new ProvinceRedisService(_configuration);
            _hotelBookingRepository = hotelBookingRepository;
            _hotelBookingRoomRepository = hotelBookingRoomRepository;
            _hotelBookingRoomRatesRepository = hotelBookingRoomRatesRepository;
            _hotelBookingRoomExtraPackageRepository = hotelBookingRoomExtraPackageRepository;
            _hotelBookingGuestRepository = hotelBookingGuestRepository;
            _flyBookingDetailRepository = flyBookingDetailRepository;
            _airlinesRepository = airlinesRepository;
            _passengerRepository = passengerRepository;
            _tourRepository = tourRepository;
            _provinceRepository = provinceRepository;
            _nationalRepository = nationalRepository;
            indentiferService = new IndentiferService(configuration);
            _supplierRepository = supplierRepository;
            _ManagementUser = managementUser;
            _contractRepository = contractRepository;
            _AttachFileRepository = attachFileRepository;
            _otherBookingRepository = otherBookingRepository;
            apiService = new APIService(configuration,userRepository);
            _vinWonderBookingRepository = vinWonderBookingRepository;
            _groupProductRepository = groupProductRepository;
            _tourESRepository = new TourESRepository(_configuration["DataBaseConfig:Elastic:Host"]);
            _hotelBookingCodeRepository = hotelBookingCodeRepository;
            LogActionMongo = new LogActionMongoService(configuration);

        }

        [HttpPost]
        public IActionResult CreateOrderManual()
        {
            ViewBag.Branch = _allCodeRepository.GetListByType(AllCodeType.BRANCH_CODE);
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddHotelService(long hotel_booking_id)
        {
            try
            {
                long _UserId = 0;
                if (HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) != null)
                {
                    _UserId = Convert.ToInt64(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                ViewBag.User = new UserESViewModel();
                var user = await _userRepository.GetChiefofDepartmentByServiceType((int)ServiceType.BOOK_HOTEL_ROOM_VIN);
                if (user != null && user.Id > 0)
                {
                    ViewBag.User = new UserESViewModel() { 
                        email=user.Email,
                        fullname=user.FullName,
                        id=user.Id,
                        phone=user.Phone,
                        username=user.UserName,
                        _id=user.Id
                    };
                }
                ViewBag.IsOrderManual = true;
                ViewBag.AllowToEdit = true;
                ViewBag.HotelBooking = new HotelBooking();
                ViewBag.Hotel = new HotelESViewModel();
                var booking = await _hotelBookingRepository.GetHotelBookingByID(hotel_booking_id);
                if (booking != null && booking.Id > 0)
                {
                    var order = await _orderRepository.GetOrderByID(booking.OrderId);
                    ViewBag.IsLock = order!=null && order.IsLock!=null? order.IsLock:false;
                    ViewBag.IsOrderManual = false;
                    ViewBag.HotelBooking = booking;
                    var hotel = await _hotelESRepository.GetHotelByID(booking.PropertyId);
                    if (hotel == null) hotel = new HotelESViewModel();
                    ViewBag.Hotel = hotel;
                    var user_by_booking = await _userESRepository.GetUserByID(booking.SalerId.ToString());
                    if(user_by_booking!=null && user_by_booking.id > 0)
                    {
                        ViewBag.User = user_by_booking;
                    }
                    ViewBag.IsOrderManual = false;
                    if (order != null)
                    {
                        ViewBag.IsOrderManual = true;
                    }
                    bool is_allow_to_edit = false;
                    if((order.SalerId != null && order.SalerId == _UserId) || (order.SalerGroupId != null && order.SalerGroupId.Contains(_UserId.ToString())))
                    {
                        if (!list_order_status_not_allow_to_edit.Contains((int)order.OrderStatus) && !list_service_status_not_allow_to_edit.Contains((int)booking.Status))
                        {
                            is_allow_to_edit = true;
                        }
                        if ((int)booking.Status == (int)ServiceStatus.Decline)
                        {
                            is_allow_to_edit = true;
                        }
                    }
                    ViewBag.AllowToEdit = is_allow_to_edit;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("AddHotelService - OrderController: " + ex);
            }
            return View();

        }
        [HttpPost]
        public async Task<IActionResult> AddHotelServiceRoom(long hotel_booking_id)
        {
            try
            {
                ViewBag.room_list = new List<HotelBookingRooms>();
                ViewBag.package_list = new List<HotelBookingRoomRates>();
                ViewBag.num_people = 0;
                if (hotel_booking_id > 0)
                {
                    var booking = await _hotelBookingRepository.GetHotelBookingByID(hotel_booking_id);
                    if(booking!=null && booking.Id > 0)
                    {
                        var rooms = await _hotelBookingRoomRepository.GetByHotelBookingID(hotel_booking_id);
                        var packages = await _hotelBookingRoomRatesRepository.GetByHotelBookingID(hotel_booking_id);
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
                
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("AddHotelServiceRoom - OrderController: " + ex);
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddHotelServiceRoomPackages(long hotel_booking_id)
        {
            var booking = await _hotelBookingRepository.GetHotelBookingByID(hotel_booking_id);
            if (booking == null || booking.Id < 0)
            {
                ViewBag.ExtraPackages = new List<HotelBookingRoomExtraPackages>();
                return View();

            }
            var extra_package = await _hotelBookingRoomExtraPackageRepository.GetByBookingID(hotel_booking_id);
            ViewBag.ExtraPackages = extra_package != null ? extra_package : new List<HotelBookingRoomExtraPackages>();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddHotelServiceGuest(long hotel_booking_id)
        {
            var booking = await _hotelBookingRepository.GetHotelBookingByID(hotel_booking_id);
            if (booking == null || booking.Id < 0)
            {
                ViewBag.Rooms = new List<HotelBookingRooms>();
                ViewBag.Guests = new List<HotelGuest>();
                return View();

            }
            ViewBag.Type = new List<AllCode>() { new AllCode() { CodeValue = 0, Description = "Người lớn" }, new AllCode() { CodeValue = 1, Description = "Trẻ em" }, new AllCode() { CodeValue = 2, Description = "Em bé" } };
            var rooms = await _hotelBookingRoomRepository.GetByHotelBookingID(hotel_booking_id);
            var guests = await _hotelBookingGuestRepository.GetByHotelBookingID(hotel_booking_id);
            ViewBag.Rooms = rooms != null ? rooms : new List<HotelBookingRooms>();
            ViewBag.Guests = guests != null ? guests : new List<HotelGuest>();

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ClientSuggestion(string txt_search)
        {

            try
            {
                if (txt_search != null)
                {
                    var data = await _clientESRepository.GetClientSuggesstion(txt_search);
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        data = data
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        data = new List<CustomerESViewModel>()
                    });
                }

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ClientSuggestion - OrderManualController: " + ex.ToString());
                return Ok(new
                {
                    status = (int)ResponseType.SUCCESS,
                    data = new List<CustomerESViewModel>()
                });
            }

        }
        [HttpPost]
        public async Task<IActionResult> UserSuggestion(string txt_search,int service_type=0)
        {

            try
            {
                long _UserId = 0;
                if (HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) != null)
                {
                    _UserId = Convert.ToInt64(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                if (txt_search == null) txt_search = "";
                var data = await _userESRepository.GetUserSuggesstion(txt_search);

                if (service_type <= 0)
                {
                    if (data == null || data.Count <= 0)
                    {
                        var data_sql = await _userRepository.GetUserSuggesstion(txt_search);
                        data = new List<UserESViewModel>();
                        if (data_sql != null && data_sql.Count > 0)
                        {
                            data.AddRange(data_sql.Select(x => new UserESViewModel() { email = x.Email, fullname = x.FullName, id = x.Id, phone = x.Phone, username = x.UserName, _id = x.Id }));
                        }
                    }
                }
                else
                {
                    List<int?> deparments = new List<int?>();
                    switch (service_type)
                    {
                        case (int)ServiceType.BOOK_HOTEL_ROOM_VIN:
                            {
                                deparments.Add(ReadFile.LoadConfig().Department_Operator_Hotel);
                            }
                            break;
                      case (int)ServiceType.PRODUCT_FLY_TICKET:
                            {
                                deparments.Add(ReadFile.LoadConfig().Department_Operator_Fly);

                            }
                            break;
                        case (int)ServiceType.Tour:
                            {
                                deparments.Add(ReadFile.LoadConfig().Department_Operator_Tour);

                            }
                            break;
                        case (int)ServiceType.VinWonder:
                        case (int)ServiceType.Other:
                            {
                                deparments.Add(ReadFile.LoadConfig().Department_Operator_Hotel);
                                deparments.Add(ReadFile.LoadConfig().Department_Operator_Fly);
                                deparments.Add(ReadFile.LoadConfig().Department_Operator_Tour);
                            }
                            break;
                        case (int)ServiceType.WaterSport:
                            {
                                deparments.Add(ReadFile.LoadConfig().Department_Operator_WaterSport);

                            }
                            break;
                    }
                    var list_user_ids = await _userRepository.GetListUserDepartById(deparments);
                    var data_sql = await _userRepository.GetUserSuggesstion(txt_search, list_user_ids.Where(x=> x.UserId != null).Select(x=>(int)x.UserId ).ToList());
                    if (data_sql != null && data_sql.Count > 0)
                    {
                        data=data_sql.Select(x => new UserESViewModel() { email = x.Email, fullname = x.FullName, id = x.Id, phone = x.Phone, username = x.UserName, _id = x.Id }).ToList();
                    }
                }
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
        public async Task<IActionResult> HotelSuggestion(string txt_search)
        {

            try
            {
                var data = await _hotelESRepository.GetListProduct(txt_search);
                return Ok(new
                {
                    status = (int)ResponseType.SUCCESS,
                    data = data
                });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("HotelSuggestion - OrderManualController: " + ex.ToString());
                return Ok(new
                {
                    status = (int)ResponseType.SUCCESS,
                    data = new List<CustomerESViewModel>()
                });
            }

        }
        [HttpPost]
        public async Task<IActionResult> SummitHotelServiceData(OrderManualHotelSerivceSummitHotel data)
        {

            try
            {
                HotelESRepository _ESRepository = new HotelESRepository(_configuration["DataBaseConfig:Elastic:Host"]);
                HotelESViewModel hotel_detail = await _hotelESRepository.GetHotelByID(data.hotel.hotel_id);
                if (hotel_detail.checkintime <= DateTime.Now.AddDays(-30)) hotel_detail.checkintime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 10, 0, 0);
                if (hotel_detail.checkouttime <= DateTime.Now.AddDays(-30)) hotel_detail.checkouttime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 13, 0, 0);
                int user_id = 0;
                if (HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) != null)
                {
                    user_id = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                //-- Check user & permission
                if (user_id <= 0)
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = "You do not have permission to do this."
                    });
                }
                //-- Check if order is manual Order:
                var exists_order = await _orderRepository.GetOrderByID(data.order_id);
               
                //-- Validate Data(server-side):
                if (data.hotel == null)
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = "Dữ liệu khách sạn gửi lên không chính xác, vui lòng kiểm tra lại"
                    });
                }
                else if (data.rooms.Any(x => x.package == null || x.package.Count < 1))
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = "Mỗi phòng trong khách sạn phải có ít nhất 01 gói"
                    });
                }
                /*
                else if(data.rooms.Count != data.hotel.number_of_rooms)
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = "Tổng số phòng trong phần thông tin khách sạn không được khác tổng số phòng nhập ở mục danh sách phòng"
                    });
                }*/
                if (data.hotel.id <= 0 || data.hotel.service_code == null || data.hotel.service_code.Trim() == "")
                {
                    data.hotel.service_code = await indentiferService.GetServiceCodeByType((int)ServicesType.VINHotelRent);
                }

                #region Check Client Debt:
                long id = 0;
                double total_amount = 0;
                int service_status = (int)ServiceStatus.OnExcution;
                if (data.hotel.id <= 0)
                {
                    service_status = (int)ServiceStatus.New;
                    total_amount += data.rooms.Sum(x => x.package.Sum(x=>x.amount));
                    total_amount += data.extra_package!=null && data.extra_package.Count>0? data.extra_package.Sum(x => x.amount):0;
                }
                else
                {
                    var exists_hotel = await _hotelBookingRepository.GetHotelBookingByID(data.hotel.id);
                    service_status = (int)exists_hotel.Status;
                    total_amount += data.rooms.Sum(x => x.package.Sum(x => x.amount));
                    total_amount += data.extra_package != null && data.extra_package.Count > 0 ? data.extra_package.Sum(x => x.amount) : 0;
                    total_amount -= exists_hotel.TotalAmount;
                }
                bool is_allow_to_edit = false;
                if (exists_order != null && exists_order.OrderStatus != null && ((exists_order.SalerId != null && exists_order.SalerId == user_id) || (exists_order.SalerGroupId != null && exists_order.SalerGroupId.Contains(user_id.ToString()))))
                {
                    if (!list_order_status_not_allow_to_edit.Contains((int)exists_order.OrderStatus) && !list_service_status_not_allow_to_edit.Contains(service_status))
                    {
                        is_allow_to_edit = true;
                    }
                    if ((int)service_status == (int)ServiceStatus.Decline)
                    {
                        is_allow_to_edit = true;
                    }
                }
                if (!is_allow_to_edit)
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = "Bạn không có quyền chỉnh sửa dịch vụ này."
                    });
                }
                int is_debt_able = await _orderRepository.IsClientAllowedToDebtNewService(total_amount, (long)exists_order.ClientId, exists_order.OrderId, (int)ServiceType.BOOK_HOTEL_ROOM_VIN);
                /*
                var client = await _clientRepository.GetClientDetailByClientId((long)exists_order.ClientId);
                if(client!=null && client.ClientType == (int)ClientType.kl)
                {
                    //-- Update Booking
                    id = await _hotelBookingRepository.UpdateHotelBooking(data, hotel_detail, user_id);
                }
                else if(client!=null)
                {
                    double total_amount = 0;
                    string msg = "Giá trị dịch vụ thêm mới trong đơn hàng vượt quá hạn mức công nợ còn lại của khách hàng. Bạn không thể thêm dịch vụ này";
                    if (data.hotel.id <= 0 )
                    {
                        total_amount += data.rooms.Sum(x => x.total_amount);
                        total_amount += data.extra_package.Sum(x => x.amount);
                    }
                    else
                    {
                        var exists_hotel = await _hotelBookingRepository.GetHotelBookingByID(data.hotel.id);
                        total_amount += data.rooms.Sum(x => x.total_amount);
                        total_amount += data.extra_package.Sum(x => x.amount);
                        total_amount -= exists_hotel.TotalAmount;
                        msg = "Giá trị dịch vụ cập nhật thêm vượt quá hạn mức công nợ còn lại của khách hàng. Bạn không thể cập nhật dịch vụ này";
                    }
                    int is_debt_able = await _orderRepository.IsClientAllowedToDebtNewService(total_amount, (long)exists_order.ClientId, exists_order.OrderId, (int)ServiceType.BOOK_HOTEL_ROOM_VIN);
                    if (is_debt_able == (int)DebtType.DEBT_ACCEPTED)
                    {
                        //-- Update Booking
                        id = await _hotelBookingRepository.UpdateHotelBooking(data, hotel_detail, user_id);
                    }
                    else
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = msg
                        });
                    }
                }
                else
                {
                    LogHelper.InsertLogTelegram("SummitHotelServiceData - OrderManualController: Client = "+ exists_order.ClientId ==null ? "NULL": exists_order.ClientId.ToString() +" in OrderID = "+ exists_order.OrderId);

                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = "Thông tin đơn hàng không chính xác, vui lòng liên hệ IT"
                    });
                }
                */

                #endregion
                //-- Update Booking
                id = await _hotelBookingRepository.UpdateHotelBooking(data, hotel_detail, user_id, is_debt_able);
                if (id <= 0)
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = "Thêm mới / Cập nhật dịch vụ vé máy bay thất bại, vui lòng liên hệ IT",
                        data = id
                    });

                }
               

                #region Update Order Amount:
                await _orderRepository.UpdateOrderDetail(data.order_id, user_id);
                await _orderRepository.ReCheckandUpdateOrderPayment(data.order_id); 
                #endregion

                return Ok(new
                {
                    status = (int)ResponseType.SUCCESS,
                    msg = "Thêm mới / Cập nhật dịch vụ khách sạn thành công",
                    data=id
                });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SummitHotelServiceData - OrderManualController: " + ex.ToString());
            }
            return Ok(new
            {
                status = (int)ResponseType.FAILED,
                msg = "Thêm mới / Cập nhật dịch vụ khách sạn thất bại, vui lòng liên hệ IT"
            });

        }
        [HttpPost]
        public async Task<IActionResult> AddFlyBookingService(long order_id, string group_fly)
        {
            ViewBag.FlyDetailCount = 1;
            ViewBag.Go = null;
            ViewBag.Back = null;
            ViewBag.User = new UserESViewModel();
            ViewBag.flybooking_from = null;
            ViewBag.flybooking_to = null;
            ViewBag.IsOrderManual = false;
            ViewBag.AllowToEdit = false;
            try
            {
                

                long _UserId = 0;
                if (HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) != null)
                {
                    _UserId = Convert.ToInt64(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
               
                var user = await _userRepository.GetChiefofDepartmentByServiceType((int)ServiceType.BOOK_HOTEL_ROOM_VIN);
                if (user != null && user.Id > 0)
                {
                    ViewBag.User = new UserESViewModel()
                    {
                        email = user.Email,
                        fullname = user.FullName,
                        id = user.Id,
                        phone = user.Phone,
                        username = user.UserName,
                        _id = user.Id
                    };
                }
                if (order_id > 0)
                {
                    var fly_detail = await _flyBookingDetailRepository.GetListByGroupFlyID(order_id, group_fly);
                    int fly_status = (int)ServiceStatus.OnExcution;
                    if (fly_detail != null && fly_detail.Count > 0)
                    {
                        ViewBag.FlyDetailCount = fly_detail.Count;
                        ViewBag.Go = fly_detail.Where(x => x.Leg == 0).FirstOrDefault();
                        if (fly_detail.Count > 1)
                        {
                            ViewBag.Back = fly_detail.Where(x => x.Leg == 1).FirstOrDefault();
                        }
                        var user_by_booking = await _userESRepository.GetUserByID(fly_detail[0].SalerId.ToString());
                        if(user_by_booking!=null && user_by_booking.id > 0)
                        {
                            ViewBag.User = user_by_booking;
                        }
                        ViewBag.flybooking_from = await _airlinesRepository.GetAirportByCode(fly_detail[0].StartPoint);
                        ViewBag.flybooking_to = await _airlinesRepository.GetAirportByCode(fly_detail[0].EndPoint);
                        fly_status = (int)fly_detail[0].Status;
                       
                    }
                    else
                    {
                        fly_status = (int)ServiceStatus.New;

                    }
                    var order = await _orderRepository.GetOrderByID(order_id);
                    if (order != null)
                    {
                        ViewBag.IsLock = order != null && order.IsLock != null ? order.IsLock : false;
                        ViewBag.IsOrderManual = true;
                    }
                    bool is_allow_to_edit = false;
                    if ((order.SalerId != null && order.SalerId == _UserId) || (order.SalerGroupId != null && order.SalerGroupId.Contains(_UserId.ToString())))
                    {
                        if (!list_order_status_not_allow_to_edit.Contains((int)order.OrderStatus) && !list_service_status_not_allow_to_edit.Contains(fly_status))
                        {
                            is_allow_to_edit = true;
                        }
                        if (fly_status == (int)ServiceStatus.Decline)
                        {
                            is_allow_to_edit = true;
                        }
                    }
                    ViewBag.AllowToEdit = is_allow_to_edit;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("AddFlyBookingService - OrderManualController: " + ex.ToString());
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddFlyBookingServicePassenger(long order_id, string group_fly)
        {
            var passengers = await _passengerRepository.GetByOrderID(order_id, group_fly);
            ViewBag.passengers = passengers;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddFlyBookingServiceRoute(long order_id, string group_fly, int leg)
        {
            try
            {
                ViewBag.leg = leg;
                var fly_detail = await _flyBookingDetailRepository.GetByFlyGroupAndLeg(order_id, leg, group_fly);
                ViewBag.fly_detail = fly_detail;

                
                if (fly_detail != null)
                {
                    if (fly_detail.SalerId != null && fly_detail.SalerId > 0)
                    {
                        ViewBag.User = await _userESRepository.GetUserByID(fly_detail.SalerId.ToString());
                    }
                    else
                    {
                        ViewBag.User = null;
                    }
                    ViewBag.airline = await _airlinesRepository.GetAirLineByCode(fly_detail.Airline);
                }
                else
                {
                    ViewBag.User = null;
                    ViewBag.airline = null;
                }

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("AddFlyBookingServiceRoute - OrderManualController: " + ex.ToString());

            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddFlyBookingServiceExtraPackages(long order_id, string group_fly)
        {
            List<FlyBookingExtraPackages> extra_list = new List<FlyBookingExtraPackages>();
            ViewBag.Go = null;
            ViewBag.Back = null;
            ViewBag.TotalAmountAdt = 0;
            ViewBag.TotalAmountChd = 0;
            ViewBag.TotalAmountInf = 0;
            ViewBag.TotalProfitAdt = 0;
            ViewBag.TotalProfitChd = 0;
            ViewBag.TotalProfitInf = 0;
            ViewBag.TotalAmount = 0;
            double total_amount = 0;
            ViewBag.passengers = new List<Passenger>();
            ViewBag.FlyDetailCount = 1;
            ViewBag.PriceDetail = null;
            ViewBag.package_adt = 0;
            ViewBag.package_chd = 0;
            ViewBag.package_inf = 0;
            if (order_id > 0)
            {
                ViewBag.PriceDetail = await _flyBookingDetailRepository.GetActiveFlyBookingPriceDetailByOrder(order_id);
                var fly_detail = await _flyBookingDetailRepository.GetListByGroupFlyID(order_id, group_fly);
                if (fly_detail != null && fly_detail.Count > 0)
                {
                    int adt_number = (fly_detail[0].AdultNumber != null ? (int)fly_detail[0].AdultNumber : 1);
                    int chd_number = (fly_detail[0].ChildNumber != null ? (int)fly_detail[0].ChildNumber : 1);
                    int inf_number = (fly_detail[0].InfantNumber != null ? (int)fly_detail[0].InfantNumber : 0);
                    double total_profit = fly_detail.Sum(x => (x.Profit != null ? (double)x.Profit : 0)+ (x.Adgcommission != null ? (double)x.Adgcommission : 0) + (x.OthersAmount != null ? (double)x.OthersAmount : 0));
                    var fly_count = fly_detail.Count;
                    ViewBag.FlyDetailCount = fly_count;
                    var go = fly_detail.Where(x => x.Leg == 0).FirstOrDefault();
                    ViewBag.Go = go;
                    var go_airline = await _airlinesRepository.GetAirLineByCode(fly_detail.Where(x => x.Leg == 0).First().Airline);
                    if (go_airline != null) ViewBag.GoAirLineName = go_airline.NameVi;
                    double profit_per_one = (double)total_profit / (double)fly_count / (double)(adt_number + chd_number);
                    double package_adt = (double)go.FareAdt + (double)go.TaxAdt + (double)go.FeeAdt + (double)go.ServiceFeeAdt;
                    double package_chd = (double)go.FareChd + (double)go.TaxChd + (double)go.FeeChd + (double)go.ServiceFeeChd;
                    double package_inf = (double)go.FareInf + (double)go.TaxInf + (double)go.FeeInf + (double)go.ServiceFeeInf;
                    double TotalProfitAdt = go.ProfitAdt != null ? (double)go.ProfitAdt : 0;
                    double TotalProfitChd = go.ProfitChd != null ? (double)go.ProfitChd : 0; 
                    double TotalProfitInf = go.ProfitInf != null ? (double)go.ProfitInf : 0;
                    double TotalAmountAdt = go.AmountAdt != null ? (double)go.AmountAdt : 0;
                    double TotalAmountChd = go.AmountChd != null ? (double)go.AmountChd : 0;
                    double TotalAmountInf = go.AmountInf != null ? (double)go.AmountInf : 0;

                    //TotalAmountAdt += go.ProfitAdt != null ? (double)go.ProfitAdt : 0;
                    //TotalAmountChd += go.ProfitChd != null ? (double)go.ProfitChd : 0;
                    //TotalAmountInf += go.ProfitInf != null ? (double)go.ProfitInf : 0;
                    if (fly_detail.Count > 1)
                    {
                        var back = fly_detail.Where(x => x.Leg == 1).FirstOrDefault();
                        ViewBag.Back = back;
                        package_adt += (double)back.FareAdt + (double)back.TaxAdt + (double)back.FeeAdt + (double)back.ServiceFeeAdt;
                        package_chd += (double)back.FareChd + (double)back.TaxChd + (double)back.FeeChd + (double)back.ServiceFeeChd;
                        package_inf += (double)back.FareInf + (double)back.TaxInf + (double)back.FeeInf + (double)back.ServiceFeeInf;
                        TotalProfitAdt += back.ProfitAdt != null ? (double)back.ProfitAdt : 0;
                        TotalProfitChd += back.ProfitChd != null ? (double)back.ProfitChd : 0;
                        TotalProfitInf += back.ProfitInf != null ? (double)back.ProfitInf : 0;
                        TotalAmountAdt += back.AmountAdt != null ? (double)back.AmountAdt : 0;
                        TotalAmountChd += back.AmountChd != null ? (double)back.AmountChd : 0;
                        TotalAmountInf += back.AmountInf != null ? (double)back.AmountInf : 0;

                        //TotalAmountAdt += back.ProfitAdt != null ? (double)back.ProfitAdt : 0;
                        //TotalAmountChd += back.ProfitChd != null ? (double)back.ProfitChd : 0;
                        //TotalAmountInf += back.ProfitInf != null ? (double)back.ProfitInf : 0;

                        var back_airline = await _airlinesRepository.GetAirLineByCode(fly_detail.Where(x => x.Leg == 1).First().Airline);
                        if (back_airline != null) ViewBag.BackAirLineName = back_airline.NameVi;
                    }
                    
                    ViewBag.package_adt = package_adt;
                    ViewBag.package_chd = package_chd;
                    ViewBag.package_inf = package_inf;
                    ViewBag.TotalAmountAdt = TotalAmountAdt;
                    ViewBag.TotalAmountChd = TotalAmountChd;
                    ViewBag.TotalAmountInf = TotalAmountInf;
                    ViewBag.TotalProfitAdt = Math.Round(((TotalAmountAdt / (double)adt_number) - package_adt) * (double)adt_number, 0);
                    ViewBag.TotalProfitChd = Math.Round(((TotalAmountChd / (double)chd_number) - package_chd) * (double)chd_number, 0);
                    ViewBag.TotalProfitInf = Math.Round(((TotalAmountInf / (double)inf_number) - package_inf) * (double)inf_number, 0);
                    total_amount += fly_detail.Sum(x => x.Amount);
                    extra_list = await _flyBookingDetailRepository.GetExtraPackageByFlyBookingId(group_fly);
                }
                var guests = await _passengerRepository.GetByOrderID(order_id, group_fly);
                if (guests != null) ViewBag.passengers = guests;
            }
            ViewBag.extra_list = extra_list;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AirlinesSuggestion(string txt_search)
        {

            try
            {
                var data = await _airlinesRepository.SearchAirlines(txt_search);
                return Ok(new
                {
                    status = (int)ResponseType.SUCCESS,
                    data = data
                });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("HotelSuggestion - OrderManualController: " + ex.ToString());
                return Ok(new
                {
                    status = (int)ResponseType.SUCCESS,
                    data = new List<CustomerESViewModel>()
                });
            }

        }
        [HttpPost]
        public async Task<IActionResult> AirPortCodeSuggestion(string txt_search)
        {

            try
            {
                var data = await _airlinesRepository.GetAirportCode(txt_search);
                return Ok(new
                {
                    status = (int)ResponseType.SUCCESS,
                    data = data
                });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("AirPortCodeSuggestion - OrderManualController: " + ex.ToString());
                return Ok(new
                {
                    status = (int)ResponseType.SUCCESS,
                    data = new List<AirPortCode>()
                });
            }

        }
        [HttpPost]
        public async Task<IActionResult> SummitFlyBookingServiceData(OrderManualFlyBookingServiceSummitModel data)
        {

            try
            {
                //-- Check if order is manual Order:
                var exists_order = await _orderRepository.GetOrderByID(data.order_id);
                /*
                if (exists_order == null || exists_order.OrderId <= 0 || (!exists_order.OrderNo.StartsWith("O") && !exists_order.OrderNo.StartsWith("DH")))
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = "Không thể thêm dịch vụ cho đơn hàng không phải là đơn tạo thủ công."
                    });
                }*/
                int _UserId = 0;
                if (HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) != null)
                {
                    _UserId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
               
                if (data.go == null || data.start_point == null || data.start_point.Trim() == "" || data.end_point == null || data.end_point.Trim() == "")
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = "Dữ liệu gửi lên không chính xác, vui lòng kiểm tra lại."
                    });
                }

                data.user_summit = (int)_UserId;
                var client = await _clientRepository.GetClientDetailByClientId((long)exists_order.ClientId);
                data.client_type = (int)client.ClientType;
                if (data.service_code == null || data.service_code.Trim() == "")
                {
                    data.service_code = await indentiferService.GetServiceCodeByType((int)ServicesType.FlyingTicket);
                }
                if (data.end_date < data.start_date)
                {
                    data.end_date = data.start_date;
                }

                #region Check Client Debt:
                long id = 0;
                double total_amount = 0;
                int service_status = (int)ServiceStatus.OnExcution;
                if (data.go.id <= 0)
                {
                    service_status = (int)ServiceStatus.New;
                    total_amount += data.extra_packages.Sum(x => x.amount);
                }
                else
                {
                    var exists_fly = await _flyBookingDetailRepository.GetListByGroupFlyID(data.group_id);
                    service_status = (int)exists_fly[0].Status;
                    total_amount += data.extra_packages.Sum(x => x.amount);
                    double total_exists_fly_amount = exists_fly.Sum(x => x.Amount);
                    total_amount -= total_exists_fly_amount;
                }
                bool is_allow_to_edit = false;
                if (exists_order != null && exists_order.OrderStatus != null && ((exists_order.SalerId != null && exists_order.SalerId == _UserId) || (exists_order.SalerGroupId != null && exists_order.SalerGroupId.Contains(_UserId.ToString()))))
                {
                    if (!list_order_status_not_allow_to_edit.Contains((int)exists_order.OrderStatus) && !list_service_status_not_allow_to_edit.Contains(service_status))
                    {
                        is_allow_to_edit = true;
                    }
                    if ((int)service_status == (int)ServiceStatus.Decline)
                    {
                        is_allow_to_edit = true;
                    }
                }
                if (!is_allow_to_edit)
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = "Bạn không có quyền chỉnh sửa dịch vụ này."
                    });
                }
                int is_debt_able = await _orderRepository.IsClientAllowedToDebtNewService(total_amount, (long)exists_order.ClientId, exists_order.OrderId, (int)ServiceType.PRODUCT_FLY_TICKET);

                /*
                if (client != null && client.ClientType == (int)ClientType.kl)
                {
                    //-- Update Booking
                    id = await _flyBookingDetailRepository.SummitFlyBookingServiceData(data);
                }
                else if (client != null)
                {
                    double total_amount = 0;
                    string msg = "Giá trị dịch vụ thêm mới trong đơn hàng vượt quá hạn mức công nợ còn lại của khách hàng. Bạn không thể thêm dịch vụ này";
                    if (data.go.id <= 0)
                    {
                        total_amount += data.extra_packages.Sum(x => x.amount);
                    }
                    else
                    {
                        var exists_fly = await _flyBookingDetailRepository.GetListByGroupFlyID(data.group_id);
                        total_amount += data.extra_packages.Sum(x => x.amount);
                        double total_exists_fly_amount = exists_fly.Sum(x => x.Amount);
                        total_amount -= total_exists_fly_amount;
                        msg = "Giá trị dịch vụ cập nhật thêm vượt quá hạn mức công nợ còn lại của khách hàng. Bạn không thể cập nhật dịch vụ này";
                    }
                    int is_debt_able = await _orderRepository.IsClientAllowedToDebtNewService(total_amount, (long)exists_order.ClientId, exists_order.OrderId, (int)ServiceType.PRODUCT_FLY_TICKET);
                    if (is_debt_able == (int)DebtType.DEBT_ACCEPTED)
                    {
                        //-- Update Booking
                        id = await _flyBookingDetailRepository.SummitFlyBookingServiceData(data,true);
                    }
                    else
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = msg
                        });
                    }
                }
                else
                {
                    LogHelper.InsertLogTelegram("SummitFlyBookingServiceData - OrderManualController: Client = " + exists_order.ClientId == null ? "NULL" : exists_order.ClientId.ToString() + " in OrderID = " + exists_order.OrderId);

                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = "Thông tin đơn hàng không chính xác, vui lòng liên hệ IT"
                    });
                }
                */

                #endregion

                //-- Update Booking
                id = await _flyBookingDetailRepository.SummitFlyBookingServiceData(data,is_debt_able);
                if (id <= 0)
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = "Thêm mới / Cập nhật dịch vụ vé máy bay thất bại, vui lòng liên hệ IT",
                        data = id
                    });

                }
                //-- Update Code:
                var detail = await _flyBookingDetailRepository.GetListByGroupFlyID(data.order_id, data.group_id);
                if(detail!=null && detail.Count > 0)
                {
                    var go_detail = detail.First(x => x.Leg == 0);
                    foreach (var route in detail)
                    {
                        if (route.BookingCode != null && route.BookingCode.Trim() != "")
                        {
                            var hotel_code_view_model = new HotelBookingCodeViewModel()
                            {
                                AttactFile = "",
                                BookingCode = route.BookingCode,
                                CreatedBy = _UserId,
                                Description = "Code vé máy bay chiều " + (route.Leg == 0 ? "đi" : "về"),
                                HotelBookingId = go_detail.Id,
                                IsDelete = 0,
                                Note = (route.Leg == 0 ? "go_" : "back_") + data.group_id.Replace(",", "_"),
                                Type = (int)ServicesType.FlyingTicket,
                                UpdatedBy = _UserId
                            };
                            await _hotelBookingCodeRepository.InsertHotelBookingCode(hotel_code_view_model);
                        }
                    }
                    if (detail.Count < 2)
                    {
                        await _hotelBookingCodeRepository.DeleteBookingCodeByIdandNote(go_detail.Id, "back_" + data.group_id.Replace(",", "_"));
                    }
                }
                #region Update Order Amount:

                await _orderRepository.UpdateOrderDetail(data.order_id, _UserId);
                await _orderRepository.ReCheckandUpdateOrderPayment(data.order_id);

                #endregion

                return Ok(new
                {
                    status = (int)ResponseType.SUCCESS,
                    msg = "Thêm mới / Cập nhật dịch vụ vé máy bay thành công",
                    data=id
                });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SummitFlyBookingServiceData - OrderManualController: " + ex.ToString());
            }
            return Ok(new
            {
                status = (int)ResponseType.FAILED,
                msg = "Thêm mới / Cập nhật dịch vụ vé máy bay thất bại, vui lòng liên hệ IT"
            });

        }
        [HttpPost]
        public async Task<IActionResult> CreateManualOrder(OrderManualSummitViewModel model)
        {

            try
            {
                if (model == null || model.client_id <= 0 || model.branch <= 0 || model.main_sale_id <= 0)
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = "Dữ liệu gửi lên không chính xác, vui lòng kiểm tra lại"
                    });
                }
                long _UserId = 0;
                if (HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) != null)
                {
                    _UserId = Convert.ToInt64(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                int company_type = 0;
                try
                {
                    company_type = Convert.ToInt32(_configuration["CompanyType"]);
                }
                catch { }
                Entities.Models.Order order = new Entities.Models.Order()
                {
                    OrderNo = await _identifierServiceRepository.buildOrderNoManual(company_type),
                    SalerId = model.main_sale_id,
                    SalerGroupId = string.Join(",", model.sub_sale_id),
                    Note = model.note,
                    BranchCode = model.branch,
                    UserUpdateId = _UserId,
                    CreatedBy = _UserId,
                    CreateTime = DateTime.Now,
                    UpdateLast = DateTime.Now,
                    ClientId = model.client_id,
                    AccountClientId = _accountClientRepository.GetMainAccountClientByClientId(model.client_id),
                    SystemType = (int)SystemType.Backend,
                    OrderStatus = (int)OrderStatus.CREATED_ORDER,
                    PaymentStatus = (int)PaymentStatus.UNPAID,
                    ExpriryDate = DateTime.Now.AddMonths(1),
                    StartDate = null,
                    EndDate = null,
                    Amount = 0,
                    Label = model.label
                };
                var exists = await _orderRepository.GetOrderByOrderNo(order.OrderNo);
                if (exists != null && exists.OrderId > 0)
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = "Mã đơn hàng đã tồn tại,vui lòng thử lại.",
                        order_id = -1
                    });
                }
                var result = await _orderRepository.CreateOrder(order);
                var current_user = _ManagementUser.GetCurrentUser();
               
                string link = "/Order/" + result.OrderId;
                 apiService.SendMessage( _UserId.ToString(),((int) ModuleType.DON_HANG).ToString(), ((int)ActionType.TAO_MOI).ToString(), order.OrderNo, link, current_user==null? "0": current_user.Role);
                //-- Bo sung tao contact client:
                //-- Get Contract va bo sung contractID
                return Ok(new
                {
                    status = (int)ResponseType.SUCCESS,
                    msg = "Tạo đơn hàng thành công. Mã đơn hàng: " + result.OrderNo,
                    order_id = result.OrderId
                });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CreateManualOrder - OrderManualController: " + ex.ToString());
                return Ok(new
                {
                    status = (int)ResponseType.ERROR,
                    msg = "Tạo đơn hàng không thành công. Vui lòng liên hệ IT"
                });
            }

        }
        [HttpPost]
        public async Task<IActionResult> AddTourService(long order_id, long tour_id)
        {
            ViewBag.User = new UserESViewModel();
            ViewBag.Tour = null;
            ViewBag.TourType = _allCodeRepository.GetListByType(AllCodeType.TOUR_TYPE);
            ViewBag.OrganizingType = _allCodeRepository.GetListByType(AllCodeType.ORGANIZING_TYPE);
            ViewBag.StartPoint = null;
            ViewBag.TourProduct = null;
            ViewBag.StartPoint = null;
            ViewBag.IsOrderManual = false;
            ViewBag.IsSelfDesigned = false;
            ViewBag.AllowToEdit = true;
            ViewBag.IsLock = false;
            try
            {
                if (order_id > 0)
                {
                    long _UserId = 0;
                    if (HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) != null)
                    {
                        _UserId = Convert.ToInt64(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                    }
                   
                    ViewBag.User = new UserESViewModel();
                    var user = await _userRepository.GetChiefofDepartmentByServiceType((int)ServiceType.BOOK_HOTEL_ROOM_VIN);
                    if (user != null && user.Id>0)
                    {
                        ViewBag.User = new UserESViewModel()
                        {
                            email = user.Email,
                            fullname = user.FullName,
                            id = user.Id,
                            phone = user.Phone,
                            username = user.UserName,
                            _id = user.Id
                        };
                    }

                    int tour_status = (int)ServiceStatus.OnExcution;
                    var tour = await _tourRepository.GetTourById(tour_id);
                    if (tour != null && tour.Id > 0)
                    {
                        tour_status = (int)tour.Status;
                        var user_by_booking = await _userESRepository.GetUserByID(((int)tour.SalerId).ToString());
                        if(user_by_booking!=null && user_by_booking.id > 0)
                        {
                            ViewBag.User = user_by_booking;
                        }
                        ViewBag.Tour = tour;
                        var product = await _tourRepository.GetTourProductById(tour.TourProductId == null ? 0 : (long)tour.TourProductId);
                        if (product != null && product.Id>0)
                        {
                            ViewBag.IsSelfDesigned = product.IsSelfDesigned!=null ? product.IsSelfDesigned : false;
                            ViewBag.TourProduct = product;
                        }

                    }
                    else
                    {
                        tour_status = (int)ServiceStatus.New;

                    }
                    var order = await _orderRepository.GetOrderByID(order_id);
                    if (order != null)
                    {
                        ViewBag.IsLock = order != null && order.IsLock != null ? order.IsLock : false;
                        ViewBag.IsOrderManual = true;

                    }
                    bool is_allow_to_edit = false;
                    if (order!=null && order.OrderStatus!=null && ((order.SalerId != null && order.SalerId == _UserId) || (order.SalerGroupId != null && order.SalerGroupId.Contains(_UserId.ToString()))))
                    {
                        if (!list_order_status_not_allow_to_edit.Contains((int)order.OrderStatus) && !list_service_status_not_allow_to_edit.Contains(tour!=null && tour.Status!=null ? ((int)tour.Status):(int)ServiceStatus.OnExcution))
                        {
                            is_allow_to_edit = true;
                        }
                        if (tour == null || (int)tour.Status == (int)ServiceStatus.Decline)
                        {
                            is_allow_to_edit = true;
                        }
                    }
                    ViewBag.AllowToEdit = is_allow_to_edit;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("AddTourService - OrderManualController: " + ex.ToString());
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddTourServicePackages(long tour_id)
        {
            ViewBag.TourPackages = null;
            var list_default_code= new List<string>() { PackagesConstant.Adult, PackagesConstant.Child, PackagesConstant.Infant };
            ViewBag.DefaultPackagesCode = list_default_code;
            ViewBag.TourPassengerPackage = new List<TourPackages>();
            ViewBag.TourExtraPackages = new List<TourPackages>();
            try
            {
                var list= await _tourRepository.GetTourPackagesByTourId(tour_id);
                var Adult = list.Where(x => x.PackageCode == PackagesConstant.Adult).FirstOrDefault();
                var Child = list.Where(x => x.PackageCode == PackagesConstant.Child).FirstOrDefault();
                var Infant = list.Where(x => x.PackageCode == PackagesConstant.Infant).FirstOrDefault();
                ViewBag.Adult = Adult;
                ViewBag.Child = Child;
                ViewBag.Infant = Infant;
                ViewBag.TourExtraPackages = list.Where(x=>!list_default_code.Contains(x.PackageCode)).ToList();
                ViewBag.List = list;
                double base_price = 0;
                double amount = 0;
                double profit = 0;
                if (Adult != null && Adult.Id > 0)
                {
                    base_price += (double)Adult.BasePrice;
                    amount += (double)Adult.Amount;
                    profit += (double)Adult.Profit;
                }
                if (Child != null && Child.Id > 0)
                {
                    base_price += (double)Child.BasePrice;
                    amount += (double)Child.Amount;
                    profit += (double)Child.Profit;

                }
                if (Infant != null && Infant.Id > 0)
                {
                    base_price += (double)Infant.BasePrice;
                    amount += (double)Infant.Amount;
                    profit += (double)Infant.Profit;
                }
                ViewBag.BasePrice = base_price;
                ViewBag.Amount = amount;
                ViewBag.Profit = profit;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("AddTourServicePackages - OrderManualController: " + ex.ToString());
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddTourServiceTourists(long tour_id)
        {
            ViewBag.TourGuest = null;

            try
            {
                ViewBag.TourGuest = await _tourRepository.GetTourGuestsByTourId(tour_id);


            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("AddTourServiceTourists - OrderManualController: " + ex.ToString());
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SummitTourServiceData(OrderManualTourBookingServiceSummitModel data)
        {

            try
            {
                //-- Check if order is manual Order:
                var exists_order = await _orderRepository.GetOrderByID(data.order_id);

                if (data.extra_packages == null || data.extra_packages.Count <= 0)
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = "Dữ liệu gửi lên không chính xác, vui lòng kiểm tra lại."
                    });
                }
               
                int _UserId = 0;
                if (HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) != null)
                {
                    _UserId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                data.user_summit = (int)_UserId;
               
                var client = await _clientRepository.GetClientDetailByClientId((long)exists_order.ClientId);
                data.client_type = (int)client.ClientType;
                if (data.tour_id <= 0 || data.service_code == null || data.service_code.Trim() == "")
                {
                    data.service_code = await indentiferService.GetServiceCodeByType((int)ServicesType.Tourist);
                }


                #region Check Client Debt:
                long id = 0;
                double total_amount = 0;
                int tour_status = (int)ServiceStatus.OnExcution;
                if (data.tour_id <= 0)
                {
                    total_amount += data.extra_packages.Sum(x => x.amount);
                    tour_status = (int)ServiceStatus.New;
                }
                else
                {
                    var exists_tour = await _tourRepository.GetTourById(data.tour_id);
                    tour_status = (int)exists_tour.Status;
                    total_amount += data.extra_packages.Sum(x => x.amount);
                    double total_exists_tour_amount = (double)exists_tour.Amount;
                    total_amount -= total_exists_tour_amount;
                }
                bool is_allow_to_edit = false;
                if (exists_order != null && exists_order.OrderStatus != null && ((exists_order.SalerId != null && exists_order.SalerId == _UserId) || (exists_order.SalerGroupId != null && exists_order.SalerGroupId.Contains(_UserId.ToString()))))
                {
                    if (!list_order_status_not_allow_to_edit.Contains((int)exists_order.OrderStatus) && !list_service_status_not_allow_to_edit.Contains(tour_status))
                    {
                        is_allow_to_edit = true;
                    }
                    if ((int)tour_status == (int)ServiceStatus.Decline)
                    {
                        is_allow_to_edit = true;
                    }
                }
                if (!is_allow_to_edit)
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = "Bạn không có quyền chỉnh sửa dịch vụ này."
                    });
                }
                int is_debt_able = await _orderRepository.IsClientAllowedToDebtNewService(total_amount, (long)exists_order.ClientId, exists_order.OrderId, (int)ServiceType.Tour);
                /*
                if (client != null && client.ClientType == (int)ClientType.kl)
                {
                    //-- Update Booking
                    id = await _tourRepository.SummitTourServiceData(data, _UserId);
                }
                else if (client != null)
                {
                    double total_amount = 0;
                    string msg = "Giá trị dịch vụ thêm mới trong đơn hàng vượt quá hạn mức công nợ còn lại của khách hàng. Bạn không thể thêm dịch vụ này";
                    if (data.tour_id <= 0)
                    {
                        total_amount += data.extra_packages.Sum(x => x.amount);
                    }
                    else
                    {
                        var exists_tour = await _tourRepository.GetDetailTourByID(data.tour_id);
                        total_amount += data.extra_packages.Sum(x => x.amount);
                        double total_exists_tour_amount = (double)exists_tour.Amount;
                        total_amount -= total_exists_tour_amount;
                        msg = "Giá trị dịch vụ cập nhật thêm vượt quá hạn mức công nợ còn lại của khách hàng. Bạn không thể cập nhật dịch vụ này";
                    }
                    int is_debt_able = await _orderRepository.IsClientAllowedToDebtNewService(total_amount, (long)exists_order.ClientId, exists_order.OrderId, (int)ServiceType.Tour);
                    if (is_debt_able == (int)DebtType.DEBT_ACCEPTED)
                    {
                        //-- Update Booking
                        id = await _tourRepository.SummitTourServiceData(data, _UserId);
                    }
                    else
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = msg
                        });
                    }
                }
                else
                {
                    LogHelper.InsertLogTelegram("SummitTourServiceData - OrderManualController: Client = " + exists_order.ClientId == null ? "NULL" : exists_order.ClientId.ToString() + " in OrderID = " + exists_order.OrderId);

                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = "Thông tin đơn hàng không chính xác, vui lòng liên hệ IT"
                    });
                }*/
                #endregion
                //-- Update Booking
                id = await _tourRepository.SummitTourServiceData(data, _UserId, is_debt_able);
                if (id <= 0)
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = "Thêm mới / Cập nhật dịch vụ tour thất bại, vui lòng liên hệ IT"
                    });
                }
                //-- Notify
               

                #region Update Order Amount:
               
                await _orderRepository.UpdateOrderDetail(data.order_id, _UserId);
                await _orderRepository.ReCheckandUpdateOrderPayment(data.order_id);

                #endregion
                return Ok(new
                {
                    status = (int)ResponseType.SUCCESS,
                    msg = "Thêm mới / Cập nhật dịch vụ tour thành công",
                    data=id
                });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SummitTourServiceData - OrderManualController: " + ex.ToString());
            }
            return Ok(new
            {
                status = (int)ResponseType.FAILED,
                msg = "Thêm mới / Cập nhật dịch vụ tour thất bại, vui lòng liên hệ IT"
            });

        }
        [HttpPost]
        public async Task<IActionResult> GetTourProductDetail(long id)
        {

            try
            {
                var tour_product = await _tourRepository.GetTourProductById(id);
                if (tour_product == null || tour_product.Id <= 0)
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED
                    });
                }
                var tour_destination = await _tourRepository.GetTourDestinationByTourProductId(tour_product.Id);
                List<OrderManualTourProductEndpoint> endpoint_name = new List<OrderManualTourProductEndpoint>();
                string start_point_name = null;
                if (tour_destination != null && tour_destination.Count > 0)
                {
                    switch (tour_product.TourType)
                    {
                        case (int)TourType.Noi_Dia:
                            {
                                var start = await _provinceRedisService.GetProvicedById((int)tour_product.StartPoint);
                                if (start != null)
                                {
                                    start_point_name = start.Name;
                                }
                                foreach (var p in tour_destination)
                                {
                                    var province = await _provinceRedisService.GetProvicedById((int)p.LocationId);
                                    if (province != null)
                                    {
                                        endpoint_name.Add(new OrderManualTourProductEndpoint()
                                        {
                                            id = province.Id,
                                            endpoint_name = province.Name
                                        });
                                    }
                                }
                            }
                            break;
                        case (int)TourType.In_bound:
                            {
                                var start = await _nationalESRepository.GetNationalByID(tour_product.StartPoint.ToString());
                                if (start != null)
                                {
                                    start_point_name = start.name;
                                }
                                foreach (var p in tour_destination)
                                {
                                    var province = await _provinceRedisService.GetProvicedById((int)p.LocationId);
                                    if (province != null)
                                    {
                                        endpoint_name.Add(new OrderManualTourProductEndpoint()
                                        {
                                            id = province.Id,
                                            endpoint_name = province.Name
                                        });
                                    }
                                   
                                }
                            }
                            break;
                        case (int)TourType.Out_bound:
                            {
                                var start = await _provinceRedisService.GetProvicedById((int)tour_product.StartPoint);
                                if (start != null)
                                {
                                    start_point_name = start.Name;
                                }
                                foreach (var n in tour_destination)
                                {
                                    var province = await _nationalESRepository.GetNationalByID(n.LocationId.ToString());
                                    if (province != null)
                                    {
                                        endpoint_name.Add(new OrderManualTourProductEndpoint()
                                        {
                                            id = province.id,
                                            endpoint_name = province.name
                                        });
                                    }
                                    
                                }
                            }
                            break;
                    }
                }
                var data = JsonConvert.DeserializeObject<OrderManualTourProductViewModel>(JsonConvert.SerializeObject(tour_product));
                data.end_point_name = endpoint_name;
                data.start_point_name = start_point_name;
                try
                {
                    data.start_date = data.DateDeparture == null || data.DateDeparture.Trim() == "" ? DateTime.Now : Convert.ToDateTime(data.DateDeparture.Split(",")[0]);
                }
                catch { data.start_date = DateTime.Now; }
                data.end_date = data.start_date.AddDays((int)data.Days);

                return Ok(new
                {
                    status = (int)ResponseType.SUCCESS,
                    data = data
                });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetTourProductDetail - OrderManualController: " + ex);
                return Ok(new
                {
                    status = (int)ResponseType.SUCCESS,
                    data = new TourProduct()
                });
            }

        }
        [HttpPost]
        public async Task<IActionResult> ExistsTourSuggesstion(string txt_search)
        {

            try
            {
                var data = await _tourRepository.TourProductSuggesstion(txt_search);
                return Ok(new
                {
                    status = (int)ResponseType.SUCCESS,
                    data = data
                });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ExistsTourSuggesstion - OrderManualController: " + ex.ToString());
                return Ok(new
                {
                    status = (int)ResponseType.SUCCESS,
                    data = new List<NationalESViewModel>()
                });
            }

        }
        [HttpPost]
        public async Task<IActionResult> NationalSuggestion(string txt_search)
        {

            try
            {
                var data = await _nationalESRepository.SearchNational(txt_search);
                return Ok(new
                {
                    status = (int)ResponseType.SUCCESS,
                    data = data
                });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("NationalSuggestion - OrderManualController: " + ex.ToString());
                return Ok(new
                {
                    status = (int)ResponseType.SUCCESS,
                    data = new List<NationalESViewModel>()
                });
            }

        }
        [HttpPost]
        public async Task<IActionResult> ProvinceSuggestion(string txt_search)
        {

            try
            {
                var data = await _provinceRedisService.SearchProvince(txt_search);
                if(data==null || data.Count <= 0)
                {
                    data = await _provinceRepository.GetProvincesList();
                    await _provinceRedisService.SetProvinces(data);
                }
                return Ok(new
                {
                    status = (int)ResponseType.SUCCESS,
                    data = data
                });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ProvinceSuggestion - OrderManualController: " + ex.ToString());
                return Ok(new
                {
                    status = (int)ResponseType.SUCCESS,
                    data = new List<Province>()
                });
            }

        }
        [HttpPost]
        public async Task<IActionResult> ChangeOrderSaler(long? order_id,int saleid,string OrderNo)
        {

            try
            {
                var model = new LogActionModel();
                model.Type = (int)AttachmentType.OrderDetail;
                model.LogId = (long)order_id;
                if (order_id == null || order_id <= 0)
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = "Dữ liệu gửi lên không chính xác, vui lòng kiểm tra lại"
                    });
                }
                int _UserId = 0;
                if (HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) != null)
                {
                    _UserId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                if (saleid != 0) _UserId = _UserId = saleid;
                 var success = await _orderRepository.UpdateOrderSaler((long)order_id, _UserId);
                var current_user = _ManagementUser.GetCurrentUser();
                string link = "/Order/" + order_id;   
                apiService.SendMessage( _UserId.ToString(), ((int)ModuleType.DON_HANG).ToString(), ((int)ActionType.NHAN_TRIEN_KHAI).ToString(), OrderNo, link, current_user.Role);
                var user = await _userRepository.GetById(_UserId);
                model.Log = "Nhận triển khai";
                model.Note = user.FullName + " nhận xử lý đơn hàng ";
                model.CreatedUserName = user.FullName;
                LogActionMongo.InsertLog(model);
                return Ok(new
                {
                    status = (int)ResponseType.SUCCESS,
                    msg = "Đổi điều hành viên thành công"
                });

            }
            catch
            {
                return Ok(new
                {
                    status = (int)ResponseType.ERROR,
                    msg = "Đổi điều hành viên thất bại, vui lòng liên hệ IT"
                });
            }
        }
        [HttpPost]
        public async Task<IActionResult> GetActiveContractByClientId(long client_id)
        {

            try
            {
                string msg = "Khách hàng chưa được kích hoạt";
                if ( client_id <= 0)
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        detail="",
                        msg,

                        
                    });
                }
                var client = await _clientRepository.GetClientDetailByClientId(client_id);
                if (client != null && client.Id > 0 && client.ClientType!=null && client.ClientType>0) 
                {
                    switch ((int)client.ClientType){
                        case (int)ClientTypeEnum.DALC1:
                        case (int)ClientTypeEnum.DALC2:
                        case (int)ClientTypeEnum.DLC3:
                        case (int)ClientTypeEnum.DL:
                        case (int)ClientTypeEnum.DN:
                        case (int)ClientTypeEnum.CTV:
                            {
                                var contract = await _contractRepository.GetActiveContractByClientId(client_id);
                                if (contract != null && contract.ContractId > 0)
                                {
                                    return Ok(new
                                    {
                                        status = (int)ResponseType.SUCCESS,
                                        detail = "[B2B] " + client.ClientName + "(" + client.Phone + " - " + client.Email + ")",
                                        msg = ""
                                    });
                                }
                                else
                                {
                                    return Ok(new
                                    {
                                        status = (int)ResponseType.FAILED,
                                        detail = client.ClientName + "(" + client.Phone + " - " + client.Email + ")",
                                        msg = "[B2B] Khách hàng " + client.ClientName + " (" + client.Email + ") chưa có / chưa được xét duyệt hợp đồng",
                                    });
                                }
                            }
                        case (int)ClientTypeEnum.kl:
                            {
                                return Ok(new
                                {
                                    status = (int)ResponseType.SUCCESS,
                                    detail = "[B2C] " + client.ClientName + "(" + client.Phone + " - " + client.Email + ")",
                                    msg = ""
                                });
                            }
                        default:
                            {
                                return Ok(new
                                {
                                    status = (int)ResponseType.SUCCESS,
                                    detail = "[] " + client.ClientName + "(" + client.Phone + " - " + client.Email + ")",
                                    msg = ""
                                });
                            }
                    }
                   
                }
                else
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        detail = "Khách hàng không tồn tại / chưa được phân loại, vui lòng chọn khách hàng khác",
                        msg = ""
                    });
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetActiveContractByClientId - OrderManualController: " + ex.ToString());
                return Ok(new
                {
                    status = (int)ResponseType.ERROR,
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetGuestFromFile(IFormFile file, int service_type)
        {

            try
            {
                var stream = new MemoryStream();
                await file.CopyToAsync(stream);
                var file_name = file.FileName.Split(".");
                if(!file_name[file_name.Length-1].ToLower().Contains("xls")&& !file_name[file_name.Length - 1].ToLower().Contains("xlsx"))
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = "File upload ko đúng định dạng. File upload chỉ chấp nhận file excel có định dạng xls, xlsx"
                    });
                }
                var package = new ExcelPackage(stream);
                ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault();

                var Cells = worksheet.Cells;

                int startRow = 3;
                int endRow = worksheet.Dimension.Rows;

                switch (service_type)
                {
                    case (int)ServiceType.BOOK_HOTEL_ROOM:
                    case (int)ServiceType.BOOK_HOTEL_ROOM_VIN:
                        {
                            List<object> passengers = new List<object>();
                            for (int row = startRow; row <= endRow; row++)
                            {
                                passengers.Add(new 
                                {
                                    Name = Cells[row, 1].Value,
                                    Birthday = Cells[row, 2].Value,
                                    Note = Cells[row, 3].Value
                                });
                            }
                            return Ok(new
                            {
                                status = (int)ResponseType.SUCCESS,
                                msg = "Import thành công",
                                data = passengers
                            });
                        }
                    case (int)ServiceType.PRODUCT_FLY_TICKET:
                        {
                            List<object> passengers = new List<object>();
                            for (int row = startRow; row <= endRow; row++)
                            {
                               
                              
                                passengers.Add(new 
                                {
                                    Gender = Cells[row, 1].Value,
                                    Name = Cells[row, 2].Value,
                                    Birthday = Cells[row, 3].Value,
                                    Note = Cells[row, 4].Value,
                                });
                            }
                            return Ok(new
                            {
                                status = (int)ResponseType.SUCCESS,
                                msg = "Import thành công",
                                data = passengers
                            });
                        }
                    case (int)ServiceType.Tour:
                        {
                            List<object> passengers = new List<object>();
                            for (int row = startRow; row <= endRow; row++)
                            {
                               
                                passengers.Add(new
                                {
                                    FullName = Cells[row, 1].Value,
                                    Gender = Cells[row, 2].Value,
                                    Birthday = Cells[row, 3].Value,
                                    Cccd = Cells[row, 4].Value,
                                    RoomNumber = Cells[row, 5].Value,
                                    Note = Cells[row, 6].Value,
                                });
                            }
                            return Ok(new
                            {
                                status = (int)ResponseType.SUCCESS,
                                msg = "Import thành công",
                                data = passengers
                            });
                        }
                    case (int)ServiceType.VinWonder:
                        {
                            List<object> passengers = new List<object>();
                            for (int row = startRow; row <= endRow; row++)
                            {
                               
                                passengers.Add(new
                                {
                                    FullName = Cells[row, 1].Value,
                                    Email = Cells[row, 2].Value,
                                    Phone = Cells[row, 3].Value,
                                    Note= Cells[row, 4].Value,
                                });
                            }
                            return Ok(new
                            {
                                status = (int)ResponseType.SUCCESS,
                                msg = "Import thành công",
                                data = passengers
                            });
                        }
                    default:
                        {
                            return Ok(new
                            {
                                status = (int)ResponseType.FAILED,
                                msg = "File upload ko đúng định dạng. File upload chỉ chấp nhận file excel có định dạng xls, xlsx"
                            });
                        }
                }


            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = (int)ResponseType.ERROR
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddOtherService(long order_id, long other_booking_id)
        {
            try
            {
                ViewBag.Service = null;
                ViewBag.User = null;
                ViewBag.Booking = null;
                ViewBag.IsOrderManual = false;
                ViewBag.AllowToEdit = false;
                var all_service = _allCodeRepository.GetListByType(AllCodeType.SERVICE_TYPE_OTHER);
                var all_service_main = _allCodeRepository.GetListByType(AllCodeType.SERVICE_TYPE_OTHER_MAIN).Select(x=>x.CodeValue);
                if(all_service_main!=null && all_service_main.Count() > 0)
                {
                    all_service = all_service.Where(x => !all_service_main.Contains(x.CodeValue)).ToList();
                }
                ViewBag.AllService = all_service;
                if (order_id > 0)
                {
                    long _UserId = 0;
                    if (HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) != null)
                    {
                        _UserId = Convert.ToInt64(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                    }


                    var order = await _orderRepository.GetOrderByID(order_id);
                    if (order != null )
                    {
                        ViewBag.IsLock = order != null && order.IsLock != null ? order.IsLock : false;
                        ViewBag.IsOrderManual = true;
                    }
                    var other_booking = await _otherBookingRepository.GetOtherBookingById(other_booking_id);
                    bool is_allow_to_edit = false;
                    if (other_booking!=null && other_booking.Id>0)
                    {
                        ViewBag.Service = all_service.FirstOrDefault(x => x.CodeValue == other_booking.ServiceType);
                        ViewBag.Booking = other_booking;
                        var user_by_booking = await _userESRepository.GetUserByID(((int)other_booking.OperatorId).ToString());
                        if (user_by_booking != null && user_by_booking.id > 0)
                        {
                            ViewBag.User = user_by_booking;
                        }

                        if ((order.SalerId != null && order.SalerId == _UserId) || (order.SalerGroupId != null && order.SalerGroupId.Contains(_UserId.ToString())))
                        {
                            if (!list_order_status_not_allow_to_edit.Contains((int)order.OrderStatus) && !list_service_status_not_allow_to_edit.Contains((int)other_booking.Status))
                            {
                                is_allow_to_edit = true;
                            }
                            if ((int)other_booking.Status == (int)ServiceStatus.Decline)
                            {
                                is_allow_to_edit = true;
                            }
                        }
                    }
                    else
                    {
                        is_allow_to_edit = true;
                    }
                    ViewBag.AllowToEdit = is_allow_to_edit;
                }

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("AddOtherService - OrderManualController: " + ex.ToString());
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddOtherServicePackages(long other_booking_id)
        {
            try
            {
                ViewBag.ExtraList = new List<OtherBookingPackages>();
                if (other_booking_id > 0)
                {
                    var list = await _otherBookingRepository.GetOtherBookingPackagesByBookingId(other_booking_id);
                    if (list != null)
                    {
                        ViewBag.ExtraList = list;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("AddOtherServicePackages - OrderManualController: " + ex.ToString());
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SummitOtherServicePackages(OrderManualOtherBookingServiceSummitModel data)
        {
            try
            {
                int _UserId = 0;
                if (HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) != null)
                {
                    _UserId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                if (data.from_date >= data.to_date )
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = "Thời gian bắt đầu không được lớn hơn hoặc bằng thời gian kết thúc"
                    });
                }
                if (data.order_id<=0|| data.service_type <= 0)
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = "Dữ liệu gửi lên không chính xác, vui lòng kiểm tra lại."
                    });
                }
                if (data.service_code == null || data.service_code.Trim() == "")
                {
                    data.service_code = await indentiferService.GetServiceCodeByType((int)ServiceType.Other);
                }
                var exists_order = await _orderRepository.GetOrderByID(data.order_id);
                int service_status = (int)ServiceStatus.OnExcution;
                double total_amount = 0;
                if (data.id <= 0)
                {
                    service_status = (int)ServiceStatus.New;
                    total_amount += data.packages.Sum(x=>x.amount);
                }
                else
                {
                    var otherBooking = await _otherBookingRepository.GetOtherBookingById(data.id);
                    service_status = otherBooking.Status;
                    if(data.packages!=null && data.packages.Count > 0)
                    {
                        total_amount += data.packages.Sum(x => x.amount);
                    }
                    double total_exists_other_amount = otherBooking.Amount;
                    total_amount -= total_exists_other_amount;

                }
                bool is_allow_to_edit = false;
                if (exists_order != null && exists_order.OrderStatus != null && ((exists_order.SalerId != null && exists_order.SalerId == _UserId) || (exists_order.SalerGroupId != null && exists_order.SalerGroupId.Contains(_UserId.ToString()))))
                {
                    if (!list_order_status_not_allow_to_edit.Contains((int)exists_order.OrderStatus) && !list_service_status_not_allow_to_edit.Contains(service_status))
                    {
                        is_allow_to_edit = true;
                    }
                    if ((int)service_status == (int)ServiceStatus.Decline)
                    {
                        is_allow_to_edit = true;
                    }
                }
                if (!is_allow_to_edit)
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = "Bạn không có quyền chỉnh sửa dịch vụ này."
                    });
                }
                var id = await _otherBookingRepository.SummitOtherBooking(data, _UserId);
                #region Update Order Amount:
                await _orderRepository.UpdateOrderDetail(data.order_id, _UserId);
                await _orderRepository.ReCheckandUpdateOrderPayment(data.order_id);

                #endregion

               

                return Ok(new
                {
                    status = (int)ResponseType.SUCCESS,
                    msg = "Thêm mới / Cập nhật dịch vụ thành công",
                    data=id
                });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SummitOtherServicePackages - OrderManualController: " + ex.ToString());
            }
            return Ok(new
            {
                status = (int)ResponseType.FAILED,
                msg = "Thêm mới / Cập nhật dịch vụ thất bại, vui lòng liên hệ IT"
            });
        }
        public async Task<IActionResult> AddVinWonderService(long order_id, long booking_id)
        {
            try
            {
                ViewBag.Service = null;
                ViewBag.User = null;
                ViewBag.Booking = null;
                ViewBag.IsOrderManual = false;
                ViewBag.AllowToEdit = false;

                var all_service = _allCodeRepository.GetListByType(AllCodeType.SERVICE_TYPE);
                if (order_id > 0)
                {
                    long _UserId = 0;
                    if (HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) != null)
                    {
                        _UserId = Convert.ToInt64(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                    }


                    var order = await _orderRepository.GetOrderByID(order_id);
                    if (order != null)
                    {
                        ViewBag.IsLock = order != null && order.IsLock != null ? order.IsLock : false;
                        ViewBag.IsOrderManual = true;
                    }
                    var vinwonder_booking =  _vinWonderBookingRepository.GetVinWonderBookingById(booking_id);
                    bool is_allow_to_edit = false;
                    if (vinwonder_booking != null && vinwonder_booking.Id > 0)
                    {
                        ViewBag.Booking = vinwonder_booking;
                       var user_by_booking = await _userESRepository.GetUserByID(((int)vinwonder_booking.SalerId).ToString());
                        if (user_by_booking != null && user_by_booking.id > 0)
                        {
                            ViewBag.User = user_by_booking;
                        }

                        if ((order.SalerId != null && order.SalerId == _UserId) || (order.SalerGroupId != null && order.SalerGroupId.Contains(_UserId.ToString())))
                        {
                            if (!list_order_status_not_allow_to_edit.Contains((int)order.OrderStatus) && !list_service_status_not_allow_to_edit.Contains((int)vinwonder_booking.Status))
                            {
                                is_allow_to_edit = true;
                            }
                            if ((int)vinwonder_booking.Status == (int)ServiceStatus.Decline)
                            {
                                is_allow_to_edit = true;
                            }
                        }
                    }
                    else
                    {
                        is_allow_to_edit = true;
                    }
                    ViewBag.AllowToEdit = is_allow_to_edit;
                }

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("AddvinwonderService - OrderManualController: " + ex.ToString());
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddVinWonderServicePackages(long booking_id)
        {
            try
            {
                ViewBag.ExtraList = new List<VinWonderBookingTicket>();
                if (booking_id > 0)
                {
                    var list = await _vinWonderBookingRepository.GetVinWonderTicketByBookingIdSP(booking_id);
                    if (list != null)
                    {
                        ViewBag.ExtraList = list;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("AddVinWonderServicePackages - OrderManualController: " + ex.ToString());
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddVinWonderServiceGuests(long booking_id)
        {
            try
            {
                ViewBag.Guest = new List<VinWonderBookingTicketCustomer>();
                if (booking_id > 0)
                {
                    var list = await _vinWonderBookingRepository.GetVinWonderTicketCustomerByBookingIdSP(booking_id);
                    if (list != null)
                    {
                        ViewBag.Guest = list;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("AddVinWonderServicePackages - OrderManualController: " + ex.ToString());
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> VinWonderLocationSuggestion(string txt_search)
        {

            try
            {
                long _UserId = 0;
                if (HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) != null)
                {
                    _UserId = Convert.ToInt64(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                if (txt_search == null) txt_search = "";
                var data = await _groupProductRepository.GetProductGroupByParentID(ReadFile.LoadConfig().Vinwonder_groupproduct_parentID, ReadFile.LoadConfig().STATIC_IMAGE_DOMAIN);
                if (data == null) data = new List<ProductGroupViewModel>();
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
        public async Task<IActionResult> SummitVinWonderServicePackages(OrderManualVinWonderBookingServiceSummitModel data)
        {
            try
            {
                int _UserId = 0;
                if (HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) != null)
                {
                    _UserId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                
                if (data.order_id <= 0 || data.location_id <= 0)
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = "Dữ liệu gửi lên không chính xác, vui lòng kiểm tra lại."
                    });
                }
                if (data.service_code == null || data.service_code.Trim() == "")
                {
                   // data.service_code = await indentiferService.GetServiceCodeByType((int)ServiceType.VinWonder);
                    data.service_code = "";
                }
                var exists_order = await _orderRepository.GetOrderByID(data.order_id);
                int service_status = (int)ServiceStatus.OnExcution;
                double total_amount = 0;
                if (data.id <= 0)
                {
                    service_status = (int)ServiceStatus.New;
                    total_amount += data.packages.Sum(x => x.amount);
                }
                else
                {
                    var vinWonderBooking =  _vinWonderBookingRepository.GetVinWonderBookingById(data.id);
                    service_status = (int)vinWonderBooking.Status;
                    total_amount += data.packages.Sum(x => x.amount);
                    double total_exists_other_amount = (double)vinWonderBooking.Amount;
                    total_amount -= total_exists_other_amount;
                    vinWonderBooking.Commission = vinWonderBooking.Commission;
                    vinWonderBooking.OthersAmount = vinWonderBooking.OthersAmount;
                }
                bool is_allow_to_edit = false;
                if (exists_order != null && exists_order.OrderStatus != null && ((exists_order.SalerId != null && exists_order.SalerId == _UserId) || (exists_order.SalerGroupId != null && exists_order.SalerGroupId.Contains(_UserId.ToString()))))
                {
                    if (!list_order_status_not_allow_to_edit.Contains((int)exists_order.OrderStatus) && !list_service_status_not_allow_to_edit.Contains(service_status))
                    {
                        is_allow_to_edit = true;
                    }
                    if ((int)service_status == (int)ServiceStatus.Decline)
                    {
                        is_allow_to_edit = true;
                    }
                }
                if (!is_allow_to_edit)
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = "Bạn không có quyền chỉnh sửa dịch vụ này."
                    });
                }
                var id = await _vinWonderBookingRepository.SummitVinWonderServiceData(data, _UserId);

                #region Update Order Amount:
                await _orderRepository.UpdateOrderDetail(data.order_id, _UserId);
                await _orderRepository.ReCheckandUpdateOrderPayment(data.order_id);
                #endregion

                return Ok(new
                {
                    status = (int)ResponseType.SUCCESS,
                    msg = "Thêm mới / Cập nhật dịch vụ thành công",
                    data=id
                });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SummitVinWonderServicePackages - OrderManualController: " + ex.ToString());
            }
            return Ok(new
            {
                status = (int)ResponseType.FAILED,
                msg = "Thêm mới / Cập nhật dịch vụ thất bại, vui lòng liên hệ IT"
            });
        }
       
    }
}
