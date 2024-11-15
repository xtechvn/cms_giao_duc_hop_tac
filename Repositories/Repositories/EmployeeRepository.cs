using Aspose.Cells;
using DAL.Report;
using Entities.ConfigModels;
using Entities.ViewModels;
using Entities.ViewModels.Report;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Repositories.IRepositories;
using Repositories.Repositories.BaseRepos;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Utilities;
using Utilities.Contants;

namespace Repositories.Repositories
{
    public class EmployeeRepository : BaseRepository, IEmployeeRepository
    {
        private readonly EmployeeDAL employeeDAL;

        public EmployeeRepository(IHttpContextAccessor context, IOptions<DataBaseConfig> dataBaseConfig, IUserRepository userRepository, IConfiguration configuration) : base(context, dataBaseConfig, configuration, userRepository)
        {
            employeeDAL = new EmployeeDAL(_SqlServerConnectString);
        }

        public string ExportDeposit(SearchReportEmployeeViewModel searchModel, string FilePath, int currentPage, int pageSize)
        {
            var pathResult = string.Empty;
            try
            {
                var data = new List<SearchReportDepartmentViewModel>();
                var dt = employeeDAL.GetReportEmployeeRevenue(searchModel, currentPage, pageSize, StoreProcedureConstant.SP_Report_TotalRevenueByDepartment);
                if (dt != null && dt.Rows.Count > 0)
                {
                    data = dt.ToList<SearchReportDepartmentViewModel>();
                }
                if (data != null && data.Count > 0)
                {
                    Workbook wb = new Workbook();
                    Worksheet ws = wb.Worksheets[0];
                    ws.Name = "Doanh thu tổng theo NVBH";
                    Cells cell = ws.Cells;

                    var range = ws.Cells.CreateRange(0, 0, 1, 1);
                    StyleFlag st = new StyleFlag();
                    st.All = true;
                    Style style = ws.Cells["A1"].GetStyle();

                    #region Header
                    range = cell.CreateRange(0, 0, 1, 11);
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
                    cell.SetColumnWidth(5, 30);
                    cell.SetColumnWidth(6, 30);
                    cell.SetColumnWidth(7, 25);
                    cell.SetColumnWidth(8, 25);
                    cell.SetColumnWidth(9, 25);
                    cell.SetColumnWidth(10, 25);
                    cell.SetColumnWidth(11, 25);


                    // Set header value
                    ws.Cells["A1"].PutValue("STT");
                    ws.Cells["B1"].PutValue("Phòng ban");
                    ws.Cells["C1"].PutValue("Số lượng đơn hàng");
                    ws.Cells["D1"].PutValue("Giá thu khách hàng");
                    ws.Cells["E1"].PutValue("Giá thanh toán NCC");
                    ws.Cells["F1"].PutValue("Hoa hồng CTV");
                    ws.Cells["G1"].PutValue("Lợi nhuận (có VAT)");
                    ws.Cells["H1"].PutValue("Tỷ suất");
                    ws.Cells["I1"].PutValue("Doanh thu");
                    ws.Cells["J1"].PutValue("Giá vốn");
                    ws.Cells["k1"].PutValue("Lợi nhuận");

                    #endregion

                    #region Body

                    range = cell.CreateRange(1, 0, data.Count, 11);
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

                    Style numberStyle = ws.Cells["I2"].GetStyle();
                    numberStyle.Number = 3;
                    numberStyle.HorizontalAlignment = TextAlignmentType.Right;
                    numberStyle.VerticalAlignment = TextAlignmentType.Center;

                    int RowIndex = 1;

                    foreach (var item in data)
                    {

                        RowIndex++;
                        ws.Cells["A" + RowIndex].PutValue(RowIndex - 1);
                        ws.Cells["A" + RowIndex].SetStyle(alignCenterStyle);
                        ws.Cells["B" + RowIndex].PutValue(item.DepartmentName);
                        ws.Cells["C" + RowIndex].PutValue(item.TotalOrder);
                        ws.Cells["D" + RowIndex].PutValue(item.Price.ToString("N0"));
                        ws.Cells["E" + RowIndex].PutValue(item.Amount.ToString("N0"));
                        ws.Cells["F" + RowIndex].PutValue(item.Comission.ToString("N0"));
                        ws.Cells["G" + RowIndex].PutValue(item.Profit.ToString("N0"));
                        ws.Cells["H" + RowIndex].PutValue(item.Percent.ToString("N2") + "%");
                        ws.Cells["I" + RowIndex].PutValue(item.AmountVat.ToString("N0"));
                        ws.Cells["J" + RowIndex].PutValue(item.PriceVat.ToString("N0"));
                        ws.Cells["k" + RowIndex].PutValue(item.ProfitVat.ToString("N0"));

                    }

                    #endregion
                    wb.Save(FilePath);
                    pathResult = FilePath;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ExportDeposit - DepartmentRepository: " + ex);
            }
            return pathResult;
        }

        public GenericViewModel<ReportEmployeeViewModel> GetEmployeeRevenue(SearchReportEmployeeViewModel searchModel
            , int currentPage, int pageSize)
        {
            var model = new GenericViewModel<ReportEmployeeViewModel>();
            try
            {
                var dt = employeeDAL.GetReportEmployeeRevenue(searchModel, currentPage, pageSize, StoreProcedureConstant.SP_Report_TotalRevenueByDepartment);
                if (dt != null && dt.Rows.Count > 0)
                {

                    model.ListData = dt.ToList<ReportEmployeeViewModel>();
                    model.PageSize = (int)searchModel.PageSize;
                    model.CurrentPage = (int)searchModel.PageIndex;
                    model.TotalRecord = dt.Rows[0]["TotalRow"].Equals(DBNull.Value) ? 0 : Convert.ToInt32(dt.Rows[0]["TotalRow"].ToString());
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetReportDepartment - DepartmentRepository: " + ex);
            }
            return model;
        }
    }
}
