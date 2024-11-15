using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Entities.ViewModels;
using Entities.ViewModels.Report;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Repositories.IRepositories;
using Utilities;
using WEB.CMS.Customize;

namespace GDHT.CMS.Controllers.Report
{
    [CustomAuthorize]
    public class ReportEmployeeRevenueController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IWebHostEnvironment _WebHostEnvironment;

        public ReportEmployeeRevenueController(IOrderRepository orderRepository, IWebHostEnvironment WebHostEnvironment)
        {
            _orderRepository = orderRepository;
            _WebHostEnvironment = WebHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        public async Task<IActionResult> SearchReportDepartment(ReportDepartmentViewModel searchModel)
        {
            var model = new GenericViewModel<SearchReportDepartmentViewModel>();
            //model = await _DepartmentRepository.GetReportDepartment(searchModel);
            return PartialView(model);
        }
        [HttpPost]
        public async Task<IActionResult> ExportExcel(ReportDepartmentViewModel searchModel)
        {
            try
            {
                int _UserId = 0;
                if (HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) != null)
                {
                    _UserId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                string _FileName = StringHelpers.GenFileName("Doanh thu tổng theo nhân viên bán hàng", _UserId, "xlsx");

                string _UploadFolder = @"Template\Export";
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

                string FilePath = Path.Combine(_UploadDirectory, _FileName);

                //var rsPath = await _DepartmentRepository.ExportDeposit(searchModel, FilePath);
                var rsPath = string.Empty;

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
                LogHelper.InsertLogTelegram("ExportExcel - ReportDepartmentController: " + ex);
                return new JsonResult(new
                {
                    isSuccess = false,
                    message = ex.Message.ToString()
                });
            }
        }
    }
}
