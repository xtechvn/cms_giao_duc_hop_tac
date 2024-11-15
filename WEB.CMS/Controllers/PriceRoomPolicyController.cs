using Caching.RedisWorker;
using Entities.Models;
using Entities.ViewModels;
using Entities.ViewModels.PricePolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;
using WEB.CMS.Customize;

namespace GDHT.CMS.Controllers
{

    public partial class PricePolicyController : Controller
    {


        public async Task<IActionResult> RoomPricePolicy(string campaign_code, int campaign_id)
        {

            ViewBag.Hotel = new HotelESViewModel();
            ViewBag.Campaign = new Campaign();
            ViewBag.ClientType = new List<AllCode>();
            try
            {
                bool is_campaign_exists = false;
                Campaign campaign = await _campaignRepository.GetDetailByID(campaign_id);
                if (campaign == null || campaign.Id <= 0)
                {
                    campaign = await _campaignRepository.GetDetailByCode(campaign_code);
                    if (campaign != null && campaign.Id > 0)
                    {
                        is_campaign_exists = true;
                    }
                }
                else
                {
                    is_campaign_exists = true;
                }
                if (is_campaign_exists)
                {
                    ViewBag.Campaign = campaign;
                    var campaign_detail = await _campaignRepository.GetFirstProductServiceRoombyCampaignID(campaign_id);
                    if (campaign_detail.Id > 0)
                    {
                        ViewBag.Hotel = await _hotelESRepository.GetHotelByID((int)campaign_detail.HotelId);
                    }
                }
                ViewBag.ClientType = _allCodeRepository.GetListByType(AllCodeType.CLIENT_TYPE);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("RoomPricePolicy - PricePolicyController: " + ex.ToString());

            }
            return PartialView();
        }

        [HttpPost]
        public async Task<IActionResult> PolicyRule(string campaign_id, string hotel_id,DateTime? from_date, DateTime? to_date)
        {
            ViewBag.Model = new HotelPricePolicyCampaignModel()
            {
                PricePolicy = new List<HotelPricePolicyDetail>(),
                Detail = new Campaign()
                {
                    Id = -1,
                    FromDate = from_date?? new DateTime(DateTime.Now.Year, 01, 01, 00, 00, 00),
                    ToDate = to_date?? new DateTime(DateTime.Now.Year + 1, 01, 01, 00, 00, 00),

                },
            };
            ViewBag.Contracts = new List<int>();
            ViewBag.Packages = new List<string>();
            ViewBag.Rooms = new List<int>();
            try
            {
                int campaignid = Convert.ToInt32(campaign_id);
                int hotelid = Convert.ToInt32(hotel_id);
                var model = await _campaignRepository.GetPolicyDetailViewByCampaignID(campaignid, hotelid,from_date,to_date);
                ViewBag.Model = model;
                if (model!=null && model.BasedProgram!=null && model.BasedProgram.Count > 0)
                {
                    ViewBag.Contracts  = model.BasedProgram.Select(x => x.ProgramId).Distinct().ToList();
                    ViewBag.Packages = model.BasedProgram.Select(x => x.PackageCode).Distinct().ToList();
                    ViewBag.Rooms = model.BasedProgram.Select(x => x.RoomId).Distinct().ToList();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("PolicyRule - PricePolicyController: " + ex.ToString());

            }
            return PartialView();
        }

        
        [HttpPost]
        public async Task<IActionResult> SummitHotelPolicy(string model)
        {
            string message = "";
            int status = (int)ResponseType.FAILED;
            try
            {
                HotelPricePolicyCampaignModel summit_model = JsonConvert.DeserializeObject<HotelPricePolicyCampaignModel>(model);
                if (summit_model == null || summit_model.Detail.CampaignCode == null || summit_model.Detail.CampaignCode.Trim() == "" || summit_model.Detail.ClientTypeId < 1 || summit_model.Detail.Status < 0
                    || summit_model.Detail.Status > 1
                    || summit_model.Detail.FromDate == DateTime.MinValue || summit_model.Detail.FromDate == DateTime.MaxValue
                    || summit_model.Detail.ToDate == DateTime.MinValue || summit_model.Detail.ToDate == DateTime.MaxValue
                   )
                {
                    message = "Dữ liệu gửi lên không chính xác, vui lòng kiểm tra lại";
                    return Ok(new
                    {
                        status = status,
                        msg = message
                    });
                }
                int userId = 0;
                if (HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) != null)
                {
                    userId = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                }

                //-- Check campaign date:
                summit_model.Detail.ContractType = (int)ServicesType.VINHotelRent;
                summit_model.Detail.UserCreateId = userId;


                if (summit_model.Detail.Id < 0)
                {
                    var exists_campaign = await _campaignRepository.GetDetailByCode(summit_model.Detail.CampaignCode);
                    if (exists_campaign != null && exists_campaign.Id > 0)
                    {
                        message = "Chiến dịch [" + summit_model.Detail.CampaignCode + "] đã tồn tại, vui lòng thêm mã chiến dịch khác.";
                        return Ok(new
                        {
                            status = status,
                            msg = message
                        });
                    }
                }
               
                status = await _campaignRepository.CreateOrUpdateHotelCampaign(summit_model, userId);
                if (status == (int)ResponseType.SUCCESS)
                {
                    message = "Lưu chính sách giá thành công";
                    var hotel = _hotelRepository.GetHotelById(summit_model.PricePolicy[0].HotelId);
                    await _redisService.DeleteCacheByKeyword(EncodeHelpers.MD5Hash(hotel.HotelId) ,Convert.ToInt32(_configuration["Redis:Database:db_search_result"]));
                }
                else
                {
                    message = "Lưu chính sách giá thất bại";

                }
            }
            catch (Exception ex)
            {
                status = (int)ResponseType.ERROR;
                message = "Lỗi khi xử lý, vui lòng liên hệ bộ phận kỹ thuật";
                LogHelper.InsertLogTelegram("SummitHotelCampaignData - PricePolicyController: " + ex.ToString());
            }
            return Ok(new
            {
                status = status,
                msg = message
            });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateHotelPriceDetail(HotelPricePolicyCampaignModel summit_model)
        {
            string message = "";
            int status = (int)ResponseType.FAILED;
            try
            {
              
                if (summit_model.Detail.Id < 0)
                {
                    var exists_campaign = await _campaignRepository.GetDetailByCode(summit_model.Detail.CampaignCode);
                    if (exists_campaign != null && exists_campaign.Id > 0)
                    {
                        message = "Chiến dịch [" + summit_model.Detail.CampaignCode + "] đã tồn tại, vui lòng thêm mã chiến dịch khác.";
                        return Ok(new
                        {
                            status = status,
                            msg = message
                        });
                    }
                }
                int userId = 0;
                if (HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) != null)
                {
                    userId = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                }

                status = await _campaignRepository.CreateOrUpdateHotelPriceDetail(summit_model, userId);
                if (status == (int)ResponseType.SUCCESS)
                {
                    message = "Lưu chính sách giá thành công";
                    //_redisService.FlushDatabaseByIndex(Convert.ToInt32(_configuration["Redis:Database:db_search_result"]));

                }
                else
                {
                    message = "Lưu chính sách giá thất bại";

                }
            }
            catch (Exception ex)
            {
                status = (int)ResponseType.ERROR;
                message = "Lỗi khi xử lý, vui lòng liên hệ bộ phận kỹ thuật";
                LogHelper.InsertLogTelegram("SummitHotelCampaignData - PricePolicyController: " + ex.ToString());
            }
            return Ok(new
            {
                status = status,
                msg = message
            });
        }
        [HttpPost]
        public async Task<IActionResult> RemovePriceDetail(int id)
        {
            string message = "";
            int status = (int)ResponseType.FAILED;
            try
            {
                if (id <= 0)
                {
                    status = (int)ResponseType.SUCCESS;
                    message = "Xóa chính sách giá thành công.";
                }
                else
                {
                    status = await _priceDetailRepository.RemoveByID(id);
                    if (status == (int)ResponseType.SUCCESS)
                    {
                        message = "Xóa chính sách giá thành công.";

                    }
                    else
                    {
                        message = "Xóa chính sách giá thất bại.";
                    }
                }
            }
            catch (Exception ex)
            {
                status = (int)ResponseType.ERROR;
                message = "Lỗi khi xử lý, vui lòng liên hệ bộ phận kỹ thuật";
                LogHelper.InsertLogTelegram("RemovePriceDetail - PricePolicyController: " + ex.ToString());
            }
            return Ok(new
            {
                status = status,
                message = message
            });
        }


    }

}
