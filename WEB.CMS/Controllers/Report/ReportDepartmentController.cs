using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Entities.Models;
using Entities.ViewModels;
using Entities.ViewModels.Funding;
using Entities.ViewModels.Report;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using Repositories.IRepositories;
using Utilities;
using Utilities.Contants;
using WEB.CMS.Customize;

namespace GDHT.CMS.Controllers.Report
{
    [CustomAuthorize]
    public class ReportDepartmentController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IDepartmentRepository _DepartmentRepository;
        private readonly IWebHostEnvironment _WebHostEnvironment;
        private readonly IHotelBookingRepositories _hotelBookingRepositories;
        private readonly IOtherBookingRepository _otherBookingRepository;
        private readonly IHotelBookingRoomExtraPackageRepository _hotelBookingRoomExtraPackageRepository;

        private readonly IReportRepository _reportRepository;
        private ManagementUser _ManagementUser;
        private readonly IAllCodeRepository _allCodeRepository;

        public ReportDepartmentController(IOrderRepository orderRepository, IDepartmentRepository departmentRepository,
            IWebHostEnvironment WebHostEnvironment, IHotelBookingRepositories hotelBookingRepositories, IHotelBookingRoomExtraPackageRepository hotelBookingRoomExtraPackageRepository,
             IReportRepository reportRepository, ManagementUser managementUser, IAllCodeRepository allcodeRepository, IOtherBookingRepository otherBookingRepository)
        {
            _orderRepository = orderRepository;
            _DepartmentRepository = departmentRepository;
            _WebHostEnvironment = WebHostEnvironment;
            _hotelBookingRepositories = hotelBookingRepositories;
            _hotelBookingRoomExtraPackageRepository = hotelBookingRoomExtraPackageRepository;
            _reportRepository = reportRepository;
            _ManagementUser = managementUser;
            _allCodeRepository = allcodeRepository;
            _otherBookingRepository = otherBookingRepository;
           

        }
        public async Task<IActionResult> Index()
        {
            var departments = await _DepartmentRepository.GetAll("");
            var orderStatus = _allCodeRepository.GetListByType(AllCodeType.ORDER_STATUS);
            var branchs = _allCodeRepository.GetListByType(AllCodeType.BRANCH_CODE);
            var serviceType = _allCodeRepository.GetListByType(AllCodeType.SERVICE_TYPE);
            var PAYMENT_STATUS = _allCodeRepository.GetListByType(AllCodeType.PAYMENT_STATUS);
            var PERMISION_TYPE = _allCodeRepository.GetListByType(AllCodeType.PERMISION_TYPE);
    
            ViewBag.PAYMENT_STATUS = PAYMENT_STATUS;
            ViewBag.PERMISION_TYPE = PERMISION_TYPE;
            ViewBag.departments = departments;
            ViewBag.orderStatus = orderStatus;
            ViewBag.Role = 0;
            ViewBag.Branch = branchs;
            ViewBag.serviceType = serviceType;
            var current_user = _ManagementUser.GetCurrentUser();
            if (current_user.Role != "")
            {
                var list = current_user.Role.Split(',');
                foreach (var item in list)
                {
                    switch (Convert.ToInt32(item))
                    {
                        case (int)RoleType.SaleOnl:
                        case (int)RoleType.SaleTour:
                        case (int)RoleType.SaleKd:
                            {
                                ViewBag.Role = 1;
                            }
                            break;
                    }
                }
            }
                return View();
        }

        public async Task<IActionResult> SearchReportDepartment(ReportDepartmentViewModel searchModel)
        {
            try
            {
                if (searchModel.HINHTHUCTT != null)
                {
                    var listHINHTHUCTT = searchModel.HINHTHUCTT.Split(',');
                    searchModel.PermisionType = Convert.ToInt32(listHINHTHUCTT[0]);
                    searchModel.PaymentStatus = Convert.ToInt32(listHINHTHUCTT[1]);
                }
                var current_user = _ManagementUser.GetCurrentUser();
                if (current_user != null)
                {
                    if (current_user.Role != "")
                    {
                        var list = current_user.Role.Split(',');
                        foreach (var item in list)
                        {
                            switch (Convert.ToInt32(item))
                            {
                                case (int)RoleType.SaleOnl:
                                case (int)RoleType.SaleTour:
                                case (int)RoleType.SaleKd:
                                    {
                                        searchModel.SalerPermission += current_user.UserUnderList;
                                    }
                                    break;
                                case (int)RoleType.TPDHKS:
                                case (int)RoleType.TPDHVe:
                                case (int)RoleType.TPDHTour:
                                case (int)RoleType.TPKS:
                                case (int)RoleType.TPTour:
                                case (int)RoleType.TPVe:
                                case (int)RoleType.DHPQ:
                                case (int)RoleType.DHTour:
                                case (int)RoleType.DHVe:
                                    {
                                        searchModel.SalerPermission += current_user.UserUnderList;
                                    }
                                    break;
                                case (int)RoleType.Admin:
                                case (int)RoleType.KT:
                                case (int)RoleType.GDHN:
                                case (int)RoleType.GDHPQ:
                                case (int)RoleType.GD:
                                case (int)RoleType.PhoTPKeToan:
                                case (int)RoleType.KeToanTruong:
                                    {
                                        searchModel.SalerPermission = null;
                                    }
                                    break;
                            }
                        }

                        var data = new List<SearchReportDepartmentViewModel>();
                        var model = new GenericViewModel<SearchReportDepartmentViewModel>();
                        model = await _DepartmentRepository.GetReportDepartment(searchModel);

                        if (model.ListData != null)
                        {
                            var data_ks = model.ListData.Where(s => s.ParentDepartmentId == 16).ToList();
                            if(data_ks!=null && data_ks.Count > 0) { data.AddRange(data_ks);
                                foreach(var item in data_ks)
                                {
                                    model.ListData.Remove(item);
                                }
                                
                            }

                            var data_DHLenDon = model.ListData.Where(s => s.DepartmentId == 36).ToList();
                            if (data_DHLenDon != null && data_ks.Count > 0) { data.AddRange(data_DHLenDon);
                                foreach (var item in data_DHLenDon)
                                {
                                    model.ListData.Remove(item);
                                }
                            } 

                            var data_LH1 = model.ListData.Where(s => s.DepartmentId == 14).ToList();
                            if (data_LH1 != null && data_LH1.Count > 0) { data.AddRange(data_LH1);
                                foreach (var item in data_LH1)
                                {
                                    model.ListData.Remove(item);
                                }
                            }

                            var data_LH2 = model.ListData.Where(s => s.DepartmentId == 47).ToList();
                            if (data_LH2 != null && data_LH2.Count > 0) { data.AddRange(data_LH2);
                                foreach (var item in data_LH2)
                                {
                                    model.ListData.Remove(item);
                                }
                            }

                            var data_LH3 = model.ListData.Where(s => s.DepartmentId == 48).ToList();
                            if (data_LH3 != null && data_LH3.Count > 0) { data.AddRange(data_LH3);
                                foreach (var item in data_LH3)
                                {
                                    model.ListData.Remove(item);
                                }
                            }

                            var data_Inbound = model.ListData.Where(s => s.DepartmentId == 38).ToList();
                            if (data_Inbound != null && data_Inbound.Count > 0) { data.AddRange(data_Inbound);
                                foreach (var item in data_Inbound)
                                {
                                    model.ListData.Remove(item);
                                }
                            }

                            var data_Sk = model.ListData.Where(s => s.DepartmentId == 50).ToList();
                            if (data_Sk != null && data_Sk.Count > 0) { data.AddRange(data_Sk);
                                foreach (var item in data_Sk)
                                {
                                    model.ListData.Remove(item);
                                }
                            }

                            var data_SG_LH1 = model.ListData.Where(s => s.DepartmentId == 20).ToList();
                            if (data_SG_LH1 != null && data_SG_LH1.Count > 0) { data.AddRange(data_SG_LH1);
                                foreach (var item in data_SG_LH1)
                                {
                                    model.ListData.Remove(item);
                                }
                            }

                            var data_SG_LH3 = model.ListData.Where(s => s.DepartmentId == 51).ToList();
                            if (data_SG_LH3 != null && data_SG_LH3.Count > 0) { data.AddRange(data_SG_LH3);
                                foreach (var item in data_SG_LH3)
                                {
                                    model.ListData.Remove(item);
                                }
                            }
                            data.AddRange(model.ListData);
                        }

                        model.ListData = data;

                        var model2 = new GenericViewModel<SearchReportDepartmentViewModel>();
                      var searchModel2 = searchModel;
                        searchModel2.PageIndex = -1;
                        model2 = await _DepartmentRepository.GetReportDepartment(searchModel2);
                        if(model2!=null && model2.ListData!=null && model2.ListData.Count > 0)
                        {
                            ViewBag.TotalOrder = model2.ListData.Sum(s => s.TotalOrder);
                            ViewBag.Amount = model2.ListData.Sum(s => s.Amount);
                            ViewBag.Price = model2.ListData.Sum(s => s.Price);
                            ViewBag.Comission = model2.ListData.Sum(s => s.Comission);
                            ViewBag.Profit = model2.ListData.Sum(s => s.Profit);
                            ViewBag.AmountVat = model2.ListData.Sum(s => s.AmountVat);
                            ViewBag.PriceVat = model2.ListData.Sum(s => s.PriceVat);
                            ViewBag.ProfitVat = model2.ListData.Sum(s => s.ProfitVat);
                        }
                      
                        return PartialView(model);
                    }

                }
                return PartialView();
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SearchReportDepartment - ReportDepartmentController: " + ex);
                return PartialView();
            }
        }
        public async Task<IActionResult> SearchReportDepartmentsaler(ReportDepartmentViewModel searchModel)
        {
            try
            {
                if (searchModel.HINHTHUCTT != null)
                {
                    var listHINHTHUCTT = searchModel.HINHTHUCTT.Split(',');
                    searchModel.PermisionType = Convert.ToInt32(listHINHTHUCTT[0]);
                    searchModel.PaymentStatus = Convert.ToInt32(listHINHTHUCTT[1]);
                }
                var current_user = _ManagementUser.GetCurrentUser();
                if (current_user != null)
                {
                    if (current_user.Role != "")
                    {
                        var list = current_user.Role.Split(',');
                        foreach (var item in list)
                        {
                            switch (Convert.ToInt32(item))
                            {
                                case (int)RoleType.SaleOnl:
                                case (int)RoleType.SaleTour:
                                case (int)RoleType.SaleKd:
                                    {
                                        searchModel.SalerPermission += current_user.UserUnderList;
                                     
                                    }
                                    break;
                                case (int)RoleType.TPDHKS:
                                case (int)RoleType.TPDHVe:
                                case (int)RoleType.TPDHTour:
                                case (int)RoleType.TPKS:
                                case (int)RoleType.TPTour:
                                case (int)RoleType.TPVe:
                                case (int)RoleType.DHPQ:
                                case (int)RoleType.DHTour:
                                case (int)RoleType.DHVe:
                                    {
                                        searchModel.SalerPermission += current_user.UserUnderList;
                                    }
                                    break;
                                case (int)RoleType.Admin:
                                case (int)RoleType.KT:
                                case (int)RoleType.GDHN:
                                case (int)RoleType.GDHPQ:
                                case (int)RoleType.GD:
                                case (int)RoleType.PhoTPKeToan:
                                case (int)RoleType.KeToanTruong:
                                    {
                                        searchModel.SalerPermission = null;
                                    }
                                    break;
                            }
                        }
                        var model = new GenericViewModel<ListReportDepartmentViewModel>();
                        model = await _DepartmentRepository.GetReportDepartmentsaler(searchModel);
                        var model2 = new GenericViewModel<ListReportDepartmentViewModel>();
                        var searchModel2 = searchModel;
                        searchModel2.PageIndex = -1;
                        model2 = await _DepartmentRepository.GetReportDepartmentsaler(searchModel2);
                        if (model2 != null && model2.ListData != null && model2.ListData.Count > 0)
                        {
                            ViewBag.TotalOrder = model2.ListData.Sum(s => s.ParentDepartmentTotalOrder);
                            ViewBag.Amount = model2.ListData.Sum(s => s.ParentDepartmentAmount);
                            ViewBag.Price = model2.ListData.Sum(s => s.ParentDepartmentPrice);
                            ViewBag.Comission = model2.ListData.Sum(s => s.ParentDepartmentComission);
                            ViewBag.Profit = model2.ListData.Sum(s => s.ParentDepartmentProfit);
                            ViewBag.AmountVat = model2.ListData.Sum(s => s.ParentDepartmentAmountVat);
                            ViewBag.PriceVat = model2.ListData.Sum(s => s.ParentDepartmentPriceVat);
                            ViewBag.ProfitVat = model2.ListData.Sum(s => s.ParentDepartmentProfitVat);
                        }
                        
                        return PartialView(model);
                    }

                }
                return PartialView();
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SearchReportDepartmentsaler - ReportDepartmentController: " + ex);
                return PartialView();
            }
           

        }
        public async Task<IActionResult> SearchReportDepartmentBySupplier(ReportDepartmentViewModel searchModel)
        {
            try
            {
                if (searchModel.HINHTHUCTT != null)
                {
                    var listHINHTHUCTT = searchModel.HINHTHUCTT.Split(',');
                    searchModel.PermisionType = Convert.ToInt32(listHINHTHUCTT[0]);
                    searchModel.PaymentStatus = Convert.ToInt32(listHINHTHUCTT[1]);
                }
                var current_user = _ManagementUser.GetCurrentUser();
                if (current_user != null)
                {
                    if (current_user.Role != "")
                    {
                        var list = current_user.Role.Split(',');
                        foreach (var item in list)
                        {
                            switch (Convert.ToInt32(item))
                            {
                                case (int)RoleType.SaleOnl:
                                case (int)RoleType.SaleTour:
                                case (int)RoleType.SaleKd:
                                    {
                                        searchModel.SalerPermission += current_user.UserUnderList;
                                    }
                                    break;
                                case (int)RoleType.TPDHKS:
                                case (int)RoleType.TPDHVe:
                                case (int)RoleType.TPDHTour:
                                case (int)RoleType.TPKS:
                                case (int)RoleType.TPTour:
                                case (int)RoleType.TPVe:
                                case (int)RoleType.DHPQ:
                                case (int)RoleType.DHTour:
                                case (int)RoleType.DHVe:
                                    {
                                        searchModel.SalerPermission += current_user.UserUnderList;
                                    }
                                    break;
                                case (int)RoleType.Admin:
                                case (int)RoleType.KT:
                                case (int)RoleType.GDHN:
                                case (int)RoleType.GDHPQ:
                                case (int)RoleType.GD:
                                case (int)RoleType.PhoTPKeToan:
                                case (int)RoleType.KeToanTruong:
                                    {
                                        searchModel.SalerPermission = null;
                                    }
                                    break;
                            }
                        }
                        var model = new GenericViewModel<SearchReportDepartmentSupplier>();
                        model = await _DepartmentRepository.GetRevenueBySupplier(searchModel);

                        return PartialView(model);
                    }

                }
                return PartialView();
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SearchReportDepartmentBySupplier - ReportDepartmentController: " + ex);
                return PartialView();
            }


        }
        public async Task<IActionResult> SearchReportDepartmentClient(ReportDepartmentViewModel searchModel)
        {
            try
            {
                if (searchModel.HINHTHUCTT != null)
                {
                    var listHINHTHUCTT = searchModel.HINHTHUCTT.Split(',');
                    searchModel.PermisionType = Convert.ToInt32(listHINHTHUCTT[0]);
                    searchModel.PaymentStatus = Convert.ToInt32(listHINHTHUCTT[1]);
                }
                var current_user = _ManagementUser.GetCurrentUser();
                if (current_user != null)
                {
                    if (current_user.Role != "")
                    {
                        var list = current_user.Role.Split(',');
                        foreach (var item in list)
                        {
                            switch (Convert.ToInt32(item))
                            {
                                case (int)RoleType.SaleOnl:
                                case (int)RoleType.SaleTour:
                                case (int)RoleType.SaleKd:
                                    {
                                        searchModel.SalerPermission += current_user.UserUnderList;
                                    }
                                    break;
                                case (int)RoleType.TPDHKS:
                                case (int)RoleType.TPDHVe:
                                case (int)RoleType.TPDHTour:
                                case (int)RoleType.TPKS:
                                case (int)RoleType.TPTour:
                                case (int)RoleType.TPVe:
                                case (int)RoleType.DHPQ:
                                case (int)RoleType.DHTour:
                                case (int)RoleType.DHVe:
                                    {
                                        searchModel.SalerPermission += current_user.UserUnderList;
                                    }
                                    break;
                                case (int)RoleType.Admin:
                                case (int)RoleType.KT:
                                case (int)RoleType.GDHN:
                                case (int)RoleType.GDHPQ:
                                case (int)RoleType.GD:
                                case (int)RoleType.PhoTPKeToan:
                                case (int)RoleType.KeToanTruong:
                                    {
                                        searchModel.SalerPermission = null;
                                    }
                                    break;
                            }
                        }
                        var model = new GenericViewModel<SearchReportDepartmentClient>();
                        model = await _DepartmentRepository.GetRevenueByClient(searchModel);

                        return PartialView(model);
                    }

                }
                return PartialView();
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SearchReportDepartmentClient - ReportDepartmentController: " + ex);
                return PartialView();
            }
          

        }
        public async Task<IActionResult> GetListDetailRevenueByDepartment(ReportDepartmentViewModel searchModel)
        {
            try
            {
                ViewBag.ServiceType = searchModel.ServiceType;
                var current_user = _ManagementUser.GetCurrentUser();
                if (current_user != null)
                {
                    if (current_user.Role != "")
                    {
                        var list = current_user.Role.Split(',');
                        foreach (var item in list)
                        {
                            switch (Convert.ToInt32(item))
                            {
                                case (int)RoleType.SaleOnl:
                                case (int)RoleType.SaleTour:
                                case (int)RoleType.SaleKd:
                                    {
                                        searchModel.SalerPermission += current_user.UserUnderList;
                                    }
                                    break;
                                case (int)RoleType.TPDHKS:
                                case (int)RoleType.TPDHVe:
                                case (int)RoleType.TPDHTour:
                                case (int)RoleType.TPKS:
                                case (int)RoleType.TPTour:
                                case (int)RoleType.TPVe:
                                case (int)RoleType.DHPQ:
                                case (int)RoleType.DHTour:
                                case (int)RoleType.DHVe:
                                    {
                                        searchModel.SalerPermission += current_user.UserUnderList;
                                    }
                                    break;
                                case (int)RoleType.Admin:
                                case (int)RoleType.KT:
                                case (int)RoleType.GDHN:
                                case (int)RoleType.GDHPQ:
                                case (int)RoleType.GD:
                                case (int)RoleType.PhoTPKeToan:
                                case (int)RoleType.KeToanTruong:
                                    {
                                        searchModel.SalerPermission = null;
                                    }
                                    break;
                            }
                        }
                        var data = new List<DetailRevenueByDepartmentViewModel>();
                        var model = new GenericViewModel<DetailRevenueByDepartmentViewModel>();
                        model = await _DepartmentRepository.GetListDetailRevenueByDepartment(searchModel);
                        if (model.ListData != null)
                        {
                            var data_ks = model.ListData.Where(s => s.ParentDepartmentId == 16).ToList();
                            if (data_ks != null && data_ks.Count > 0)
                            {
                                data.AddRange(data_ks);
                                foreach (var item in data_ks)
                                {
                                    model.ListData.Remove(item);
                                }

                            }

                            var data_DHLenDon = model.ListData.Where(s => s.DepartmentId == 36).ToList();
                            if (data_DHLenDon != null && data_ks.Count > 0)
                            {
                                data.AddRange(data_DHLenDon);
                                foreach (var item in data_DHLenDon)
                                {
                                    model.ListData.Remove(item);
                                }
                            }

                            var data_LH1 = model.ListData.Where(s => s.DepartmentId == 14).ToList();
                            if (data_LH1 != null && data_LH1.Count > 0)
                            {
                                data.AddRange(data_LH1);
                                foreach (var item in data_LH1)
                                {
                                    model.ListData.Remove(item);
                                }
                            }

                            var data_LH2 = model.ListData.Where(s => s.DepartmentId == 47).ToList();
                            if (data_LH2 != null && data_LH2.Count > 0)
                            {
                                data.AddRange(data_LH2);
                                foreach (var item in data_LH2)
                                {
                                    model.ListData.Remove(item);
                                }
                            }

                            var data_LH3 = model.ListData.Where(s => s.DepartmentId == 48).ToList();
                            if (data_LH3 != null && data_LH3.Count > 0)
                            {
                                data.AddRange(data_LH3);
                                foreach (var item in data_LH3)
                                {
                                    model.ListData.Remove(item);
                                }
                            }

                            var data_Inbound = model.ListData.Where(s => s.DepartmentId == 38).ToList();
                            if (data_Inbound != null && data_Inbound.Count > 0)
                            {
                                data.AddRange(data_Inbound);
                                foreach (var item in data_Inbound)
                                {
                                    model.ListData.Remove(item);
                                }
                            }

                            var data_Sk = model.ListData.Where(s => s.DepartmentId == 50).ToList();
                            if (data_Sk != null && data_Sk.Count > 0)
                            {
                                data.AddRange(data_Sk);
                                foreach (var item in data_Sk)
                                {
                                    model.ListData.Remove(item);
                                }
                            }

                            var data_SG_LH1 = model.ListData.Where(s => s.DepartmentId == 20).ToList();
                            if (data_SG_LH1 != null && data_SG_LH1.Count > 0)
                            {
                                data.AddRange(data_SG_LH1);
                                foreach (var item in data_SG_LH1)
                                {
                                    model.ListData.Remove(item);
                                }
                            }

                            var data_SG_LH3 = model.ListData.Where(s => s.DepartmentId == 51).ToList();
                            if (data_SG_LH3 != null && data_SG_LH3.Count > 0)
                            {
                                data.AddRange(data_SG_LH3);
                                foreach (var item in data_SG_LH3)
                                {
                                    model.ListData.Remove(item);
                                }
                            }
                            data.AddRange(model.ListData);
                        }

                        model.ListData = data;
                        return PartialView(model);
                    }

                }
                return PartialView();
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetListDetailRevenueByDepartment - ReportDepartmentController: " + ex);
                return PartialView();
            }
       

        }
        public async Task<IActionResult> GetListDetailRevenueByDepartmentsaler(ReportDepartmentViewModel searchModel)
        {
            try
            {
                ViewBag.ServiceType = searchModel.ServiceType;
                var current_user = _ManagementUser.GetCurrentUser();
                if (current_user != null)
                {
                    if (current_user.Role != "")
                    {
                        var list = current_user.Role.Split(',');
                        foreach (var item in list)
                        {
                            switch (Convert.ToInt32(item))
                            {
                                case (int)RoleType.SaleOnl:
                                case (int)RoleType.SaleTour:
                                case (int)RoleType.SaleKd:
                                    {
                                        searchModel.SalerPermission += current_user.UserUnderList;
                                       
                                    }
                                    break;
                                case (int)RoleType.TPDHKS:
                                case (int)RoleType.TPDHVe:
                                case (int)RoleType.TPDHTour:
                                case (int)RoleType.TPKS:
                                case (int)RoleType.TPTour:
                                case (int)RoleType.TPVe:
                                case (int)RoleType.DHPQ:
                                case (int)RoleType.DHTour:
                                case (int)RoleType.DHVe:
                                    {
                                        searchModel.SalerPermission += current_user.UserUnderList;
                                    }
                                    break;
                                case (int)RoleType.Admin:
                                case (int)RoleType.KT:
                                case (int)RoleType.GDHN:
                                case (int)RoleType.GDHPQ:
                                case (int)RoleType.GD:
                                case (int)RoleType.PhoTPKeToan:
                                case (int)RoleType.KeToanTruong:
                                    {
                                        searchModel.SalerPermission = null;
                                    }
                                    break;
                            }
                        }
                        var model = new GenericViewModel<ListDetailRevenueByDepartmentViewModel>();
                        model = await _DepartmentRepository.GetListDetailRevenueBySaler(searchModel);

                        return PartialView(model);
                    }

                }
                return PartialView();
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetListDetailRevenueByDepartmentsaler - ReportDepartmentController: " + ex);
                return PartialView();
            }


        }
        public async Task<IActionResult> GetListDetailRevenueByDepartmentSupplier(ReportDepartmentViewModel searchModel)
        {
            try
            {
                ViewBag.ServiceType = searchModel.ServiceType;
                var current_user = _ManagementUser.GetCurrentUser();
                if (current_user != null)
                {
                    if (current_user.Role != "")
                    {
                        var list = current_user.Role.Split(',');
                        foreach (var item in list)
                        {
                            switch (Convert.ToInt32(item))
                            {
                                case (int)RoleType.SaleOnl:
                                case (int)RoleType.SaleTour:
                                case (int)RoleType.SaleKd:
                                    {
                                        searchModel.SalerPermission += current_user.UserUnderList;
                                    }
                                    break;
                                case (int)RoleType.TPDHKS:
                                case (int)RoleType.TPDHVe:
                                case (int)RoleType.TPDHTour:
                                case (int)RoleType.TPKS:
                                case (int)RoleType.TPTour:
                                case (int)RoleType.TPVe:
                                case (int)RoleType.DHPQ:
                                case (int)RoleType.DHTour:
                                case (int)RoleType.DHVe:
                                    {
                                        searchModel.SalerPermission += current_user.UserUnderList;
                                    }
                                    break;
                                case (int)RoleType.Admin:
                                case (int)RoleType.KT:
                                case (int)RoleType.GDHN:
                                case (int)RoleType.GDHPQ:
                                case (int)RoleType.GD:
                                case (int)RoleType.PhoTPKeToan:
                                case (int)RoleType.KeToanTruong:
                                    {
                                        searchModel.SalerPermission = null;
                                    }
                                    break;
                            }
                        }
                        var model = new GenericViewModel<DetailRevenueByDepartmentViewModel>();
                        model = await _DepartmentRepository.GetListDetailRevenueBySupplier(searchModel);

                        return PartialView(model);
                    }

                }
                return PartialView();
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetListDetailRevenueByDepartmentSupplier - ReportDepartmentController: " + ex);
                return PartialView();
            }


        }
        public async Task<IActionResult> GetListDetailRevenueByDepartmentClient(ReportDepartmentViewModel searchModel)
        {
            try
            {
                ViewBag.ServiceType = searchModel.ServiceType;
                var current_user = _ManagementUser.GetCurrentUser();
                if (current_user != null)
                {
                    if (current_user.Role != "")
                    {
                        var list = current_user.Role.Split(',');
                        foreach (var item in list)
                        {
                            switch (Convert.ToInt32(item))
                            {
                                case (int)RoleType.SaleOnl:
                                case (int)RoleType.SaleTour:
                                case (int)RoleType.SaleKd:
                                    {
                                        searchModel.SalerPermission += current_user.UserUnderList;
                                    }
                                    break;
                                case (int)RoleType.TPDHKS:
                                case (int)RoleType.TPDHVe:
                                case (int)RoleType.TPDHTour:
                                case (int)RoleType.TPKS:
                                case (int)RoleType.TPTour:
                                case (int)RoleType.TPVe:
                                case (int)RoleType.DHPQ:
                                case (int)RoleType.DHTour:
                                case (int)RoleType.DHVe:
                                    {
                                        searchModel.SalerPermission += current_user.UserUnderList;
                                    }
                                    break;
                                case (int)RoleType.Admin:
                                case (int)RoleType.KT:
                                case (int)RoleType.GDHN:
                                case (int)RoleType.GDHPQ:
                                case (int)RoleType.GD:
                                case (int)RoleType.PhoTPKeToan:
                                case (int)RoleType.KeToanTruong:
                                    {
                                        searchModel.SalerPermission = null;
                                    }
                                    break;
                            }
                        }
                        var model = new GenericViewModel<DetailRevenueByDepartmentViewModel>();
                        model = await _DepartmentRepository.GetListDetailRevenueByClient(searchModel);

                        return PartialView(model);
                    }

                }
                return PartialView();
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetListDetailRevenueByDepartmentClient - ReportDepartmentController: " + ex);
                return PartialView();
            }
        

        }

        public async Task<IActionResult> SearchReportDepartmentListOrder(ReportDepartmentViewModel searchModel)
        {
            try
            {
                ViewBag.DepartmentId = searchModel.DepartmentId;
                ViewBag.SalerId = searchModel.SalerId;
                if (searchModel.HINHTHUCTT != null)
                {
                    var listHINHTHUCTT = searchModel.HINHTHUCTT.Split(',');
                    searchModel.PermisionType = Convert.ToInt32(listHINHTHUCTT[0]);
                    searchModel.PaymentStatus = Convert.ToInt32(listHINHTHUCTT[1]);
                }
                var current_user = _ManagementUser.GetCurrentUser();
                if (current_user != null)
                {
                    if (current_user.Role != "")
                    {
                        var list = current_user.Role.Split(',');
                        foreach (var item in list)
                        {
                            switch (Convert.ToInt32(item))
                            {
                                case (int)RoleType.SaleOnl:
                                case (int)RoleType.SaleTour:
                                case (int)RoleType.SaleKd:
                                    {
                                        searchModel.SalerPermission += current_user.UserUnderList;
                                       
                                    }
                                    break;
                                case (int)RoleType.TPDHKS:
                                case (int)RoleType.TPDHVe:
                                case (int)RoleType.TPDHTour:
                                case (int)RoleType.TPKS:
                                case (int)RoleType.TPTour:
                                case (int)RoleType.TPVe:
                                case (int)RoleType.DHPQ:
                                case (int)RoleType.DHTour:
                                case (int)RoleType.DHVe:
                                    {
                                        searchModel.SalerPermission += current_user.UserUnderList;
                                    }
                                    break;
                                case (int)RoleType.Admin:
                                case (int)RoleType.KT:
                                case (int)RoleType.GDHN:
                                case (int)RoleType.GDHPQ:
                                case (int)RoleType.GD:
                                case (int)RoleType.PhoTPKeToan:
                                case (int)RoleType.KeToanTruong:
                                    {
                                        searchModel.SalerPermission = null;
                                    }
                                    break;
                            }
                        }
                        var model = new GenericViewModel<OrderViewModel>();
                        model = await _DepartmentRepository.GetListOrder(searchModel);

                        return PartialView(model);
                    }

                }
                return PartialView();
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SearchReportDepartmentListOrrder - ReportDepartmentController: " + ex);
                return PartialView();
            }


        }

        [HttpPost]
        public async Task<IActionResult> ExportExcel(ReportDepartmentViewModel searchModel)
        {
            try
            {
                string _UploadFolder = @"Template\Export";
                string _UploadDirectory = Path.Combine(_WebHostEnvironment.WebRootPath, _UploadFolder);

                var current_user = _ManagementUser.GetCurrentUser();

                if (current_user != null)
                {
                    if (current_user.Role != "")
                    {
                        var list = current_user.Role.Split(',');
                        foreach (var item in list)
                        {
                            switch (Convert.ToInt32(item))
                            {
                                case (int)RoleType.SaleOnl:
                                case (int)RoleType.SaleTour:
                                case (int)RoleType.SaleKd:
                                    {
                                        searchModel.SalerPermission += current_user.UserUnderList;
                                    }
                                    break;
                                case (int)RoleType.TPDHKS:
                                case (int)RoleType.TPDHVe:
                                case (int)RoleType.TPDHTour:
                                case (int)RoleType.TPKS:
                                case (int)RoleType.TPTour:
                                case (int)RoleType.TPVe:
                                case (int)RoleType.DHPQ:
                                case (int)RoleType.DHTour:
                                case (int)RoleType.DHVe:
                                case (int)RoleType.GDHN:
                                case (int)RoleType.GDHPQ:
                                    {
                                        searchModel.SalerPermission += current_user.UserUnderList;
                                    }
                                    break;
                                case (int)RoleType.Admin:
                                case (int)RoleType.KT:
                                case (int)RoleType.PhoTPKeToan:
                                case (int)RoleType.KeToanTruong:
                                case (int)RoleType.GD:
                                    {
                                        searchModel.SalerPermission = null;
                                    }
                                    break;
                            }
                        }

                    }

                }
                string _FileName_Base = "Doanh thu tổng theo phòng ban bán hàng";
                if (searchModel.Type == 1)
                {
                    switch (searchModel.DepartmentType)
                    {
                        case (int)DepartmentType.RevenueByDepartment:
                            {
                                _FileName_Base = "Doanh thu tổng theo phòng ban bán hàng";
                            }
                            break;
                        case (int)DepartmentType.RevenueByDepartmentsaler:
                            {
                                _FileName_Base = "Doanh thu tổng theo nhân viên bán hàng";
                            }
                            break;
                        case (int)DepartmentType.RevenueByDepartmentSupplier:
                            {
                                _FileName_Base = "Doanh thu tổng theo nhà cung cấp";
                            }
                            break;
                        case (int)DepartmentType.RevenueByDepartmentClient:
                            {
                                _FileName_Base = "Doanh thu tổng theo khách hàng";
                            }
                            break;
                    }
                }
                else
                {
                    switch (searchModel.DepartmentType)
                    {
                        case (int)DepartmentType.RevenueByDepartment:
                            {
                                _FileName_Base = "Doanh thu chi tiết dịch vụ theo phòng ban bán hàng";
                            }
                            break;
                        case (int)DepartmentType.RevenueByDepartmentsaler:
                            {
                                _FileName_Base = "Doanh thu chi tiết dịch vụ theo nhân viên bán hàng";
                            }
                            break;
                        case (int)DepartmentType.RevenueByDepartmentSupplier:
                            {
                                _FileName_Base = "Doanh thu chi tiết dịch vụ theo nhà cung cấp";
                            }
                            break;
                        case (int)DepartmentType.RevenueByDepartmentClient:
                            {
                                _FileName_Base = "Doanh thu chi tiết dịch vụ theo khách hàng";
                            }
                            break;
                    }
                }
                string _FileName = StringHelpers.GenFileName(_FileName_Base, current_user.Id, "xlsx");

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
                LogHelper.InsertLogTelegram("ExportExcel - ReportDepartmentController: " + ex);
                return new JsonResult(new
                {
                    isSuccess = false,
                    message = ex.Message.ToString()
                });
            }
        }
        public async Task<IActionResult> ExportExcelListOrrder(ReportDepartmentViewModel searchModel)
        {
            try
            {
                string _UploadFolder = @"Template\Export";
                string _UploadDirectory = Path.Combine(_WebHostEnvironment.WebRootPath, _UploadFolder);
                var date = DateTime.Now;
                var current_user = _ManagementUser.GetCurrentUser();
                int _UserId = 0;
                if (HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) != null)
                {
                    _UserId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                string _FileName = StringHelpers.GenFileName("Danh sách đơn hàng", _UserId, "xlsx");

                if (current_user != null)
                {
                    if (current_user.Role != "")
                    {
                        var list = current_user.Role.Split(',');
                        foreach (var item in list)
                        {
                            switch (Convert.ToInt32(item))
                            {
                                case (int)RoleType.SaleOnl:
                                case (int)RoleType.SaleTour:
                                case (int)RoleType.SaleKd:
                                    {
                                        searchModel.SalerPermission += current_user.UserUnderList;
                                    }
                                    break;
                                case (int)RoleType.TPDHKS:
                                case (int)RoleType.TPDHVe:
                                case (int)RoleType.TPDHTour:
                                case (int)RoleType.TPKS:
                                case (int)RoleType.TPTour:
                                case (int)RoleType.TPVe:
                                case (int)RoleType.DHPQ:
                                case (int)RoleType.DHTour:
                                case (int)RoleType.DHVe:
                                case (int)RoleType.GDHN:
                                case (int)RoleType.GDHPQ:
                                    {
                                        searchModel.SalerPermission += current_user.UserUnderList;
                                    }
                                    break;
                                case (int)RoleType.Admin:
                                case (int)RoleType.KT:
                                case (int)RoleType.PhoTPKeToan:
                                case (int)RoleType.KeToanTruong:
                                case (int)RoleType.GD:
                                    {
                                        searchModel.SalerPermission = null;
                                    }
                                    break;
                            }
                        }

                    }

                }
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

                var rsPath = await _DepartmentRepository.ExportDepositListOrder(searchModel, FilePath);

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
        public async Task<string> GetSuppliersSuggest(long id,long type)
        {
            try
            {
                switch (type)
                {
                    case (int)ServicesType.VINHotelRent:
                        {
                            var supplierList = await _hotelBookingRepositories.GetHotelBookingOptionalListByHotelBookingId(id);
                            var hotel = await _hotelBookingRepositories.GetHotelBookingById(id);
                            var extra_package = await _hotelBookingRoomExtraPackageRepository.GetByBookingID(id);
                            var suggestionlist = supplierList.Select(s => new SupplierViewModel
                            {
                                id = (int)s.SupplierId,
                                fullname = s.SupplierName,

                            }).ToList();

                            if (hotel != null && hotel.Count > 0 && extra_package != null && extra_package.Count > 0)
                            {
                                var hotellist = hotel.Select(s => new SupplierViewModel
                                {
                                    id = (int)s.SupplierId,
                                    fullname = s.SuplierName,

                                }).ToList();
                                suggestionlist.AddRange(hotellist);
                            }
                            suggestionlist = suggestionlist.GroupBy(s => s.id).Select(s => s.First()).ToList();
                            return JsonConvert.SerializeObject(suggestionlist);
                        }
                        break;
                    case (int)ServicesType.Other:
                        {
                            var supplierList =  _otherBookingRepository.GetOtherBookingPackagesOptionalByServiceId(id);
                        
                      
                            var suggestionlist = supplierList.Select(s => new SupplierViewModel
                            {
                                id = (int)s.SuplierId,
                                fullname = s.SupplierName,

                            }).ToList();

                            suggestionlist = suggestionlist.GroupBy(s => s.id).Select(s => s.First()).ToList();
                            return JsonConvert.SerializeObject(suggestionlist);
                        }
                        break;
                }
                return null;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetSuppliersSuggest - ReportDepartmentController: " + ex);
                return null;
            }
        }
        [HttpPost]
        public async Task<IActionResult> SearchOperatorReport(OperatorReportSearchModel searchModel)
        {
            try
            {
                int _UserId = 0;
                if (HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) != null)
                {
                    _UserId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                searchModel.SalerPermission = "" + _UserId;

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
                                        searchModel.SalerPermission = null;

                                        is_exceed_permission = true;
                                    }
                                    break;
                                default:
                                    {
                                        ViewBag.Model = new GenericViewModel<OperatorReportViewModel>();
                                        ViewBag.Sum = new SumOperatorReportViewModel();
                                        ViewBag.Department = new List<Department>();
                                        return View();
                                    }
                            }
                            if (is_exceed_permission) break;
                        }

                    }

                }


                var model = await _reportRepository.GetOperatorReport(searchModel, 1, 20000);
                ViewBag.Model = model;
                SumOperatorReportViewModel sum = new SumOperatorReportViewModel();
                if (model!=null &&model.ListData!=null && model.ListData.Count > 0)
                {
                    sum = new SumOperatorReportViewModel()
                    {
                        AMOUNT = model.ListData.Sum(x => x.Amount),
                        AmountPay = model.ListData.Sum(x => x.AmountPay != null ? (double)x.AmountPay : 0),
                        AmountRemain = model.ListData.Sum(x => x.AmountRemain != null ? (double)x.AmountRemain : x.Amount),
                        Comission = model.ListData.Sum(x => x.Comission != null ? (double)x.Comission : 0),
                        Price = model.ListData.Sum(x => x.Price != null ? (double)x.Price : 0),
                        PricePay = model.ListData.Sum(x => x.PricePay != null ? (double)x.PricePay : 0),
                        PriceRemain = model.ListData.Sum(x => x.PriceRemain != null ? (double)x.PriceRemain : (x.Price != null ? (double)x.Price : 0)),
                        Profit = model.ListData.Sum(x => x.Profit != null ? (double)x.Profit : 0),
                        Refund = model.ListData.Sum(x => x.Refund != null ? (double)x.Refund : 0),
                    };
                }
                else
                {
                    sum = await _reportRepository.GetSumOperatorReport(searchModel);
                }
                ViewBag.Sum = sum;
                var departments = await _DepartmentRepository.GetAll("");
                ViewBag.Department = departments.ToList();
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Search - OperatorReportController: " + ex);
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ExportOperatorExcel(OperatorReportSearchModel searchModel)
        {
            try
            {
                int _UserId = 0;
                if (HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) != null)
                {
                    _UserId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                searchModel.SalerPermission = "" + _UserId;

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
                                        searchModel.SalerPermission = null;

                                        is_exceed_permission = true;
                                    }
                                    break;
                                default:
                                    {
                                        return Ok(new
                                        {
                                            status = (int)ResponseType.FAILED,
                                            msg = "Xuất dữ liệu thất bại, bạn không có quyền để xuất báo cáo này",
                                            path = ""
                                        });
                                    }
                            }
                            if (is_exceed_permission) break;
                        }

                    }

                }
                string folder = @"\wwwroot\Template\Export\" + _UserId;
                string full_path = Directory.GetCurrentDirectory() + folder;
                string file_name = StringHelpers.GenFileName("Doanh thu theo Phát sinh chi tiết đơn hàng", current_user.Id, "xlsx");

                string _UploadDirectory = Path.Combine(_WebHostEnvironment.WebRootPath, folder);
                string file_path_combine = Path.Combine(_UploadDirectory, file_name);
                try
                {
                    System.IO.DirectoryInfo di = new DirectoryInfo(full_path);
                    if (!di.Exists)
                    {
                        Directory.CreateDirectory(full_path);
                    }
                    else
                    {
                        foreach (FileInfo file in di.GetFiles())
                        {
                            file.Delete();
                        }
                    }
                }
                catch
                {

                }


                var file_path = await _reportRepository.ExportOperatorExcel( await _reportRepository.GetOperatorReport(searchModel, 1, 20000), file_path_combine);

                return Ok(new
                {
                    status = (int)ResponseType.SUCCESS,
                    msg = "Xuất dữ liệu thành công",
                    path = file_path.Replace(@"\wwwroot", "")
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
        public async Task<IActionResult> SearchReportDepartmentBysaler(ReportDepartmentViewModel searchModel)
        {
            try
            {

                var current_user = _ManagementUser.GetCurrentUser();
                if (current_user != null)
                {
                    if (current_user.Role != "")
                    {
                        var list = current_user.Role.Split(',');
                        foreach (var item in list)
                        {
                            switch (Convert.ToInt32(item))
                            {
                                case (int)RoleType.SaleOnl:
                                case (int)RoleType.SaleTour:
                                case (int)RoleType.SaleKd:
                                    {
                                        searchModel.SalerPermission += current_user.UserUnderList;

                                    }
                                    break;
                                case (int)RoleType.TPDHKS:
                                case (int)RoleType.TPDHVe:
                                case (int)RoleType.TPDHTour:
                                case (int)RoleType.TPKS:
                                case (int)RoleType.TPTour:
                                case (int)RoleType.TPVe:
                                case (int)RoleType.DHPQ:
                                case (int)RoleType.DHTour:
                                case (int)RoleType.DHVe:
                                    {
                                        searchModel.SalerPermission += current_user.UserUnderList;
                                    }
                                    break;
                                case (int)RoleType.Admin:
                                case (int)RoleType.KT:
                                case (int)RoleType.GDHN:
                                case (int)RoleType.GDHPQ:
                                case (int)RoleType.GD:
                                case (int)RoleType.KeToanTruong:
                                case (int)RoleType.PhoTPKeToan:
                                    {
                                        searchModel.SalerPermission = null;
                                    }
                                    break;
                            }
                        }
                        var model = new GenericViewModel<ReportDepartmentBysaleViewModel>();
                        model = await _DepartmentRepository.GetDepartmentBysaler(searchModel);
                        return PartialView(model);
                    }

                }
                return PartialView();
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SearchReportDepartmentBysaler - ReportDepartmentController: " + ex);
                return PartialView();
            }


        }
        public async Task<IActionResult> DetailReportDepartmentBysaler(ReportDepartmentViewModel searchModel)
        {
            try
            {

                var current_user = _ManagementUser.GetCurrentUser();
                if (current_user != null)
                {
                    if (current_user.Role != "")
                    {
                        var list = current_user.Role.Split(',');
                        foreach (var item in list)
                        {
                            switch (Convert.ToInt32(item))
                            {
                                case (int)RoleType.SaleOnl:
                                case (int)RoleType.SaleTour:
                                case (int)RoleType.SaleKd:
                                    {
                                        searchModel.SalerPermission += current_user.UserUnderList;

                                    }
                                    break;
                                case (int)RoleType.TPDHKS:
                                case (int)RoleType.TPDHVe:
                                case (int)RoleType.TPDHTour:
                                case (int)RoleType.TPKS:
                                case (int)RoleType.TPTour:
                                case (int)RoleType.TPVe:
                                case (int)RoleType.DHPQ:
                                case (int)RoleType.DHTour:
                                case (int)RoleType.DHVe:
                                    {
                                        searchModel.SalerPermission += current_user.UserUnderList;
                                    }
                                    break;
                                case (int)RoleType.Admin:
                                case (int)RoleType.KT:
                                case (int)RoleType.GDHN:
                                case (int)RoleType.GDHPQ:
                                case (int)RoleType.GD:
                                    {
                                        searchModel.SalerPermission = null;
                                    }
                                    break;
                            }
                        }
                        var model = new GenericViewModel<DetailDepartmentBysaleViewModel>();
                        model = await _DepartmentRepository.DetailDepartmentBysale(searchModel);
                        ViewBag.saleid = searchModel.SalerId;
                        return PartialView(model);
                    }

                }
                return PartialView();
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SearchReportDepartmentBysaler - ReportDepartmentController: " + ex);
                return PartialView();
            }


        }
        public async Task<IActionResult> ExportExcelReportDepartmentBysaler(ReportDepartmentViewModel searchModel)
        {
            try
            {

                string _UploadFolder = @"Template\Export";
                string _UploadDirectory = Path.Combine(_WebHostEnvironment.WebRootPath, _UploadFolder);

                var current_user = _ManagementUser.GetCurrentUser();
                string _FileName = StringHelpers.GenFileName("Danh sách báo cáo phòng ban sale", current_user.Id, "xlsx");

                if (current_user != null)
                {
                    if (current_user.Role != "")
                    {
                        var list = current_user.Role.Split(',');
                        foreach (var item in list)
                        {
                            switch (Convert.ToInt32(item))
                            {
                                case (int)RoleType.SaleOnl:
                                case (int)RoleType.SaleTour:
                                case (int)RoleType.SaleKd:
                                    {
                                        searchModel.SalerPermission += current_user.UserUnderList;
                                    }
                                    break;
                                case (int)RoleType.TPDHKS:
                                case (int)RoleType.TPDHVe:
                                case (int)RoleType.TPDHTour:
                                case (int)RoleType.TPKS:
                                case (int)RoleType.TPTour:
                                case (int)RoleType.TPVe:
                                case (int)RoleType.DHPQ:
                                case (int)RoleType.DHTour:
                                case (int)RoleType.DHVe:
                                case (int)RoleType.GDHN:
                                case (int)RoleType.GDHPQ:
                                    {
                                        searchModel.SalerPermission += current_user.UserUnderList;
                                    }
                                    break;
                                case (int)RoleType.Admin:
                                case (int)RoleType.KT:

                                case (int)RoleType.GD:
                                    {
                                        searchModel.SalerPermission = null;
                                    }
                                    break;
                            }
                        }

                    }

                }
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

                var rsPath = await _DepartmentRepository.ExportDepartmentBysaler(searchModel, FilePath);

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
        public async Task<IActionResult> ExportExcelDetailDepartmentBysaler(ReportDepartmentViewModel searchModel)
        {
            try
            {
                string _UploadFolder = @"Template\Export";
                string _UploadDirectory = Path.Combine(_WebHostEnvironment.WebRootPath, _UploadFolder);

                var current_user = _ManagementUser.GetCurrentUser();
                string _FileName = StringHelpers.GenFileName("Danh sách chi tiết báo cáo phòng ban sale", current_user.Id, "xlsx");

                if (current_user != null)
                {
                    if (current_user.Role != "")
                    {
                        var list = current_user.Role.Split(',');
                        foreach (var item in list)
                        {
                            switch (Convert.ToInt32(item))
                            {
                                case (int)RoleType.SaleOnl:
                                case (int)RoleType.SaleTour:
                                case (int)RoleType.SaleKd:
                                    {
                                        searchModel.SalerPermission += current_user.UserUnderList;
                                    }
                                    break;
                                case (int)RoleType.TPDHKS:
                                case (int)RoleType.TPDHVe:
                                case (int)RoleType.TPDHTour:
                                case (int)RoleType.TPKS:
                                case (int)RoleType.TPTour:
                                case (int)RoleType.TPVe:
                                case (int)RoleType.DHPQ:
                                case (int)RoleType.DHTour:
                                case (int)RoleType.DHVe:
                                case (int)RoleType.GDHN:
                                case (int)RoleType.GDHPQ:
                                    {
                                        searchModel.SalerPermission += current_user.UserUnderList;
                                    }
                                    break;
                                case (int)RoleType.Admin:
                                case (int)RoleType.KT:

                                case (int)RoleType.GD:
                                    {
                                        searchModel.SalerPermission = null;
                                    }
                                    break;
                            }
                        }

                    }

                }
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

                var rsPath = await _DepartmentRepository.ExportDetailDepartmentBysaler(searchModel, FilePath);

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
