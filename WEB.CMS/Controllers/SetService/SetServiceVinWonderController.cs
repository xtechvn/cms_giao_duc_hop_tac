using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Caching.Elasticsearch;
using Entities.Models;
using Entities.ViewModels;
using Entities.ViewModels.HotelBookingCode;
using Entities.ViewModels.SetServices;
using Entities.ViewModels.VinWonder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Repositories.IRepositories;
using Utilities;
using Utilities.Contants;
using GDHT.CMS.Service;
using GDHT.CMS.Service.ServiceInterface;
using WEB.CMS.Customize;
using WEB.CMS.Models;
using static Utilities.DepositHistoryConstant;

namespace GDHT.CMS.Controllers.SetService.VinWonder
{
    public class SetServiceController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IOrderRepositor _orderRepository;
        private readonly IOrderRepository _orderRepository2;
        private readonly IClientRepository _clientRepository;
        private readonly IContactClientRepository _contactClientRepository;
        private readonly OrderESRepository _orderESRepository;
        private readonly FlyBookingESRepository _flyBookingESRepository;
        private readonly IAllCodeRepository _allCodeRepository;
        private readonly IUserRepository _userRepository;
        private UserESRepository _userESRepository;
        private IndentiferService _indentiferService;
        private IPassengerRepository _passengerRepository;
        private IAirlinesRepository _airlinesRepository;
        private readonly IPaymentRequestRepository _paymentRequestRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly IVinWonderBookingRepository _VinWonderBookingRepository;
        private ManagementUser _ManagementUser;
        private APIService apiService;
        private readonly IWebHostEnvironment _WebHostEnvironment;
        private readonly IVinWonderBookingRepository _vinWonderBookingRepository;
        private readonly IHotelBookingCodeRepository _hotelBookingCodeRepository;
        private readonly IEmailService _emailService;
        private readonly IAttachFileRepository _AttachFileRepository;
        private readonly IContractPayRepository _contractPayRepository;

        public SetServiceController(IConfiguration configuration, IOrderRepositor orderRepository, IAllCodeRepository allcodeRepository, IContactClientRepository contactClientRepository,
          IUserRepository userRepository, IPassengerRepository passengerRepository, IAirlinesRepository airlinesRepository, IVinWonderBookingRepository VinWonderBookingRepository,
          IPaymentRequestRepository paymentRequestRepository, ISupplierRepository supplierRepository, IOrderRepository orderRepository2, IWebHostEnvironment WebHostEnvironment,
          ManagementUser managementUser, IVinWonderBookingRepository vinWonderBookingRepository, IHotelBookingCodeRepository hotelBookingCodeRepository, IEmailService emailService,
          IAttachFileRepository AttachFileRepository, IClientRepository clientRepository, IContractPayRepository contractPayRepository)
        {

            _configuration = configuration;
            _orderRepository = orderRepository;
            _contactClientRepository = contactClientRepository;
            _orderESRepository = new OrderESRepository(_configuration["DataBaseConfig:Elastic:Host"]);
            _flyBookingESRepository = new FlyBookingESRepository(_configuration["DataBaseConfig:Elastic:Host"]);
            _allCodeRepository = allcodeRepository;
            _userESRepository = new UserESRepository(_configuration["DataBaseConfig:Elastic:Host"]);
            _indentiferService = new IndentiferService(configuration);
            _userRepository = userRepository;
            _passengerRepository = passengerRepository;
            _airlinesRepository = airlinesRepository;
            _paymentRequestRepository = paymentRequestRepository;
            _supplierRepository = supplierRepository;
            _orderRepository2 = orderRepository2;
            _VinWonderBookingRepository = VinWonderBookingRepository;
            _ManagementUser = managementUser;
            apiService = new APIService(configuration, userRepository);
            _WebHostEnvironment = WebHostEnvironment;
            _vinWonderBookingRepository = vinWonderBookingRepository;
            _hotelBookingCodeRepository = hotelBookingCodeRepository;
            _emailService = emailService;
            _AttachFileRepository = AttachFileRepository;
            _clientRepository = clientRepository;
            _contractPayRepository = contractPayRepository;
        }
        public IActionResult VinWonder()
        {
            try
            {
                var allcode_list = _allCodeRepository.GetListByType(AllCodeType.BOOKING_HOTEL_ROOM_STATUS);
                ViewBag.status = allcode_list.Where(x => x.CodeValue != (int)ServiceStatus.New).ToList();
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("VinWonders - SetServiceVinWonderController: " + ex);

            }
            return View();
        }
        public async Task<IActionResult> VinWonderSearch(SearchFlyBookingViewModel searchModel)
        {
            ViewBag.Model = new GenericViewModel<VinWonderBookingSearchViewModel>();

            try
            {
                if (searchModel == null) searchModel = new SearchFlyBookingViewModel();
                if (searchModel.pageSize <= 0) searchModel.pageSize = 30;
                if (searchModel.PageIndex <= 0) searchModel.PageIndex = 1;
                if (searchModel.StatusBooking == null || searchModel.StatusBooking.Trim() == "")
                {
                    var s = _allCodeRepository.GetListByType(AllCodeType.BOOKING_HOTEL_ROOM_STATUS).Where(x => x.CodeValue != (int)ServiceStatus.New).Select(x => x.CodeValue);
                    searchModel.StatusBooking = string.Join(",", s);
                }
                if (searchModel.StartDateFrom != null && (searchModel.StartDateTo == null || searchModel.StartDateTo < searchModel.StartDateFrom)) searchModel.StartDateTo = searchModel.StartDateFrom;

                var current_user = _ManagementUser.GetCurrentUser();
                if (current_user != null)
                {
                    if (current_user.Role != "")
                    {
                        var list = current_user.Role.Split(',');
                        foreach (var item in list)
                        {
                            bool is_admin_or_department = false;
                            switch (Convert.ToInt32(item))
                            {

                                case (int)RoleType.TPDHVe:
                                case (int)RoleType.TPDHTour:
                                case (int)RoleType.TPKS:
                                case (int)RoleType.TPTour:
                                case (int)RoleType.TPVe:
                                case (int)RoleType.DHPQ:
                                case (int)RoleType.DHTour:
                                case (int)RoleType.DHVe:
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
                                    {
                                        searchModel.SalerPermission = null;
                                        is_admin_or_department = true;
                                    }
                                    break;
                            }
                            if (is_admin_or_department) break;
                        }

                        ViewBag.Model = await _vinWonderBookingRepository.GetPagingList(searchModel, searchModel.PageIndex, searchModel.pageSize);
                    }
                }

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("VinWonderSearch - SetServiceVinWonderController: " + ex);
            }

            return View();
        }
        public async Task<IActionResult> VinWonderDetail(long id)
        {
            ViewBag.user = new User();
            ViewBag.Booking = new VinWonderBooking();
            ViewBag.UserCreated = new User();
            ViewBag.UserUpdated = new User();
            ViewBag.Saler = new User();
            ViewBag.Operator = new User();
            ViewBag.OrderAmount = 0;
            ViewBag.ClientId = 0;
            ViewBag.RefundAmount = 0;
            try
            {
                var amount = _contractPayRepository.GetTotalAmountContractPayByServiceId(id.ToString(), (int)ServicesType.VinWonder, (int)CONTRACT_PAY_TYPE.THU_TIEN_NCC_HOAN_TRA);
                ViewBag.amountNcc = amount;
                var allcode_list = _allCodeRepository.GetListByType(AllCodeType.BOOKING_HOTEL_ROOM_STATUS);
                var vinwonder_detail = _vinWonderBookingRepository.GetVinWonderBookingById(id);
                if (vinwonder_detail != null && vinwonder_detail.Id > 0)
                {
                    ViewBag.StatusName = allcode_list.First(x => x.CodeValue == vinwonder_detail.Status).Description;
                    ViewBag.user = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                    ViewBag.Booking = vinwonder_detail;
                    ViewBag.UserCreated = await _userRepository.GetById((long)vinwonder_detail.CreatedBy);
                    var order = _orderRepository.GetByOrderId((long)vinwonder_detail.OrderId);
                    if (order != null && order.OrderId > 0)
                    {
                        ViewBag.RefundAmount = (double)(order.Refund == null ? 0 : order.Refund);
                        ViewBag.OrderAmount = order.Amount;
                        if (order.SalerId != null && order.SalerId > 0)
                        {
                            ViewBag.Saler = await _userRepository.GetById((long)order.SalerId);
                        }
                        ViewBag.ClientId = order.ClientId ?? 0;
                    }
                    if (vinwonder_detail.UpdatedBy != null)
                    {
                        ViewBag.UserUpdated = await _userRepository.GetById((long)vinwonder_detail.UpdatedBy);

                    }
                    ViewBag.Operator = await _userRepository.GetById((long)vinwonder_detail.SalerId);
                    var tickets = await _vinWonderBookingRepository.GetVinWonderBookingTicketByBookingID(id);

                    ViewBag.AllowToFinishPayment = false;
                    if (tickets != null && tickets.Count > 0)
                    {
                        var max_date = tickets.OrderByDescending(x => x.DateUsed).First().DateUsed;
                        if (max_date < DateTime.Now)
                        {
                            ViewBag.AllowToFinishPayment = true;

                        }
                    }
                    //var data_refund = _paymentRequestRepository.GetRequestByClientId((long)order.ClientId, order.OrderId);
                    //if (data_refund != null && data_refund.Count > 0)
                    //{
                    //    ViewBag.RefundAmount = data_refund.Where(n => (n.Status == (int)(PAYMENT_REQUEST_STATUS.DA_CHI)) && !string.IsNullOrEmpty(n.PaymentVoucherCode)).Sum(n => n.Amount);

                    //}
                    return View();

                }
                else
                {
                    return RedirectToAction("Error", "Home");
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("VinWonderDetail - SetServiceVinWonderController: " + ex);
                return RedirectToAction("Error", "Home");
            }

        }
        [HttpPost]
        public async Task<IActionResult> VinWonderDetailBookingSale(long id)
        {
            try
            {
                var VinWonder_detail = _vinWonderBookingRepository.GetVinWonderBookingById(id);
                if (VinWonder_detail != null && VinWonder_detail.Id > 0)
                {
                    ViewBag.Booking = VinWonder_detail;
                    ViewBag.Packages = await _VinWonderBookingRepository.GetVinWonderTicketByBookingIdSP(VinWonder_detail.Id);
                    ViewBag.Guests = await _VinWonderBookingRepository.GetVinWonderTicketCustomerByBookingIdSP(VinWonder_detail.Id);
                    return View();
                }
                else
                {
                    return RedirectToAction("Error", "Home");
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("VinWonderDetailBookingSale - SetServiceVinWonderController: " + ex);
                return RedirectToAction("Error", "Home");
            }
        }
        [HttpPost]
        public async Task<IActionResult> VinWonderDetailBookingOperator(long id)
        {
            try
            {
                var VinWonder_detail = _vinWonderBookingRepository.GetVinWonderBookingById(id);
                if (VinWonder_detail != null && VinWonder_detail.Id > 0)
                {
                    ViewBag.Booking = VinWonder_detail;
                    ViewBag.Packages = await _VinWonderBookingRepository.GetVinWonderTicketByBookingIdSP(VinWonder_detail.Id);
                    return View();
                }
                else
                {
                    return RedirectToAction("Error", "Home");
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("VinWonderDetailBookingOperator - SetServiceVinWonderController: " + ex);
                return RedirectToAction("Error", "Home");
            }
        }
        public async Task<IActionResult> VinWonderDetailBookingChangeOperator(string operator_name, long booking_id)
        {

            try
            {
                ViewBag.Name = operator_name;
                ViewBag.Id = booking_id;

            }
            catch
            {

            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SummitVinWonderDetailChangeOperator(long booking_id, int user_id)
        {

            try
            {
                if (booking_id <= 0 || user_id <= 0)
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
                var success = await _VinWonderBookingRepository.UpdateServiceOperator(booking_id, user_id, _UserId);

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
        public async Task<IActionResult> UpdateVinWonderOperatorOrderPrice(List<VinWonderBookingTicket> data)
        {

            try
            {
                if (data == null || data.Count <= 0)
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = "Dữ liệu gửi lên không chính xác, vui lòng kiểm tra lại"
                    });
                }
                var VinWonder_booking = _VinWonderBookingRepository.GetVinWonderBookingById((long)data[0].BookingId);
                double amount = 0;
                double price = 0;
                double profit = 0;
                if (VinWonder_booking != null && VinWonder_booking.Id > 0)
                {
                    int _UserId = 0;
                    if (HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) != null)
                    {
                        _UserId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                    }
                    var id = await _VinWonderBookingRepository.UpdateVinWonderTicketOperatorPrice(data, _UserId);
                    VinWonder_booking = _VinWonderBookingRepository.GetVinWonderBookingById((long)data[0].BookingId);

                    amount = (double)VinWonder_booking.Amount;
                    price = VinWonder_booking.TotalUnitPrice != null ? (double)VinWonder_booking.TotalUnitPrice : 0;
                    profit = amount - price - (VinWonder_booking.Commission == null ? 0 : (double)VinWonder_booking.Commission) - (VinWonder_booking.OthersAmount == null ? 0 : (double)VinWonder_booking.OthersAmount);

                    #region Update Order Amount:

                    await _orderRepository2.UpdateOrderDetail((long)VinWonder_booking.OrderId, _UserId);
                    #endregion
                }
                return Ok(new
                {
                    status = (int)ResponseType.SUCCESS,
                    msg = "Cập nhật giá đặt dịch vụ thành công",
                    amount = price,
                    profit = profit
                });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateOperatorOrderPrice - SetServiceVinWonderController: " + ex.ToString());

            }
            return Ok(new
            {
                status = (int)ResponseType.FAILED,
                msg = "Cập nhật giá đặt dịch vụ thất bại, vui lòng liên hệ IT"
            });

        }

        [HttpPost]
        public async Task<IActionResult> GrantVinWonderServiceOrderPermission(long booking_id)
        {
            try
            {
                var current_user = _ManagementUser.GetCurrentUser();
                if (booking_id <= 0)
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = "Nhận đặt dịch vụ thất bại, vui lòng tải lại trang và thử lại"
                    });
                }
                int _UserId = 0;
                if (HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) != null)
                {
                    _UserId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                var id = await _VinWonderBookingRepository.UpdateServiceOperator(booking_id, _UserId);
                if (id > 0)
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        msg = "Nhận đặt dịch vụ thành công"
                    });
                }

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GrantOrderPermission - SetServiceVinWonderController: " + ex.ToString());

            }
            return Ok(new
            {
                status = (int)ResponseType.ERROR,
                msg = "Nhận đặt dịch vụ thất bại, vui lòng liên hệ IT"
            });
        }

        [HttpPost]
        public async Task<IActionResult> VinWonderServiceSendServiceCode(long booking_id)
        {
            try
            {
                var current_user = _ManagementUser.GetCurrentUser();
                if (booking_id <= 0)
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
                var hotelBookingCode = await _hotelBookingCodeRepository.GetListlBookingCodeByHotelBookingId(booking_id, (int)ServicesType.VinWonder);
                if (hotelBookingCode != null && hotelBookingCode.Count > 0)
                {
                    var id = await _VinWonderBookingRepository.UpdateServiceStatus((int)ServiceStatus.ServeCode, booking_id, _UserId);
                    if (id > 0)
                    {
                        var detail = _VinWonderBookingRepository.GetVinWonderBookingById(booking_id);
                        var order = _orderRepository.GetByOrderId((long)detail.OrderId);
                        string link = "/Order/" + detail.OrderId;
                        apiService.SendMessage(_UserId.ToString(), ((int)ModuleType.DICH_VU).ToString(), ((int)ActionType.TRA_CODE).ToString(), order.OrderNo, link, current_user.Role, detail.ServiceCode);
                        return Ok(new
                        {
                            status = (int)ResponseType.SUCCESS,
                            msg = "Gửi Code dịch vụ thành công"
                        });
                    }
                }
                else
                {

                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = "Chưa có Code dịch vụ"
                    });

                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SendServiceCode - SetServiceVinWonderController: " + ex.ToString());

            }
            return Ok(new
            {
                status = (int)ResponseType.ERROR,
                msg = "Cập nhật trạng thái đặt dịch vụ thất bại, vui lòng liên hệ IT"
            });
        }
        [HttpPost]
        public async Task<IActionResult> VinWonderServiceChangeToConfirmPaymentStatus(long booking_id)
        {
            try
            {
                var current_user = _ManagementUser.GetCurrentUser();
                if (booking_id <= 0)
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
                var payment_request = _paymentRequestRepository.GetByServiceId(booking_id, (int)ServicesType.VinWonder);
                if (payment_request == null || payment_request.Count <= 0)
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = "Dịch vụ chưa có yêu cầu chi nào, vui lòng bổ sung"
                    });
                }
                var detail = _VinWonderBookingRepository.GetVinWonderBookingById(booking_id);
                double amount = detail.TotalUnitPrice != null ? (double)detail.TotalUnitPrice : ((double)detail.Amount - (double)detail.TotalProfit);
                var request_amount = Convert.ToDouble(payment_request.Sum(x => (((x.Status == (int)(PAYMENT_REQUEST_STATUS.DA_CHI) || x.Status == (int)(PAYMENT_REQUEST_STATUS.CHO_CHI) || x.IsSupplierDebt == true))) ? x.Amount : 0));
                if (request_amount >= amount)
                {
                    var id = await _VinWonderBookingRepository.UpdateServiceStatus((int)ServiceStatus.Payment, booking_id, _UserId);
                    var dataOrder2 = await _orderRepository2.ProductServiceName(detail.OrderId.ToString());
                    var count_dataOrder2 = dataOrder2.Where(s => s.Status != (int)ServiceStatus.Cancel).ToList().Count;
                    var data = dataOrder2.Where(s => s.Status == (int)ServiceStatus.Payment ? s.Status == (int)ServiceStatus.Payment : s.Status == (int)ServiceStatus.Decline).ToList();
                    if (data.Count == count_dataOrder2)
                    {
                        long UpdatedBy = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                        await _orderRepository2.UpdateOrderStatus((long)detail.OrderId, (int)OrderStatus.WAITING_FOR_ACCOUNTANT, UpdatedBy, 0);
                    }
                    if (id > 0)
                    {
                        var order = _orderRepository.GetByOrderId((long)detail.OrderId);
                        string link = "/Order/" + detail.OrderId;
                        apiService.SendMessage(_UserId.ToString(), ((int)ModuleType.DICH_VU).ToString(), ((int)ActionType.QUYET_TOAN).ToString(), order.OrderNo, link, current_user.Role, detail.ServiceCode);
                        //-- update order payment_status
                        await _orderRepository2.UpdateOrderDetail((long)detail.OrderId, _UserId);

                        return Ok(new
                        {
                            status = (int)ResponseType.SUCCESS,
                            msg = "Quyết toán dịch vụ thành công"
                        });
                    }
                }
                else
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = "Đơn hàng chưa được thanh toán đủ"
                    });
                }


            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ChangeToConfirmPaymentStatus - SetServiceVinWonderController: " + ex.ToString());

            }
            return Ok(new
            {
                status = (int)ResponseType.ERROR,
                msg = "Cập nhật trạng thái đặt dịch vụ thất bại, vui lòng liên hệ IT"
            });
        }

        [HttpPost]
        public async Task<IActionResult> vinwonderExportExcel(SearchFlyBookingViewModel searchModel)
        {
            try
            {
                var current_user = _ManagementUser.GetCurrentUser();
                string _FileName = "Danh sách đặt dịch vụ VinWonder(" + current_user.Id + ").xlsx";
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

                var rsPath = await _vinWonderBookingRepository.ExportDeposit(searchModel, FilePath);

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
                LogHelper.InsertLogTelegram("ExportExcel - SetServiceVinWonderController: " + ex);
                return new JsonResult(new
                {
                    isSuccess = false,
                    message = ex.Message.ToString()
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ExportVinWonderTicket(long booking_id)
        {
            try
            {
                int _UserId = 0;
                if (HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) != null)
                {
                    _UserId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                List<int> status_allowed = new List<int>() { (int)ServiceStatus.OnExcution, (int)ServiceStatus.ServeCode };
                var vinwonder = _vinWonderBookingRepository.GetVinWonderBookingById(booking_id);
                if (vinwonder == null || !status_allowed.Contains(vinwonder.Status == null ? 0 : (int)vinwonder.Status))
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = "Xuất vé VinWonder tự động thất bại: Trạng thái dịch vụ hiện tại không cho phép xuất vé VinWonder "
                    });
                }
                var hotelBookingCode = await _hotelBookingCodeRepository.GetListlBookingCodeByHotelBookingId(booking_id, (int)ServicesType.VinWonder);
                if (hotelBookingCode != null && hotelBookingCode.Count > 0)
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = "Xuất vé VinWonder tự động thất bại: Thông tin Code dịch vụ đã tồn tại"
                    });

                }
                //-- Get Ticket:
                HttpClient httpClient = new HttpClient();
                string url_push = ReadFile.LoadConfig().API_URL + ReadFile.LoadConfig().LockBookingVinwonder;
                var vinwonder_booking = _vinWonderBookingRepository.GetVinWonderBookingById(booking_id);
                var order = _orderRepository.GetByOrderId((long)vinwonder_booking.OrderId);
                var object_mail = new Dictionary<string, string>
               {
                   {"order_id",vinwonder_booking.OrderId.ToString() },
               };
                string token = CommonHelper.Encode(JsonConvert.SerializeObject(object_mail), ReadFile.LoadConfig().KEY_TOKEN_API_MANUAL);
                var content_2 = new FormUrlEncodedContent(new[]
                {
                   new KeyValuePair<string, string>("token", token),
               });
                var result_post = await httpClient.PostAsync(url_push, content_2);
                var result = result_post.Content.ReadAsStringAsync().Result;
                var model = JsonConvert.DeserializeObject<VinWonderConfirmBookingOutputModel>(result);

                if (model != null && model.data != null && model.data.Count > 0 && model.data[0].Data != null && model.data[0].Data.Tickets != null && model.data[0].Data.Tickets.Count > 0)
                {
                    foreach (var data in model.data)
                    {
                        string urls = "";
                        foreach (var ticket in data.Data.Tickets)
                        {
                            urls += ticket.QrCodeUrl + "<br />";

                        }
                        HotelBookingCodeViewModel order_code = new HotelBookingCodeViewModel()
                        {
                            AttactFile = "",
                            BookingCode = data.Data.BookingCode,
                            CreatedBy = _UserId,
                            CreatedDate = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"),
                            Description = "Link lấy mặt vé: <br />" + urls,
                            HotelBookingId = booking_id,
                            Id = 0,
                            IsDelete = 0,
                            Note = "",
                            Type = (int)ServiceType.VinWonder,
                            UpdatedBy = _UserId,
                            UpdatedDate = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss")
                        };
                        var id = await _hotelBookingCodeRepository.InsertHotelBookingCode(order_code);

                    }

                }

                // Send Email Ticket
                if (model != null && model.data != null && model.data.Count > 0)
                {
                    List<string> images_ticket = new List<string>();
                    if (model.data != null && model.data[0] != null && (model.data[0].Data == null || model.data[0].Data.Tickets == null || model.data[0].Data.Tickets.Count <= 0))
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = "Xuất vé VinWonder tự động thất bại: " + model.data[0].Result.Message
                        });
                    }
                    foreach (var data in model.data)
                    {
                        foreach (var ticket in data.Data.Tickets)
                        {
                            var file_path = await _emailService.PathAttachmentVeVinWonder(ticket);
                            if (file_path != null && file_path.Trim() != "")
                            {
                                byte[] bytes_file = System.IO.File.ReadAllBytes(file_path);
                                string base64_file = Convert.ToBase64String(bytes_file);
                                var url_static = await _emailService.UploadImageBase64(base64_file, "png", file_path);

                                if (url_static != null && url_static.Trim() != "")
                                {
                                    images_ticket.Add(url_static);

                                }
                            }
                        }
                    }
                    if (images_ticket != null && images_ticket.Count > 0)
                    {
                        foreach (var url in images_ticket)
                        {
                            var ext = url.Split(".");
                            AttachFile file = new AttachFile()
                            {
                                Capacity = 0,
                                CreateDate = DateTime.Now,
                                DataId = booking_id,
                                Ext = ext[^1],
                                Path = url,
                                Type = (int)AttachmentType.Email_VinWonder_Ticket,
                                UserId = _UserId,

                            };
                            var id = await _AttachFileRepository.AddAttachFile(file);
                        }
                    }
                }

                return Ok(new
                {
                    status = (int)ResponseType.SUCCESS,
                    msg = "Xuất vé VinWonder tự động thành công"
                });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ExportVinWonderTicket - SetServiceVinWonderController with BookingID=" + booking_id + " : " + ex.ToString());
                return Ok(new
                {
                    status = (int)ResponseType.ERROR,
                    msg = "Lỗi trong quá trình xử lý, vui lòng liên hệ IT"
                });

            }
        }
        public async Task<IActionResult> EmailVinWonderTicket(long booking_id)
        {
            VinWonderTicketPreviewModel mail = new VinWonderTicketPreviewModel();
            try
            {
                if (booking_id > 0)
                {
                    var vinwonder_booking = _vinWonderBookingRepository.GetVinWonderBookingById(booking_id);
                    var order = _orderRepository.GetByOrderId((long)vinwonder_booking.OrderId);
                    var list_email = new List<VinWonderTicketPreviewEmail>();
                    ViewBag.OrderId = order.OrderId;
                    ViewBag.BookingId = vinwonder_booking.Id;
                    if (order.ContactClientId == null)
                    {
                        var client = await _clientRepository.GetClientDetailByClientId((long)order.ClientId);
                        if (client != null && client.Id > 0)
                        {
                            list_email.Add(new VinWonderTicketPreviewEmail()
                            {
                                email = client.Email,
                                username = client.ClientName
                            });
                        }
                    }
                    else
                    {
                        var contact_client = _contactClientRepository.GetByContactClientId((long)order.ContactClientId);
                        if (contact_client != null && contact_client.Id > 0)
                        {
                            list_email.Add(new VinWonderTicketPreviewEmail()
                            {
                                email = contact_client.Email,
                                username = contact_client.Name
                            });
                            mail.contact_client = contact_client;
                        }
                    }
                    var sale_order = await _userRepository.GetById(order.SalerId == null ? 0 : (int)order.SalerId);
                    if (sale_order != null && sale_order.Id > 0)
                    {
                        list_email.Add(new VinWonderTicketPreviewEmail()
                        {
                            email = sale_order.Email,
                            username = sale_order.FullName
                        });
                    }
                    var operator_service = await _userRepository.GetById(vinwonder_booking.SalerId == null ? 0 : (int)vinwonder_booking.SalerId);
                    if (operator_service != null && operator_service.Id > 0)
                    {
                        list_email.Add(new VinWonderTicketPreviewEmail()
                        {
                            email = operator_service.Email,
                            username = operator_service.FullName
                        });
                    }
                    mail.to_email = list_email;
                    mail.cc_email = list_email;
                    mail.bcc_email = list_email;
                    mail.body = await _emailService.GetTemplateVinWordbookingTC(order.OrderId);
                    mail.can_sendemail = true;
                    var list = await _AttachFileRepository.GetListByDataID(booking_id, (int)AttachmentType.Email_VinWonder_Ticket);
                    if (list != null && list.Count > 0)
                    {
                        if (mail.file_attachment == null) mail.file_attachment = new List<LightGalleryViewModel>();
                        foreach (var f in list)
                        {
                            mail.file_attachment.Add(new LightGalleryViewModel()
                            {
                                ext = f.Ext,
                                thumb = f.Path,
                                url = f.Path
                            });
                        }

                    }
                    mail.subject = "[Adavigo] Vé Điện Tử Vinwonder Của Quý Khách - Mã Đơn hàng " + order.OrderNo;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SendEmailVinWonderTicket - SetServiceVinWonderController: " + ex);
            }
            ViewBag.Mail = mail;
            return PartialView();
        }
        [HttpPost]
        public async Task<IActionResult> SendEmailVinWonderTicket(long order_id, long booking_id, string subject, List<string> to_email, List<string> cc_email, List<string> bcc_email)
        {
            try
            {
                if (order_id <= 0 || booking_id <= 0 || subject == null || subject.Trim() == "" || to_email == null || to_email.Count <= 0)
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = "Dữ liệu gửi lên không chính xác, Vui lòng tải lại trang / Chọn ít nhất 1 địa chỉ email tại mục người nhận"
                    });
                }
                var id = await _emailService.SendEmailVinwonderTicket(order_id, booking_id, subject, to_email, cc_email, bcc_email);
                return Ok(new
                {
                    status = (int)ResponseType.SUCCESS,
                    msg = "Gửi Email vé VinWonder thành công"
                });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SendEmailVinWonderTicket - SetServiceVinWonderController: " + ex);
            }
            return Ok(new
            {
                status = (int)ResponseType.FAILED,
                msg = "Gửi Email vé VinWonder thất bại, vui lòng liên hệ IT"
            });
        }
    }
}
