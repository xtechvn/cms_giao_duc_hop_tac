using Aspose.Cells;
using DAL;
using DAL.Funding;
using DAL.StoreProcedure;
using Entities.ConfigModels;
using Entities.Models;
using Entities.ViewModels;
using Entities.ViewModels.Funding;
using Entities.ViewModels.HotelBookingRoom;
using Entities.ViewModels.SetServices;
using Entities.ViewModels.Tour;
using Microsoft.Extensions.Options;
using Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using Utilities;
using Utilities.Contants;

namespace Repositories.Repositories
{
    public class PaymentRequestRepository : IPaymentRequestRepository
    {
        private readonly UserDAL userDAL;
        private readonly PaymentRequestDAL paymentRequestDAL;
        private readonly PaymentVoucherDAL paymentVoucherDAL;
        private readonly FlyBookingDetailDAL flyBookingDetailDAL;
        private readonly OrderDAL orderDAL;
        private readonly ITourPackagesOptionalRepository _tourPackagesOptionalRepository;
        private readonly IOtherBookingRepository _otherBookingRepository;
        private readonly IHotelBookingRepositories _hotelBookingRepositories;
        private readonly IFlyBookingDetailRepository _flyBookingDetailRepository;

        public PaymentRequestRepository(IOptions<DataBaseConfig> dataBaseConfig, ITourPackagesOptionalRepository tourPackagesOptionalRepository
            , IOtherBookingRepository otherBookingRepository, IHotelBookingRepositories hotelBookingRepositories,
            IFlyBookingDetailRepository flyBookingDetailRepository)
        {
            userDAL = new UserDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            paymentRequestDAL = new PaymentRequestDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            paymentVoucherDAL = new PaymentVoucherDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            flyBookingDetailDAL = new FlyBookingDetailDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            orderDAL = new OrderDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            _tourPackagesOptionalRepository = tourPackagesOptionalRepository;
            _otherBookingRepository = otherBookingRepository;
            _hotelBookingRepositories = hotelBookingRepositories;
            _flyBookingDetailRepository = flyBookingDetailRepository;
        }

        public int CreatePaymentRequest(PaymentRequestViewModel model)
        {
            var paymentRequest = paymentRequestDAL.GetByPaymentCode(model.PaymentCode);
            if (paymentRequest != null)
                return -2;
            return paymentRequestDAL.CreatePaymentRequest(model);
        }

        public string ExportPaymentRequest(PaymentRequestSearchModel searchModel, string FilePath)
        {
            var pathResult = string.Empty;
            try
            {
                var listPaymentRequests = GetPaymentRequests(searchModel, out long total, 1, -1);

                if (listPaymentRequests.FirstOrDefault() != null)
                    total = listPaymentRequests.FirstOrDefault().TotalRow;
                var listServiceFlyStr = new List<string>();
                foreach (var item in listPaymentRequests)
                {
                    if (!string.IsNullOrEmpty(item.PaymentVoucherNo))
                        item.PaymentVoucherNo = item.PaymentVoucherNo.Replace(",", "").Trim();
                    if (string.IsNullOrEmpty(item.ListServiceCode)) continue;
                    var listServiceCode = item.ListServiceCode.Split(",");
                    foreach (string serviceCode in listServiceCode)
                    {
                        listServiceFlyStr.Add(serviceCode);
                    }
                }
                var listServiceFly = flyBookingDetailDAL.GetByServiceCodes(listServiceFlyStr);
                var listPaymentVoucher = paymentVoucherDAL.GetByPaymentCodes(listPaymentRequests.Where(n => !string.IsNullOrEmpty(n.PaymentVoucherNo)).Select(n => n.PaymentVoucherNo.Replace(",", " ")).ToList());
                var listOrder = orderDAL.GetByOrderNos(listPaymentRequests.Where(n => !string.IsNullOrEmpty(n.OrderNo)).Select(n => n.OrderNo).ToList());
                foreach (var item in listPaymentRequests)
                {
                    if (!string.IsNullOrEmpty(item.PaymentVoucherNo))
                    {
                        var paymentVoucher = listPaymentVoucher.FirstOrDefault(n => item.PaymentVoucherNo.Replace(",", "").Contains(n.PaymentCode));
                        item.PaymentVoucherId = paymentVoucher != null ? paymentVoucher.Id : 0;
                    }
                    if (!string.IsNullOrEmpty(item.OrderNo))
                    {
                        var order = listOrder.FirstOrDefault(n => n.OrderNo == item.OrderNo);
                        item.OrderId = order != null ? (int)order.OrderId : 0;
                    }
                    if (string.IsNullOrEmpty(item.ListServiceCode)) continue;
                    var listServiceCode = item.ListServiceCode.Split(",");
                    var listServiceId = item.ListServiceId.Split(",");
                    item.ListServiceCodeAndType = new List<CountStatus>();
                    int index = 0;
                    foreach (var serviceCode in listServiceCode)
                    {
                        CountStatus countStatus = new CountStatus();
                        var flyService = listServiceFly.FirstOrDefault(n => n.ServiceCode == serviceCode);
                        countStatus.DataNo = serviceCode;
                        countStatus.DataId = int.Parse(listServiceId[index]);
                        if (flyService != null)
                        {
                            countStatus.DataIdFly = flyService.GroupBookingId.Replace(",", "_");
                        }
                        if (serviceCode.Contains("TOUR"))
                            countStatus.ServiceType = (int)ServiceType.Tour;
                        if (serviceCode.Contains("FLIGHT"))
                            countStatus.ServiceType = (int)ServiceType.PRODUCT_FLY_TICKET;
                        if (serviceCode.Contains("HOTEL"))
                            countStatus.ServiceType = (int)ServiceType.BOOK_HOTEL_ROOM;
                        if (serviceCode.Contains("OTHER"))
                            countStatus.ServiceType = (int)ServiceType.Other;
                        item.ListServiceCodeAndType.Add(countStatus);
                        index++;
                    }
                }

                if (listPaymentRequests != null && listPaymentRequests.Count > 0)
                {
                    Workbook wb = new Workbook();
                    Worksheet ws = wb.Worksheets[0];
                    ws.Name = "Danh sách phiếu yêu cầu chi";
                    Cells cell = ws.Cells;

                    var range = ws.Cells.CreateRange(0, 0, 1, 1);
                    StyleFlag st = new StyleFlag();
                    st.All = true;
                    Style style = ws.Cells["A1"].GetStyle();

                    #region Header
                    range = cell.CreateRange(0, 0, 1, 15);
                    style = ws.Cells["A1"].GetStyle();
                    style.Font.IsBold = true;
                    style.IsTextWrapped = true;
                    style.ForegroundColor = Color.FromArgb(33, 88, 103);
                    style.BackgroundColor = Color.FromArgb(33, 88, 103);
                    style.Pattern = BackgroundType.Solid;
                    style.Font.Color = Color.White;
                    style.VerticalAlignment = TextAlignmentType.Center;
                    style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                    style.Borders[BorderType.TopBorder].Color = Color.Black;
                    style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
                    style.Borders[BorderType.BottomBorder].Color = Color.Black;
                    style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                    style.Borders[BorderType.LeftBorder].Color = Color.Black;
                    style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                    style.Borders[BorderType.RightBorder].Color = Color.Black;
                    range.ApplyStyle(style, st);

                    // Set column width
                    cell.SetColumnWidth(0, 8);
                    cell.SetColumnWidth(1, 20);
                    cell.SetColumnWidth(2, 40);
                    cell.SetColumnWidth(3, 20);
                    cell.SetColumnWidth(4, 20);
                    cell.SetColumnWidth(5, 40);
                    cell.SetColumnWidth(6, 40);
                    cell.SetColumnWidth(7, 25);
                    cell.SetColumnWidth(8, 25);
                    cell.SetColumnWidth(9, 20);
                    cell.SetColumnWidth(10, 50);
                    cell.SetColumnWidth(11, 25);
                    cell.SetColumnWidth(12, 25);
                    cell.SetColumnWidth(13, 25);
                    cell.SetColumnWidth(14, 25);
                    cell.SetColumnWidth(15, 25);
                    // Set header value
                    ws.Cells["A1"].PutValue("STT");
                    ws.Cells["B1"].PutValue("Mã phiếu");
                    ws.Cells["C1"].PutValue("Loại nghiệp vụ");
                    ws.Cells["D1"].PutValue("Hình thức");
                    ws.Cells["E1"].PutValue("Mã nhà cung cấp");
                    ws.Cells["F1"].PutValue("Nhà cung cấp/Khách hàng");
                    ws.Cells["G1"].PutValue("Số tiền");
                    ws.Cells["H1"].PutValue("Thời hạn thanh toán");
                    ws.Cells["I1"].PutValue("Trạng thái");
                    ws.Cells["J1"].PutValue("Mã dịch vụ");
                    ws.Cells["K1"].PutValue("Mã đơn");
                    ws.Cells["L1"].PutValue("Nội dung");
                    ws.Cells["M1"].PutValue("Ngày tạo");
                    ws.Cells["N1"].PutValue("Người tạo");
                    ws.Cells["O1"].PutValue("Ngày duyệt");
                    ws.Cells["P1"].PutValue("Người duyệt");
                    #endregion

                    #region Body

                    range = cell.CreateRange(1, 0, listPaymentRequests.Count, 15);
                    style = ws.Cells["A2"].GetStyle();
                    style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                    style.Borders[BorderType.TopBorder].Color = Color.Black;
                    style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
                    style.Borders[BorderType.BottomBorder].Color = Color.Black;
                    style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                    style.Borders[BorderType.LeftBorder].Color = Color.Black;
                    style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                    style.Borders[BorderType.RightBorder].Color = Color.Black;
                    style.VerticalAlignment = TextAlignmentType.Center;
                    range.ApplyStyle(style, st);

                    Style alignCenterStyle = ws.Cells["A2"].GetStyle();
                    alignCenterStyle.HorizontalAlignment = TextAlignmentType.Center;

                    Style numberStyle = ws.Cells["J2"].GetStyle();
                    numberStyle.Number = 3;
                    numberStyle.HorizontalAlignment = TextAlignmentType.Right;
                    numberStyle.VerticalAlignment = TextAlignmentType.Center;

                    int RowIndex = 1;

                    foreach (var item in listPaymentRequests)
                    {
                        RowIndex++;
                        ws.Cells["A" + RowIndex].PutValue(RowIndex - 1);
                        ws.Cells["A" + RowIndex].SetStyle(alignCenterStyle);
                        ws.Cells["B" + RowIndex].PutValue(item.PaymentCode);
                        ws.Cells["C" + RowIndex].PutValue(item.PaymentRequestType);
                        ws.Cells["D" + RowIndex].PutValue(item.PaymentTypeStr);
                        ws.Cells["E" + RowIndex].PutValue(item.SupplierId);
                        ws.Cells["F" + RowIndex].PutValue(!string.IsNullOrEmpty(item.SupplierName) ?
                            item.SupplierName : item.ClientName);
                        ws.Cells["G" + RowIndex].PutValue(item.Amount.ToString("N0"));
                        ws.Cells["G" + RowIndex].SetStyle(numberStyle);
                        ws.Cells["H" + RowIndex].PutValue(item.PaymentDate.Value.ToString("dd/MM/yyyy"));
                        ws.Cells["I" + RowIndex].PutValue(item.PaymentRequestStatus);
                        if (item.ListServiceCodeAndType != null)
                        {
                            foreach (var dataNo in item.ListServiceCodeAndType)
                            {
                                ws.Cells["J" + RowIndex].PutValue(dataNo.DataNo);
                            }
                        }
                        else
                        {
                            ws.Cells["J" + RowIndex].PutValue("");
                        }
                        ws.Cells["K" + RowIndex].PutValue(item.OrderNo);
                        ws.Cells["L" + RowIndex].PutValue(item.Note);
                        ws.Cells["M" + RowIndex].PutValue(item.CreatedDate.Value.ToString("dd/MM/yyyy"));
                        ws.Cells["N" + RowIndex].PutValue(item.UserName);
                        ws.Cells["O" + RowIndex].PutValue(item.CreatedDate.Value.ToString("dd/MM/yyyy"));
                        ws.Cells["P" + RowIndex].PutValue(item.UserVerifyName);
                    }

                    #endregion
                    wb.Save(FilePath);
                    pathResult = FilePath;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ExportPaymentRequest - PaymentRequestRepository: " + ex);
            }
            return pathResult;
        }

        public PaymentRequestViewModel GetById(int paymentRequestId)
        {
            try
            {
                var requestInfos = paymentRequestDAL.GetRequestDetail(paymentRequestId,
                    ProcedureConstants.sp_GetDetailPaymentRequest).ToList<PaymentRequestViewModel>();
                var requestInfo = requestInfos.FirstOrDefault();
                requestInfo.PaymentDateStr = DateUtil.DateToString(requestInfo.PaymentDate);
                requestInfo.RelateData = new List<PaymentRequestDetailViewModel>();
                if (requestInfo.Type == (int)PAYMENT_VOUCHER_TYPE.HOAN_TRA_KHACH_HANG)
                {
                    foreach (var item in requestInfos)
                    {
                        PaymentRequestDetailViewModel model = new PaymentRequestDetailViewModel();
                        item.CopyProperties(model);
                        model.OrderId = item.OrderId;
                        model.OrderNo = item.OrderNo;
                        model.Amount = item.Amount;
                        model.OrderAmount = item.OrderAmount;
                        model.OrderAmountPay = item.OrderAmountPay;
                        model.ServiceId = (int)item.ServiceId;
                        model.UserCreateFullName = item.UserCreateFullName;
                        model.DepartmentName = item.DepartmentName;
                        model.ServiceId = (int)item.ServiceId;
                        model.BankIdName = item.BankIdName;
                        model.AccountNumber = item.AccountNumber;
                        model.ServiceId = (int)item.ServiceId;
                        var serviceInfo = paymentRequestDAL.GetDetailServiceById(item.ServiceId, item.ServiceType,
                          ProcedureConstants.Sp_GetDetailServiceById).ToList<PaymentRequestViewModel>().FirstOrDefault();
                        model.ServiceAmount = serviceInfo != null ? serviceInfo.Amount : 0;
                        model.ServicePrice = serviceInfo != null ? serviceInfo.Price : 0;
                        requestInfo.RelateData.Add(model);
                    }
                }
                if (requestInfo.Type == (int)PAYMENT_VOUCHER_TYPE.THANH_TOAN_DICH_VU || requestInfo.Type == (int)PAYMENT_VOUCHER_TYPE.CHI_PHI_MARKETING)
                {
                    var requestServiceDetails = paymentRequestDAL.GetRequestDetail(paymentRequestId,
                        ProcedureConstants.sp_GetAllServiceByRequestiD).ToList<PaymentRequestViewModel>();
                    foreach (var item in requestServiceDetails)
                    {
                        PaymentRequestDetailViewModel model = new PaymentRequestDetailViewModel();
                        item.CopyProperties(model);
                        model.OrderId = item.OrderId;
                        model.ServiceId = (int)item.ServiceId;
                        var serviceInfo = paymentRequestDAL.GetDetailServiceById(item.ServiceId, item.ServiceType,
                        ProcedureConstants.Sp_GetDetailServiceById).ToList<PaymentRequestViewModel>().FirstOrDefault();
                        model.ServiceAmount = serviceInfo != null ? serviceInfo.Amount : 0;
                        double servicePrice = 0;
                        if (requestInfo.Type == (int)PAYMENT_VOUCHER_TYPE.THANH_TOAN_DICH_VU)
                        {
                            servicePrice = GetAmontRequestForSupplier(item.ServiceId, item.ServiceType, requestInfo.SupplierId.Value, serviceInfo.Price);
                        }
                        model.ServicePrice = servicePrice;
                        requestInfo.RelateData.Add(model);
                    }
                }
                //foreach (var item in requestInfo.RelateData)
                //{
                //    var serviceInfo = paymentRequestDAL.GetDetailServiceById(item.ServiceId, item.ServiceType,
                //        ProcedureConstants.sp_GetAllServiceByRequestiD).ToList<PaymentRequestViewModel>().FirstOrDefault();
                //    item.ServiceAmount = serviceInfo != null ? serviceInfo.Amount : 0;
                //    item.ServicePrice = serviceInfo != null ? serviceInfo.Price : 0;
                //}
                requestInfo.IsIncludeService = requestInfo.IsServiceIncluded == true ? 1 : 0;
                return requestInfo;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetById - PaymentRequestRepository: " + ex);
                return null;
            }
        }

        public PaymentRequest GetByRequestNo(string paymentRequestNo)
        {
            try
            {
                return paymentRequestDAL.GetByRequestNo(paymentRequestNo);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetByRequestNo - PaymentRequestRepository: " + ex);
                return null;
            }
        }

        public double GetAmontRequestForSupplier(long serviceId, int serviceType, long supplierId, double serviceAmount)
        {
            if (serviceType == (int)ServiceType.BOOK_HOTEL_ROOM_VIN)
            {
                var listHotelPackages = _hotelBookingRepositories.GetHotelBookingOptionalListByHotelBookingId(serviceId).Result;
                var listHotelExtraPackages = _hotelBookingRepositories.GetListHotelBookingRoomExtraPackagesHotelBookingId(serviceId).Result;
                foreach (var item in listHotelExtraPackages)
                {
                    item.Amount = item.UnitPrice;
                    item.TotalAmount = item.UnitPrice;
                    listHotelPackages.Add(item);
                }
                var listHotelPackageReturn = new List<HotelBookingsRoomOptionalViewModel>();
                foreach (var item in listHotelPackages)
                {
                    item.Amount = item.TotalAmount;
                    var getBySupplierId = listHotelPackageReturn.Where(n => n.SupplierId == item.SupplierId).ToList();
                    if (getBySupplierId.Count > 0)
                    {
                        foreach (var sup in listHotelPackageReturn)
                        {
                            if (sup.SupplierId == item.SupplierId)
                            {
                                sup.TotalAmount += item.TotalAmount;
                                sup.Amount += item.Amount;
                            }
                        }
                    }
                    else
                    {
                        listHotelPackageReturn.Add(item);
                    }
                }
                var service = listHotelPackageReturn.FirstOrDefault(n => n.SupplierId == supplierId);
                serviceAmount = service != null ? service.Amount : serviceAmount;
            }
            if (serviceType == (int)ServiceType.Other)
            {
                var listOtherPackage = _otherBookingRepository.GetOtherBookingPackagesOptionalByServiceId(serviceId);
                listOtherPackage = listOtherPackage.Where(n => n.SuplierId != 0).ToList();
                var listOtherPackageReturn = new List<OtherBookingPackagesOptionalViewModel>();
                foreach (var item in listOtherPackage)
                {
                    var getBySupplierId = listOtherPackageReturn.Where(n => n.SuplierId == item.SuplierId).ToList();
                    if (getBySupplierId.Count > 0)
                    {
                        foreach (var sup in listOtherPackageReturn)
                        {
                            if (sup.SuplierId == item.SuplierId)
                                sup.Amount += item.Amount;
                        }
                    }
                    else
                    {
                        listOtherPackageReturn.Add(item);
                    }
                }
                var service = listOtherPackageReturn.FirstOrDefault(n => n.SuplierId == supplierId);
                serviceAmount = service != null ? service.Amount : serviceAmount;
            }
            if (serviceType == (int)ServiceType.Tour)
            {
                var listTourPackage = _tourPackagesOptionalRepository.GetTourPackagesOptional(serviceId).Result;
                listTourPackage = listTourPackage.Where(n => n.SupplierId != 0).ToList();
                var listTourPackageReturn = new List<TourPackagesOptionalViewModel>();
                foreach (var item in listTourPackage)
                {
                    var getBySupplierId = listTourPackageReturn.Where(n => n.SupplierId == item.SupplierId).ToList();
                    if (getBySupplierId.Count > 0)
                    {
                        foreach (var sup in listTourPackageReturn)
                        {
                            if (sup.SupplierId == item.SupplierId)
                                sup.Amount += item.Amount;
                        }
                    }
                    else
                    {
                        listTourPackageReturn.Add(item);
                    }
                }
                var service = listTourPackageReturn.FirstOrDefault(n => n.SupplierId == supplierId);
                serviceAmount = service != null ? service.Amount.Value : serviceAmount;
            }
            if (serviceType == (int)ServiceType.PRODUCT_FLY_TICKET)
            {
                var listFlyBookingPackagesOptional = _flyBookingDetailRepository.GetBookingPackagesOptionalsByBookingId(serviceId).Result;
                var listFlyBookingPackagesOptionalViewModel = new List<FlyBookingPackagesOptionalViewModel>();
                foreach (var item in listFlyBookingPackagesOptional)
                {
                    FlyBookingPackagesOptionalViewModel model = new FlyBookingPackagesOptionalViewModel();
                    item.CopyProperties(model);
                    listFlyBookingPackagesOptionalViewModel.Add(model);
                }
                var listFlyBookingPackagesOptionalReturn = new List<FlyBookingPackagesOptionalViewModel>();
                foreach (var item in listFlyBookingPackagesOptionalViewModel)
                {
                    var getBySupplierId = listFlyBookingPackagesOptionalReturn.Where(n => n.SuplierId == item.SuplierId).ToList();
                    if (getBySupplierId.Count > 0)
                    {
                        foreach (var sup in listFlyBookingPackagesOptionalReturn)
                        {
                            if (sup.SuplierId == item.SuplierId)
                                sup.Amount += item.Amount;
                        }
                    }
                    else
                    {
                        listFlyBookingPackagesOptionalReturn.Add(item);
                    }
                }
                var service = listFlyBookingPackagesOptionalReturn.FirstOrDefault(n => n.SuplierId == supplierId);
                serviceAmount = service != null ? service.Amount : serviceAmount;
            }
            return serviceAmount;
        }

        public List<PaymentRequestViewModel> GetPaymentRequests(PaymentRequestSearchModel searchModel, out long total, int currentPage = 1, int pageSize = 20)
        {
            total = 0;
            try
            {
                var listPaymentRequests = paymentRequestDAL.GetPagingList(searchModel, currentPage, pageSize,
                ProcedureConstants.SP_GetListPaymentRequest).ToList<PaymentRequestViewModel>();
                if (listPaymentRequests.FirstOrDefault() != null)
                    total = listPaymentRequests.FirstOrDefault().TotalRow;
                var listServiceFlyStr = new List<string>();
                foreach (var item in listPaymentRequests)
                {
                    if (!string.IsNullOrEmpty(item.PaymentVoucherNo))
                        item.PaymentVoucherNo = item.PaymentVoucherNo.Replace(",", "").Trim();
                    if (string.IsNullOrEmpty(item.ListServiceCode)) continue;
                    var listServiceCode = item.ListServiceCode.Split(",");
                    foreach (string serviceCode in listServiceCode)
                    {
                        listServiceFlyStr.Add(serviceCode);
                    }
                }
                var listServiceFly = flyBookingDetailDAL.GetByServiceCodes(listServiceFlyStr);
                var listPaymentVoucher = paymentVoucherDAL.GetByPaymentCodes(listPaymentRequests.Where(n => !string.IsNullOrEmpty(n.PaymentVoucherNo)).Select(n => n.PaymentVoucherNo.Replace(",", " ")).ToList());
                var listOrder = orderDAL.GetByOrderNos(listPaymentRequests.Where(n => !string.IsNullOrEmpty(n.OrderNo)).Select(n => n.OrderNo).ToList());
                foreach (var item in listPaymentRequests)
                {
                    if (!string.IsNullOrEmpty(item.PaymentVoucherNo))
                    {
                        var paymentVoucher = listPaymentVoucher.FirstOrDefault(n => item.PaymentVoucherNo.Replace(",", "").Contains(n.PaymentCode));
                        item.PaymentVoucherId = paymentVoucher != null ? paymentVoucher.Id : 0;
                    }
                    if (!string.IsNullOrEmpty(item.OrderNo))
                    {
                        var order = listOrder.FirstOrDefault(n => n.OrderNo == item.OrderNo);
                        item.OrderId = order != null ? (int)order.OrderId : 0;
                    }
                    if (string.IsNullOrEmpty(item.ListServiceCode)) continue;
                    var listServiceCode = item.ListServiceCode.Split(",");
                    var listServiceId = item.ListServiceId.Split(",");
                    item.ListServiceCodeAndType = new List<CountStatus>();
                    int index = 0;
                    foreach (var serviceCode in listServiceCode)
                    {
                        CountStatus countStatus = new CountStatus();
                        var flyService = listServiceFly.FirstOrDefault(n => n.ServiceCode == serviceCode);
                        countStatus.DataNo = serviceCode;
                        countStatus.DataId = int.Parse(listServiceId[index]);
                        if (flyService != null)
                        {
                            countStatus.DataIdFly = flyService.GroupBookingId.Replace(",", "_");
                        }
                        if (serviceCode.Contains("TOUR"))
                            countStatus.ServiceType = (int)ServiceType.Tour;
                        if (serviceCode.Contains("FLIGHT"))
                            countStatus.ServiceType = (int)ServiceType.PRODUCT_FLY_TICKET;
                        if (serviceCode.Contains("HOTEL"))
                            countStatus.ServiceType = (int)ServiceType.BOOK_HOTEL_ROOM;
                        if (serviceCode.Contains("OTHER"))
                            countStatus.ServiceType = (int)ServiceType.Other;
                        item.ListServiceCodeAndType.Add(countStatus);
                        index++;
                    }
                }
                return listPaymentRequests;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetPaymentRequests - PaymentRequestRepository: " + ex);
                return new List<PaymentRequestViewModel>();
            }
        }

        public List<CountStatus> GetCountStatus(PaymentRequestSearchModel searchModel)
        {
            try
            {
                var listPaymentRequests = paymentRequestDAL.GetCountStatus(searchModel,
                ProcedureConstants.SP_CountPaymentRequestByStatus).ToList<CountStatus>();
                return listPaymentRequests;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetCountStatus - PaymentRequestRepository: " + ex);
                return new List<CountStatus>();
            }
        }

        public int UpdatePaymentRequest(PaymentRequestViewModel model)
        {
            var entity = paymentRequestDAL.GetById(model.Id);
            var result = paymentRequestDAL.UpdatePaymentRequest(model);
            if (result > 0 && entity.Status == (int)PAYMENT_REQUEST_STATUS.TU_CHOI && model.Amount != entity.Amount && model.IsServiceIncluded.Value)
            {
                var detailRequests = paymentRequestDAL.GetByPaymentRequestId((int)model.Id);
                foreach (var item in detailRequests)
                {
                    item.Amount = model.Amount;
                    paymentRequestDAL.UpdateRequestDetail(item);
                }
            }
            if (result > 0 && model.IsAdminEdit && model.Amount != entity.Amount && model.IsServiceIncluded.Value)
            {
                var detailRequests = paymentRequestDAL.GetByPaymentRequestId((int)model.Id);
                foreach (var item in detailRequests)
                {
                    item.Amount = model.Amount;
                    paymentRequestDAL.UpdateRequestDetail(item);
                }
                //tính lại số tiền phiếu chi nếu có
                var amountChange = entity.Amount - model.Amount;
                var paymentVoucher = paymentVoucherDAL.CheckExistsPaymentRequest(new List<long>() { model.Id },
                    ProcedureConstants.SP_CheckExistsPaymentVoucherByRequestId);
                if (paymentVoucher.Count > 0)
                {
                    foreach (var item in paymentVoucher)
                    {
                        item.Amount = item.Amount - amountChange;
                        paymentVoucherDAL.Update(item);
                    }
                }
            }
            return result;
        }

        public int ApprovePaymentRequest(string paymentRequestNo, int userId, int status)
        {
            var entity = paymentRequestDAL.GetByRequestNo(paymentRequestNo);
            entity.UpdatedBy = userId;
            entity.Status = status;
            return paymentRequestDAL.ApproveRequest(entity);
        }

        public int RejectPaymentRequest(string paymentRequestNo, string noteReject, int userId)
        {
            var entity = paymentRequestDAL.GetByRequestNo(paymentRequestNo);
            entity.Status = (int)PAYMENT_REQUEST_STATUS.TU_CHOI;
            entity.DeclineReason = noteReject;
            entity.UpdatedBy = userId;
            entity.UpdatedDate = DateTime.Now;
            return paymentRequestDAL.UpdateRequest(entity);
        }

        public int UndoApprove(string paymentRequestNo, string note, int userId, int status)
        {
            var entity = paymentRequestDAL.GetByRequestNo(paymentRequestNo);
            entity.UpdatedBy = userId;
            entity.Status = status;
            entity.Note = note;
            return paymentRequestDAL.UndoApproveRequest(entity);
        }

        public List<PaymentRequestViewModel> GetServiceListBySupplierId(long supplierId, int requestId = 0, int serviceId = 0)
        {
            try
            {
                var listService = new List<PaymentRequestViewModel>();
                var listServiceOutput = paymentRequestDAL.GetServiceListBySupplierId(supplierId,
                    ProcedureConstants.SP_GetAllServiceBySupplierId).ToList<PaymentRequestViewModel>();
                var listServiceId = listServiceOutput.Select(n => Convert.ToInt64(n.ServiceId)).ToList();
                var listRequestDetail = paymentRequestDAL.GetByDataIds(listServiceId);
                if (requestId == 0)
                    listRequestDetail = listRequestDetail.Where(n => n.Status != (int)PAYMENT_REQUEST_STATUS.TU_CHOI).ToList();
                if (serviceId != 0)
                    listServiceOutput = listServiceOutput.Where(n => n.ServiceId == serviceId).ToList();
                foreach (var item in listServiceOutput)
                {
                    item.TotalAmount = item.Amount;
                    var detail = listRequestDetail.Where(n => n.OrderId == item.OrderId && n.RequestId == requestId && n.ServiceId == item.ServiceId).FirstOrDefault();
                    if (detail != null)
                    {
                        item.IsChecked = true;
                        item.Id = detail.Id;
                        item.AmountPayment = detail.Amount;
                    }
                    item.TotalDisarmed = item.AmountPay;
                    item.TotalNeedPayment = item.Amount - item.TotalDisarmed;
                    if (item.TotalNeedPayment < 0)
                        item.TotalNeedPayment = 0;
                    PaymentRequestViewModel model = new PaymentRequestViewModel();
                    item.CopyProperties(model);
                    if (requestId > 0)
                    {
                        if (listService.FirstOrDefault(n => n.ServiceId == item.ServiceId) == null)
                        {
                            if (detail != null)
                            {
                                item.IsDisabled = true;
                                item.IsChecked = true;
                            }
                            listService.Add(item);
                        }
                    }
                    else
                    {
                        if (listService.FirstOrDefault(n => n.ServiceId == item.ServiceId) == null && item.TotalNeedPayment > 0)
                            listService.Add(item);
                    }
                }
                return listService;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetServiceListBySupplierId - PaymentRequestRepository: " + ex);
            }
            return new List<PaymentRequestViewModel>();
        }

        public List<PaymentRequestViewModel> GetServiceListByClientId(long clientId, int requestId = 0)
        {
            try
            {
                var listServiceOutput = paymentRequestDAL.GetServiceListByClientId(clientId,
                    ProcedureConstants.SP_GetAllServiceByClientId).ToList<PaymentRequestViewModel>();
                var listService = new List<PaymentRequestViewModel>();
                var listRequestDetail = paymentRequestDAL.GetByDataIds(listServiceOutput.Select(n => Convert.ToInt64(n.OrderId)).ToList());
                foreach (var item in listServiceOutput)
                {
                    item.TotalAmount = item.Amount;
                    var detail = listRequestDetail.Where(n => n.OrderId == item.OrderId && n.RequestId == requestId).FirstOrDefault();
                    if (detail != null)
                    {
                        item.IsChecked = true;
                        item.Id = detail.Id;
                        item.AmountPayment = detail.Amount;
                    }
                    item.TotalDisarmed = listRequestDetail.Where(n => n.OrderId == item.OrderId).Sum(n => n.Amount);
                    item.TotalNeedPayment = item.Amount - item.TotalDisarmed;
                    if (item.TotalNeedPayment < 0)
                        item.TotalNeedPayment = 0;
                    PaymentRequestViewModel model = new PaymentRequestViewModel();
                    item.CopyProperties(model);
                    if (requestId > 0)
                    {
                        var requestServiceDetails = paymentRequestDAL.GetRequestDetail(requestId,
                       ProcedureConstants.sp_GetAllServiceByRequestiD).ToList<PaymentRequestViewModel>();
                        foreach (var service in requestServiceDetails)
                        {
                            if (listService.FirstOrDefault(n => n.ServiceId == service.ServiceId) == null)
                            {
                                service.IsChecked = true;
                                service.IsDisabled = true;
                                service.AmountPayment = service.AmountPay;
                                listService.Add(service);
                            }
                        }
                    }
                    if (listService.FirstOrDefault(n => n.ServiceId == item.ServiceId) == null && item.TotalNeedPayment > 0)
                        listService.Add(item);
                }
                return listService;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetServiceListBySupplierId - PaymentRequestRepository: " + ex);
            }
            return new List<PaymentRequestViewModel>();
        }

        public List<PaymentRequestViewModel> GetByClientId(long clientId, long Type, int paymentVoucherId = 0)
        {
            try
            {
                var listPaymentRequestByClientId = paymentRequestDAL.GetListPaymentRequestByClientId(clientId, Type, ProcedureConstants.SP_GetListPaymentRequestByClientId).ToList<PaymentRequestViewModel>();
                var listPaymentRequest = listPaymentRequestByClientId.Where(n => n.Status == (int)PAYMENT_REQUEST_STATUS.CHO_CHI).ToList();
                var listPaymentRequestOutput = new List<PaymentRequestViewModel>();
                var listPaymentRequestExists = paymentRequestDAL.GetPaymentRequestExists(listPaymentRequest.Select(n => n.Id).ToList(),
                    ProcedureConstants.SP_CheckCreatePaymentVoucher).ToList<PaymentRequestViewModel>();
                var listRequetIdExists = new List<long>();
                foreach (var item in listPaymentRequestExists)
                {
                    var requestIds = item.RequestIds.Split(",");
                    foreach (var requestId in requestIds)
                    {
                        if (!string.IsNullOrEmpty(requestId))
                            listRequetIdExists.Add(int.Parse(requestId));
                    }
                }
                if (listPaymentRequestExists.Count > 0)
                {
                    foreach (var item in listPaymentRequest)
                    {
                        PaymentRequestViewModel model = new PaymentRequestViewModel();
                        var exists = listRequetIdExists.Contains(item.Id);
                        if (!exists)
                        {
                            item.CopyProperties(model);
                            listPaymentRequestOutput.Add(model);
                        }
                    }
                }
                else
                {
                    listPaymentRequestOutput = listPaymentRequest;
                }
                var paymentVoucher = paymentVoucherDAL.GetById(paymentVoucherId);
                if (paymentVoucher != null)
                {
                    var listRequest = paymentRequestDAL.GetByIds(
                   paymentVoucher.RequestId.Split(",").Select(n => Convert.ToInt64(n)).ToList());
                    if (paymentVoucherId > 0)
                    {
                        foreach (var item in listRequest)
                        {
                            PaymentRequestViewModel model = new PaymentRequestViewModel();
                            item.CopyProperties(model);
                            model.IsChecked = true;
                            listPaymentRequestOutput.Add(model);

                        }
                    }
                }
                return listPaymentRequestOutput;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetByClientId - PaymentRequestRepository: " + ex);
            }
            return new List<PaymentRequestViewModel>();
        }

        public List<PaymentRequestViewModel> GetBySupplierId(long supplierId, int paymentVoucherId = 0, string requestType = "1,2")
        {
            try
            {
                var listPaymentRequest = paymentRequestDAL.GetServiceListBySupplierId(supplierId,
                    ProcedureConstants.SP_GetListPaymentRequestBySupplierId, requestType).ToList<PaymentRequestViewModel>();
                var listPaymentRequestOutput = new List<PaymentRequestViewModel>();
                var listPaymentRequestExists = paymentRequestDAL.GetPaymentRequestExists(listPaymentRequest.Select(n => n.Id).ToList(),
                    ProcedureConstants.SP_CheckCreatePaymentVoucher).ToList<PaymentRequestViewModel>();
                var listRequetIdExists = new List<long>();
                foreach (var item in listPaymentRequestExists)
                {
                    var requestIds = item.RequestIds.Split(",");
                    foreach (var requestId in requestIds)
                    {
                        if (!string.IsNullOrEmpty(requestId))
                            listRequetIdExists.Add(int.Parse(requestId));
                    }
                }
                if (listPaymentRequestExists.Count > 0)
                {
                    foreach (var item in listPaymentRequest)
                    {
                        PaymentRequestViewModel model = new PaymentRequestViewModel();
                        var exists = listRequetIdExists.Contains(item.Id);
                        if (!exists)
                        {
                            item.CopyProperties(model);
                            listPaymentRequestOutput.Add(model);
                        }
                    }
                }
                else
                {
                    listPaymentRequestOutput = listPaymentRequest;
                }
                var paymentVoucher = paymentVoucherDAL.GetById(paymentVoucherId);
                if (paymentVoucher != null && !string.IsNullOrEmpty(paymentVoucher.RequestId))
                {
                    var listRequest = paymentRequestDAL.GetByIds(
                   paymentVoucher.RequestId.Split(",").Select(n => Convert.ToInt64(n)).ToList());
                    if (paymentVoucherId > 0)
                    {
                        foreach (var item in listRequest)
                        {
                            PaymentRequestViewModel model = new PaymentRequestViewModel();
                            item.CopyProperties(model);
                            model.IsChecked = true;
                            listPaymentRequestOutput.Add(model);
                        }
                    }
                }

                return listPaymentRequestOutput;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetBySupplierId - PaymentRequestRepository: " + ex);
            }
            return new List<PaymentRequestViewModel>();
        }

        public List<PaymentRequestViewModel> GetByServiceId(long serviceId, int type, int requestType = 0)
        {
            try
            {
                var listServiceOutput = paymentRequestDAL.GetListPaymentRequestByServiceId(serviceId, type,
                    ProcedureConstants.sp_GetListPaymentRequestByServiceId, requestType).ToList<PaymentRequestViewModel>();
                foreach (var item in listServiceOutput)
                {
                    item.ListServiceCodeAndType = new List<CountStatus>();
                    if (!string.IsNullOrEmpty(item.PaymentVoucherCode))
                    {
                        var listPaymentVoucher = paymentVoucherDAL.GetByPaymentCodes(item.PaymentVoucherCode.Split(",").ToList());
                        foreach (var paymentVoucher in listPaymentVoucher)
                        {
                            CountStatus countStatus = new CountStatus();
                            countStatus.DataNo = paymentVoucher.PaymentCode;
                            countStatus.DataId = paymentVoucher.Id;
                            item.ListServiceCodeAndType.Add(countStatus);
                        }
                    }
                }
                return listServiceOutput;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetByServiceId - PaymentRequestRepository: " + ex);
            }
            return new List<PaymentRequestViewModel>();
        }

        public List<PaymentRequestViewModel> GetRequestByClientId(long clientId, long orderid = 0)
        {
            try
            {
                var listPaymentRequest = paymentRequestDAL.GetListPaymentRequestByClientId(clientId,3,
                    ProcedureConstants.SP_GetListPaymentRequestByClientId).ToList<PaymentRequestViewModel>();
                foreach (var item in listPaymentRequest)
                {
                    item.ListServiceCodeAndType = new List<CountStatus>();
                    if (!string.IsNullOrEmpty(item.PaymentVoucherCode))
                    {
                        var listPaymentVoucher = paymentVoucherDAL.GetByPaymentCodes(item.PaymentVoucherCode.Split(",").ToList());
                        foreach (var paymentVoucher in listPaymentVoucher)
                        {
                            CountStatus countStatus = new CountStatus();
                            countStatus.DataNo = paymentVoucher.PaymentCode;
                            countStatus.DataId = paymentVoucher.Id;
                            item.ListServiceCodeAndType.Add(countStatus);
                        }
                    }
                }
                if (orderid > 0)
                {
                    var listTemp = new List<PaymentRequestViewModel>();
                    foreach (var item in listPaymentRequest)
                    {
                        var requestDetails = paymentRequestDAL.GetByPaymentRequestId((int)item.Id);
                        if (requestDetails.FirstOrDefault(n => n.OrderId == orderid) != null)
                            listTemp.Add(item);
                    }
                    listPaymentRequest = listTemp;
                }
                return listPaymentRequest;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetByServiceId - PaymentRequestRepository: " + ex);
            }
            return new List<PaymentRequestViewModel>();
        }

        public int DeletePaymentRequest(string paymentRequestNo, int userId)
        {
            var entity = paymentRequestDAL.GetByRequestNo(paymentRequestNo);
            entity.UpdatedBy = userId;
            entity.IsDelete = true;
            entity.UpdatedDate = DateTime.Now;
            return paymentRequestDAL.UpdateRequest(entity);
        }
        public List<OrderPaymentRequest> GetListPaymentRequestByOrderId(int Orderid)
        {
            try
            {
                var dt = paymentRequestDAL.GetListPaymentRequestByOrderId(Orderid);
                if (dt != null && dt.Rows.Count > 0)
                {
                    var data = dt.ToList<OrderPaymentRequest>();
                    return data;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetListPaymentRequestByOrderId - PaymentRequestRepository: " + ex);
            }
            return null;
        }

    }
}
