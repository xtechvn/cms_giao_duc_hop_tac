using Entities.Models;
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
using System.IO;
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

    public class InvoiceRequestController : Controller
    {
        private readonly IAllCodeRepository _allCodeRepository;
        private readonly IOrderRepositor _orderRepository;
        private readonly IInvoiceRequestRepository _invoiceRequestRepository;
        private readonly IWebHostEnvironment _WebHostEnvironment;
        private readonly IUserRepository _userRepository;
        private readonly IClientRepository _clientRepository;
        private ManagementUser _ManagementUser;

        public InvoiceRequestController(IAllCodeRepository allCodeRepository, IInvoiceRequestRepository invoiceRequestRepository,
            IWebHostEnvironment hostEnvironment, ManagementUser ManagementUser, IUserRepository userRepository,
            IClientRepository clientRepository, IOrderRepositor orderRepository)
        {
            _allCodeRepository = allCodeRepository;
            _invoiceRequestRepository = invoiceRequestRepository;
            _WebHostEnvironment = hostEnvironment;
            _ManagementUser = ManagementUser;
            _userRepository = userRepository;
            _clientRepository = clientRepository;
            _orderRepository = orderRepository;
        }

        public IActionResult Index()
        {
            var listStatus = _allCodeRepository.GetListByType(AllCodeType.INVOICE_REQUEST_STATUS);
            ViewBag.allCode_PAYMENT_REQUEST_STATUS = listStatus;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Search(InvoiceRequestSearchModel searchModel, int currentPage = 1, int pageSize = 20)
        {
            var model = new GenericViewModel<InvoiceRequestViewModel>();
            try
            {
                if (searchModel.CreateByIds == null) searchModel.CreateByIds = new List<int>();
                if (searchModel.StatusMulti == null) searchModel.StatusMulti = new List<int>();
                if (searchModel.Status > -1) searchModel.StatusMulti.Add(searchModel.Status);
                if (!string.IsNullOrEmpty(searchModel.InvoiceRequestNo)) searchModel.InvoiceRequestNo = searchModel.InvoiceRequestNo.Trim();
                if (!string.IsNullOrEmpty(searchModel.InvoiceCode)) searchModel.InvoiceCode = searchModel.InvoiceCode.Trim();
                if (!string.IsNullOrEmpty(searchModel.Note)) searchModel.Note = searchModel.Note.Trim();
                var current_user = _ManagementUser.GetCurrentUser();
                if (!string.IsNullOrEmpty(current_user.UserUnderList) && (searchModel.CreateByIds == null || searchModel.CreateByIds.Count == 0))
                    searchModel.CreateByIds = current_user.UserUnderList.Split(',').Select(n => int.Parse(n)).ToList();
                var listInvoiceRequest = _invoiceRequestRepository.GetInvoiceRequests(searchModel, out long total, currentPage, pageSize);
                if (searchModel.Status != -1)
                {
                    listInvoiceRequest = listInvoiceRequest.Where(n => n.Status == searchModel.Status).ToList();
                }
                model.CurrentPage = currentPage;
                model.ListData = listInvoiceRequest;
                model.PageSize = pageSize;
                model.TotalRecord = total;
                model.TotalPage = (int)Math.Ceiling((double)total / pageSize);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Search - InvoiceRequestController: " + ex);
            }
            return PartialView(model);
        }

        [HttpPost]
        public async Task<IActionResult> CountStatus(InvoiceRequestSearchModel searchModel)
        {
            try
            {
                if (searchModel.CreateByIds == null) searchModel.CreateByIds = new List<int>();
                if (searchModel.StatusMulti == null) searchModel.StatusMulti = new List<int>();
                if (searchModel.Status > -1)
                    searchModel.StatusMulti.Add(searchModel.Status);
                if (!string.IsNullOrEmpty(searchModel.InvoiceRequestNo))
                    searchModel.InvoiceRequestNo = searchModel.InvoiceRequestNo.Trim();
                if (!string.IsNullOrEmpty(searchModel.Note))
                    searchModel.Note = searchModel.Note.Trim();
                var current_user = _ManagementUser.GetCurrentUser();
                if (!string.IsNullOrEmpty(current_user.UserUnderList))
                    searchModel.CreateByIds = current_user.UserUnderList.Split(',').Select(n => int.Parse(n)).ToList();
                var countStatus = _invoiceRequestRepository.GetCountStatus(searchModel);
                var countStatusAll = new CountStatus();
                countStatusAll.Status = -1;
                countStatusAll.Total = countStatus.Sum(n => n.Total);
                countStatus.Add(countStatusAll);
                return Ok(new
                {
                    isSuccess = true,
                    data = countStatus
                });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CountStatus - InvoiceRequestController: " + ex);
                return Ok(new
                {
                    isSuccess = false,
                    data = new List<CountStatus>()
                });
            }
        }

        public async Task<IActionResult> Detail(int invoiceRequestId)
        {
            var detail = _invoiceRequestRepository.GetById(invoiceRequestId);
            var current_user = _ManagementUser.GetCurrentUser();
            ViewBag.TBP_DUYET_YEU_CAU_XUAT_HOA_DON = 0;
            ViewBag.KTT_DUYET_YEU_CAU_XUAT_HOA_DON = 0;
            ViewBag.KTV_DUYET_YEU_CAU_XUAT_HOA_DON = 0;
            ViewBag.isAdmin = false;
            if (current_user != null && !string.IsNullOrEmpty(current_user.Role))
            {
                bool isAdmin = _userRepository.IsAdmin(current_user.Id);
                ViewBag.isAdmin = isAdmin;

                bool isHeadOfAccountant = _userRepository.IsHeadOfAccountant(current_user.Id);
                if (isHeadOfAccountant)
                    ViewBag.KTT_DUYET_YEU_CAU_XUAT_HOA_DON = 1;

                bool IsAccountant = _userRepository.IsAccountant(current_user.Id);
                if (IsAccountant)
                    ViewBag.KTV_DUYET_YEU_CAU_XUAT_HOA_DON = 1;

                var listRole = Array.ConvertAll(current_user.Role.Split(','), int.Parse);
                foreach (var item in listRole)
                {
                    //kiểm tra chức năng có đc phép sử dụng
                    var checkRolePermission = await _userRepository.CheckRolePermissionByUserAndRole(current_user.Id, item,
                        (int)SortOrder.DUYET, (int)MenuId.YEU_CAU_XUAT_HOA_DON);
                    if (checkRolePermission)
                    {
                        ViewBag.TBP_DUYET_YEU_CAU_XUAT_HOA_DON = 1;
                        break;
                    }
                }
            }
            return View(detail);
        }

        [HttpPost]
        public IActionResult GetDetail(int invoiceRequestId)
        {
            try
            {
                var detail = _invoiceRequestRepository.GetById(invoiceRequestId);
                return Ok(new
                {
                    isSuccess = true,
                    data = detail
                });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetDetail - InvoiceRequestController: " + ex);
                return Ok(new
                {
                    isSuccess = false,
                    data = new InvoiceRequestViewModel()
                });
            }
        }

        public IActionResult Add(long orderId)
        {
            ViewBag.orderId = 0;
            ViewBag.ClientId = 0;
            ViewBag.ClientName = string.Empty;
            ViewBag.TaxNo = string.Empty;
            ViewBag.Address = string.Empty;
            var orderDetail = _orderRepository.GetDetailOrderByOrderId((int)orderId).Result.FirstOrDefault();
            if (orderDetail != null)
            {
                ViewBag.orderId = orderId;
                ViewBag.ClientId = orderDetail.ClientId;
                ViewBag.ClientName = orderDetail.ClientName;
                ViewBag.TaxNo = orderDetail.TaxNo;
                ViewBag.Address = orderDetail.BusinessAddress;
            }
            var userLogin = 0;
            if (HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) != null)
            {
                userLogin = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            }
            ViewBag.UserId = userLogin;
            ViewBag.Type = (int)AttachmentType.Invoice_Request;
            return PartialView();
        }

        [HttpPost]
        public async Task<IActionResult> AddJson(InvoiceRequestViewModel model)
        {
            try
            {
                var userLogin = 0;
                if (HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) != null)
                {
                    userLogin = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                model.CreatedBy = userLogin;
                var validate = Validate(model, out string messages);
                if (validate < 0)
                {
                    return Ok(new
                    {
                        isSuccess = false,
                        message = messages
                    });
                }
                var client = new HttpClient();
                var apiPrefix = ReadFile.LoadConfig().API_URL + ReadFile.LoadConfig().API_GET_INVOICE_REQUEST_NO;
                var key_token_api = ReadFile.LoadConfig().KEY_TOKEN_API_MANUAL;
                HttpClient httpClient = new HttpClient();
                JObject jsonObject = new JObject(
                   new JProperty("code_type", ((int)GET_CODE_MODULE.YEU_CAU_XUAT_HOA_DON).ToString())
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
                model.InvoiceRequestNo = output.code;
                model.PlanDate = DateUtil.StringToDateTime(model.PlanDateStr, "dd/MM/yyyy");
                var result = _invoiceRequestRepository.CreateInvoiceRequest(model);
                if (result == -2)
                    return Ok(new
                    {
                        isSuccess = false,
                        message = "Mã yêu cầu xuất hóa đơn " + output.code + " đã tồn tại. Vui lòng kiểm tra lại"
                    });

                if (result < 1)
                    return Ok(new
                    {
                        isSuccess = false,
                        message = "Thêm yêu cầu xuất hóa đơn thất bại"
                    });
                return Ok(new
                {
                    isSuccess = true,
                    id = result,
                    message = "Thêm yêu cầu xuất hóa đơn thành công"
                });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("AddJson - InvoiceRequestController: " + ex);
                return Ok(new
                {
                    isSuccess = false,
                    message = "Thêm yêu cầu xuất hóa đơn thất bại. Đã có lỗi xảy ra."
                });
            }
        }

        public IActionResult Edit(int invoiceRequestId)
        {
            var userLogin = 0;
            if (HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) != null)
            {
                userLogin = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            }
            ViewBag.UserId = userLogin;
            var detail = _invoiceRequestRepository.GetById(invoiceRequestId);
            ViewBag.Type = (int)AttachmentType.Invoice_Request;
            return PartialView(detail);
        }

        [HttpPost]
        public async Task<IActionResult> Update(InvoiceRequestViewModel model)
        {
            try
            {
                var userLogin = 0;
                if (HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) != null)
                {
                    userLogin = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                model.UpdatedBy = userLogin;
                model.PlanDate = DateUtil.StringToDateTime(model.PlanDateStr, "dd/MM/yyyy");
                var validate = Validate(model, out string messages, true);
                if (validate < 0)
                {
                    return Ok(new
                    {
                        isSuccess = false,
                        message = messages
                    });
                }
                var result = _invoiceRequestRepository.UpdateInvoiceRequest(model);
                if (result < 1)
                    return Ok(new
                    {
                        isSuccess = false,
                        message = "Cập nhật yêu cầu xuất hóa đơn thất bại"
                    });
                return Ok(new
                {
                    isSuccess = true,
                    id = result,
                    message = "Cập nhật yêu cầu xuất hóa đơn thành công"
                });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Update - InvoiceRequestController: " + ex);
                return Ok(new
                {
                    isSuccess = false,
                    message = "Cập nhật yêu cầu xuất hóa đơn thất bại"
                });
            }
        }

        private int Validate(InvoiceRequestViewModel model, out string message, bool isEdit = false)
        {
            var result = 1;
            message = string.Empty;
            if (model.ClientId == 0)
            {
                message = "Vui lòng chọn khách hàng";
                return -1;
            }

            if (string.IsNullOrEmpty(model.CompanyName))
            {
                message = "Vui lòng nhập công ty";
                return -1;
            }

            return result;
        }

        [HttpPost]
        public IActionResult Approve(int invoiceRequestId, int status)
        {
            try
            {
                var userLogin = 0;
                if (HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) != null)
                {
                    userLogin = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                var result = _invoiceRequestRepository.ApproveInvoiceRequest(invoiceRequestId, userLogin, status);
                if (result < 1)
                    return Ok(new
                    {
                        isSuccess = false,
                        message = "Phê duyệt yêu cầu xuất hóa đơn thất bại"
                    });

                return Ok(new
                {
                    isSuccess = true,
                    message = "Phê duyệt yêu cầu xuất hóa đơn thành công"
                });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Approve - InvoiceRequestController: " + ex);
                return Ok(new
                {
                    isSuccess = false,
                    message = "Phê duyệt yêu cầu xuất hóa đơn thất bại"
                });
            }
        }

        public IActionResult Reject()
        {
            return PartialView();
        }

        [HttpPost]
        public async Task<IActionResult> RejectRequest(int invoiceRequestId, string note)
        {
            try
            {
                var userLogin = 0;
                if (HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) != null)
                {
                    userLogin = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                var result = _invoiceRequestRepository.RejectRequest(invoiceRequestId, note, userLogin);
                if (result < 1)
                    return Ok(new
                    {
                        isSuccess = true,
                        message = "Từ chối yêu cầu xuất hóa đơn thất bại"
                    });

                return Ok(new
                {
                    isSuccess = true,
                    message = "Từ chối yêu cầu xuất hóa đơn thành công"
                });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Approve - InvoiceRequestController: " + ex);
                return Ok(new
                {
                    isSuccess = false,
                    message = "Từ chối yêu cầu xuất hóa đơn thất bại"
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int invoiceRequestId)
        {
            try
            {
                var userLogin = 0;
                if (HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) != null)
                {
                    userLogin = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                var result = _invoiceRequestRepository.DeleteInvoiceRequest(invoiceRequestId, userLogin);
                if (result < 1)
                    return Ok(new
                    {
                        isSuccess = true,
                        message = "Xóa yêu cầu xuất hóa đơn thất bại"
                    });

                return Ok(new
                {
                    isSuccess = true,
                    message = "Xóa yêu cầu xuất hóa đơn thành công"
                });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Approve - InvoiceRequestController: " + ex);
                return Ok(new
                {
                    isSuccess = false,
                    message = "Xóa yêu cầu xuất hóa đơn thất bại. Vui lòng liên hệ quản trị viên"
                });
            }
        }

        [HttpPost]
        public IActionResult ExportExcel(InvoiceRequestSearchModel searchModel)
        {
            try
            {
                string _FileName = "InvoiceRequest_" + Guid.NewGuid() + ".xlsx";
                string _UploadFolder = @"Template/Export";
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
                if (searchModel.CreateByIds == null) searchModel.CreateByIds = new List<int>();
                if (!string.IsNullOrEmpty(searchModel.InvoiceRequestNo))
                    searchModel.InvoiceRequestNo = searchModel.InvoiceRequestNo.Trim();
                if (!string.IsNullOrEmpty(searchModel.Note))
                    searchModel.Note = searchModel.Note.Trim();

                string FilePath = Path.Combine(_UploadDirectory, _FileName);
                var rsPath = _invoiceRequestRepository.ExportInvoiceRequest(searchModel, FilePath);

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
                return new JsonResult(new
                {
                    isSuccess = false,
                    message = ex.Message.ToString()
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetOrderListByClientId(int clientId, int invoiceRequestId = 0)
        {
            try
            {
                var listOrder = _invoiceRequestRepository.GetByClientId(clientId, invoiceRequestId, (int)OrderStatus.WAITING_FOR_OPERATOR);
                var clientInfo = _clientRepository.GetClientDetailByClientId(clientId).Result;
                return Ok(new
                {
                    isSuccess = true,
                    message = "Thành công",
                    data = listOrder,
                    client = clientInfo
                });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetOrderListByClientId - InvoiceRequestController: " + ex);
                return Ok(new
                {
                    isSuccess = false,
                    message = "Thất bại",
                    data = new List<Entities.Models.Order>(),
                    client = new Client()
                });
            }
        }

        public IActionResult History(int requestId)
        {
            ViewBag.listRequestHistory = _invoiceRequestRepository.GetHistoriesByRequestId(requestId);
            return PartialView();
        }

        [HttpPost]
        public async Task<IActionResult> GetListFilter(string jsonData, string text = null)
        {
            try
            {
                if (string.IsNullOrEmpty(jsonData))
                    return Ok(new
                    {
                        isSuccess = true,
                        message = "Thành công",
                        data = new List<OrderViewModel>()
                    });
                var listFilter = JsonConvert.DeserializeObject<List<OrderViewModel>>(jsonData);
                if (!string.IsNullOrEmpty(text))
                    listFilter = listFilter.Where(n => n.OrderCode.Contains(text.Trim())).ToList();
                return Ok(new
                {
                    isSuccess = true,
                    message = "Thành công",
                    data = listFilter
                });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetListFilter - InvoiceRequestController: " + ex);
                return Ok(new
                {
                    isSuccess = false,
                    message = "Thất bại",
                    data = new List<OrderViewModel>()
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> FinishRequest(InvoiceRequestViewModel model)
        {
            try
            {
                var userLogin = 0;
                if (HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) != null)
                {
                    userLogin = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                model.CreatedBy = userLogin;
                var result = _invoiceRequestRepository.FinishInvoiceRequest(model);
                if (result < 1)
                    return Ok(new
                    {
                        isSuccess = false,
                        message = "Hoàn thành yêu cầu xuất hóa đơn thất bại"
                    });
                return Ok(new
                {
                    isSuccess = true,
                    id = model.Id,
                    message = "Hoàn thành yêu cầu xuất hóa đơn thành công"
                });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("FinishRequest - InvoiceRequestController: " + ex);
                return Ok(new
                {
                    isSuccess = false,
                    message = "THoàn thànhhêm yêu cầu xuất hóa đơn thất bại. Đã có lỗi xảy ra."
                });
            }
        }

    }
}
