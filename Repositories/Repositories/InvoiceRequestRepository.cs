using Aspose.Cells;
using DAL;
using DAL.Invoice;
using DAL.StoreProcedure;
using Entities.ConfigModels;
using Entities.Models;
using Entities.ViewModels;
using Entities.ViewModels.Funding;
using Entities.ViewModels.Invoice;
using Microsoft.Extensions.Options;
using Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using Utilities;
using Utilities.Contants;

namespace Repositories.Repositories
{
    public class InvoiceRequestRepository : IInvoiceRequestRepository
    {
        private readonly UserDAL userDAL;
        private readonly InvoiceRequestDAL invoiceRequestDAL;
        private readonly InvoiceDAL invoiceDAL;
        private readonly OrderDAL orderDal;
        private readonly AttachFileDAL attachFileDAL;
        private readonly string _UrlStaticImage;

        public InvoiceRequestRepository(IOptions<DataBaseConfig> dataBaseConfig, IOptions<DomainConfig> domainConfig)
        {
            orderDal = new OrderDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            userDAL = new UserDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            invoiceRequestDAL = new InvoiceRequestDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            invoiceDAL = new InvoiceDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            attachFileDAL = new AttachFileDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            _UrlStaticImage = domainConfig.Value.ImageStatic;

        }

        public int ApproveInvoiceRequest(int invoiceRequestId, int userId, int status)
        {
            return invoiceRequestDAL.ApproveRequest(invoiceRequestId, userId, status);
        }

        public int CreateInvoiceRequest(InvoiceRequestViewModel model)
        {
            var paymentRequest = invoiceRequestDAL.GetByRequestNo(model.InvoiceRequestNo);
            if (paymentRequest != null)
                return -2;

            if (model.OtherImages != null && model.OtherImages.Count() > 0)
            {
                var arrImage = new List<string>();
                foreach (var image in model.OtherImages)
                {
                    arrImage.Add(UpLoadHelper.UploadBase64Src(image, _UrlStaticImage).Result);
                }
                model.AttachFile = String.Join(",", arrImage);
            }
            return invoiceRequestDAL.CreateInvoiceRequest(model);
        }

        public string ExportInvoiceRequest(InvoiceRequestSearchModel searchModel, string FilePath)
        {
            var pathResult = string.Empty;
            try
            {
                var invoiceRequests = GetInvoiceRequests(searchModel, out long total, 1, -1);

                if (invoiceRequests != null && invoiceRequests.Count > 0)
                {
                    Workbook wb = new Workbook();
                    Worksheet ws = wb.Worksheets[0];
                    ws.Name = "Yêu cầu xuất hóa đơn";
                    Cells cell = ws.Cells;

                    var range = ws.Cells.CreateRange(0, 0, 1, 1);
                    StyleFlag st = new StyleFlag();
                    st.All = true;
                    Style style = ws.Cells["A1"].GetStyle();

                    #region Header
                    range = cell.CreateRange(0, 0, 1, 17);
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
                    cell.SetColumnWidth(3, 30);
                    cell.SetColumnWidth(4, 30);
                    cell.SetColumnWidth(5, 40);
                    cell.SetColumnWidth(6, 40);
                    cell.SetColumnWidth(7, 25);
                    cell.SetColumnWidth(8, 25);
                    cell.SetColumnWidth(9, 50);
                    cell.SetColumnWidth(10, 25);
                    cell.SetColumnWidth(11, 40);
                    cell.SetColumnWidth(12, 30);
                    cell.SetColumnWidth(13, 30);
                    cell.SetColumnWidth(14, 30);
                    cell.SetColumnWidth(15, 30);
                    cell.SetColumnWidth(16, 30);

                    // Set header value
                    ws.Cells["A1"].PutValue("STT");
                    ws.Cells["B1"].PutValue("Mã phiếu");
                    ws.Cells["C1"].PutValue("Ngày dự kiến sản xuất");
                    ws.Cells["D1"].PutValue("Số hóa đơn");
                    ws.Cells["E1"].PutValue("Ngày xuất");
                    ws.Cells["F1"].PutValue("Khách hàng");
                    ws.Cells["G1"].PutValue("Tiền trước VAT");
                    ws.Cells["H1"].PutValue("Xuất thêm");
                    ws.Cells["I1"].PutValue("Thu thêm");
                    ws.Cells["J1"].PutValue("Đơn hàng liên quan");
                    ws.Cells["K1"].PutValue("Trạng thái");
                    ws.Cells["L1"].PutValue("Ngày tạo");
                    ws.Cells["M1"].PutValue("Người tạo");
                    ws.Cells["N1"].PutValue("Ngày duyệt");
                    ws.Cells["O1"].PutValue("Người duyệt");
                    #endregion

                    #region Body

                    range = cell.CreateRange(1, 0, invoiceRequests.Count, 13);
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
                    RowIndex++;
                    ws.Cells["A" + RowIndex].PutValue(RowIndex - 1);
                    ws.Cells["A" + RowIndex].SetStyle(alignCenterStyle);
                    ws.Cells["B" + RowIndex].PutValue("");
                    ws.Cells["C" + RowIndex].PutValue("");
                    ws.Cells["D" + RowIndex].PutValue(string.Empty);
                    ws.Cells["E" + RowIndex].PutValue(string.Empty);
                    ws.Cells["F" + RowIndex].PutValue("Tổng cộng");
                    ws.Cells["G" + RowIndex].PutValue(invoiceRequests.Sum(n => n.TotalPrice).Value.ToString("N0"));
                    ws.Cells["G" + RowIndex].SetStyle(numberStyle);
                    ws.Cells["H" + RowIndex].PutValue(invoiceRequests.Sum(n => n.PriceExtraExport).Value.ToString("N0"));
                    ws.Cells["H" + RowIndex].SetStyle(numberStyle);
                    ws.Cells["I" + RowIndex].PutValue(invoiceRequests.Sum(n => n.PriceExtra).Value.ToString("N0"));
                    ws.Cells["I" + RowIndex].SetStyle(numberStyle);
                    ws.Cells["J" + RowIndex].PutValue(string.Empty);
                    ws.Cells["K" + RowIndex].PutValue(string.Empty);
                    ws.Cells["L" + RowIndex].PutValue(string.Empty);
                    ws.Cells["M" + RowIndex].PutValue(string.Empty);
                    ws.Cells["N" + RowIndex].PutValue(string.Empty);
                    ws.Cells["O" + RowIndex].PutValue(string.Empty);

                    foreach (var item in invoiceRequests)
                    {
                        item.TotalPrice = item.TotalPriceVAT;
                        RowIndex++;
                        ws.Cells["A" + RowIndex].PutValue(RowIndex - 1);
                        ws.Cells["A" + RowIndex].SetStyle(alignCenterStyle);
                        ws.Cells["B" + RowIndex].PutValue(item.InvoiceRequestNo);
                        ws.Cells["C" + RowIndex].PutValue(item.PlanDate != null ? item.PlanDate.Value.ToString("dd/MM/yyyy") : string.Empty);
                        ws.Cells["D" + RowIndex].PutValue(item.InvoiceNo);
                        ws.Cells["E" + RowIndex].PutValue(item.PlanDate != null ? item.PlanDate.Value.ToString("dd/MM/yyyy") : string.Empty);
                        ws.Cells["F" + RowIndex].PutValue(item.ClientName);
                        ws.Cells["G" + RowIndex].PutValue(item.TotalPrice);
                        ws.Cells["G" + RowIndex].SetStyle(numberStyle);
                        ws.Cells["H" + RowIndex].PutValue(item.PriceExtra);
                        ws.Cells["H" + RowIndex].SetStyle(numberStyle);
                        ws.Cells["I" + RowIndex].PutValue(item.PriceExtraExport);
                        ws.Cells["I" + RowIndex].SetStyle(numberStyle);
                        ws.Cells["J" + RowIndex].PutValue(item.OrderNo);
                        ws.Cells["K" + RowIndex].PutValue(item.InvoiceRequestStatus);
                        ws.Cells["L" + RowIndex].PutValue(item.CreatedDate.Value.ToString("dd/MM/yyyy"));
                        ws.Cells["M" + RowIndex].PutValue(item.UserName);
                        ws.Cells["N" + RowIndex].PutValue(item.VerifyDate != null ? item.VerifyDate.Value.ToString("dd/MM/yyyy") : string.Empty);
                        ws.Cells["O" + RowIndex].PutValue(item.UserVerifyName);
                    }

                    #endregion
                    wb.Save(FilePath);
                    pathResult = FilePath;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ExportInvoiceRequest - InvoiceRequestRepository: " + ex);
            }
            return pathResult;
        }

        public InvoiceRequestViewModel GetById(int invoiceRequestId)
        {
            try
            {
                var requestInfos = invoiceRequestDAL.GetRequestDetail(invoiceRequestId)
                    .ToList<InvoiceRequestViewModel>();
                var requestInfo = requestInfos.FirstOrDefault();
                requestInfo.AttachFiles = attachFileDAL.GetListByDataID(invoiceRequestId, (int)AttachmentType.Invoice_Request).Result;
                requestInfo.InvoiceRequestDetails = new List<InvoiceRequestDetailViewModel>();
                foreach (var item in requestInfos)
                {
                    InvoiceRequestDetailViewModel model = new InvoiceRequestDetailViewModel();
                    item.CopyProperties(model);
                    model.Id = item.InvoiceRequestDetailId;
                    model.ProductName = item.ProductName;
                    model.PriceExtra = item.PriceExtra;
                    model.PriceExtraExport = item.PriceExtraExport;
                    model.TotalPrice = item.Quantity * item.Price;
                    model.Unit = item.Unit;
                    model.Quantity = item.Quantity;
                    model.Price = item.Price;
                    //model.PriceVat = (double?)Math.Round((decimal)(model.TotalPrice - (((float)item.VAT / (float)100) * model.TotalPrice)), MidpointRounding.AwayFromZero);
                    //model.Vat = item.VAT;
                    requestInfo.InvoiceRequestDetails.Add(model);
                }
                requestInfo.VAT = requestInfos.FirstOrDefault().VAT;
                return requestInfo;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetById - InvoiceRequestRepository: " + ex);
                return new InvoiceRequestViewModel();
            }
        }

        public List<InvoiceRequestViewModel> GetInvoiceRequests(InvoiceRequestSearchModel searchModel, out long total, int currentPage = 1, int pageSize = 20)
        {
            total = 0;
            try
            {
                var listInvoiceRequests = invoiceRequestDAL.GetPagingList(searchModel, currentPage, pageSize,
                ProcedureConstants.SP_GetListInvoiceRequest).ToList<InvoiceRequestViewModel>();
                if (listInvoiceRequests.FirstOrDefault() != null)
                    total = listInvoiceRequests.FirstOrDefault().TotalRow;
                foreach (var item in listInvoiceRequests)
                {
                    item.PriceExtraExport += (double)item.TotalPrice;
                }
                return listInvoiceRequests;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetInvoiceRequests - InvoiceRequestRepository: " + ex);
                return new List<InvoiceRequestViewModel>();
            }
        }

        public int RejectRequest(int invoiceRequestId, string noteReject, int userId)
        {
            return invoiceRequestDAL.RejectRequest(invoiceRequestId, noteReject, userId);
        }

        public int UpdateInvoiceRequest(InvoiceRequestViewModel model)
        {
            if (model.OtherImages != null && model.OtherImages.Count() > 0)
            {
                var arrImage = new List<string>();
                foreach (var image in model.OtherImages)
                {
                    if (string.IsNullOrEmpty(image)) continue;
                    if (image.Contains("http") || image.Contains("static-image") || image.Contains("uploads"))
                        arrImage.Add(image.Replace(_UrlStaticImage, ""));
                    else
                        arrImage.Add(UpLoadHelper.UploadBase64Src(image, _UrlStaticImage).Result);
                }
                model.AttachFile = String.Join(",", arrImage);
            }
            return invoiceRequestDAL.UpdateInvoiceRequest(model);
        }

        public List<CountStatus> GetCountStatus(InvoiceRequestSearchModel searchModel)
        {
            try
            {
                var listPaymentRequests = invoiceRequestDAL.GetCountStatus(searchModel,
                ProcedureConstants.SP_CountInvoiceRequestStatus).ToList<CountStatus>();
                return listPaymentRequests;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetCountStatus - InvoiceRequestRepository: " + ex);
                return new List<CountStatus>();
            }
        }

        public List<OrderViewModel> GetByClientId(long clientId, int invoiceRequestId = 0, int status = 0)
        {
            try
            {
                var listOrderOutput = new List<OrderViewModel>();
                var dt = orderDal.GetListOrderByClientId(clientId, ProcedureConstants.SP_GetDetailOrderByClientId, status);
                if (dt != null && dt.Rows.Count > 0)
                {
                    var listOrder = (from row in dt.AsEnumerable()
                                     select new OrderViewModel
                                     {
                                         OrderId = row["OrderId"].ToString(),
                                         OrderCode = row["OrderNo"].ToString(),
                                         StartDate = !row["StartDate"].Equals(DBNull.Value) ? Convert.ToDateTime(row["StartDate"]).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : "",
                                         EndDate = !row["EndDate"].Equals(DBNull.Value) ? Convert.ToDateTime(row["EndDate"]).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : "",
                                         Status = row["OrderStatus"].ToString(),
                                         PaymentStatus = row["PaymentStatus"].ToString(),
                                         SalerName = row["SalerName"].ToString(),
                                         CreateDate = row["CreateTime"].Equals(DBNull.Value) ? "" : Convert.ToDateTime(row["CreateTime"]).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                                         Amount = !row["Amount"].Equals(DBNull.Value) ? Convert.ToDouble(row["Amount"].ToString()) : 0,
                                     }).ToList();
                    foreach (var item in listOrder)
                    {
                        listOrderOutput.Add(item);
                    }
                }
                if (invoiceRequestId != 0)
                {
                    var detail = GetById(invoiceRequestId);
                    OrderViewModel orderViewModel = new OrderViewModel();
                    var orderInfo = orderDal.GetByOrderId(detail.OrderId.Value);
                    orderViewModel.OrderId = orderInfo.OrderId.ToString();
                    orderViewModel.OrderCode = orderInfo.OrderNo;
                    orderViewModel.StartDate = orderInfo.StartDate != null ?
                        orderInfo.StartDate.Value.ToString("dd:MM:yyyy") : string.Empty;
                    orderViewModel.EndDate = orderInfo.EndDate != null ?
                        orderInfo.EndDate.Value.ToString("dd:MM:yyyy") : string.Empty;
                    orderViewModel.SalerName = userDAL.GetById(orderInfo.SalerId != null ? orderInfo.SalerId.Value : 0).Result?.FullName;
                    orderViewModel.Amount = orderInfo.Amount != null ? orderInfo.Amount.Value : 0;
                    orderViewModel.IsChecked = true;
                    if (listOrderOutput.FirstOrDefault(n => n.OrderId == orderViewModel.OrderId) == null)
                        listOrderOutput.Add(orderViewModel);
                }
                return listOrderOutput;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetByClientId - OrderRepository: " + ex);
            }
            return new List<OrderViewModel>();
        }

        public int DeleteInvoiceRequest(int invoiceRequestId, int userId)
        {
            return invoiceRequestDAL.DeleteRequest(invoiceRequestId, userId);
        }

        public List<InvoiceRequestViewModel> GetInvoiceRequestsByClientId(long clientId, int invoiceId = 0)
        {
            try
            {
                var listInvoiceRequests = invoiceRequestDAL.GetByClientId(clientId).ToList<InvoiceRequestViewModel>();
                var listInvoiceRequestDetail = invoiceRequestDAL.GetByInvoiceRequestIds(listInvoiceRequests.Select(n => n.Id).ToList());
                foreach (var item in listInvoiceRequests)
                {
                    //item.VATAmount = ((decimal)item.VAT / (decimal)100) * item.TotalPrice;
                    //item.TotalPriceVAT = (decimal)(item.TotalPrice + item.VATAmount);

                    item.InvoiceRequestDetails = new List<InvoiceRequestDetailViewModel>();
                    var listDetail = listInvoiceRequestDetail.Where(n => n.InvoiceRequestId == item.Id).ToList();
                    foreach (var detail in listDetail)
                    {
                        InvoiceRequestDetailViewModel model = new InvoiceRequestDetailViewModel();
                        detail.CopyProperties(model);
                        item.InvoiceRequestDetails.Add(model);
                    }
                }
                if (invoiceId != 0)
                {
                    var listInvoiceDetail = invoiceDAL.GetByInvoiceId(invoiceId);
                    var listRequestByInvoiceId = GetInvoiceRequestsByInvoiceId(invoiceId);
                    foreach (var item in listRequestByInvoiceId)
                    {
                        var exists = listInvoiceDetail.FirstOrDefault(n => n.InvoiceRequestId == item.Id);
                        if (exists != null)
                            item.InvoiceDetailId = exists.Id;
                        //item.VATAmount = ((decimal)item.VAT / (decimal)100) * item.TotalPrice;
                        //item.TotalPriceVAT = (decimal)(item.TotalPrice + item.VATAmount);
                        item.InvoiceRequestId = item.Id;
                        item.InvoiceRequestDetails = new List<InvoiceRequestDetailViewModel>();
                        var listDetail = listInvoiceRequestDetail.Where(n => n.InvoiceRequestId == item.Id).ToList();
                        foreach (var detail in listDetail)
                        {
                            InvoiceRequestDetailViewModel model = new InvoiceRequestDetailViewModel();
                            detail.CopyProperties(model);
                            item.InvoiceRequestDetails.Add(model);
                        }
                        item.IsChecked = true;
                        listInvoiceRequests.Add(item);
                    }
                }
                return listInvoiceRequests;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetInvoiceRequests - InvoiceRequestRepository: " + ex);
                return new List<InvoiceRequestViewModel>();
            }
        }

        public List<InvoiceRequestViewModel> GetInvoiceRequestsByInvoiceId(long invoiceId)
        {
            try
            {
                return invoiceRequestDAL.GetByInvoiceId(invoiceId).ToList<InvoiceRequestViewModel>();
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetInvoiceRequests - InvoiceRequestRepository: " + ex);
                return new List<InvoiceRequestViewModel>();
            }
        }

        public List<InvoiceRequestViewModel> GetInvoiceRequestByOrderId(string orderId)
        {
            try
            {
                List<InvoiceRequestViewModel> invoiceRequestViewModels = new List<InvoiceRequestViewModel>();
                var listRequest = invoiceRequestDAL.GetByOrderId(orderId);
                foreach (var item in listRequest)
                {
                    InvoiceRequestViewModel model = new InvoiceRequestViewModel();
                    //item.VATAmount = ((decimal)item.VAT / (decimal)100) * item.TotalPrice;
                    //item.TotalPriceVAT = (decimal)(item.TotalPrice + item.VATAmount);
                    item.CopyProperties(model);
                    invoiceRequestViewModels.Add(model);
                }
                return invoiceRequestViewModels;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetInvoiceRequests - InvoiceRequestRepository: " + ex);
                return new List<InvoiceRequestViewModel>();
            }
        }

        public List<InvoiceRequestHistoryViewModel> GetHistoriesByRequestId(long requestId)
        {
            var listHistory = invoiceRequestDAL.GetHistoriesByRequestId(requestId);
            var listUserCreate = userDAL.GetByIds(listHistory.Where(n => n.CreatedBy != null).Select(n => (long)n.CreatedBy.Value).ToList()).Result;
            List<InvoiceRequestHistoryViewModel> result = new List<InvoiceRequestHistoryViewModel>();
            foreach (var item in listHistory)
            {
                var user = listUserCreate.FirstOrDefault(n => n.Id == item.CreatedBy);
                InvoiceRequestHistoryViewModel model = new InvoiceRequestHistoryViewModel();
                item.CopyProperties(model);
                model.UserCreateName = user?.FullName;
                result.Add(model);
            }
            return result;
        }

        public int FinishInvoiceRequest(InvoiceRequestViewModel model)
        {
            if (model.OtherImages != null && model.OtherImages.Count() > 0)
            {
                var arrImage = new List<string>();
                foreach (var image in model.OtherImages)
                {
                    arrImage.Add(UpLoadHelper.UploadBase64Src(image, _UrlStaticImage).Result);
                }
            }
            return invoiceRequestDAL.ApproveRequest((int)model.Id, model.CreatedBy.Value, (int)INVOICE_REQUEST_STATUS.HOAN_THANH);
        }
    }
}
