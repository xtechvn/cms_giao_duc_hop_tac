using Entities.Models;
using Entities.ViewModels;
using Entities.ViewModels.Hotel;
using Entities.ViewModels.Programs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;
using GDHT.CMS.Service;
using WEB.CMS.Customize;
using WEB.CMS.Models;
using static Entities.ViewModels.Programs.InsertProgramsPackageViewModel;

namespace GDHT.CMS.Controllers.Programs
{
    [CustomAuthorize]
    public class ProgramsPackageController : Controller
    {
        private IProgramsPackageReprository _programsPackageReprository;
        private IProgramsReprository _programsReprository;
        private IUserRepository _userRepository;
        private ManagementUser _ManagementUser;
        private IHotelRepository _hotelRepository;
        private readonly IAllCodeRepository _allCodeRepository;
        public ProgramsPackageController(IProgramsPackageReprository programsPackageReprository, IProgramsReprository programsReprository, IUserRepository userRepository, ManagementUser ManagementUser, IAllCodeRepository allcodeRepository, IHotelRepository hotelRepository)
        {
            _programsPackageReprository = programsPackageReprository;
            _programsReprository = programsReprository;
            _userRepository = userRepository;
            _ManagementUser = ManagementUser;
            _allCodeRepository = allcodeRepository;
            _hotelRepository = hotelRepository;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> DetailProgramsPackage(int id, long type, string RoomType, string ProgramName)
        {
            try
            {
                var dataProgram = await _programsReprository.DetailPrograms(id);
                ViewBag.SupplierId = dataProgram.SupplierId;
                ViewBag.FromDate2 = Convert.ToDateTime(dataProgram.StayStartDate).ToString("dd/MM/yyyy");
                ViewBag.ToDate2 = Convert.ToDateTime(dataProgram.StayEndDate).ToString("dd/MM/yyyy");
                ViewBag.FromDateDB2 = Convert.ToDateTime(dataProgram.StayStartDate).ToString("yyyy-MM-dd");
                ViewBag.ToDateDB2 = Convert.ToDateTime(dataProgram.StayEndDate).ToString("yyyy-MM-dd");
                ViewBag.ProgramName = ProgramName;
                if (type != 0)
                {
                    var model = new ProgramsPackageSearchViewModel();
                    model.ProgramId = id.ToString();
                    model.PageIndex = 1;
                    model.PageSize = 10;
                    var data = await _programsPackageReprository.ListProgramsPackageDaily(model);
                    ViewBag.id = data.ListData[0].Id;
                    var DetailPrograms = await _programsReprository.DetailPrograms(id);
                    ViewBag.FromDate = Convert.ToDateTime(DetailPrograms.StayStartDate).ToString("dd/MM/yyyy");
                    ViewBag.ToDate = Convert.ToDateTime(DetailPrograms.StayEndDate).ToString("dd/MM/yyyy");
                    ViewBag.FromDatedb = Convert.ToDateTime(DetailPrograms.StayStartDate).ToString("yyyy-MM-dd");
                    ViewBag.ToDatedb = Convert.ToDateTime(DetailPrograms.StayEndDate).ToString("yyyy-MM-dd");
                    ViewBag.RoomType = RoomType;


                    if (type == 2)
                    {
                        var data2 = data.ListData.Where(s => s.ProgramName == ProgramName).ToList();
                        ViewBag.type2 = 1;
                        ViewBag.data = null;
                        ViewBag.PackageCode = data2[0].ProgramName;
                        ViewBag.btnthem = "Thêm hạng phòng";
                    }
                    else
                    {
                        ViewBag.type2 = 2;
                        ViewBag.type = type;
                        var ListProgramsPackageDaily = data.ListData.Where(s => s.ProgramName == ProgramName).ToList();
                        ViewBag.data = ListProgramsPackageDaily;
                        var listdata = ListProgramsPackageDaily[0].ProgramsPackage.Where(s => s.RoomType == RoomType).ToList();
                        var applydate = ListProgramsPackageDaily[0].ProgramsPackage.Where(s => s.RoomType == RoomType).GroupBy(s => new { s.FromDate }).Select(s => s.First()).ToList();
                        ViewBag.FromDate = Convert.ToDateTime(applydate[0].FromDate).ToString("dd/MM/yyyy");
                        ViewBag.ToDate = Convert.ToDateTime(applydate[0].ToDate).ToString("dd/MM/yyyy");
                        ViewBag.applydate = applydate;


                    }

                    return PartialView();
                }
                else
                {

                    ViewBag.FromDate = Convert.ToDateTime(dataProgram.StayStartDate).ToString("dd/MM/yyyy");
                    ViewBag.ToDate = Convert.ToDateTime(dataProgram.StayEndDate).ToString("dd/MM/yyyy");
                    ViewBag.FromDatedb = Convert.ToDateTime(dataProgram.StayStartDate).ToString("yyyy-MM-dd");
                    ViewBag.ToDatedb = Convert.ToDateTime(dataProgram.StayEndDate).ToString("yyyy-MM-dd");
                    ViewBag.id = id;

                    ViewBag.ServiceName = dataProgram.ServiceName;
                    ViewBag.type2 = 0;
                    ViewBag.btnthem = "Thêm gói";
                    return PartialView();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("DetailProgramsPackage - ProgramsPackageController: " + ex);
                return PartialView();
            }

        }

        public async Task<IActionResult> DetailProgramsPackage2(int id, long type, string RoomType, string ProgramName)
        {
            try
            {
                var current_user = _ManagementUser.GetCurrentUser();
                if (current_user != null)
                {
                    var list = Array.ConvertAll(current_user.Role.Split(','), int.Parse);
                    foreach (var item in list)
                    {
                        var listPermissions2 = await _userRepository.CheckRolePermissionByUserAndRole(current_user.Id, item, (int)SortOrder.SUA, ReadFile.LoadConfig().Programs_Menuid);

                        if (listPermissions2 == true)
                        {
                            ViewBag.btnSua = 1;
                        }
                    }

                }
                if (type != 0)
                {

                    var model = new ProgramsPackageSearchViewModel();
                    model.ProgramId = id.ToString();
                    model.PageIndex = 1;
                    model.PageSize = 10;
                    var data = await _programsPackageReprository.ListProgramsPackageDaily(model);
                    ViewBag.id = data.ListData.Count > 0 ? data.ListData[0].Id : 0;


                    if (type == 2)
                    {
                        ViewBag.type2 = 1;
                        ViewBag.data = null;
                        ViewBag.PackageCode = data.ListData[0].PackageCode;
                    }
                    else
                    {
                        var data_list = data.ListData.Where(s => s.ProgramName == ProgramName).ToList();
                        ViewBag.type2 = 2;
                        ViewBag.RoomType = RoomType;
                        ViewBag.data = data.ListData.Where(s => s.ProgramName == ProgramName).ToList();
                        ViewBag.mindate = data_list[0].ProgramsPackage.Where(s => s.RoomType == RoomType).Min(s => Convert.ToDateTime(s.FromDate).ToString("dd/MM/yyyy"));
                        ViewBag.maxdate = data_list[0].ProgramsPackage.Where(s => s.RoomType == RoomType).Max(s => Convert.ToDateTime(s.ToDate).ToString("dd/MM/yyyy"));

                        ViewBag.ProgramName = ProgramName;
                    }

                    return PartialView();
                }
                else
                {
                    ViewBag.id = id;
                    ViewBag.SupplierId = 0;
                    ViewBag.type2 = 0;
                    return PartialView();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("DetailProgramsPackage - ProgramsPackageController: " + ex);
                return PartialView();
            }

        }
        public async Task<IActionResult> DetailListProgramsPackage(long id, long Packageid, string ProgramName, long RoomTypeid = 0)
        {
            try
            {

                var data = await _programsReprository.DetailPrograms(id);
                ViewBag.ServiceName = data.ServiceName;
                ViewBag.HotelId = data.HotelId;
                ViewBag.ProgramName = ProgramName;
                ViewBag.RoomTypeid = RoomTypeid;
                if (data.Status != (int)ProgramsStatus.Cho_TBP_duyet && data.Status != (int)ProgramsStatus.Da_duyet)
                {
                    ViewBag.btnThem = 1;
                    ViewBag.Status = 1;
                }
                var current_user = _ManagementUser.GetCurrentUser();
                if (current_user != null)
                {
                    var list = Array.ConvertAll(current_user.Role.Split(','), int.Parse);
                    foreach (var item in list)
                    {
                        var listPermissions2 = await _userRepository.CheckRolePermissionByUserAndRole(current_user.Id, item, (int)SortOrder.SUA, ReadFile.LoadConfig().Programs_Menuid);

                        if (listPermissions2 == true)
                        {
                            ViewBag.btnSua = 1;
                        }
                    }

                }

                ViewBag.id = id;
                ViewBag.Packageid = Packageid;
                if (Packageid != 0)
                {
                    var searchModel = new ProgramsPackageSearchViewModel();
                    searchModel.ProgramId = id.ToString();
                    searchModel.PageIndex = 1;
                    searchModel.PageSize = 10;
                    var model = new GenericViewModel<ProgramsPackagePriceViewModel>();

                    model = await _programsPackageReprository.GetListProgramPackageDaily(searchModel);

                    var dataProgramPackage = await _programsPackageReprository.ListdataProgramPackage(searchModel);
                    ViewBag.data = dataProgramPackage;
                    ViewBag.ProgramPackageDaily = model != null ? model.ListData.Where(s => s.ProgramName == ProgramName).ToList() : null;
                    if (RoomTypeid > 0)
                    {
                        ViewBag.data = dataProgramPackage != null && dataProgramPackage.Count > 0 ? dataProgramPackage.Where(s => s.RoomTypeId == RoomTypeid.ToString()).ToList() : null;
                        ViewBag.ProgramPackageDaily = model != null ? model.ListData.Where(s => s.ProgramName == ProgramName && s.RoomTypeId == RoomTypeid.ToString()).ToList() : null;
                    }

                }

                ViewBag.id = id;
                return View();

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("DetailListProgramsPackage - ProgramsPackageController: " + ex);
                return View();
            }

        }
        public async Task<IActionResult> ListProgramsPackagePrice(ProgramsPackageSearchViewModel searchModel)
        {
            try
            {
                var data = await _programsReprository.DetailPrograms(Convert.ToInt32(searchModel.ProgramId));

                if (data.Status != (int)ProgramsStatus.Cho_TBP_duyet && data.Status != (int)ProgramsStatus.Da_duyet)
                {
                    ViewBag.btnThem = 1;
                    ViewBag.Status = 1;
                }
                var current_user = _ManagementUser.GetCurrentUser();
                if (current_user != null)
                {
                    var list = Array.ConvertAll(current_user.Role.Split(','), int.Parse);
                    foreach (var item in list)
                    {
                        var listPermissions2 = await _userRepository.CheckRolePermissionByUserAndRole(current_user.Id, item, (int)SortOrder.SUA, ReadFile.LoadConfig().Programs_Menuid);

                        if (listPermissions2 == true)
                        {
                            ViewBag.btnSua = 1;
                        }
                    }

                }
                var model = new GenericViewModel<ProgramsPackagePriceViewModel>();

                model = await _programsPackageReprository.GetListProgramPackageDaily(searchModel);
                ViewBag.id = searchModel.ProgramId;
                var dataProgramPackage = await _programsPackageReprository.ListdataProgramPackage(searchModel);
                ViewBag.data = dataProgramPackage;
                ViewBag.ProgramPackageDaily = model != null ? model.ListData.Where(s => s.ProgramName == searchModel.ProgramName).ToList() : null;

                return PartialView();

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ListProgramsPrice - ProgramsPackageController: " + ex);
                return PartialView();
            }

        }
        public async Task<IActionResult> ListProgramsPackage(ProgramsPackageSearchViewModel searchModel)
        {
            try
            {
                var data = await _programsReprository.DetailPrograms(Convert.ToInt32(searchModel.ProgramId));
                if (data.Status != (int)ProgramsStatus.Da_duyet)
                {
                    ViewBag.edit = 1;
                }
                if (data.Status != (int)ProgramsStatus.Da_duyet && data.Status != (int)ProgramsStatus.Cho_TBP_duyet)
                {
                    ViewBag.xoa = 1;
                }
                var current_user = _ManagementUser.GetCurrentUser();
                if (current_user != null)
                {
                    var list = Array.ConvertAll(current_user.Role.Split(','), int.Parse);
                    foreach (var item in list)
                    {
                        var listPermissions2 = await _userRepository.CheckRolePermissionByUserAndRole(current_user.Id, item, (int)SortOrder.SUA, ReadFile.LoadConfig().Programs_Menuid);

                        if (listPermissions2 == true)
                        {
                            ViewBag.btnSua = 1;
                        }
                    }

                }
                var model = new GenericViewModel<ProgramsPackageModel>();
                model = await _programsPackageReprository.ListProgramsPackageDaily(searchModel);

                return PartialView(model);

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ListProgramsPackage - ProgramsPackageController: " + ex);
                return PartialView();
            }

        }
        public async Task<IActionResult> ListProgramsPrice(ProgramsPackageSearchViewModel searchModel)
        {
            try
            {
                var model = new GenericViewModel<ProgramsPackagePriceViewModel>();
                model = await _programsPackageReprository.GetListProgramPackageDaily(searchModel);

                //var data = await _programsPackageReprository.ListdataProgramPackage(searchModel);
                //ViewBag.data = data;
                if (searchModel.RoomType != null && searchModel.RoomType != "")
                {
                    model.ListData = model.ListData.Where(s => s.RoomType == searchModel.RoomType && s.ProgramName == searchModel.PackageCode).ToList();
                }
                return PartialView(model);

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ListProgramsPrice - ProgramsPackageController: " + ex);
                return PartialView();
            }

        }

        public async Task<IActionResult> SummitProgramsPackage(List<InsertProgramsPackageViewModel> data, List<InsertProgramsPackageViewModel> datadate, long type, long type2)
        {
            try
            {

                var Userid = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                if (data != null && type != 0)
                {
                    var detailPrograms = await _programsReprository.DetailPrograms(data[0].ProgramId);
                    if (Convert.ToDateTime(detailPrograms.StayStartDate) > Convert.ToDateTime(data[0].FromDate) || Convert.ToDateTime(detailPrograms.StayEndDate) < Convert.ToDateTime(data[0].ToDate))
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.ERROR,
                            msg = "Thời gian áp dụng của hạng phòng nằm ngoài thời gian áp dụng của chương trình"
                        });
                    }
                    if (datadate != null)
                    {
                        foreach (var item in datadate)
                        {

                            if (DateUtil.StringToDate(detailPrograms.StayStartDate) > DateUtil.StringToDate(item.ApplyDateStr) || DateUtil.StringToDate(detailPrograms.StayEndDate) < DateUtil.StringToDate(item.ApplyDateStr))
                            {
                                return Ok(new
                                {
                                    status = (int)ResponseType.ERROR,
                                    msg = "Thời gian áp dụng theo Ngày đặc biệt của hạng phòng nằm ngoài thời gian áp dụng của chương trình"
                                });
                            }
                        }
                    }
                    if (datadate == null || datadate.Count == 0)
                    {
                        datadate = null;
                    }
                    var Update = await _programsPackageReprository.UpdateProgramPackageDaily(data, datadate, Userid, type2, type);
                    if (Update > 0)
                    {
                        if (type == 3)
                        {
                            return Ok(new
                            {
                                status = (int)ResponseType.SUCCESS,
                                msg = "Thêm mới thành công",
                                id = Update
                            });
                        }
                        else
                        {
                            return Ok(new
                            {
                                status = (int)ResponseType.SUCCESS,
                                msg = "Cập nhật thành công",
                                id = Update
                            });

                        }

                    }
                    else
                    {
                        if (type == 3)
                        {
                            return Ok(new
                            {
                                status = (int)ResponseType.ERROR,
                                msg = "Thời gian áp dụng của hạng phòng đã có vui lòng thử khoảng thời gian khác"
                            });
                        }
                        else
                        {
                            return Ok(new
                            {
                                status = (int)ResponseType.ERROR,
                                msg = "Cập nhật không thành công"
                            });
                        }



                    }
                }
                else
                {
                    var detailPrograms = await _programsReprository.DetailPrograms(data[0].ProgramId);
                    if (Convert.ToDateTime(detailPrograms.StayStartDate) > Convert.ToDateTime(data[0].FromDate) || Convert.ToDateTime(detailPrograms.StayEndDate) < Convert.ToDateTime(data[0].ToDate))
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.ERROR,
                            msg = "thời gian áp dụng của hạng phòng nằm ngoài thời gian áp dụng của chương trình"
                        });
                    }

                    var Insert = await _programsPackageReprository.InsertProgramPackageDaily(data, datadate, Userid, type2);
                    if (Insert > 0)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.SUCCESS,
                            msg = "Thêm mới thành công",
                            id = Insert
                        });
                    }
                    if (Insert == -3)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.ERROR,
                            msg = "Thời gian áp dụng của hạng phòng không có thứ " + data[0].WeekDay.Replace("0", "CN")
                        }); ;
                    }
                    if (Insert == -1)
                    {
                        if (type2 == 1)
                        {
                            return Ok(new
                            {
                                status = (int)ResponseType.ERROR,
                                msg = "Thời gian áp dụng của hạng phòng đã có vui lòng thử khoảng thời gian khác"
                            });
                        }
                        return Ok(new
                        {
                            status = (int)ResponseType.ERROR,
                            msg = "Mã gói đã tồn tại"
                        });
                    }
                    else
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.ERROR,
                            msg = "Thêm mới không thành công"
                        });
                    }
                }

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SummitProgramsPackage - ProgramsPackageController: " + ex);
                return Ok(new
                {
                    status = (int)ResponseType.ERROR,
                    msg = "Thêm mới không thành công vui lòng liên hệ bộ phận IT"
                });
            }



        }
        public async Task<IActionResult> WeekDaySuggestion(string txt_search)
        {

            try
            {
                txt_search = txt_search != null ? txt_search.TrimEnd(',', ' ') : null;

                var WeekDay = new List<WeekDayModel>();
                if (txt_search != null && txt_search != "")
                {

                    var data = txt_search.Split(',');

                    for (int i = 0; i < 8; i++)
                    {
                        var WeekDaymodel = new WeekDayModel();
                        int dem = 0;
                        foreach (var item in data)
                        {
                            if (i != 1)
                            {

                                if (item != null && item != "" && i == Convert.ToInt32(item))
                                {

                                    dem++;
                                }


                            }
                        }
                        if (dem == 0)
                        {
                            if (i != 1)
                            {
                                if (i == 0)
                                {
                                    WeekDaymodel.id = i.ToString();
                                    WeekDaymodel.name = "CN";
                                }
                                else
                                {
                                    WeekDaymodel.id = i.ToString();
                                    WeekDaymodel.name = "Thứ " + i;
                                }
                                WeekDay.Add(WeekDaymodel);
                            }
                        }


                    }
                }
                else
                {

                    for (int i = 0; i < 8; i++)
                    {
                        var WeekDaymodel = new WeekDayModel();

                        if (i != 1)
                        {
                            if (i == 0)
                            {
                                WeekDaymodel.id = i.ToString();
                                WeekDaymodel.name = "CN";
                            }
                            else
                            {
                                WeekDaymodel.id = i.ToString();
                                WeekDaymodel.name = "Thứ " + i;
                            }
                            WeekDay.Add(WeekDaymodel);
                        }
                    }

                }


                return Ok(new
                {
                    status = (int)ResponseType.SUCCESS,
                    data = WeekDay
                });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("WeekDaySuggestion - ProgramsController: " + ex.ToString());
                return Ok(new
                {
                    status = (int)ResponseType.SUCCESS,
                    data = new List<WeekDayModel>()
                });
            }

        }
        public async Task<IActionResult> WeekDaySuggestion2(string txt_search)
        {

            try
            {
                txt_search = txt_search != null ? txt_search.TrimEnd(',', ' ') : null;
                var WEEKDAY_TYPE = _allCodeRepository.GetListByType(AllCodeType.WEEKDAY_TYPE);
                var WeekDay = new List<WeekDayModel>();
                if (txt_search != null && txt_search != "" && txt_search != "1")
                {
                    int dem = 0;
                    foreach (var item in WEEKDAY_TYPE)
                    {
                        if (txt_search.Contains("2") || txt_search.Contains("3"))
                            txt_search += "2,3,6";
                        if (txt_search.Contains("4") || txt_search.Contains("5"))
                            txt_search += "4,5,6";
                        if (txt_search.Equals("6"))
                        {
                            return Ok(new
                            {
                                status = (int)ResponseType.SUCCESS,
                                data = new List<WeekDayModel>()
                            });
                        }

                        if (txt_search.Contains(item.CodeValue.ToString()) == false)
                        {

                            var WeekDaymodel = new WeekDayModel();
                            WeekDaymodel.id = item.CodeValue.ToString();
                            WeekDaymodel.name = item.Description;
                            WeekDay.Add(WeekDaymodel);
                        }
                    }
                }
                else
                {
                    foreach (var item in WEEKDAY_TYPE)
                    {
                        var WeekDaymodel = new WeekDayModel();
                        WeekDaymodel.id = item.CodeValue.ToString();
                        WeekDaymodel.name = item.Description;
                        WeekDay.Add(WeekDaymodel);
                    }
                }
                return Ok(new
                {
                    status = (int)ResponseType.SUCCESS,
                    data = WeekDay
                });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("WeekDaySuggestion - ProgramsController: " + ex.ToString());
                return Ok(new
                {
                    status = (int)ResponseType.SUCCESS,
                    data = new List<WeekDayModel>()
                });
            }

        }
        public async Task<IActionResult> DeleteProgramsPackage(long id, string packagecode, string roomtype, long amount, string date, long type, string WeekDay, string date2)
        {
            try
            {

                var Userid = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var delete = 0;
                DateTime? FromDate = DateUtil.StringToDate(date);
                DateTime? ApplyDate = null;
                if (date2 != null)
                {
                    ApplyDate = DateUtil.StringToDate(date2);
                }

                if (type == 1)
                {
                    var Searchmodel = new ProgramsPackageSearchViewModel();
                    Searchmodel.ProgramId = id.ToString();
                    Searchmodel.ProgramName = packagecode;
                    Searchmodel.PageIndex = -1;
                    Searchmodel.PageSize = 10;
                    var check = await _programsPackageReprository.CheckProgramsPackageDaily(Searchmodel, packagecode, roomtype);
                    if (check == 0)
                    {
                        delete = await _programsPackageReprository.DeleteProgramPackagesbyProgramId(id, packagecode, roomtype, amount, null, ApplyDate);
                    }
                    else
                    {
                        Searchmodel.FromDate = date;
                        Searchmodel.RoomType = roomtype;
                        var checkProgramsPackage = await _programsPackageReprository.CheckProgramsPackage(Searchmodel, packagecode, roomtype);
                        if (checkProgramsPackage > 0 && check==1)
                        {
                            return Ok(new
                            {
                                status = (int)ResponseType.ERROR,
                                msg = "Xóa không thành công,vui lòng xóa ngày đặc biệt trước"
                            });
                        }
                        else
                        {


                            if (WeekDay != null)
                            {
                                var listWeekDay = WeekDay.Split(',');
                                foreach (var item in listWeekDay)
                                {

                                    switch (item.TrimEnd(' ').TrimStart(' '))
                                    {
                                        case "Thứ 2":
                                            {
                                                delete = await _programsPackageReprository.DeleteProgramPackagesDailyByProgramId(id, packagecode, roomtype, amount, FromDate, 2);
                                            }
                                            break;
                                        case "Thứ 3":
                                            {
                                                delete = await _programsPackageReprository.DeleteProgramPackagesDailyByProgramId(id, packagecode, roomtype, amount, FromDate, 3);
                                            }
                                            break;
                                        case "Thứ 4":
                                            {
                                                delete = await _programsPackageReprository.DeleteProgramPackagesDailyByProgramId(id, packagecode, roomtype, amount, FromDate, 4);
                                            }
                                            break;
                                        case "Thứ 5":
                                            {
                                                delete = await _programsPackageReprository.DeleteProgramPackagesDailyByProgramId(id, packagecode, roomtype, amount, FromDate, 5);
                                            }
                                            break;
                                        case "Thứ 6":
                                            {
                                                delete = await _programsPackageReprository.DeleteProgramPackagesDailyByProgramId(id, packagecode, roomtype, amount, FromDate, 6);
                                            }
                                            break;
                                        case "Thứ 7":
                                            {
                                                delete = await _programsPackageReprository.DeleteProgramPackagesDailyByProgramId(id, packagecode, roomtype, amount, FromDate, 7);
                                            }
                                            break;
                                        case "CN":
                                            {
                                                delete = await _programsPackageReprository.DeleteProgramPackagesDailyByProgramId(id, packagecode, roomtype, amount, FromDate, 0);
                                            }
                                            break;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    delete = await _programsPackageReprository.DeleteProgramPackagesbyProgramId(id, packagecode, roomtype, amount, null, ApplyDate);
                }

                if (delete > 0)
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        msg = "Xóa thành công"
                    });
                }
                else
                {


                    return Ok(new
                    {
                        status = (int)ResponseType.ERROR,
                        msg = "Xóa không thành công"
                    });


                }


            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("DeleteProgramsPackage - ProgramsPackageController: " + ex);
                return Ok(new
                {
                    status = (int)ResponseType.ERROR,
                    msg = "Xóa không thành công vui lòng liên hệ bộ phận IT"
                });
            }



        }


        public async Task<IActionResult> Edit(int id, int type, string date, Double amount, long ProgramId, string roomtype, string PackageName)
        {

            try
            {
                ViewBag.id = id;
                ViewBag.ProgramId = ProgramId;
                ViewBag.type = type;
                ViewBag.date = date;
                ViewBag.amount = amount;
                ViewBag.roomtype = roomtype;
                ViewBag.PackageName = PackageName;
                if (type != 1)
                {
                    var model = await _programsPackageReprository.DetailPackagesbyProgramById(id);
                    return PartialView(model);
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Edit - ProgramsPackageController: " + ex);

            }
            return PartialView();
        }
        public async Task<IActionResult> EditProgramsPackage(int id, long amount, string date, int type, long ProgramId, string roomtype, string PackageName)
        {

            try
            {
                var Userid = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var model = new List<InsertProgramsPackageViewModel>();
                var data = new InsertProgramsPackageViewModel();

                var searchModel = new ProgramsPackageSearchViewModel();
                searchModel.ProgramId = ProgramId.ToString();
                searchModel.PageIndex = 1;
                searchModel.PageSize = 10;

                var WeekDay = (int)Convert.ToDateTime(date).DayOfWeek;
                if (WeekDay != 0) { WeekDay = WeekDay + 1; }
                data.id = id;
                data.Price = amount;

                data.ProgramId = (int)ProgramId;

                data.WeekDay = WeekDay.ToString();
                //data.OpenStatus = dataProgramPackage[0].OpenStatus;


                model.Add(data);

                if (type == 0)
                {
                    var Edit = await _programsPackageReprository.EditProgramPackagesbyProgram(data, Userid);
                }
                else
                {
                    var dataProgramsPackageDaily = await _programsPackageReprository.ListProgramsPackageDaily(searchModel);
                    var dataProgramPackage = dataProgramsPackageDaily.ListData.Where(s => s.ProgramName == PackageName).ToList();
                    var ProgramPackage = dataProgramPackage[0].ProgramsPackage.Where(s => s.RoomType == roomtype && Convert.ToDateTime(s.ToDate) >= Convert.ToDateTime(date) && Convert.ToDateTime(date) >= Convert.ToDateTime(s.FromDate)).ToList();

                    data.ApplyDateStr = Convert.ToDateTime(date).ToString("dd/MM/yyyy");
                    data.FromDateStr = ProgramPackage[0].FromDate;
                    data.ToDateStr = ProgramPackage[0].ToDate;
                    data.PackageCode = ProgramPackage[0].PackageCode;
                    data.PackageName = ProgramPackage[0].PackageName;
                    data.RoomType = ProgramPackage[0].RoomType;
                    data.RoomTypeId = ProgramPackage[0].RoomTypeId;

                    var Inser = await _programsPackageReprository.InsertProgramPackage(model, Userid, 1);
                    if (Inser < 0)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.FAILED,
                            msg = "Sửa giá không thành công"
                        });

                    }

                }

                return Ok(new
                {
                    status = (int)ResponseType.SUCCESS,
                    msg = "Sửa giá thành công"
                });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("EditProgramsPackage - ProgramsPackageController: " + ex);
                return Ok(new
                {
                    status = (int)ResponseType.FAILED,
                    msg = "Sửa giá không thành công vui lòng liên hệ bộ phận IT"
                });
            }
        }
        public async Task<IActionResult> ListDetailProgramsPrice(ProgramsPackageSearchViewModel searchModel)
        {
            try
            {
                var data = await _programsPackageReprository.ListdataProgramPackage(searchModel);

                var model = await _programsPackageReprository.ListDetaiProgramPackage(Convert.ToInt32(searchModel.ProgramId), data[0].PackageCode);
                ViewBag.data = model;
                ViewBag.FromDate = searchModel.FromDate;
                ViewBag.ToDate = searchModel.ToDate;
                if (data[0].OpenStatus != (int)ProgramsStatus.Da_duyet)
                {
                    ViewBag.Status = 1;
                }
                return PartialView();

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ListDetailProgramsPrice - ProgramsPackageController: " + ex);
                return PartialView();
            }

        }
        public IActionResult ProgramsPriceHotelIndex()
        {
            var CLIENT_TYPE = _allCodeRepository.GetListByType(AllCodeType.CLIENT_TYPE);
            ViewBag.CLIENT_TYPE = CLIENT_TYPE;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ListProgramsPriceHotel(ProgramsPackageSearchViewModel searchModel)
        {
            try
            {
                var model = new GenericViewModel<ProgramsPackagePriceViewModel>();
                if (searchModel.SupplierID == null && searchModel.HotelId == null)
                {
                    model = null;
                }
                else
                {

                    if (searchModel.StayStartDateFrom == null && searchModel.StayStartDateTo == null && searchModel.StayEndDateFrom == null && searchModel.StayEndDateTo == null)
                    {
                        model = null;
                    }
                    else
                    {

                        ViewBag.daymunber = (DateTime.ParseExact(searchModel.StayStartDateTo, "dd/MM/yyyy", null) - DateTime.ParseExact(searchModel.StayStartDateFrom, "dd/MM/yyyy", null)).Days;

                        DateTime departure_date2 = DateTime.ParseExact(searchModel.StayStartDateTo, "dd/MM/yyyy", null);

                        searchModel.StayStartDateTo = departure_date2.AddDays(-1).ToString("dd/MM/yyyy");
                        searchModel.ToDate = searchModel.StayStartDateTo;
                        ViewBag.Type = searchModel.Type;
                        if (searchModel.ClientType == -1)
                        {

                            model = await _programsPackageReprository.GetListProgramPriceHotel(searchModel);
                        }
                        else
                        {
                            //-- Tính giá về tay thông qua chính sách giá
                            var ListData = new List<ProgramsPackagePriceViewModel>();
                            //int nights = Convert.ToInt32((departure_date - arrival_date).TotalDays < 1 ? 1 : (departure_date - arrival_date).TotalDays);

                            if (searchModel.HotelId != null)
                            {
                                model = await _programsPackageReprository.GetListProgramPriceHotel(searchModel);
                                var profit_list = _hotelRepository.GetHotelRoomPricePolicy(searchModel.HotelId, "", string.Join(",", searchModel.ClientType));
                                if (profit_list != null && profit_list.Count > 0)
                                {
                                    foreach (var item in profit_list)
                                    {
                                        var new_ListData = new List<ProgramsPackagePriceViewModel>();
                                        new_ListData = model.ListData.Where(s => s.ProgramName == item.ProgramName && s.ProgramsPackageName == item.PackageCode && s.RoomType == item.RoomName).ToList();
                                        foreach (var item2 in new_ListData)
                                        {
                                            foreach (var item3 in item2.ProgramsPackagePrice)
                                            {
                                                if (item3.ProgramsPackage != null)
                                                {
                                                    foreach (var item4 in item3.ProgramsPackage)
                                                    {
                                                        switch (item.UnitId)
                                                        {
                                                            case (int)PriceUnitType.VND:
                                                                {
                                                                    item4.Price = item4.Price + item.Profit;
                                                                }
                                                                break;
                                                            case (int)PriceUnitType.PERCENT:
                                                                {
                                                                    item4.Price = item4.Price + Math.Round((item4.Price * item.Profit / (double)100), 0);
                                                                }
                                                                break;
                                                        }
                                                    }
                                                }
                                                if (item3.ProgramsPackageDaily != null)
                                                {
                                                    foreach (var item4 in item3.ProgramsPackageDaily)
                                                    {
                                                        switch (item.UnitId)
                                                        {
                                                            case (int)PriceUnitType.VND:
                                                                {
                                                                    item4.Price = item4.Price + item.Profit;
                                                                }
                                                                break;
                                                            case (int)PriceUnitType.PERCENT:
                                                                {
                                                                    item4.Price = item4.Price + Math.Round((item4.Price * item.Profit / (double)100), 0);
                                                                }
                                                                break;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        ListData.AddRange(new_ListData);
                                    }
                                    model.ListData = ListData;
                                }
                                else
                                {
                                    model = null;
                                }
                            }
                        }


                        return PartialView(model);
                    }

                }

                return PartialView(model);

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ListProgramsPriceHotel - ProgramsPackageController: " + ex);
                return PartialView();
            }

        }
        public async Task<IActionResult> ListPricePolicyHotel(ProgramsPackageSearchViewModel searchModel)
        {
            try
            {
                var model = new GenericViewModel<ProgramsPackagePriceViewModel>();
                model = await _programsPackageReprository.GetListProgramPackageDaily(searchModel);


                if (searchModel.RoomType != null && searchModel.RoomType != "")
                {
                    if (searchModel.ClientType == -1)
                    {

                        model.ListData = model.ListData.Where(s => s.RoomType == searchModel.RoomType && s.ProgramName == searchModel.PackageCode).ToList();
                    }
                    else
                    {
                        ViewBag.Type = 1;
                        //-- Tính giá về tay thông qua chính sách giá
                        var ListData = new List<ProgramsPackagePriceViewModel>();
                        model = await _programsPackageReprository.GetListProgramPriceHotel(searchModel);
                        var profit_list = _hotelRepository.GetHotelRoomPricePolicy(searchModel.HotelId, "", string.Join(",", searchModel.ClientType));
                        if (profit_list != null && profit_list.Count > 0)
                        {
                            foreach (var item in profit_list)
                            {
                                var new_ListData = new List<ProgramsPackagePriceViewModel>();
                                new_ListData = model.ListData.Where(s => s.ProgramName == item.ProgramName && s.ProgramsPackageName == item.PackageCode && s.RoomType == item.RoomName).ToList();
                                foreach (var item2 in new_ListData)
                                {
                                    foreach (var item3 in item2.ProgramsPackagePrice)
                                    {
                                        if (item3.ProgramsPackage != null)
                                        {
                                            foreach (var item4 in item3.ProgramsPackage)
                                            {
                                                switch (item.UnitId)
                                                {
                                                    case (int)PriceUnitType.VND:
                                                        {
                                                            item4.Price = item4.Price + item.Profit;
                                                        }
                                                        break;
                                                    case (int)PriceUnitType.PERCENT:
                                                        {
                                                            item4.Price = item4.Price + Math.Round((item4.Price * item.Profit / (double)100), 0);
                                                        }
                                                        break;
                                                }
                                            }
                                        }
                                        if (item3.ProgramsPackageDaily != null)
                                        {
                                            foreach (var item4 in item3.ProgramsPackageDaily)
                                            {
                                                switch (item.UnitId)
                                                {
                                                    case (int)PriceUnitType.VND:
                                                        {
                                                            item4.Price = item4.Price + item.Profit;
                                                        }
                                                        break;
                                                    case (int)PriceUnitType.PERCENT:
                                                        {
                                                            item4.Price = item4.Price + Math.Round((item4.Price * item.Profit / (double)100), 0);
                                                        }
                                                        break;
                                                }
                                            }
                                        }
                                    }
                                }
                                ListData.AddRange(new_ListData);
                            }
                            model.ListData = ListData.Where(s => s.RoomType == searchModel.RoomType && s.ProgramsPackageName == searchModel.PackageCode).ToList();
                        }
                        else
                        {
                            model = null;
                        }
                    }

                }
                return PartialView(model);

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ListPricePolicyHotel - ProgramsPackageController: " + ex);
                return PartialView();
            }

        }
        public async Task<IActionResult> AddListProgramsPackage(long id, long Packageid, string ProgramName, long RoomTypeid = 0, long type = 4)
        {
            try
            {

                var data = await _programsReprository.DetailPrograms(id);
                ViewBag.ServiceName = data.ServiceName;
                ViewBag.ProgramName = ProgramName;
                ViewBag.RoomTypeid = RoomTypeid;
                ViewBag.type = type;
                if (data.Status != (int)ProgramsStatus.Cho_TBP_duyet && data.Status != (int)ProgramsStatus.Da_duyet)
                {
                    ViewBag.btnThem = 1;
                    ViewBag.Status = 1;
                }
                var current_user = _ManagementUser.GetCurrentUser();
                if (current_user != null)
                {
                    var list = Array.ConvertAll(current_user.Role.Split(','), int.Parse);
                    foreach (var item in list)
                    {
                        var listPermissions2 = await _userRepository.CheckRolePermissionByUserAndRole(current_user.Id, item, (int)SortOrder.SUA, ReadFile.LoadConfig().Programs_Menuid);

                        if (listPermissions2 == true)
                        {
                            ViewBag.btnSua = 1;
                        }
                    }

                }

                ViewBag.id = id;
                ViewBag.Packageid = Packageid;
                if (Packageid != 0)
                {
                    var searchModel = new ProgramsPackageSearchViewModel();
                    searchModel.ProgramId = id.ToString();
                    searchModel.PageIndex = 1;
                    searchModel.PageSize = 10;
                    var model = new GenericViewModel<ProgramsPackagePriceViewModel>();

                    model = await _programsPackageReprository.GetListProgramPackageDaily(searchModel);

                    var dataProgramPackage = await _programsPackageReprository.ListdataProgramPackage(searchModel);
                    ViewBag.data = dataProgramPackage;
                    ViewBag.ProgramPackageDaily = model != null ? model.ListData.Where(s => s.ProgramName == ProgramName).ToList() : null;
                    if (RoomTypeid > 0)
                    {
                        ViewBag.data = dataProgramPackage != null && dataProgramPackage.Count > 0 ? dataProgramPackage.Where(s => s.RoomTypeId == RoomTypeid.ToString()).ToList() : null;
                        ViewBag.ProgramPackageDaily = model != null ? model.ListData.Where(s => s.ProgramName == ProgramName && s.RoomTypeId == RoomTypeid.ToString()).ToList() : null;
                    }

                }

                ViewBag.id = id;
                return View();

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("AddListProgramsPackage - ProgramsPackageController: " + ex);
                return View();
            }

        }
        public async Task<IActionResult> ListDataAddProgramsPackage(long id, long Packageid, string ProgramName, long RoomTypeid = 0, long type = 4)
        {
            try
            {

                var data = await _programsReprository.DetailPrograms(id);
                ViewBag.ServiceName = data.ServiceName;
                ViewBag.ProgramName = ProgramName;
                ViewBag.RoomTypeid = RoomTypeid;
                ViewBag.type = type;
                if (data.Status != (int)ProgramsStatus.Cho_TBP_duyet && data.Status != (int)ProgramsStatus.Da_duyet)
                {
                    ViewBag.btnThem = 1;
                    ViewBag.Status = 1;
                }
                var current_user = _ManagementUser.GetCurrentUser();
                if (current_user != null)
                {
                    var list = Array.ConvertAll(current_user.Role.Split(','), int.Parse);
                    foreach (var item in list)
                    {
                        var listPermissions2 = await _userRepository.CheckRolePermissionByUserAndRole(current_user.Id, item, (int)SortOrder.SUA, ReadFile.LoadConfig().Programs_Menuid);

                        if (listPermissions2 == true)
                        {
                            ViewBag.btnSua = 1;
                        }
                    }

                }

                ViewBag.id = id;
                ViewBag.Packageid = Packageid;
                if (Packageid != 0)
                {
                    var searchModel = new ProgramsPackageSearchViewModel();
                    searchModel.ProgramId = id.ToString();
                    searchModel.PageIndex = 1;
                    searchModel.PageSize = 10;
                    var model = new GenericViewModel<ProgramsPackagePriceViewModel>();

                    model = await _programsPackageReprository.GetListProgramPackageDaily(searchModel);

                    var dataProgramPackage = await _programsPackageReprository.ListdataProgramPackage(searchModel);
                    ViewBag.data = dataProgramPackage;
                    ViewBag.ProgramPackageDaily = model != null ? model.ListData.Where(s => s.ProgramName == ProgramName).ToList() : null;
                    if (RoomTypeid > 0)
                    {
                        ViewBag.data = dataProgramPackage != null && dataProgramPackage.Count > 0 ? dataProgramPackage.Where(s => s.RoomTypeId == RoomTypeid.ToString()).ToList() : null;
                        ViewBag.ProgramPackageDaily = model != null ? model.ListData.Where(s => s.ProgramName == ProgramName && s.RoomTypeId == RoomTypeid.ToString()).ToList() : null;
                    }

                }

                ViewBag.id = id;
                return PartialView();

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("AddListProgramsPackage - ProgramsPackageController: " + ex);
                return PartialView();
            }

        }
    }
}
