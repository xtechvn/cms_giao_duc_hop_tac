using Entities.ViewModels;
using Entities.ViewModels.Report;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;
using WEB.CMS.Customize;

namespace GDHT.CMS.Controllers.Report
{
    [CustomAuthorize]
    public class ReportSupplierController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IDepartmentRepository _DepartmentRepository;
        private readonly IWebHostEnvironment _WebHostEnvironment;
        private readonly IHotelBookingRepositories _hotelBookingRepositories;
        private readonly IHotelBookingRoomExtraPackageRepository _hotelBookingRoomExtraPackageRepository;
        private readonly IReportRepository _reportRepository;
        private ManagementUser _ManagementUser;
        private readonly IAllCodeRepository _allCodeRepository;

        public ReportSupplierController(IOrderRepository orderRepository, IDepartmentRepository departmentRepository,
              IWebHostEnvironment WebHostEnvironment, IHotelBookingRepositories hotelBookingRepositories, IHotelBookingRoomExtraPackageRepository hotelBookingRoomExtraPackageRepository,
               IReportRepository reportRepository, ManagementUser managementUser, IAllCodeRepository allcodeRepository)
        {
            _orderRepository = orderRepository;
            _DepartmentRepository = departmentRepository;
            _WebHostEnvironment = WebHostEnvironment;
            _hotelBookingRepositories = hotelBookingRepositories;
            _hotelBookingRoomExtraPackageRepository = hotelBookingRoomExtraPackageRepository;
            _reportRepository = reportRepository;
            _ManagementUser = managementUser;
            _allCodeRepository = allcodeRepository;

        }
        public IActionResult Index()
        {
            var BRANCH_CODE = _allCodeRepository.GetListByType(AllCodeType.BRANCH_CODE);
            ViewBag.BRANCH_CODE = BRANCH_CODE;
            return View();
        }
        public async Task<IActionResult> SearchReportSupplier(RevenueBySupplierViewModel searchModel)
        {
            try
            {

                var model = new GenericViewModel<SearchRevenueBySupplierViewModel>();
                model = await _DepartmentRepository.GetListTotalDebtRevenueBySupplier(searchModel);
                var model2 = new GenericViewModel<SearchRevenueBySupplierViewModel>();
                var searchModel2 = searchModel;
                searchModel2.PageIndex = -1;
                model2 = await _DepartmentRepository.GetListTotalDebtRevenueBySupplier(searchModel2);
                if (model2 != null && model2.ListData != null && model2.ListData.Count > 0)
                {
                    ViewBag.SumAmountOpeningBalanceDebit = model2.ListData.Sum(s => s.AmountOpeningBalanceDebit).ToString("N0");
                    ViewBag.SumAmountOpeningBalanceCredit = model2.ListData.Sum(s => s.AmountOpeningBalanceCredit).ToString("N0");
                    ViewBag.SumAmountDebit = model2.ListData.Sum(s => s.AmountDebit).ToString("N0");
                    ViewBag.SumAmountCredit = model2.ListData.Sum(s => s.AmountCredit).ToString("N0");
                    ViewBag.SumAmountClosingBalanceDebit = model2.ListData.Sum(s => s.AmountClosingBalanceDebit).ToString("N0");
                    ViewBag.SumAmountClosingBalanceCredit = model2.ListData.Sum(s => s.AmountClosingBalanceCredit).ToString("N0");
                }

                return PartialView(model);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SearchReportSupplier - ReportSupplierController: " + ex);
                return PartialView();
            }
        }
        public async Task<IActionResult> DetailReportSupplier(long id, string Name, string FromDate, string ToDate, long Amount, long Amount2, long Amount3, long Amount4)
        {
            try
            {
                ViewBag.FromDate = FromDate;
                ViewBag.ToDate = ToDate;
                ViewBag.Name = Name;
                ViewBag.id = id;
                ViewBag.AmountOpeningBalanceDebit = Amount;
                ViewBag.AmountDebit = Amount2;
                ViewBag.AmountCredit = Amount3;
                ViewBag.AmountClosingBalanceCredit = Amount4;
                var searchModel = new RevenueBySupplierViewModel();
                searchModel.FromDate = FromDate;
                searchModel.ToDate = ToDate;
                searchModel.SupplierId = id;
                searchModel.PageIndex = 1;
                searchModel.PageSize = 20;
                var model = new GenericViewModel<SearchDetailRevenueBySupplierViewModel>();
                double amount = Amount;
                model = await _DepartmentRepository.GetListDetailRevenueBySupplier(searchModel);
                if (model != null && model.ListData.Count > 0)
                    foreach (var item in model.ListData)
                    {
                        item.AmountOpeningBalance = amount + item.AmountCredit - item.AmountDebit;
                        amount = item.AmountOpeningBalance;
                    }
                ViewBag.model = model;
                return PartialView();
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("DetailReportSupplier - ReportSupplierController: " + ex);
                return PartialView();
            }
        }
        public async Task<IActionResult> ListDetailReportSupplier(RevenueBySupplierViewModel searchModel, long Amount)
        {
            try
            {
                ViewBag.Amount = Amount;
                double amount = Amount;
                var model = new GenericViewModel<SearchDetailRevenueBySupplierViewModel>();
                model = await _DepartmentRepository.GetListDetailRevenueBySupplier(searchModel);
                if (model != null && model.ListData.Count > 0)
                    foreach (var item in model.ListData)
                    {
                        item.AmountOpeningBalance = amount + item.AmountCredit - item.AmountDebit;
                        amount = item.AmountOpeningBalance;
                    }

                ViewBag.model = model;

                return PartialView();
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ListDetailReportSupplier - ReportSupplierController: " + ex);
                return PartialView();
            }
        }
        public async Task<IActionResult> ExportExcel(RevenueBySupplierViewModel searchModel)
        {
            try
            {
                int _UserId = 0;
                if (HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) != null)
                {
                    _UserId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                string _FileName = StringHelpers.GenFileName("Danh sách công nợ NCC", _UserId, "xlsx");
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

                var rsPath = await _DepartmentRepository.ExportDeposit(searchModel, FilePath);

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
                LogHelper.InsertLogTelegram("ExportExcel - ReportSupplierController: " + ex);
                return new JsonResult(new
                {
                    isSuccess = false,
                    message = ex.Message.ToString()
                });
            }
        }
        public async Task<IActionResult> ExportExcelReportSupplier(RevenueBySupplierViewModel searchModel, double amount)
        {
            try
            {
                int _UserId = 0;
                if (HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) != null)
                {
                    _UserId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                string _FileName = StringHelpers.GenFileName("Danh sách CT công nợ NCC", _UserId, "xlsx");
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

                var rsPath = await _DepartmentRepository.ExportDepositSupplier(searchModel, FilePath, amount);

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
                LogHelper.InsertLogTelegram("ExportExcel - ReportSupplierController: " + ex);
                return new JsonResult(new
                {
                    isSuccess = false,
                    message = ex.Message.ToString()
                });
            }
        }
    }
}
