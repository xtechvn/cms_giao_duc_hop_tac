using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Caching.Elasticsearch;
using Entities.Models;
using Entities.ViewModels;
using Entities.ViewModels.ElasticSearch;
using Entities.ViewModels.HotelBookingCode;
using Entities.ViewModels.SetServices;
using ENTITIES.ViewModels.ElasticSearch;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Repositories.IRepositories;
using Utilities;
using Utilities.Contants;
using GDHT.CMS.Service;
using WEB.CMS.Customize;
using static Utilities.DepositHistoryConstant;

namespace GDHT.CMS.Controllers.SetService.Fly
{
    [CustomAuthorize]
    public class SetServiceController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IOrderRepositor _orderRepository;
        private readonly IOrderRepository _orderRepository2;
        private readonly IContactClientRepository _contactClientRepository;
        private IFlyBookingDetailRepository _flyBookingDetailRepository;
        private readonly OrderESRepository _orderESRepository;
        private readonly FlyBookingESRepository _flyBookingESRepository;
        private readonly IAllCodeRepository _allCodeRepository;
        private readonly IUserRepository _userRepository;
        private UserESRepository _userESRepository;
        private IndentiferService _indentiferService;
        private IPassengerRepository _passengerRepository;
        private IAirlinesRepository _airlinesRepository;
        private readonly IPaymentRequestRepository _paymentRequestRepository;
        private readonly IHotelBookingCodeRepository _hotelBookingCodeRepository;
        private readonly ISupplierRepository _supplierRepository;
        private ManagementUser _ManagementUser;
        private APIService apiService;
        private readonly IWebHostEnvironment _WebHostEnvironment;
        private readonly IContractPayRepository _contractPayRepository;
        public SetServiceController(IConfiguration configuration, IOrderRepositor orderRepository, IAllCodeRepository allcodeRepository, IContactClientRepository contactClientRepository, IWebHostEnvironment WebHostEnvironment,
            IFlyBookingDetailRepository flyBookingDetailRepository, IUserRepository userRepository, IPassengerRepository passengerRepository, IAirlinesRepository airlinesRepository,
            IPaymentRequestRepository paymentRequestRepository, IHotelBookingCodeRepository hotelBookingCodeRepository, ISupplierRepository supplierRepository, IOrderRepository orderRepository2,
            ManagementUser managementUser, IContractPayRepository contractPayRepository)
        {

            _configuration = configuration;
            _orderRepository = orderRepository;
            _flyBookingDetailRepository = flyBookingDetailRepository;
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
            _hotelBookingCodeRepository = hotelBookingCodeRepository;
            _supplierRepository = supplierRepository;
            _orderRepository2 = orderRepository2;
            _ManagementUser = managementUser;
            apiService = new APIService(configuration, userRepository);
            _WebHostEnvironment = WebHostEnvironment;
            _contractPayRepository = contractPayRepository;
        }
        public IActionResult Fly()
        {
            ViewBag.status = _allCodeRepository.GetListByType(AllCodeType.BOOKING_HOTEL_ROOM_STATUS).Where(x => x.CodeValue != (int)ServiceStatus.New).ToList();
            return View();
        }

        public async Task<IActionResult> FlySearch(SearchFlyBookingViewModel searchModel)
        {
            var model = new GenericViewModel<FlyBookingSearchViewModel>();

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
                if (searchModel.EndDateFrom != null && (searchModel.EndDateTo == null || searchModel.EndDateTo < searchModel.EndDateFrom)) searchModel.EndDateTo = searchModel.EndDateFrom;
                if (searchModel.StartDateTo < searchModel.EndDateFrom && searchModel.StartDateTo < searchModel.EndDateTo)
                {
                    searchModel.StartDateTo = null;
                    searchModel.EndDateFrom = null;
                }
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
                                case (int)RoleType.TPVe:
                                case (int)RoleType.DHPQ:
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
                                case (int)RoleType.TPDHVe:
                                    {
                                        searchModel.SalerPermission = null;
                                    }
                                    break;
                            }
                        }

                        model = await _flyBookingDetailRepository.GetPagingList(searchModel, searchModel.PageIndex, searchModel.pageSize);
                    }
                }

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("FlySearch - SetServiceFlyBookingController: " + ex);
            }

            return PartialView(model);
        }

        public async Task<IActionResult> FlyDetail(string group_booking_id)
        {
            SetServiceFlyBookingDetailViewModel model = new SetServiceFlyBookingDetailViewModel();
            ViewBag.GoServiceId = 0;
            ViewBag.BackServiceId = 0;
            ViewBag.ClientId = 0;
            ViewBag.RefundAmount = 0;
            try
            {
                var amount = _contractPayRepository.GetTotalAmountContractPayByServiceId(group_booking_id.Split("_")[0], (int)ServicesType.FlyingTicket, (int)CONTRACT_PAY_TYPE.THU_TIEN_NCC_HOAN_TRA);
                ViewBag.amountNcc = amount;
                var search_group = group_booking_id.Replace("_", ",");
                var fly_detail = await _flyBookingDetailRepository.GetListByGroupFlyID(search_group);
                if (fly_detail != null && fly_detail.Count > 0)
                {
                    ViewBag.FlyCount = fly_detail.Count;

                    if (fly_detail[0].SupplierId != null)
                    {
                        var suplier = _supplierRepository.GetById((int)fly_detail[0].SupplierId);
                        if (suplier != null && suplier.SupplierId > 0)
                        {
                            model.SuplierId = suplier.SupplierId;
                            model.SuplierName = suplier.FullName;
                        }
                    }

                    var order = _orderRepository.GetByOrderId(fly_detail[0].OrderId);
                    var go_detail = fly_detail.FirstOrDefault(x => x.Leg == 0);
                    var back_detail = fly_detail.FirstOrDefault(x => x.Leg == 1);
                    ViewBag.GoServiceId = go_detail.Id;
                    if (fly_detail.Count > 1)
                    {
                        ViewBag.BackServiceId = back_detail.Id;
                    }
                    model.CreatedDate = (DateTime)order.CreateTime;
                    model.DepartmentName = "";
                    model.EndDate = back_detail != null ? (DateTime)back_detail.EndDate : (DateTime)go_detail.EndDate;
                    model.StartDate = (DateTime)go_detail.StartDate;
                    model.ServiceCode = go_detail.ServiceCode;
                    model.FlyBookingStatus = (int)go_detail.Status;
                    model.FlyBookingStatusName = _allCodeRepository.GetListByType(AllCodeType.BOOKING_HOTEL_ROOM_STATUS).First(x => x.CodeValue == (int)go_detail.Status).Description;
                    model.OrderId = fly_detail[0].OrderId;
                    model.Refund = (double)(order.Refund == null ? 0 : order.Refund);
                    model.GroupBookingId = search_group;
                    User user = null;
                    ViewBag.ClientId = order.ClientId ?? 0;
                    if (order.SalerId != null && (long)order.SalerId > 0)
                    {
                        var saler = await _userRepository.GetById((long)order.SalerId);
                        if (saler != null && saler.Id > 0)
                        {
                            model.SalerName = saler.UserName;

                        }
                        else
                        {
                            model.SalerName = "";
                        }
                    }
                    if (go_detail.SalerId != null && (long)go_detail.SalerId > 0)
                    {
                        user = null;
                        user = await _userRepository.GetById((long)go_detail.SalerId);
                        if (user != null && user.Id > 0)
                        {
                            model.OperatorName = user.FullName;

                        }
                        else
                        {
                            model.OperatorName = "";
                        }
                    }
                    model.BaseTotalAmount = fly_detail.Sum(x => x.Amount != null ? (double)x.Amount : 0);
                    model.BaseSalerTotalAmount = fly_detail.Sum(x => x.TotalNetPrice != null ? (double)x.TotalNetPrice : 0);
                    var operator_order_amount = fly_detail.Sum(x => x.Price != null ? (double)x.Price : 0);
                    if (operator_order_amount > 0)
                    {
                        model.OperatorOrderTotalAmount = operator_order_amount;
                        model.OperatorOrderProfit = fly_detail.Sum(x => (double)x.Amount /* - (x.Adgcommission == null ? 0 : (double)x.Adgcommission) - (x.OthersAmount == null ? 0 : (double)x.OthersAmount)*/) - operator_order_amount;
                    }
                    else
                    {
                        model.OperatorOrderTotalAmount = fly_detail.Sum(x => x.Amount - (x.Profit != null && x.Profit > 0 ? (double)x.Profit : 0)); //- (x.Adgcommission == null ? 0 : (double)x.Adgcommission) - (x.OthersAmount == null ? 0 : (double)x.OthersAmount));
                        model.OperatorOrderProfit = fly_detail.Sum(x => (x.Profit != null && x.Profit > 0 ? (double)x.Profit : 0));
                    }
                    user = null;
                    user = await _userRepository.GetById((long)order.CreatedBy);
                    if (user != null && user.Id > 0)
                    {
                        model.UserCreate = user.UserName;

                    }
                    model.CreatedDate = (DateTime)order.CreateTime;
                    model.UserUpdate = null;
                    model.UpdatedDate = DateTime.MinValue;
                    user = null;
                    user = await _userRepository.GetById(Convert.ToInt64(go_detail.UpdatedBy));
                    if (user != null && user.Id > 0)
                    {
                        model.UserUpdate = user.UserName;
                        model.UpdatedDate = (DateTime)go_detail.UpdatedDate;
                    }
                    model.TotalOrderAmount = (double)order.Amount;
                    model.TotalSalerProfit = fly_detail.Sum(x => (double)x.Profit);

                    var max_date = fly_detail.OrderByDescending(x => x.EndDate).First().EndDate;
                    if (max_date < DateTime.Now)
                    {
                        ViewBag.AllowToFinishPayment = true;

                    }
                    else
                    {
                        ViewBag.AllowToFinishPayment = false;

                    }
                    //var data_refund = _paymentRequestRepository.GetRequestByClientId((long)order.ClientId, order.OrderId);
                    //if(data_refund!=null && data_refund.Count > 0)
                    //{
                    //    ViewBag.RefundAmount = data_refund.Where(n => (n.Status == (int)(PAYMENT_REQUEST_STATUS.DA_CHI)) && !string.IsNullOrEmpty(n.PaymentVoucherCode)).Sum(n => n.Amount);

                    //}
                    return PartialView(model);

                }
                else
                {
                    return RedirectToAction("Error", "Home");
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("FlyDetail - SetServiceFlyBookingController: " + ex);
                return RedirectToAction("Error", "Home");
            }

        }
        public async Task<IActionResult> FlyDetailChangeOperator(string operator_name, string group_booking_id)
        {

            try
            {
                ViewBag.Name = operator_name;
                ViewBag.GroupBookingId = group_booking_id;

            }
            catch
            {

            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SummitFlyDetailChangeOperator(string group_booking_id, int user_id)
        {

            try
            {
                if (group_booking_id == null || group_booking_id.Trim() == "" || group_booking_id.Split(",") == null || user_id <= 0)
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
                var success = await _flyBookingDetailRepository.UpdateServiceOperator(group_booking_id, user_id, _UserId);
                var detail = await _flyBookingDetailRepository.GetListByGroupFlyID(group_booking_id);
                if (detail != null && detail.Count > 0)
                {
                    var id = _orderRepository2.UpdateOrderOperator(detail[0].OrderId);
                }
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
        public async Task<IActionResult> FlyDetailBookingCode(long order_id, string group_fly)
        {

            try
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
                ViewBag.package_adt = 0;
                ViewBag.package_chd = 0;
                ViewBag.package_inf = 0;
                ViewBag.OperatorName = "";
                if (order_id > 0)
                {
                    var fly_detail = await _flyBookingDetailRepository.GetListByGroupFlyID(order_id, group_fly);
                    if (fly_detail != null && fly_detail.Count > 0)
                    {
                        int adt_number = (fly_detail[0].AdultNumber != null ? (int)fly_detail[0].AdultNumber : 1);
                        int chd_number = (fly_detail[0].ChildNumber != null ? (int)fly_detail[0].ChildNumber : 1);
                        int inf_number = (fly_detail[0].InfantNumber != null ? (int)fly_detail[0].InfantNumber : 0);
                        double total_profit = fly_detail.Sum(x => x.Profit != null ? (double)x.Profit : 0);
                        var fly_count = fly_detail.Count;
                        double profit_per_one = (double)total_profit / (double)fly_count / (double)(adt_number + chd_number);
                        ViewBag.FlyDetailCount = fly_count;
                        var go = fly_detail.Where(x => x.Leg == 0).FirstOrDefault();
                        ViewBag.Go = go;
                        var go_airline = await _airlinesRepository.GetAirLineByCode(fly_detail.Where(x => x.Leg == 0).First().Airline);
                        if (go_airline != null) ViewBag.GoAirLineName = go_airline.NameVi;
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

                        if ((long)go.SalerId > 0)
                        {
                            var user = await _userRepository.GetById((long)go.SalerId);
                            ViewBag.OperatorName = user != null && user.Id > 0 ? user.FullName : "";
                        }
                    }
                    var guests = await _passengerRepository.GetByOrderID(order_id, group_fly);
                    if (guests != null) ViewBag.passengers = guests;
                    var order = _orderRepository.GetByOrderId(order_id);
                    if (order.ContactClientId != null && order.ContactClientId > 0)
                    {
                        ViewBag.ContactClient = _contactClientRepository.GetByContactClientId((long)order.ContactClientId);

                    }
                }

                ViewBag.extra_list = extra_list;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("FlyDetailBookingCode - SetServiceFlyBookingController: " + ex.ToString());
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> FlyDetailBookingOrdered(long order_id, string group_fly)
        {

            try
            {
                List<FlyBookingExtraPackages> extra_list = new List<FlyBookingExtraPackages>();
                ViewBag.Go = null;
                ViewBag.Back = null;
                ViewBag.ListPackagesOptional = new List<FlyBookingPackagesOptionalViewModel>();
                ViewBag.FlyDetailCount = 1;
                ViewBag.OperatorName = "";

                if (order_id > 0)
                {


                    var fly_detail = await _flyBookingDetailRepository.GetListByGroupFlyID(order_id, group_fly);
                    if (fly_detail != null && fly_detail.Count > 0)
                    {
                        ViewBag.FlyDetailCount = fly_detail.Count;

                        var go = fly_detail.Where(x => x.Leg == 0).FirstOrDefault();
                        ViewBag.Go = go;
                        var go_airline = await _airlinesRepository.GetAirLineByCode(fly_detail.Where(x => x.Leg == 0).First().Airline);
                        if (go_airline != null) ViewBag.GoAirLineName = go_airline.NameVi;
                        if (fly_detail.Count > 1)
                        {
                            var back = fly_detail.Where(x => x.Leg == 1).FirstOrDefault();
                            ViewBag.Back = back;
                            var back_airline = await _airlinesRepository.GetAirLineByCode(fly_detail.Where(x => x.Leg == 1).First().Airline);
                            if (back_airline != null) ViewBag.BackAirLineName = back_airline.NameVi;
                        }
                        var package_optional = await _flyBookingDetailRepository.GetBookingPackagesOptionalsByBookingId(go.Id);
                        var package_optional_list = new List<FlyBookingPackagesOptionalViewModel>();
                        if (package_optional != null && package_optional.Count > 0)
                        {
                            foreach (var item in package_optional)
                            {
                                var item_convert = JsonConvert.DeserializeObject<FlyBookingPackagesOptionalViewModel>(JsonConvert.SerializeObject(item));
                                item_convert.supplier = _supplierRepository.GetSuplierById((int)item_convert.SuplierId);
                                package_optional_list.Add(item_convert);
                            }
                        }
                        ViewBag.ListPackagesOptional = package_optional_list;

                        if (go != null && go.SalerId != null && (long)go.SalerId > 0)
                        {
                            var user = await _userRepository.GetById((long)go.SalerId);
                            ViewBag.OperatorName = user != null && user.Id > 0 ? user.FullName : "";
                        }
                        #region OldVersion (not implemented)
                        /*
                        ViewBag.PriceDetail = await _flyBookingDetailRepository.GetActiveFlyBookingPriceDetailByOrder(order_id);
                        int adt_number = (fly_detail[0].AdultNumber != null ? (int)fly_detail[0].AdultNumber : 1);
                        int chd_number = (fly_detail[0].ChildNumber != null ? (int)fly_detail[0].ChildNumber : 1);
                        int inf_number = (fly_detail[0].InfantNumber != null ? (int)fly_detail[0].InfantNumber : 0);
                        double total_profit = fly_detail.Sum(x => x.Profit != null ? (double)x.Profit : 0);
                        var fly_count = fly_detail.Count;
                        double profit_per_one = (double)total_profit / (double)fly_count / (double)(adt_number + chd_number);
                        var go = fly_detail.Where(x => x.Leg == 0).FirstOrDefault();
                        ViewBag.Go = go;
                        double base_adt = (double)go.FareAdt + (double)go.TaxAdt + (double)go.FeeAdt + (double)go.ServiceFeeAdt;
                        double base_chd = (double)go.FareChd + (double)go.TaxChd + (double)go.FeeChd + (double)go.ServiceFeeChd;
                        double base_inf = (double)go.FareInf + (double)go.TaxInf + (double)go.FeeInf + (double)go.ServiceFeeInf;

                        double package_adt = (double)(go.PriceAdt == null ? base_adt : (go.PriceAdt / (double)adt_number));
                        double package_chd = (double)(go.PriceChd == null ? base_chd : (go.PriceChd / (double)chd_number));
                        double package_inf = (double)(go.PriceInf == null ? base_inf : (go.PriceInf / (double)inf_number));
                        var go_airline = await _airlinesRepository.GetAirLineByCode(fly_detail.Where(x => x.Leg == 0).First().Airline);
                        if (go_airline != null) ViewBag.GoAirLineName = go_airline.NameVi;
                        double TotalProfitAdt = go.ProfitAdt != null ? (double)go.ProfitAdt : 0;
                        double TotalProfitChd = go.ProfitChd != null ? (double)go.ProfitChd : 0;
                        double TotalProfitInf = go.ProfitInf != null ? (double)go.ProfitInf : 0;
                        double TotalAmountAdt = go.AmountAdt != null ? (double)go.AmountAdt : 0;
                        double TotalAmountChd = go.AmountChd != null ? (double)go.AmountChd : 0;
                        double TotalAmountInf = go.AmountInf != null ? (double)go.AmountInf : 0;
                        if (fly_detail.Count > 1)
                        {
                            var back = fly_detail.Where(x => x.Leg == 1).FirstOrDefault();
                            ViewBag.Back = back;
                            double base_adt_back = (double)back.FareAdt + (double)back.TaxAdt + (double)back.FeeAdt + (double)back.ServiceFeeAdt;
                            double base_chd_back = (double)back.FareChd + (double)back.TaxChd + (double)back.FeeChd + (double)back.ServiceFeeChd;
                            double base_inf_back = (double)back.FareInf + (double)back.TaxInf + (double)back.FeeInf + (double)back.ServiceFeeInf;

                            package_adt += (double)(back.PriceAdt == null ? base_adt_back : (back.PriceAdt / (double)adt_number));
                            package_chd += (double)(back.PriceChd == null ? base_chd_back : (back.PriceChd / (double)chd_number));
                            package_inf += (double)(back.PriceInf == null ? base_inf_back : (back.PriceInf / (double)inf_number));
                            var back_airline = await _airlinesRepository.GetAirLineByCode(fly_detail.Where(x => x.Leg == 1).First().Airline);
                            if (back_airline != null) ViewBag.BackAirLineName = back_airline.NameVi;
                            TotalProfitAdt += back.ProfitAdt != null ? (double)back.ProfitAdt : 0;
                            TotalProfitChd += back.ProfitChd != null ? (double)back.ProfitChd : 0;
                            TotalProfitInf += back.ProfitInf != null ? (double)back.ProfitInf : 0;
                            TotalAmountAdt += back.AmountAdt != null ? (double)back.AmountAdt : 0;
                            TotalAmountChd += back.AmountChd != null ? (double)back.AmountChd : 0;
                            TotalAmountInf += back.AmountInf != null ? (double)back.AmountInf : 0;
                        }
                        ViewBag.package_adt = package_adt;
                        ViewBag.package_chd = package_chd;
                        ViewBag.package_inf = package_inf;
                        ViewBag.TotalAmountAdt = package_adt * adt_number;
                        ViewBag.TotalAmountChd = package_chd * chd_number;
                        ViewBag.TotalAmountInf = package_inf * inf_number;

                        ViewBag.TotalProfitAdt = TotalProfitAdt > 0 ? TotalProfitAdt : profit_per_one * adt_number * fly_count;
                        ViewBag.TotalProfitChd = TotalProfitChd > 0 ? TotalProfitChd : profit_per_one * chd_number * fly_count; ;
                        ViewBag.TotalProfitInf = TotalProfitInf;
                        extra_list = await _flyBookingDetailRepository.GetExtraPackageByFlyBookingId(group_fly);
                       */
                        #endregion
                    }


                    var guests = await _passengerRepository.GetByOrderID(order_id, group_fly);
                    if (guests != null) ViewBag.passengers = guests;
                    var order = _orderRepository.GetByOrderId(order_id);
                    if (order.ContactClientId != null && order.ContactClientId > 0)
                    {
                        ViewBag.ContactClient = _contactClientRepository.GetByContactClientId((long)order.ContactClientId);

                    }
                }

                ViewBag.extra_list = extra_list;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("FlyDetailBookingOrdered - SetServiceFlyBookingController: " + ex.ToString());
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> OrderSuggestion(string txt_search)
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
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        data = new List<OrderSelectViewModel>()
                    });
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("OrderNoSuggestion - SetServiceFlyBookingController: " + ex.ToString());
                return Ok(new
                {
                    status = (int)ResponseType.ERROR,
                    data = new List<OrderSelectViewModel>()
                });
            }

        }
        [HttpPost]
        public async Task<IActionResult> FlyBookingSuggestion(string txt_search)
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
                    var data = await _flyBookingESRepository.GetFlyBookingSuggesstion(txt_search);
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
                        data = new List<FlyBookingESViewModel>()
                    });
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("FlyBookingSuggestion - SetServiceFlyBookingController: " + ex.ToString());
                return Ok(new
                {
                    status = (int)ResponseType.ERROR,
                    data = new List<FlyBookingESViewModel>()
                });
            }

        }
        [HttpPost]
        public async Task<IActionResult> UpdateServiceCode(string group_booking_id)
        {

            try
            {
                long x = Convert.ToInt64(group_booking_id.Split(",")[0]);
                var new_servicecode = "FLIGHT" + string.Format(String.Format("{0,4:0000}", x));
                await _flyBookingDetailRepository.UpdateServiceCode(group_booking_id, new_servicecode);
                return Ok(new
                {
                    status = (int)ResponseType.SUCCESS,
                    msg = "Success"
                });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateServiceCode - OrderManualController: " + ex.ToString());
                return Ok(new
                {
                    status = (int)ResponseType.ERROR,
                    msg = "Failed"
                });
            }

        }

        [HttpPost]
        public async Task<IActionResult> UpdateFlyOperatorOrderPrice(List<FlyBookingPackagesOptional> data)
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
                var fly = await _flyBookingDetailRepository.GetFlyBookingById(data[0].BookingId);
                double amount = 0;
                double price = 0;
                double profit = 0;
                if (fly != null && fly.Id > 0)
                {
                    int _UserId = 0;
                    if (HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) != null)
                    {
                        _UserId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                    }
                    var id = await _flyBookingDetailRepository.UpdateFlyBookingOptional(data, fly.GroupBookingId, _UserId);
                    var fly_route = await _flyBookingDetailRepository.GetListByGroupFlyID(fly.GroupBookingId);
                    amount = fly_route.Sum(x => x.Amount - (x.Adgcommission == null ? 0 : (double)x.Adgcommission) - (x.OthersAmount == null ? 0 : (double)x.OthersAmount));
                    price = data.Sum(x => x.Amount);
                    profit = amount - price;

                    #region Update Order Amount:
                    await _orderRepository2.UpdateOrderDetail(fly.OrderId, _UserId);
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
                LogHelper.InsertLogTelegram("UpdateOperatorOrderPrice - OrderManualController: " + ex.ToString());

            }
            return Ok(new
            {
                status = (int)ResponseType.FAILED,
                msg = "Cập nhật giá đặt dịch vụ thất bại, vui lòng liên hệ IT"
            });

        }
        [HttpPost]
        public async Task<IActionResult> DeclineFlyBooking(string group_booking_id)
        {
            try
            {
                if (group_booking_id == null || group_booking_id.Trim() == "")
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
                var id = await _flyBookingDetailRepository.UpdateServiceStatus((int)ServiceStatus.Decline, group_booking_id, _UserId);
                if (id > 0)
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        msg = "Đôi trạng thái từ chối dịch vụ thành công"
                    });
                }

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("DeclineFlyBooking - OrderManualController: " + ex.ToString());

            }
            return Ok(new
            {
                status = (int)ResponseType.ERROR,
                msg = "Cập nhật trạng thái đặt dịch vụ thất bại, vui lòng liên hệ IT"
            });
        }
        [HttpPost]
        public async Task<IActionResult> GrantOrderPermission(string group_booking_id)
        {
            try
            {
                var current_user = _ManagementUser.GetCurrentUser();
                if (group_booking_id == null || group_booking_id.Trim() == "")
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
                var id = await _flyBookingDetailRepository.UpdateServiceStatus((int)ServiceStatus.ServeCode, group_booking_id, _UserId);
                var success = await _flyBookingDetailRepository.UpdateServiceOperator(group_booking_id, _UserId, _UserId);
                if (id > 0)
                {

                    var detail = await _flyBookingDetailRepository.GetListByGroupFlyID(group_booking_id);
                    if (detail != null && detail.Count > 0)
                    {
                        id = _orderRepository2.UpdateOrderOperator(detail[0].OrderId);
                        var order = _orderRepository.GetByOrderId(detail[0].OrderId);
                        string link = "/Order/" + detail[0].OrderId;
                        apiService.SendMessage(_UserId.ToString(), ((int)ModuleType.DICH_VU).ToString(), ((int)ActionType.NHAN_TRIEN_KHAI).ToString(), order.OrderNo, link, current_user.Role, detail[0].ServiceCode);
                    }

                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        msg = "Nhận đặt dịch vụ thành công"
                    });
                }

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GrantOrderPermission - OrderManualController: " + ex.ToString());

            }
            return Ok(new
            {
                status = (int)ResponseType.ERROR,
                msg = "Nhận đặt dịch vụ thất bại, vui lòng liên hệ IT"
            });
        }
        [HttpPost]
        public async Task<IActionResult> SendServiceCode(string group_booking_id)
        {
            try
            {
                var current_user = _ManagementUser.GetCurrentUser();
                if (group_booking_id == null || group_booking_id.Trim() == "")
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
                var hotelBookingCode = await _hotelBookingCodeRepository.GetListlBookingCodeByHotelBookingId(Convert.ToInt64(group_booking_id.Split(",")[0]), (int)ServicesType.FlyingTicket);
                if (hotelBookingCode != null && hotelBookingCode.Count > 0)
                {
                    var id = await _flyBookingDetailRepository.UpdateServiceStatus((int)ServiceStatus.ServeCode, group_booking_id, _UserId);
                    if (id > 0)
                    {
                        var detail = await _flyBookingDetailRepository.GetListByGroupFlyID(group_booking_id);
                        var order = _orderRepository.GetByOrderId(detail[0].OrderId);
                        string link = "/Order/" + detail[0].OrderId;
                        apiService.SendMessage(_UserId.ToString(), ((int)ModuleType.DICH_VU).ToString(), ((int)ActionType.TRA_CODE).ToString(), order.OrderNo, link, current_user.Role, detail[0].ServiceCode);
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
                LogHelper.InsertLogTelegram("SendServiceCode - OrderManualController: " + ex.ToString());

            }
            return Ok(new
            {
                status = (int)ResponseType.ERROR,
                msg = "Cập nhật trạng thái đặt dịch vụ thất bại, vui lòng liên hệ IT"
            });
        }
        [HttpPost]
        public async Task<IActionResult> ChangeToConfirmPaymentStatus(string group_booking_id, List<long> group_list)
        {
            try
            {
                var current_user = _ManagementUser.GetCurrentUser();
                if (group_booking_id == null || group_booking_id.Trim() == "" || group_list == null || group_list.Count <= 0)
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
                var payment_request = _paymentRequestRepository.GetByServiceId(group_list[0], (int)ServicesType.FlyingTicket);
                if (payment_request == null || payment_request.Count <= 0)
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.FAILED,
                        msg = "Dịch vụ chưa có yêu cầu chi nào, vui lòng bổ sung"
                    });
                }
                var fly_detail = await _flyBookingDetailRepository.GetListByGroupFlyID(group_booking_id);
                var amount = Convert.ToDouble(fly_detail.Sum(x => x.Price));
                var request_amount = Convert.ToDouble(payment_request.Sum(x => (((x.Status == (int)(PAYMENT_REQUEST_STATUS.DA_CHI) || x.Status == (int)(PAYMENT_REQUEST_STATUS.CHO_CHI) || x.IsSupplierDebt == true))) ? x.Amount : 0));
                if (request_amount >= amount)
                {
                    var id = await _flyBookingDetailRepository.UpdateServiceStatus((int)ServiceStatus.Payment, group_booking_id, _UserId);
                    if (id > 0)
                    {
                        //var dataOrder2 = await _orderRepository2.ProductServiceName(fly_detail[0].OrderId.ToString());
                        //List<int> confirm_payment_status_allow = new List<int>() { (int)ServiceStatus.Payment, (int)ServiceStatus.Cancel };
                        //var is_other_than_payment = dataOrder2.Any(x => !confirm_payment_status_allow.Contains(x.Status));
                        //var is_has_payment = dataOrder2.Any(x => x.Status == (int)ServiceStatus.Payment);
                        //if (!is_other_than_payment && is_has_payment)
                        //{
                        //    long UpdatedBy = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                        //    long UserVerify = 0;
                        //    var UpdateOrderStatus = await _orderRepository2.UpdateOrderStatus(fly_detail[0].OrderId, (int)OrderStatus.WAITING_FOR_ACCOUNTANT, UpdatedBy, UserVerify);
                        //}
                        await _orderRepository2.UpdateOrderDetail(fly_detail[0].OrderId, _UserId);

                        var detail = await _flyBookingDetailRepository.GetListByGroupFlyID(group_booking_id);
                        var order = _orderRepository.GetByOrderId(detail[0].OrderId);
                        string link = "/Order/" + fly_detail[0].OrderId;
                        apiService.SendMessage(_UserId.ToString(), ((int)ModuleType.DICH_VU).ToString(), ((int)ActionType.QUYET_TOAN).ToString(), order.OrderNo, link, current_user.Role, detail[0].ServiceCode);
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
                LogHelper.InsertLogTelegram("ChangeToConfirmPaymentStatus - OrderManualController: " + ex.ToString());

            }
            return Ok(new
            {
                status = (int)ResponseType.ERROR,
                msg = "Cập nhật trạng thái đặt dịch vụ thất bại, vui lòng liên hệ IT"
            });
        }
        [HttpPost]
        public async Task<IActionResult> FlyExportExcel(SearchFlyBookingViewModel searchModel)
        {
            try
            {
                var current_user = _ManagementUser.GetCurrentUser();
                string _FileName = "Danh sách đặt dịch vụ vé máy bay(" + current_user.Id + ").xlsx";
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

                var rsPath = await _flyBookingDetailRepository.ExportDeposit(searchModel, FilePath);

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

    }
}
