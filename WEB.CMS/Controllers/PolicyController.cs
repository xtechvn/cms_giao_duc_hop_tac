using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Entities.Models;
using Entities.ViewModels;
using Entities.ViewModels.Policy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Repositories.IRepositories;
using Utilities;
using Utilities.Contants;
using WEB.CMS.Customize;
using WEB.CMS.Models;

namespace GDHT.CMS.Controllers
{
    [CustomAuthorize]

    public class PolicyController : Controller
    {
        private readonly IPolicyRepository _policyRepository;
        private readonly IConfiguration _configuration;
        private readonly IAllCodeRepository _allCodeRepository;
        public PolicyController(IConfiguration configuration, IPolicyRepository policyRepository, IAllCodeRepository allCodeRepository)
        {

            _configuration = configuration;
            _policyRepository = policyRepository;
            _allCodeRepository = allCodeRepository;
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                var key_token_api = _configuration["DataBaseConfig:key_api:api_manual"];
                string ApiPrefix = ReadFile.LoadConfig().API_URL + ReadFile.LoadConfig().API_ALLCODE;
                var PermisionType =  _allCodeRepository.GetListByType(AllCodeType.PERMISION_TYPE);
                ViewBag.PermisionType = PermisionType;

                return View();
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Index - PolicyController: " + ex);
                return null;
            }
        }
        public async Task<IActionResult> Search(PolicySearchViewModel searchModel, int currentPage = 1, int pageSize = 20)
        {

            var model = new GenericViewModel<PolicyViewModel>();

            try
            {
                model = await _policyRepository.GetPagingList(searchModel, currentPage, pageSize);


            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Search - PolicyController: " + ex);
            }

            return PartialView(model);
        }
        public async Task<IActionResult> Detail(int id)
        {

            try
            {
                var key_token_api = _configuration["DataBaseConfig:key_api:api_manual"];
                string ApiPrefix = ReadFile.LoadConfig().API_URL + ReadFile.LoadConfig().API_ALLCODE;

                var PermisionType = await _allCodeRepository.GetAllCodeValueByType<AllCodeData>(ApiPrefix, key_token_api, "type", "PERMISION_TYPE");
                var DebtType = await _allCodeRepository.GetAllCodeValueByType<AllCodeData>(ApiPrefix, key_token_api, "type", "DEBT_TYPE");
                var Client_Type = await _allCodeRepository.GetAllCodeValueByType<AllCodeData>(ApiPrefix, key_token_api, "type", "CLIENT_TYPE");
                var ClientType = Client_Type.Data.Where(s => s.CodeValue != Utilities.Contants.ClientType.kl).ToList();
                ViewBag.PermisionType = PermisionType.Data;
                ViewBag.DebtType = DebtType.Data;
                ViewBag.ClientType = ClientType;
                if (id != 0)
                {
                    var model = _policyRepository.DetailPolicy(id).Result;
                    return PartialView(model);
                }
                return PartialView();
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Detail - PolicyController: " + ex);
                return PartialView();
            }

        }
        public async Task<IActionResult> DetailTable(int id)
        {

            try
            {
                var key_token_api = _configuration["DataBaseConfig:key_api:api_manual"];
                string ApiPrefix = ReadFile.LoadConfig().API_URL + ReadFile.LoadConfig().API_ALLCODE;

                var PermisionType = await _allCodeRepository.GetAllCodeValueByType<AllCodeData>(ApiPrefix, key_token_api, "type", "PERMISION_TYPE");
                var DebtType = await _allCodeRepository.GetAllCodeValueByType<AllCodeData>(ApiPrefix, key_token_api, "type", "DEBT_TYPE");
                var Client_Type = await _allCodeRepository.GetAllCodeValueByType<AllCodeData>(ApiPrefix, key_token_api, "type", "CLIENT_TYPE");
                var ClientType = Client_Type.Data.Where(s => s.CodeValue != Utilities.Contants.ClientType.kl).ToList();
                ViewBag.PermisionType = PermisionType.Data;
                ViewBag.DebtType = DebtType.Data;
                ViewBag.ClientType = ClientType;
                ViewBag.Type = id;
                if (id != 0)
                {
                    var model = 0;
                    return PartialView(model);
                }

                return PartialView();
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("DetailTable - PolicyController: " + ex);
                return PartialView();
            }            
        }
        public async Task<IActionResult> Setup(AddPolicyDtailViewModel data)
        {
            int stt_code = (int)ResponseType.FAILED;
            string msg = "Error On Excution";
            try
            {
                data.CreatedBy = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                data.PolicyCode = "CSHT";
                if (data.PolicyId == 0)
                {
                    var CreatedBy = await _policyRepository.CreatePolicy(data);

                    if (CreatedBy != 0 && CreatedBy != -1)
                    {
                        stt_code = (int)ResponseType.SUCCESS;
                        msg = "Thêm mới thành công";
                    }
                    else
                    {
                        stt_code = (int)ResponseType.ERROR;
                        msg = "Thêm mới không thành công";
                    }
                }
                else
                {
                    var CreatedBy = await _policyRepository.updatePolicy(data);
                    if (CreatedBy != 0 && CreatedBy != -1)
                    {
                        stt_code = (int)ResponseType.SUCCESS;
                        msg = "Cập nhật thành công";
                    }
                    else
                    {
                        stt_code = (int)ResponseType.ERROR;
                        msg = "Cập nhật không thành công";
                    }
                }


            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Setup - PolicyController: " + ex);
                stt_code = (int)ResponseType.ERROR;
                msg = "Lỗi kỹ thuật vui lòng liên hệ bộ phận IT";
            }
            return Ok(new
            {
                stt_code = stt_code,
                msg = msg,

            });
        }
        [HttpPost]
        public async Task<IActionResult> AddPolicyPackages(long policy_id)
        {
            try
            {
                var key_token_api = _configuration["DataBaseConfig:key_api:api_manual"];
                string ApiPrefix = ReadFile.LoadConfig().API_URL + ReadFile.LoadConfig().API_ALLCODE;


                var DebtType = await _allCodeRepository.GetAllCodeValueByType<AllCodeData>(ApiPrefix, key_token_api, "type", "DEBT_TYPE");
                var ClientType = await _allCodeRepository.GetAllCodeValueByType<AllCodeData>(ApiPrefix, key_token_api, "type", "CLIENT_TYPE");
                ViewBag.DebtType = DebtType.Data;
                ViewBag.ClientType = ClientType.Data;
                //var policy =  _policyRepository.GetPolicyDetail(policy_id);

                //if (policy == null || policy.PolicyId < 0)
                //{
                //    ViewBag.ExtraPackages = new List<Policy>();
                //    return View();

                //}
                return View();
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("AddPolicyPackages - PolicyController: " + ex);
                return null;
            }
            
        }
        public async Task<IActionResult> PolicyDetail(int id)
        {
            try
            {
                var key_token_api = _configuration["DataBaseConfig:key_api:api_manual"];
                string ApiPrefix = ReadFile.LoadConfig().API_URL + ReadFile.LoadConfig().API_ALLCODE;

                var PermisionType = await _allCodeRepository.GetAllCodeValueByType<AllCodeData>(ApiPrefix, key_token_api, "type", "PERMISION_TYPE");
                var DebtType = await _allCodeRepository.GetAllCodeValueByType<AllCodeData>(ApiPrefix, key_token_api, "type", "DEBT_TYPE");
                var Client_Type = await _allCodeRepository.GetAllCodeValueByType<AllCodeData>(ApiPrefix, key_token_api, "type", "CLIENT_TYPE");
                var ClientType = Client_Type.Data.Where(s => s.CodeValue != Utilities.Contants.ClientType.kl).ToList();
                ViewBag.PermisionType = PermisionType.Data;
                ViewBag.DebtType = DebtType.Data;
                ViewBag.ClientType = ClientType;
                if (id != 0)
                {
                    var model = _policyRepository.DetailPolicy(id).Result;
                    return PartialView(model);
                }
                return PartialView();
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("PolicyDetail - PolicyController: " + ex);
                return PartialView();
            }
        }
        public async Task<IActionResult> PolicyDelete(int id)
        {

            int stt_code = (int)ResponseType.ERROR;
            string msg = "Error On Excution";
            try
            {
                if (id != 0)
                {
                    var model = await _policyRepository.DeletePolicy(id);
                    if (model != 0 && model != -1)
                    {
                        stt_code = (int)ResponseType.SUCCESS;
                        msg = "Xóa chính sách thành công";
                    }
                    else
                    {
                        stt_code = (int)ResponseType.ERROR;
                        msg = "Xóa chính sách không thành công";
                    }

                }

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("PolicyDetail - PolicyController: " + ex);
                stt_code = (int)ResponseType.ERROR;
                msg = "Xóa chính sách không thành công, vui lòng liên hệ IT";
            }
            return Ok(new
            {
                stt_code = stt_code,
                msg = msg,

            });
        }
    }
}
