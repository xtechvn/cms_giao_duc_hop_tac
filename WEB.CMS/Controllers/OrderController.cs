using Caching.Elasticsearch;
using Entities.Models;
using Entities.ViewModels;
using Entities.ViewModels.Attachment;
using Entities.ViewModels.Contract;
using Entities.ViewModels.ElasticSearch;
using Entities.ViewModels.Funding;
using Entities.ViewModels.HotelBookingCode;
using Entities.ViewModels.Invoice;
using Entities.ViewModels.Mongo;
using Entities.ViewModels.OrderManual;
using ENTITIES.ViewModels.ElasticSearch;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver.Linq;
using Newtonsoft.Json;
using OfficeOpenXml;
using Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;
using GDHT.CMS.Service;
using GDHT.CMS.Service.ServiceInterface;
using WEB.CMS.Customize;
using WEB.CMS.Models;

namespace GDHT.CMS.Controllers
{
    [CustomAuthorize]
    public class OrderController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IOrderRepository _orderRepository;
        private readonly IAccountClientRepository _accountClientRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IAllCodeRepository _allCodeRepository;
        private readonly IContactClientRepository _contactClientRepository;
        private readonly IOrderRepositor _iOrderRepositories;
        private OrderESRepository _orderESRepository;
        private readonly IHotelBookingRepositories _hotelBookingRepositories;
        private readonly IFlyBookingDetailRepository _flyBookingDetailRepository;
        private readonly IUserRepository _userRepository;
        private readonly IContractPayRepository _contractPayRepository;
        private readonly IFlightSegmentRepository _flightSegmentRepository;
        private readonly IBagageRepository _bagageRepository;
        private readonly IContactClientRepository _ContactClientRepository;
        private readonly IAttachFileRepository _AttachFileRepository;
        private readonly ITourRepository _tourRepository;
        private readonly IContractRepository _contractRepository;
        private ManagementUser _ManagementUser;
        private readonly IEmailService _emailService;
        private readonly IHotelBookingCodeRepository _hotelBookingCodeRepository;
        private readonly IInvoiceRequestRepository _invoiceRequestRepository;
        private readonly IVinWonderBookingRepository _vinWonderBookingRepository;
        private IOtherBookingRepository _otherBookingRepository;
        private readonly IWebHostEnvironment _WebHostEnvironment;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IPaymentRequestRepository _paymentRequestRepository;
        private APIService apiService;
        private BoongKingCodeESRepository _boongKingCodeESRepository;
        private LogActionMongoService LogActionMongo;

        private readonly List<int> list_order_status_not_allow_to_edit = new List<int>() { (int)OrderStatus.FINISHED, (int)OrderStatus.CANCEL, (int)OrderStatus.WAITING_FOR_ACCOUNTANT, (int)OrderStatus.WAITING_FOR_OPERATOR };
        public OrderController(IConfiguration configuration, IOrderRepository orderRepository, IClientRepository clientRepository, IHotelBookingRepositories hotelBookingRepositories, ManagementUser managementUser, IContractRepository contractRepository,
            IAllCodeRepository allcodeRepository, IContactClientRepository contactClientRepository, IOrderRepositor iOrderRepositories, IFlightSegmentRepository flightSegmentRepository, IBagageRepository bagageRepository, IContactClientRepository ContactClientRepository, IHotelBookingCodeRepository hotelBookingCodeRepository,
            IFlyBookingDetailRepository flyBookingDetailRepository, IUserRepository userRepository, IContractPayRepository contractPayRepository, IAttachFileRepository AttachFileRepository, ITourRepository tourRepository, IEmailService emailService, IAttachFileRepository attachFileRepository, IOtherBookingRepository otherBookingRepository,
            IInvoiceRequestRepository invoiceRequestRepository, IVinWonderBookingRepository vinWonderBookingRepository, IWebHostEnvironment WebHostEnvironment, IInvoiceRepository invoiceRepository, IAccountClientRepository accountClientRepository, IPaymentRequestRepository paymentRequestRepository)
        {
            _invoiceRequestRepository = invoiceRequestRepository;
            _configuration = configuration;
            _orderRepository = orderRepository;
            _clientRepository = clientRepository;
            _allCodeRepository = allcodeRepository;
            _contactClientRepository = contactClientRepository;
            _iOrderRepositories = iOrderRepositories;
            _orderESRepository = new OrderESRepository(_configuration["DataBaseConfig:Elastic:Host"]);
            _flyBookingDetailRepository = flyBookingDetailRepository;
            _hotelBookingRepositories = hotelBookingRepositories;
            _userRepository = userRepository;
            _contractPayRepository = contractPayRepository;
            _flightSegmentRepository = flightSegmentRepository;
            _bagageRepository = bagageRepository;
            _ContactClientRepository = ContactClientRepository;
            _AttachFileRepository = AttachFileRepository;
            _tourRepository = tourRepository;
            _ManagementUser = managementUser;
            _emailService = emailService;
            _contractRepository = contractRepository;
            _hotelBookingCodeRepository = hotelBookingCodeRepository;
            apiService = new APIService(configuration, userRepository);
            _otherBookingRepository = otherBookingRepository;
            _vinWonderBookingRepository = vinWonderBookingRepository;
            _WebHostEnvironment = WebHostEnvironment;
            _invoiceRepository = invoiceRepository;
            _accountClientRepository = accountClientRepository;
            _paymentRequestRepository = paymentRequestRepository;
            _boongKingCodeESRepository = new BoongKingCodeESRepository(_configuration["DataBaseConfig:Elastic:Host"]);
            LogActionMongo = new LogActionMongoService(configuration);
        }


        public async Task<IActionResult> Index()
        {
            int currentPage = 1;
            int pageSize = 20;
            try
            {
                var key_token_api = _configuration["DataBaseConfig:key_api:api_manual"];

                var serviceType = _allCodeRepository.GetListByType("SERVICE_TYPE");
                var systemtype = _allCodeRepository.GetListByType("SYSTEM_TYPE");
                var utmSource = _allCodeRepository.GetListByType("UTM_SOURCE");
                var orderStatus = _allCodeRepository.GetListByType("ORDER_STATUS");
                var PAYMENT_STATUS = _allCodeRepository.GetListByType("PAYMENT_STATUS");
                var PERMISION_TYPE = _allCodeRepository.GetListByType("PERMISION_TYPE");
                //var model = await _orderRepository.GetPagingList(new OrderViewSearchModel(), currentPage, pageSize);
                //var model = await _orderRepository.GetList(new OrderViewSearchModel(), currentPage, pageSize);
                ViewBag.PAYMENT_STATUS = PAYMENT_STATUS;
                ViewBag.PERMISION_TYPE = PERMISION_TYPE;
                ViewBag.FilterOrder = new FilterOrder()
                {
                    SysTemType = systemtype,
                    Source = utmSource,
                    Type = serviceType,
                    Status = orderStatus,
                };
            }
            catch (System.Exception ex)
            {
                LogHelper.InsertLogTelegram("Index - OrderController: " + ex.ToString());
                return Content("");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Search(OrderViewSearchModel searchModel, int currentPage = 1, int pageSize = 20)
        {
            var model = new GenericViewModel<OrderViewModel>();
            var model2 = new TotalCountSumOrder();
            try
            {
                if (searchModel.OrderNo != null && searchModel.OrderNo.Trim() != "") searchModel.OrderNo = searchModel.OrderNo.ToUpper();
                if (searchModel.HINHTHUCTT != null && searchModel.HINHTHUCTT[0] != null)
                {
                    foreach (var item in searchModel.HINHTHUCTT)
                    {
                        var listHINHTHUCTT = item.Split('_');
                        if (searchModel.PermisionType == null)
                        {
                            searchModel.PermisionType = listHINHTHUCTT[0];
                        }
                        else
                        {
                            searchModel.PermisionType += "," + listHINHTHUCTT[0];
                        }
                        if (searchModel.PaymentStatus == null)
                        {
                            searchModel.PaymentStatus = listHINHTHUCTT[1];
                        }
                        else
                        {
                            searchModel.PaymentStatus += "," + listHINHTHUCTT[1];
                        }

                    }


                }
                var current_user = _ManagementUser.GetCurrentUser();
                if (current_user != null)
                {
                    if (current_user.Role != "")
                    {
                        var list = current_user.Role.Split(',');
                        foreach (var item in list)
                        {
                            bool is_admin = false;
                            switch (Convert.ToInt32(item))
                            {
                                case (int)RoleType.SaleOnl:
                                case (int)RoleType.SaleKd:
                                case (int)RoleType.SaleTour:
                                case (int)RoleType.TPDHKS:
                                case (int)RoleType.TPDHVe:
                                case (int)RoleType.TPDHTour:
                                case (int)RoleType.TPKS:
                                case (int)RoleType.TPTour:
                                case (int)RoleType.TPVe:
                                case (int)RoleType.DHPQ:
                                case (int)RoleType.DHTour:
                                case (int)RoleType.DHVe:
                                case (int)RoleType.GDHN:
                                case (int)RoleType.GDHPQ:
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
                                case (int)RoleType.GD:
                                case (int)RoleType.PhoTPKeToan:
                                    {
                                        searchModel.Sale = null;
                                        searchModel.SalerPermission = null;
                                        is_admin = true;
                                    }
                                    break;
                            }
                            if (is_admin) break;
                        }

                        model = await _orderRepository.GetList(searchModel, currentPage, pageSize);
                        model2 = await _orderRepository.GetTotalCountSumOrder(searchModel, -1, pageSize);
                    }

                }

                long records;

                switch (searchModel.StatusTab)
                {
                    case 99:
                        records = model.TotalRecord;
                        break;
                    case 0:
                        records = model.TotalRecord4;
                        break;
                    case 1:
                        records = model.TotalRecord1;
                        break;
                    case 2:
                        records = model.TotalRecord2;
                        break;
                    case 3:
                        records = model.TotalRecord3;
                        break;
                    default:
                        records = model.TotalrecordErr;
                        break;
                }
                //model.TotalPage = (int)Math.Ceiling((double)records / model.PageSize);

                ViewBag.FilterOrder = new FilterOrder()
                {
                    Totalrecord = model.TotalRecord,
                    TotalData = records,
                    Totalrecord1 = model.TotalRecord1,
                    Totalrecord2 = model.TotalRecord2,
                    Totalrecord3 = model.TotalRecord3,
                    Totalrecord4 = model.TotalRecord4,
                    TotalrecordErr = model.TotalrecordErr,
                    TotalValueOrder = new TotalValueOrder()
                    {
                        //theo All
                        TotalAmmount = model2.Amount.ToString("N0"),
                        TotalDone = model?.ListData?.Sum(x => x.Amount).ToString("N0"),
                        TotalProductService = model2.Price.ToString("N0"),
                        TotalProfit = model2.Profit.ToString("N0")

                        //theo pageSize
                        //TotalAmmount = model?.ListData?.Sum(x => x.Amount).ToString("N0"),
                        //TotalDone = model?.ListData?.Sum(x => x.Amount).ToString("N0"),
                        //TotalProductService = model?.ListData?.Sum(x => x.Payment).ToString("N0"),
                        //TotalProfit = model?.ListData?.Sum(x => x.Profit).ToString("N0")
                    }
                };
                //model = await _orderRepository.GetPagingList(searchModel, currentPage, pageSize);
                // Add Invoice Code:
                ViewBag.Invoice = new List<InvoiceRequestViewModel>();
                if (model != null && model.ListData != null && model.ListData.Count > 0)
                {
                    var order_ids = string.Join(",", model.ListData.Select(x => x.OrderId));
                    ViewBag.Invoice = await _invoiceRepository.GetListInvoiceRequestbyOrderId(order_ids);
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Search - OrderController: " + ex);
            }

            return PartialView(model);
        }

        public async Task<IActionResult> Orderdetails(long id)
        {
            try
            {

                var result = id;
                if (id != 0)
                {
                    ViewBag.OrderClosing = false;
                    var ListOrderBookClosing = await _orderRepository.GetListOrderBookClosingByOrderId(id);
                    ViewBag.ServiceStatus = 0;
                    ViewBag.IsDeclineOrder = false;
                    var current_user = _ManagementUser.GetCurrentUser();
                    IEnumerable<int> menu_ids = new List<int>();
                    if(ListOrderBookClosing != null && ListOrderBookClosing.Count > 0)
                    {
                        ViewBag.OrderClosing = true;
                    }
                    if (current_user != null && current_user.Role != null)
                    {
                        List<int> role = new List<int>();
                        foreach (var a in current_user.Role.Split(","))
                        {
                            try
                            {
                                role.Add(Convert.ToInt32(a));
                            }
                            catch { }
                        }
                        ViewBag.Role = role;
                        if (current_user.Role.Contains(((int)RoleType.KeToanTruong).ToString()) || current_user.Role.Contains(((int)RoleType.Admin).ToString())|| current_user.Role.Contains(((int)RoleType.PhoTPKeToan).ToString()) )
                        {
                            ViewBag.RoleKTTruong = 1;
                        }
                    }
                    var dataOrder = _iOrderRepositories.GetByOrderId(id);
                    ViewBag.IsLock = dataOrder != null ? dataOrder.IsLock : false;
                    var dataOrderService = await _orderRepository.GetAllServiceByOrderId(dataOrder.OrderId);
                    bool triggered = false;
                    ViewBag.OrderNo = dataOrder.OrderNo;
                    if (dataOrderService != null && dataOrderService.Count > 0)
                    {
                        foreach (var item in dataOrderService)
                        {
                            if (item.Status == (int)ServiceStatus.Decline || item.Status == (int)ServiceStatus.New)
                            {
                                ViewBag.ServiceStatus = 1;
                                ViewBag.ServiceStatus2 = 0;
                                if (item.Status == (int)ServiceStatus.Decline)
                                {
                                    ViewBag.ServiceStatus2 = 1;
                                }
                                if (item.Status == (int)ServiceStatus.Decline && !triggered)
                                {
                                    ViewBag.IsDeclineOrder = true;
                                    triggered = true;
                                }
                            }
                        }
                    }

                    var ClientDetai = await _clientRepository.GetClientDetailByClientId((long)dataOrder.ClientId);
                    if (ClientDetai != null)
                    {
                        ViewBag.PermisionType = ClientDetai.PermisionType;
                    }

                    if (OrderIndentiferService.IsOrderManual(dataOrder.OrderNo))
                    {
                        ViewBag.OrderNo_Type = 1;
                    }
                    else
                    {
                        ViewBag.OrderNo_Type = 0;
                    }
                    ViewBag.IsManualOrder = false;
                    ViewBag.OrderServiceType = 0;
                    ViewBag.IsB2COrder = false;
                    ViewBag.HasSaleToProgress = true;
                    if (OrderIndentiferService.IsOrderManual(dataOrder.OrderNo))
                    {
                        ViewBag.IsManualOrder = true;
                    }
                    else
                    {
                        if (dataOrder.OrderNo.StartsWith("C"))
                        {
                            ViewBag.IsB2COrder = true;
                        }
                        ViewBag.OrderServiceType = (int)dataOrder.ServiceType;
                        if (dataOrder.SalerId == null || dataOrder.SalerId <= 0)
                        {
                            ViewBag.HasSaleToProgress = false;
                        }
                    }


                    var dataOrder2 = await _orderRepository.ProductServiceName(id.ToString());
                    var data = dataOrder2.Where(s => s.Status == 4 ? s.Status == 4 : s.Status == 5).ToList();
                    var data2 = dataOrder2.Any(s => s.Status != 5);
                    if (data.Count == dataOrder2.Count)
                    {
                        ViewBag.StatusButton = 1;
                    }
                    else
                    {
                        ViewBag.StatusButton = 0;
                    }
                    if (!data2)
                    {
                        ViewBag.stauseButHUY = 1;
                    }
                    else
                    {
                        ViewBag.stauseButHUY = 0;
                    }
                    var FlyingTicket = (int)ServicesType.FlyingTicket;
                    if (dataOrder.ProductService != null && dataOrder.ProductService.Contains(FlyingTicket.ToString()))
                    {
                        ViewBag.FlyingTicketStause = 1;
                    }
                    else
                    {
                        ViewBag.FlyingTicketStause = 0;
                    }

                    ViewBag.OrderId = id;
                    ViewBag.OrderNo = dataOrder.OrderNo;
                    ViewBag.ProductService = dataOrder.ProductService;
                    ViewBag.OrderStatus = dataOrder.OrderStatus;
                    ViewBag.SalerId = dataOrder.SalerId;
                    ViewBag.ListOrderStatusNotAllowEdit = new List<int>() { (int)OrderStatus.FINISHED, (int)OrderStatus.CANCEL, (int)OrderStatus.WAITING_FOR_ACCOUNTANT, (int)OrderStatus.WAITING_FOR_OPERATOR };
                    ViewBag.ListOrderStatusNotAllowToChangeDetail = new List<int>() { (int)OrderStatus.OPERATOR_DECLINE, (int)OrderStatus.ACCOUNTANT_DECLINE };
                    ViewBag.ListSaleRole = new List<int>() { (int)RoleType.SaleKd, (int)RoleType.SaleOnl, (int)RoleType.SaleTour, (int)RoleType.Admin, (int)RoleType.TPKS, (int)RoleType.TPTour, (int)RoleType.TPVe, (int)RoleType.TPSaleOnl };
                    return View(result);
                }

                return View();
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Order - Orderdetails - Orderid=" + id + ";" + ex.ToString());

            }
            return View();
        }

        public IActionResult OrderdetailNo(string orderNo)
        {
            try
            {
                var order = _orderRepository.GetOrderByOrderNo(orderNo).Result;
                var result = order.OrderId;
                if (order.OrderId != 0)
                {
                    ViewBag.OrderId = order.OrderId;
                    return View(result);
                }

                return View();
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("OrderController - Orderdetails:" + ex.ToString());

            }
            return View();
        }

        public async Task<string> GetSuggestion(string name)
        {
            try
            {
                var key_token_api = _configuration["DataBaseConfig:key_api:api_manual"];
                string ApiGetList = ReadFile.LoadConfig().API_URL + ReadFile.LoadConfig().API_ORDER_LIST;

                var listData = await _allCodeRepository.GetAllCodeValueByType<SearchOrderModels>(ApiGetList, key_token_api, "txtsearch", "1");
                var SuggestOrder = listData.Data?.Where(x => x.OrderNo.ToLower().Contains(name)).Select(s => new
                {
                    id = s.Id,
                    name = s.OrderNo
                });
                return JsonConvert.SerializeObject(SuggestOrder);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetSuggestion - OrderController: " + ex);
                return null;
            }
        }

        public async Task<string> GetSallseSuggestionList(string name)
        {
            try
            {

                var rolelist = _clientRepository.GetClientType(ClientType.kl).Result;
                if (!string.IsNullOrEmpty(name))
                {
                    rolelist = rolelist.Where(s => StringHelpers.ConvertStringToNoSymbol(s.ClientName.Trim().ToLower())
                                                   .Contains(StringHelpers.ConvertStringToNoSymbol(name.Trim().ToLower())))
                                                   .ToList();
                }
                var suggestionlist = rolelist.Take(5).Select(s => new
                {
                    id = s.Id,
                    name = s.ClientName
                }).ToList();

                return JsonConvert.SerializeObject(suggestionlist);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetSallseSuggestionList - OrderController: " + ex);
                return null;
            }
        }
        [HttpPost]
        public async Task<IActionResult> ReCreateOrder(long orderId)
        {

            try
            {
                var key_token_api = _configuration["DataBaseConfig:key_api:b2c"];
                if (orderId <= 0)
                {
                    return new JsonResult(new
                    {
                        status = (int)ResponseType.FAILED,
                        message = "Dữ liệu gửi lên không chính xác, vui lòng kiểm tra lại / liên hệ IT",
                    });
                }
                var order = await _orderRepository.GetOrderByID(orderId);
                if (order != null && order.OrderId > 0 && order.BookingInfo != null && order.BookingInfo.Trim() != "")
                {
                    string api_url = ReadFile.LoadConfig().API_URL + ReadFile.LoadConfig().API_Re_Send_Order;
                    var j_param = new Dictionary<string, string> {
                    {"j_param_queue",  order.BookingInfo},  // data json cần push queue              
                    {"queue_name", "queue_checkout_order"},
                };
                    HttpClient httpClient = new HttpClient();

                    var token = CommonHelper.Encode(JsonConvert.SerializeObject(j_param), key_token_api);
                    var content = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("token", token) });
                    var response = await httpClient.PostAsync(api_url, content);
                }

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ReCreateOrder - OrderController: " + ex.ToString());
                return Ok(new
                {
                    status = (int)ResponseType.ERROR,
                    msg = "Có lỗi xảy ra trong quá trình xử lý. Vui lòng liên hệ IT"
                });
            }
            return new JsonResult(new
            {
                status = (int)ResponseType.SUCCESS,
                message = "Gửi yêu cầu thành công",
            });
        }
        [HttpPost]
        public async Task<IActionResult> UpdateContactClient(OrderManualHotelSerivceSummitContactClient model)
        {

            try
            {
                try
                {
                    var num = Convert.ToDouble(model.phone);
                }
                catch (Exception)
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = "Thông tin liên hệ không chính xác, vui lòng kiểm tra lại"
                    });
                }
                if (model.client_id <= 0 || model.order_id <= 0 || model.email == null || model.email.Trim() == "" || !model.email.Contains("@")
                    || model.phone == null || model.phone.Trim() == "" || model.name == null || model.name.Trim() == "")
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = "Thông tin liên hệ không chính xác, vui lòng kiểm tra lại"
                    });
                }
                var client = new ContactClient()
                {
                    Id = model.id,
                    ClientId = model.client_id,
                    CreateDate = DateTime.Now,
                    Email = model.email,
                    Mobile = model.phone,
                    Name = model.name,
                    OrderId = model.order_id
                };
                var id = await _contactClientRepository.UpdateContactClient(client);
                if (id == -1)
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = "Cập nhật thông tin thành viên liên hệ thất bại.Tài khoản của khách hàng chưa được khởi tạo."
                    });
                }
                if (id > 0)
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        msg = "Cập nhật thông tin thành viên liên hệ thành công."
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = "Cập nhật thông tin thành viên liên hệ thất bại."
                    });
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateContactClient - OrderController: " + ex.ToString());
                return Ok(new
                {
                    status = (int)ResponseType.ERROR,
                    msg = "Cập nhật thông tin thành viên không thành công. Vui lòng liên hệ IT"
                });
            }

        }
        public IActionResult ContactClientdetails(long id, long orderId)
        {
            try
            {

                var dataOrder = _iOrderRepositories.GetByOrderId(orderId);
                if (dataOrder != null)
                {
                    ViewBag.ClientId = dataOrder.ClientId;
                    ViewBag.orderId = dataOrder.OrderId;
                }

                ViewBag.id = id;
                if (id != 0)
                {
                    var model = _contactClientRepository.GetByContactClientId(id);
                    return View(model);
                }

                return View();
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ContactClientdetails - OrderController: " + ex);
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> OrderNoSuggestion(string txt_search, string systemtype)
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
                    var data_order = new List<OrderSelectViewModel>();

                    if (Convert.ToInt32(systemtype) < 0 || systemtype == "")
                    {
                        var data = await _orderESRepository.GetOrderNoSuggesstion(txt_search);
                        data_order.AddRange(data.Select(s => new OrderSelectViewModel { orderid = s.orderid, orderno = s.orderno }));
                        return Ok(new
                        {
                            status = (int)ResponseType.SUCCESS,
                            data = data_order,
                            selected = _UserId
                        });
                    }
                    else
                    {
                        var data = await _orderESRepository.GetOrderNoSuggesstion2(txt_search, Convert.ToInt32(systemtype));
                        data_order.AddRange(data.Select(s => new OrderSelectViewModel { orderid = s.orderid, orderno = s.orderno }));

                        return Ok(new
                        {
                            status = (int)ResponseType.SUCCESS,
                            data = data_order,
                            selected = _UserId
                        });
                    }
                }
                else
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        data = new List<OrderSelectViewModel>()
                    });
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("OrderNoSuggestion - OrderController: " + ex.ToString());
                return Ok(new
                {
                    status = (int)ResponseType.SUCCESS,
                    data = new List<OrderSelectViewModel>()
                });
            }

        }
        [HttpPost]
        public async Task<IActionResult> ClientDetail(int orderId)
        {

            try
            {
                if (orderId != 0)
                {
                    var dataOrder = _iOrderRepositories.GetByOrderId(orderId);
                    if (dataOrder != null)
                    {
                        if (dataOrder.AccountClientId != null)
                        {
                            var UserCreateclient = await _clientRepository.GetClientDetailByClientId((long)dataOrder.ClientId);
                            if (UserCreateclient != null)
                            {
                                ViewBag.client = UserCreateclient;
                            }
                        }

                    }
                }
                return PartialView();
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ClientDetail- OrderController" + ex.ToString());
                return PartialView();
            }
        }
        public async Task<IActionResult> Packages(int orderId)
        {

            try
            {
                var st = new Stopwatch();
                st.Start();
                ViewBag.AllowToEdit = false;
                ViewBag.IsAddMoreService = false;

                long _UserId = 0;
                if (HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) != null)
                {
                    _UserId = Convert.ToInt64(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                if (orderId != 0)
                {
                    ViewBag.OrderId = orderId;
                    ViewBag.OrderStatus = (int)OrderStatus.FINISHED;
                    var dataOrder = _iOrderRepositories.GetByOrderId(orderId);
                    ViewBag.IsLock = dataOrder != null && dataOrder.IsLock != null ? dataOrder.IsLock : false;
                    if (dataOrder != null)
                    {
                        //var st2 = new Stopwatch();
                        //st2.Start();
                        //var listPaymentRequest = _paymentRequestRepository.GetListPaymentRequestByOrderId(orderId);
                        //if (listPaymentRequest != null)
                        //{
                        //    ViewBag.PaymentRequestAmount = listPaymentRequest.Sum(s => s.Amount);
                        //}
                        //else
                        //{
                        //    ViewBag.PaymentRequestAmount = 0;
                        //}
                        //st2.Stop();
                        //if (st2.ElapsedMilliseconds > 1000)
                        //    LogHelper.InsertLogTelegram("Packages-OrderController-GetListPaymentRequestByOrderId:" + st2.ElapsedMilliseconds + "ms");
                        var st3 = new Stopwatch();
                        st3.Start();

                        ViewBag.OrderStatus = dataOrder.OrderStatus == null ? (int)OrderStatus.CREATED_ORDER : (byte)dataOrder.OrderStatus;
                        bool is_allow_to_edit = false;
                        if ((dataOrder.SalerId != null && dataOrder.SalerId == _UserId) || (dataOrder.SalerGroupId != null && dataOrder.SalerGroupId.Contains(_UserId.ToString())))
                        {
                            is_allow_to_edit = true;
                        }
                        ViewBag.AllowToEdit = is_allow_to_edit;
                        if (OrderIndentiferService.IsOrderManual(dataOrder.OrderNo))
                        {
                            ViewBag.OrderNo_Type = 1;
                        }
                        else
                        {
                            ViewBag.OrderNo_Type = 0;
                        }
                        if (list_order_status_not_allow_to_edit.Contains((int)dataOrder.OrderStatus))
                        {
                            ViewBag.OrderStatus_Type = 1;
                        }
                        else
                        {
                            ViewBag.OrderStatus_Type = 0;
                        }
                        var data = await _orderRepository.GetAllServiceByOrderId(dataOrder.OrderId);
                        if (data != null)
                            foreach (var item in data)
                            {
                                item.Price += item.Profit;
                                switch (item.Type)
                                {
                                    case "Tour":
                                        {
                                            item.tour = await _tourRepository.GetDetailTourByID(Convert.ToInt32(item.ServiceId));
                                            var note = await _hotelBookingRepositories.GetServiceDeclinesByServiceId(item.ServiceId, 5);
                                            if (note != null)
                                                item.Note = note.UserName + " đã từ chối lý do: " + note.Note;
                                        }
                                        break;
                                    case "Khách sạn":
                                        {
                                            item.Hotel = await _hotelBookingRepositories.GetDetailHotelBookingByID(Convert.ToInt32(item.ServiceId));
                                            var note = await _hotelBookingRepositories.GetServiceDeclinesByServiceId(item.ServiceId, 1);
                                            if (note != null)
                                                item.Note = note.UserName + " đã từ chối lý do: " + note.Note;
                                        }
                                        break;
                                    case "Vé máy bay":
                                        {
                                            item.Flight = await _flyBookingDetailRepository.GetDetailFlyBookingDetailById(Convert.ToInt32(item.ServiceId));
                                            if (item.Flight.GroupBookingId != null)
                                            {
                                                var note = await _hotelBookingRepositories.GetServiceDeclinesByServiceId(item.Flight.GroupBookingId, 3);
                                                if (note != null)
                                                    item.Note = note.UserName + " đã từ chối lý do: " + note.Note;
                                            }
                                        }
                                        break;
                                    case "Dịch vụ khác":
                                        {
                                            item.OtherBooking = await _otherBookingRepository.GetDetailOtherBookingById(Convert.ToInt32(item.ServiceId));
                                            var note = await _hotelBookingRepositories.GetServiceDeclinesByServiceId(item.ServiceId, (int)ServicesType.Other);
                                            if (note != null)
                                                item.Note = note.UserName + " đã từ chối lý do: " + note.Note;
                                        }
                                        break;
                                    case "Vinwonder":
                                        {
                                            item.VinWonderBooking = await _vinWonderBookingRepository.GetDetailVinWonderByBookingId(Convert.ToInt32(item.ServiceId));
                                            var note = await _hotelBookingRepositories.GetServiceDeclinesByServiceId(item.ServiceId, (int)ServicesType.VinWonder);
                                            if (note != null)
                                                item.Note = note.UserName + " đã từ chối lý do: " + note.Note;
                                        }
                                        break;
                                }
                            }
                        st3.Stop();
                        if (st3.ElapsedMilliseconds > 1000)
                            LogHelper.InsertLogTelegram("Packages-OrderController-GetAllServiceByOrderId" + st3.ElapsedMilliseconds + "ms");
                        if (data != null && data.Count > 1)
                        {
                            for (int o = 0; o < data.Count - 1; o++)
                            {

                                if (data[o].Flight != null && data[o + 1].Flight != null)
                                {
                                    if (data[o].Flight.GroupBookingId == data[o + 1].Flight.GroupBookingId && data[o].Flight.Leg != data[o + 1].Flight.Leg)
                                    {
                                        data[o].Flight.StartDistrict2 = data[o + 1].Flight.StartDistrict;
                                        data[o].Flight.EndDistrict2 = data[o + 1].Flight.EndDistrict;
                                        data[o].Flight.Leg2 = 3;
                                        data[o].Flight.BookingCode2 = data[o + 1].Flight.BookingCode;
                                        data[o].Amount = data[o].Flight.Amount + data[o + 1].Flight.Amount;
                                        data[o].EndDate = data[o + 1].EndDate;

                                        data.Remove(data[o + 1]);

                                    }
                                }

                            }
                        }
                        var data2 = await _contractPayRepository.GetContractPayByOrderId(dataOrder.OrderId);
                        if (data2 != null)
                        {
                            ViewBag.paymentAmount = data2.Sum(s => s.AmountPay);
                        }
                        if (dataOrder != null && (dataOrder.OrderStatus == (int)OrderStatus.CREATED_ORDER || dataOrder.OrderStatus == (int)OrderStatus.CONFIRMED_SALE))
                        {
                            ViewBag.IsAddMoreService = true;
                        }
                        ViewBag.data = data;
                        if (data != null)
                            ViewBag.quyCSKH = dataOrder.TotalFundCustomerCare != null ? ((double)dataOrder.TotalFundCustomerCare).ToString("N0") : "0";
                        st.Stop();
                        if (st.ElapsedMilliseconds > 1000)
                            LogHelper.InsertLogTelegram("Packages-OrderController - id:" + orderId + "-" + st.ElapsedMilliseconds + "ms");
                        return PartialView(data);
                    }
                }
                return PartialView();
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Packages-OrderController" + ex.ToString());
                return PartialView();
            }
        }
        public async Task<IActionResult> ContractPay(int orderId)
        {
            try
            {
                if (orderId != 0)
                {

                    var dataOrder = _iOrderRepositories.GetByOrderId(orderId);
                    if (dataOrder != null)
                    {
                        var data = await _contractPayRepository.GetContractPayByOrderId(dataOrder.OrderId);
                        if (data != null)
                        {
                            ViewBag.listPayment = data;
                            ViewBag.paymentAmount = data.Sum(s => s.AmountPay);
                            return PartialView(data);
                        }
                    }
                }

                return PartialView();
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ContractPay - OrderController" + ex.ToString());
                return PartialView();
            }

        }
        public async Task<IActionResult> BillVAT(int orderId)
        {
            try
            {
                var listRequest = _invoiceRequestRepository.GetInvoiceRequestByOrderId(orderId.ToString());
                ViewBag.listRequest = listRequest;
                ViewBag.totalRequest = listRequest.Where(n => n.Status != (int)INVOICE_REQUEST_STATUS.TU_CHOI).Count();
                var orderInfo = _iOrderRepositories.GetByOrderId(orderId);
                ViewBag.orderStatus = orderInfo?.OrderStatus;
                return PartialView();
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("BillVAT-OrderController" + ex.ToString());
                return PartialView();
            }
        }
        public async Task<IActionResult> ListPassenger(int orderId)
        {

            try
            {
                var data = new List<HotelBookingCodeModel>();
                if (orderId != 0)
                {

                    var dataOrder = _iOrderRepositories.GetByOrderId(orderId);

                    if (dataOrder != null)
                    {
                        if (dataOrder.ContactClientId != null)
                        {
                            var ContactClient = _ContactClientRepository.GetByContactClientId((long)dataOrder.ContactClientId);
                            ViewBag.ContactClient = ContactClient;
                        }
                        data = await _hotelBookingCodeRepository.GetListHotelBookingCodeByOrderId(orderId);
                        if (data != null && data.Count > 0)
                        {
                            foreach (var item in data)
                            {
                                var models = await _AttachFileRepository.GetListByType(item.Id, (int)AttachmentType.ServiceCode);
                                item.attachFiles = models;
                            }
                        }
                    }
                }
                return PartialView(data);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ListPassenger-OrderController:" + ex.ToString());
                return PartialView();
            }

        }
        public async Task<IActionResult> SingleInformation(int orderId)
        {
            try
            {
                var dataallcode = _allCodeRepository.GetListByType(AllCodeType.SYSTEM_TYPE);
                var BRANCH_CODE = _allCodeRepository.GetListByType(AllCodeType.BRANCH_CODE);
                var Order_CODE = _allCodeRepository.GetListByType(AllCodeType.ORDER_STATUS);
                var PERMISION_TYPE = _allCodeRepository.GetListByType(AllCodeType.PERMISION_TYPE);
                var PAYMENT_STATUS = _allCodeRepository.GetListByType(AllCodeType.PAYMENT_STATUS);
                if (orderId != 0)
                {

                    var dataOrder = _iOrderRepositories.GetByOrderId(orderId);
                    var client = await _clientRepository.GetClientDetailByClientId((long)dataOrder.ClientId);
                    if (dataOrder != null)
                    {
                        ViewBag.BranchCode = dataOrder.BranchCode;
                        ViewBag.Label = dataOrder.Label;
                        ViewBag.Note = dataOrder.Note;
                        ViewBag.orderNo = dataOrder.OrderNo;
                        ViewBag.OrderStatus = dataOrder.OrderStatus;
                        foreach (var item in dataallcode)
                        {
                            if (dataOrder.SystemType == item.CodeValue)
                            {
                                ViewBag.systemOrder = item.Description;
                                ViewBag.systemType = dataOrder.SystemType;
                            }
                        }
                        foreach (var item in Order_CODE)
                        {
                            if (dataOrder.OrderStatus == item.CodeValue)
                            {
                                ViewBag.statusName = item.Description;
                            }
                        }
                        foreach (var item in PAYMENT_STATUS)
                        {
                            if (dataOrder.PaymentStatus == item.CodeValue)
                            {
                                ViewBag.PaymentStatus = item.Description;
                            }
                        }
                        if (client != null)
                            foreach (var item in PERMISION_TYPE)
                            {
                                if (client.PermisionType == item.CodeValue)
                                {
                                    ViewBag.PermisionType = item.Description;
                                }
                            }



                        ViewBag.OrderStatus = dataOrder.OrderStatus;
                        if (dataOrder.StartDate != null)
                            ViewBag.createTime = Convert.ToDateTime(dataOrder.StartDate).ToString("dd/MM/yyyy HH:mm:ss");
                        if (dataOrder.EndDate != null)
                            ViewBag.ExpriryDate = Convert.ToDateTime(dataOrder.EndDate).ToString("dd/MM/yyyy HH:mm:ss");
                    }

                }
                ViewBag.system = dataallcode;
                ViewBag.BRANCH_CODE = BRANCH_CODE;
                return PartialView();
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SingleInformation-OrderController" + ex.ToString());
                return PartialView();
            }
        }
        public async Task<IActionResult> PersonInCharge(int orderId)
        {
            try
            {
                var _UserId = Convert.ToInt64(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var data_Client = _userRepository.GetAll();
                if (orderId != 0)
                {
                    var current_user = _ManagementUser.GetCurrentUser();
                    ViewBag.IsEdit = false;
                       var dataOrder = _iOrderRepositories.GetByOrderId(orderId);

                    var data = _iOrderRepositories.GetByOrderId(orderId);
                    if (data.SalerId != null)
                    {
                     if(_UserId != data.SalerId && current_user.UserUnderList.Contains(data.SalerId.ToString()))
                        {
                            ViewBag.IsEdit = true;
                        }
                        ViewBag.SalerId = data.SalerId;
                    }
                    List<User> List_SalerGroup = new List<User>();
                    if (data.SalerGroupId != null && data.SalerGroupId != "")
                    {
                        var list_SalerGroupId = Array.ConvertAll(data.SalerGroupId.ToString().Split(','), s => (s).ToString());

                        foreach (var item in list_SalerGroupId)
                        {
                            long id = Convert.ToInt32(item);
                            var SalerGroup = _userRepository.GetClientDetailAsync(id).Result;
                            if (SalerGroup != null)
                            {
                                var ClientName = SalerGroup.FullName.ToString();
                                List_SalerGroup.Add(SalerGroup);
                            }
                            ViewBag.SalerGroup = List_SalerGroup;
                        }
                        ViewBag.SalerGroupId = data.SalerGroupId;
                    }
                }
                ViewBag.dataclient = data_Client;
                return PartialView();
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("PersonInCharge-OrderController" + ex.ToString());
                return PartialView();
            }
        }

        public async Task<IActionResult> SystemInformation(int orderId)
        {
            try
            {
                if (orderId != 0)
                {
                    ViewBag.orderId = orderId;
                    var dataOrder = _iOrderRepositories.GetByOrderId(orderId);
                    if (dataOrder != null)
                    {
                        ViewBag.OrderNo = dataOrder.OrderNo;
                        if (dataOrder.CreatedBy != null)
                            ViewBag.UserCreateId = dataOrder.CreatedBy;
                        if (dataOrder.CreateTime != null)
                            ViewBag.UserCreateTime = ((DateTime)dataOrder.CreateTime).ToString("dd/MM/yyyy HH:mm:ss");
                        if (dataOrder.UpdateLast != null)
                            ViewBag.UserUpdateTime = ((DateTime)dataOrder.UpdateLast).ToString("dd/MM/yyyy HH:mm:ss");
                        if (dataOrder.CreatedBy != null && dataOrder.CreatedBy != 0)
                        {
                            var UserCreateclient = await _userRepository.FindById((int)dataOrder.CreatedBy);
                            if (UserCreateclient != null)
                                ViewBag.UserCreateClientName = UserCreateclient.FullName;

                        }
                        if (dataOrder.UserUpdateId != null && dataOrder.UserUpdateId != 0)
                        {
                            var UserUpdateclient = await _userRepository.FindById((int)dataOrder.UserUpdateId);
                            if (UserUpdateclient != null)
                                ViewBag.UserUpdateClientName = UserUpdateclient.FullName;

                        }

                    }
                }

                return PartialView();
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SystemInformation-OrderController" + ex.ToString());
                return PartialView();
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrderStatus(int status, long OrderId)
        {
            var sst_status = (int)ResponseType.SUCCESS;
            var smg = "Không thành công";
            try
            {
                if (OrderId != 0)
                {
                    var current_user = _ManagementUser.GetCurrentUser();
                    long UpdatedBy = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                    long UserVerify = 0;
                    if (status == (int)OrderStatus.FINISHED)
                    {
                        UserVerify = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                    }
                    var order = await _orderRepository.GetOrderByID(OrderId);
                    var _UserId = Convert.ToInt64(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                    var orderStatus = _allCodeRepository.GetListByType("ORDER_STATUS");
                    var user = await _userRepository.GetById(_UserId);
                    var model = new LogActionModel();
                    model.Type = (int)AttachmentType.OrderDetail;
                    model.LogId = OrderId;
                    model.CreatedUserName = user.FullName;
                    switch (status)
                    {
                        case (int)OrderStatus.FINISHED:
                            {
                                var data = await _contractPayRepository.GetContractPayByOrderId(OrderId);
                                var sumAmonunt = data.Sum(a => a.AmountPay);
                                if (sumAmonunt != order.Amount)
                                {
                                    sst_status = (int)ResponseType.FAILED;
                                    smg = "Đơn hàng chưa được thanh toán đủ";
                                    break;
                                }
                                var data2 = await _orderRepository.UpdateOrderStatus(OrderId, status, UpdatedBy, UserVerify);
                                if (data2 > 0)
                                {
                                    var allCodes = orderStatus.Where(s => s.CodeValue == status).ToList();
                                    model.Log = allCodes[0].Description;
                                    model.Note = user.FullName + " " + allCodes[0].Description + " đơn";
                                    LogActionMongo.InsertLog(model);
                                    string link = "/Order/" + OrderId;

                                    apiService.SendMessage(_UserId.ToString(), ((int)ModuleType.DON_HANG).ToString(), ((int)ActionType.HOAN_THANH).ToString(), order.OrderNo, link, current_user.Role.ToString());
                                    sst_status = (int)ResponseType.SUCCESS;
                                    smg = "Đổi trạng thái thành công";
                                }
                                break;
                            }
                        case (int)OrderStatus.CANCEL:
                            {
                                var data2 = await _orderRepository.UpdateOrderStatus(OrderId, status, UpdatedBy, UserVerify);
                                if (data2 > 0)
                                {
                                    var allCodes = orderStatus.Where(s => s.CodeValue == status).ToList();
                                    model.Log = allCodes[0].Description;
                                    model.Note = user.FullName + " " + allCodes[0].Description + " đơn";
                                    LogActionMongo.InsertLog(model);
                                    var updateOrderService = await _orderRepository.UpdateAllServiceStatusByOrderId(OrderId, (int)ServiceStatus.Cancel);

                                    string link = "/Order/" + OrderId;
                                    apiService.SendMessage(_UserId.ToString(), ((int)ModuleType.DON_HANG).ToString(), ((int)ActionType.HUY).ToString(), order.OrderNo, link, current_user.Role.ToString());
                                    await _orderRepository.UndoContractPayByOrderId(OrderId, (int)UserVerify);
                                    sst_status = (int)ResponseType.SUCCESS;
                                    smg = "Đổi trạng thái thành công";
                                }
                                break;
                            }
                        case (int)OrderStatus.DonRac:
                        case (int)OrderStatus.CONFIRMED_SALE:
                        case (int)OrderStatus.CREATED_ORDER:
                        case (int)OrderStatus.OPERATOR_DECLINE:
                        case (int)OrderStatus.WAITING_FOR_OPERATOR:
                        case (int)OrderStatus.WAITING_FOR_ACCOUNTANT:
                        case (int)OrderStatus.ACCOUNTANT_DECLINE:
                            {
                                var allCodes = orderStatus.Where(s => s.CodeValue == status).ToList();
                                model.Log = allCodes[0].Description;
                                model.Note = user.FullName + " " + allCodes[0].Description + " ";
                                LogActionMongo.InsertLog(model);
                                var data2 = await _orderRepository.UpdateOrderStatus(OrderId, status, UpdatedBy, UserVerify);
                                if (data2 > 0)
                                {
                                    sst_status = (int)ResponseType.SUCCESS;
                                    smg = "Đổi trạng thái thành công";
                                }
                                break;
                            }
                    }

                }

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateOrderStatus - OrderController: " + ex);
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
        public async Task<IActionResult> UpdateOrder(Entities.Models.Order model)
        {
            var sst_status = (int)ResponseType.SUCCESS;
            var smg = "Lưu thông tin không thành công";
            try
            {
                var _UserId = Convert.ToInt64(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var user = await _userRepository.GetById(_UserId);
                if (model != null)
                {
                    var order = await _orderRepository.GetOrderByID(model.OrderId);

                    var mdoelLog = new LogActionModel();
                    mdoelLog.Log = "Cập nhật thông tin đơn";
                    mdoelLog.LogId = model.OrderId;
                    mdoelLog.Type = (int)AttachmentType.OrderDetail; ;
                    mdoelLog.Note = user.FullName + " cập nhật thông tin đơn ";
                    mdoelLog.CreatedUserName = user.FullName;


                    var data2 = _orderRepository.UpdateOrder(model);
                    if (data2 > 0)
                    {
                        if (model.SalerId != null && order.SalerId != model.SalerId)
                        {
                            var sale1 = await _userRepository.GetById((long)order.SalerId);
                            var sale12 = await _userRepository.GetById((long)model.SalerId);
                            var sale1fullname = sale1 != null ? sale1.FullName.ToString() : "N/A";
                            var sale12fullname = sale12 != null ? sale12.FullName.ToString() : "N/A";
                            mdoelLog.Note += user.FullName + " đổi nhân viên chính " + sale1fullname + " thành " + sale12fullname + " ";
                        }
                        if (model.SalerGroupId != null && model.SalerGroupId != "" && order.SalerGroupId != model.SalerGroupId)
                        {
                            var list_SalerGroupId = Array.ConvertAll(model.SalerGroupId.ToString().Split(','), s => (s).ToString());
                            var SalerGroupFullName = "";
                            foreach (var item in list_SalerGroupId)
                            {
                                long id = Convert.ToInt32(item);
                                var SalerGroup = _userRepository.GetClientDetailAsync(id).Result;
                                if (SalerGroup != null)
                                {
                                    var ClientName = SalerGroup.FullName.ToString();
                                    SalerGroupFullName += ClientName + ", ";
                                }

                            }
                            mdoelLog.Note += user.FullName + " đổi nhân viên phụ thành " + SalerGroupFullName + " ";
                        }
                        sst_status = (int)ResponseType.SUCCESS;
                        smg = "Lưu thông tin thành công";

                        LogActionMongo.InsertLog(mdoelLog);
                    }
                }

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateOrderStatus - OrderController: " + ex);
                sst_status = (int)ResponseType.ERROR;
                smg = "Đã xảy ra lỗi, vui lòng liên hệ IT";
            }

            return Ok(new
            {
                sst_status = sst_status,
                smg = smg
            });
        }
        public async Task<IActionResult> PopupAddSale(long id)
        {

            try
            {
                ViewBag.orderid = id;
                var UserId = Convert.ToInt64(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var user = await _userRepository.GetById(UserId);
                ViewBag.id = UserId;
                ViewBag.fullname = user.FullName;
                ViewBag.email = user.Email;

                return PartialView();
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("DetailUserAgent - OrderController: " + ex);
                return PartialView();
            }

        }
        public async Task<IActionResult> PopupFly(long Orderid)
        {

            try
            {
                ViewBag.Body = await _emailService.GetOrderFlyTemplateBody(Orderid, "", "", true);
                return PartialView();
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("PopupFly - OrderController: " + ex);
                return PartialView();
            }

        }
        public async Task<IActionResult> SendEmail(long id, long Orderid, int type, string group_booking_id = "")
        {

            try
            {
                var dataOrder = await _iOrderRepositories.GetDetailOrderByOrderId(Convert.ToInt32(Orderid));
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

                switch (type)
                {

                    case (int)EmailType.DON_HANG:
                        {
                            var model = new SendEmailViewModel();
                            model = null;
                            ViewBag.EmailBody = await _emailService.OrderTemplateBody(model, Orderid, "", true);

                        }
                        break;

                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SendEmail - OrderController: " + ex);
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
                LogHelper.InsertLogTelegram("ConfirmSendEmail - OrderController: " + ex);
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
        public async Task<IActionResult> UpdateOrderFinishPayment(long OrderId, int type)
        {
            var sst_status = (int)ResponseType.ERROR;
            var smg = "Không thành công";

            try
            {
                var current_user = _ManagementUser.GetCurrentUser();
                if (OrderId != 0)
                {
                    var listrole = new List<int>();
                    var model = new GenericViewModel<ContractViewModel>();
                    var dataOrder = _iOrderRepositories.GetByOrderId(OrderId);
                    model = await _contractRepository.GetListByType((long)dataOrder.ClientId, 1, 10);
                    /* var Listcontract = model.ListData.ToList();
                     Listcontract = Listcontract.Where(n => n.ContractStatus == ContractStatus.CHO_CHI && n.ExpireDate > DateTime.Now).ToList();
                     var DetailContract = await _contractRepository.GetDetailContract((int)Listcontract[0].ContractId);
                     if (Listcontract != null && DetailContract != null)
                     {*/
                    if (dataOrder.ProductService != null)
                    {
                        var list = Array.ConvertAll(dataOrder.ProductService.Split(','), int.Parse);

                        foreach (var item in list)
                        {
                            switch (Convert.ToInt32(item))
                            {
                                case (int)ServicesType.VINHotelRent:
                                case (int)ServicesType.OthersHotelRent:
                                    {

                                        var listHotel = await _hotelBookingRepositories.GetListByOrderId(OrderId);
                                        var sum = listHotel.Where(s => (s.Status == (int)ServiceStatus.New || s.Status == (int)ServiceStatus.Decline)).Sum(s => s.TotalAmount);
                                        var isCheck = await _orderRepository.IsClientAllowedToDebtNewService(sum, (long)dataOrder.ClientId, dataOrder.OrderId, (int)ServiceType.BOOK_HOTEL_ROOM_VIN);
                                        if (isCheck == 0)
                                        {
                                            return Ok(new
                                            {
                                                sst_status = (int)ResponseType.ERROR,
                                                smg = "Dịch vụ khách sạn vượt quá hạn mức công nợ"
                                            });
                                        }

                                    }
                                    break;
                                case (int)ServicesType.FlyingTicket:
                                    {

                                        var fly_list = _flyBookingDetailRepository.GetListByOrderId(OrderId).ToList();
                                        var sum = fly_list.Where(s => (s.Status == (int)ServiceStatus.New || s.Status == (int)ServiceStatus.Decline)).Sum(s => s.Amount);
                                        var isCheck = await _orderRepository.IsClientAllowedToDebtNewService(sum, (long)dataOrder.ClientId, dataOrder.OrderId, (int)ServiceType.PRODUCT_FLY_TICKET);
                                        if (isCheck == 0)
                                        {

                                            return Ok(new
                                            {
                                                sst_status = (int)ResponseType.ERROR,
                                                smg = "Dịch vụ vé máy bay vượt quá hạn mức công nợ"
                                            });
                                        }


                                    }
                                    break;
                                case (int)ServicesType.Tourist:
                                    {
                                        var listtour = await _tourRepository.GetTourByOrderId(OrderId);
                                        var sum = listtour.Where(s => (s.Status == (int)ServiceStatus.New || s.Status == (int)ServiceStatus.Decline)).Sum(s => s.Amount);
                                        var isCheck = await _orderRepository.IsClientAllowedToDebtNewService((double)sum, (long)dataOrder.ClientId, dataOrder.OrderId, (int)ServiceType.Tour);
                                        if (isCheck == 0)
                                        {

                                            return Ok(new
                                            {
                                                sst_status = (int)ResponseType.ERROR,
                                                smg = "Dịch vụ tour vượt quá hạn mức công nợ"
                                            });
                                        }

                                    }
                                    break;
                                case (int)ServicesType.VinWonder:
                                    {
                                        var listtour = await _vinWonderBookingRepository.GetVinWonderBookingByOrderId(OrderId);
                                        var sum = listtour.Where(s => (s.Status == (int)ServiceStatus.New || s.Status == (int)ServiceStatus.Decline)).Sum(s => s.Amount);
                                        var isCheck = await _orderRepository.IsClientAllowedToDebtNewService((double)sum, (long)dataOrder.ClientId, dataOrder.OrderId, (int)ServiceType.VinWonder);
                                        if (isCheck == 0)
                                        {

                                            return Ok(new
                                            {
                                                sst_status = (int)ResponseType.ERROR,
                                                smg = "Dịch vụ VinWonder vượt quá hạn mức công nợ"
                                            });
                                        }

                                    }
                                    break;
                                case (int)ServicesType.VehicleRent:
                                case (int)ServicesType.Other:
                                    {
                                        var listtour = await _otherBookingRepository.getListOtherBookingByOrderId(OrderId);
                                        var sum = listtour.Where(s => (s.Status == (int)ServiceStatus.New || s.Status == (int)ServiceStatus.Decline)).Sum(s => s.Amount);
                                        var isCheck = await _orderRepository.IsClientAllowedToDebtNewService((double)sum, (long)dataOrder.ClientId, dataOrder.OrderId, (int)ServiceType.Other);
                                        if (isCheck == 0)
                                        {

                                            return Ok(new
                                            {
                                                sst_status = (int)ResponseType.ERROR,
                                                smg = "Dịch vụ khác vượt quá hạn mức công nợ"
                                            });
                                        }

                                    }
                                    break;
                            }
                        }

                    }
                    var _UserId = Convert.ToInt64(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                    var user = await _userRepository.GetById(_UserId);
                    var orderStatus = _allCodeRepository.GetListByType("ORDER_STATUS");
                    var allCodes = orderStatus.Where(s => s.CodeValue == (int)OrderStatus.WAITING_FOR_OPERATOR).ToList();
                    var modelLog = new LogActionModel();
                    modelLog.Type = (int)AttachmentType.OrderDetail;
                    modelLog.LogId = OrderId;
                    modelLog.CreatedUserName = user.FullName;
                    modelLog.Log = allCodes[0].Description;
                    modelLog.Note = user.FullName + " công nợ đơn hàng";
                    var data = await _orderRepository.GetAllServiceByOrderId(dataOrder.OrderId);
                    if (data != null)
                        foreach (var item in data)
                        {
                            item.Price += item.Profit;
                            if (item.Type.Equals("Tour"))
                            {
                                item.tour = await _tourRepository.GetDetailTourByID(Convert.ToInt32(item.ServiceId));
                            }
                            if (item.Type.Equals("Khách sạn"))
                            {
                                item.Hotel = await _hotelBookingRepositories.GetDetailHotelBookingByID(Convert.ToInt32(item.ServiceId));
                            }
                            if (item.Type.Equals("Vé máy bay"))
                            {
                                item.Flight = await _flyBookingDetailRepository.GetDetailFlyBookingDetailById(Convert.ToInt32(item.ServiceId));
                            }
                            if (item.Type.Equals("Dịch vụ khác"))
                            {
                                item.OtherBooking = await _otherBookingRepository.GetDetailOtherBookingById(Convert.ToInt32(item.ServiceId));
                            }
                            if (item.Type.Equals("Vinwonder"))
                            {
                                item.VinWonderBooking = await _vinWonderBookingRepository.GetDetailVinWonderByBookingId(Convert.ToInt32(item.ServiceId));
                            }
                        }
                    if (data != null && data.Count > 1)
                    {
                        for (int o = 0; o < data.Count - 1; o++)
                        {

                            if (data[o].Flight != null && data[o + 1].Flight != null)
                            {
                                if (data[o].Flight.GroupBookingId == data[o + 1].Flight.GroupBookingId && data[o].Flight.Leg != data[o + 1].Flight.Leg)
                                {
                                    data[o].Flight.StartDistrict2 = data[o + 1].Flight.StartDistrict;
                                    data[o].Flight.EndDistrict2 = data[o + 1].Flight.EndDistrict;
                                    data[o].Flight.Leg2 = 3;
                                    data[o].Flight.BookingCode2 = data[o + 1].Flight.BookingCode;
                                    data[o].Amount = data[o].Flight.Amount + data[o + 1].Flight.Amount;
                                    data[o].EndDate = data[o + 1].EndDate;

                                    data.Remove(data[o + 1]);

                                }
                            }

                        }
                    }
                    if (type == 0)
                    {
                        long status = Convert.ToInt32((int)OrderStatus.WAITING_FOR_OPERATOR);
                        var data2 = await _orderRepository.UpdateOrderFinishPayment(OrderId, status);
                        if (data2 >= 0)
                        {

                            string link = "/Order/" + OrderId;

                            foreach (var item in data)
                            {
                                if (item.Type.Equals("Tour"))
                                {
                                    apiService.SendMessage(_UserId.ToString(), ((int)ModuleType.DON_HANG).ToString(), ((int)ActionType.DUYET_DICH_VU).ToString(), item.OrderNo, link, current_user.Role, item.tour.ServiceCode);
                                }
                                if (item.Type.Equals("Khách sạn"))
                                {
                                    apiService.SendMessage(_UserId.ToString(), ((int)ModuleType.DON_HANG).ToString(), ((int)ActionType.DUYET_DICH_VU).ToString(), item.OrderNo, link, current_user.Role, item.Hotel[0].ServiceCode);
                                }
                                if (item.Type.Equals("Vé máy bay"))
                                {
                                    apiService.SendMessage(_UserId.ToString(), ((int)ModuleType.DON_HANG).ToString(), ((int)ActionType.DUYET_DICH_VU).ToString(), item.OrderNo, link, current_user.Role, item.Flight.ServiceCode);
                                }
                                if (item.Type.Equals("Dịch vụ khác"))
                                {
                                    apiService.SendMessage(_UserId.ToString(), ((int)ModuleType.DON_HANG).ToString(), ((int)ActionType.DUYET_DICH_VU).ToString(), item.OrderNo, link, current_user.Role, item.OtherBooking[0].ServiceCode);
                                }
                                if (item.Type.Equals("Vinwonder"))
                                {
                                    apiService.SendMessage(_UserId.ToString(), ((int)ModuleType.DON_HANG).ToString(), ((int)ActionType.DUYET_DICH_VU).ToString(), item.OrderNo, link, current_user.Role, item.VinWonderBooking[0].ServiceCode);
                                }
                            }
                            var modelEmail = new SendEmailViewModel();
                            modelEmail.Orderid = OrderId;
                            modelEmail.ServiceType = (int)EmailType.SaleDH;
                            var attach_file = new List<AttachfileViewModel>();
                            bool resulstSendMail = await _emailService.SendEmail(modelEmail, attach_file);
                            sst_status = (int)ResponseType.SUCCESS;
                            smg = "Công nợ thành công thành công";

                            LogActionMongo.InsertLog(modelLog);
                        }
                    }
                    else
                    {

                        var data2 = await _orderRepository.UpdateServiceStatusByOrderId(OrderId, (long)ServiceStatus.Decline, (long)ServiceStatus.OnExcution);
                        if (data2 >= 0)
                        {

                            string link = "/Order/" + OrderId;
                            foreach (var item in data)
                            {
                                if (item.Type.Equals("Tour"))
                                {
                                    apiService.SendMessage(_UserId.ToString(), ((int)ModuleType.DON_HANG).ToString(), ((int)ActionType.DUYET_DICH_VU).ToString(), item.OrderNo, link, current_user.Role, item.tour.ServiceCode);
                                }
                                if (item.Type.Equals("Khách sạn"))
                                {
                                    apiService.SendMessage(_UserId.ToString(), ((int)ModuleType.DON_HANG).ToString(), ((int)ActionType.DUYET_DICH_VU).ToString(), item.OrderNo, link, current_user.Role, item.Hotel[0].ServiceCode);
                                }
                                if (item.Type.Equals("Vé máy bay"))
                                {
                                    apiService.SendMessage(_UserId.ToString(), ((int)ModuleType.DON_HANG).ToString(), ((int)ActionType.DUYET_DICH_VU).ToString(), item.OrderNo, link, current_user.Role, item.Flight.ServiceCode);
                                }
                                if (item.Type.Equals("Dịch vụ khác"))
                                {
                                    apiService.SendMessage(_UserId.ToString(), ((int)ModuleType.DON_HANG).ToString(), ((int)ActionType.DUYET_DICH_VU).ToString(), item.OrderNo, link, current_user.Role, item.OtherBooking[0].ServiceCode);
                                }
                                if (item.Type.Equals("Vinwonder"))
                                {
                                    apiService.SendMessage(_UserId.ToString(), ((int)ModuleType.DON_HANG).ToString(), ((int)ActionType.DUYET_DICH_VU).ToString(), item.OrderNo, link, current_user.Role, item.VinWonderBooking[0].ServiceCode);
                                }
                            }
                            sst_status = (int)ResponseType.SUCCESS;
                            smg = "Công nợ thành công thành công";
                            LogActionMongo.InsertLog(modelLog);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateOrderFinishPayment - OrderController: " + ex);
                sst_status = (int)ResponseType.ERROR;
                smg = "Đã xảy ra lỗi, vui lòng liên hệ IT";
            }

            return Ok(new
            {
                sst_status = sst_status,
                smg = smg
            });
        }
        public async Task<IActionResult> DeleteService(int service_type, string id)
        {

            var status = (int)ResponseType.ERROR;
            var msg = "Không thành công";

            try
            {
                long order_id = 0;
                switch (service_type)
                {
                    case (int)ServiceType.BOOK_HOTEL_ROOM_VIN:
                    case (int)ServiceType.BOOK_HOTEL_ROOM:
                        {
                            long hotel_booking_id = 0;
                            try
                            {
                                hotel_booking_id = Convert.ToInt64(id);
                            }
                            catch { }
                            var hotel = await _hotelBookingRepositories.GetHotelBookingByID(hotel_booking_id);
                            if (hotel != null && hotel.Id > 0)
                            {
                                order_id = (long)hotel.OrderId;
                            }
                            var success = await _hotelBookingRepositories.DeleteHotelBookingByID(hotel_booking_id);
                            break;
                        }
                    case (int)ServiceType.PRODUCT_FLY_TICKET:
                        {
                            var fly = await _flyBookingDetailRepository.GetListByGroupFlyID(id);

                            if (fly != null && fly.Count > 0 && fly[0].Id > 0)
                            {
                                order_id = (long)fly[0].OrderId;
                            }
                            var success = await _flyBookingDetailRepository.DeleteFlyBookingByID(id);

                            break;
                        }
                    case (int)ServiceType.Tour:
                        {
                            long tour_id = 0;
                            try
                            {
                                tour_id = Convert.ToInt64(id);
                            }
                            catch { }
                            var tour = await _tourRepository.GetDetailTourByID(tour_id);
                            if (tour != null && tour.Id > 0)
                            {
                                order_id = (long)tour.OrderId;
                            }
                            var success = await _tourRepository.DeleteTourByID(tour_id);
                            break;
                        }
                    case (int)ServiceType.Other:
                        {
                            long other_id = 0;
                            try
                            {
                                other_id = Convert.ToInt64(id);
                            }
                            catch { }
                            var other_service = await _otherBookingRepository.GetOtherBookingById(other_id);
                            if (other_service != null && other_service.Id > 0)
                            {
                                order_id = other_service.OrderId;
                            }
                            var success = await _otherBookingRepository.DeleteOtherBookingById(other_id);

                            break;
                        }
                    case (int)ServiceType.VinWonder:
                        {
                            long other_id = 0;
                            try
                            {
                                other_id = Convert.ToInt64(id);
                            }
                            catch { }
                            var other_service = _vinWonderBookingRepository.GetVinWonderBookingById(other_id);
                            if (other_service != null && other_service.Id > 0)
                            {
                                order_id = (long)other_service.OrderId;
                            }
                            var success = await _vinWonderBookingRepository.DeleteVinWonderBookingByID(other_id);

                            break;
                        }
                }
                #region Update Order Amount:
                if (order_id > 0)
                {
                    var _UserLogin = 0;
                    if (HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) != null)
                    {
                        _UserLogin = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                    }
                    await _orderRepository.UpdateOrderDetail(order_id, _UserLogin);
                }
                #endregion
                return Ok(new
                {
                    status = (int)ResponseType.SUCCESS,
                    msg = "Xóa dịch vụ thành công"
                });

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("DeleteService - OrderController: " + ex);
                status = (int)ResponseType.ERROR;
                msg = "Đã xảy ra lỗi, vui lòng liên hệ IT";
            }
            return Ok(new
            {
                status = status,
                msg = msg
            });
        }
        public async Task<IActionResult> CancelService(int service_type, string id)
        {

            var status = (int)ResponseType.ERROR;
            var msg = "Không thành công";
            try
            {
                long order_id = 0;
                var _UserLogin = 0;
                if (HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) != null)
                {
                    _UserLogin = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                switch (service_type)
                {
                    case (int)ServiceType.BOOK_HOTEL_ROOM_VIN:
                    case (int)ServiceType.BOOK_HOTEL_ROOM:
                        {

                            long hotel_booking_id = 0;
                            try
                            {
                                hotel_booking_id = Convert.ToInt64(id);
                            }
                            catch { }
                            HotelBooking hotel = await _hotelBookingRepositories.GetHotelBookingByID(hotel_booking_id);
                            if (hotel != null && hotel.Id > 0)
                            {
                                order_id = (long)hotel.OrderId;
                                var success = await _hotelBookingRepositories.CancelHotelBookingByID(hotel_booking_id, _UserLogin);
                                //var list_contract_pay = await _contractPayRepository.GetContractPayByOrderId((long)hotel.OrderId);
                                //if (list_contract_pay != null && list_contract_pay.Count > 0)
                                //{
                                //    foreach (var contract in list_contract_pay)
                                //    {
                                //        _contractPayRepository.UndoContractPayByCancelService(contract.PayId, (long)hotel.OrderId, _UserLogin);
                                //    }
                                //}
                            }

                            break;
                        }
                    case (int)ServiceType.PRODUCT_FLY_TICKET:
                        {
                            var fly = await _flyBookingDetailRepository.GetListByGroupFlyID(id);

                            if (fly != null && fly.Count > 0 && fly[0].Id > 0)
                            {
                                order_id = (long)fly[0].OrderId;
                                var success = await _flyBookingDetailRepository.CancelHotelBookingByID(id, _UserLogin);

                            }
                            break;
                        }
                    case (int)ServiceType.Tour:
                        {
                            long tour_id = 0;
                            try
                            {
                                tour_id = Convert.ToInt64(id);
                            }
                            catch { }
                            Tour tour = await _tourRepository.GetTourById(tour_id);
                            if (tour != null && tour.Id > 0)
                            {
                                order_id = (long)tour.OrderId;
                                var success = await _tourRepository.CancelTourByID(tour_id, _UserLogin);

                            }
                            break;
                        }
                    case (int)ServiceType.Other:
                        {
                            long other_id = 0;
                            try
                            {
                                other_id = Convert.ToInt64(id);
                            }
                            catch { }
                            var other_service = await _otherBookingRepository.GetOtherBookingById(other_id);
                            if (other_service != null && other_service.Id > 0)
                            {
                                order_id = other_service.OrderId;
                                var success = await _otherBookingRepository.CancelOtherBookingById(other_service.Id, _UserLogin);

                            }
                            break;
                        }
                    case (int)ServiceType.VinWonder:
                        {
                            long other_id = 0;
                            try
                            {
                                other_id = Convert.ToInt64(id);
                            }
                            catch { }
                            var vinwonder_service = _vinWonderBookingRepository.GetVinWonderBookingById(other_id);
                            if (vinwonder_service != null && vinwonder_service.Id > 0)
                            {
                                order_id = (long)vinwonder_service.OrderId;
                                var success = await _vinWonderBookingRepository.CancelVinWonderByID(vinwonder_service.Id, _UserLogin);

                            }
                            break;
                        }
                }
                #region Update Order Amount:
                if (order_id > 0)
                {

                    await _orderRepository.UpdateOrderDetail(order_id, _UserLogin);
                }
                #endregion
                return Ok(new
                {
                    status = (int)ResponseType.SUCCESS,
                    msg = "Hủy dịch vụ thành công"
                });


            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CancelService - OrderController: " + ex);
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
        public async Task<IActionResult> RePushOrderServiceToOperator(long order_id)
        {

            try
            {
                if (order_id <= 0)
                {
                    return new JsonResult(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = "Dữ liệu gửi lên không chính xác, vui lòng kiểm tra lại / liên hệ IT",
                    });
                }
                var order = await _orderRepository.GetOrderByID(order_id);
                if (order == null || order.OrderId <= 0)
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = "Dữ liệu gửi lên không chính xác, vui lòng kiểm tra lại / liên hệ IT",
                    });
                }
                await _orderRepository.RePushDeclineServiceToOperator(order_id);

                long UpdatedBy = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var user = await _userRepository.GetById(UpdatedBy);
                var modelLog = new LogActionModel();
                modelLog.LogId = Convert.ToInt32(order_id);
                modelLog.Type = (int)AttachmentType.OrderDetail;
                modelLog.CreatedUserName = user.FullName;
                modelLog.Log = "Chờ điều hành duyệt";
                modelLog.Note = user.FullName + " chuyển dịch vụ lên điều hành";
                LogActionMongo.InsertLog(modelLog);
                return Ok(new
                {
                    status = (int)ResponseType.SUCCESS,
                    msg = "Gửi yêu cầu thành công",
                });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("RePushOrderServiceToOperator - OrderController: " + ex.ToString());
                return Ok(new
                {
                    status = (int)ResponseType.ERROR,
                    msg = "Có lỗi xảy ra trong quá trình xử lý. Vui lòng liên hệ IT"
                });
            }

        }
        [HttpPost]
        public async Task<IActionResult> ExportExcel(OrderViewSearchModel searchModel, FieldOrder field)
        {
            try
            {

                int _UserId = 0;
                if (HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) != null)
                {
                    _UserId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                string _FileName = StringHelpers.GenFileName("Danh sách đơn hàng", _UserId, "xlsx");
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

                var rsPath = await _orderRepository.ExportDeposit(searchModel, FilePath, field, searchModel.PageIndex, searchModel.pageSize);

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
                LogHelper.InsertLogTelegram("ExportExcel - OrderController: " + ex);
                return new JsonResult(new
                {
                    isSuccess = false,
                    message = ex.Message.ToString()
                });
            }
        }

        public async Task<IActionResult> PopupOrderStatus(long OrderId)
        {

            try
            {
                if (OrderId != 0)
                {
                    var orderStatus = _allCodeRepository.GetListByType("ORDER_STATUS");
                    var PAYMENT_STATUS = _allCodeRepository.GetListByType("PAYMENT_STATUS");
                    var Service_STATUS = _allCodeRepository.GetListByType("BOOKING_HOTEL_ROOM_STATUS");
                    ViewBag.orderStatus = orderStatus;
                    ViewBag.PAYMENT_STATUS = PAYMENT_STATUS;
                    ViewBag.Service_STATUS = Service_STATUS;
                    var order = await _orderRepository.GetOrderByID(OrderId);

                    if (order != null)
                    {
                        var Service = await _orderRepository.GetAllServiceByOrderId(OrderId);
                        if (Service != null && Service.Count > 1)
                        {
                            for (int o = 0; o < Service.Count - 1; o++)
                            {

                                if (Service[o].Type == "Vé máy bay" && Service[o + 1].Type == "Vé máy bay")
                                {
                                    if (Service[o].ServiceCode == Service[o + 1].ServiceCode)
                                    {

                                        Service[o].ServiceId = Service[o].ServiceId + "," + Service[o + 1].ServiceId;

                                        Service.Remove(Service[o + 1]);

                                    }
                                }

                            }
                        }
                        ViewBag.Service = Service;
                        ViewBag.Order = order;
                    }
                }
                return PartialView();
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("PopupOrderStatus - OrderController: " + ex);

            }
            return PartialView();

        }
        [HttpPost]
        public async Task<IActionResult> UpdateOrderandServiceStatus(OrderandService model)
        {
            var status = (int)ResponseType.ERROR;
            var smg = "Cập nhật không thành công";
            try
            {
                if (model.OrderId != null)
                {
                    long UpdatedBy = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                    var user = await _userRepository.GetById(UpdatedBy);
                    var modelLog = new LogActionModel();
                    modelLog.LogId = Convert.ToInt32(model.OrderId);
                    modelLog.Type = (int)AttachmentType.OrderDetail;
                    modelLog.CreatedUserName = user.FullName;
                    modelLog.Log = "Cập nhật trạng thái đơn";
                    modelLog.Note = user.FullName + " cập nhật trạng thái đơn";



                    long UserVerify = 0;

                    var modelOrder = new Entities.Models.Order();
                    modelOrder.OrderId = Convert.ToInt32(model.OrderId);
                    modelOrder.OrderStatus = (byte?)Convert.ToInt32(model.OrderStatus);
                    modelOrder.PaymentStatus = Convert.ToInt32(model.PaymentStatus);
                    var updateOrder = _orderRepository.UpdateOrder(modelOrder);
                    //var data2 = await _orderRepository.UpdateOrderStatus(Convert.ToInt32(model.OrderId), Convert.ToInt32(model.OrderStatus), UpdatedBy, UserVerify);
                    if (updateOrder > 0)
                    {

                        status = (int)ResponseType.SUCCESS;
                        smg = "Cập nhật thành công";
                        LogActionMongo.InsertLog(modelLog);
                    }
                    var data = await _orderRepository.GetAllServiceByOrderId(Convert.ToInt32(model.OrderId));
                    if (data != null)
                        foreach (var item in data)
                        {

                            switch (item.Type)
                            {
                                case "Tour":
                                    {
                                        if (model.ListTour != null && model.ListTour.Count > 0)
                                        {
                                            foreach (var i in model.ListTour)
                                            {
                                                var update = await _tourRepository.UpdateTourStatus(Convert.ToInt32(i.Id), Convert.ToInt32(i.Status));
                                            }
                                        }

                                    }
                                    break;
                                case "Khách sạn":
                                    {
                                        if (model.ListHotel != null && model.ListHotel.Count > 0)
                                        {
                                            foreach (var i in model.ListHotel)
                                            {
                                                var update = await _hotelBookingRepositories.UpdateHotelBookingStatus(i.Id, Convert.ToInt32(i.Status));
                                            }
                                        }

                                    }
                                    break;
                                case "Vé máy bay":
                                    {
                                        if (model.ListFly != null && model.ListFly.Count > 0)
                                        {
                                            foreach (var i in model.ListFly)
                                            {
                                                var update = await _flyBookingDetailRepository.UpdateServiceStatus(i.Status, i.GroupBookingId, Convert.ToInt32(UpdatedBy));
                                            }
                                        }

                                    }
                                    break;
                                case "Dịch vụ khác":
                                    {
                                        if (model.ListOther != null && model.ListOther.Count > 0)
                                        {
                                            foreach (var i in model.ListOther)
                                            {
                                                var update = await _otherBookingRepository.UpdateServiceStatus(i.Status, i.Id, Convert.ToInt32(UpdatedBy));

                                            }
                                        }
                                    }
                                    break;
                                case "Vinwonder":
                                    {
                                        if (model.ListVin != null && model.ListVin.Count > 0)
                                        {
                                            foreach (var i in model.ListVin)
                                            {
                                                var update = await _vinWonderBookingRepository.UpdateServiceStatus((int)i.Status, i.Id, Convert.ToInt32(UpdatedBy));

                                            }
                                        }
                                    }
                                    break;
                            }
                        }
                }

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateOrderStatus - OrderController: " + ex);
                status = (int)ResponseType.ERROR;
                smg = "Đã xảy ra lỗi, vui lòng liên hệ IT";
            }

            return Ok(new
            {
                status = status,
                smg = smg
            });
        }
        [HttpPost]
        public async Task<IActionResult> BoongKingCodeSuggestion(string txt_search)
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

                    var data = await _boongKingCodeESRepository.BoongKingCodeSuggesstion(txt_search);
                    var data2 = data.GroupBy(s => s.bookingcode).Select(i => i.First()).ToList();
                    foreach (var item in data2)
                    {
                        item.listorderid = string.Join(',', data.Where(s => s.bookingcode == item.bookingcode).Select(i => i.orderid).ToList());
                    }

                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        data = data2,
                        selected = _UserId
                    });

                }
                else
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        data = new List<BoongKingCodeESViewModel>()
                    });
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("OrderNoSuggestion - OrderController: " + ex.ToString());
                return Ok(new
                {
                    status = (int)ResponseType.SUCCESS,
                    data = new List<BoongKingCodeESViewModel>()
                });
            }
        }
        public async Task<IActionResult> getLog(long orderid)
        {

            try
            {

                var model = new LogActionModel();
                model.Type = (int)AttachmentType.OrderDetail;
                model.LogId = orderid;
                var data = LogActionMongo.GetListLogActions(model);

                return PartialView(data);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("getLog - OrderController: " + ex.ToString());

            }
            return PartialView();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrderIsLock(long OrderId, long IsLock)
        {
            var status = (int)ResponseType.SUCCESS;
            var smg = "Mở sổ đơn hàng không thành công";
            try
            {
                if (OrderId != 0)
                {
                    long UpdatedBy = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                    var orderDetail = await _orderRepository.GetOrderByID(OrderId);
                    var user = await _userRepository.GetById(UpdatedBy);
                    var update = await _orderRepository.UpdateBookClosingByOrderId(OrderId, IsLock, UpdatedBy);
                    if (update > 0)
                    {
                        var model = new LogActionModel();
                        model.Type = (int)AttachmentType.OrderDetail;
                        model.LogId = OrderId;
                        model.CreatedUserName = user.FullName;
                        if (IsLock > 0)
                        {

                            smg = "khóa sổ đơn hàng thành công";
                            model.Log = "khóa sổ đơn hàng";
                            model.Note = user.FullName + " khóa sổ đơn hàng " + ((double)orderDetail.Amount).ToString("N0");
                          //var ListOrderBookClosing= await _orderRepository.GetListOrderBookClosingByOrderId(OrderId);

                          //  var OrderBookClosing = new OrderBookClosingViewModel
                          //  {
                          //      FromDateStr = ListOrderBookClosing[0].FromDate.ToString("dd/MM/yyyy"),
                          //      ToDateStr = ListOrderBookClosing[0].ToDate.ToString("dd/MM/yyyy"),
                          //      UserFinalize= UpdatedBy,
                          //  };
                          //  var date = DateUtil.StringToDate(OrderBookClosing.ToDateStr);
                          //  OrderBookClosing.ToDate = ((DateTime)date).AddHours(23).AddMinutes(59).AddSeconds(59);
                          //  var Request = await _orderRepository.OrderBookClosing(OrderBookClosing);

                            LogActionMongo.InsertLog(model);
                        }
                        else
                        {
                            smg = "Mở sổ đơn hàng thành công";
                            model.Log = "Mở sổ đơn hàng";
                            model.Note = user.FullName + " Mở sổ đơn hàng " + ((double)orderDetail.Amount).ToString("N0");
                            LogActionMongo.InsertLog(model);
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateOrderIsLock - OrderController: " + ex);
                status = (int)ResponseType.ERROR;
                smg = "Đã xảy ra lỗi, vui lòng liên hệ IT";
            }

            return Ok(new
            {
                status = status,
                smg = smg
            });
        }
        public async Task<IActionResult> GetLogOrderIsLock(long orderid)
        {

            try
            {
                var ListOrderBookClosing = await _orderRepository.GetListOrderBookClosingByOrderId(orderid);
                return PartialView(ListOrderBookClosing);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetLogOrderIsLock - OrderController: " + ex.ToString());

            }
            return PartialView();
        }
    }
}

