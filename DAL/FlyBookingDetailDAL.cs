using DAL.Generic;
using DAL.StoreProcedure;
using Entities.Models;
using Entities.ViewModels;
using Entities.ViewModels.HotelBooking;
using Entities.ViewModels.OrderManual;
using Entities.ViewModels.SetServices;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace DAL
{
    public class FlyBookingDetailDAL : GenericService<FlyBookingDetail>
    {
        private static DbWorker _DbWorker;
        public FlyBookingDetailDAL(string connection) : base(connection)
        {
            _DbWorker = new DbWorker(connection);
        }
        public FlyBookingDetail GetDetail(long orderId)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.FlyBookingDetail.AsNoTracking().FirstOrDefault(s => s.OrderId == orderId);
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetDetail - FlyBookingDetailDAL: " + ex);
                return null;
            }
        }

        public FlyBookingDetail GetDetailByLeg(long orderId, int leg, string group_fly = null)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.FlyBookingDetail.AsNoTracking().FirstOrDefault(s => s.OrderId == orderId && s.GroupBookingId == group_fly.Trim() && s.Leg == leg);
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetDetailByLeg - FlyBookingDetailDAL: " + ex);
                return null;
            }
        }
        public List<FlyBookingDetail> GetListByOrderId(long orderId)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.FlyBookingDetail.AsNoTracking().Where(s => s.OrderId == orderId).ToList();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetListByOrderId - FlyBookingDetailDAL: " + ex);
                return null;
            }
        }
        public async Task<List<FlyBookingDetail>> GetListByGroupFlyID(long orderId, string group_fly)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return await _DbContext.FlyBookingDetail.AsNoTracking().Where(s => s.OrderId == orderId && s.GroupBookingId.Trim() == group_fly.Trim()).ToListAsync();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetListByGroupFlyID - FlyBookingDetailDAL: " + ex);
                return null;
            }
        }
        public async Task<FlyBookingDetail> GetFlyBookingById(long id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return await _DbContext.FlyBookingDetail.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetFlyBookingById - FlyBookingDetailDAL: " + ex);
                return null;
            }
        }
        public async Task<List<FlyBookingDetail>> GetListByGroupFlyID(string group_fly)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return await _DbContext.FlyBookingDetail.AsNoTracking().Where(s => s.GroupBookingId.Trim() == group_fly.Trim()).ToListAsync();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetListByGroupFlyID - FlyBookingDetailDAL: " + ex);
                return null;
            }
        }
        public async Task<long> UpdateServiceCode(string group_booking_id, string service_code)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var exists = await _DbContext.FlyBookingDetail.AsNoTracking().Where(s => s.GroupBookingId.Trim() == group_booking_id.Trim()).ToListAsync();
                    if (exists != null && exists.Count > 0)
                    {
                        foreach (var item in exists)
                        {
                            item.ServiceCode = service_code;
                            item.UpdatedDate = DateTime.Now;
                            _DbContext.FlyBookingDetail.Update(item);
                            await _DbContext.SaveChangesAsync();
                        }
                    }
                    return 1;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetListByGroupFlyID - FlyBookingDetailDAL: " + ex);
                return -2;
            }
        }

        public List<Bookingdetail> GetListBookingdetailByOrderId(long orderId)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {

                    var listbooking = (from a in _DbContext.FlyBookingDetail.AsNoTracking().OrderByDescending(s => s.ExpiryDate).Where(s => s.OrderId == orderId)
                                       join g in _DbContext.User.AsNoTracking() on a.SalerId equals g.Id
                                       join c in _DbContext.Airlines.AsNoTracking() on a.Airline equals c.Code
                                       join d in _DbContext.AirPortCode.AsNoTracking() on a.StartPoint equals d.Code
                                       join e in _DbContext.AirPortCode.AsNoTracking() on a.EndPoint equals e.Code
                                       join f in _DbContext.Order.AsNoTracking().Where(s => s.OrderId == orderId) on a.OrderId equals f.OrderId
                                       select new Bookingdetail
                                       {
                                           Id = a.Id,
                                           OrderId = a.OrderId,
                                           EndPoint = a.EndPoint,
                                           EndDistrict = e.DistrictVi,
                                           StartPoint = a.StartPoint,
                                           StartDistrict = d.DistrictVi,
                                           Leg = a.Leg,
                                           AirlineLogo = c.Logo,
                                           AirlineName_Vi = c.NameVi,
                                           BookingCode = a.BookingCode,
                                           Amount = (double)a.Amount,
                                           Discount = (double)f.Discount,
                                           GroupBookingId = a.GroupBookingId,
                                           StartDate = ((DateTime)a.StartDate),
                                           EndDate = ((DateTime)a.EndDate),
                                           SalerIdName = g.FullName,
                                       }).ToList();
                    return listbooking;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetListByOrderId - FlyBookingDetailDAL: " + ex);
                return null;
            }
        }
        public async Task<long> UpdateFlyBooking(OrderManualFlyBookingSQLServiceSummitModel model)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var order = await _DbContext.Order.AsNoTracking().FirstOrDefaultAsync(x => x.OrderId == model.order_id);
                    var exists_go = _DbContext.FlyBookingDetail.AsNoTracking().FirstOrDefault(x => x.Id == model.go.Id);
                    if (exists_go != null)
                    {
                        _DbContext.FlyBookingDetail.Update(model.go);
                        await _DbContext.SaveChangesAsync();
                    }
                    else
                    {
                        var id = CreateFlyBookingDetail(model.go);
                    }
                    if (model.back != null)
                    {
                        var exists_back = _DbContext.FlyBookingDetail.AsNoTracking().FirstOrDefault(x => x.Id == model.back.Id);
                        if (exists_back != null)
                        {
                            UpdateFlyBookingDetail(model.back);
                            //--Update amount by back:
                            double total_amount = model.total_amount / (double)2;
                            if (order.Amount == null || order.Amount <= 0) order.Amount = total_amount;
                            else
                            {
                                order.Amount = order.Amount - exists_back.Amount + total_amount;
                            }
                            if (order.Profit == null) order.Profit = model.profit / (double)2;
                            else
                            {
                                order.Profit = order.Profit - exists_back.Amount + model.profit / (double)2;
                            }
                            order.Price = order.Amount;
                            _DbContext.Order.Update(order);
                            await _DbContext.SaveChangesAsync();
                        }
                        else
                        {
                            CreateFlyBookingDetail(model.back);
                            //--add amount by back:
                            double total_amount = model.total_amount / (double)2;
                            if (order.Amount == null || order.Amount <= 0) order.Amount = total_amount;
                            else
                            {
                                order.Amount += total_amount;
                            }
                            if (order.Profit == null) order.Profit = model.profit / (double)2;
                            else
                            {
                                order.Profit += model.profit / (double)2;
                            }
                            order.Price = order.Amount;
                            _DbContext.Order.Update(order);
                            await _DbContext.SaveChangesAsync();
                        }
                    }
                    else
                    {
                        var exists_back = _DbContext.FlyBookingDetail.AsNoTracking().FirstOrDefault(x => x.GroupBookingId == model.go.GroupBookingId && x.Leg == 1);
                        if (exists_back != null && exists_back.Id > 0)
                        {
                            _DbContext.FlyBookingDetail.Remove(exists_back);
                            await _DbContext.SaveChangesAsync();
                        }
                    }
                    string group_id = model.go.Id.ToString();
                    List<int?> fly_booking_ids = new List<int?>() { (int?)model.go.Id };
                    if (model.back != null)
                    {
                        group_id = model.go.Id + "," + model.back.Id;
                        model.back.GroupBookingId = group_id;
                        model.back.Note = (model.back.Note != null && model.back.Note.Trim() != "" && model.back.Note.Length >= 500 ? model.back.Note.Substring(0, 499) : model.back.Note);
                        _DbContext.FlyBookingDetail.Update(model.back);
                        await _DbContext.SaveChangesAsync();
                        fly_booking_ids.Add((int?)model.back.Id);
                    }
                    model.go.GroupBookingId = group_id;
                    UpdateFlyBookingDetail(model.go);

                    List<long> ids_extra_new = new List<long>();
                    if (model.extra_packages.Count > 0)
                    {
                        foreach (var package in model.extra_packages)
                        {
                            var exists_package = await _DbContext.FlyBookingExtraPackages.AsNoTracking().FirstOrDefaultAsync(x => x.Id == package.Id);
                            if (exists_package != null)
                            {
                                package.GroupFlyBookingId = group_id;
                                _DbContext.FlyBookingExtraPackages.Update(package);
                                await _DbContext.SaveChangesAsync();
                                ids_extra_new.Add(package.Id);
                            }
                            else
                            {
                                package.GroupFlyBookingId = group_id;
                                _DbContext.FlyBookingExtraPackages.Add(package);
                                await _DbContext.SaveChangesAsync();
                                ids_extra_new.Add(package.Id);
                            }

                        }
                    }
                    var delete_packages = _DbContext.FlyBookingExtraPackages.AsNoTracking().Where(x => x.GroupFlyBookingId == group_id && !ids_extra_new.Contains(x.Id));
                    if (delete_packages != null && delete_packages.Count() > 0)
                    {
                        _DbContext.FlyBookingExtraPackages.RemoveRange(delete_packages);
                        await _DbContext.SaveChangesAsync();
                    }
                    List<long> ids_new = new List<long>();

                    if (model.passengers.Count > 0)
                    {

                        foreach (var passengers in model.passengers)
                        {
                            passengers.GroupBookingId = group_id;
                            var exists_package = await _DbContext.Passenger.AsNoTracking().FirstOrDefaultAsync(x => x.Id == passengers.Id);
                            if (exists_package != null)
                            {
                                _DbContext.Passenger.Update(passengers);
                                await _DbContext.SaveChangesAsync();
                            }
                            else
                            {
                                _DbContext.Passenger.Add(passengers);
                                await _DbContext.SaveChangesAsync();
                            }
                            ids_new.Add(passengers.Id);
                        }
                    }
                    var delete_passengers = _DbContext.Passenger.AsNoTracking().Where(x => x.OrderId == model.order_id && x.GroupBookingId == group_id && !ids_new.Contains(x.Id));
                    if (delete_passengers != null && delete_passengers.Count() > 0)
                    {
                        _DbContext.Passenger.RemoveRange(delete_passengers);
                        await _DbContext.SaveChangesAsync();
                    }

                }
                return model.go.Id;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateFlyBooking - FlyBookingDetailDAL: " + ex);
                return -2;
            }
        }
        public async Task<long> DeleteFlyBookingByID(string group_booking_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var list = await _DbContext.FlyBookingDetail.AsNoTracking().Where(x => x.GroupBookingId == group_booking_id).ToListAsync();
                    if (list != null && list.Count > 0)
                    {
                        foreach (var booking in list)
                        {
                            var flight_segment = await _DbContext.FlightSegment.AsNoTracking().Where(x => x.FlyBookingId == booking.Id).ToListAsync();
                            if (flight_segment != null && flight_segment.Count > 0)
                            {
                                _DbContext.FlightSegment.RemoveRange(flight_segment);
                                await _DbContext.SaveChangesAsync();
                            }

                        }
                        var fly_extra = await _DbContext.FlyBookingExtraPackages.AsNoTracking().Where(x => x.GroupFlyBookingId == group_booking_id).ToListAsync();
                        if (fly_extra != null && fly_extra.Count > 0)
                        {
                            _DbContext.FlyBookingExtraPackages.RemoveRange(fly_extra);
                            await _DbContext.SaveChangesAsync();
                        }

                        var passengers = await _DbContext.Passenger.AsNoTracking().Where(x => x.GroupBookingId == group_booking_id).ToListAsync();
                        if (passengers != null && passengers.Count > 0)
                        {
                            _DbContext.Passenger.RemoveRange(passengers);
                            await _DbContext.SaveChangesAsync();
                        }
                        _DbContext.FlyBookingDetail.RemoveRange(list);
                        await _DbContext.SaveChangesAsync();
                    }
                    return 1;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("DeleteFlyBookingByID - FlyBookingDetailDAL. " + ex);
                return -1;
            }
        }
        public async Task<long> CancelFlyBookingByID(string group_booking_id, int user_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var list = await _DbContext.FlyBookingDetail.AsNoTracking().Where(x => x.GroupBookingId == group_booking_id).ToListAsync();
                    if (list != null && list.Count > 0)
                    {
                        foreach (var booking in list)
                        {
                            if (booking.Status == (int)ServiceStatus.Decline)
                            {
                                booking.Status = (int)ServiceStatus.Cancel;
                                booking.UpdatedBy = user_id;
                                booking.UpdatedDate = DateTime.Now;
                                _DbContext.FlyBookingDetail.Update(booking);
                                await _DbContext.SaveChangesAsync();
                            }

                        }
                    }
                    return 1;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("DeleteFlyBookingByID - FlyBookingDetailDAL. " + ex);
                return -1;
            }
        }
        private int CreateFlyBookingDetail(FlyBookingDetail model)
        {
            try
            {

                SqlParameter[] objParam_order = new SqlParameter[53];
                objParam_order[0] = new SqlParameter("@OrderId", model.OrderId);
                objParam_order[1] = new SqlParameter("@PriceId", model.PriceId);
                objParam_order[2] = new SqlParameter("@BookingCode", model.BookingCode);
                objParam_order[3] = new SqlParameter("@Amount", model.Amount);
                objParam_order[4] = new SqlParameter("@Difference", model.Difference);
                objParam_order[5] = new SqlParameter("@Currency", model.Currency);
                objParam_order[6] = new SqlParameter("@Flight", model.Flight);
                objParam_order[7] = new SqlParameter("@ExpiryDate", model.ExpiryDate);
                objParam_order[8] = new SqlParameter("@Session", model.Session);
                objParam_order[9] = new SqlParameter("@Airline", model.Airline);
                objParam_order[10] = new SqlParameter("@StartPoint", model.StartPoint);
                objParam_order[11] = new SqlParameter("@EndPoint", model.EndPoint);
                objParam_order[12] = new SqlParameter("@GroupClass", model.GroupClass);
                objParam_order[13] = new SqlParameter("@Leg", model.Leg);
                objParam_order[14] = new SqlParameter("@AdultNumber", model.AdultNumber);
                objParam_order[15] = new SqlParameter("@ChildNumber", model.ChildNumber);
                objParam_order[16] = new SqlParameter("@InfantNumber", model.InfantNumber);
                objParam_order[17] = new SqlParameter("@FareAdt", model.FareAdt);
                objParam_order[18] = new SqlParameter("@FareChd", model.FareChd);
                objParam_order[19] = new SqlParameter("@FareInf", model.FareInf);
                objParam_order[20] = new SqlParameter("@TaxAdt", model.TaxAdt);
                objParam_order[21] = new SqlParameter("@TaxChd", model.TaxChd);
                objParam_order[22] = new SqlParameter("@TaxInf", model.TaxInf);
                objParam_order[23] = new SqlParameter("@FeeAdt", model.FeeAdt);
                objParam_order[24] = new SqlParameter("@FeeChd", model.FeeChd);
                objParam_order[25] = new SqlParameter("@FeeInf", model.FeeInf);
                objParam_order[26] = new SqlParameter("@ServiceFeeAdt", model.ServiceFeeAdt);
                objParam_order[27] = new SqlParameter("@ServiceFeeChd", model.ServiceFeeChd);
                objParam_order[28] = new SqlParameter("@ServiceFeeInf", model.ServiceFeeInf);
                objParam_order[29] = new SqlParameter("@AmountAdt", model.AmountAdt);
                objParam_order[30] = new SqlParameter("@AmountChd", model.AmountChd);
                objParam_order[31] = new SqlParameter("@AmountInf", model.AmountInf);
                objParam_order[32] = new SqlParameter("@TotalNetPrice", model.TotalNetPrice);
                objParam_order[33] = new SqlParameter("@TotalDiscount", model.TotalDiscount);
                objParam_order[34] = new SqlParameter("@TotalCommission", model.TotalCommission);
                objParam_order[35] = new SqlParameter("@TotalBaggageFee", model.TotalBaggageFee);
                objParam_order[36] = new SqlParameter("@StartDate", model.StartDate);
                objParam_order[37] = new SqlParameter("@EndDate", model.EndDate);
                objParam_order[38] = new SqlParameter("@BookingId", model.BookingId);
                objParam_order[39] = new SqlParameter("@Status", model.Status);
                objParam_order[40] = new SqlParameter("@Profit", model.Profit);
                if (model.ServiceCode != null)
                {
                    objParam_order[41] = new SqlParameter("@ServiceCode", model.ServiceCode);
                }
                else
                {
                    objParam_order[41] = new SqlParameter("@ServiceCode", DBNull.Value);

                }
                objParam_order[42] = new SqlParameter("@Price", model.Price);
                objParam_order[43] = new SqlParameter("@PriceAdt", model.PriceAdt);
                objParam_order[44] = new SqlParameter("@PriceChd", model.PriceChd);
                objParam_order[45] = new SqlParameter("@PriceInf", model.PriceInf);
                if (model.SupplierId != null)
                {
                    objParam_order[46] = new SqlParameter("@SupplierId", model.SupplierId);
                }
                else
                {
                    objParam_order[46] = new SqlParameter("@SupplierId", DBNull.Value);

                }
                if (model.Note != null)
                {
                    objParam_order[47] = new SqlParameter("@Note", model.Note);
                }
                else
                {
                    objParam_order[47] = new SqlParameter("@Note", DBNull.Value);

                }
                objParam_order[48] = new SqlParameter("@ProfitAdt", model.ProfitAdt);
                objParam_order[49] = new SqlParameter("@ProfitChd", model.ProfitChd);
                objParam_order[50] = new SqlParameter("@ProfitInf", model.ProfitInf);
                objParam_order[51] = new SqlParameter("@AdgCommission", model.Adgcommission);
                if (model.Adgcommission != null)
                {
                    objParam_order[51] = new SqlParameter("@AdgCommission", model.Adgcommission);
                }
                else
                {
                    objParam_order[51] = new SqlParameter("@AdgCommission", DBNull.Value);

                }
                objParam_order[52] = new SqlParameter("@OthersAmount", model.OthersAmount);
                var id = _DbWorker.ExecuteNonQuery(StoreProcedureConstant.InsertFlyBookingDetail, objParam_order);
                model.Id = id;
                return id;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CreateFlyBookingDetail - FlyBookingDetailDAL. " + ex);
                return -1;
            }
        }
        public int UpdateFlyBookingDetail(FlyBookingDetail model)
        {
            try
            {

                SqlParameter[] objParam_order = new SqlParameter[57];
                objParam_order[0] = new SqlParameter("@OrderId", model.OrderId);
                objParam_order[1] = new SqlParameter("@PriceId", model.PriceId);
                objParam_order[2] = new SqlParameter("@BookingCode", model.BookingCode);
                objParam_order[3] = new SqlParameter("@Amount", model.Amount);
                objParam_order[4] = new SqlParameter("@Difference", model.Difference);
                objParam_order[5] = new SqlParameter("@Currency", model.Currency);
                objParam_order[6] = new SqlParameter("@Flight", model.Flight);
                objParam_order[7] = new SqlParameter("@ExpiryDate", model.ExpiryDate);
                objParam_order[8] = new SqlParameter("@Session", model.Session);
                objParam_order[9] = new SqlParameter("@Airline", model.Airline);
                objParam_order[10] = new SqlParameter("@StartPoint", model.StartPoint);
                objParam_order[11] = new SqlParameter("@EndPoint", model.EndPoint);
                objParam_order[12] = new SqlParameter("@GroupClass", model.GroupClass);
                objParam_order[13] = new SqlParameter("@Leg", model.Leg);
                objParam_order[14] = new SqlParameter("@AdultNumber", model.AdultNumber);
                objParam_order[15] = new SqlParameter("@ChildNumber", model.ChildNumber);
                objParam_order[16] = new SqlParameter("@InfantNumber", model.InfantNumber);
                objParam_order[17] = new SqlParameter("@FareAdt", model.FareAdt);
                objParam_order[18] = new SqlParameter("@FareChd", model.FareChd);
                objParam_order[19] = new SqlParameter("@FareInf", model.FareInf);
                objParam_order[20] = new SqlParameter("@TaxAdt", model.TaxAdt);
                objParam_order[21] = new SqlParameter("@TaxChd", model.TaxChd);
                objParam_order[22] = new SqlParameter("@TaxInf", model.TaxInf);
                objParam_order[23] = new SqlParameter("@FeeAdt", model.FeeAdt);
                objParam_order[24] = new SqlParameter("@FeeChd", model.FeeChd);
                objParam_order[25] = new SqlParameter("@FeeInf", model.FeeInf);
                objParam_order[26] = new SqlParameter("@ServiceFeeAdt", model.ServiceFeeAdt);
                objParam_order[27] = new SqlParameter("@ServiceFeeChd", model.ServiceFeeChd);
                objParam_order[28] = new SqlParameter("@ServiceFeeInf", model.ServiceFeeInf);
                objParam_order[29] = new SqlParameter("@AmountAdt", model.AmountAdt);
                objParam_order[30] = new SqlParameter("@AmountChd", model.AmountChd);
                objParam_order[31] = new SqlParameter("@AmountInf", model.AmountInf);
                objParam_order[32] = new SqlParameter("@TotalNetPrice", model.TotalNetPrice);
                objParam_order[33] = new SqlParameter("@TotalDiscount", model.TotalDiscount);
                objParam_order[34] = new SqlParameter("@TotalCommission", model.TotalCommission);
                objParam_order[35] = new SqlParameter("@TotalBaggageFee", model.TotalBaggageFee);
                objParam_order[36] = new SqlParameter("@StartDate", model.StartDate);
                objParam_order[37] = new SqlParameter("@EndDate", model.EndDate);
                objParam_order[38] = new SqlParameter("@BookingId", model.BookingId);
                objParam_order[39] = new SqlParameter("@Status", model.Status);
                objParam_order[40] = new SqlParameter("@Profit", model.Profit);
                if (model.ServiceCode != null)
                {
                    objParam_order[41] = new SqlParameter("@ServiceCode", model.ServiceCode);
                }
                else
                {
                    objParam_order[41] = new SqlParameter("@ServiceCode", DBNull.Value);

                }
                objParam_order[42] = new SqlParameter("@Id", model.Id);
                objParam_order[43] = new SqlParameter("@UpdatedBy", model.UpdatedBy);
                objParam_order[44] = new SqlParameter("@SalerId", model.SalerId);
                if (model.ServiceCode != null)
                {
                    objParam_order[45] = new SqlParameter("@GroupBookingId", model.GroupBookingId);
                }
                else
                {
                    objParam_order[45] = new SqlParameter("@GroupBookingId", DBNull.Value);

                }
                objParam_order[46] = new SqlParameter("@Price", model.Price);
                objParam_order[47] = new SqlParameter("@PriceAdt", model.PriceAdt);
                if (model.PriceChd != null)
                {
                    objParam_order[48] = new SqlParameter("@PriceChd", model.PriceChd);
                }
                else
                {
                    objParam_order[48] = new SqlParameter("@PriceChd", DBNull.Value);

                }
                if (model.PriceInf != null)
                {
                    objParam_order[49] = new SqlParameter("@PriceInf", model.PriceInf);
                }
                else
                {
                    objParam_order[49] = new SqlParameter("@PriceInf", DBNull.Value);

                }
                if (model.SupplierId != null)
                {
                    objParam_order[50] = new SqlParameter("@SupplierId", model.SupplierId);
                }
                else
                {
                    objParam_order[50] = new SqlParameter("@SupplierId", DBNull.Value);

                }
                if (model.Note != null)
                {
                    objParam_order[51] = new SqlParameter("@Note", model.Note);
                }
                else
                {
                    objParam_order[51] = new SqlParameter("@Note", DBNull.Value);

                }
                objParam_order[52] = new SqlParameter("@ProfitAdt", model.ProfitAdt);
                objParam_order[53] = new SqlParameter("@ProfitChd", model.ProfitChd);
                objParam_order[54] = new SqlParameter("@ProfitInf", model.ProfitInf);
                if (model.Adgcommission != null)
                {
                    objParam_order[55] = new SqlParameter("@AdgCommission", model.Adgcommission);
                }
                else
                {
                    objParam_order[55] = new SqlParameter("@AdgCommission", DBNull.Value);

                }
                objParam_order[56] = new SqlParameter("@OthersAmount", model.OthersAmount == null ? DBNull.Value : model.OthersAmount);
                var id = _DbWorker.ExecuteNonQuery(StoreProcedureConstant.UpdateFlyBookingDetail, objParam_order);
                return id;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateFlyBookingDetail - FlyBookingDetailDAL. " + ex);
                return -1;
            }
        }
        public async Task<DataTable> GetDetailFlyBookingDetailById(long FlyBookingDetailId)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@FlyBookingDetailId", FlyBookingDetailId);

                return _DbWorker.GetDataTable(StoreProcedureConstant.SP_GetDetailFlyBookingDetailById, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetDetailOrderServiceByOrderId - FlyBookingDetailDAL: " + ex);
            }
            return null;
        }

        public DataTable GetPagingList(SearchFlyBookingViewModel searchModel, int currentPage, int pageSize)
        {
            try
            {

                SqlParameter[] objParam = new SqlParameter[16];
                if (searchModel.ServiceCode != null)
                {
                    objParam[0] = new SqlParameter("@ServiceCode", searchModel.ServiceCode);
                }
                else
                {
                    objParam[0] = new SqlParameter("@ServiceCode", DBNull.Value);

                }
                if (searchModel.OrderCode != null)
                {
                    objParam[1] = new SqlParameter("@OrderCode", searchModel.OrderCode);
                }
                else
                {
                    objParam[1] = new SqlParameter("@OrderCode", DBNull.Value);

                }
                objParam[2] = new SqlParameter("@StatusBooking", searchModel.StatusBooking);
                if (searchModel.StartDateFrom != null)
                {
                    objParam[3] = new SqlParameter("@StartDateFrom", ((DateTime)searchModel.StartDateFrom).Date);
                }
                else
                {
                    objParam[3] = new SqlParameter("@StartDateFrom", DBNull.Value);

                }
                if (searchModel.StartDateTo != null)
                {
                    objParam[4] = new SqlParameter("@StartDateTo", ((DateTime)searchModel.StartDateTo).Date);
                }
                else
                {
                    objParam[4] = new SqlParameter("@StartDateTo", DBNull.Value);

                }
                if (searchModel.EndDateFrom != null)
                {
                    objParam[5] = new SqlParameter("@EndDateFrom", ((DateTime)searchModel.EndDateFrom).Date);
                }
                else
                {
                    objParam[5] = new SqlParameter("@EndDateFrom", DBNull.Value);

                }
                if (searchModel.EndDateTo != null)
                {
                    objParam[6] = new SqlParameter("@EndDateTo", ((DateTime)searchModel.EndDateTo).Date);
                }
                else
                {
                    objParam[6] = new SqlParameter("@EndDateTo", DBNull.Value);

                }
                if (searchModel.UserCreate != null)
                {
                    objParam[7] = new SqlParameter("@UserCreate", searchModel.UserCreate);
                }
                else
                {
                    objParam[7] = new SqlParameter("@UserCreate", DBNull.Value);

                }
                if (searchModel.CreateDateFrom != null)
                {
                    objParam[8] = new SqlParameter("@CreateDateFrom", ((DateTime)searchModel.CreateDateFrom).Date);
                }
                else
                {
                    objParam[8] = new SqlParameter("@CreateDateFrom", DBNull.Value);

                }
                if (searchModel.CreateDateTo != null)
                {
                    objParam[9] = new SqlParameter("@CreateDateTo", ((DateTime)searchModel.CreateDateTo).Date);
                }
                else
                {
                    objParam[9] = new SqlParameter("@CreateDateTo", DBNull.Value);

                }
                if (searchModel.SalerId > 0)
                {
                    objParam[10] = new SqlParameter("@SalerId", searchModel.SalerId);
                }
                else
                {
                    objParam[10] = new SqlParameter("@SalerId", DBNull.Value);

                }
                if (searchModel.OperatorId > 0)
                {
                    objParam[11] = new SqlParameter("@OperatorId", searchModel.OperatorId);
                }
                else
                {
                    objParam[11] = new SqlParameter("@OperatorId", DBNull.Value);

                }
                objParam[12] = new SqlParameter("@PageIndex", currentPage);
                objParam[13] = new SqlParameter("@PageSize", pageSize);
                objParam[14] = new SqlParameter("@SalerPermission", searchModel.SalerPermission);
                objParam[15] = new SqlParameter("@BookingCode", searchModel.BookingCode);

                string procedure = StoreProcedureConstant.GetListFlyBooking;

                return _DbWorker.GetDataTable(procedure, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetPagingList - FlyBookingDetailDAL: " + ex);
            }
            return null;
        }
        public async Task<long> UpdateOperatorOrderPrice(FlyOperatorOrderPriceSummitSQLModel model)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    foreach (var fly in model.detail)
                    {
                        var id = UpdateFlyBookingDetail(fly);
                        if (id <= 0) return -1;
                    }
                    foreach (var extra in model.extra)
                    {
                        _DbContext.FlyBookingExtraPackages.Update(extra);
                        await _DbContext.SaveChangesAsync();
                    }
                    return model.detail[0].Id;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateOperatorOrderPrice - FlyBookingDetailDAL: " + ex);
                return -2;
            }
        }
        public async Task<long> UpdateServiceStatus(int status, string group_booking_id, int user_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var exists = await _DbContext.FlyBookingDetail.AsNoTracking().Where(s => s.GroupBookingId.Trim() == group_booking_id.Trim()).ToListAsync();
                    if (exists != null && exists.Count > 0)
                    {
                        foreach (var item in exists)
                        {
                            item.StatusOld = item.Status;
                            item.UpdatedDate = DateTime.Now;
                            item.UpdatedBy = user_id;
                            item.Status = status;
                            _DbContext.FlyBookingDetail.Update(item);
                            await _DbContext.SaveChangesAsync();
                        }
                    }
                    return 1;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetListByGroupFlyID - FlyBookingDetailDAL: " + ex);
                return -2;
            }
        }
        public async Task<long> UpdateServiceOperator(string group_booking_id, int user_id, int user_commit)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var exists = await _DbContext.FlyBookingDetail.AsNoTracking().Where(s => s.GroupBookingId.Trim() == group_booking_id.Trim()).ToListAsync();
                    if (exists != null && exists.Count > 0)
                    {
                        foreach (var item in exists)
                        {
                            item.UpdatedDate = DateTime.Now;
                            item.UpdatedBy = user_commit;
                            item.SalerId = user_id;
                            _DbContext.FlyBookingDetail.Update(item);
                            await _DbContext.SaveChangesAsync();
                        }
                    }
                    return 1;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateServiceOperator - FlyBookingDetailDAL: " + ex);
                return -2;
            }
        }
        public async Task<long> UpdateBookingUnitPriceByGroupBookingId(string group_booking_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var list = await _DbContext.FlyBookingDetail.AsNoTracking().Where(x => x.GroupBookingId == group_booking_id).ToListAsync();
                    if (list != null && list.Count > 0)
                    {
                        double unit_price = 0;
                        unit_price += list.Sum(x => x.Price == null ? 0 : (double)x.Price);
                        var extra = await _DbContext.FlyBookingExtraPackages.AsNoTracking().Where(x => x.GroupFlyBookingId == group_booking_id).ToListAsync();
                        if (extra != null && extra.Count > 0)
                        {
                            unit_price += extra.Sum(x => x.Price == null ? 0 : (double)x.Price);
                        }
                        foreach (var detail in list)
                        {
                            detail.Price = Math.Round(unit_price / (double)list.Count, 0);
                            _DbContext.FlyBookingDetail.Update(detail);
                            await _DbContext.SaveChangesAsync();
                        }

                    }
                    return 1;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateBookingUnitPriceById - FlyBookingDetailDAL: " + ex);
                return -1;
            }
        }
        public List<FlyBookingDetail> GetByServiceCodes(List<string> serviceCodes)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.FlyBookingDetail.AsNoTracking().Where(s => serviceCodes.Contains(s.ServiceCode)).ToList();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetListByOrderId - FlyBookingDetailDAL: " + ex);
                return null;
            }
        }
        public async Task<long> RemoveNonExistsFlyBookingOpional(long booking_id, List<long> remain_optional_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {

                    var del_bookings = await _DbContext.FlyBookingPackagesOptional.AsNoTracking().Where(x => x.BookingId == booking_id && !remain_optional_id.Contains(x.Id)).ToListAsync();
                    if (del_bookings != null && del_bookings.Count > 0)
                    {
                        foreach (var del_item in del_bookings)
                        {
                            del_item.BookingId = -1 * del_item.BookingId;
                            del_item.SuplierId = -1 * del_item.SuplierId;
                            _DbContext.FlyBookingPackagesOptional.Update(del_item);
                            await _DbContext.SaveChangesAsync();
                        }

                    }
                }
                return 0;

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("RemoveNonExistsFlyBookingOpional - FlyBookingDetailDAL. " + ex);
                return -1;
            }
        }

        public async Task<DataTable> GetFlyBookingRoomsOptionalByBookingId(long hotelbookingid)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@BookingId", hotelbookingid);
                return _DbWorker.GetDataTable(StoreProcedureConstant.SP_GetListFlyBookingOptionalByBookingId, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetFlyBookingRoomsOptionalByBookingId - ContractDAL: " + ex.ToString());
            }
            return null;
        }
    }
}
