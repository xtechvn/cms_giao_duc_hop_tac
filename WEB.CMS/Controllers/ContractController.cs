using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Caching.Elasticsearch;
using Entities.Models;
using Entities.ViewModels;
using Entities.ViewModels.Contract;
using Entities.ViewModels.ElasticSearch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Repositories.IRepositories;
using Utilities;
using Utilities.Contants;
using GDHT.CMS.Service;
using WEB.CMS.Customize;
using WEB.CMS.Models;


namespace GDHT.CMS.Controllers
{
    [CustomAuthorize]

    public class ContractController : Controller
    {

        private readonly IConfiguration _configuration;
        private readonly IAllCodeRepository _allCodeRepository;
        private readonly IContractRepository _contractRepository;
        private UserESRepository _userESRepository;
        private ContractESRepository _contractESRepository;

        private IClientRepository _clientRepository;
        private IUserAgentRepository _userAgentRepository;
        private IPolicyRepository _policyRepository;
        private IIdentifierServiceRepository _identifierServiceRepository;
        private ManagementUser _ManagementUser;
        private IUserRepository _userRepository;
        private APIService apiService;
        private ICustomerManagerRepository _customerManagerRepository;

        public ContractController(IConfiguration configuration, IAllCodeRepository allCodeRepository, IContractRepository contractRepository, ManagementUser ManagementUser, IUserRepository userRepository,
            IClientRepository clientRepository, IUserAgentRepository userAgentRepository, IIdentifierServiceRepository identifierServiceRepository, IPolicyRepository policyRepository, ICustomerManagerRepository customerManagerRepository)
        {

            _configuration = configuration;
            _allCodeRepository = allCodeRepository;
            _contractRepository = contractRepository;
            _userESRepository = new UserESRepository(_configuration["DataBaseConfig:Elastic:Host"]);
            _contractESRepository = new ContractESRepository(_configuration["DataBaseConfig:Elastic:Host"]);
            _clientRepository = clientRepository;
            _userAgentRepository = userAgentRepository;
            _identifierServiceRepository = identifierServiceRepository;
            _policyRepository = policyRepository;
            _ManagementUser = ManagementUser;
            _userRepository = userRepository;
            apiService = new APIService(configuration, userRepository);
            _customerManagerRepository = customerManagerRepository;

        }
        public async Task<IActionResult> Index()
        {

            var key_token_api = _configuration["DataBaseConfig:key_api:api_manual"];
            string ApiPrefix = ReadFile.LoadConfig().API_URL + ReadFile.LoadConfig().API_ALLCODE;
            var AgencyType = _allCodeRepository.GetListByType(AllCodeType.AGENCY_TYPE);
            var PermisionType = _allCodeRepository.GetListByType(AllCodeType.PERMISION_TYPE);
            var ClientType = _allCodeRepository.GetListByType(AllCodeType.CLIENT_TYPE);
            var ContractStatus = _allCodeRepository.GetListByType(AllCodeType.CONTRACT_STATUS);
            var DEBT_TYPE = _allCodeRepository.GetListByType(AllCodeType.DEBT_TYPE);
            ViewBag.AgencyType = AgencyType;//tổ chức
            ViewBag.PermisionType = PermisionType;//nhóm kh
            ViewBag.ClientType = ClientType;//loại kh
            ViewBag.ContractStatus = ContractStatus;//loại kh
            ViewBag.DEBT_TYPE = DEBT_TYPE;//loại kh
            var searchModel = new ContractSearchViewModel();

            ViewBag.PermissionsStatus = 0;
            var current_user = _ManagementUser.GetCurrentUser();
            if (current_user != null)
            {
                var i = 0;
                if (current_user != null && !string.IsNullOrEmpty(current_user.Role))
                {
                    var list = Array.ConvertAll(current_user.Role.Split(','), int.Parse);
                    foreach (var item in list)
                    {
                        //kiểm tra chức năng có đc phép sử dụng
                        var listPermissions = await _userRepository.CheckRolePermissionByUserAndRole(current_user.Id, item, (int)SortOrder.THEM, (int)MenuId.HOP_DONG);
                        if (listPermissions == true)
                        {
                            ViewBag.PermissionsStatus = 1;
                        }

                    }


                    foreach (var item in list)
                    {
                        //kiểm tra chức năng có đc phép sử dụng
                        var listPermissions = await _userRepository.CheckRolePermissionByUserAndRole(current_user.Id, item, (int)SortOrder.TRUY_CAP, (int)MenuId.HOP_DONG);
                        var listPermissions6 = await _userRepository.CheckRolePermissionByUserAndRole(current_user.Id, item, (int)SortOrder.VIEW_ALL, (int)MenuId.HOP_DONG);
                        var listPermissions7 = await _userRepository.CheckRolePermissionByUserAndRole(current_user.Id, item, (int)SortOrder.DUYET, (int)MenuId.HOP_DONG);

                        if (listPermissions == true)
                        {
                            searchModel.SalerPermission = current_user.UserUnderList; i++;
                        }
                        if ((listPermissions == true && listPermissions7 == true))
                        {
                            searchModel.SalerPermission = current_user.UserUnderList;
                            i++;
                        }
                        if (listPermissions6 == true)
                        {
                            searchModel.SalerPermission = null;
                            i++;
                        }
                    }
                    if (i != 0)
                    {
                        var data = await _contractRepository.TotalConTract(searchModel, 0, 0);
                        if (data != null)
                        {
                            ViewBag.TotalConTract = data;//loại kh
                            int Total = 0;
                            foreach (var t in data)
                            {
                                Total = Total + t.Total;
                            }
                            ViewBag.Total = Total;//loại kh
                        }


                    }
                }
            }
            return View();
        }

        public async Task<IActionResult> Search(ContractSearchViewModel searchModel, int currentPage = 1, int pageSize = 20)
        {
            var model = new GenericViewModel<ContractViewModel>();

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
                            //kiểm tra chức năng có đc phép sử dụng
                            var listPermissions = await _userRepository.CheckRolePermissionByUserAndRole(current_user.Id, item, (int)SortOrder.TRUY_CAP, (int)MenuId.HOP_DONG);
                            var listPermissions6 = await _userRepository.CheckRolePermissionByUserAndRole(current_user.Id, item, (int)SortOrder.VIEW_ALL, (int)MenuId.HOP_DONG);
                            var listPermissions7 = await _userRepository.CheckRolePermissionByUserAndRole(current_user.Id, item, (int)SortOrder.DUYET, (int)MenuId.HOP_DONG);

                            if (listPermissions == true)
                            {
                                searchModel.SalerPermission = current_user.UserUnderList; i++;
                            }
                            if ((listPermissions == true && listPermissions7 == true))
                            {
                                searchModel.SalerPermission = current_user.UserUnderList;
                                i++;
                            }
                            if (listPermissions6 == true)
                            {
                                searchModel.SalerPermission = null;
                                i++;
                            }
                        }
                    }
                    if (i != 0)
                    {
                        model = await _contractRepository.GetPagingList(searchModel, currentPage, pageSize);
                    }
                    else
                    {
                        return PartialView(model);
                    }
                }


            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Search - ContractController: " + ex);
            }

            return PartialView(model);
        }
        public async Task<IActionResult> Detail(int id)
        {
      

            var DebtType = _allCodeRepository.GetListByType(AllCodeType.DEBT_TYPE);
            var PermisionType = _allCodeRepository.GetListByType(AllCodeType.PERMISION_TYPE);
            var ClientType = _allCodeRepository.GetListByType(AllCodeType.CLIENT_TYPE);
            var Service_type = _allCodeRepository.GetListByType(AllCodeType.SERVICE_TYPE);



            ViewBag.DebtType = DebtType;
            ViewBag.PermisionType = PermisionType;
            ViewBag.ClientType = ClientType;
            ViewBag.Servicetype = Service_type;
            try
            {
                if (id != 0)
                {
                    var model = await _contractRepository.GetDetailContract(id);
                    model[0].ExpireDate = Convert.ToDateTime(model[0].ExpireDate);
                    return PartialView(model[0]);
                }
                return PartialView();
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Detail - ContractController: " + ex);
                return PartialView();
            }
        }

        public async Task<IActionResult> loadPolicy(int ClientType, int PermisionType, int type)
        {
            try
            {

                var DebtType = _allCodeRepository.GetListByType("DEBT_TYPE"); ;
                ViewBag.DebtType = DebtType;
                var data = _contractRepository.GetPolicyDetailByType(ClientType, PermisionType);
                ViewBag.PermisionType = PermisionType;
                ViewBag.type = type;
                return PartialView(data);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("loadPolicy - ContractController: " + ex);
                return PartialView();
            }
        }
        public async Task<IActionResult> ClientSuggestion(string txt_search)
        {

            try
            {

                if (string.IsNullOrEmpty(txt_search))
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.EMPTY
                    });
                }
                else
                {
                    bool isUnicode = Encoding.ASCII.GetByteCount(txt_search) != Encoding.UTF8.GetByteCount(txt_search);


                    byte[] utfBytes = Encoding.UTF8.GetBytes(txt_search.Trim());
                    txt_search = Encoding.UTF8.GetString(utfBytes);
                }

                var es_service = new esService(_configuration);
                var data_hotel = await es_service.search(txt_search, "searchClient.json");
                if (data_hotel != "{}")
                {
                    //var es_result =// ((RestSharp.RestResponseBase)find_hotel).Content;                       

                    JObject jsonObject = JObject.Parse(data_hotel);
                    var hits = (JArray)jsonObject["hits"]["hits"];
                    var hotel_result = new List<earchClientESViewModel>();
                    foreach (var hit in hits)
                    {
                        var source = JsonConvert.DeserializeObject<earchClientESViewModel>(hit["_source"].ToString());
                        hotel_result.Add(source);
                    }

                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        data = hotel_result,
                    });
                    //var data = await _clientESRepository.GetClientSuggesstion2(txt_search);

                }
                else
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.EMPTY,
                        msg = "Không có dữ liệu nào thỏa mãn từ khóa " + txt_search
                    });
                }
                //var data = await _clientESRepository.GetClientSuggesstion(txt_search);
                //return Ok(new
                //{
                //    status = (int)ResponseType.SUCCESS,
                //    data = data,
                //});

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ClientSuggestion - ContractController: " + ex);
                return Ok(new
                {
                    status = (int)ResponseType.SUCCESS,
                    data = new List<CustomerESViewModel>()
                });
            }

        }
        public async Task<IActionResult> ContractNoSuggestion(string txt_search)
        {

            try
            {

                if (string.IsNullOrEmpty(txt_search))
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.EMPTY
                    });
                }
                else
                {

                    var data = await _contractESRepository.GetContractNoSuggesstion(txt_search);
                    if (data != null)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.SUCCESS,
                            data = data,

                        });
                    }
                    else
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.SUCCESS,
                            data = new List<ContractNoESViewModel>()

                        });
                    }



                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ContractNoSuggestion - ContractController: " + ex);
                return Ok(new
                {
                    status = (int)ResponseType.SUCCESS,
                    data = new List<ContractNoESViewModel>()
                });
            }

        }
        public async Task<IActionResult> GetSuggestionClientBySale(string txt_search)
        {
            try
            {
                if (string.IsNullOrEmpty(txt_search))
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.EMPTY
                    });
                }
                else
                {
                    bool isUnicode = Encoding.ASCII.GetByteCount(txt_search) != Encoding.UTF8.GetByteCount(txt_search);
                    byte[] utfBytes = Encoding.UTF8.GetBytes(txt_search.Trim());
                    txt_search = Encoding.UTF8.GetString(utfBytes);
                }
                string saleid = Convert.ToString(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var current_user = _ManagementUser.GetCurrentUser();
                var es_service = new esService(_configuration);
                var data_client = await es_service.search(txt_search, "searchClient.json");
                if (data_client != "{}")
                {
                    JObject jsonObject = JObject.Parse(data_client);
                    var hits = (JArray)jsonObject["hits"]["hits"];
                    var client_result = new List<earchClientESViewModel>();
                    if (current_user.Role.Contains(((int)RoleType.Admin).ToString()))
                    {
                        foreach (var hit in hits)
                        {
                            var source = JsonConvert.DeserializeObject<earchClientESViewModel>(hit["_source"].ToString());
                            client_result.Add(source);
                        }
                    }
                    else
                    {
                        foreach (var hit in hits)
                        {
                            var source = JsonConvert.DeserializeObject<earchClientESViewModel>(hit["_source"].ToString());

                            if ((current_user == null ? saleid : current_user.UserUnderList).Contains(source.userid.ToString()))
                            {
                                client_result.Add(source);
                            }
                        }
                    }


                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        data = client_result,

                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.EMPTY,
                        msg = "Không có dữ liệu nào thỏa mãn từ khóa " + txt_search
                    });
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetSuggestionClientBySale - OrderManualController: " + ex.ToString());
                return Ok(new
                {
                    status = (int)ResponseType.SUCCESS,
                    data = new List<CustomerESViewModel>(),

                });
            }
        }
        [HttpPost]
        public async Task<IActionResult> Setup(ContractViewModel model)
        {
            int stt_code = (int)ResponseType.FAILED;
            string msg = "Error On Excution";
            try
            {
                string userId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value).ToString();

                APIService apiService = new APIService(_configuration, _userRepository);
                var userAgent = _userAgentRepository.GetUserAgentClient(model.ClientId);
                model.UserIdCreate = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

                if (model.ContractNo == null)
                {
                    model.ContractNo = await apiService.buildContractNo();

                }

                if (userAgent == null)
                {
                    model.SalerId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                else
                {
                    model.SalerId = userAgent.UserId;
                }
                if (model.ContractExpireDate != null)
                {
                    model.ExpireDate = CheckDate(model.ContractExpireDate);
                }
                if (model.ContractNo != null)
                {
                    var data = await _contractRepository.CreateContact(model);
                    if (data != 0)
                    {
                        //var SendMessage = apiService.SendMessage(userId, ModuleType.HOP_DONG.ToString(), ActionType.TAO_MOI.ToString(), model.ContractNo);
                        stt_code = (int)ResponseType.SUCCESS;
                        msg = "Gửi thành công";
                        if (model.ContractStatus == ContractStatus.DOI_DUYET)
                        {
                            var current_user = _ManagementUser.GetCurrentUser();
                            string link = "/Contract/DetailContract/" + data;
                            apiService.SendMessage(userId, ((int)ModuleType.HOP_DONG).ToString(), ((int)ActionType.DUYET).ToString(), model.ContractNo, link, current_user.Role);
                        }
                    }
                    else
                    {
                        stt_code = (int)ResponseType.SUCCESS;
                        msg = "Gửi không thành công";
                    }

                }
                else
                {
                    stt_code = (int)ResponseType.ERROR;
                    msg = "Thêm chính sách không thành công";
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Setup - ContractController: " + ex);
                stt_code = (int)ResponseType.ERROR;
                msg = "Lỗi kỹ thuật vui lòng liên hệ bộ phận IT";
            }

            return Ok(new
            {
                stt_code = stt_code,
                msg = msg,

            });
        }
        public async Task<IActionResult> ClientDetail(int client)
        {
            int stt_code = 0;
            int stt_ClientType = 0;
            int stt_PermisionType = 0;

            try
            {
                var data = await _clientRepository.GetClientDetailByClientId(client);
                if (data != null)
                {
                    stt_code = (int)data.AgencyType;
                    stt_ClientType = (int)data.ClientType;
                    stt_PermisionType = (int)data.PermisionType;
                }


            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ClientDetail - ContractController: " + ex);
            }
            return Ok(new
            {
                stt_code = stt_code,
                stt_ClientType = stt_ClientType,
                stt_PermisionType = stt_PermisionType,
            });
        }
        public async Task<IActionResult> DetailContract(int id)
        {

            try
            {
                if (id != 0)
                {
                    ViewBag.UserLogin = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                    var current_user = _ManagementUser.GetCurrentUser();
                    ViewBag.DUYETHD = 0;
                    if (current_user != null && !string.IsNullOrEmpty(current_user.Role))
                    {
                        var list = Array.ConvertAll(current_user.Role.Split(','), int.Parse);
                        foreach (var item in list)
                        {
                            //kiểm tra chức năng có đc phép sử dụng
                            var ListRolePermission = await _userRepository.CheckRolePermissionByUserAndRole(current_user.Id, item, (int)SortOrder.DUYET, (int)MenuId.HOP_DONG);
                            if (ListRolePermission == true)
                            {
                                ViewBag.DUYETHD = 1;
                            }
                        }
                    }
                   
                    var DebtType= _allCodeRepository.GetListByType(AllCodeType.DEBT_TYPE);
                    ViewBag.DebtType = DebtType;
                    var ActionBy = -1;
                    var ActionBy2 = 2;
                    var data = await _contractRepository.GetPagingListContractHistory(id, ActionBy);
                    var data2 = await _contractRepository.GetPagingListContractHistory(id, ActionBy2);
                    var model = await _contractRepository.GetDetailContract(id);
                    if (model != null)
                    {
                        if (data != null) { ViewBag.ContractHistory = data; }

                        if (data2 != null) { ViewBag.ContractHistory2 = data2; }
                        return View(model[0]);

                    }
                    else
                    {
                        return View();

                    }

                }
                return View();
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("DetailContract - ContractController: " + ex);
                return View();
            }
        }
        [HttpPost]
        public async Task<IActionResult> ResetStatus(long id, long Status, string Note)
        {
            int status = 0;
            string msg = "Thay đổi không thành công";
            try
            {
                string userId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value).ToString();

                if (Status != ContractStatus.HUY_BO)
                {
                    var model = await _contractRepository.GetDetailContract((int)id);
                    var Contract_model = new Contract();
                    if (Status == ContractStatus.DA_DUYET || Status == ContractStatus.TU_CHOI)
                    {
                        Contract_model.VerifyStatus = (byte)Status;
                    }
                    var datetime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                    Contract_model.UpdateLast = Convert.ToDateTime(datetime);
                    Contract_model.ContractDate = model[0].ContractDate;
                    Contract_model.ClientId = model[0].ClientId;
                    Contract_model.UserIdCreate = model[0].UserIdCreate;
                    Contract_model.CreateDate = model[0].CreateDate;
                    Contract_model.ExpireDate = model[0].ExpireDate;
                    Contract_model.VerifyDate = model[0].VerifyDate;
                    Contract_model.VerifyStatus = model[0].VerifyStatus;
                    Contract_model.ContractStatus = Convert.ToInt32(Status);
                    Contract_model.ContractNo = model[0].ContractNo;
                    Contract_model.UserIdVerify = model[0].UserIdCreate;
                    Contract_model.DebtType = model[0].DebtType;
                    Contract_model.ServiceType = model[0].ServiceType;
                    Contract_model.TotalVerify = model[0].TotalVerify;
                    Contract_model.ClientType = model[0].ClientType;
                    Contract_model.PermisionType = model[0].PermisionType;
                    Contract_model.ContractId = model[0].ContractId;
                    Contract_model.Note = model[0].Note;
                    Contract_model.IsDelete = model[0].IsDelete;
                    Contract_model.SalerId = model[0].SalerId;
                    Contract_model.PolicyId = model[0].PolicyId;
                    Contract_model.UserIdUpdate = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                    if (Contract_model.SalerId == 0) { Contract_model.SalerId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value); }

                    var data = await _contractRepository.CreateContact2(Contract_model, Note);
                    if (data != 0)
                    {
                        var current_user = _ManagementUser.GetCurrentUser();
                        string link = "/Contract/DetailContract/" + id;
                        if (Status == ContractStatus.DA_DUYET)
                        {
                           _customerManagerRepository.ResetStatusAc(model[0].ClientId, (long)StatusType.BINH_THUONG, (int)model[0].ClientType);
                            apiService.SendMessage(userId, ((int)ModuleType.HOP_DONG).ToString(), ((int)ActionType.DA_DUYET).ToString(), model[0].ContractNo, link, current_user.Role);
                        }
                        if (Status == ContractStatus.TU_CHOI)
                        {
                            apiService.SendMessage(userId, ((int)ModuleType.HOP_DONG).ToString(), ((int)ActionType.TU_CHOI_HOP_DONG).ToString(), model[0].ContractNo, link, current_user.Role);
                        }

                        status = (int)ResponseType.SUCCESS;
                        msg = "Thay đổi trạng tái thành công";
                    }
                    else
                    {
                        status = (int)ResponseType.SUCCESS;
                        msg = "Thay đổi trạng tái không thành công";
                    }
                }
                else
                {
                    var model = await _contractRepository.GetDetailContract((int)id);
                    var UserIdUpdate = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                    var data = await _contractRepository.UpdataContactStatus(id, Status, Note, UserIdUpdate);
                    if (data != 0)
                    {
                        // apiService.SendMessage( userId, ModuleType.HOP_DONG.ToString(), ActionType.HUY.ToString(), model[0].ContractNo);
                        status = (int)ResponseType.SUCCESS;
                        msg = "Hủy hợp đồng thành công";
                    }
                    else
                    {
                        status = (int)ResponseType.FAILED;
                        msg = "Hủy không thành công";
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ResetStatus - ContractController: " + ex.ToString());
                return Ok(new
                {
                    status = (int)ResponseType.FAILED,
                    msg = "Không thành công vui lòng liên hệ bộ phận IT"
                });
            }
            return Ok(new
            {
                status = status,
                msg = msg
            });

        }
        public async Task<IActionResult> ContractStatusDetail(int id)
        {

            try
            {
                ViewBag.id = id;
                return View();
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ContractStatusDetail - ContractController: " + ex);
                return View();
            }
        }
        public async Task<IActionResult> DeleteContract(int id)
        {
            int status = 0;
            string msg = "Thay đổi không thành công";
            try
            {
                var data = await _contractRepository.DeleteContact(id);
                if (data != 0)
                {
                    status = (int)ResponseType.SUCCESS;
                    msg = "Xóa hợp đồng thành công";
                }
                else
                {
                    status = (int)ResponseType.FAILED;
                    msg = "Xóa không thành công";
                }

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("DetailContract - ContractController: " + ex);
                status = (int)ResponseType.FAILED;
                msg = "Không thành công vui lòng liên hệ bộ phận IT";
            }
            return Ok(new
            {
                status = status,
                msg = msg
            });
        }
        private DateTime CheckDate(string dateTime)
        {
            DateTime _date = DateTime.MinValue;
            if (!string.IsNullOrEmpty(dateTime))
            {
                _date = DateTime.ParseExact(dateTime, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }

            return _date != DateTime.MinValue ? _date : DateTime.MinValue;
        }
        //public async Task<IActionResult> ClientSuggestion3(string txt_search)
        //{

        //    try
        //    {
        //        if (txt_search != null)
        //        {
        //            var data = await _clientESRepository.GetClientSuggesstion(txt_search);
        //            return Ok(new
        //            {
        //                status = (int)ResponseType.SUCCESS,
        //                data = data
        //            });
        //        }
        //        else
        //        {
        //            return Ok(new
        //            {
        //                status = (int)ResponseType.SUCCESS,
        //                data = new List<CustomerESViewModel>()
        //            });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogHelper.InsertLogTelegram("ClientSuggestion3 - ContractController: " + ex);
        //        return Ok(new
        //        {
        //            status = (int)ResponseType.SUCCESS,
        //            data = new List<CustomerESViewModel>()
        //        });
        //    }

        //}
        [HttpPost]
        public async Task<IActionResult> CountStatus(ContractSearchViewModel searchModel, int currentPage = 1, int pageSize = 20)
        {
            var model = new GenericViewModel<ContractViewModel>();
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
                            //kiểm tra chức năng có đc phép sử dụng
                            var listPermissions = await _userRepository.CheckRolePermissionByUserAndRole(current_user.Id, item, (int)SortOrder.TRUY_CAP, (int)MenuId.HOP_DONG);
                            var listPermissions6 = await _userRepository.CheckRolePermissionByUserAndRole(current_user.Id, item, (int)SortOrder.VIEW_ALL, (int)MenuId.HOP_DONG);
                            var listPermissions7 = await _userRepository.CheckRolePermissionByUserAndRole(current_user.Id, item, (int)SortOrder.DUYET, (int)MenuId.HOP_DONG);

                            if (listPermissions == true)
                            {
                                searchModel.SalerPermission = current_user.UserUnderList; i++;
                            }
                            if ((listPermissions == true && listPermissions7 == true))
                            {
                                searchModel.SalerPermission = current_user.UserUnderList;
                                i++;
                            }
                            if (listPermissions6 == true)
                            {
                                searchModel.SalerPermission = null;
                                i++;
                            }
                        }
                    }
                    if (i != 0)
                    {
                        model = await _contractRepository.GetPagingList(searchModel, currentPage, pageSize);

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
                LogHelper.InsertLogTelegram("CountStatus - ContractController: " + ex);
                return Ok(new
                {
                    isSuccess = false,
                    data = 0
                });
            }
            return Ok(new
            {
                isSuccess = false,
                data = 0
            });
        }
        [HttpPost]
        public async Task<IActionResult> UpdataPolicydetail(PolicyDetail Model)
        {
            var status = (int)ResponseType.FAILED;
            var msg = "Hủy không thành công";
            try
            {
                Model.UpdatedBy = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var data = await _policyRepository.UpdatePolicyDetail(Model);
                if (data > 0)
                {
                    status = (int)ResponseType.SUCCESS;
                    msg = "Thành công";
                }
                else
                {
                    status = (int)ResponseType.ERROR;
                    msg = "Không thành công";
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdataPolicydetail - ContractController: " + ex);
                status = (int)ResponseType.ERROR;
                msg = "không thành công vui lòng liên hệ IT";
            }
            return Ok(new
            {
                status = status,
                msg = msg
            });
        }

    }
}
