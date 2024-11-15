using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Entities.ViewModels;
using Entities.ViewModels.CustomerManager;
using Entities.ViewModels.Funding;
using Microsoft.AspNetCore.Mvc;
using Repositories.IRepositories;
using Repositories.Repositories;
using Utilities;
using Utilities.Contants;
using WEB.CMS.Customize;

namespace GDHT.CMS.Controllers.FundCustomer
{
    public class FundCustomerCareController : Controller
    {
        private ManagementUser _ManagementUser;
        private readonly IPaymentRequestRepository _paymentRequestRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IUserRepository _userRepository;
        public FundCustomerCareController(ManagementUser ManagementUser, IPaymentRequestRepository paymentRequestRepository, IOrderRepository orderRepository, IUserRepository userRepository, IClientRepository clientRepository)
        {
            _ManagementUser = ManagementUser;
            _paymentRequestRepository = paymentRequestRepository;
            _orderRepository = orderRepository;
            _clientRepository = clientRepository;
            _userRepository = userRepository;
        }
        public async Task<IActionResult> Index()
        {
            var current_user = _ManagementUser.GetCurrentUser();
            var data = await _orderRepository.GetTotalCustomerCareFund(current_user.UserUnderList, 0);
            ViewBag.TotalFundCustomerCare = data.TotalFundCustomerCare > 0 ? (data.TotalFundCustomerCare - data.TotalAmountPaymentRequestComplete) : 0;
            ViewBag.TotalAmountPaymentRequestPending = data.TotalAmountPaymentRequestPending.ToString("N0");
            return View();
        }

        [HttpPost]
        public IActionResult Search(PaymentRequestSearchModel searchModel, int currentPage = 1, int pageSize = 20)
        {
            var model = new GenericViewModel<PaymentRequestViewModel>();
            try
            {
                if (searchModel.CreateByIds == null) searchModel.CreateByIds = new List<int>();
                if (searchModel.PaymentTypeMulti == null) searchModel.PaymentTypeMulti = new List<int>();
                if (searchModel.StatusMulti == null) searchModel.StatusMulti = new List<int>();
                if (searchModel.Status > -1)
                    searchModel.StatusMulti.Add(searchModel.Status);
                if (searchModel.TypeMulti == null) searchModel.TypeMulti = new List<int>();
                if (!string.IsNullOrEmpty(searchModel.PaymentCode))
                    searchModel.PaymentCode = searchModel.PaymentCode.Trim();
                if (!string.IsNullOrEmpty(searchModel.OrderNo))
                    searchModel.OrderNo = searchModel.OrderNo.Trim();
                if (!string.IsNullOrEmpty(searchModel.ServiceCode))
                    searchModel.ServiceCode = searchModel.ServiceCode.Trim();
                if (!string.IsNullOrEmpty(searchModel.Content))
                    searchModel.Content = searchModel.Content.Trim();
                var current_user = _ManagementUser.GetCurrentUser();
                if (searchModel.CreateByIds.Count == 0)
                {
                    if (!string.IsNullOrEmpty(current_user.UserUnderList))
                        searchModel.CreateByIds = current_user.UserUnderList.Split(',').Select(n => int.Parse(n)).ToList();

                }
                var listPaymentRequest = _paymentRequestRepository.GetPaymentRequests(searchModel, out long total, currentPage, pageSize);
                model.CurrentPage = currentPage;
                model.ListData = listPaymentRequest;
                model.PageSize = pageSize;
                model.TotalRecord = total;
                model.TotalPage = (int)Math.Ceiling((double)total / pageSize);


            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Search - FundCustomerCareController: " + ex + ". Đã có lỗi xảy ra");
            }
            return PartialView(model);
        }
        [HttpPost]
        public async Task<IActionResult> GetTotalCustomerCareFundByClientid(long id)
        {

            try
            {
                var _UserId = Convert.ToInt64(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var data = await _orderRepository.GetTotalCustomerCareFund(_UserId.ToString(), id);

                return Ok(new
                {
                    status = (int)ResponseType.SUCCESS,
                    data = data.TotalFundCustomerCare - data.TotalAmountPaymentRequestPending - data.TotalAmountPaymentRequestComplete
                });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetTotalCustomerCareFundByClientid - FundCustomerCareController: " + ex + ". Đã có lỗi xảy ra");
            }
            return Ok(new
            {
                status = (int)ResponseType.ERROR,
                data = 0
            });
        }
        [HttpPost]
        public async Task<IActionResult> ListClient(CustomerManagerViewSearchModel searchModel, int currentPage = 1, int pageSize = 20)
        {
            var model = new GenericViewModel<ClientCustomerCareFundModel>();

            try
            {
                var current_user = _ManagementUser.GetCurrentUser();
                if (current_user != null)
                {
                    var i = 0;
                    if (current_user != null && !string.IsNullOrEmpty(current_user.Role))
                    {
                        var list = Array.ConvertAll(current_user.Role.Split(','), int.Parse);
                        foreach (var item in list)
                        {

                            searchModel.SalerPermission = current_user.UserUnderList;

                            if (item == (int)RoleType.Admin)
                            {
                                searchModel.SalerPermission = null;
                                i++;
                            }
                        }
                    }
                    if (i != 0)
                    {
                        model = await _clientRepository.GetListClientCustomerCareFund(searchModel);
                    }

                }

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ListClient - CustomerManagerController: " + ex);
            }

            return PartialView(model);
        }
        public async Task<IActionResult> FundCustomerCareClient()
        {

            return View();
        }

    }
}

