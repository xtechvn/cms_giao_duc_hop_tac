using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Entities.ViewModels;
using Entities.ViewModels.Funding;
using Entities.ViewModels.Report;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Repositories.IRepositories;
using Repositories.Repositories;
using Utilities;

namespace GDHT.CMS.Controllers.Report
{
    public class ReportHotelRevenueController : Controller
    {
        private readonly IHotelBookingRepositories _hotelBookingRepositories;
        private readonly IWebHostEnvironment _WebHostEnvironment;

        public ReportHotelRevenueController(IHotelBookingRepositories hotelBookingRepositories, IWebHostEnvironment hostEnvironment)
        {
            _hotelBookingRepositories = hotelBookingRepositories;
            _WebHostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Search(ReportClientDebtSearchModel searchModel, int currentPage = 1, int pageSize = 10)
        {
            var model = new GenericViewModel<ReportHotelRevenueViewModel>();
            try
            {
                searchModel.PageSize = pageSize;
                searchModel.PageIndex = currentPage;
                if (searchModel.HotelIds == null) searchModel.HotelIds = new List<string>();
                var data = _hotelBookingRepositories.GetHotelBookingRevenue(searchModel, out long total);
                model.CurrentPage = currentPage;
                model.ListData = data;
                model.PageSize = pageSize;
                model.TotalRecord = total;
                model.TotalPage = (int)Math.Ceiling((double)total / pageSize);
                return PartialView(model);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Search - ReportHotelRevenueController: " + ex);
                return PartialView();
            }
        }

        [HttpPost]
        public IActionResult ExportExcel(ReportClientDebtSearchModel searchModel)
        {
            try
            {
                string _FileName = "HotelRevenue_" + Guid.NewGuid() + ".xlsx";
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
                if (searchModel.HotelIds == null) searchModel.HotelIds = new List<string>();
                string FilePath = Path.Combine(_UploadDirectory, _FileName);
                searchModel.PageIndex = -1;
                searchModel.PageSize = -1;
                var rsPath = _hotelBookingRepositories.ExportHotelRevenue(searchModel, FilePath);

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
                    message = ex.Message.ToString() + ". Đã có lỗi xảy ra"
                });
            }
        }

    }
}
