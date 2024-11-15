using Aspose.Cells;
using DAL.Funding;
using DAL.StoreProcedure;
using Entities.ConfigModels;
using Entities.ViewModels;
using Entities.ViewModels.Funding;
using Microsoft.Extensions.Options;
using Nest;
using Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Utilities;
using Utilities.Contants;

namespace Repositories.Repositories
{
    public class DebtStatisticRepository : IDebtStatisticRepository
    {
        private readonly DebtStatisticDAL debtStatisticDAL;

        public DebtStatisticRepository(IOptions<DataBaseConfig> dataBaseConfig)
        {
            debtStatisticDAL = new DebtStatisticDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
        }

        public int CreateDebtStatistic(DebtStatisticViewModel model)
        {
            var paymentRequest = debtStatisticDAL.GetByCode(model.Code);
            if (paymentRequest != null)
                return -2;
            return debtStatisticDAL.CreateDebtStatistic(model);
        }

        public int UpdateDebtStatistic(DebtStatisticViewModel model)
        {
            return debtStatisticDAL.UpdateDebtStatistic(model);
        }

        public string ExportDebtStatistic(DebtStatisticViewModel searchModel, string FilePath)
        {
            var pathResult = string.Empty;
            try
            {
                var listPaymentRequests = GetDebtStatistics(searchModel, out long total, 1, -1);

                if (listPaymentRequests != null && listPaymentRequests.Count > 0)
                {
                    Workbook wb = new Workbook();
                    Worksheet ws = wb.Worksheets[0];
                    ws.Name = "Danh sách bảng kê";
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
                    // Set header value
                    ws.Cells["A1"].PutValue("STT");
                    ws.Cells["B1"].PutValue("Mã phiếu");
                    ws.Cells["C1"].PutValue("Khách hàng");
                    ws.Cells["D1"].PutValue("Thời gian");
                    ws.Cells["E1"].PutValue("Số tiền");
                    ws.Cells["F1"].PutValue("Nội dung");
                    ws.Cells["G1"].PutValue("Trạng thái");
                    ws.Cells["H1"].PutValue("Ngày tạo");
                    ws.Cells["I1"].PutValue("Người tạo");
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
                        ws.Cells["B" + RowIndex].PutValue(item.Code);
                        ws.Cells["C" + RowIndex].PutValue(item.ClientName);
                        ws.Cells["D" + RowIndex].PutValue(item.FromDateStr + " - " + item.ToDateStr);
                        ws.Cells["E" + RowIndex].SetStyle(numberStyle);
                        ws.Cells["E" + RowIndex].PutValue(item.AmountPay.ToString("N0") + "/" + item.Amount.Value.ToString("N0"));
                        ws.Cells["F" + RowIndex].PutValue(item.Note);
                        ws.Cells["G" + RowIndex].PutValue(item.StatusName);
                        ws.Cells["H" + RowIndex].PutValue(item.CreatedDate.Value.ToString("dd/MM/yyyy"));
                        ws.Cells["I" + RowIndex].PutValue(item.UserCreateName);
                    }

                    #endregion
                    wb.Save(FilePath);
                    pathResult = FilePath;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ExportPaymentRequest - DebtStatisticRepository: " + ex);
            }
            return pathResult;
        }

        public string ExportDebtStatistic_OrderList(DebtStatisticViewModel model, string FilePath)
        {
            var pathResult = string.Empty;
            try
            {
                var detail = GetDetailDebtStatistic(model.Id);

                if (detail.RelateData != null && detail.RelateData.Count > 0)
                {
                    Workbook wb = new Workbook();
                    Worksheet ws = wb.Worksheets[0];
                    ws.Name = "Danh sách đơn hàng bảng kê";
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
                    // Set header value
                    ws.Cells["A1"].PutValue("STT");
                    ws.Cells["B1"].PutValue("Mã đơn hàng");
                    ws.Cells["C1"].PutValue("Ngày tạo");
                    ws.Cells["D1"].PutValue("Số tiền");
                    ws.Cells["E1"].PutValue("Đã thanh toán");
                    ws.Cells["F1"].PutValue("Chưa thanh toán");
                    #endregion

                    #region Body

                    range = cell.CreateRange(1, 0, detail.RelateData.Count, 15);
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

                    foreach (var item in detail.RelateData)
                    {
                        RowIndex++;
                        ws.Cells["A" + RowIndex].PutValue(RowIndex - 1);
                        ws.Cells["A" + RowIndex].SetStyle(alignCenterStyle);
                        ws.Cells["B" + RowIndex].PutValue(item.OrderNo);
                        ws.Cells["C" + RowIndex].PutValue(item.CreateDateStr);
                        ws.Cells["D" + RowIndex].SetStyle(numberStyle);
                        ws.Cells["D" + RowIndex].PutValue(item.Amount.Value.ToString("N0"));
                        ws.Cells["E" + RowIndex].SetStyle(numberStyle);
                        ws.Cells["E" + RowIndex].PutValue(item.Payment.ToString("N0"));
                        ws.Cells["F" + RowIndex].SetStyle(numberStyle);
                        ws.Cells["F" + RowIndex].PutValue(item.NotPayment.ToString("N0"));
                    }

                    #endregion
                    wb.Save(FilePath);
                    pathResult = FilePath;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ExportPaymentRequest - DebtStatisticRepository: " + ex);
            }
            return pathResult;
        }

        public DebtStatisticViewModel GetDetailDebtStatistic(int debtStatisticId)
        {
            try
            {
                var requestInfos = debtStatisticDAL.GetRequestDetail(debtStatisticId,
                    ProcedureConstants.SP_GetDetailDebtStatistic).ToList<DebtStatisticViewModel>();
                var requestInfo = requestInfos.FirstOrDefault();
                requestInfo.RelateData = new List<DebtStatisticViewModel>();
                var listOrder = GetOrderListByClientId(requestInfo.ClientId.Value, requestInfo.FromDate.Value, requestInfo.ToDate.Value, requestInfo.Id, true);
                foreach (var item in listOrder)
                {
                    DebtStatisticViewModel model = new DebtStatisticViewModel();
                    item.CopyProperties(model);
                    model.NotPayment = model.Amount.Value - model.AmountPay;
                    requestInfo.RelateData.Add(model);
                }
                return requestInfo;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetCountStatus - DebtStatisticRepository: " + ex);
                return new DebtStatisticViewModel();
            }
        }

        public List<CountStatus> GetCountStatus(DebtStatisticViewModel searchModel)
        {
            try
            {
                var listPaymentRequests = debtStatisticDAL.GetCountStatus(searchModel, ProcedureConstants.SP_CountListDebtStatistic).ToList<CountStatus>();
                return listPaymentRequests;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetCountStatus - DebtStatisticRepository: " + ex);
                return new List<CountStatus>();
            }
        }

        public List<DebtStatisticViewModel> GetDebtStatistics(DebtStatisticViewModel searchModel, out long total, int currentPage = 1, int pageSize = 20)
        {
            total = 0;
            try
            {
                var listDebtStatistic = debtStatisticDAL.GetPagingList(searchModel, currentPage, pageSize,
                ProcedureConstants.SP_GetListDebtStatistic).ToList<DebtStatisticViewModel>();

                //foreach (var item in listPaymentRequests)
                //{

                //}
                return listDebtStatistic;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetDebtStatistics - DebtStatisticRepository: " + ex);
                return new List<DebtStatisticViewModel>();
            }
        }

        public List<DebtStatisticViewModel> GetOrderListByClientId(long clientId, DateTime fromDate, DateTime toDate, int debtStatisticId = 0, bool isDetail = false)
        {
            try
            {
                var listOutput = new List<DebtStatisticViewModel>();
                var listOrderListByClientId = debtStatisticDAL.GetOrderListByClientId(clientId, fromDate, toDate,
                    ProcedureConstants.SP_GetListOrder, isDetail).ToList<DebtStatisticViewModel>();
                listOutput = listOrderListByClientId;
                if (debtStatisticId != 0)
                {
                    listOutput = new List<DebtStatisticViewModel>();
                    var model = debtStatisticDAL.GetById(debtStatisticId);
                    foreach (var item in listOrderListByClientId)
                    {
                        if (!string.IsNullOrEmpty(model.OrderIds) && model.OrderIds.Contains(item.OrderId.ToString()))
                            item.IsChecked = true;
                    }
                    if (isDetail)
                    {
                        foreach (var item in listOrderListByClientId)
                        {
                            if (!string.IsNullOrEmpty(model.OrderIds) && model.OrderIds.Contains(item.OrderId.ToString()))
                            {
                                listOutput.Add(item);
                            }
                        }
                    }
                    else
                    {
                        listOutput = listOrderListByClientId;
                    }
                }
                return listOutput;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetCountStatus - DebtStatisticRepository: " + ex);
                return new List<DebtStatisticViewModel>();
            }
        }

        public int ApproveDebtStatistic(string debtStatisticNo, int userId, int status)
        {
            try
            {
                var debtInfo = debtStatisticDAL.GetByCode(debtStatisticNo);
                debtInfo.UpdatedBy = userId;
                debtInfo.Status = status;
                debtStatisticDAL.UpdateDebtStatisticStatus(debtInfo);
                return 1;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ApproveDebtStatistic - DebtStatisticRepository. " + ex);
                return -1;
            }
        }

        public int RejectDebtStatistic(string debtStatisticNo, string noteReject, int userId)
        {
            try
            {
                var debtInfo = debtStatisticDAL.GetByCode(debtStatisticNo);
                debtInfo.UpdatedBy = userId;
                debtInfo.DeclineReason = noteReject;
                debtInfo.Status = (int)DEBT_STATISTIC_STATUS.TU_CHOI;
                debtStatisticDAL.UpdateDebtStatisticStatus(debtInfo);
                return 1;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ApproveDebtStatistic - DebtStatisticRepository. " + ex);
                return -1;
            }
        }

        public int CancelDebtStatistic(string debtStatisticNo, int userId)
        {
            try
            {
                var debtInfo = debtStatisticDAL.GetByCode(debtStatisticNo);
                debtInfo.UpdatedBy = userId;
                debtInfo.Status = (int)DEBT_STATISTIC_STATUS.HUY;
                debtStatisticDAL.UpdateDebtStatisticStatus(debtInfo);
                return 1;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ApproveDebtStatistic - DebtStatisticRepository. " + ex);
                return -1;
            }
        }

        public int DeleteDebtStatistic(string debtStatisticNo, int userId)
        {
            try
            {
                var debtInfo = debtStatisticDAL.GetByCode(debtStatisticNo);
                debtInfo.UpdatedBy = userId;
                debtInfo.IsDelete = true;
                debtStatisticDAL.UpdateDebtStatisticStatus(debtInfo);
                return 1;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("DeleteDebtStatistic - DebtStatisticRepository. " + ex);
                return -1;
            }
        }

        public string CheckApproveDebtStatistic(string debtStatisticNo, out string orderNoList)
        {
            orderNoList = string.Empty;
            var model = debtStatisticDAL.GetByCode(debtStatisticNo);
            var result = debtStatisticDAL.CheckApproveDebtStatistic(model.OrderIds,
                StoreProcedureConstant.SP_CheckExistsDebtStatisticByOrderId).ToList<DebtStatisticViewModel>();
            if (result.Count > 0)
            {
                orderNoList = string.Join(",", result.Select(n => n.Code).ToList());
                return string.Join(",", result.Select(n => n.OrderNo).ToList());
            }
            return string.Empty;
        }
    }
}
