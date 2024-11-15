using DAL.Generic;
using DAL.StoreProcedure;
using Entities.Models;
using Entities.ViewModels.HotelBooking;
using Entities.ViewModels.OrderManual;
using Entities.ViewModels.Report;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PdfSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace DAL
{
    public class HotelBookingDAL : GenericService<HotelBooking>
    {
        private DbWorker dbWorker;
        public HotelBookingDAL(string connection) : base(connection)
        {
            dbWorker = new DbWorker(connection);
        }
        public async Task<List<HotelBookingViewModel>> GetListByOrderId(long orderId)
        {
            try
            {
                SqlParameter[] objParam_order = new SqlParameter[1];
                objParam_order[0] = new SqlParameter("@OrderId", orderId);
                DataTable dt = dbWorker.GetDataTable(StoreProcedureConstant.SP_GetHotelBookingByOrderID, objParam_order);
                if (dt != null && dt.Rows.Count > 0)
                {
                    var data = dt.ToList<HotelBookingViewModel>();
                    return data;
                }
                return null;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetListByOrderId - HotelBookingDAL. " + ex);
                return null;
            }
        }
        public async Task<HotelBooking> GetHotelBookingByID(long id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var detail = await _DbContext.HotelBooking.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                    return detail;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetHotelBookingByID - HotelBookingDAL: " + ex);
                return null;
            }
        }

        public async Task<long> UpdateHotelBooking(OrderManualHotelServiceSQLSummitModel model, int user_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    //-- Update Order:
                    var order = await _DbContext.Order.AsNoTracking().FirstOrDefaultAsync(x => x.OrderId == model.order_id);
                    if (order == null || order.OrderId <= 0)
                    {
                        return -1;
                    }

                    //-- create HotelBooking
                    if (model.detail.Id <= 0)
                    {
                        model.detail.OrderId = order.OrderId;
                        List<long> booking_room_ids = new List<long>();
                        List<long> booking_room_rate_ids = new List<long>();
                        List<long> booking_room_guest_ids = new List<long>();
                        List<long> booking_room_extra_packages_ids = new List<long>();

                        model.detail.OrderId = order.OrderId;
                        CreateHotelBooking(model.detail);
                        //-- create HotelBookingRoom
                        foreach (var room in model.rooms)
                        {
                            room.detail.HotelBookingId = model.detail.Id;
                            CreateHotelBookingRooms(room.detail);
                            booking_room_ids.Add(room.detail.Id);

                            //-- create HotelRoomGuest:
                            foreach (var guest in room.guests)
                            {
                                guest.HotelBookingRoomsId = room.detail.Id;
                                guest.HotelBookingId = model.detail.Id;
                                CreateHotelGuest(guest);
                                booking_room_guest_ids.Add(guest.Id);

                            }
                            //-- create HotelRoomRate
                            foreach (var rate in room.rates)
                            {
                                rate.HotelBookingRoomId = room.detail.Id;
                                CreateHotelBookingRoomRates(rate);
                                booking_room_rate_ids.Add(rate.Id);

                            }
                            if (room.extra_packages != null && room.extra_packages.Count > 0)
                            {
                                //-- create HotelRoomRate
                                foreach (var ex in room.extra_packages)
                                {
                                    ex.HotelBookingRoomId = room.detail.Id;
                                    ex.HotelBookingId = model.detail.Id;
                                    CreateHotelBookingRoomExtraPackages(ex);
                                    booking_room_extra_packages_ids.Add(ex.Id);

                                }
                            }

                        }
                        if (model.extra_packages != null && model.extra_packages.Count > 0)
                        {
                            //-- create HotelRoomRate
                            foreach (var ex in model.extra_packages)
                            {
                                ex.HotelBookingRoomId = 0;
                                ex.HotelBookingId = model.detail.Id;
                                CreateHotelBookingRoomExtraPackages(ex);
                                booking_room_extra_packages_ids.Add(ex.Id);
                            }
                        }

                        //-- update HotelRoomGuest:
                        foreach (var guest in model.booking_guests)
                        {
                            guest.HotelBookingId = model.detail.Id;
                            await UpdateHotelBookingRoomsGuest(guest);
                            booking_room_guest_ids.Add(guest.Id);
                        }
                        //-- Remove Ids not exists:
                        await RemoveNonExistsHotelBookingRoom(model.detail.OrderId, model.detail.Id, booking_room_ids, booking_room_rate_ids, booking_room_guest_ids, booking_room_extra_packages_ids);
                    }
                    //-- Update HotelBooking
                    else
                    {
                        //--Update Amount Order
                        var hotel_booking = await _DbContext.HotelBooking.AsNoTracking().FirstOrDefaultAsync(x => x.Id == model.detail.Id);
                        if (hotel_booking == null || hotel_booking.Id <= 0)
                        {
                            return -1;
                        }
                        model.detail.OrderId = order.OrderId;

                        List<long> booking_room_ids = new List<long>();
                        List<long> booking_room_rate_ids = new List<long>();
                        List<long> booking_room_guest_ids = new List<long>();
                        List<long> booking_room_extra_packages_ids = new List<long>();

                        model.detail.Id = hotel_booking.Id;
                        model.detail.OrderId = hotel_booking.OrderId;
                        model.detail.CreatedBy = hotel_booking.CreatedBy;
                        model.detail.StatusOld = hotel_booking.StatusOld;
                        _DbContext.HotelBooking.Update(model.detail);
                        await _DbContext.SaveChangesAsync();
                        //--Update HotelBookingRoom
                        foreach (var room in model.rooms)
                        {
                            room.detail.HotelBookingId = model.detail.Id;
                            await UpdateHotelBookingRoom(room.detail);
                            booking_room_ids.Add(room.detail.Id);
                            //-- update HotelRoomGuest:
                            foreach (var guest in room.guests)
                            {
                                guest.HotelBookingRoomsId = room.detail.Id;
                                guest.HotelBookingId = model.detail.Id;
                                await UpdateHotelBookingRoomsGuest(guest);
                                booking_room_guest_ids.Add(guest.Id);
                            }
                            //-- update HotelRoomRate
                            foreach (var rate in room.rates)
                            {
                                rate.HotelBookingRoomId = room.detail.Id;
                                await UpdateHotelBookingRoomrate(rate);
                                booking_room_rate_ids.Add(rate.Id);
                            }
                            if (room.extra_packages != null && room.extra_packages.Count > 0)
                            {
                                foreach (var ex in room.extra_packages)
                                {
                                    ex.HotelBookingRoomId = room.detail.Id;
                                    await UpdateHotelBookingExtraPackages(ex);
                                    booking_room_extra_packages_ids.Add(ex.Id);
                                }
                            }

                        }
                        if (model.extra_packages != null && model.extra_packages.Count > 0)
                        {
                            //-- create HotelRoomRate
                            foreach (var ex in model.extra_packages)
                            {
                                ex.HotelBookingRoomId = 0;
                                ex.HotelBookingId = model.detail.Id;
                                await UpdateHotelBookingExtraPackages(ex);
                                booking_room_extra_packages_ids.Add(ex.Id);
                            }
                        }
                        //-- update HotelRoomGuest:
                        foreach (var guest in model.booking_guests)
                        {
                            await UpdateHotelBookingRoomsGuest(guest);
                            booking_room_guest_ids.Add(guest.Id);
                        }
                        //-- Remove Ids not exists:
                        await RemoveNonExistsHotelBookingRoom(model.detail.OrderId, model.detail.Id, booking_room_ids, booking_room_rate_ids, booking_room_guest_ids, booking_room_extra_packages_ids);

                    }
                }
                return model.detail.Id;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateHotelBooking - HotelBookingDAL. " + ex);
                return -1;
            }
        }
        private async Task<long> RemoveNonExistsHotelBookingRoom(long order_id, long hotel_booking_id, List<long> booking_room_ids, List<long> booking_room_rate_ids, List<long> booking_room_guest_ids, List<long> booking_room_extra_packages_ids)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var ids_bookings = new List<long>() { hotel_booking_id };

                    var del_bookings = await _DbContext.HotelBooking.AsNoTracking().Where(x => x.OrderId == order_id && x.Id != hotel_booking_id).ToListAsync();
                    //--delete all room not exists after update:
                    var booking_room_del = await _DbContext.HotelBookingRooms.AsNoTracking().Where(x => ids_bookings.Contains(x.HotelBookingId) && !booking_room_ids.Contains(x.Id)).ToListAsync();
                    if (booking_room_del != null && booking_room_del.Count > 0)
                    {
                        var ids_booking_room_del = booking_room_del.Select(y => y.Id);
                        var ids_extrapackage_del = await _DbContext.HotelBookingRoomExtraPackages.AsNoTracking().Where(x => ids_booking_room_del.Contains((long)x.HotelBookingRoomId)).ToListAsync();
                        if (ids_extrapackage_del != null && ids_extrapackage_del.Count > 0)
                        {
                            _DbContext.HotelBookingRoomExtraPackages.RemoveRange(ids_extrapackage_del);
                            await _DbContext.SaveChangesAsync();
                        }
                        var ids_rate_del = await _DbContext.HotelBookingRoomRates.AsNoTracking().Where(x => ids_booking_room_del.Contains(x.HotelBookingRoomId)).ToListAsync();
                        if (ids_rate_del != null && ids_rate_del.Count > 0)
                        {

                            _DbContext.HotelBookingRoomRates.RemoveRange(ids_rate_del);
                            await _DbContext.SaveChangesAsync();
                        }
                        var ids_guest_del = await _DbContext.HotelGuest.AsNoTracking().Where(x => ids_booking_room_del.Contains(x.HotelBookingRoomsId)).ToListAsync();
                        if (ids_guest_del != null && ids_guest_del.Count > 0)
                        {
                            _DbContext.HotelGuest.RemoveRange(ids_guest_del);
                            await _DbContext.SaveChangesAsync();
                        }
                        _DbContext.HotelBookingRooms.RemoveRange(booking_room_del);
                        await _DbContext.SaveChangesAsync();
                    }

                    //--Delete extra_package not exists after update
                    var ids_booking_extra_packages_del = await _DbContext.HotelBookingRoomExtraPackages.AsNoTracking().Where(x => x.HotelBookingId == hotel_booking_id && !booking_room_extra_packages_ids.Contains(x.Id)).ToListAsync();
                    if (ids_booking_extra_packages_del != null && ids_booking_extra_packages_del.Count > 0)
                    {
                        _DbContext.HotelBookingRoomExtraPackages.RemoveRange(ids_booking_extra_packages_del);
                        await _DbContext.SaveChangesAsync();
                    }
                    //--Delete rate not exists after update
                    var ids_booking_rate_del = await _DbContext.HotelBookingRoomRates.AsNoTracking().Where(x => booking_room_ids.Contains(x.HotelBookingRoomId) && !booking_room_rate_ids.Contains(x.Id)).ToListAsync();
                    if (ids_booking_rate_del != null && ids_booking_rate_del.Count > 0)
                    {
                        _DbContext.HotelBookingRoomRates.RemoveRange(ids_booking_rate_del);
                        await _DbContext.SaveChangesAsync();
                    }
                    //--Delete guest not exists after update
                    var ids_booking_guest_del = await _DbContext.HotelGuest.AsNoTracking().Where(x => x.HotelBookingId == hotel_booking_id && !booking_room_guest_ids.Contains(x.Id)).ToListAsync();
                    if (ids_booking_guest_del != null && ids_booking_guest_del.Count > 0)
                    {
                        _DbContext.HotelGuest.RemoveRange(ids_booking_guest_del);
                        await _DbContext.SaveChangesAsync();
                    }
                    return 0;
                }


            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("RemoveNonExistsHotelBookingRoom - HotelBookingDAL. " + ex);
                return -1;
            }
        }

        public async Task<long> UpdateHotelBookingRoom(HotelBookingRooms booking)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var hotel_booking_room = await _DbContext.HotelBookingRooms.AsNoTracking().FirstOrDefaultAsync(x => x.Id == booking.Id);
                    if (hotel_booking_room != null && hotel_booking_room.Id > 0)
                    {
                        booking.Id = hotel_booking_room.Id;
                        booking.CreatedBy = hotel_booking_room.CreatedBy;
                        booking.CreatedDate = hotel_booking_room.CreatedDate;
                        _DbContext.HotelBookingRooms.Update(booking);
                        await _DbContext.SaveChangesAsync();
                    }
                    else
                    {
                        CreateHotelBookingRooms(booking);
                    }
                    return booking.Id;
                }

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateHotelBookingRoom - HotelBookingDAL. " + ex);
                return -1;
            }
        }
        public async Task<long> UpdateHotelBookingRoomrate(HotelBookingRoomRates rate)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var hotel_booking_rate = await _DbContext.HotelBookingRoomRates.AsNoTracking().FirstOrDefaultAsync(x => x.Id == rate.Id);
                    if (hotel_booking_rate != null && hotel_booking_rate.Id > 0)
                    {
                        rate.Id = hotel_booking_rate.Id;
                        rate.CreatedBy = hotel_booking_rate.CreatedBy;
                        rate.CreatedDate = hotel_booking_rate.CreatedDate;
                        _DbContext.HotelBookingRoomRates.Update(rate);
                        await _DbContext.SaveChangesAsync();
                    }
                    else
                    {
                        CreateHotelBookingRoomRates(rate);
                    }
                    return rate.Id;
                }

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateHotelBookingRoomrate - HotelBookingDAL. " + ex);
                return -1;
            }
        }
        private async Task<long> UpdateHotelBookingRoomsGuest(HotelGuest guest)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var hotel_guest = await _DbContext.HotelGuest.AsNoTracking().FirstOrDefaultAsync(x => x.Id == guest.Id);
                    if (hotel_guest != null && hotel_guest.Id > 0)
                    {
                        guest.Id = hotel_guest.Id;
                        _DbContext.HotelGuest.Update(guest);
                        await _DbContext.SaveChangesAsync();
                    }
                    else
                    {
                        CreateHotelGuest(guest);
                    }
                    return guest.Id;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateHotelBookingRoomsGuest - HotelBookingDAL. " + ex);
                return -1;
            }
        }
        public async Task<long> DeleteHotelBookingByID(long id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var booking = await _DbContext.HotelBooking.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                    if (booking != null && booking.Id > 0)
                    {
                        var booking_room = await _DbContext.HotelBookingRooms.AsNoTracking().Where(x => x.HotelBookingId == id).ToListAsync();
                        if (booking_room != null && booking_room.Count > 0)
                        {
                            foreach (var room in booking_room)
                            {
                                var room_rate = await _DbContext.HotelBookingRoomRates.AsNoTracking().Where(x => x.HotelBookingRoomId == room.Id).ToListAsync();
                                if (room_rate != null && room_rate.Count > 0)
                                {
                                    _DbContext.HotelBookingRoomRates.RemoveRange(room_rate);
                                    await _DbContext.SaveChangesAsync();
                                }
                            }
                            _DbContext.HotelBookingRooms.RemoveRange(booking_room);
                            await _DbContext.SaveChangesAsync();

                            var room_extra = await _DbContext.HotelBookingRoomExtraPackages.AsNoTracking().Where(x => x.HotelBookingId == id).ToListAsync();
                            if (room_extra != null && room_extra.Count > 0)
                            {
                                _DbContext.HotelBookingRoomExtraPackages.RemoveRange(room_extra);
                                await _DbContext.SaveChangesAsync();
                            }

                            var room_guest = await _DbContext.HotelGuest.AsNoTracking().Where(x => x.HotelBookingId == id).ToListAsync();
                            if (room_guest != null && room_guest.Count > 0)
                            {
                                _DbContext.HotelGuest.RemoveRange(room_guest);
                                await _DbContext.SaveChangesAsync();
                            }
                        }
                        _DbContext.HotelBooking.Remove(booking);
                        await _DbContext.SaveChangesAsync();

                    }
                    return id;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("DeleteHotelBoookingByID - HotelBookingDAL. " + ex);
                return -1;
            }
        }
        public async Task<long> CancelHotelBookingByID(long id, int user_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var booking = await _DbContext.HotelBooking.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                    if (booking != null && booking.Id > 0)
                    {
                        if (booking.Status == (int)ServiceStatus.Decline)
                        {
                            booking.Status = (int)ServiceStatus.Cancel;
                            booking.UpdatedBy = user_id;
                            booking.UpdatedDate = DateTime.Now;
                            _DbContext.HotelBooking.Update(booking);
                            await _DbContext.SaveChangesAsync();
                        }
                    }
                    return id;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CancelHotelBookingByID - HotelBookingDAL. " + ex);
                return -1;
            }
        }
        public async Task<long> UpdateHotelBookingExtraPackages(HotelBookingRoomExtraPackages package)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var package_exists = await _DbContext.HotelBookingRoomExtraPackages.AsNoTracking().FirstOrDefaultAsync(x => x.Id == package.Id);
                    if (package_exists != null && package_exists.Id > 0)
                    {
                        package.Id = package_exists.Id;
                        package.CreatedBy = package_exists.CreatedBy;
                        package.CreatedDate = package_exists.CreatedDate;
                        UpdateHotelBookingExtraPackagesSP(package);
                    }
                    else
                    {
                        CreateHotelBookingRoomExtraPackages(package);
                    }
                    return package.Id;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateHotelBookingExtraPackages - HotelBookingDAL. " + ex);
                return -1;
            }
        }

        private int CreateHotelBooking(HotelBooking booking)
        {
            try
            {

                SqlParameter[] objParam_order = new SqlParameter[31];
                objParam_order[0] = new SqlParameter("@OrderId", booking.OrderId);
                objParam_order[1] = new SqlParameter("@BookingId", booking.BookingId);
                objParam_order[2] = new SqlParameter("@PropertyId", booking.PropertyId);
                if (booking.HotelType != null)
                {
                    objParam_order[3] = new SqlParameter("@HotelType", booking.HotelType);
                }
                else
                {
                    objParam_order[3] = new SqlParameter("@HotelType", (int)ServicesType.VINHotelRent);
                }
                objParam_order[4] = new SqlParameter("@ArrivalDate", booking.ArrivalDate);
                objParam_order[5] = new SqlParameter("@DepartureDate", booking.DepartureDate);
                objParam_order[6] = new SqlParameter("@numberOfRoom", booking.NumberOfRoom);
                objParam_order[7] = new SqlParameter("@numberOfAdult", booking.NumberOfAdult);
                objParam_order[8] = new SqlParameter("@numberOfChild", booking.NumberOfChild);
                objParam_order[9] = new SqlParameter("@numberOfInfant", booking.NumberOfInfant);
                objParam_order[10] = new SqlParameter("@totalPrice", booking.TotalPrice);
                objParam_order[11] = new SqlParameter("@totalProfit", booking.TotalProfit);
                objParam_order[12] = new SqlParameter("@totalAmount", booking.TotalAmount);
                objParam_order[13] = new SqlParameter("@Status", booking.Status);
                objParam_order[14] = new SqlParameter("@HotelName", booking.HotelName);
                if (booking.Telephone != null)
                {
                    objParam_order[15] = new SqlParameter("@Telephone", booking.Telephone);
                }
                else
                {
                    objParam_order[15] = new SqlParameter("@Telephone", DBNull.Value);
                }
                if (booking.Email != null)
                {
                    objParam_order[16] = new SqlParameter("@Email", booking.Email);
                }
                else
                {
                    objParam_order[16] = new SqlParameter("@Email", DBNull.Value);
                }
                if (booking.Address != null)
                {
                    objParam_order[17] = new SqlParameter("@Address", booking.Address);
                }
                else
                {
                    objParam_order[17] = new SqlParameter("@Address", DBNull.Value);
                }
                if (booking.ImageThumb != null)
                {
                    objParam_order[18] = new SqlParameter("@ImageThumb", booking.ImageThumb);
                }
                else
                {
                    objParam_order[18] = new SqlParameter("@ImageThumb", DBNull.Value);
                }
                objParam_order[19] = new SqlParameter("@CheckinTime", booking.CheckinTime);
                objParam_order[20] = new SqlParameter("@CheckoutTime", booking.CheckoutTime);
                objParam_order[21] = new SqlParameter("@ExtraPackageAmount", booking.ExtraPackageAmount);
                if (booking.SalerId != null)
                {
                    objParam_order[22] = new SqlParameter("@SalerId", booking.SalerId);
                }
                else
                {
                    objParam_order[22] = new SqlParameter("@SalerId", DBNull.Value);
                }
                objParam_order[23] = new SqlParameter("@CreatedBy", booking.CreatedBy);
                objParam_order[24] = new SqlParameter("@CreatedDate", booking.CreatedDate);
                if (booking.ServiceCode != null)
                {
                    objParam_order[25] = new SqlParameter("@ServiceCode", booking.ServiceCode);
                }
                else
                {
                    objParam_order[25] = new SqlParameter("@ServiceCode", DBNull.Value);
                }
                objParam_order[26] = new SqlParameter("@Price", booking.Price);
                if (booking.SupplierId != null)
                {
                    objParam_order[27] = new SqlParameter("@SupplierId", booking.SupplierId);
                }
                else
                {
                    objParam_order[27] = new SqlParameter("@SupplierId", DBNull.Value);
                }
                if (booking.Note != null)
                {
                    objParam_order[28] = new SqlParameter("@Note", booking.Note);
                }
                else
                {
                    objParam_order[28] = new SqlParameter("@Note", DBNull.Value);
                }
                objParam_order[29] = new SqlParameter("@TotalDiscount", booking.TotalDiscount);
                objParam_order[30] = new SqlParameter("@TotalOthersAmount", booking.TotalOthersAmount);
                var id = dbWorker.ExecuteNonQuery(StoreProcedureConstant.CreateHotelBooking, objParam_order);
                booking.Id = id;
                return id;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CreateHotelBooking - HotelBookingDAL. " + ex);
                return -1;
            }
        }

        private int CreateHotelBookingRooms(HotelBookingRooms booking)
        {
            try
            {

                SqlParameter[] objParam_order = new SqlParameter[18];
                objParam_order[0] = new SqlParameter("@HotelBookingId", booking.HotelBookingId);
                objParam_order[1] = new SqlParameter("@RoomTypeID", booking.RoomTypeId);
                objParam_order[2] = new SqlParameter("@Price", booking.Price);
                objParam_order[3] = new SqlParameter("@Profit", booking.Profit);
                objParam_order[4] = new SqlParameter("@TotalAmount", booking.TotalAmount);
                objParam_order[5] = new SqlParameter("@RoomTypeCode", booking.RoomTypeCode);
                objParam_order[6] = new SqlParameter("@RoomTypeName", booking.RoomTypeName);
                objParam_order[7] = new SqlParameter("@numberOfAdult", booking.NumberOfAdult);
                objParam_order[8] = new SqlParameter("@numberOfChild", booking.NumberOfChild);
                objParam_order[9] = new SqlParameter("@numberOfInfant", booking.NumberOfInfant);
                objParam_order[10] = new SqlParameter("@PackageIncludes", booking.PackageIncludes);
                objParam_order[11] = new SqlParameter("@ExtraPackageAmount", booking.ExtraPackageAmount);
                objParam_order[12] = new SqlParameter("@Status", booking.Status);
                objParam_order[13] = new SqlParameter("@TotalUnitPrice", booking.TotalUnitPrice);
                objParam_order[14] = new SqlParameter("@CreateBy", booking.CreatedBy);
                objParam_order[15] = new SqlParameter("@CreatedDate", booking.CreatedDate);
                objParam_order[16] = new SqlParameter("@NumberOfRooms", booking.NumberOfRooms);
                objParam_order[17] = new SqlParameter("@SupplierId", DBNull.Value);
                var id = dbWorker.ExecuteNonQuery(StoreProcedureConstant.CreateHotelBookingRooms, objParam_order);
                booking.Id = id;
                return id;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CreateHotelBookingRooms - HotelBookingDAL. " + ex);
                return -1;
            }
        }
        private int CreateHotelBookingRoomExtraPackages(HotelBookingRoomExtraPackages packages)
        {
            try
            {

                SqlParameter[] objParam_order = new SqlParameter[17];
                objParam_order[0] = new SqlParameter("@PackageId", packages.PackageId);
                objParam_order[1] = new SqlParameter("@PackageCode", packages.PackageCode);
                objParam_order[2] = new SqlParameter("@HotelBookingId", packages.HotelBookingId);
                objParam_order[3] = new SqlParameter("@HotelBookingRoomID", packages.HotelBookingRoomId);
                objParam_order[4] = new SqlParameter("@Amount", packages.Amount);
                objParam_order[5] = new SqlParameter("@CreatedBy", packages.CreatedBy);
                objParam_order[6] = new SqlParameter("@CreatedDate", packages.CreatedDate);
                objParam_order[7] = new SqlParameter("@StartDate", packages.StartDate);
                objParam_order[8] = new SqlParameter("@EndDate", packages.EndDate);
                objParam_order[9] = new SqlParameter("@Profit", packages.Profit);
                objParam_order[10] = new SqlParameter("@PackageCompanyId", packages.PackageCompanyId);
                objParam_order[11] = new SqlParameter("@OperatorPrice", packages.OperatorPrice);
                objParam_order[12] = new SqlParameter("@SalePrice", packages.SalePrice);
                objParam_order[13] = new SqlParameter("@Nights", packages.Nights);
                objParam_order[14] = new SqlParameter("@Quantity", packages.Quantity);
                objParam_order[15] = new SqlParameter("@UnitPrice", packages.UnitPrice);
                if (packages.SupplierId != null)
                {
                    objParam_order[16] = new SqlParameter("@SupplierId", packages.SupplierId);
                }
                else
                {
                    objParam_order[16] = new SqlParameter("@SupplierId", DBNull.Value);
                }
                var id = dbWorker.ExecuteNonQuery(StoreProcedureConstant.InsertHotelBookingRoomExtraPackages, objParam_order);
                packages.Id = id;
                return id;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CreateHotelBookingRoomExtraPackages - HotelBookingDAL. " + ex);
                return -1;
            }
        }
        private int UpdateHotelBookingExtraPackagesSP(HotelBookingRoomExtraPackages packages)
        {
            try
            {

                SqlParameter[] objParam_order = new SqlParameter[17];
                objParam_order[0] = new SqlParameter("@PackageId", packages.PackageId);
                objParam_order[1] = new SqlParameter("@PackageCode", packages.PackageCode);
                objParam_order[2] = new SqlParameter("@HotelBookingId", packages.HotelBookingId);
                objParam_order[3] = new SqlParameter("@HotelBookingRoomID", packages.HotelBookingRoomId);
                objParam_order[4] = new SqlParameter("@Amount", packages.Amount);
                objParam_order[5] = new SqlParameter("@UnitPrice", packages.UnitPrice);
                objParam_order[6] = new SqlParameter("@UpdatedBy", packages.UpdatedBy);
                objParam_order[7] = new SqlParameter("@StartDate", packages.StartDate);
                objParam_order[8] = new SqlParameter("@EndDate", packages.EndDate);
                objParam_order[9] = new SqlParameter("@Profit", packages.Profit);
                objParam_order[10] = new SqlParameter("@PackageCompanyId", packages.PackageCompanyId);
                objParam_order[11] = new SqlParameter("@OperatorPrice", packages.OperatorPrice);
                objParam_order[12] = new SqlParameter("@SalePrice", packages.SalePrice);
                objParam_order[13] = new SqlParameter("@Nights", packages.Nights);
                objParam_order[14] = new SqlParameter("@Quantity", packages.Quantity);
                objParam_order[15] = new SqlParameter("@Id", packages.Id);
                if (packages.SupplierId != null)
                {
                    objParam_order[16] = new SqlParameter("@SupplierId", packages.SupplierId);
                }
                else
                {
                    objParam_order[16] = new SqlParameter("@SupplierId", DBNull.Value);
                }

                var id = dbWorker.ExecuteNonQuery(StoreProcedureConstant.SP_UpdateHotelBookingRoomExtraPackages, objParam_order);
                packages.Id = id;
                return id;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CreateHotelBookingRoomExtraPackages - HotelBookingDAL. " + ex);
                return -1;
            }
        }
        private int CreateHotelBookingRoomRates(HotelBookingRoomRates booking)
        {
            try
            {

                SqlParameter[] objParam_order = new SqlParameter[15];
                objParam_order[0] = new SqlParameter("@HotelBookingRoomId", booking.HotelBookingRoomId);
                objParam_order[1] = new SqlParameter("@RatePlanId", booking.RatePlanId);
                objParam_order[2] = new SqlParameter("@StayDate", booking.StayDate);
                objParam_order[3] = new SqlParameter("@Price", booking.Price);
                objParam_order[4] = new SqlParameter("@Profit", booking.Profit);
                objParam_order[5] = new SqlParameter("@TotalAmount", booking.TotalAmount);
                if (booking.AllotmentId != null)
                {
                    objParam_order[6] = new SqlParameter("@AllotmentId", booking.AllotmentId);
                }
                else
                {
                    objParam_order[6] = new SqlParameter("@AllotmentId", DBNull.Value);
                }
                if (booking.RatePlanCode != null)
                {
                    objParam_order[7] = new SqlParameter("@RatePlanCode", booking.RatePlanCode);
                }
                else
                {
                    objParam_order[7] = new SqlParameter("@RatePlanCode", DBNull.Value);
                }
                if (booking.PackagesInclude != null)
                {
                    objParam_order[8] = new SqlParameter("@PackagesInclude", booking.PackagesInclude);
                }
                else
                {
                    objParam_order[8] = new SqlParameter("@PackagesInclude", DBNull.Value);
                }
                objParam_order[9] = new SqlParameter("@Nights", booking.Nights);
                objParam_order[10] = new SqlParameter("@StartDate", booking.StartDate);
                objParam_order[11] = new SqlParameter("@EndDate", booking.EndDate);
                objParam_order[12] = new SqlParameter("@OperatorPrice", booking.OperatorPrice);
                objParam_order[13] = new SqlParameter("@SalePrice", booking.SalePrice);
                objParam_order[14] = new SqlParameter("@CreatedBy", booking.CreatedBy);
                var id = dbWorker.ExecuteNonQuery(StoreProcedureConstant.CreateHotelBookingRoomRates, objParam_order);
                booking.Id = id;
                return id;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CreateHotelBookingRoomRates - HotelBookingDAL. " + ex);
                return -1;
            }
        }


        private int CreateHotelGuest(HotelGuest guest)
        {
            try
            {

                SqlParameter[] objParam_order = new SqlParameter[6];
                objParam_order[0] = new SqlParameter("@Name", guest.Name);
                if (guest.Birthday != null)
                {
                    objParam_order[1] = new SqlParameter("@Birthday", guest.Birthday);
                }
                else
                {
                    objParam_order[1] = new SqlParameter("@Birthday", DBNull.Value);
                }
                objParam_order[2] = new SqlParameter("@HotelBookingRoomsID", guest.HotelBookingRoomsId);
                objParam_order[3] = new SqlParameter("@HotelBookingId", guest.HotelBookingId);
                objParam_order[4] = new SqlParameter("@Note", guest.Note == null ? "" : guest.Note);
                objParam_order[5] = new SqlParameter("@Type", guest.Type == null ? 0 : guest.Type);


                var id = dbWorker.ExecuteNonQuery(StoreProcedureConstant.CreateHotelGuest, objParam_order);
                guest.Id = id;
                return id;

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CreateHotelGuest - HotelBookingDAL. " + ex);
                return -1;
            }
        }
        //Get List dịch vụ khách sạn :
        public async Task<DataTable> GetPagingList(SearchHotelBookingViewModel searchModel, int currentPage, int pageSize, string proc)
        {
            try
            {

                SqlParameter[] objParam = new SqlParameter[16];
                objParam[0] = new SqlParameter("@ServiceCode", searchModel.ServiceCode);
                objParam[1] = new SqlParameter("@OrderCode", searchModel.OrderCode);
                objParam[2] = new SqlParameter("@StatusBooking", searchModel.StatusBooking);
                objParam[3] = (CheckDate(searchModel.CheckinDateFrom) == DateTime.MinValue) ? new SqlParameter("@CheckinDateFrom", DBNull.Value) : new SqlParameter("@CheckinDateFrom", CheckDate(searchModel.CheckinDateFrom));

                objParam[4] = (CheckDate(searchModel.CheckinDateTo) == DateTime.MinValue) ? new SqlParameter("@CheckinDateTo", DBNull.Value) : new SqlParameter("@CheckinDateTo", CheckDate(searchModel.CheckinDateTo));

                objParam[5] = (CheckDate(searchModel.CheckoutDateFrom) == DateTime.MinValue) ? new SqlParameter("@CheckoutDateFrom", DBNull.Value) : new SqlParameter("@CheckoutDateFrom", CheckDate(searchModel.CheckoutDateFrom));

                objParam[6] = (CheckDate(searchModel.CheckoutDateTo) == DateTime.MinValue) ? new SqlParameter("@CheckoutDateTo", DBNull.Value) : new SqlParameter("@CheckoutDateTo", CheckDate(searchModel.CheckoutDateTo));

                objParam[7] = new SqlParameter("@UserCreate", searchModel.UserCreate);
                objParam[8] = (CheckDate(searchModel.CreateDateFrom) == DateTime.MinValue) ? new SqlParameter("@CreateDateFrom", DBNull.Value) : new SqlParameter("@CreateDateFrom", CheckDate(searchModel.CreateDateFrom));

                objParam[9] = (CheckDate(searchModel.CreateDateTo) == DateTime.MinValue) ? new SqlParameter("@CreateDateTo", DBNull.Value) : new SqlParameter("@CreateDateTo", CheckDate(searchModel.CreateDateTo));

                objParam[10] = new SqlParameter("@SalerId", searchModel.SalerId);
                objParam[11] = new SqlParameter("@OperatorId", searchModel.OperatorId);
                objParam[12] = new SqlParameter("@PageIndex", searchModel.PageIndex);
                objParam[13] = new SqlParameter("@PageSize", searchModel.PageSize);
                objParam[14] = new SqlParameter("@SalerPermission", searchModel.SalerPermission);
                objParam[15] = new SqlParameter("@BookingCode", searchModel.BookingCode);



                return dbWorker.GetDataTable(proc, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetPagingList - ContractDAL: " + ex);
            }
            return null;
        }
        public async Task<DataTable> GetDetailHotelBookingByOrderID(long OrderID)
        {
            try
            {

                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@OrderID", OrderID);
                return dbWorker.GetDataTable(StoreProcedureConstant.SP_GetDetailHotelBookingByOrderID, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetPagingList - HotelBookingDAL: " + ex);
            }
            return null;
        }
        public async Task<DataTable> GetHotelBookingById(long HotelBookingId)
        {
            try
            {

                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@HotelBookingId", HotelBookingId);
                return dbWorker.GetDataTable(StoreProcedureConstant.SP_GetHotelBookingById, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetHotelBookingById - HotelBookingDAL: " + ex);
            }
            return null;
        }
        public async Task<DataTable> GetHotelBookingByOrderID(long OrderID)
        {
            try
            {

                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@OrderID", OrderID);
                return dbWorker.GetDataTable(StoreProcedureConstant.SP_GetHotelBookingByOrderID, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetPagingList - HotelBookingDAL: " + ex);
            }
            return null;
        }

        public async Task<int> UpdateHotelBookingStatus(long HotelBookingId, int Status)
        {
            try
            {

                SqlParameter[] objParam = new SqlParameter[2];
                objParam[0] = new SqlParameter("@HotelBookingId", HotelBookingId);
                objParam[1] = new SqlParameter("@Status", Status);
                return dbWorker.ExecuteNonQuery(StoreProcedureConstant.SP_UpdateHotelBookingStatus, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetPagingList - HotelBookingDAL: " + ex);
            }
            return 0;
        }

        public async Task<DataTable> GetDetailHotelBookingByID(long HotelBookingId)
        {
            try
            {

                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@HotelBookingId", HotelBookingId);

                return dbWorker.GetDataTable(StoreProcedureConstant.SP_GetDetailHotelBookingByID, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetDetailHotelBookingByID - HotelBookingDAL: " + ex);
            }
            return null;
        }
        public async Task<DataTable> TotalHotelBooking(SearchHotelBookingViewModel searchModel)
        {
            try
            {

                SqlParameter[] objParam = new SqlParameter[14];
                objParam[0] = new SqlParameter("@ServiceCode", searchModel.ServiceCode);
                objParam[1] = new SqlParameter("@OrderCode", searchModel.OrderCode);
                objParam[2] = new SqlParameter("@StatusBooking", searchModel.StatusBooking);
                objParam[3] = (CheckDate(searchModel.CheckinDateFrom) == DateTime.MinValue) ? new SqlParameter("@CheckinDateFrom", DBNull.Value) : new SqlParameter("@CheckinDateFrom", CheckDate(searchModel.CheckinDateFrom));

                objParam[4] = (CheckDate(searchModel.CheckinDateTo) == DateTime.MinValue) ? new SqlParameter("@CheckinDateTo", DBNull.Value) : new SqlParameter("@CheckinDateTo", CheckDate(searchModel.CheckinDateTo));

                objParam[5] = (CheckDate(searchModel.CheckoutDateFrom) == DateTime.MinValue) ? new SqlParameter("@CheckoutDateFrom", DBNull.Value) : new SqlParameter("@CheckoutDateFrom", CheckDate(searchModel.CheckoutDateFrom));

                objParam[6] = (CheckDate(searchModel.CheckoutDateTo) == DateTime.MinValue) ? new SqlParameter("@CheckoutDateTo", DBNull.Value) : new SqlParameter("@CheckoutDateTo", CheckDate(searchModel.CheckoutDateTo));

                objParam[7] = new SqlParameter("@UserCreate", searchModel.UserCreate);
                objParam[8] = (CheckDate(searchModel.CreateDateFrom) == DateTime.MinValue) ? new SqlParameter("@CreateDateFrom", DBNull.Value) : new SqlParameter("@CreateDateFrom", CheckDate(searchModel.CreateDateFrom));

                objParam[9] = (CheckDate(searchModel.CreateDateTo) == DateTime.MinValue) ? new SqlParameter("@CreateDateTo", DBNull.Value) : new SqlParameter("@CreateDateTo", CheckDate(searchModel.CreateDateTo));

                objParam[10] = new SqlParameter("@SalerId", searchModel.SalerId);
                objParam[11] = new SqlParameter("@OperatorId", searchModel.OperatorId);
                objParam[12] = new SqlParameter("@SalerPermission", searchModel.SalerPermission);
                objParam[13] = new SqlParameter("@BookingCode", searchModel.BookingCode);
                return dbWorker.GetDataTable(StoreProcedureConstant.SP_CountHotelBookingByStatus, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetPagingList - ContractDAL: " + ex);
            }
            return null;
        }
        public async Task<int> UpdateHotelBooking(HotelBooking model)
        {
            try
            {

                SqlParameter[] objParam = new SqlParameter[31];
                objParam[0] = new SqlParameter("@Id", model.Id);
                objParam[1] = model.OrderId == 0 ? new SqlParameter("@OrderId", DBNull.Value) : new SqlParameter("@OrderId", model.OrderId);
                objParam[2] = model.BookingId == null ? new SqlParameter("@BookingId", DBNull.Value) : new SqlParameter("@BookingId", model.BookingId);
                objParam[3] = model.PropertyId == null ? new SqlParameter("@PropertyId", DBNull.Value) : new SqlParameter("@PropertyId", model.PropertyId);
                objParam[4] = model.HotelType == null ? new SqlParameter("@HotelType", DBNull.Value) : new SqlParameter("@HotelType", model.HotelType);
                objParam[5] = model.ArrivalDate == DateTime.MinValue ? new SqlParameter("@ArrivalDate", DBNull.Value) : new SqlParameter("@ArrivalDate", model.ArrivalDate);
                objParam[6] = model.DepartureDate == DateTime.MinValue ? new SqlParameter("@DepartureDate", DBNull.Value) : new SqlParameter("@DepartureDate", model.DepartureDate);
                objParam[7] = model.NumberOfRoom == 0 ? new SqlParameter("@numberOfRoom", DBNull.Value) : new SqlParameter("@numberOfRoom", model.NumberOfRoom);
                objParam[8] = model.NumberOfAdult == 0 ? new SqlParameter("@numberOfAdult", DBNull.Value) : new SqlParameter("@numberOfAdult", model.NumberOfAdult);
                objParam[9] = model.NumberOfChild == 0 ? new SqlParameter("@numberOfChild", DBNull.Value) : new SqlParameter("@numberOfChild", model.NumberOfChild);
                objParam[10] = model.NumberOfInfant == 0 ? new SqlParameter("@numberOfInfant", DBNull.Value) : new SqlParameter("@numberOfInfant", model.NumberOfInfant);
                objParam[11] = model.TotalPrice == 0 ? new SqlParameter("@totalPrice", DBNull.Value) : new SqlParameter("@totalPrice", model.TotalPrice);
                objParam[12] = model.TotalProfit == 0 ? new SqlParameter("@totalProfit", DBNull.Value) : new SqlParameter("@totalProfit", model.TotalProfit);
                objParam[13] = model.TotalAmount == 0 ? new SqlParameter("@totalAmount", DBNull.Value) : new SqlParameter("@totalAmount", model.TotalAmount);
                objParam[14] = model.Status == null ? new SqlParameter("@Status", DBNull.Value) : new SqlParameter("@Status", model.Status);
                objParam[15] = model.HotelName == null ? new SqlParameter("@HotelName", DBNull.Value) : new SqlParameter("@HotelName", model.HotelName);
                objParam[16] = model.Telephone == null ? new SqlParameter("@Telephone", DBNull.Value) : new SqlParameter("@Telephone", model.Telephone);
                objParam[17] = model.Email == null ? new SqlParameter("@Email", DBNull.Value) : new SqlParameter("@Email", model.Email);
                objParam[18] = model.Address == null ? new SqlParameter("@Address", DBNull.Value) : new SqlParameter("@Address", model.Address);
                objParam[19] = model.ImageThumb == null ? new SqlParameter("@ImageThumb", DBNull.Value) : new SqlParameter("@ImageThumb", model.ImageThumb);
                objParam[20] = model.CheckoutTime == null ? new SqlParameter("@CheckoutTime", DBNull.Value) : new SqlParameter("@CheckoutTime", model.CheckoutTime);
                objParam[21] = model.CheckinTime == null ? new SqlParameter("@CheckinTime", DBNull.Value) : new SqlParameter("@CheckinTime", model.CheckinTime);
                objParam[22] = model.ExtraPackageAmount == null ? new SqlParameter("@ExtraPackageAmount", DBNull.Value) : new SqlParameter("@ExtraPackageAmount", model.ExtraPackageAmount);
                objParam[23] = model.SalerId == null ? new SqlParameter("@SalerId", DBNull.Value) : new SqlParameter("@SalerId", model.SalerId);
                objParam[24] = model.ServiceCode == null ? new SqlParameter("@ServiceCode", DBNull.Value) : new SqlParameter("@ServiceCode", model.ServiceCode);
                objParam[25] = model.Price == null ? new SqlParameter("@Price", DBNull.Value) : new SqlParameter("@Price", model.Price);
                objParam[26] = model.SupplierId == null ? new SqlParameter("@SupplierId", DBNull.Value) : new SqlParameter("@SupplierId", model.SupplierId);
                objParam[27] = model.UpdatedBy == null ? new SqlParameter("@UpdatedBy", DBNull.Value) : new SqlParameter("@UpdatedBy", model.UpdatedBy);
                if (model.Note != null)
                {
                    objParam[28] = new SqlParameter("@Note", model.Note);
                }
                else
                {
                    objParam[28] = new SqlParameter("@Note", DBNull.Value);
                }
                objParam[29] = model.TotalDiscount == null ? new SqlParameter("@TotalDiscount", DBNull.Value) : new SqlParameter("@TotalDiscount", model.TotalDiscount);
                objParam[30] = model.TotalDiscount == null ? new SqlParameter("@TotalOthersAmount", DBNull.Value) : new SqlParameter("@TotalOthersAmount", model.TotalOthersAmount);
                return dbWorker.ExecuteNonQuery(StoreProcedureConstant.SP_UpdateHotelBooking, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateHotelBooking - HotelBookingDAL: " + ex);
            }
            return 0;
        }
        public async Task<int> InsertServiceDeclines(ServiceDeclines model)
        {
            try
            {

                SqlParameter[] objParam = new SqlParameter[6];
                objParam[0] = new SqlParameter("@OrderId", model.OrderId);
                objParam[1] = new SqlParameter("@ServiceId", model.ServiceId);
                objParam[2] = new SqlParameter("@Type", model.Type);
                objParam[3] = new SqlParameter("@Note", model.Note);
                objParam[4] = new SqlParameter("@CreatedBy", model.CreatedBy);
                objParam[5] = new SqlParameter("@CreatedDate", DBNull.Value);

                return dbWorker.ExecuteNonQuery(StoreProcedureConstant.Sp_InsertServiceDeclines, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetDetailHotelBookingByID - HotelBookingDAL: " + ex);
            }
            return 0;
        }
        public async Task<DataTable> GetServiceDeclinesByServiceId(string ServiceId, int type)
        {
            try
            {

                SqlParameter[] objParam = new SqlParameter[2];
                objParam[0] = new SqlParameter("@ServiceId", ServiceId);
                objParam[1] = new SqlParameter("@Type", type);

                return dbWorker.GetDataTable(StoreProcedureConstant.Sp_GetServiceDeclinesByOrderId, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetDetailHotelBookingByID - HotelBookingDAL: " + ex);
            }
            return null;
        }

        private DateTime CheckDate(string dateTime)
        {
            DateTime _date = DateTime.MinValue;
            if (!string.IsNullOrEmpty(dateTime))
            {
                _date = DateTime.ParseExact(dateTime, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }

            return _date != DateTime.MinValue ? _date : DateTime.MinValue;
        }
        public async Task<long> SummitHotelBookingRoomOptional(OrderManualHotelServiceSQLSummitModel model, int user_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    foreach (var room in model.rooms)
                    {
                        var room_optional = new HotelBookingRoomsOptional()
                        {
                            CreatedBy = user_id,
                            CreatedDate = DateTime.Now,
                            HotelBookingId = model.detail.Id,
                            HotelBookingRoomId = room.detail.Id,
                            NumberOfRooms = room.detail.NumberOfRooms,
                            Id = 0,
                            Price = room.detail.Price,
                            Profit = room.detail.Profit,
                            SupplierId = room.detail.SupplierId != null ? (int)room.detail.SupplierId : 0,
                            TotalAmount = room.detail.Price,
                            UpdatedBy = user_id,
                            UpdatedDate = DateTime.Now
                        };
                        var exists = await _DbContext.HotelBookingRoomsOptional.AsNoTracking().FirstOrDefaultAsync(x => x.HotelBookingId == room_optional.HotelBookingId && x.HotelBookingRoomId == room_optional.HotelBookingRoomId);
                        if (exists != null && exists.Id > 0)
                        {
                            room_optional.Id = exists.Id;
                            exists.CreatedBy = room_optional.CreatedBy;
                            exists.CreatedDate = room_optional.CreatedDate;
                            exists.NumberOfRooms = room_optional.NumberOfRooms;
                            exists.Price = room_optional.Price;
                            exists.Profit = room_optional.Profit;
                            exists.SupplierId = room_optional.SupplierId <=0 ? exists.SupplierId: room_optional.SupplierId;
                            exists.TotalAmount = room_optional.TotalAmount;
                            exists.UpdatedBy = room_optional.UpdatedBy;
                            exists.UpdatedDate = room_optional.UpdatedDate;
                            UpdateHotelBookingRoomsOptional(exists);
                            foreach (var rate in room.rates)
                            {
                                var rate_optional = new HotelBookingRoomRatesOptional
                                {
                                    Price = rate.Price,
                                    Profit = rate.Profit,
                                    TotalAmount = rate.Price,
                                    Id = 0,
                                    HotelBookingRoomOptionalId = room_optional.Id,
                                    UpdatedBy = user_id,
                                    UpdatedDate = DateTime.Now,
                                    CreatedBy = user_id,
                                    CreatedDate = DateTime.Now,
                                    HotelBookingRoomRatesId = rate.Id,
                                    OperatorPrice = rate.OperatorPrice
                                };
                                var exists_rate_optional = await _DbContext.HotelBookingRoomRatesOptional.AsNoTracking().FirstOrDefaultAsync(x => x.HotelBookingRoomRatesId == rate.Id);
                                if (exists_rate_optional != null && exists_rate_optional.Id > 0)
                                {
                                    exists_rate_optional.CreatedBy = rate_optional.CreatedBy;
                                    exists_rate_optional.CreatedDate = rate_optional.CreatedDate;
                                    exists_rate_optional.Price = rate_optional.Price;
                                    exists_rate_optional.Profit = rate_optional.Profit;
                                    exists_rate_optional.TotalAmount = rate_optional.TotalAmount;
                                    exists_rate_optional.UpdatedBy = rate_optional.UpdatedBy;
                                    exists_rate_optional.UpdatedDate = rate_optional.UpdatedDate;
                                    exists_rate_optional.OperatorPrice = rate_optional.OperatorPrice;
                                    UpdateHotelBookingRoomsRateOptional(exists_rate_optional);
                                }
                                else
                                {
                                    CreateHotelBookingRoomsRateOptional(rate_optional);
                                }
                            }
                        }
                        else
                        {
                            room_optional.Id = CreateHotelBookingRoomsOptional(room_optional);
                            foreach (var rate in room.rates)
                            {
                                var rate_optional = new HotelBookingRoomRatesOptional
                                {
                                    Price = rate.Price,
                                    Profit = rate.Profit,
                                    TotalAmount = rate.Price,
                                    Id = 0,
                                    HotelBookingRoomOptionalId = room_optional.Id,
                                    UpdatedBy = user_id,
                                    UpdatedDate = DateTime.Now,
                                    CreatedBy = user_id,
                                    CreatedDate = DateTime.Now,
                                    HotelBookingRoomRatesId = rate.Id,
                                    OperatorPrice = rate.OperatorPrice
                                };
                                CreateHotelBookingRoomsRateOptional(rate_optional);

                            }
                        }

                    }
                }
                return 0;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CreateHotelBookingRoomOptional - HotelBookingDAL. " + ex.ToString());
                return -1;
            }
        }


        public int CreateHotelBookingRoomsOptional(HotelBookingRoomsOptional booking)
        {
            try
            {

                SqlParameter[] objParam_order = new SqlParameter[11];
                objParam_order[0] = new SqlParameter("@HotelBookingId", booking.HotelBookingId);
                objParam_order[1] = new SqlParameter("@HotelBookingRoomId", booking.HotelBookingRoomId);
                objParam_order[2] = new SqlParameter("@Price", booking.Price);
                objParam_order[3] = new SqlParameter("@Profit", booking.Profit);
                objParam_order[4] = new SqlParameter("@TotalAmount", booking.TotalAmount);
                objParam_order[5] = new SqlParameter("@CreatedBy", booking.CreatedBy);
                objParam_order[6] = new SqlParameter("@CreatedDate", booking.CreatedDate);
                objParam_order[7] = new SqlParameter("@NumberOfRooms", booking.NumberOfRooms);
                objParam_order[8] = new SqlParameter("@SupplierId", booking.SupplierId);
                objParam_order[9] = new SqlParameter("@PackageName", booking.PackageName);
                if (booking.IsRoomFund != null)
                {
                    objParam_order[10] = new SqlParameter("@IsRoomFund", booking.IsRoomFund);
                }
                else
                {
                    objParam_order[10] = new SqlParameter("@IsRoomFund", DBNull.Value);

                }
                var id = dbWorker.ExecuteNonQuery(StoreProcedureConstant.InsertHotelBookingRoomsOptional, objParam_order);
                booking.Id = id;
                return id;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CreateHotelBookingRoomsOptional - HotelBookingDAL. " + ex.ToString());
                return -1;
            }
        }
        public int UpdateHotelBookingRoomsOptional(HotelBookingRoomsOptional booking)
        {
            try
            {

                SqlParameter[] objParam_order = new SqlParameter[11];
                objParam_order[0] = new SqlParameter("@HotelBookingId", booking.HotelBookingId);
                objParam_order[1] = new SqlParameter("@HotelBookingRoomId", booking.HotelBookingRoomId);
                objParam_order[2] = new SqlParameter("@Price", booking.Price);
                objParam_order[3] = new SqlParameter("@Profit", booking.Profit);
                objParam_order[4] = new SqlParameter("@TotalAmount", booking.TotalAmount);
                objParam_order[5] = new SqlParameter("@UpdatedBy", booking.UpdatedBy);
                objParam_order[6] = new SqlParameter("@NumberOfRooms", booking.NumberOfRooms);
                objParam_order[7] = new SqlParameter("@SupplierId", booking.SupplierId);
                objParam_order[8] = new SqlParameter("@Id", booking.Id);
                objParam_order[9] = new SqlParameter("@PackageName", booking.PackageName);
                if (booking.IsRoomFund != null)
                {
                    objParam_order[10] = new SqlParameter("@IsRoomFund", booking.IsRoomFund);
                }
                else
                {
                    objParam_order[10] = new SqlParameter("@IsRoomFund", DBNull.Value);

                }
                var id = dbWorker.ExecuteNonQuery(StoreProcedureConstant.UpdateHotelBookingRoomsOptional, objParam_order);
                booking.Id = id;
                return id;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateHotelBookingRoomsOptional - HotelBookingDAL. " + ex.ToString());
                return -1;
            }
        }
        public async Task<DataTable> GetHotelBookingRoomsOptionalByBookingId(long hotelbookingid)
        {
            try
            {

                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@HotelBookingId", hotelbookingid);

                return dbWorker.GetDataTable(StoreProcedureConstant.GetListHotelBookingRoomsOptionalByBookingId, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetHotelBookingRoomsOptionalByBookingId - ContractDAL: " + ex.ToString());
            }
            return null;
        }

        public async Task<DataTable> GetListHotelBookingRoomExtraPackagesByBookingId(long hotelbookingid)
        {
            try
            {

                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@HotelBookingId", hotelbookingid);

                return dbWorker.GetDataTable(StoreProcedureConstant.SP_GetListHotelBookingRoomsExtraPackageByBookingId, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetListHotelBookingRoomExtraPackagesByBookingId - ContractDAL: " + ex.ToString());
            }
            return null;
        }

        public int CreateHotelBookingRoomsRateOptional(HotelBookingRoomRatesOptional booking)
        {
            try
            {

                SqlParameter[] objParam_order = new SqlParameter[8];
                objParam_order[0] = new SqlParameter("@HotelBookingRoomRatesId", booking.HotelBookingRoomRatesId);
                objParam_order[1] = new SqlParameter("@HotelBookingRoomOptionalId", booking.HotelBookingRoomOptionalId);
                objParam_order[2] = new SqlParameter("@Price", booking.Price);
                objParam_order[3] = new SqlParameter("@Profit", booking.Profit);
                objParam_order[4] = new SqlParameter("@TotalAmount", booking.TotalAmount);
                objParam_order[5] = new SqlParameter("@CreatedBy", booking.CreatedBy);
                objParam_order[6] = new SqlParameter("@CreatedDate", booking.CreatedDate);
                objParam_order[7] = new SqlParameter("@OperatorPrice", booking.OperatorPrice);
                var id = dbWorker.ExecuteNonQuery(StoreProcedureConstant.InsertHotelBookingRoomRatesOptional, objParam_order);
                booking.Id = id;
                return id;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CreateHotelBookingRoomsRateOptional - HotelBookingDAL. " + ex.ToString());
                return -1;
            }
        }
        public int UpdateHotelBookingRoomsRateOptional(HotelBookingRoomRatesOptional booking)
        {
            try
            {

                SqlParameter[] objParam_order = new SqlParameter[8];
                objParam_order[0] = new SqlParameter("@HotelBookingRoomRatesId", booking.HotelBookingRoomRatesId);
                objParam_order[1] = new SqlParameter("@HotelBookingRoomOptionalId", booking.HotelBookingRoomOptionalId);
                objParam_order[2] = new SqlParameter("@Price", booking.Price);
                objParam_order[3] = new SqlParameter("@Profit", booking.Profit);
                objParam_order[4] = new SqlParameter("@TotalAmount", booking.TotalAmount);
                objParam_order[5] = new SqlParameter("@UpdatedBy", booking.UpdatedBy);
                objParam_order[6] = new SqlParameter("@Id", booking.Id);
                objParam_order[7] = new SqlParameter("@OperatorPrice", booking.OperatorPrice);
                var id = dbWorker.ExecuteNonQuery(StoreProcedureConstant.UpdateHotelBookingRoomRatesOptional, objParam_order);
                booking.Id = id;
                return id;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateHotelBookingRoomsRateOptional - HotelBookingDAL. " + ex.ToString());
                return -1;
            }
        }
        public async Task<DataTable> GetHotelBookingRoomRatesOptionalByBookingId(long hotelbookingid)
        {
            try
            {

                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@HotelBookingId", hotelbookingid);

                return dbWorker.GetDataTable(StoreProcedureConstant.GetListHotelBookingRoomRatesOptionalByBookingId, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetPagingList - ContractDAL: " + ex);
            }
            return null;
        }
        public async Task<bool> DeleteNonExistsHotelOptionalData(List<long> remain_room, List<long> remain_rate, long hotel_booking_id, int? updated_user)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var hotel_room = await _DbContext.HotelBookingRoomsOptional.AsNoTracking().Where(x => x.HotelBookingId == hotel_booking_id && !remain_room.Contains(x.Id)).ToListAsync();
                    if (hotel_room != null && hotel_room.Count > 0)
                    {
                        foreach (var guest in hotel_room)
                        {
                            guest.HotelBookingId = guest.HotelBookingId * -1;
                            guest.UpdatedBy = updated_user;
                            var delete_packages = await _DbContext.HotelBookingRoomRatesOptional.AsNoTracking().Where(x => x.HotelBookingRoomOptionalId == guest.Id && !remain_rate.Contains(x.Id)).ToListAsync();
                            if (delete_packages != null && delete_packages.Count > 0)
                            {
                                foreach (var package in delete_packages)
                                {
                                    package.HotelBookingRoomOptionalId = package.HotelBookingRoomOptionalId * -1;
                                    package.UpdatedBy = updated_user;
                                    package.TotalAmount = 0;
                                    UpdateHotelBookingRoomsRateOptional(package);
                                }
                            }
                            UpdateHotelBookingRoomsOptional(guest);

                        }
                    }

                }
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("DeleteNonExistsTourData - TourDAL. " + ex.ToString());
                return false;
            }
        }

        public DataTable GetHotelRevenue(ReportClientDebtSearchModel searchModel)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[9];
                if (searchModel.HotelIds == null || searchModel.HotelIds.Count == 0)
                    objParam[0] = new SqlParameter("@HotelId", DBNull.Value);
                else
                    objParam[0] = new SqlParameter("@HotelId", string.Join(",", searchModel.HotelIds));
                if (searchModel.CheckInDateFrom == null)
                    objParam[1] = new SqlParameter("@CheckinDateFrom", DBNull.Value);
                else
                    objParam[1] = new SqlParameter("@CheckinDateFrom", searchModel.CheckInDateFrom);
                if (searchModel.CheckInDateTo == null)
                    objParam[2] = new SqlParameter("@CheckinDateTo", DBNull.Value);
                else
                    objParam[2] = new SqlParameter("@CheckinDateTo", searchModel.CheckInDateTo);
                if (searchModel.CheckOutDateFrom == null)
                    objParam[3] = new SqlParameter("@CheckoutDateFrom", DBNull.Value);
                else
                    objParam[3] = new SqlParameter("@CheckoutDateFrom", searchModel.CheckOutDateFrom);
                if (searchModel.CheckOutDateTo == null)
                    objParam[4] = new SqlParameter("@CheckoutDateTo", DBNull.Value);
                else
                    objParam[4] = new SqlParameter("@CheckoutDateTo", searchModel.CheckOutDateTo);
                if (searchModel.CreateDateFrom == null)
                    objParam[5] = new SqlParameter("@CreateDateFrom", DBNull.Value);
                else
                    objParam[5] = new SqlParameter("@CreateDateFrom", searchModel.CreateDateFrom);
                if (searchModel.CreateDateTo == null)
                    objParam[6] = new SqlParameter("@CreateDateTo", DBNull.Value);
                else
                    objParam[6] = new SqlParameter("@CreateDateTo", searchModel.CreateDateTo);
                if (searchModel.PageSize == -1)
                {
                    objParam[7] = new SqlParameter("@PageIndex", -1);
                    objParam[8] = new SqlParameter("@PageSize", DBNull.Value);
                }
                else
                {
                    objParam[7] = new SqlParameter("@PageIndex", searchModel.PageIndex);
                    objParam[8] = new SqlParameter("@PageSize", searchModel.PageSize);
                }
                return dbWorker.GetDataTable(StoreProcedureConstant.sp_GetReportRevenueHotel, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetHotelRevenue - HotelBookingDAL: " + ex.ToString());
            }
            return null;
        }
        public async Task<DataTable> GetListHotelBookingRoomByHotelBookingId(List<string>  Ids)
        {
            try
            {

                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@HotelBookingId", string.Join(',', Ids));

                return dbWorker.GetDataTable(StoreProcedureConstant.Sp_GetListHotelBookingRoomByHotelBookingId, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetDetailHotelBookingByID - HotelBookingDAL: " + ex);
            }
            return null;
        }
    }
}
