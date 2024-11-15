using DAL.Generic;
using DAL.StoreProcedure;
using Entities.Models;
using Entities.ViewModels.OrderManual;
using Entities.ViewModels.SetServices;
using Entities.ViewModels.VinWonder;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace DAL
{

    public class VinWonderBookingDAL : GenericService<VinWonderBooking>
    {
        private DbWorker dbWorker;

        public VinWonderBookingDAL(string connection) : base(connection)
        {
            dbWorker = new DbWorker(connection);

        }
        public VinWonderBooking GetVinWonderBookingById(long booking_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.VinWonderBooking.FirstOrDefault(x => x.Id == booking_id);
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetVinWonderBookingById - VinWonderBookingDAL: " + ex);
                return null;
            }
        }
        public async Task<long> SummitData(OrderManualVinWonderBookingServiceSummitSQLModel model)
        {
            try
            {

                using (var _DbContext = new EntityDataContext(_connection))
                {
                    List<long> vinwonder_guest_new_id = new List<long>();
                    List<long> vinwonder_packages_new_id = new List<long>();
                    var order = await _DbContext.Order.AsNoTracking().FirstOrDefaultAsync(x => x.OrderId == model.detail.OrderId);
                    if (order == null || order.OrderId <= 0)
                    {
                        return -1;
                    }

                    var exists_vinwonder = await _DbContext.VinWonderBooking.AsNoTracking().FirstOrDefaultAsync(x => x.Id == model.detail.Id);
                    if (exists_vinwonder != null && exists_vinwonder.Id > 0)
                    {

                        exists_vinwonder.Amount = model.detail.Amount;
                        exists_vinwonder.TotalPrice = model.detail.TotalPrice;
                        exists_vinwonder.TotalProfit = model.detail.TotalProfit;
                        exists_vinwonder.TotalUnitPrice = model.detail.TotalUnitPrice;
                        exists_vinwonder.Commission = model.detail.Commission;
                        exists_vinwonder.OthersAmount = model.detail.OthersAmount;
                        exists_vinwonder.SiteCode = model.detail.SiteCode;
                        exists_vinwonder.SiteName = model.detail.SiteName;
                        exists_vinwonder.Note = model.detail.Note;
                        exists_vinwonder.SalerId = model.detail.SalerId;
                        exists_vinwonder.UpdatedBy = model.detail.UpdatedBy;
                        exists_vinwonder.UpdatedDate = model.detail.UpdatedDate;
                        if(exists_vinwonder.ServiceCode==null || exists_vinwonder.ServiceCode.Trim() == "")
                        {
                            exists_vinwonder.ServiceCode = "VINWONDER" + string.Format(String.Format("{0,4:0000}", exists_vinwonder.Id));
                        }
                        UpdateVinWonderBooking(exists_vinwonder);
                        model.detail = exists_vinwonder;
                        List<long> new_extra_package_id = new List<long>();
                        foreach (var package in model.packages)
                        {
                            package.BookingId = model.detail.Id;
                            var exists_vinwonder_package = await _DbContext.VinWonderBookingTicket.AsNoTracking().FirstOrDefaultAsync(x => x.Id == package.Id);
                            if (exists_vinwonder_package != null && exists_vinwonder_package.Id > 0)
                            {
                                package.CreatedBy = exists_vinwonder_package.CreatedBy;
                                package.CreatedDate = exists_vinwonder_package.CreatedDate;
                                UpdateVinWonderPackages(package);
                            }
                            else
                            {
                                CreateVinWonderPackages(package);
                            }
                            vinwonder_packages_new_id.Add(package.Id);
                        }
                        if (model.guests != null && model.guests.Count > 0)
                        {
                            foreach (var guest in model.guests)
                            {
                                guest.BookingId = model.detail.Id;
                                var exists_vinwonder_guest = await _DbContext.VinWonderBookingTicketCustomer.AsNoTracking().FirstOrDefaultAsync(x => x.Id == guest.Id);
                                if (exists_vinwonder_guest != null && exists_vinwonder_guest.Id > 0)
                                {
                                    guest.CreatedBy = exists_vinwonder_guest.CreatedBy;
                                    guest.CreatedDate = exists_vinwonder_guest.CreatedDate;
                                   
                                    UpdateVinWonderGuest(guest);
                                }
                                else
                                {
                                    CreateVinWonderGuest(guest);
                                }
                                vinwonder_guest_new_id.Add(guest.Id);
                            }
                        }
                        await DeleteNonExistsVinWonderData(vinwonder_guest_new_id, vinwonder_packages_new_id, model.detail.Id, model.detail.UpdatedBy);

                    }
                    else
                    {
                        CreateVinWonderBooking(model.detail);
                        model.detail.ServiceCode = "VINWONDER" + string.Format(String.Format("{0,4:0000}", model.detail.Id));

                        _DbContext.VinWonderBooking.Update(model.detail);
                        await _DbContext.SaveChangesAsync();
                        foreach (var package in model.packages)
                        {
                            package.BookingId = model.detail.Id;
                            CreateVinWonderPackages(package);
                            vinwonder_packages_new_id.Add(package.Id);
                        }
                        if (model.guests != null && model.guests.Count > 0)
                        {
                            foreach (var guest in model.guests)
                            {
                                guest.BookingId = model.detail.Id;
                                CreateVinWonderGuest(guest);
                                vinwonder_guest_new_id.Add(guest.Id);
                            }
                        }

                    }


                    return model.detail.Id;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SummitData - VinWonderBookingDAL: " + ex);
                return -2;
            }
        }
        private async Task<bool> DeleteNonExistsVinWonderData(List<long> remain_guests, List<long> remain_packages, long vinwonder_id, int? updated_user)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var delete_guest = await _DbContext.VinWonderBookingTicketCustomer.AsNoTracking().Where(x => x.BookingId == vinwonder_id && !remain_guests.Contains(x.Id)).ToListAsync();
                    if (delete_guest != null && delete_guest.Count > 0)
                    {
                        foreach (var guest in delete_guest)
                        {
                            guest.BookingId = guest.BookingId * -1;
                            guest.UpdatedBy = updated_user;
                            UpdateVinWonderGuest(guest);
                        }
                    }
                    var delete_packages = await _DbContext.VinWonderBookingTicket.AsNoTracking().Where(x => x.BookingId == vinwonder_id && !remain_packages.Contains(x.Id)).ToListAsync();
                    if (delete_packages != null && delete_packages.Count > 0)
                    {
                        foreach (var package in delete_packages)
                        {
                            package.BookingId = package.BookingId * -1;
                            package.UpdatedBy = updated_user;
                            if (package.UnitPrice == null) package.UnitPrice = 0;
                            UpdateVinWonderPackages(package);
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("DeleteNonExistsVinWonderData - VinWonderBookingDAL. " + ex);
                return false;
            }
        }
        public async Task<long> DeleteVinWonderBookingByID(long id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var booking = await _DbContext.VinWonderBooking.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                    if (booking != null && booking.Id > 0)
                    {
                        
                        var extra = await _DbContext.VinWonderBookingTicket.AsNoTracking().Where(x => x.BookingId == booking.Id).ToListAsync();
                        var ticket_id = extra.Select(x => x.Id);
                        if (extra != null && extra.Count > 0)
                        {
                            _DbContext.VinWonderBookingTicket.RemoveRange(extra);
                            await _DbContext.SaveChangesAsync();
                        }
                        
                        if(ticket_id != null && ticket_id.Count() > 0)
                        {
                            var ticket_ids = ticket_id.ToList();
                            var ticket_details = await _DbContext.VinWonderBookingTicketDetail.AsNoTracking().Where(x => x.BookingTicketId!=null?  ticket_ids.Contains((long)x.BookingTicketId) : x.Id<=0).ToListAsync();
                            if (extra != null && extra.Count > 0)
                            {
                                _DbContext.VinWonderBookingTicketDetail.RemoveRange(ticket_details);
                                await _DbContext.SaveChangesAsync();
                            }
                        }
                        var passengers = await _DbContext.VinWonderBookingTicketCustomer.AsNoTracking().Where(x => x.BookingId == booking.Id).ToListAsync();
                        if (passengers != null && passengers.Count > 0)
                        {
                            _DbContext.VinWonderBookingTicketCustomer.RemoveRange(passengers);
                            await _DbContext.SaveChangesAsync();
                        }
                        _DbContext.VinWonderBooking.Remove(booking);
                        await _DbContext.SaveChangesAsync();
                    }
                    return 1;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("DeleteTourByID - TourDAL. " + ex);
                return -1;
            }
        }
        public async Task<long> CancelVinWonderByID(long id, int user_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var booking = await _DbContext.VinWonderBooking.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                    if (booking != null && booking.Id > 0)
                    {
                        if (booking.Status == (int)ServiceStatus.Decline)
                        {
                            booking.Status = (int)ServiceStatus.Cancel;
                            booking.UpdatedBy = user_id;
                            booking.UpdatedDate = DateTime.Now;
                            _DbContext.VinWonderBooking.Update(booking);
                            await _DbContext.SaveChangesAsync();
                        }
                    }
                    return 1;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("DeleteTourByID - TourDAL. " + ex);
                return -1;
            }
        }
        private int CreateVinWonderBooking(VinWonderBooking model)
        {
            try
            {
                SqlParameter[] objParam_order = new SqlParameter[17];
                objParam_order[0] = new SqlParameter("@SiteName", model.SiteName);
                objParam_order[1] = new SqlParameter("@SiteCode", model.SiteCode);
                objParam_order[2] = new SqlParameter("@Status", model.Status);
                objParam_order[3] = new SqlParameter("@Amount", model.Amount);
                objParam_order[4] = new SqlParameter("@TotalPrice", model.TotalPrice);
                objParam_order[5] = new SqlParameter("@TotalProfit", model.TotalProfit);
                objParam_order[6] = new SqlParameter("@SupplierId", model.SupplierId);
                objParam_order[7] = new SqlParameter("@OrderId", model.OrderId);
                objParam_order[8] = new SqlParameter("@SalerId", model.SalerId);
                objParam_order[9] = new SqlParameter("@Note", model.Note);
                objParam_order[10] = new SqlParameter("@TotalUnitPrice", model.TotalUnitPrice);
                objParam_order[11] = new SqlParameter("@AdavigoBookingId", model.AdavigoBookingId);
                if (model.ServiceCode != null)
                {
                    objParam_order[12] = new SqlParameter("@ServiceCode", model.ServiceCode);
                }
                else
                {
                    objParam_order[12] = new SqlParameter("@ServiceCode", DBNull.Value);
                }
                objParam_order[13] = new SqlParameter("@CreatedBy", model.CreatedBy);
                objParam_order[14] = new SqlParameter("@CreatedDate", model.CreatedDate);
                objParam_order[15] = new SqlParameter("@Commission", model.Commission);
                objParam_order[16] = new SqlParameter("@OthersAmount", model.OthersAmount);
                var id = dbWorker.ExecuteNonQuery(StoreProcedureConstant.InsertVinWonderBooking, objParam_order);
                model.Id = id;
                return id;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CreateVinWonder - VinWonderBookingDAL. " + ex);
                return -1;
            }
        }
        private int UpdateVinWonderBooking(VinWonderBooking model)
        {
            try
            {
                SqlParameter[] objParam_order = new SqlParameter[17];
                objParam_order[0] = new SqlParameter("@SiteName", model.SiteName);
                objParam_order[1] = new SqlParameter("@SiteCode", model.SiteCode);
                objParam_order[2] = new SqlParameter("@Status", model.Status);
                objParam_order[3] = new SqlParameter("@Amount", model.Amount);
                objParam_order[4] = new SqlParameter("@TotalPrice", model.TotalPrice);
                objParam_order[5] = new SqlParameter("@TotalProfit", model.TotalProfit);
                objParam_order[6] = new SqlParameter("@SupplierId", model.SupplierId);
                objParam_order[7] = new SqlParameter("@OrderId", model.OrderId);
                objParam_order[8] = new SqlParameter("@SalerId", model.SalerId);
                objParam_order[9] = new SqlParameter("@Note", model.Note);
                objParam_order[10] = new SqlParameter("@TotalUnitPrice", model.TotalUnitPrice);
                objParam_order[11] = new SqlParameter("@AdavigoBookingId", model.AdavigoBookingId);
                if (model.ServiceCode != null)
                {
                    objParam_order[12] = new SqlParameter("@ServiceCode", model.ServiceCode);
                }
                else
                {
                    objParam_order[12] = new SqlParameter("@ServiceCode", DBNull.Value);
                }
                objParam_order[13] = new SqlParameter("@UpdatedBy", model.UpdatedBy);
                objParam_order[14] = new SqlParameter("@Id", model.Id);
                objParam_order[15] = new SqlParameter("@Commission", model.Commission);
                objParam_order[16] = new SqlParameter("@OthersAmount", model.OthersAmount);
                var id = dbWorker.ExecuteNonQuery(StoreProcedureConstant.UpdateVinWonderBooking, objParam_order);
                model.Id = id;
                return id;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CreateVinWonder - VinWonderBookingDAL. " + ex);
                return -1;
            }
        }
        private int CreateVinWonderGuest(VinWonderBookingTicketCustomer model)
        {
            try
            {

                SqlParameter[] objParam_order = new SqlParameter[9];
                objParam_order[0] = new SqlParameter("@BookingId", model.BookingId);
                objParam_order[1] = new SqlParameter("@FullName", model.FullName);
                if (model.Birthday != null)
                {
                    objParam_order[2] = new SqlParameter("@Birthday", model.Birthday);
                }
                else
                {
                    objParam_order[2] = new SqlParameter("@Birthday", DBNull.Value);

                }
                if (model.Phone != null && model.Phone.Trim() != "")
                {
                    objParam_order[3] = new SqlParameter("@Phone", model.Phone);
                }
                else
                {
                    objParam_order[3] = new SqlParameter("@Phone", DBNull.Value);

                }

                if (model.Note != null)
                {
                    objParam_order[4] = new SqlParameter("@Note", model.Note);
                }
                else
                {
                    objParam_order[4] = new SqlParameter("@Note", DBNull.Value);

                }


                objParam_order[5] = new SqlParameter("@CreatedBy", model.CreatedBy);
                objParam_order[6] = new SqlParameter("@CreatedDate", model.CreatedDate);
                if (model.Genre != null)
                {
                    objParam_order[7] = new SqlParameter("@Genre", model.Genre);
                }
                else
                {
                    objParam_order[7] = new SqlParameter("@Genre", DBNull.Value);

                }
                objParam_order[8] = new SqlParameter("@Email", model.Email);
             
                var id = dbWorker.ExecuteNonQuery(StoreProcedureConstant.InsertVinWonderBookingTicketCustomer, objParam_order);
                model.Id = id;
                return id;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CreateVinWonderGuest - VinWonderBookingDAL. " + ex.ToString());
                return -1;
            }
        }
        private int UpdateVinWonderGuest(VinWonderBookingTicketCustomer model)
        {
            try
            {

                SqlParameter[] objParam_order = new SqlParameter[10];
                objParam_order[0] = new SqlParameter("@BookingId", model.BookingId);
                objParam_order[1] = new SqlParameter("@FullName", model.FullName);
                if (model.Birthday != null)
                {
                    objParam_order[2] = new SqlParameter("@Birthday", model.Birthday);
                }
                else
                {
                    objParam_order[2] = new SqlParameter("@Birthday", DBNull.Value);

                }
                if (model.Phone != null && model.Phone.Trim() != "")
                {
                    objParam_order[3] = new SqlParameter("@Phone", model.Phone);
                }
                else
                {
                    objParam_order[3] = new SqlParameter("@Phone", DBNull.Value);

                }

                if (model.Note != null)
                {
                    objParam_order[4] = new SqlParameter("@Note", model.Note);
                }
                else
                {
                    objParam_order[4] = new SqlParameter("@Note", DBNull.Value);

                }
                objParam_order[5] = new SqlParameter("@UpdatedBy", model.UpdatedBy);


                objParam_order[6] = new SqlParameter("@Id", model.Id);
                if (model.Genre != null)
                {
                    objParam_order[7] = new SqlParameter("@Genre", model.Genre);
                }
                else
                {
                    objParam_order[7] = new SqlParameter("@Genre", DBNull.Value);

                }
                objParam_order[8] = new SqlParameter("@Email", model.Email);
                objParam_order[9] = new SqlParameter("@OtherDetail", model.OtherDetail);


                var id = dbWorker.ExecuteNonQuery(StoreProcedureConstant.UpdateVinWonderBookingTicketCustomer, objParam_order);
                model.Id = id;
                return id;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateVinWonderGuest - VinWonderBookingDAL. " + ex.ToString());
                return -1;
            }
        }
        private int CreateVinWonderPackages(VinWonderBookingTicket model)
        {
            try
            {

                SqlParameter[] objParam_order = new SqlParameter[15];
                objParam_order[0] = new SqlParameter("@BookingId", model.BookingId);
                objParam_order[1] = new SqlParameter("@RateCode", model.RateCode);

                objParam_order[2] = new SqlParameter("@Name", model.Name);
                objParam_order[3] = new SqlParameter("@Quantity", model.Quantity);
                objParam_order[4] = new SqlParameter("@Amount", model.Amount);
                objParam_order[5] = new SqlParameter("@BasePrice", model.BasePrice);
                objParam_order[6] = new SqlParameter("@Profit", model.Profit);

                objParam_order[7] = new SqlParameter("@DateUsed", model.DateUsed);

                objParam_order[8] = new SqlParameter("@adt", model.Adt);
                objParam_order[9] = new SqlParameter("@child", model.Child);
                objParam_order[10] = new SqlParameter("@old", model.Old);
                objParam_order[11] = new SqlParameter("@totalPrice", model.TotalPrice);
                objParam_order[12] = new SqlParameter("@UnitPrice", model.UnitPrice);
                objParam_order[13] = new SqlParameter("@CreatedBy", model.CreatedBy);
                objParam_order[14] = new SqlParameter("@CreatedDate", model.CreatedDate);


                var id = dbWorker.ExecuteNonQuery(StoreProcedureConstant.InsertVinWonderBookingTicket, objParam_order);
                model.Id = id;
                return id;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CreateVinWonderPackages - VinWonderBookingDAL. " + ex);
                return -1;
            }
        }
        private int UpdateVinWonderPackages(VinWonderBookingTicket model)
        {
            try
            {

                SqlParameter[] objParam_order = new SqlParameter[15];
                objParam_order[0] = new SqlParameter("@BookingId", model.BookingId);
                objParam_order[1] = new SqlParameter("@RateCode", model.RateCode);

                objParam_order[2] = new SqlParameter("@Name", model.Name);
                objParam_order[3] = new SqlParameter("@Quantity", model.Quantity);
                objParam_order[4] = new SqlParameter("@Amount", model.Amount);
                objParam_order[5] = new SqlParameter("@BasePrice", model.BasePrice);
                objParam_order[6] = new SqlParameter("@Profit", model.Profit);

                objParam_order[7] = new SqlParameter("@DateUsed", model.DateUsed);

                objParam_order[8] = new SqlParameter("@adt", model.Adt);
                objParam_order[9] = new SqlParameter("@child", model.Child);
                objParam_order[10] = new SqlParameter("@old", model.Old);
                objParam_order[11] = new SqlParameter("@totalPrice", model.TotalPrice);
                objParam_order[12] = new SqlParameter("@UnitPrice", model.UnitPrice);
                objParam_order[13] = new SqlParameter("@UpdatedBy", model.UpdatedBy);
                objParam_order[14] = new SqlParameter("@Id", model.Id);

                var id = dbWorker.ExecuteNonQuery(StoreProcedureConstant.UpdateVinWonderBookingTicket, objParam_order);
                model.Id = id;
                return id;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateVinWonderPackages - VinWonderBookingDAL. " + ex);
                return -1;
            }
        }
        public async Task<DataTable> GetDetailVinWonderByBookingId(long BookingId)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@BookingId", BookingId);
                return dbWorker.GetDataTable(StoreProcedureConstant.sp_GetDetailVinwonder, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetDetailVinWonderByBookingId - HotelBookingDAL: " + ex);
            }
            return null;
        }
        public DataTable GetPagingList(SearchFlyBookingViewModel searchModel, int currentPage, int pageSize)
        {
            try
            {

                SqlParameter[] objParam = new SqlParameter[14];
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

                if (searchModel.UserCreate != null)
                {
                    objParam[5] = new SqlParameter("@UserCreate", searchModel.UserCreate);
                }
                else
                {
                    objParam[5] = new SqlParameter("@UserCreate", DBNull.Value);

                }
                if (searchModel.CreateDateFrom != null)
                {
                    objParam[6] = new SqlParameter("@CreateDateFrom", ((DateTime)searchModel.CreateDateFrom).Date);
                }
                else
                {
                    objParam[6] = new SqlParameter("@CreateDateFrom", DBNull.Value);

                }
                if (searchModel.CreateDateTo != null)
                {
                    objParam[7] = new SqlParameter("@CreateDateTo", ((DateTime)searchModel.CreateDateTo).Date);
                }
                else
                {
                    objParam[7] = new SqlParameter("@CreateDateTo", DBNull.Value);

                }
                if (searchModel.SalerId > 0)
                {
                    objParam[8] = new SqlParameter("@SalerId", searchModel.SalerId);
                }
                else
                {
                    objParam[8] = new SqlParameter("@SalerId", DBNull.Value);

                }
                if (searchModel.OperatorId > 0)
                {
                    objParam[9] = new SqlParameter("@OperatorId", searchModel.OperatorId);
                }
                else
                {
                    objParam[9] = new SqlParameter("@OperatorId", DBNull.Value);

                }
                objParam[10] = new SqlParameter("@PageIndex", currentPage);
                objParam[11] = new SqlParameter("@PageSize", pageSize);
                if (searchModel.SalerPermission != null)
                {
                    objParam[12] = new SqlParameter("@SalerPermission", searchModel.SalerPermission);
                }
                else
                {
                    objParam[12] = new SqlParameter("@SalerPermission", DBNull.Value);

                }
                if (searchModel.BookingCode != null)
                {
                    objParam[13] = new SqlParameter("@BookingCode", searchModel.BookingCode);
                }
                else
                {
                    objParam[13] = new SqlParameter("@BookingCode", DBNull.Value);

                }

                string procedure = StoreProcedureConstant.GetListVinWonderBooking;

                return dbWorker.GetDataTable(procedure, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetPagingList - VinWonderBookingDAL: " + ex);
            }
            return null;
        }
        public async Task<long> UpdateServiceOperator(long booking_id, int user_id, int user_commit)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var exists = await _DbContext.VinWonderBooking.AsNoTracking().FirstOrDefaultAsync(s => s.Id == booking_id);
                    if (exists != null && exists.Id > 0)
                    {
                        exists.UpdatedDate = DateTime.Now;
                        exists.UpdatedBy = user_commit;
                        exists.SalerId = user_id;
                        _DbContext.VinWonderBooking.Update(exists);
                        await _DbContext.SaveChangesAsync();
                    }
                    return 1;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateServiceOperator - VinWonderBookingDAL: " + ex);
                return -2;
            }
        }
        public async Task<long> UpdateServiceOperator(long booking_id, int user_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var exists = await _DbContext.VinWonderBooking.AsNoTracking().Where(s => s.Id == booking_id).FirstOrDefaultAsync();
                    if (exists != null && exists.Id > 0 && exists.Status == (int)ServiceStatus.WaitingExcution)
                    {
                        exists.UpdatedDate = DateTime.Now;
                        exists.UpdatedBy = user_id;
                        exists.SalerId = user_id;
                        exists.Status = (int)ServiceStatus.OnExcution;
                        _DbContext.VinWonderBooking.Update(exists);
                        await _DbContext.SaveChangesAsync();
                    }
                    return 1;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateServiceOperator - VinWonderBookingDAL: " + ex);
                return -2;
            }
        }
        public async Task<long> UpdateServiceStatus(int status, long booking_id, int user_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var exists = await _DbContext.VinWonderBooking.AsNoTracking().Where(s => s.Id == booking_id).FirstOrDefaultAsync();
                    if (exists != null && exists.Id > 0)
                    {
                        exists.StatusOld = exists.Status;
                        exists.UpdatedDate = DateTime.Now;
                        exists.UpdatedBy = user_id;
                        exists.Status = status;
                        _DbContext.VinWonderBooking.Update(exists);
                        await _DbContext.SaveChangesAsync();
                    }
                    return 1;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateServiceStatus - OtherBookingDAL: " + ex);
                return -2;
            }
        }
        public VinWonderBooking GetVinWonderByServiceCode(string service_code)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.VinWonderBooking.FirstOrDefault(x => x.ServiceCode.ToUpper().Contains(service_code.ToUpper()));
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetVinWonderBookingById - VinWonderBookingDAL: " + ex);
                return null;
            }
        }
        public async Task<List<ListVinWonderemialViewModel>> GetVinWonderBookingEmailByOrderID(long orderid)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@OrderID", orderid);

                var dt = dbWorker.GetDataTable(StoreProcedureConstant.SP_GetVinWonderBookingEmailByOrderID, objParam);
                if (dt != null && dt.Rows.Count > 0)
                {
                    var listData = dt.ToList<ListVinWonderemialViewModel>();
                    return listData;
                }
                return null;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateHotelDetail - HotelDAL. " + ex);
                return null;
            }
        }
        public async Task<List<VinWonderBooking>> GetVinWonderBookingByOrderId(long orderid)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@OrderID", orderid);

                var dt = dbWorker.GetDataTable(StoreProcedureConstant.SP_GetVinWonderBookingByOrderID, objParam);
                if (dt != null && dt.Rows.Count > 0)
                {
                    var listData = dt.ToList<VinWonderBooking>();
                    return listData;
                }
                return null;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetVinWonderBookingByOrderID - HotelDAL. " + ex);
                return null;
            }
        }
        public async Task<List<VinWonderBookingTicket>> GetVinWonderBookingTicketByBookingID(long BookingId)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@BookingId", BookingId);

                var dt = dbWorker.GetDataTable(StoreProcedureConstant.SP_GetVinWonderBookingTicketByBookingID, objParam);
                if (dt != null && dt.Rows.Count > 0)
                {
                    var listData = dt.ToList<VinWonderBookingTicket>();
                    return listData;
                }
                return null;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetVinWonderBookingByOrderID - HotelDAL. " + ex);
                return null;
            }
        }
        public async Task<List<VinWonderBookingTicketCustomer>> GetVinWondeCustomerByBookingId(long BookingId)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@BookingId", BookingId);

                var dt = dbWorker.GetDataTable(StoreProcedureConstant.SP_GetVinWonderBookingCustomerByBookingId, objParam);
                if (dt != null && dt.Rows.Count > 0)
                {
                    var listData = dt.ToList<VinWonderBookingTicketCustomer>();
                    return listData;
                }
                return null;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetVinWonderBookingByOrderID - HotelDAL. " + ex);
                return null;
            }
        }
    }
}
