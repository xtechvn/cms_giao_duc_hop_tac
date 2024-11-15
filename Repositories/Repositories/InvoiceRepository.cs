using Aspose.Cells;
using DAL;
using DAL.Funding;
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
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;


namespace Repositories.Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly InvoiceDAL invoiceDAL;
        private readonly OrderDAL orderDAL;
        private readonly InvoiceRequestDAL invoiceRequestDAL;
        private readonly string _UrlStaticImage;

        public InvoiceRepository(IOptions<DataBaseConfig> dataBaseConfig, IOptions<DomainConfig> domainConfig)
        {
            _UrlStaticImage = domainConfig.Value.ImageStatic;
            invoiceDAL = new InvoiceDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            orderDAL = new OrderDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            invoiceRequestDAL = new InvoiceRequestDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
        }

        public int CreateInvoice(InvoiceViewModel model)
        {
            var paymentRequest = invoiceDAL.GetByRequestNo(model.InvoiceCode);
            if (paymentRequest != null)
                return -2;

            if (model.OtherImages != null && model.OtherImages.Count() > 0)
            {
                var arrImage = new List<string>();
                foreach (var image in model.OtherImages)
                {
                    arrImage.Add(UpLoadHelper.UploadBase64Src(image, _UrlStaticImage).Result);
                }
                model.AttactFile = String.Join(",", arrImage);
            }
            return invoiceDAL.CreateInvoice(model);
        }

        public int DeleteInvoice(int invoiceId, int userId)
        {
            return invoiceDAL.DeleteInvoice(invoiceId, userId);
        }

        public string ExportInvoice(InvoiceSearchModel searchModel, string FilePath)
        {
            var pathResult = string.Empty;
            try
            {
                var invoices = GetInvoices(searchModel, out long total, 1, -1);

                if (invoices != null && invoices.Count > 0)
                {
                    Workbook wb = new Workbook();
                    Worksheet ws = wb.Worksheets[0];
                    ws.Name = "Danh sách hóa đơn";
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

                    // Set header value
                    ws.Cells["A1"].PutValue("STT");
                    ws.Cells["B1"].PutValue("Mã phiếu");
                    ws.Cells["C1"].PutValue("Số hóa đơn");
                    ws.Cells["D1"].PutValue("Ngày xuất");
                    ws.Cells["E1"].PutValue("Khách hàng");
                    ws.Cells["F1"].PutValue("Tiền trước VAT");
                    ws.Cells["G1"].PutValue("VAT");
                    ws.Cells["H1"].PutValue("Tiền sau VAT");
                    ws.Cells["I1"].PutValue("Xuất thêm");
                    ws.Cells["J1"].PutValue("Thu thêm");
                    ws.Cells["K1"].PutValue("Yêu cầu xuất liên quan");
                    ws.Cells["L1"].PutValue("Ngày tạo");
                    ws.Cells["M1"].PutValue("Người tạo");
                    #endregion

                    #region Body

                    range = cell.CreateRange(1, 0, invoices.Count, 13);
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
                    ws.Cells["D" + RowIndex].PutValue("");
                    ws.Cells["E" + RowIndex].PutValue("Tổng cộng");
                    ws.Cells["F" + RowIndex].PutValue(invoices.Sum(n => n.TotalPrice).Value.ToString("N0"));
                    ws.Cells["F" + RowIndex].SetStyle(numberStyle);
                    ws.Cells["G" + RowIndex].PutValue(invoices.Sum(n => n.VATAmount).Value.ToString("N0"));
                    ws.Cells["G" + RowIndex].SetStyle(numberStyle);
                    ws.Cells["H" + RowIndex].PutValue(invoices.Sum(n => n.TotalPriceVAT).ToString("N0"));
                    ws.Cells["H" + RowIndex].SetStyle(numberStyle);
                    ws.Cells["I" + RowIndex].PutValue(invoices.Sum(n => n.PriceExtraExport).Value.ToString("N0"));
                    ws.Cells["I" + RowIndex].SetStyle(numberStyle);
                    ws.Cells["J" + RowIndex].PutValue(invoices.Sum(n => n.PriceExtra).Value.ToString("N0"));
                    ws.Cells["J" + RowIndex].SetStyle(numberStyle);
                    ws.Cells["K" + RowIndex].PutValue("");
                    ws.Cells["L" + RowIndex].PutValue(string.Empty);
                    ws.Cells["M" + RowIndex].PutValue(string.Empty);

                    foreach (var item in invoices)
                    {
                        RowIndex++;
                        ws.Cells["A" + RowIndex].PutValue(RowIndex - 1);
                        ws.Cells["A" + RowIndex].SetStyle(alignCenterStyle);
                        ws.Cells["B" + RowIndex].PutValue(item.InvoiceCode);
                        ws.Cells["C" + RowIndex].PutValue(item.InvoiceNo);
                        ws.Cells["D" + RowIndex].PutValue(item.ExportDate != null ? item.ExportDate.Value.ToString("dd/MM/yyyy") : string.Empty);
                        ws.Cells["E" + RowIndex].PutValue(item.ClientName);
                        ws.Cells["F" + RowIndex].PutValue(item.TotalPrice);
                        ws.Cells["F" + RowIndex].SetStyle(numberStyle);
                        ws.Cells["G" + RowIndex].PutValue(item.VATAmount);
                        ws.Cells["G" + RowIndex].SetStyle(numberStyle);
                        ws.Cells["H" + RowIndex].PutValue(item.TotalPriceVAT);
                        ws.Cells["H" + RowIndex].SetStyle(numberStyle);
                        ws.Cells["I" + RowIndex].PutValue(item.PriceExtraExport);
                        ws.Cells["I" + RowIndex].SetStyle(numberStyle);
                        ws.Cells["J" + RowIndex].PutValue(item.PriceExtra);
                        ws.Cells["J" + RowIndex].SetStyle(numberStyle);
                        ws.Cells["K" + RowIndex].PutValue(item.InvoiceRequestNo);
                        ws.Cells["L" + RowIndex].PutValue(item.CreatedDate.Value.ToString("dd/MM/yyyy"));
                        ws.Cells["M" + RowIndex].PutValue(item.UserName);
                    }

                    #endregion
                    wb.Save(FilePath);
                    pathResult = FilePath;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ExportInvoiceRequest - InvoiceRepository: " + ex);
            }
            return pathResult;
        }

        public InvoiceViewModel GetById(int invoiceId)
        {
            try
            {
                if (invoiceId == 0)
                    return new InvoiceViewModel();
                var invoiceInfos = invoiceDAL.GetInvoiceInfo(invoiceId)
                    .ToList<InvoiceViewModel>();
                var invoiceInfo = invoiceInfos.FirstOrDefault();
                if (!string.IsNullOrEmpty(invoiceInfo.AttactFile))
                {
                    invoiceInfo.AttachFiles = invoiceInfo.AttactFile.Split(",").ToList();
                }

                invoiceInfo.InvoiceRequests = new List<InvoiceRequestDetailViewModel>();
                foreach (var item in invoiceInfos)
                {
                    InvoiceRequestDetailViewModel model = new InvoiceRequestDetailViewModel();
                    item.CopyProperties(model);
                    model.Id = item.Id;
                    model.InvoiceRequestNo = item.InvoiceRequestNo;
                    model.InvoiceRequestId = item.InvoiceRequestId;
                    model.PlanDate = item.PlanDate;
                    model.PriceExtra = item.PriceExtra;
                    model.PriceExtraExport = item.PriceExtraExport;
                    model.TotalPrice = (double)item.TotalPrice;
                    model.VatAmount = (((float)item.VAT / (float)100) * model.TotalPrice);
                    model.Unit = item.Unit;
                    //model.PriceVat = (double?)Math.Round((decimal)(model.TotalPrice + (((float)item.VAT / (float)100) * model.TotalPrice)), MidpointRounding.AwayFromZero);
                    //model.Vat = item.VAT;
                    invoiceInfo.InvoiceRequests.Add(model);
                }
                invoiceInfo.InvoiceDetails = new List<InvoiceDetailViewModel>();
                var listInvoiceRequestDetail = invoiceRequestDAL.GetByInvoiceRequestIds(
                    invoiceInfo.InvoiceRequests.Select(n => n.InvoiceRequestId.Value).ToList());
                foreach (var item in listInvoiceRequestDetail)
                {
                    InvoiceDetailViewModel model = new InvoiceDetailViewModel();
                    item.CopyProperties(model);
                    model.TotalPrice = item.Quantity * item.Price;
                    //model.VATAmount = ((double)item.Vat / (double)100) * model.TotalPrice;
                    //model.TotalPriceVAT = model.TotalPrice + model.VATAmount;
                    model.PriceExtra = (decimal)item.PriceExtra;
                    model.PriceExtraExport = (decimal)item.PriceExtraExport;
                    invoiceInfo.InvoiceDetails.Add(model);
                }
                invoiceInfo.VAT = invoiceInfos.FirstOrDefault().VAT;
                return invoiceInfo;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetById - InvoiceRepository: " + ex);
                return new InvoiceViewModel();
            }
        }

        public List<InvoiceViewModel> GetInvoices(InvoiceSearchModel searchModel, out long total, int currentPage = 1, int pageSize = 20)
        {
            total = 0;
            try
            {
                var listInvoices = invoiceDAL.GetPagingList(searchModel, currentPage, pageSize,
                ProcedureConstants.SP_GetListInvoice).ToList<InvoiceViewModel>();
                if (listInvoices.FirstOrDefault() != null)
                    total = listInvoices.FirstOrDefault().TotalRow;
                foreach (var item in listInvoices)
                {
                    item.PriceExtraExport += (double)item.TotalPrice;
                    var listInvoiceRequestNo = item.InvoiceRequestNo.Split(",");
                    var listInvoiceRequest = invoiceRequestDAL.GetByInvoiceRequestNo(listInvoiceRequestNo.ToList());
                    item.RelateRequest = listInvoiceRequest;
                    if (!string.IsNullOrEmpty(item.OrderNo))
                    {
                        var listOrderID = item.OrderNo.Split(",").Select(n => n).ToList();
                        item.RelateOrder = new List<CountStatus>();
                        var listOrder = orderDAL.GetByOrderNos(listOrderID);
                        foreach (var order in listOrder)
                        {
                            CountStatus countStatus = new CountStatus();
                            countStatus.DataId = order.OrderId;
                            countStatus.DataNo = order.OrderNo;
                            item.RelateOrder.Add(countStatus);
                        }
                    }
                }
                return listInvoices;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetInvoices - InvoiceRepository: " + ex);
                return new List<InvoiceViewModel>();
            }
        }

        public int UpdateInvoice(InvoiceViewModel model)
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
                model.AttactFile = String.Join(",", arrImage);
            }
            return invoiceDAL.UpdateInvoice(model);
        }
        public async Task<List<InvoiceCodeViewModel>> GetListInvoiceCodebyOrderId(string orderIds)
        {
            var model = new List<InvoiceCodeViewModel>();
            try
            {

                DataTable dt =  invoiceDAL.GetListInvoiceCodebyOrderId(orderIds);
                if (dt != null && dt.Rows.Count > 0)
                {
                    model = dt.ToList<InvoiceCodeViewModel>();
                    return model;
                }
                return new List<InvoiceCodeViewModel>();
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetFlyBookingPackagesOptionalsByBookingId - InvoiceRepository: " + ex.ToString());
                return new List<InvoiceCodeViewModel>();
            }
        }
        public async Task<List<InvoiceRequestViewModel>> GetListInvoiceRequestbyOrderId(string orderIds)
        {
            
            try
            {
                return invoiceRequestDAL.GetByOrderId(orderIds);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetListInvoiceRequestbyOrderId - InvoiceRepository: " + ex.ToString());
                return new List<InvoiceRequestViewModel>();
            }
        }
    }
}
