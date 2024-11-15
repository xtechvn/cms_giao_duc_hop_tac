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
    public class ReportClientDebtController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IDepartmentRepository _DepartmentRepository;
        private readonly IWebHostEnvironment _WebHostEnvironment;
        private readonly IHotelBookingRepositories _hotelBookingRepositories;
        private readonly IHotelBookingRoomExtraPackageRepository _hotelBookingRoomExtraPackageRepository;
        private readonly IReportRepository _reportRepository;
        private ManagementUser _ManagementUser;
        private readonly IAllCodeRepository _allCodeRepository;

        public ReportClientDebtController(IOrderRepository orderRepository, IDepartmentRepository departmentRepository,
            IWebHostEnvironment WebHostEnvironment, IHotelBookingRepositories hotelBookingRepositories, IHotelBookingRoomExtraPackageRepository hotelBookingRoomExtraPackageRepository,
             IReportRepository reportRepository, ManagementUser managementUser, IAllCodeRepository allcodeRepository, IClientRepository clientRepository)
        {
            _orderRepository = orderRepository;
            _DepartmentRepository = departmentRepository;
            _WebHostEnvironment = WebHostEnvironment;
            _hotelBookingRepositories = hotelBookingRepositories;
            _hotelBookingRoomExtraPackageRepository = hotelBookingRoomExtraPackageRepository;
            _reportRepository = reportRepository;
            _ManagementUser = managementUser;
            _allCodeRepository = allcodeRepository;
            _clientRepository = clientRepository;

        }
        public async Task<IActionResult> Index()
        {
            var branch = _allCodeRepository.GetListByType(AllCodeType.BRANCH_CODE);
            ViewBag.Branch = branch;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Search(ReportClientDebtSearchModel searchModel)
        {
            try
            {
                int _UserId = 0;
                if (HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) != null)
                {
                    _UserId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                }

                var current_user = _ManagementUser.GetCurrentUser();
                if (current_user != null)
                {
                    if (current_user.Role != "")
                    {
                        var list = current_user.Role.Split(',');
                        bool is_exceed_permission = false;
                        foreach (var item in list)
                        {
                            switch (Convert.ToInt32(item))
                            {
                                case (int)RoleType.Admin:
                                case (int)RoleType.KT:
                                case (int)RoleType.KeToanTruong:
                                case (int)RoleType.PhoTPKeToan:
                                case (int)RoleType.GDHN:
                                case (int)RoleType.GDHPQ:
                                case (int)RoleType.GD:
                                    {

                                    }
                                    break;
                                default:
                                    {
                                        ViewBag.Model = new GenericViewModel<ReportClientDebtViewModel>();
                                        ViewBag.Sum = new SumReportClientDebtViewModel();
                                        return View();
                                    }
                            }
                            if (is_exceed_permission) break;
                        }

                    }

                }


                var model = await _reportRepository.GetTotalDebtRevenueByClient(searchModel);
                ReportClientDebtSearchModel sum_search_model = searchModel;
                sum_search_model.PageIndex = -1;
                sum_search_model.PageSize = 99999;
                var sum_model = await _reportRepository.GetTotalDebtRevenueByClient(sum_search_model);
                ViewBag.Model = model;
                var sum = new SumReportClientDebtViewModel();
                if(sum_model.ListData!=null && sum_model.ListData.Count > 0)
                {
                    sum = new SumReportClientDebtViewModel()
                    {
                        AmountClosingBalanceCredit = sum_model.ListData.Sum(x => x.AmountClosingBalanceCredit == null ? 0 : (double)x.AmountClosingBalanceCredit),
                        AmountClosingBalanceDebit = sum_model.ListData.Sum(x => x.AmountClosingBalanceDebit == null ? 0 : (double)x.AmountClosingBalanceDebit),
                        AmountCredit = sum_model.ListData.Sum(x => x.AmountCredit == null ? 0 : (double)x.AmountCredit),
                        AmountDebit = sum_model.ListData.Sum(x => x.AmountDebit == null ? 0 : (double)x.AmountDebit),
                        AmountOpeningBalanceCredit = sum_model.ListData.Sum(x => x.AmountOpeningBalanceCredit == null ? 0 : (double)x.AmountOpeningBalanceCredit),
                        AmountOpeningBalanceDebit = sum_model.ListData.Sum(x => x.AmountOpeningBalanceDebit == null ? 0 : (double)x.AmountOpeningBalanceDebit)
                    };
                }
                ViewBag.Sum = sum;

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Search - ReportClientDebtController: " + ex);
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ExportExcel(ReportClientDebtSearchModel searchModel)
        {
            try
            {
                int _UserId = 0;
                if (HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) != null)
                {
                    _UserId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                }

                var current_user = _ManagementUser.GetCurrentUser();
                if (current_user != null)
                {
                    if (current_user.Role != "")
                    {
                        var list = current_user.Role.Split(',');
                        bool is_exceed_permission = false;
                        foreach (var item in list)
                        {
                            switch (Convert.ToInt32(item))
                            {
                                case (int)RoleType.Admin:
                                case (int)RoleType.KT:
                                case (int)RoleType.KeToanTruong:
                                case (int)RoleType.PhoTPKeToan:
                                case (int)RoleType.GDHN:
                                case (int)RoleType.GDHPQ:
                                case (int)RoleType.GD:
                                    {

                                    }
                                    break;
                                default:
                                    {
                                        ViewBag.Model = new GenericViewModel<ReportClientDebtViewModel>();
                                        ViewBag.Sum = new SumReportClientDebtViewModel();
                                        return View();
                                    }
                            }
                            if (is_exceed_permission) break;
                        }

                    }

                }

                string folder = @"\Template\Export\";
                string file_name = StringHelpers.GenFileName("Tổng hợp nợ phải thu của Khách hàng",_UserId, "xlsx");
                string _UploadDirectory = Path.Combine(_WebHostEnvironment.WebRootPath, folder);
                string file_path_combine = Path.Combine(_UploadDirectory, file_name);
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
                searchModel.PageIndex = 1;
                searchModel.PageSize = 20000;
                var file_path = await _reportRepository.ExportTotalDebtRevenueByClient(await _reportRepository.GetTotalDebtRevenueByClient(searchModel), file_path_combine);

                return Ok(new
                {
                    status = (int)ResponseType.SUCCESS,
                    msg = "Xuất dữ liệu thành công",
                    path = file_path
                });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Search - OperatorReportController: " + ex);
            }
            return Ok(new
            {
                status = (int)ResponseType.FAILED,
                msg = "Xuất dữ liệu thất bại, vui lòng liên hệ IT",
                path = ""
            });
        }

        [HttpPost]
        public async Task<IActionResult> Detail(ReportDetailClientDebtSearchModel searchModel)
        {
            try
            {
                int _UserId = 0;
                if (HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) != null)
                {
                    _UserId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                }

                var current_user = _ManagementUser.GetCurrentUser();
                if (current_user != null)
                {
                    if (current_user.Role != "")
                    {
                        var list = current_user.Role.Split(',');
                        bool is_exceed_permission = false;
                        foreach (var item in list)
                        {
                            switch (Convert.ToInt32(item))
                            {
                                case (int)RoleType.Admin:
                                case (int)RoleType.KT:
                                case (int)RoleType.KeToanTruong:
                                case (int)RoleType.PhoTPKeToan:
                                case (int)RoleType.GDHN:
                                case (int)RoleType.GDHPQ:
                                case (int)RoleType.GD:
                                    {

                                    }
                                    break;
                                default:
                                    {
                                        ViewBag.SumModel = null;
                                        ViewBag.Model = null;
                                        ViewBag.SearchModel = null;
                                        return View();
                                    }
                            }
                            if (is_exceed_permission) break;
                        }

                    }

                }


                var model = await _reportRepository.GetDetailDebtRevenueByClient(searchModel);
                var sum= await _reportRepository.GetTotalDebtRevenueByClient(new ReportClientDebtSearchModel() { 
                    BranchCode=null,
                    ClientID=searchModel.ClientID,
                    FromDate=searchModel.FromDate,
                    PageIndex=1,
                    PageSize=1,
                    ToDate=searchModel.ToDate
                });
                ReportClientDebtViewModel sum_model = new ReportClientDebtViewModel();
                if (sum!=null && sum.ListData!=null && sum.ListData.Count > 0)
                {
                    sum_model = sum.ListData[0];
                }
                ViewBag.OpeningCredit = searchModel.OpeningCredit;
                ViewBag.SumModel = sum_model;
                ViewBag.Model = model;
                ViewBag.SearchModel = searchModel;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Detail - ReportClientDebtController: " + ex);
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> DetailSearch(ReportDetailClientDebtSearchModel searchModel)
        {
            try
            {
                int _UserId = 0;
                if (HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) != null)
                {
                    _UserId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                }

                var current_user = _ManagementUser.GetCurrentUser();
                if (current_user != null)
                {
                    if (current_user.Role != "")
                    {
                        var list = current_user.Role.Split(',');
                        bool is_exceed_permission = false;
                        foreach (var item in list)
                        {
                            switch (Convert.ToInt32(item))
                            {
                                case (int)RoleType.Admin:
                                case (int)RoleType.KT:
                                case (int)RoleType.KeToanTruong:
                                case (int)RoleType.PhoTPKeToan:
                                case (int)RoleType.GDHN:
                                case (int)RoleType.GDHPQ:
                                case (int)RoleType.GD:
                                    {

                                    }
                                    break;
                                default:
                                    {
                                        ViewBag.SumModel = null;
                                        ViewBag.Model = null;
                                        ViewBag.SearchModel = null;
                                        return View();
                                    }
                            }
                            if (is_exceed_permission) break;
                        }

                    }

                }


                var model = await _reportRepository.GetDetailDebtRevenueByClient(searchModel);
                var sum = await _reportRepository.GetTotalDebtRevenueByClient(new ReportClientDebtSearchModel()
                {
                    BranchCode = null,
                    ClientID = searchModel.ClientID,
                    FromDate = searchModel.FromDate,
                    PageIndex = 1,
                    PageSize = 1,
                    ToDate = searchModel.ToDate
                });
                ReportClientDebtViewModel sum_model = new ReportClientDebtViewModel();
                if (sum != null && sum.ListData != null && sum.ListData.Count > 0)
                {
                    sum_model = sum.ListData[0];
                }
                ViewBag.OpeningCredit = searchModel.OpeningCredit;
                ViewBag.SumModel = sum_model;
                ViewBag.Model = model;
                ViewBag.SearchModel = searchModel;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Detail - ReportClientDebtController: " + ex);
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ExportExcelDetail(ReportDetailClientDebtSearchModel searchModel)
        {
            try
            {
                int _UserId = 0;
                if (HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) != null)
                {
                    _UserId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                }

                var current_user = _ManagementUser.GetCurrentUser();
                if (current_user != null)
                {
                    if (current_user.Role != "")
                    {
                        var list = current_user.Role.Split(',');
                        bool is_exceed_permission = false;
                        foreach (var item in list)
                        {
                            switch (Convert.ToInt32(item))
                            {
                                case (int)RoleType.Admin:
                                case (int)RoleType.KT:
                                case (int)RoleType.KeToanTruong:
                                case (int)RoleType.PhoTPKeToan:
                                case (int)RoleType.GDHN:
                                case (int)RoleType.GDHPQ:
                                case (int)RoleType.GD:
                                    {

                                    }
                                    break;
                                default:
                                    {
                                        ViewBag.Model = new GenericViewModel<ReportClientDebtViewModel>();
                                        ViewBag.Sum = new SumReportClientDebtViewModel();
                                        return View();
                                    }
                            }
                            if (is_exceed_permission) break;
                        }

                    }

                }

                var sum = await _reportRepository.GetTotalDebtRevenueByClient(new ReportClientDebtSearchModel()
                {
                    BranchCode = null,
                    ClientID = searchModel.ClientID,
                    FromDate = searchModel.FromDate,
                    PageIndex = 1,
                    PageSize = 1,
                    ToDate = searchModel.ToDate
                });
                ReportClientDebtViewModel sum_model = new ReportClientDebtViewModel();
                if (sum != null && sum.ListData != null && sum.ListData.Count > 0)
                {
                    sum_model = sum.ListData[0];
                }

                string folder = @"\Template\Export\";
                string file_name = StringHelpers.GenFileName("Chi tiết nợ phải thu của Khách hàng " + sum_model.ClientName, _UserId, "xlsx");

                string _UploadDirectory = Path.Combine(_WebHostEnvironment.WebRootPath, folder);
                string file_path_combine = Path.Combine(_UploadDirectory, file_name);
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
                searchModel.PageIndex = 1;
                searchModel.PageSize = 20000;

                var file_path = await _reportRepository.ExportDetailDebtRevenueByClient(await _reportRepository.GetDetailDebtRevenueByClient(searchModel), file_path_combine,searchModel);

                return Ok(new
                {
                    status = (int)ResponseType.SUCCESS,
                    msg = "Xuất dữ liệu thành công",
                    path = file_path
                });

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ExportExcelDetail - ReportClientDebtController: " + ex);
            }
            return Ok(new
            {
                status = (int)ResponseType.FAILED,
                msg = "Xuất dữ liệu thất bại, vui lòng liên hệ IT",
                path = ""
            });
        }

    }
}
