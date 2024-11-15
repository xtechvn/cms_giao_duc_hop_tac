using Entities.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace GDHT.CMS.Controllers.CloseTheBook
{
    public class CloseTheBookController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        public CloseTheBookController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> GetDateByYear(int date)
        {
            try
            {
                var list_date = new List<CloseTheBookViewModel>();
                for (int i = 1; i < 13; i++)
                {
                    var detail = new CloseTheBookViewModel();
                    detail.Month = i;
                    detail.Year = date;
                    var start_date = GetFirstDayOfMonth(i, date);
                    var end_date = GetLastDayOfMonth(i, date);
                    detail.StartDate = start_date.ToString("dd/MM/yyyy");
                    detail.EndDate = end_date.ToString("dd/MM/yyyy");
                    var CheckBookClosing = await _orderRepository.CheckBookClosingByDate(start_date, end_date);
                    if (CheckBookClosing > 0) detail.Status = true;
                    list_date.Add(detail);
                }

                return PartialView(list_date);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetDateByYear - CloseTheBookController: " + ex);
            }

            return PartialView();
        }
        public async Task<IActionResult> OrderBookClosing(OrderBookClosingViewModel model)
        {
            string msgerr = "Đã có lỗi sảy ra vui lòng liên hệ IT";
            try
            {
                long _UserId = 0;
                var date = DateUtil.StringToDate(model.ToDateStr);
                model.ToDate = ((DateTime)date).AddHours(23).AddMinutes(59).AddSeconds(59);
                msgerr = "Khóa sổ tháng " + ((DateTime)date).Month + " từ ngày : " + model.FromDateStr + " đến ngày : " + model.ToDateStr + "không thành công";
                if (HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) != null)
                {
                    _UserId = Convert.ToInt64(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                model.UserFinalize = _UserId;
                var Request = await _orderRepository.OrderBookClosing(model);
                if (Request > 0)
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        message = "Khóa sổ tháng " + ((DateTime)date).Month + " từ ngày : " + model.FromDateStr + " đến ngày : " + model.ToDateStr + " thành công",
                    });
                }
                if (Request == 0)
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        message = "Sổ tháng " + ((DateTime)date).Month + " từ ngày : " + model.FromDateStr + " đến ngày : " + model.ToDateStr + " này đã khóa",
                    });
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("OrderBookClosing - CloseTheBookController: " + ex);
            }
            return Ok(new
            {
                status = (int)ResponseType.ERROR,
                message = msgerr,
            });

        }
        //lấy ngày đầu tháng
        public static DateTime GetFirstDayOfMonth(int iMonth, int Year)
        {
            DateTime dtResult = new DateTime(Year, iMonth, 1);
            dtResult = dtResult.AddDays((-dtResult.Day) + 1);
            return dtResult;
        }
        // ngày cuối tháng
        public static DateTime GetLastDayOfMonth(int iMonth, int Year)
        {
            DateTime dtResult = new DateTime(Year, iMonth, 1);
            dtResult = dtResult.AddMonths(1);
            dtResult = dtResult.AddDays(-(dtResult.Day));
            return dtResult;
        }
    }
}
