using Entities.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repositories.IRepositories;
using System;
using System.Linq;
using System.Threading.Tasks;
using Utilities;
using WEB.CMS.Customize;

namespace GDHT.CMS.Controllers
{
    [CustomAuthorize]
    public class VinWonderPolicyController : Controller
    {
        private readonly IPolicyRepository _PolicyRepository;
        private readonly IAllCodeRepository _AllCodeRepository;
        private readonly ICampaignRepository _CampaignRepository;

        public VinWonderPolicyController(IPolicyRepository policyRepository, IAllCodeRepository allCodeRepository,
            ICampaignRepository campaignRepository)
        {
            _PolicyRepository = policyRepository;
            _AllCodeRepository = allCodeRepository;
            _CampaignRepository = campaignRepository;
        }

        public async Task<IActionResult> Upsert(int id)
        {
            try
            {
                var model = new CampaignViewModel();
                if (id > 0)
                {
                    var campaign_model = await _CampaignRepository.GetDetailByID(id);
                    model = new CampaignViewModel
                    {
                        Id = id,
                        CampaignCode = campaign_model.CampaignCode,
                        ClientTypeId = campaign_model.ClientTypeId,
                        ContractType = campaign_model.ContractType,
                        Description = campaign_model.Description,
                        FromDate = campaign_model.FromDate,
                        ToDate = campaign_model.ToDate,
                        Status = campaign_model.Status,
                        PricePolycies = _PolicyRepository.GetVinWonderPricePolicyByCampaignId(id)
                    };
                }

                ViewBag.CommonProfit = _AllCodeRepository.GetListByType("PROFIT_VIN_WONDER_PRICE");
                return View(model);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Upsert - VinWonderPolicyController: " + ex);
                return null;
            }
           
        }

        [HttpPost]
        public IActionResult UpsertCampaign(CampaignViewModel model)
        {
            try
            {
                var campaign_id = 0;
                if (model.Id > 0)
                {
                    campaign_id = _PolicyRepository.UpdateCampaign(model);
                }
                else
                {
                    campaign_id = _PolicyRepository.CreateCampaign(model);
                }

                if (campaign_id > 0)
                {
                    if (model.PricePolycies != null && model.PricePolycies.Any())
                    {
                        _PolicyRepository.UpSertVinWonderPricePolicy(campaign_id, model.PricePolycies);
                    }

                    return new JsonResult(new
                    {
                        isSuccess = true,
                        message = "Cập nhật chiến dịch thành công"
                    });
                }
                else
                {
                    return new JsonResult(new
                    {
                        isSuccess = false,
                        message = "Cập nhật chiến dịch thất bại"
                    });
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpsertCampaign - VinWonderPolicyController: " + ex);
                return new JsonResult(new
                {
                    isSuccess = false,
                    message = ex.Message
                });
            }
        }

        public IActionResult UpdateCommonProfit(int type, decimal value)
        {
            try
            {
                var result = _PolicyRepository.UpdateVinWonderCommonProfit(new VinWonderCommonProfitModel
                {
                    Id = 0,
                    CodeValue = (short)type,
                    Description = value.ToString(),
                    Type = "PROFIT_VIN_WONDER_PRICE"
                });

                if (result > 0)
                {
                    return new JsonResult(new
                    {
                        isSuccess = true,
                        message = "Cập nhật lợi nhuận chung thành công"
                    });
                }
                else
                {
                    return new JsonResult(new
                    {
                        isSuccess = false,
                        message = "Cập nhật lợi nhuận chung thất bại"
                    });
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateCommonProfit - VinWonderPolicyController: " + ex);
                return new JsonResult(new
                {
                    isSuccess = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ImportDataPrice(IFormFile file)
        {
            try
            {
                var model = await _PolicyRepository.ImportExcelAsync(file);
                ViewBag.CheckedAll = true;
                return View("GridPrice", model);
            }
            catch(Exception ex)
            {
                LogHelper.InsertLogTelegram("ImportDataPrice - VinWonderPolicyController: " + ex.ToString());
                return new EmptyResult();
            }
        }


    }
}
