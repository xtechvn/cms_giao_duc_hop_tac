using Caching.Elasticsearch;
using Entities.Models;
using Entities.ViewModels;
using Entities.ViewModels.ElasticSearch;
using Entities.ViewModels.OrderManual;
using Entities.ViewModels.Request;
using Microsoft.AspNetCore.Mvc;
using Repositories.IRepositories;
using System.Security.Claims;
using Utilities;
using Utilities.Contants;
using GDHT.CMS.Service;

namespace GDHT.CMS.Controllers.Order
{
    public class RequestHotelBookingController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly IRequestRepository _requestRepository;
        private readonly IHotelBookingRepositories _hotelBookingRepository;
        private UserESRepository _userESRepository;
        private HotelESRepository _hotelESRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IndentiferService indentiferService;
        private readonly IHotelRepository _hotelRepository;
        public RequestHotelBookingController(IConfiguration configuration, IUserRepository userRepository, IHotelBookingRepositories hotelBookingRepository,
            IRequestRepository requestRepository, IClientRepository clientRepository, IOrderRepository orderRepository, IHotelRepository hotelRepository)
        {
            _configuration = configuration;
            _userRepository = userRepository;
            _hotelBookingRepository = hotelBookingRepository;
            _userESRepository = new UserESRepository(_configuration["DataBaseConfig:Elastic:Host"]);
            _hotelESRepository = new HotelESRepository(_configuration["DataBaseConfig:Elastic:Host"]);
            _requestRepository = requestRepository;
            _clientRepository = clientRepository;
            _orderRepository = orderRepository;
            indentiferService = new IndentiferService(configuration);
            _hotelRepository = hotelRepository;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Search(RequestSearchModel searchModel)
        {
            try
            {
                var data = await _requestRepository.GetPagingList(searchModel);
                return PartialView(data);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Search - RequestHotelBookingController: " + ex);
            }
            return PartialView();

        }
        public async Task<IActionResult> Detail(long hotel_booking_id, long ClientId,long id)
        {
            try
            {
                if(id!= 0)
                {
                    ViewBag.status = 0;
                    var detail =await _requestRepository.GetDetailRequest(id);
                    if (detail != null)
                    {
                        ViewBag.status = detail.Status;
                    }
                }
                ViewBag.id = id;
                if (ClientId != 0)
                {
                    var UserCreateclient = await _clientRepository.GetClientDetailByClientId(ClientId);
                    if (UserCreateclient != null)
                    {
                        ViewBag.client = UserCreateclient;
                    }
                }
               
                long _UserId = 0;
                if (HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) != null)
                {
                    _UserId = Convert.ToInt64(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                ViewBag.User = new UserESViewModel();
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
                ViewBag.IsOrderManual = true;
                ViewBag.AllowToEdit = true;
                ViewBag.HotelBooking = new HotelBooking();
                ViewBag.Hotel = new HotelESViewModel();
                var booking = await _hotelBookingRepository.GetHotelBookingByID(hotel_booking_id);
                if (booking != null && booking.Id > 0)
                {
                    ViewBag.IsOrderManual = false;
                    ViewBag.HotelBooking = booking;
                    var hotel = await _hotelESRepository.GetHotelByID(booking.PropertyId);
                    if (hotel == null) hotel = new HotelESViewModel();
                    ViewBag.Hotel = hotel;
                    var user_by_booking = await _userESRepository.GetUserByID(booking.SalerId.ToString());
                    if (user_by_booking != null && user_by_booking.id > 0)
                    {
                        ViewBag.User = user_by_booking;
                    }
                    ViewBag.IsOrderManual = false;
                    //if (order != null)
                    //{
                    //    ViewBag.IsOrderManual = true;
                    //}
                    bool is_allow_to_edit = false;
                    if (booking.SalerId != null && booking.SalerId == _UserId)
                    {
                        is_allow_to_edit = true;

                    }
                    ViewBag.AllowToEdit = is_allow_to_edit;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Detail - RequestHotelBookingController: " + ex);
            }
            return View();
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
                    total_amount += data.rooms.Sum(x => x.package.Sum(x => x.amount));
                    total_amount += data.extra_package != null && data.extra_package.Count > 0 ? data.extra_package.Sum(x => x.amount) : 0;
                }
                else
                {
                    var exists_hotel = await _hotelBookingRepository.GetHotelBookingByID(data.hotel.id);
                    service_status = (int)exists_hotel.Status;
                    total_amount += data.rooms.Sum(x => x.package.Sum(x => x.amount));
                    total_amount += data.extra_package != null && data.extra_package.Count > 0 ? data.extra_package.Sum(x => x.amount) : 0;
                    total_amount -= exists_hotel.TotalAmount;
                }
              
                int is_debt_able = await _orderRepository.IsClientAllowedToDebtNewService(total_amount, (long)exists_order.ClientId, exists_order.OrderId, (int)ServiceType.BOOK_HOTEL_ROOM_VIN);
         
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
                    data = id
                });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SummitHotelServiceData - RequestHotelBookingController: " + ex.ToString());
            }
            return Ok(new
            {
                status = (int)ResponseType.FAILED,
                msg = "Thêm mới / Cập nhật dịch vụ khách sạn thất bại, vui lòng liên hệ IT"
            });

        }
        public async Task<IActionResult> UpdateStatus(int type, int id)
        {
            var status = (int)ResponseType.FAILED;
            var msg = "Không thành công";
            try
            {
                var model = new Request();
                model.RequestId = id;
                model.Status = type;
                model.UpdatedBy = (int?)Convert.ToInt64(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var update =await _requestRepository.UpdateRequest(model);
                if (update > 0)
                {
                    status = (int)ResponseType.FAILED;
                    msg = "xác nhận thành công";
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateStatus - RequestHotelBookingController: " + ex.ToString());
            }
            return Ok(new
            {
                status = status,
                msg = msg
            });
        }
    }
}
