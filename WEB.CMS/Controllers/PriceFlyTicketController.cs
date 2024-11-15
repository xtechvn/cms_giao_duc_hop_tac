using Entities.Models;
using Entities.ViewModels;
using Entities.ViewModels.PricePolicy;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
        public IActionResult FlyTicketPolicy()
        {
            return PartialView();
        }
        public IActionResult FlyTicketPolicyUpdate(string campaignCode)
        {
            try
            {
                Campaign campaign = _campaignRepository.GetDetailByCode(campaignCode).Result;
                ProductFlyTicketViewModel model = new ProductFlyTicketViewModel();
                if (campaign != null)
                {
                    ProductFlyTicketService productFlyTicket = _productFlyTicketServiceRepository.GetByCampaignID(campaign.Id);
                    PriceDetail priceDetail = _priceDetailRepository.FindByProductServiceId(productFlyTicket.Id);
                    if (priceDetail != null)
                    {
                        if (priceDetail.MonthList != null && priceDetail.MonthList != "")
                        {
                            var lstMonth = priceDetail.MonthList.Split(',').Select(n => { return "Tháng " + n; }).ToList();
                            model.MonthList = lstMonth;
                        }
                        if (priceDetail.DayList != null && priceDetail.DayList != "")
                        {
                            var lstDay = priceDetail.DayList.Split(',');
                            model.DayList = new List<string>();
                            foreach (var item in lstDay)
                            {
                                switch (int.Parse(item))
                                {
                                    case (int)DayOfWeek.Monday:
                                        model.DayList.Add("Thứ Hai");
                                        break;
                                    case (int)DayOfWeek.Tuesday:
                                        model.DayList.Add("Thứ Ba");
                                        break;
                                    case (int)DayOfWeek.Wednesday:
                                        model.DayList.Add("Thứ Tư");
                                        break;
                                    case (int)DayOfWeek.Thursday:
                                        model.DayList.Add("Thứ Năm");
                                        break;
                                    case (int)DayOfWeek.Friday:
                                        model.DayList.Add("Thứ Sáu");
                                        break;
                                    case (int)DayOfWeek.Saturday:
                                        model.DayList.Add("Thứ Bảy");
                                        break;
                                    case (int)DayOfWeek.Sunday:
                                        model.DayList.Add("Chủ Nhật");
                                        break;
                                }
                            }
                        }
                    }
                    model.Campaign = campaign;
                    model.PriceDetail = priceDetail;
                }
                return PartialView(model);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("FlyTicketPolicyUpdate - PricePolicyController: " + ex.ToString());
                throw;
            }
        }
        /// <summary>
        /// Lưu db tất cả thông tin
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddTicketFly(string data)
        {
            string msg = "Failed";
            try
            {
                var pricePolicyModel = JsonConvert.DeserializeObject<PricePolicySummitModel>(data);
                var fromDateArray = pricePolicyModel.FromDateStr.Split(" ");
                var dateFrom = fromDateArray[0].Split("/");
                var timeFrom = fromDateArray[1].Split(":");
                var toDateArry = pricePolicyModel.ToDateStr.Split(" ");
                var dateTo = toDateArry[0].Split("/");
                var timeTo = toDateArry[1].Split(":");
                pricePolicyModel.FromDate = new DateTime(int.Parse(dateFrom[2]), int.Parse(dateFrom[1]), int.Parse(dateFrom[0]),
                   int.Parse(timeFrom[0]), int.Parse(timeFrom[1]), 00);
                pricePolicyModel.ToDate = new DateTime(int.Parse(dateTo[2]), int.Parse(dateTo[1]), int.Parse(dateTo[0]),
                   int.Parse(timeTo[0]), int.Parse(timeTo[1]), 00);
                var validate = ValidatePriceFlyTicket(pricePolicyModel, out msg);
                if (validate == (int)ResponseType.FAILED)
                {
                    return Ok(new
                    {
                        isSuccess = false,
                        message = msg
                    });
                }
                int userId = 0;
                if (HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) != null)
                {
                    userId = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                var result = _productFlyTicketServiceRepository.AddCampaginAndProduct(pricePolicyModel, userId);
                if (result != -1)
                {
                    msg = pricePolicyModel.Mode == 0 ? "Thêm mới chính sách giá vé máy bay thành công"
                        : "Cập nhật chính sách giá vé máy bay thành công";
                    string cache_name = CacheName.B2C_FLY_PRICE;
                    _redisService.DeleteCacheByKeyword(cache_name, Convert.ToInt32(_configuration["Redis:Database:db_search_result"]));
                }
                else
                {
                    msg = pricePolicyModel.Mode == 0 ? "Thêm mới chính sách giá vé máy bay thất bại"
                        : "Cập nhật chính sách giá vé máy bay thất bại";
                }
                return Ok(new
                {
                    isSuccess = result != -1,
                    message = msg
                });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("AddTicketFly - PricePolicyController: " + ex);
                return Ok(new
                {
                    isSuccess = false,
                    message = "Thêm mới chính sách giá vé máy bay thất bại"
                });
            }
        }
        public int ValidatePriceFlyTicket(PricePolicySummitModel pricePolicyModel, out string message, bool isEdit = false)
        {
            message = string.Empty;
            try
            {
              
                var status = (int)ResponseType.SUCCESS;
                if (string.IsNullOrEmpty(pricePolicyModel.CampaignCode))
                {
                    message = "Mã chiến dịch không được để trống";
                    status = (int)ResponseType.FAILED;
                }
                //check format campaign code
                var campaign = _campaignRepository.GetDetailByCode(pricePolicyModel.CampaignCode).Result;
                if (pricePolicyModel.Mode == 0)//update
                {
                    var regex = new Regex(@"^[a-zA-Z0-9-_\+]*$");
                    if (regex.IsMatch(pricePolicyModel.CampaignCode) == false)
                    {
                        message = "Mã chiến dịch chỉ chứa kí tự chữ, số, -, _";
                        status = (int)ResponseType.FAILED;
                    }
                    if (campaign != null)
                    {
                        message = "Mã chiến dịch đã tồn tại";
                        status = (int)ResponseType.FAILED;
                    }
                }
                if (string.IsNullOrEmpty(pricePolicyModel.Description))
                {
                    message = "Mô tả không được để trống";
                    status = (int)ResponseType.FAILED;
                }
                if (pricePolicyModel.FromDate == null)
                {
                    message = "Ngày bắt đầu không được để trống";
                    status = (int)ResponseType.FAILED;
                }
                if (pricePolicyModel.ToDate == null)
                {
                    message = "Ngày kết thúc không được để trống";
                    status = (int)ResponseType.FAILED;
                }
                if (pricePolicyModel.ToDate < pricePolicyModel.FromDate)
                {
                    message = "Ngày bắt đầu phải nhỏ hơn ngày kết thúc";
                    status = (int)ResponseType.FAILED;
                }

                if (pricePolicyModel.ServiceFee < 0)
                {
                    message = "Phí dịch vụ không được để trống";
                    status = (int)ResponseType.FAILED;
                }
                return status;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ValidatePriceFlyTicket - PricePolicyController: " + ex);
                return -1;
            }

        }
    }
}
