using Aspose.Cells;
using Caching.Elasticsearch;
using DAL;
using DAL.Funding;
using DAL.StoreProcedure;
using Entities.ConfigModels;
using Entities.Models;
using Entities.ViewModels;
using Entities.ViewModels.Funding;
using Entities.ViewModels.HotelBooking;
using Entities.ViewModels.HotelBookingRoom;
using Entities.ViewModels.OrderManual;
using Entities.ViewModels.Report;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace Repositories.Repositories
{
    public class HotelBookingRepositories : IHotelBookingRepositories
    {

        private readonly HotelBookingDAL _hotelBookingDAL;
        private readonly ContactClientDAL _contactClientDAL;
        private readonly HotelDAL _hotelDAL;
        private readonly OrderDAL _orderDAL;
        private readonly ClientDAL _clientDAL;
        private readonly HotelBookingRoomDAL _hotelBookingRoomDAL;
        private HotelESRepository _hotelESRepository;
        private IConfiguration _configuration;
        private IESRepository<HotelESViewModel> _ESRepository;


        public HotelBookingRepositories(IOptions<DataBaseConfig> dataBaseConfig, IConfiguration configuration)
        {
            _configuration = configuration;
            _hotelBookingDAL = new HotelBookingDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            _contactClientDAL = new ContactClientDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            _hotelDAL = new HotelDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            _orderDAL = new OrderDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            _clientDAL = new ClientDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            _hotelBookingRoomDAL = new HotelBookingRoomDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            _hotelESRepository = new HotelESRepository(_configuration["DataBaseConfig:Elastic:Host"]);
            _ESRepository = new ESRepository<HotelESViewModel>(_configuration["DataBaseConfig:Elastic:Host"]);

        }
        public async Task<List<HotelBookingViewModel>> GetListByOrderId(long orderId)
        {
            try
            {
                return await _hotelBookingDAL.GetListByOrderId(orderId);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetListByOrderId - HotelBookingRepositories. " + ex);
                return null;
            }
        }
        public async Task<HotelBooking> GetHotelBookingByID(long id)
        {
            try
            {
                return await _hotelBookingDAL.GetHotelBookingByID(id);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetHotelBookingByID - HotelBookingRepositories. " + ex);
                return null;
            }
        }
        public async Task<long> UpdateHotelBooking(OrderManualHotelSerivceSummitHotel data, HotelESViewModel hotel_detail, int user_id, int is_debt_able)
        {
            try
            {
                var hotel = _hotelDAL.GetByHotelId(hotel_detail.hotelid);
                var hotel_booking = await _hotelBookingDAL.GetHotelBookingByID(data.hotel.id);
                var order = _orderDAL.GetByOrderId(data.order_id);
                if (order == null || order.OrderId <= 0) return -1;
                var client = await _clientDAL.GetClientDetail((long)order.ClientId);
                short status = 0;
                if (hotel_booking != null && hotel_booking.Status != null)
                {
                    status = (short)hotel_booking.Status;
                    /*
                    if ((status == (int)ServiceStatus.Decline && is_debt_able == (int)DebtType.DEBT_ACCEPTED) || (status == (int)ServiceStatus.Decline && client.ClientType == (int)ClientType.kl))
                    {
                        status = (int)ServiceStatus.WaitingExcution;
                    }*/
                }
                OrderManualHotelServiceSQLSummitModel model_summit = new OrderManualHotelServiceSQLSummitModel()
                {
                    order_id = data.order_id,
                    detail = new Entities.Models.HotelBooking()
                    {
                        Address = hotel_detail.street,
                        ArrivalDate = data.hotel.arrive_date == null || data.hotel.arrive_date <= DateTime.MinValue ? new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 11, 0, 0) : data.hotel.arrive_date,
                        BookingId = "",
                        CheckinTime = hotel_detail.checkintime == null ? new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 10, 0, 0) : hotel_detail.checkintime,
                        CheckoutTime = hotel_detail.checkouttime == null ? new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 14, 0, 0) : hotel_detail.checkouttime,
                        DepartureDate = data.hotel.departure_date == null || data.hotel.departure_date <= DateTime.MinValue ? new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 11, 0, 0) : data.hotel.departure_date,
                        Email = hotel_detail.email,
                        HotelName = hotel_detail.name,
                        HotelType = null,
                        ImageThumb = hotel_detail.imagethumb,
                        NumberOfAdult = data.hotel.number_of_adult,
                        NumberOfChild = data.hotel.number_of_child,
                        NumberOfInfant = data.hotel.number_of_infant,
                        NumberOfRoom = data.hotel.number_of_rooms,
                        OrderId = data.order_id,
                        PropertyId = hotel_detail.hotelid,
                        ReservationCode = "",
                        Status = status,
                        Telephone = hotel_detail.telephone,
                        TotalAmount = 0,
                        TotalPrice = 0,
                        TotalProfit = 0,
                        ExtraPackageAmount = 0,
                        Id = data.hotel.id,
                        SalerId = data.hotel.main_staff_id,
                        CreatedBy = user_id,
                        CreatedDate = DateTime.Now,
                        ServiceCode = data.hotel.service_code,
                        Price = 0,
                        SupplierId = hotel != null ? hotel.SupplierId : null,
                        NumberOfPeople = data.hotel.number_of_adult + data.hotel.number_of_child + data.hotel.number_of_infant,
                        UpdatedBy = user_id,
                        UpdatedDate = DateTime.Now,
                        Note = data.hotel.note == null ? "" : data.hotel.note,
                        TotalDiscount = data.hotel.discount,
                        TotalOthersAmount = data.hotel.other_amount
                    },
                    rooms = new List<OrderManualHotelServiceSQLSummitModelRoom>()
                    {

                    }
                };

                double total_amount = 0;
                double total_profit = 0;
                double total_price = 0;
                double extra_package_amount_total = 0;
                double extra_package_profit_total = 0;

                foreach (var r in data.rooms)
                {
                    double room_amount = 0;
                    double room_profit = 0;
                    double room_price = 0;
                    var room = new OrderManualHotelServiceSQLSummitModelRoom()
                    {
                        detail = new HotelBookingRooms(),
                        guests = new List<Entities.Models.HotelGuest>(),
                        rates = new List<Entities.Models.HotelBookingRoomRates>()
                    };
                    foreach (var rate in r.package)
                    {
                        total_amount += rate.amount;
                        total_profit += rate.profit;
                        total_price += (rate.amount - rate.profit);

                        room_amount += rate.amount;
                        room_profit += rate.profit;
                        room_price += (rate.amount - rate.profit);
                        string rate_id = CommonHelper.RemoveUnicode(rate.package_code).Trim().Replace(" ", "-");
                        room.rates.Add(new Entities.Models.HotelBookingRoomRates()
                        {
                            AllotmentId = "",
                            PackagesInclude = "",
                            Price = (rate.amount - rate.profit),
                            Profit = rate.profit,
                            TotalAmount = rate.amount,
                            RatePlanCode = rate.package_code,
                            RatePlanId = rate_id.Substring(0, rate_id.Length < 40 ? rate_id.Length : 40),
                            StayDate = rate.from,
                            Id = rate.id,
                            HotelBookingRoomId = r.id,
                            UpdatedBy = user_id,
                            UpdatedDate = DateTime.Now,
                            CreatedBy = user_id,
                            CreatedDate = DateTime.Now,
                            UnitPrice = (rate.amount - rate.profit),
                            EndDate = rate.to,
                            Nights = rate.nights,
                            OperatorPrice = rate.operator_price,
                            SalePrice = rate.sale_price,
                            StartDate = rate.from,
                        });
                    }


                    int room_adult = 0;
                    int room_child = 0;
                    int room_infant = 0;
                    if (data.guest != null && data.guest.Count > 0)
                    {
                        var guest_room = data.guest.Where(x => x.room_no == r.room_no);
                        foreach (var g in guest_room)
                        {
                            room.guests.Add(new Entities.Models.HotelGuest()
                            {
                                Birthday = g.birthday,
                                Name = g.name,
                                HotelBookingRoomsId = 0,
                                Id = g.id,
                                HotelBookingId = data.hotel.id,
                                Note = g.note,
                                Type = g.type

                            });

                        }
                    }
                    room.detail.NumberOfAdult = room_adult;
                    room.detail.NumberOfChild = room_child;
                    room.detail.NumberOfInfant = room_infant;

                    room.detail = new Entities.Models.HotelBookingRooms()
                    {
                        NumberOfAdult = 0,
                        NumberOfChild = 0,
                        NumberOfInfant = 0,
                        PackageIncludes = "",
                        Price = room_price,
                        Profit = room_profit,
                        TotalAmount = room_amount,
                        RoomTypeCode = r.room_type_code ?? "",
                        RoomTypeName = r.room_type_name ?? "",
                        RoomTypeId = r.room_type_id ?? "",
                        HotelBookingId = data.hotel.id,
                        Id = r.id,
                        ExtraPackageAmount = 0,
                        Status = status,
                        TotalUnitPrice = room_price,
                        CreatedBy = user_id,
                        CreatedDate = DateTime.Now,
                        UpdatedBy = user_id,
                        UpdatedDate = DateTime.Now,
                        NumberOfRooms = r.number_of_rooms,
                    };
                    model_summit.rooms.Add(room);
                }
                if (data.extra_package != null && data.extra_package.Count > 0)
                {
                    model_summit.extra_packages = new List<HotelBookingRoomExtraPackages>();
                    foreach (var ex in data.extra_package)
                    {
                        extra_package_amount_total += ex.amount;
                        extra_package_profit_total += ex.profit;
                        model_summit.extra_packages.Add(new HotelBookingRoomExtraPackages()
                        {
                            Id = ex.id,
                            Amount = ex.amount,
                            CreatedBy = user_id,
                            CreatedDate = DateTime.Now,
                            HotelBookingRoomId = 0,
                            HotelBookingId = 0,
                            PackageCode = ex.name != null && ex.name.Trim() != "" ? ex.name.Substring(0, ex.name.Length < 200 ? ex.name.Length : 200) : "",
                            PackageId = ex.code != null && ex.code.Trim() != "" ? ex.code.Substring(0, ex.code.Length < 50 ? ex.code.Length : 50) : "",
                            UpdatedBy = user_id,
                            UpdatedDate = DateTime.Now,
                            UnitPrice = ex.amount - ex.profit,
                            EndDate = ex.end_date,
                            StartDate = ex.start_date,
                            Profit = ex.profit,
                            Nights = (short)ex.nights,
                            OperatorPrice = ex.operator_price,
                            PackageCompanyId = 0,
                            Quantity = ex.number_of_extrapackages,
                            SalePrice = ex.sale_price,

                        });
                    }
                }
                model_summit.booking_guests = new List<HotelGuest>();
                //-- Guest no room:
                if (data.guest != null && data.guest.Count > 0)
                {
                    var guest_room = data.guest.Where(x => x.room_no <= 0);
                    foreach (var g in guest_room)
                    {
                        model_summit.booking_guests.Add(new Entities.Models.HotelGuest()
                        {
                            Birthday = g.birthday,
                            Name = g.name ?? "",
                            HotelBookingRoomsId = 0,
                            Id = g.id,
                            HotelBookingId = data.hotel.id,
                            Note = g.note ?? "",
                            Type = g.type
                        });

                    }
                }
                model_summit.detail.ExtraPackageAmount = extra_package_amount_total;
                model_summit.detail.TotalPrice = total_price + extra_package_amount_total;
                model_summit.detail.TotalProfit = total_profit + extra_package_profit_total - (double)model_summit.detail.TotalOthersAmount - (double)model_summit.detail.TotalDiscount;
                model_summit.detail.TotalAmount = total_amount + extra_package_amount_total;
                model_summit.detail.NumberOfPeople = model_summit.detail.NumberOfAdult + model_summit.detail.NumberOfChild + model_summit.detail.NumberOfInfant;
                if (data.hotel.id > 0)
                {
                    var package_optional = await _hotelBookingRoomDAL.GetHotelBookingRoomOptionalByBookingId(data.hotel.id);
                    if (package_optional == null || package_optional.Count <= 0)
                    {
                        model_summit.detail.Price = total_price + extra_package_amount_total - extra_package_profit_total;

                    }
                }
                else
                {
                    model_summit.detail.Price = total_price + extra_package_amount_total - extra_package_profit_total;
                }
                var id = await _hotelBookingDAL.UpdateHotelBooking(model_summit, user_id);
                await _hotelBookingDAL.SummitHotelBookingRoomOptional(model_summit, user_id);

                return id;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateHotelBooking - HotelBookingRepository: " + ex);
                return -1;
            }
        }

        public async Task<GenericViewModel<SearchHotelBookingModel>> GetPagingList(SearchHotelBookingViewModel searchModel, int currentPage, int pageSize)
        {
            var model = new GenericViewModel<SearchHotelBookingModel>();
            try
            {

                DataTable dt = await _hotelBookingDAL.GetPagingList(searchModel, currentPage, pageSize, ProcedureConstants.SP_GetListHotelBooking);
                if (dt != null && dt.Rows.Count > 0)
                {
                    var data = dt.ToList<SearchHotelBookingModel>();
                    var data2 = dt.ToList<HotelBookingDetailModel>();
                    var list_id = data.Select(s => s.Id).ToList();

                        DataTable dt2 = await _hotelBookingDAL.GetListHotelBookingRoomByHotelBookingId(list_id);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        data2 = dt2.ToList<HotelBookingDetailModel>();
                    }
                       foreach(var item in data)
                    {
                        item.HotelBookingDetai = data2.Where(s=>s.Id ==Convert.ToInt32(item.Id)).ToList();
                    }
                    model.ListData = data;
                    model.CurrentPage = currentPage;
                    model.PageSize = pageSize;
                    model.TotalRecord = Convert.ToInt32(dt.Rows[0]["TotalRow"]);
                    model.TotalPage = (int)Math.Ceiling((double)model.TotalRecord / model.PageSize);
                    return model;
                }
                return null;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetPagingList - HotelBookingRepository: " + ex);
                return null;
            }
        }
        public async Task<List<HotelBookingDetailViewModel>> GetHotelBookingById(long HotelBookingId)
        {
            var model = new List<HotelBookingDetailViewModel>();
            try
            {
                DataTable dt = await _hotelBookingDAL.GetHotelBookingById(HotelBookingId);
                if (dt != null && dt.Rows.Count > 0)
                {
                    model = dt.ToList<HotelBookingDetailViewModel>();

                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetHotelBookingById - HotelBookingRepository: " + ex);
            }
            return model;
        }
        public async Task<List<HotelBooking>> GetHotelBookingByOrderID(long orderid)
        {
            var model = new List<HotelBooking>();
            try
            {
                DataTable dt = await _hotelBookingDAL.GetHotelBookingByOrderID(orderid);
                if (dt != null && dt.Rows.Count > 0)
                {
                    model = dt.ToList<HotelBooking>();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetHotelBookingByOrderID - HotelBookingRepository: " + ex);

            }
            return model;
        }
        public async Task<int> UpdateHotelBookingStatus(long HotelBookingId, int Status)
        {

            try
            {
                return await _hotelBookingDAL.UpdateHotelBookingStatus(HotelBookingId, Status);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateHotelBookingStatus - HotelBookingRepository: " + ex);

            }
            return 0;
        }
        public async Task<List<HotelBookingViewModel>> GetDetailHotelBookingByID(long HotelBookingId)
        {

            try
            {
                DataTable dt = await _hotelBookingDAL.GetDetailHotelBookingByID(HotelBookingId);
                if (dt != null && dt.Rows.Count > 0)
                {
                    var data = dt.ToList<HotelBookingViewModel>();
                    return data;
                }

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetDetailHotelBookingByID - HotelBookingRepository: " + ex);

            }
            return null;
        }
        public async Task<List<TotalHotelBookingViewModel>> TotalHotelBooking(SearchHotelBookingViewModel searchModel)
        {

            try
            {
                DataTable dt = await _hotelBookingDAL.TotalHotelBooking(searchModel);
                if (dt != null && dt.Rows.Count > 0)
                {
                    var model = dt.ToList<TotalHotelBookingViewModel>();
                    return model;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetHotelBookingByOrderID - HotelBookingRepository: " + ex);

            }
            return null;
        }
        public async Task<int> UpdateHotelBooking(HotelBooking model)
        {
            try
            {
                return await _hotelBookingDAL.UpdateHotelBooking(model);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetHotelBookingByOrderID - HotelBookingRepository: " + ex);

            }
            return 0;
        }
        public async Task<int> InsertServiceDeclines(ServiceDeclines model)
        {
            try
            {
                return await _hotelBookingDAL.InsertServiceDeclines(model);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("InsertServiceDeclines - HotelBookingRepository: " + ex);

            }
            return 0;
        }
        public async Task<ServiceDeclinesViewModel> GetServiceDeclinesByServiceId(string ServiceId, int type)
        {
            try
            {
                DataTable dt = await _hotelBookingDAL.GetServiceDeclinesByServiceId(ServiceId, type);
                if (dt != null && dt.Rows.Count > 0)
                {
                    var model = dt.ToList<ServiceDeclinesViewModel>();
                    return model[0];
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetDetailHotelBookingByID - HotelBookingDAL: " + ex);
            }
            return null;
        }
        public async Task<long> DeleteHotelBookingByID(long id)
        {
            try
            {
                return await _hotelBookingDAL.DeleteHotelBookingByID(id);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("DeleteHotelBookingByID - HotelBookingRepository: " + ex);

            }
            return 0;
        }
        public async Task<long> CancelHotelBookingByID(long id, int user_id)
        {
            try
            {
                return await _hotelBookingDAL.CancelHotelBookingByID(id, user_id);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CancelHotelBookingByID - HotelBookingRepository: " + ex);

            }
            return 0;
        }
        public async Task<int> CreateHotel(Hotel model)
        {
            try
            {
                if (model.IsDisplayWebsite == null) model.IsDisplayWebsite = false;
                var exists = _hotelDAL.GetByName(model.Name);
                if (exists != null && exists.Id > 0)
                {
                    exists.Name = model.Name;
                    exists.Street = model.Street;
                    exists.CreatedBy = model.CreatedBy;
                    exists.UpdatedBy = model.UpdatedBy;
                    exists.CreatedDate = model.CreatedDate;
                    exists.UpdatedDate = model.UpdatedDate;
                    exists.SupplierId = model.SupplierId;
                    exists.HotelId = model.HotelId;
                    _hotelDAL.UpdateHotel(exists);
                }
                else
                {
                    var dataid = _hotelDAL.CreateHotel(model);
                    model.Id = dataid;
                    model.HotelId = dataid.ToString();
                    exists = model;
                }
                var HotelESViewModel = new HotelESViewModel();
                HotelESViewModel.hotelid = exists.HotelId.ToString();
                HotelESViewModel.id = exists.Id;
                HotelESViewModel.name = exists.Name;
                HotelESViewModel.street = exists.Street;
                HotelESViewModel.checkintime = exists.CheckinTime;
                HotelESViewModel.checkouttime = exists.CheckoutTime;

                string Type = "hotel_store";
                string index_name = "hotel_store";
                _ESRepository.DeleteHotelByID(HotelESViewModel.hotelid, index_name, Type);
                _ESRepository.DeleteHotelByID("0", index_name, Type);
                _ESRepository.UpSert(HotelESViewModel, index_name, Type);

                return exists.Id;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CreateHotel - HotelBookingRepository: " + ex);

            }
            return 0;
        }
        public async Task<string> ExportDeposit(SearchHotelBookingViewModel searchModel, string FilePath)
        {
            var pathResult = string.Empty;
            try
            {
                var data = new List<SearchHotelBookingModel>();
                DataTable dt = await _hotelBookingDAL.GetPagingList(searchModel, searchModel.PageIndex, searchModel.PageSize, ProcedureConstants.SP_GetListHotelBooking);
                if (dt != null && dt.Rows.Count > 0)
                {
                    data = dt.ToList<SearchHotelBookingModel>();

                    foreach (var item in data)
                    {
                        DataTable dt2 = await _hotelBookingDAL.GetDetailHotelBookingByID(Convert.ToInt32(item.Id));
                        var data2 = dt2.ToList<HotelBookingDetailModel>();
                        item.HotelBookingDetai = data2;
                    }
                }
                if (data != null && data.Count > 0)
                {
                    Workbook wb = new Workbook();
                    Worksheet ws = wb.Worksheets[0];
                    ws.Name = "Danh sách đặt dịch vụ khách sạn";
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

                    Style numberStyle = ws.Cells["A2"].GetStyle();
                    numberStyle.Number = 3;
                    numberStyle.HorizontalAlignment = TextAlignmentType.Right;
                    numberStyle.VerticalAlignment = TextAlignmentType.Center;

                    int RowIndex = 1;

                    foreach (var item in data)
                    {
                        string ttchitiet = string.Empty;
                        foreach (var i in item.HotelBookingDetai)
                        {
                            ttchitiet += i.RoomTypeName + " : " + i.TotalRooms + " Phòng," + i.TotalDays + " Đêm";
                        }
                        RowIndex++;
                        ws.Cells["A" + RowIndex].PutValue(RowIndex - 1);
                        ws.Cells["A" + RowIndex].SetStyle(alignCenterStyle);
                        ws.Cells["B" + RowIndex].PutValue(item.ServiceCode);
                        ws.Cells["C" + RowIndex].PutValue(item.HotelName + "\n" + ttchitiet);
                        ws.Cells["D" + RowIndex].PutValue(item.ArrivalDate.ToString("dd/MM/yyyy"));
                        ws.Cells["E" + RowIndex].PutValue(item.DepartureDate.ToString("dd/MM/yyyy"));
                        ws.Cells["F" + RowIndex].PutValue(item.OrderNo);
                        ws.Cells["G" + RowIndex].PutValue(item.CreatedDate.ToString("dd/MM/yyyy"));
                        ws.Cells["H" + RowIndex].PutValue(item.SalerName);
                        ws.Cells["I" + RowIndex].PutValue(item.OperatorName);
                        ws.Cells["J" + RowIndex].PutValue(item.BookingCode);
                        ws.Cells["K" + RowIndex].PutValue(item.StatusName);
                        //ws.Cells["L" + RowIndex].PutValue(item.Amount.ToString("N0"));
                        ws.Cells["L" + RowIndex].PutValue(item.Amount);
                        ws.Cells["L" + RowIndex].SetStyle(numberStyle);
                        //ws.Cells["M" + RowIndex].PutValue(item.Price.ToString("N0"));
                        ws.Cells["M" + RowIndex].PutValue(item.Price);
                        ws.Cells["M" + RowIndex].SetStyle(numberStyle);
                        //ws.Cells["N" + RowIndex].PutValue((item.Amount - item.Price).ToString("N0"));
                        ws.Cells["N" + RowIndex].PutValue((item.Amount - item.Price));
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
        public async Task<List<HotelBookingsRoomOptionalViewModel>> GetHotelBookingOptionalListByHotelBookingId(long hotelBookingId)
        {
            var model = new List<HotelBookingsRoomOptionalViewModel>();
            try
            {

                DataTable dt = await _hotelBookingDAL.GetHotelBookingRoomsOptionalByBookingId(hotelBookingId);
                if (dt != null && dt.Rows.Count > 0)
                {
                    model = dt.ToList<HotelBookingsRoomOptionalViewModel>();
                    return model;
                }
                return null;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetHotelBookingOptionalListByHotelBookingId - HotelBookingRepository: " + ex);
                return null;
            }
        }
        public async Task<List<HotelBookingsRoomOptionalViewModel>> GetListHotelBookingRoomExtraPackagesHotelBookingId(long hotelBookingId)
        {
            var model = new List<HotelBookingsRoomOptionalViewModel>();
            try
            {

                DataTable dt = await _hotelBookingDAL.GetListHotelBookingRoomExtraPackagesByBookingId(hotelBookingId);
                if (dt != null && dt.Rows.Count > 0)
                {
                    model = dt.ToList<HotelBookingsRoomOptionalViewModel>();
                    return model;
                }
                return new List<HotelBookingsRoomOptionalViewModel>();
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetHotelBookingOptionalListByHotelBookingId - HotelBookingRepository: " + ex);
                return new List<HotelBookingsRoomOptionalViewModel>();
            }
        }

        public List<ReportHotelRevenueViewModel> GetHotelBookingRevenue(ReportClientDebtSearchModel searchModel, out long total)
        {
            var model = new List<ReportHotelRevenueViewModel>();
            total = 0;
            try
            {
                DataTable dt = _hotelBookingDAL.GetHotelRevenue(searchModel);
                if (dt != null && dt.Rows.Count > 0)
                {
                    model = dt.ToList<ReportHotelRevenueViewModel>();
                    total = model.Count > 0 ? model.FirstOrDefault().TotalRow : 0;
                    return model;
                }
                return new List<ReportHotelRevenueViewModel>();
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetHotelBookingOptionalListByHotelBookingId - HotelBookingRepository: " + ex);
                return new List<ReportHotelRevenueViewModel>();
            }
        }

        public string ExportHotelRevenue(ReportClientDebtSearchModel searchModel, string FilePath)
        {
            var pathResult = string.Empty;
            try
            {
                var listHotelRevenue = GetHotelBookingRevenue(searchModel, out long total);

                if (listHotelRevenue != null && listHotelRevenue.Count > 0)
                {
                    Workbook wb = new Workbook();
                    Worksheet ws = wb.Worksheets[0];
                    ws.Name = "Báo cáo doanh thu khách sạn";
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
                    ws.Cells["B1"].PutValue("Khách sạn");
                    ws.Cells["C1"].PutValue("Doanh thu");
                    ws.Cells["D1"].PutValue("Lợi nhuận");
                    ws.Cells["E1"].PutValue("Roomnights");
                    #endregion

                    #region Body

                    range = cell.CreateRange(1, 0, listHotelRevenue.Count, 15);
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

                    foreach (var item in listHotelRevenue)
                    {
                        RowIndex++;
                        ws.Cells["A" + RowIndex].PutValue(RowIndex - 1);
                        ws.Cells["A" + RowIndex].SetStyle(alignCenterStyle);
                        ws.Cells["B" + RowIndex].PutValue(item.HotelName);
                        ws.Cells["C" + RowIndex].PutValue(item.Amount.ToString("N0"));
                        ws.Cells["C" + RowIndex].SetStyle(numberStyle);
                        ws.Cells["D" + RowIndex].PutValue(item.Price.ToString("N0"));
                        ws.Cells["D" + RowIndex].SetStyle(numberStyle);
                        ws.Cells["E" + RowIndex].PutValue(item.RoomNights);
                        ws.Cells["E" + RowIndex].SetStyle(numberStyle);
                    }
                    #endregion
                    wb.Save(FilePath);
                    pathResult = FilePath;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ExportHotelRevenue - HotelBookingRepositories: " + ex);
            }
            return pathResult;
        }

    }
}
