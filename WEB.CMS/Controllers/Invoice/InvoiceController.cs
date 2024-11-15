using Entities.ViewModels;
using Entities.ViewModels.Funding;
using Entities.ViewModels.Invoice;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;
using WEB.CMS.Customize;
using WEB.CMS.Models;
namespace GDHT.CMS.Controllers.Invoice
{
    [CustomAuthorize]

    public class InvoiceController : Controller
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IAllCodeRepository _allCodeRepository;
        private readonly IInvoiceRequestRepository _invoiceRequestRepository;
        private readonly IWebHostEnvironment _WebHostEnvironment;
        private readonly IUserRepository _userRepository;
        private ManagementUser _ManagementUser;

        public InvoiceController(IInvoiceRepository invoiceRepository, IWebHostEnvironment hostEnvironment, IUserRepository userRepository,
            ManagementUser ManagementUser, IInvoiceRequestRepository invoiceRequestRepository, IAllCodeRepository allCodeRepository)
        {
            _invoiceRequestRepository = invoiceRequestRepository;
            _invoiceRepository = invoiceRepository;
            _WebHostEnvironment = hostEnvironment;
            _ManagementUser = ManagementUser;
            _allCodeRepository = allCodeRepository;
            _userRepository = userRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Search(InvoiceSearchModel searchModel, int currentPage = 1, int pageSize = 20)
        {
            var model = new GenericViewModel<InvoiceViewModel>();
            try
            {
                if (searchModel.CreateByIds == null) searchModel.CreateByIds = new List<int>();
                if (!string.IsNullOrEmpty(searchModel.InvoiceCode))
                    searchModel.InvoiceCode = searchModel.InvoiceCode.Trim();
                if (!string.IsNullOrEmpty(searchModel.InvoiceNo))
                    searchModel.InvoiceNo = searchModel.InvoiceNo.Trim();
                if (!string.IsNullOrEmpty(searchModel.OrderNo))
                    searchModel.OrderNo = searchModel.OrderNo.Trim();
                if (!string.IsNullOrEmpty(searchModel.InvoiceRequestNo))
                    searchModel.InvoiceRequestNo = searchModel.InvoiceRequestNo.Trim();
                var current_user = _ManagementUser.GetCurrentUser();
                if (!string.IsNullOrEmpty(current_user.UserUnderList) && (searchModel.CreateByIds == null || searchModel.CreateByIds.Count == 0))
                    searchModel.CreateByIds = current_user.UserUnderList.Split(',').Select(n => int.Parse(n)).ToList();
                var listInvoice = _invoiceRepository.GetInvoices(searchModel, out long total, currentPage, pageSize);
                model.CurrentPage = currentPage;
                model.ListData = listInvoice;
                model.PageSize = pageSize;
                model.TotalRecord = total;
                model.TotalPage = (int)Math.Ceiling((double)total / pageSize);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Search - InvoiceController: " + ex);
            }
            return PartialView(model);
        }

        public async Task<IActionResult> Detail(int invoiceId)
        {
            var detail = _invoiceRepository.GetById(invoiceId);
            //var current_user = _ManagementUser.GetCurrentUser();
            return View(detail);
        }

        [HttpPost]
        public IActionResult GetDetail(int invoiceId)
        {
            try
            {
                var detail = _invoiceRepository.GetById(invoiceId);
                return Ok(new
                {
                    isSuccess = true,
                    data = detail
                });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetDetail - InvoiceController: " + ex);
                return Ok(new
                {
                    isSuccess = false,
                    data = new InvoiceRequestViewModel()
                });
            }
        }

        public IActionResult Add()
        {
            ViewBag.allCode_PAY_TYPE = _allCodeRepository.GetListByType(AllCodeType.PAY_TYPE);
            ViewBag.listBankingAccount = _allCodeRepository.GetBankingAccounts();
            return PartialView();
        }

        [HttpPost]
        public async Task<IActionResult> AddJson(InvoiceViewModel model)
        {
            try
            {
                var userLogin = 0;
                if (HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) != null)
                {
                    userLogin = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                model.CreatedBy = userLogin;
                var client = new HttpClient();
                var apiPrefix = ReadFile.LoadConfig().API_URL + ReadFile.LoadConfig().API_GET_INVOICE_REQUEST_NO;
                var key_token_api = ReadFile.LoadConfig().KEY_TOKEN_API_MANUAL;
                HttpClient httpClient = new HttpClient();
                JObject jsonObject = new JObject(
                   new JProperty("code_type", ((int)GET_CODE_MODULE.HOA_DON).ToString())
                );
                var j_param = new Dictionary<string, object>
                 {
                     { "key",jsonObject}
                 };
                var data_product = JsonConvert.SerializeObject(j_param);
                var token = CommonHelper.Encode(data_product, key_token_api);
                var content = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("token", token) });
                var response = await httpClient.PostAsync(apiPrefix, content);
                var resultAPI = await response.Content.ReadAsStringAsync();
                var output = JsonConvert.DeserializeObject<OutputAPI>(resultAPI);
                model.InvoiceCode = output.code;
                model.ExportDate = DateUtil.StringToDateTime(model.ExportDateStr, "dd/MM/yyyy");
                var validate = Validate(model, out string messages);
                if (validate < 0)
                {
                    return Ok(new
                    {
                        isSuccess = false,
                        message = messages
                    });
                }
                var result = _invoiceRepository.CreateInvoice(model);
                if (result == -2)
                    return Ok(new
                    {
                        isSuccess = false,
                        message = "Mã hóa đơn " + model.InvoiceCode + " đã tồn tại. Vui lòng kiểm tra lại"
                    });

                if (result < 1)
                    return Ok(new
                    {
                        isSuccess = false,
                        message = "Thêm hóa đơn thất bại"
                    });
                return Ok(new
                {
                    isSuccess = true,
                    message = "Thêm hóa đơn thành công",
                    id = result
                });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("AddJson - InvoiceController: " + ex);
                return Ok(new
                {
                    isSuccess = false,
                    message = "Thêm hóa đơn thất bại. Đã có lỗi xảy ra"
                });
            }
        }

        public IActionResult Edit(int invoiceId)
        {
            ViewBag.allCode_PAY_TYPE = _allCodeRepository.GetListByType(AllCodeType.PAY_TYPE);
            ViewBag.listBankingAccount = _allCodeRepository.GetBankingAccounts();
            var detail = _invoiceRepository.GetById(invoiceId);
            return PartialView(detail);
        }

        [HttpPost]
        public async Task<IActionResult> Update(InvoiceViewModel model)
        {
            try
            {
                var userLogin = 0;
                if (HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) != null)
                {
                    userLogin = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                model.UpdatedBy = userLogin;
                model.ExportDate = DateUtil.StringToDateTime(model.ExportDateStr, "dd/MM/yyyy");
                var validate = Validate(model, out string messages, true);
                if (validate < 0)
                {
                    return Ok(new
                    {
                        isSuccess = false,
                        message = messages
                    });
                }
                var result = _invoiceRepository.UpdateInvoice(model);
                if (result < 1)
                    return Ok(new
                    {
                        isSuccess = false,
                        message = "Cập nhật hóa đơn thất bại"
                    });
                return Ok(new
                {
                    isSuccess = true,
                    message = "Cập nhật hóa đơn thành công"
                });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Update - InvoiceController: " + ex);
                return Ok(new
                {
                    isSuccess = false,
                    message = "Cập nhật hóa đơn thất bại. Đã có lỗi xảy ra"
                });
            }
        }

        private int Validate(InvoiceViewModel model, out string message, bool isEdit = false)
        {
            var result = 1;
            message = string.Empty;
            if (model.ClientId == 0)
            {
                message = "Vui lòng chọn khách hàng";
                return -1;
            }

            if (model.PayType == (int)DepositHistoryConstant.CONTRACT_PAYMENT_TYPE.CHUYEN_KHOAN && model.BankingAccountId == 0)
            {
                message = "Vui lòng chọn tài khoản";
                return -1;
            }

            if (model.ExportDate == null)
            {
                message = "Vui lòng chọn ngày xuất";
                return -1;
            }

            if (!string.IsNullOrEmpty(model.Note) && model.Note.Length > 3000)
            {
                message = "Vui lòng nhập ghi chú không quá 3000 kí tự";
                return -1;
            }

            return result;
        }

        [HttpPost]
        public async Task<IActionResult> GetInvoiceRequestsByClientId(long clientId, int invoiceId = 0)
        {
            try
            {
                var listRequest = _invoiceRequestRepository.GetInvoiceRequestsByClientId(clientId, invoiceId);
                return Ok(new
                {
                    isSuccess = true,
                    message = "Thành công",
                    data = listRequest,
                });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetInvoiceRequestsByClientId - InvoiceController: " + ex);
                return Ok(new
                {
                    isSuccess = false,
                    message = "Thất bại.Đã có lỗi xảy ra",
                    data = new List<InvoiceRequestViewModel>()
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetListFilter(string jsonData, string text = null, string planDate = null, int userCreate = 0)
        {
            try
            {
                if (string.IsNullOrEmpty(jsonData))
                    return Ok(new
                    {
                        isSuccess = true,
                        message = "Thành công",
                        data = new List<InvoiceRequestViewModel>()
                    });
                var listFilter = JsonConvert.DeserializeObject<List<InvoiceRequestViewModel>>(jsonData);
                if (!string.IsNullOrEmpty(text))
                    listFilter = listFilter.Where(n => n.InvoiceRequestNo.Contains(text.Trim())).ToList();
                if (!string.IsNullOrEmpty(planDate))
                    listFilter = listFilter.Where(n => n.PlanDateViewStr == planDate.Trim()).ToList();
                if (userCreate != 0)
                    listFilter = listFilter.Where(n => n.CreatedBy == userCreate).ToList();
                return Ok(new
                {
                    isSuccess = true,
                    message = "Thành công",
                    data = listFilter
                });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetListFilter - InvoiceController: " + ex);
                return Ok(new
                {
                    isSuccess = false,
                    message = "Thất bại",
                    data = new List<Entities.Models.Order>()
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int invoiceId)
        {
            try
            {
                var userLogin = 0;
                if (HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) != null)
                {
                    userLogin = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                var result = _invoiceRepository.DeleteInvoice(invoiceId, userLogin);
                if (result < 1)
                    return Ok(new
                    {
                        isSuccess = true,
                        message = "Xóa hóa đơn thất bại"
                    });

                return Ok(new
                {
                    isSuccess = true,
                    message = "Xóa hóa đơn thành công"
                });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Delete - InvoiceController: " + ex);
                return Ok(new
                {
                    isSuccess = false,
                    message = "Xóa hóa đơn thất bại. Vui lòng liên hệ quản trị viên"
                });
            }
        }

    }
}
