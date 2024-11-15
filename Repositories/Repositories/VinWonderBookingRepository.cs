using Aspose.Cells;
using DAL;
using Entities.ConfigModels;
using Entities.Models;
using Entities.ViewModels;
using Entities.ViewModels.OrderManual;
using Entities.ViewModels.SetServices;
using Entities.ViewModels.VinWonder;
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

namespace Repositories.Repositories
{
    public class VinWonderBookingRepository : IVinWonderBookingRepository
    {
        private readonly VinWonderBookingDAL _vinWonderBookingDAL;
        private readonly VinWonderBookingTicketDAL _vinWonderBookingTicketDAL;
        private readonly VinWonderBookingTicketCustomerDAL _vinWonderBookingTicketCustomerDAL;
        private readonly OrderDAL _orderDAL;
        private readonly ClientDAL _clientDAL;
        public VinWonderBookingRepository(IOptions<DataBaseConfig> dataBaseConfig)
        {
            var _StrConnection = dataBaseConfig.Value.SqlServer.ConnectionString;
            _vinWonderBookingDAL = new VinWonderBookingDAL(_StrConnection);
            _vinWonderBookingTicketDAL = new VinWonderBookingTicketDAL(_StrConnection);
            _vinWonderBookingTicketCustomerDAL = new VinWonderBookingTicketCustomerDAL(_StrConnection);
            _orderDAL = new OrderDAL(_StrConnection);
            _clientDAL = new ClientDAL(_StrConnection);
        }
        public VinWonderBooking GetVinWonderBookingById(long booking_id)
        {
            return _vinWonderBookingDAL.GetVinWonderBookingById(booking_id);

        }
        public async Task<List<VinWonderBookingTicket>> GetVinWonderTicketByBookingId(long booking_id)
        {
            return await _vinWonderBookingTicketDAL.GetVinWonderTicketByBookingId(booking_id);
        }
        public async Task<List<VinWonderBookingTicket>> GetVinWonderTicketByBookingIdSP(long booking_id)
        {
            try
            {
                DataTable dt = await _vinWonderBookingTicketDAL.GetVinWonderTicketByBookingIdSP(booking_id);
                if (dt != null && dt.Rows.Count > 0)
                {
                    var data = dt.ToList<VinWonderBookingTicket>();
                    return data;
                }
                return null;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetVinWonderTicketByBookingIdSP - VinWonderBookingRepository: " + ex.ToString());
                return null;
            }

        }
        public async Task<List<VinWonderBookingTicketCustomer>> GetVinWonderTicketCustomerByBookingIdSP(long booking_id)
        {
            try
            {
                DataTable dt = await _vinWonderBookingTicketCustomerDAL.GetVinWonderTicketCustomerByBookingIdSP(booking_id);
                if (dt != null && dt.Rows.Count > 0)
                {
                    var data = dt.ToList<VinWonderBookingTicketCustomer>();
                    return data;
                }
                return null;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetVinWonderTicketByBookingIdSP - VinWonderBookingRepository: " + ex.ToString());
                return null;
            }

        }
        public async Task<long> SummitVinWonderServiceData(OrderManualVinWonderBookingServiceSummitModel data, int user_id)
        {

            try
            {
                var order = _orderDAL.GetByOrderId(data.order_id);
                if (order == null || order.OrderId <= 0) return -1;
                var client = await _clientDAL.GetClientDetail((long)order.ClientId);
                OrderManualVinWonderBookingServiceSummitSQLModel summit_model = new OrderManualVinWonderBookingServiceSummitSQLModel()
                {
                    guests = new List<VinWonderBookingTicketCustomer>(),
                    packages = new List<VinWonderBookingTicket>(),
                    detail = new VinWonderBooking()
                };
                double amount = 0;
                double profit = 0;
                if (data.packages.Count > 0)
                {
                    foreach (var p in data.packages)
                    {
                        amount += p.amount;
                        profit += p.profit;
                        summit_model.packages.Add(new VinWonderBookingTicket()
                        {
                            Amount = p.amount,
                            BasePrice = (p.amount - p.profit) / p.quantity,
                            DateUsed = p.date_used,
                            CreatedDate = DateTime.Now,
                            Id = p.id,
                            Quantity = p.quantity,
                            BookingId = data.id,
                            UpdatedBy = user_id,
                            UpdatedDate = DateTime.Now,
                            UnitPrice = p.amount - p.profit,
                            Profit = p.profit,
                            Adt=0,
                            Child=0,
                            CreatedBy= user_id,
                            Name=p.package_name,
                            Old=0,
                            RateCode="",
                            TotalPrice= p.amount - p.profit ,
                            
                            
                        });
                    }
                }
                int status = 0;
                var exists_booking =  _vinWonderBookingDAL.GetVinWonderBookingById(data.id);
                if (exists_booking != null && exists_booking.Status != null)
                {
                    status = (int)exists_booking.Status;
                   
                }
                var booking_by_servicecode = _vinWonderBookingDAL.GetVinWonderByServiceCode(data.service_code);
                if(booking_by_servicecode!=null && booking_by_servicecode.Id > 0)
                {

                }
                VinWonderBooking vinwonder_product = new VinWonderBooking()
                {
                    Id = data.id<=0?0:data.id,
                    CreatedBy = user_id,
                    CreatedDate = DateTime.Now,
                    ServiceCode = data.service_code,
                    Status = 0,
                    SupplierId = 0,
                    UpdatedBy = user_id,
                    UpdatedDate = DateTime.Now,
                    AdavigoBookingId= exists_booking==null || exists_booking.AdavigoBookingId==null || exists_booking.AdavigoBookingId.Trim()==""?"": exists_booking.AdavigoBookingId,
                    Amount = amount,
                    Note=data.note,
                    OrderId=data.order_id,
                    SalerId=data.operator_id,
                    SiteCode=data.location_id.ToString(),
                    SiteName=data.location_name,
                    TotalPrice = amount-profit,
                    TotalProfit=profit - data.commission - data.others_amount,
                    TotalUnitPrice=amount-profit,
                    Commission=data.commission,
                    OthersAmount=data.others_amount
                };
     
                summit_model.detail = vinwonder_product;
                if (data.guest != null && data.guest.Count > 0)
                {
                    foreach (var g in data.guest)
                    {
                        summit_model.guests.Add(new VinWonderBookingTicketCustomer()
                        {
                            CreatedBy = user_id,
                            CreatedDate = DateTime.Now,
                            UpdatedBy = user_id,
                            UpdatedDate = DateTime.Now,
                            FullName = g.name ?? "",
                            Note = g.note ?? "",
                            Phone = g.phone??"",
                            BookingId = summit_model.detail.Id,
                            Id = g.id,
                            Email = g.email ?? "",
                            Genre="",
                            Birthday=DateTime.Now,
                            OtherDetail=""
                        });
                    }

                }
                var id = await _vinWonderBookingDAL.SummitData(summit_model);
               // await _vinWonderBookingDAL.InsertListVinWonderPackagesOptional(summit_model.extra_packages);
                return id;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SummitTourServiceData - TourRepository: " + ex.ToString());
                return -2;
            }
        }
        public async Task<long> DeleteVinWonderBookingByID(long id)
        {
            try
            {
                return await _vinWonderBookingDAL.DeleteVinWonderBookingByID(id);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("DeleteVinWonderBookingByID - VinWonderBookingRepository: " + ex);

            }
            return 0;
        }
        public async Task<long> CancelVinWonderByID(long id, int user_id)
        {
            try
            {
                return await _vinWonderBookingDAL.CancelVinWonderByID(id, user_id);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CancelVinWonderByID - VinWonderBookingRepository: " + ex);

            }
            return 0;
        }
        public async Task<List<VinWonderDetailViewModel>> GetDetailVinWonderByBookingId(long BookingId)
        {
            try
            {
                DataTable dt = await _vinWonderBookingDAL.GetDetailVinWonderByBookingId(BookingId);
                if(dt!=null && dt.Rows.Count > 0)
                {
                    var data = dt.ToList<VinWonderDetailViewModel>();
                    return data;
                }
                return null ;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CancelVinWonderByID - VinWonderBookingRepository: " + ex);
                return null;
            }
      
        }
        
        public async Task<GenericViewModel<VinWonderBookingSearchViewModel>> GetPagingList(SearchFlyBookingViewModel searchModel, int currentPage, int pageSize)
        {
            var model = new GenericViewModel<VinWonderBookingSearchViewModel>();
            try
            {

                DataTable dt = _vinWonderBookingDAL.GetPagingList(searchModel, currentPage, pageSize);
                if (dt != null && dt.Rows.Count > 0)
                {
                    model.ListData = dt.ToList<VinWonderBookingSearchViewModel>();
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
                LogHelper.InsertLogTelegram("GetPagingList - VinWonderBookingRepository: " + ex);
                return null;
            }
        }
        public async Task<long> UpdateServiceOperator(long booking_id, int user_id, int user_commit)
        {
            return await _vinWonderBookingDAL.UpdateServiceOperator(booking_id, user_id, user_commit);
        }
        public async Task<long> UpdateVinWonderTicketOperatorPrice(List<VinWonderBookingTicket> data, int user_summit)
        {
           
            try
            {
               var result = await _vinWonderBookingTicketDAL.UpdateVinWonderTicketOperatorPrice(data, user_summit);
               var booking =  _vinWonderBookingDAL.GetVinWonderBookingById((long)data[0].BookingId);
                booking.TotalUnitPrice = data.Sum(x => x.UnitPrice);
                _vinWonderBookingDAL.Update(booking);
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateVinWonderTicketOperatorPrice - VinWonderBookingRepository: " + ex);
                return -1;
            }

        }
        public async Task<long> UpdateServiceOperator(long booking_id, int user_id)
        {
            return await _vinWonderBookingDAL.UpdateServiceOperator(booking_id, user_id);
        }
        public async Task<long> UpdateServiceStatus(int status, long booking_id, int user_id)
        {
            return await _vinWonderBookingDAL.UpdateServiceStatus(status, booking_id, user_id);
        }
        public async Task<string> ExportDeposit(SearchFlyBookingViewModel searchModel, string FilePath)
        {
            var pathResult = string.Empty;
            try
            {
                var data = new List<VinWonderBookingSearchViewModel>();
                DataTable dt = _vinWonderBookingDAL.GetPagingList(searchModel, -1, searchModel.pageSize);
                if (dt != null && dt.Rows.Count > 0)
                {
                    data = dt.ToList<VinWonderBookingSearchViewModel>();
                   
                }
               

                if (data != null && data.Count > 0)
                {
                    Workbook wb = new Workbook();
                    Worksheet ws = wb.Worksheets[0];
                    ws.Name = "Danh sách đặt dịch Vinwonder";
                    Cells cell = ws.Cells;

                    var range = ws.Cells.CreateRange(0, 0, 1, 1);
                    StyleFlag st = new StyleFlag();
                    st.All = true;
                    Style style = ws.Cells["A1"].GetStyle();

                    #region Header
                    range = cell.CreateRange(0, 0, 1, 13);
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


                    // Set header value
                    ws.Cells["A1"].PutValue("STT");
                    ws.Cells["B1"].PutValue("Mã dịch vụ");
                    ws.Cells["C1"].PutValue("Chi tiết dịch vụ");
                    ws.Cells["D1"].PutValue("Mã đơn hàng");
                    ws.Cells["E1"].PutValue("Ngày bắt đầu");
                    ws.Cells["F1"].PutValue("Ngày tạo");
                    ws.Cells["G1"].PutValue("Nhân viên bán");
                    ws.Cells["H1"].PutValue("Điều hành");
                    ws.Cells["I1"].PutValue("Mã code");
                    ws.Cells["J1"].PutValue("Trạng thái");
                    ws.Cells["K1"].PutValue("Doanh thu dịch vụ");
                    ws.Cells["L1"].PutValue("Giá NET thực tế");
                    ws.Cells["M1"].PutValue("Lợi nhuận thực tế");
                    #endregion

                    #region Body

                    range = cell.CreateRange(1, 0, data.Count, 13);
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
                        ws.Cells["C" + RowIndex].PutValue("Địa điểm: "+ (item.SiteName == null ? "" : item.SiteName));
                        ws.Cells["D" + RowIndex].PutValue(item.OrderNo);
                        ws.Cells["E" + RowIndex].PutValue(item.StartDate.ToString("dd/MM/yyyy HH:mm"));
                        ws.Cells["F" + RowIndex].PutValue(item.CreatedDate.ToString("dd/MM/yyyy HH:mm"));
                        ws.Cells["G" + RowIndex].PutValue(item.SalerName);
                        ws.Cells["H" + RowIndex].PutValue(item.OperatorName);
                        ws.Cells["I" + RowIndex].PutValue(item.BookingCode);
                        ws.Cells["J" + RowIndex].PutValue(item.VinWonderBookingStatusName);
                        ws.Cells["K" + RowIndex].PutValue((item.Amount == null ? 0 : (double)item.Amount));
                        ws.Cells["K" + RowIndex].SetStyle(numberStyle);
                        ws.Cells["L" + RowIndex].PutValue((item.Price == null ? 0 : (double)item.Price));
                        ws.Cells["L" + RowIndex].SetStyle(numberStyle);
                        ws.Cells["M" + RowIndex].PutValue(((item.Amount == null ? 0 : (double)item.Amount) - (item.Price == null ? 0 : (double)item.Price)));
                        ws.Cells["M" + RowIndex].SetStyle(numberStyle);
                    }
                    ws.Cells.InsertColumn(5);
                    ws.Cells.CopyColumn(ws.Cells, ws.Cells.Columns[11].Index, ws.Cells.Columns[5].Index);
                    ws.Cells.DeleteColumn(11);
                    ws.Cells.InsertColumn(6);
                    ws.Cells.CopyColumn(ws.Cells, ws.Cells.Columns[12].Index, ws.Cells.Columns[6].Index);
                    ws.Cells.DeleteColumn(12);
                    ws.Cells.InsertColumn(7);
                    ws.Cells.CopyColumn(ws.Cells, ws.Cells.Columns[13].Index, ws.Cells.Columns[7].Index);
                    ws.Cells.DeleteColumn(13);
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
        public async Task<List<ListVinWonderemialViewModel>> GetVinWonderBookingEmailByOrderID(long orderid)
        {
            try
            {
                var result = await _vinWonderBookingDAL.GetVinWonderBookingEmailByOrderID(orderid);
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetVinWonderBookingEmailByOrderID - VinWonderBookingRepository: " + ex);
                return null;
            }
        }
        public async Task<List<VinWonderBooking>> GetVinWonderBookingByOrderId(long orderid)
        {
            try
            {
                var result = await _vinWonderBookingDAL.GetVinWonderBookingByOrderId(orderid);
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetVinWonderBookingByOrderID - VinWonderBookingRepository: " + ex);
                return null;
            }
        }
        public async Task<List<VinWonderBookingTicket>> GetVinWonderBookingTicketByBookingID(long BookingId)
        {
            try
            {
                var result = await _vinWonderBookingDAL.GetVinWonderBookingTicketByBookingID(BookingId);
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetVinWonderBookingTicketByBookingID - VinWonderBookingRepository: " + ex);
                return null;
            }
        }
        public async Task<List<VinWonderBookingTicketCustomer>> GetVinWondeCustomerByBookingId(long BookingId)
        {
            try
            {
                var result = await _vinWonderBookingDAL.GetVinWondeCustomerByBookingId(BookingId);
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetVinWonderBookingTicketByBookingID - VinWonderBookingRepository: " + ex);
                return null;
            }
        }
    }
}
