using Caching.Elasticsearch;
using Entities.ViewModels;
using Entities.ViewModels.Funding;
using Entities.ViewModels.Programs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
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
using WEB.CMS.Models;
using HotelModel = Entities.ViewModels.Programs.HotelModel;

namespace GDHT.CMS.Controllers.Programs
{
    [CustomAuthorize]
    public class ProgramsController : Controller
    {
        private readonly IProgramsReprository _programsReprository;
        private readonly IAllCodeRepository _allCodeRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly ProgramsESRepository _programsESRepository;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _WebHostEnvironment;
        private IProgramsPackageReprository _programsPackageReprository;
        private ManagementUser _ManagementUser;
        private IUserRepository _userRepository;
        public ProgramsController(IProgramsReprository programsReprository, IAllCodeRepository allCodeRepository, ISupplierRepository supplierRepository,
            IConfiguration configuration, IWebHostEnvironment WebHostEnvironment, IProgramsPackageReprository programsPackageReprository, ManagementUser ManagementUser, IUserRepository userRepository)
        {
            _programsReprository = programsReprository;
            _allCodeRepository = allCodeRepository;
            _supplierRepository = supplierRepository;
            _configuration = configuration;
            _programsESRepository = new ProgramsESRepository(configuration["DataBaseConfig:Elastic:Host"]);
            _WebHostEnvironment = WebHostEnvironment;
            _programsPackageReprository = programsPackageReprository;
            _ManagementUser = ManagementUser;
            _userRepository = userRepository;

        }
        public async Task<IActionResult> Index()
        {
            var Status = _allCodeRepository.GetListByType(AllCodeType.PROGRAM_STATUS);
            var Service = _allCodeRepository.GetListByType(AllCodeType.SERVICE_TYPE);
            ViewBag.Status = Status;
            ViewBag.Service = Service;
            var current_user = _ManagementUser.GetCurrentUser();
            if (current_user != null)
            {
                var i = 0;
                if (current_user != null && !string.IsNullOrEmpty(current_user.Role))
                {
                    var list = Array.ConvertAll(current_user.Role.Split(','), int.Parse);
                    foreach (var item in list)
                    {
                        //kiểm tra chức năng có đc phép sử dụng

                        var listPermissions2 = await _userRepository.CheckRolePermissionByUserAndRole(current_user.Id, item, (int)SortOrder.THEM, ReadFile.LoadConfig().Programs_Menuid);
                        var listPermissions5 = await _userRepository.CheckRolePermissionByUserAndRole(current_user.Id, item, (int)SortOrder.XUAT_BC, ReadFile.LoadConfig().Programs_Menuid);

                        if (listPermissions2 == true)
                        {
                            ViewBag.btnThem = 1;
                        }
                        if (listPermissions5 == true)
                        {
                            ViewBag.btnXuatEX = 1;
                        }

                    }
                }
            }
            return View();
        }

        public async Task<IActionResult> SearchPrograms(ProgramsSearchViewModel searchModel)
        {
            try
            {
                var model = new GenericViewModel<ProgramsViewModel>();
                var current_user = _ManagementUser.GetCurrentUser();
                if (current_user != null)
                {

                    var i = 0;
                    var listPermissionstype = 0;
                    var listPermissions6type = 0;
                    var listPermissions7type = 0;
                    if (current_user != null && !string.IsNullOrEmpty(current_user.Role))
                    {
                        var list = Array.ConvertAll(current_user.Role.Split(','), int.Parse);


                        foreach (var item in list)
                        {
                            switch (Convert.ToInt32(item))
                            {

                                case (int)RoleType.TPDHKS:
                                    {
                                        searchModel.ServiceType += ","+((int)ServicesType.VINHotelRent).ToString();
                                        searchModel.ServiceType += "," + ((int)ServicesType.Other).ToString();
                                        searchModel.ServiceType += "," + ((int)ServicesType.VinWonder).ToString();
                                        searchModel.ServiceType += "," + ((int)ServicesType.VehicleRent).ToString();
                                    }
                                    break;
                                case (int)RoleType.TPDHVe:
                                    {
                                        searchModel.ServiceType += "," + ((int)ServicesType.FlyingTicket).ToString();
                                        searchModel.ServiceType += "," + ((int)ServicesType.Other).ToString();
                                        searchModel.ServiceType += "," + ((int)ServicesType.VinWonder).ToString();
                                        searchModel.ServiceType += "," + ((int)ServicesType.VehicleRent).ToString();
                                    }
                                    break;
                                case (int)RoleType.TPDHTour:
                                    {
                                        searchModel.ServiceType += "," + ((int)ServicesType.Tourist).ToString();
                                        searchModel.ServiceType += "," + ((int)ServicesType.Other).ToString();
                                        searchModel.ServiceType += "," + ((int)ServicesType.VinWonder).ToString();
                                        searchModel.ServiceType += "," + ((int)ServicesType.VehicleRent).ToString();
                                  
                                    }
                                    break;

                                case (int)RoleType.DHPQ:
                                    {
                                        searchModel.ServiceType += "," + ((int)ServicesType.VINHotelRent).ToString();
                                        searchModel.ServiceType += "," + ((int)ServicesType.FlyingTicket).ToString();
                                        searchModel.ServiceType += "," + ((int)ServicesType.Tourist).ToString();
                                        searchModel.ServiceType += "," + ((int)ServicesType.Other).ToString();
                                        searchModel.ServiceType += "," + ((int)ServicesType.VinWonder).ToString();
                                        searchModel.ServiceType += "," + ((int)ServicesType.VehicleRent).ToString();  
                                    }
                                    break;
                                case (int)RoleType.DHTour:
                                    {
                                        searchModel.ServiceType += "," + ((int)ServicesType.Tourist).ToString();
                                        searchModel.ServiceType += "," + ((int)ServicesType.Other).ToString();
                                        searchModel.ServiceType += "," + ((int)ServicesType.VinWonder).ToString();
                                        searchModel.ServiceType += "," + ((int)ServicesType.VehicleRent).ToString();
                                    }
                                    break;
                                case (int)RoleType.DHVe:
                                    {
                                        searchModel.ServiceType += "," + ((int)ServicesType.FlyingTicket).ToString();
                                        searchModel.ServiceType += "," + ((int)ServicesType.Other).ToString();
                                        searchModel.ServiceType += "," + ((int)ServicesType.VinWonder).ToString();
                                        searchModel.ServiceType += "," + ((int)ServicesType.VehicleRent).ToString();
                                    }
                                    break;
                                case (int)RoleType.DHKS:
                                    {
                                        searchModel.ServiceType += "," + ((int)ServicesType.VINHotelRent).ToString();
                                        searchModel.ServiceType += "," + ((int)ServicesType.Other).ToString();
                                        searchModel.ServiceType += "," + ((int)ServicesType.VinWonder).ToString();
                                        searchModel.ServiceType += "," + ((int)ServicesType.VehicleRent).ToString();
                                    }
                                    break;
                                case (int)RoleType.Admin:
                                case (int)RoleType.KT:
                                case (int)RoleType.GDHN:
                                case (int)RoleType.GDHPQ:
                                case (int)RoleType.GD:
                                case (int)RoleType.PhoTPKeToan:
                                    {
                                        searchModel.ServiceType += null;
                                    }
                                    break;
                            }

                            //kiểm tra chức năng có đc phép sử dụng
                            var listPermissions = await _userRepository.CheckRolePermissionByUserAndRole(current_user.Id, item, (int)SortOrder.TRUY_CAP, ReadFile.LoadConfig().Programs_Menuid);
                            var listPermissions6 = await _userRepository.CheckRolePermissionByUserAndRole(current_user.Id, item, (int)SortOrder.VIEW_ALL, ReadFile.LoadConfig().Programs_Menuid);
                            var listPermissions7 = await _userRepository.CheckRolePermissionByUserAndRole(current_user.Id, item, (int)SortOrder.DUYET, ReadFile.LoadConfig().Programs_Menuid);

                            if (listPermissions == true)
                            {
                                searchModel.SalerPermission += current_user.UserUnderList; i++;
                                listPermissionstype++;
                            }
  							else
                            {
                                if (listPermissionstype == 0)
                                {
                                    searchModel.SalerPermission += "," + 0;
                                }
                               
                            }                            
                            if ((listPermissions == true && listPermissions7 == true))
                            {
                                searchModel.SalerPermission += current_user.UserUnderList;
                                i++;
                                listPermissions7type++;
                            }
  							else
                            {

                                if (listPermissions7type == 0)
                                {
                                    searchModel.SalerPermission += "," + 0;
                                }
                            }                            
                            if (listPermissions6 == true)
                            {
                                searchModel.SalerPermission = null;
                                listPermissions6type ++;
                                i++;
                            }
  							else
                            {
                                if (listPermissions6type == 0)
                                {
                                    searchModel.SalerPermission += "," + 0;
                                }
                              
                            }
                        }
                    }
                    if (i != 0)
                    {
                        searchModel.ServiceType = searchModel.ServiceType!=null? searchModel.ServiceType.TrimStart(',', ' '): searchModel.ServiceType;
                        model = await _programsReprository.SearchPrograms(searchModel);
                    }
                    else
                    {
                        return PartialView(model);
                    }

                }



                return PartialView(model);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SearchPrograms - ProgramsController: " + ex);
                return PartialView();
            }


        }
        public async Task<IActionResult> SearchProgramsBySupplierId(ProgramsSearchSupplierId searchModel)
        {


            try
            {
                var model = new GenericViewModel<ProgramsViewModel>();
                model = await _programsReprository.GetlistHotelBySupplierId(searchModel);

                return PartialView(model);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SearchProgramsBySupplierId - ProgramsController: " + ex);
                return PartialView();
            }
           ;

        }
        public async Task<IActionResult> DetailPrograms(long id)
        {
            try
            {
                var current_user = _ManagementUser.GetCurrentUser();
                if (current_user != null)
                {
                    var i = 0;
                    if (current_user != null && !string.IsNullOrEmpty(current_user.Role))
                    {
                        var list = Array.ConvertAll(current_user.Role.Split(','), int.Parse);
                        foreach (var item in list)
                        {
                            //kiểm tra chức năng có đc phép sử dụng

                            var listPermissions2 = await _userRepository.CheckRolePermissionByUserAndRole(current_user.Id, item, (int)SortOrder.THEM, ReadFile.LoadConfig().Programs_Menuid);
                            var listPermissions5 = await _userRepository.CheckRolePermissionByUserAndRole(current_user.Id, item, (int)SortOrder.DUYET, ReadFile.LoadConfig().Programs_Menuid);

                            if (listPermissions2 == true)
                            {
                                ViewBag.btnThem = 1;
                            }
                            if (listPermissions5 == true)
                            {
                                ViewBag.btnDuet = 1;
                            }

                        }
                    }
                }
                var searchModel = new ProgramsPackageSearchViewModel();
                searchModel.ProgramId = id.ToString();
                searchModel.PageIndex = -1;
                searchModel.PageSize = 10;
                var data2 = await _programsPackageReprository.ListdataProgramPackage(searchModel);
              
                var data = await _programsReprository.DetailPrograms(id);
                if (data != null)
                {
                    ViewBag.type = 1;
                }
                ViewBag.data = data;
                ViewBag.id = id;
                return View();
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("DetailPrograms - ProgramsController: " + ex);
                return View();
            }
        }

        public async Task<IActionResult> AddProgramsView(long id)
        {
            try
            {
                var serviceType = _allCodeRepository.GetListByType("SERVICE_TYPE");
                if (id != 0)
                {
                    ViewBag.id = id;
                    ViewBag.serviceType = serviceType;
                    var data = await _programsReprository.DetailPrograms(id);
                    ViewBag.data = data;
                    return PartialView();
                }
                else
                {
                    ViewBag.id = id;
                    ViewBag.serviceType = serviceType;
                    return PartialView();
                }

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("AddProgramsView - ProgramsController: " + ex);
                return PartialView();
            }
        }
        public async Task<IActionResult> SummitPrograms(ProgramsModel model)
        {
            try
            {
                var userid = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                model.CreatedBy = userid;
                model.UpdatedBy = userid;
                model.UserVerify = userid;
                if (model.Id == 0)
                {

                    var data = await _programsReprository.InsertPrograms(model);
                    if (data > 0)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.SUCCESS,
                            msg = "Thêm mới thành công",

                        });
                    }
                    else
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.SUCCESS,
                            msg = "Thêm mới không thành công",

                        });
                    }

                }
                else
                {
                    var data = await _programsReprository.UpdatePrograms(model);
                    if (data > 0)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.SUCCESS,
                            msg = "Chỉnh sửa thành công",

                        });
                    }
                    else
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.SUCCESS,
                            msg = "Chỉnh sửa không thành công",

                        });
                    }

                }


            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SummitPrograms - ProgramsController: " + ex);
                return Ok(new
                {
                    status = (int)ResponseType.SUCCESS,
                    msg = "Đã có lỗi xảy ra, vui lòng liên hệ IT",

                });
            }

        }
        public async Task<IActionResult> UpdateProgramsStatus(int statustype, long id)
        {
            try
            {

                var userid = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

                var date = await _programsReprository.UpdateProgramsStatus(statustype, id, userid);
                if (date > 0)
                {
                    if (statustype != (int)ProgramsStatus.Da_Xoa)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.SUCCESS,
                            msg = "Cập nhật trạng thái thành công",
                        });
                    }
                    else
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.SUCCESS,
                            msg = "Xóa thành công",
                        });
                    }
                }
                else
                {
                    if (statustype != (int)ProgramsStatus.Da_Xoa)
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.ERROR,
                            msg = "Cập nhật trạng thái không thành công",
                        });
                    }
                    else
                    {
                        return Ok(new
                        {
                            status = (int)ResponseType.ERROR,
                            msg = "Xóa không thành công",
                        });
                    }

                }



            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SummitPrograms - ProgramsController: " + ex);
                return Ok(new
                {
                    status = (int)ResponseType.SUCCESS,
                    msg = "Đã có lỗi xảy ra, vui lòng liên hệ IT",

                });
            }

        }
        public async Task<string> GetHotelSuggest(long id)
        {
            try
            {
                var ListSupplier = await _programsReprository.GetlistHotelBySupplierId(id);
                if (ListSupplier == null)
                {
                    return null;
                }
                var suggestionlist = ListSupplier.Select(s => new HotelModel
                {
                   HotelId=s.HotelId,
                   Name=s.Name,
                   ShortName=s.ShortName
                }).ToList();
                return JsonConvert.SerializeObject(suggestionlist);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetHotelSuggest - ProgramsController: " + ex);
                return null;
            }
        }
        public async Task<string> GetSupplierSuggest(long id)
        {
            try
            {
                var ListSupplier = await _programsReprository.GetlistSupplierByHotelId(id);
                if (ListSupplier == null)
                {
                    return null;
                }
                var suggestionlist = ListSupplier.Select(s => new SupplierModel
                {
                    SupplierId = s.SupplierId,
                    FullName = s.FullName,
                    
                }).ToList();
                return JsonConvert.SerializeObject(suggestionlist);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetHotelSuggest - ProgramsController: " + ex);
                return null;
            }
        }
        public async Task<string> GetHotelRoomSuggest(long id,string ServiceName,long HotelId)
        {
            try
            {
                var HoteRoomlList = _supplierRepository.GetRoomListOfSupplier(id, ServiceName, HotelId, 1, 50);
                if (HoteRoomlList == null)
                {
                    return null;
                }
                var suggestionlist = HoteRoomlList.Select(s => new SupplierRoomGridModel
                {
                    Id = s.Id,
                    Name = s.Name,
                }).ToList();
                return JsonConvert.SerializeObject(suggestionlist);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetHotelRoomSuggest - ProgramsController: " + ex);
                return null;
            }
        }
        [HttpPost]
        public async Task<IActionResult> ProgramSuggestion(string txt_search)
        {

            try
            {
                if (txt_search == null)
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        data = new List<ProgramsViewModel>()
                    });
                }
                var data = await _programsESRepository.GetProgramsSuggesstion(txt_search);
                return Ok(new
                {
                    status = (int)ResponseType.SUCCESS,
                    data = data
                });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ProgramSuggestion - ProgramsController: " + ex.ToString());
                return Ok(new
                {
                    status = (int)ResponseType.SUCCESS,
                    data = new List<ProgramsViewModel>()
                });
            }

        }
       
        public async Task<IActionResult> ExportExcel(ProgramsSearchViewModel searchModel, FieldPrograms field)
        {
            try
            {
                string _FileName = "Danh sách chương trình.xlsx";
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

                var rsPath = await _programsReprository.ExportDeposit(searchModel, FilePath, field);

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
                LogHelper.InsertLogTelegram("ExportExcel - OrderController: " + ex);
                return new JsonResult(new
                {
                    isSuccess = false,
                    message = ex.Message.ToString()
                });
            }
        }
    }
}
