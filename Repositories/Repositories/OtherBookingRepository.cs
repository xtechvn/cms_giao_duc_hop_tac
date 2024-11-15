using Aspose.Cells;
using DAL;
using Entities.ConfigModels;
using Entities.Models;
using Entities.ViewModels;
using Entities.ViewModels.OrderManual;
using Entities.ViewModels.SetServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace Repositories.Repositories
{
    public class OtherBookingRepository : IOtherBookingRepository
    {
        private readonly OrderDAL orderDAL;
        private readonly ContactClientDAL contactClientDAL;
        private readonly OtherBookingDAL otherBookingDAL;
        private readonly OtherBookingPackagesDAL otherBookingPackagesDAL;
        private readonly OtherBookingPackagesOptionalDAL otherBookingPackagesOptionalDAL;
        private readonly AllCodeDAL AllCodeDAL;
        private readonly IIdentifierServiceRepository _identifierServiceRepository;
        private readonly IConfiguration _configuration;

        public OtherBookingRepository(IOptions<DataBaseConfig> dataBaseConfig, IIdentifierServiceRepository identifierServiceRepository, IConfiguration configuration)
        {
            orderDAL = new OrderDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            otherBookingDAL = new OtherBookingDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            otherBookingPackagesDAL = new OtherBookingPackagesDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            otherBookingPackagesOptionalDAL = new OtherBookingPackagesOptionalDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            AllCodeDAL = new AllCodeDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            contactClientDAL = new ContactClientDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            _identifierServiceRepository = identifierServiceRepository;
            _configuration = configuration;
        }
        public async Task<List<OtherBookingPackages>> GetOtherBookingPackagesByBookingId(long booking_id)
        {
            return otherBookingPackagesDAL.GetOtherBookingPackagesByBookingId(booking_id);
        }
        public async Task<List<OtherBookingPackagesOptional>> GetOtherBookingPackagesOptionalByBookingId(long booking_id)
        {
            return otherBookingPackagesOptionalDAL.GetOtherBookingPackagesOptionalByBookingId(booking_id);
        }
        public async Task<OtherBooking> GetOtherBookingById(long booking_id)
        {
            return otherBookingDAL.GetOtherBookingById(booking_id);
        }
        public async Task<long> DeleteOtherBookingById(long id)
        {
            return await otherBookingDAL.DeleteOtherBookingById(id);
        }
        public async Task<long> CancelOtherBookingById(long id, int user_id)
        {
            try
            {
                return await otherBookingDAL.CancelOtherBookingById(id, user_id);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CancelTourByID - TourRepository: " + ex);

            }
            return 0;
        }
        public async Task<long> SummitOtherBooking(OrderManualOtherBookingServiceSummitModel data, int user_summit)
        {
            try
            {
                OrderManualOtherBookingServiceSummitSQLModel summit = new OrderManualOtherBookingServiceSummitSQLModel();

                if (data.id > 0)
                {
                    summit.booking = otherBookingDAL.GetOtherBookingById(data.id);
                }
                else
                {
                    summit.booking = new OtherBooking();
                }
                double amount = data.packages.Sum(x => x.amount);
                double profit = data.packages.Sum(x => x.profit);
                double price = amount - profit;
                summit.booking.OrderId = data.order_id;
                summit.booking.Amount = amount;
                summit.booking.ServiceType = data.service_type;
                summit.booking.Profit = profit - data.commission - data.others_amount;
                summit.booking.Price = price;
                summit.booking.StartDate = data.from_date;
                summit.booking.EndDate = data.to_date;
                summit.booking.OperatorId = data.operator_id;
                summit.booking.UpdatedBy = user_summit;
                summit.booking.UpdatedDate = DateTime.Now;
                summit.booking.CreatedBy = user_summit;
                summit.booking.CreatedDate = DateTime.Now;
                summit.booking.Note = data.note;
                summit.booking.Commission = data.commission;
                summit.booking.OthersAmount = data.others_amount;
                summit.booking.ServiceCode = data.service_code;

                if (data.id > 0)
                {
                    var package_optional = otherBookingPackagesOptionalDAL.GetOtherBookingPackagesOptionalByBookingId(data.id);
                    if (package_optional != null && package_optional.Count >= 0)
                    {
                        summit.booking.Price = package_optional.Sum(x => x.Amount);

                    }
                }

                if (data.packages != null && data.packages.Count > 0)
                {
                    summit.packages = new List<OtherBookingPackages>();
                    foreach (var item in data.packages)
                    {
                        summit.packages.Add(new OtherBookingPackages()
                        {
                            Amount = Convert.ToDecimal(item.amount),
                            BasePrice = Convert.ToDecimal(item.base_price),
                            BookingId = data.id,
                            Id = item.id,
                            Name = item.package_name,
                            Profit = item.profit,
                            Quantity = item.quantity,
                            UpdatedBy = user_summit,
                            UpdatedDate = DateTime.Now,
                            SalePrice = item.sale_price,
                            Note = "",
                            ServiceType = -1
                        });
                    }
                }
                var return_value = await otherBookingDAL.CreateOrUpdateOtherBooking(summit);
                if (data.service_code == null || (!data.service_code.ToUpper().Contains("OTHER") && summit.booking.Id > 0))
                {
                    string s_format = string.Format(String.Format("{0,5:00000}", summit.booking.Id));
                    summit.booking.ServiceCode = "OTHER" + s_format;
                    await otherBookingDAL.UpdateOtherBooking(summit.booking);
                }
                return return_value;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SummitOtherBooking - OtherBookingRepository: " + ex);
            }
            return -1;

        }
        public async Task<GenericViewModel<OtherBookingSearchViewModel>> GetPagingList(SearchFlyBookingViewModel searchModel, int currentPage, int pageSize)
        {
            var model = new GenericViewModel<OtherBookingSearchViewModel>();
            try
            {

                DataTable dt = otherBookingDAL.GetPagingList(searchModel, currentPage, pageSize);
                if (dt != null && dt.Rows.Count > 0)
                {
                    model.ListData = dt.ToList<OtherBookingSearchViewModel>();
                    model.CurrentPage = currentPage;
                    model.PageSize = pageSize;
                    model.TotalRecord = Convert.ToInt32(dt.Rows[0]["TotalRow"]);
                    model.TotalPage = (int)Math.Ceiling((double)model.TotalRecord / model.PageSize);
                    return model;
                }
                return model;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetPagingList - FlyBookingDetailRepository: " + ex);
                return null;
            }
        }
        public async Task<long> UpdateServiceOperator(long booking_id, int user_id, int user_commit)
        {
            return await otherBookingDAL.UpdateServiceOperator(booking_id, user_id, user_commit);
        }
        public async Task<long> UpdateServiceOperator(long booking_id, int user_id)
        {
            return await otherBookingDAL.UpdateServiceOperator(booking_id, user_id);
        }
        public async Task<long> UpdateServiceStatus(int status, long booking_id, int user_id)
        {
            return await otherBookingDAL.UpdateServiceStatus(status, booking_id, user_id);
        }
        public async Task<long> UpdateOtherBookingOptional(List<OtherBookingPackagesOptional> data, long booking_id, int user_summit)
        {
            try
            {
                double price = 0;
                if (data != null && data.Count > 0)
                {
                    List<long> remain_list = new List<long>();
                    foreach (var item in data)
                    {
                        if (item.Note != null && item.Note.Trim() != "")
                        {
                            item.Note = CommonHelper.RemoveSpecialCharacterExceptVietnameseCharacter(item.Note);
                        }
                        if (item.Status != 1)
                            price += item.Amount > 0 ? item.Amount : 0;
                        item.CreatedBy = user_summit;
                        item.UpdatedBy = user_summit;
                        var id = await otherBookingPackagesOptionalDAL.CreateOrUpdatePackageOptional(item);
                        remain_list.Add(item.Id);
                    }
                    await otherBookingDAL.UpdateOtherBookingPrice(booking_id, price, user_summit);
                    await otherBookingPackagesOptionalDAL.RemoveNonExistsBookingOptional(remain_list, booking_id);
                    return data[0].Id;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateFlyBookingOptional - FlyBookingDetailRepository: " + ex);

            }
            return 0;

        }
        public async Task<List<OtherBookingViewModel>> GetDetailOtherBookingById(int OtherBookingId)
        {
            try
            {
                DataTable dt = await otherBookingDAL.GetDetailOtherBookingById(OtherBookingId);
                if (dt != null && dt.Rows.Count > 0)
                {
                    var data = dt.ToList<OtherBookingViewModel>();
                    return data;
                }
                return null;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CancelTourByID - TourRepository: " + ex);
                return null;
            }
        }
        public async Task<string> ExportDeposit(SearchFlyBookingViewModel searchModel, string FilePath)
        {
            var pathResult = string.Empty;
            try
            {
                var data = new List<OtherBookingSearchViewModel>();
                DataTable dt = otherBookingDAL.GetPagingList(searchModel, searchModel.PageIndex, searchModel.pageSize);
                if (dt != null && dt.Rows.Count > 0)
                {
                    data = dt.ToList<OtherBookingSearchViewModel>();

                }

                if (data != null && data.Count > 0)
                {
                    Workbook wb = new Workbook();
                    Worksheet ws = wb.Worksheets[0];
                    ws.Name = "Danh sách đặt dịch vụ khác";
                    Cells cell = ws.Cells;

                    var range = ws.Cells.CreateRange(0, 0, 1, 1);
                    StyleFlag st = new StyleFlag();
                    st.All = true;
                    Style style = ws.Cells["A1"].GetStyle();

                    #region Header
                    range = cell.CreateRange(0, 0, 1, 14);
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
                    cell.SetColumnWidth(12, 25);
                    cell.SetColumnWidth(13, 25);


                    // Set header value
                    ws.Cells["A1"].PutValue("STT");
                    ws.Cells["B1"].PutValue("Mã dịch vụ");
                    ws.Cells["C1"].PutValue("Chi tiết dịch vụ");
                    ws.Cells["D1"].PutValue("Ngày check in");
                    ws.Cells["E1"].PutValue("Ngày check out");
                    ws.Cells["F1"].PutValue("Mã đơn hàng");
                    ws.Cells["G1"].PutValue("Ngày tạo");
                    ws.Cells["H1"].PutValue("Nhân viên bán");
                    ws.Cells["I1"].PutValue("Điều hành");
                    ws.Cells["J1"].PutValue("Mã code");
                    ws.Cells["K1"].PutValue("Trạng thái");
                    ws.Cells["L1"].PutValue("Doanh thu dịch vụ");
                    ws.Cells["M1"].PutValue("Giá NET thực tế");
                    ws.Cells["N1"].PutValue("Lợi nhuận thực tế");
                    #endregion

                    #region Body

                    range = cell.CreateRange(1, 0, data.Count, 14);
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
                        ws.Cells["B" + RowIndex].PutValue(item.ServiceCode);
                        ws.Cells["C" + RowIndex].PutValue("Dịch vụ: " + (item.ServiceTypeName == null ? "" : item.ServiceTypeName));
                        ws.Cells["D" + RowIndex].PutValue(item.StartDate.ToString("dd/MM/yyyy"));
                        ws.Cells["E" + RowIndex].PutValue(item.EndDate.ToString("dd/MM/yyyy"));
                        ws.Cells["F" + RowIndex].PutValue(item.OrderNo);
                        ws.Cells["G" + RowIndex].PutValue(item.CreatedDate.ToString("dd/MM/yyyy"));
                        ws.Cells["H" + RowIndex].PutValue(item.SalerFullName);
                        ws.Cells["I" + RowIndex].PutValue(item.OperatorFullNameName);
                        ws.Cells["J" + RowIndex].PutValue(item.BookingCode);
                        ws.Cells["K" + RowIndex].PutValue(item.OtherBookingStatusName);
                        ws.Cells["L" + RowIndex].PutValue((item.Amount == null ? 0 : (double)item.Amount));
                        ws.Cells["L" + RowIndex].SetStyle(numberStyle);
                        ws.Cells["M" + RowIndex].PutValue((item.Price == null ? 0 : (double)item.Price));
                        ws.Cells["M" + RowIndex].SetStyle(numberStyle);
                        ws.Cells["N" + RowIndex].PutValue(((item.Amount == null ? 0 : (double)item.Amount) - (item.Price == null ? 0 : (double)item.Price)));
                        ws.Cells["N" + RowIndex].SetStyle(numberStyle);
                    }
                    ws.Cells.InsertColumn(5);
                    ws.Cells.CopyColumn(ws.Cells, ws.Cells.Columns[12].Index, ws.Cells.Columns[5].Index);
                    ws.Cells.DeleteColumn(12);
                    ws.Cells.InsertColumn(6);
                    ws.Cells.CopyColumn(ws.Cells, ws.Cells.Columns[13].Index, ws.Cells.Columns[6].Index);
                    ws.Cells.DeleteColumn(13);
                    ws.Cells.InsertColumn(7);
                    ws.Cells.CopyColumn(ws.Cells, ws.Cells.Columns[14].Index, ws.Cells.Columns[7].Index);
                    ws.Cells.DeleteColumn(14);
                    #endregion
                    wb.Save(FilePath);
                    pathResult = FilePath;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ExportPaymentRequest - HotelBookingRepositories: " + ex);
            }
            return pathResult;
        }
        public List<OtherBookingPackagesOptionalViewModel> GetOtherBookingPackagesOptionalByServiceId(long serviceId)
        {
            try
            {
                DataTable dt = otherBookingDAL.GetOtherBookingPackagesOptionalByServiceId(serviceId);
                if (dt != null && dt.Rows.Count > 0)
                {
                    var data = dt.ToList<OtherBookingPackagesOptionalViewModel>();
                    return data;
                }
                return new List<OtherBookingPackagesOptionalViewModel>();
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetPagingList - FlyBookingDetailRepository: " + ex);
                return new List<OtherBookingPackagesOptionalViewModel>();
            }
        }
        public async Task<List<OtherBooking>> ServiceCodeSuggesstion(string txt_search = "")
        {
            try
            {
                return await otherBookingDAL.ServiceCodeSuggesstion(txt_search);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CancelTourByID - TourRepository: " + ex);

            }
            return new List<OtherBooking>();
        }
        public async Task<List<OtherBooking>> getListOtherBookingByOrderId(long OrderId)
        {
            try
            {
                return await otherBookingDAL.getListOtherBookingByOrderId(OrderId);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("getListOtherBookingByOrderId - TourRepository: " + ex);

            }
            return new List<OtherBooking>();
        }


    }
}
