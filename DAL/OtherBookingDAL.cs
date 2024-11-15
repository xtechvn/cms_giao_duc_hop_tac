using DAL.Generic;
using DAL.StoreProcedure;
using Entities.Models;
using Entities.ViewModels.OrderManual;
using Entities.ViewModels.SetServices;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
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

    public class OtherBookingDAL : GenericService<OtherBooking>
    {
        private DbWorker dbWorker;

        public OtherBookingDAL(string connection) : base(connection)
        {
            dbWorker = new DbWorker(connection);

        }
        public OtherBooking GetOtherBookingById(long booking_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.OtherBooking.FirstOrDefault(x => x.Id == booking_id);
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetOtherBookingById - OtherBookingDAL: " + ex);
                return null;
            }
        }
        public async Task<long> CreateOrUpdateOtherBooking(OrderManualOtherBookingServiceSummitSQLModel model)
        {
            try
            {
                if (model.booking == null)
                {
                    return 0;
                }
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    if (model.booking.Id <= 0)
                    {
                        _DbContext.OtherBooking.Add(model.booking);
                        await _DbContext.SaveChangesAsync();
                        if (model.packages != null && model.packages.Count > 0)
                        {
                            foreach (var item in model.packages)
                            {
                                var item_summit = new OtherBookingPackages()
                                {
                                    Amount = item.Amount,
                                    BasePrice = item.BasePrice,
                                    BookingId = model.booking.Id,
                                    Name = item.Name,
                                    Profit = item.Profit,
                                    Quantity = item.Quantity,
                                    UpdatedBy = item.UpdatedBy,
                                    UpdatedDate = item.UpdatedDate,
                                    SalePrice=item.SalePrice,
                                    Note=item.Note,
                                    ServiceType=item.ServiceType
                                    
                                };
                                _DbContext.OtherBookingPackages.Add(item_summit);
                                await _DbContext.SaveChangesAsync();
                            }
                        }
                    }
                    else
                    {
                        List<long> keep_ids = new List<long>();
                        var exists_booking = _DbContext.OtherBooking.FirstOrDefault(x => x.Id == model.booking.Id);
                        if (exists_booking != null && exists_booking.Id > 0)
                        {
                            exists_booking.Amount = model.booking.Amount;
                            exists_booking.ServiceType = model.booking.ServiceType;
                            exists_booking.Profit = model.booking.Profit;
                            exists_booking.Price = model.booking.Price;
                            exists_booking.StartDate = model.booking.StartDate;
                            exists_booking.EndDate = model.booking.EndDate;
                            exists_booking.OperatorId = model.booking.OperatorId;
                            exists_booking.UpdatedBy = model.booking.UpdatedBy;
                            exists_booking.UpdatedDate = model.booking.UpdatedDate;
                            exists_booking.Note = model.booking.Note;
                            exists_booking.ServiceCode = model.booking.ServiceCode;
                            exists_booking.Commission = model.booking.Commission;
                            exists_booking.OthersAmount = model.booking.OthersAmount;
                            exists_booking.RoomNo = model.booking.RoomNo;
                            exists_booking.SerialNo = model.booking.SerialNo;
                            exists_booking.ConfNo = model.booking.ConfNo;
                            _DbContext.OtherBooking.Update(exists_booking);
                            await _DbContext.SaveChangesAsync();
                            model.booking.Id = exists_booking.Id;
                            if (model.packages != null && model.packages.Count > 0)
                            {
                                foreach (var item in model.packages)
                                {
                                    if (item.Id <= 0)
                                    {
                                        item.BookingId = exists_booking.Id;
                                        _DbContext.OtherBookingPackages.Add(item);
                                        await _DbContext.SaveChangesAsync();
                                        keep_ids.Add(item.Id);
                                    }
                                    else
                                    {
                                        var exists_package = _DbContext.OtherBookingPackages.FirstOrDefault(x => x.Id == item.Id && x.BookingId == item.BookingId);
                                        if (exists_package != null && exists_package.Id > 0)
                                        {
                                            exists_package.Amount = item.Amount;
                                            exists_package.BasePrice = item.BasePrice;
                                            exists_package.BookingId = exists_booking.Id;
                                            exists_package.Name = item.Name;
                                            exists_package.Profit = item.Profit;
                                            exists_package.Quantity = item.Quantity;
                                            exists_package.UpdatedBy = item.UpdatedBy;
                                            exists_package.UpdatedDate = item.UpdatedDate;
                                            exists_package.SalePrice = item.SalePrice;
                                            exists_package.Note = item.Note;
                                            exists_package.ServiceType = item.ServiceType;

                                            _DbContext.OtherBookingPackages.Update(exists_package);
                                            await _DbContext.SaveChangesAsync();
                                            keep_ids.Add(exists_package.Id);

                                        }

                                    }

                                }
                            }
                        }
                        var ids = await RemoveNonExistsOtherBookingPackages(model.booking.Id, keep_ids);

                    }
                    return model.booking.Id;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CreateOrUpdateOtherBooking - OtherBookingDAL: " + ex);
            }
            return -1;
        }
        public DataTable GetPagingList(SearchFlyBookingViewModel searchModel, int currentPage, int pageSize)
        {
            try
            {

                SqlParameter[] objParam = new SqlParameter[17];
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
                if (searchModel.SalerPermission != null)
                {
                    objParam[14] = new SqlParameter("@SalerPermission", searchModel.SalerPermission);
                }
                else
                {
                    objParam[14] = new SqlParameter("@SalerPermission", DBNull.Value);

                }
                if (searchModel.BookingCode != null)
                {
                    objParam[15] = new SqlParameter("@BookingCode", searchModel.BookingCode);
                }
                else
                {
                    objParam[15] = new SqlParameter("@BookingCode", DBNull.Value);

                }
                if (searchModel.ServiceType != null && searchModel.ServiceType.Count>0)
                {
                    objParam[16] = new SqlParameter("@ServiceType", string.Join(",", searchModel.ServiceType));
                }
                else
                {
                    objParam[16] = new SqlParameter("@ServiceType", DBNull.Value);

                }
                string procedure = StoreProcedureConstant.GetListOtherBooking;

                return dbWorker.GetDataTable(procedure, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetPagingList - OtherBookingDAL: " + ex);
            }
            return null;
        }
        public async Task<long> DeleteOtherBookingById(long id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var other = await _DbContext.OtherBooking.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                    if (other != null && other.Id > 0)
                    {
                        var other_packages = await _DbContext.OtherBookingPackages.AsNoTracking().Where(x => x.BookingId == other.Id).ToListAsync();
                        if (other_packages != null && other_packages.Count > 0)
                        {
                            _DbContext.OtherBookingPackages.RemoveRange(other_packages);
                            await _DbContext.SaveChangesAsync();

                        }

                        _DbContext.OtherBooking.Remove(other);
                        await _DbContext.SaveChangesAsync();
                    }
                    return 1;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("DeleteOtherBookingById - OtherBookingDAL. " + ex);
                return -1;
            }
        }
        public async Task<long> CancelOtherBookingById(long id, int user_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var other = await _DbContext.OtherBooking.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                    if (other != null && other.Id > 0)
                    {
                        if (other.Status == (int)ServiceStatus.Decline)
                        {
                            other.Status = (int)ServiceStatus.Cancel;
                            other.UpdatedBy = user_id;
                            other.UpdatedDate = DateTime.Now;
                            _DbContext.OtherBooking.Update(other);
                            await _DbContext.SaveChangesAsync();
                        }
                    }
                    return 1;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("DeleteTourByID - OtherBookingDAL. " + ex);
                return -1;
            }
        }
        public async Task<long> UpdateServiceOperator(long booking_id, int user_id, int user_commit)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var exists = await _DbContext.OtherBooking.AsNoTracking().FirstOrDefaultAsync(s => s.Id == booking_id);
                    if (exists != null && exists.Id > 0)
                    {
                        exists.UpdatedDate = DateTime.Now;
                        exists.UpdatedBy = user_commit;
                        exists.OperatorId = user_id;
                        _DbContext.OtherBooking.Update(exists);
                        await _DbContext.SaveChangesAsync();
                    }
                    return 1;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateServiceOperator - OtherBookingDAL: " + ex);
                return -2;
            }
        }
        public async Task<long> UpdateOtherBookingPrice(long booking_id, double price, int user_summit)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var exists = _DbContext.OtherBooking.FirstOrDefault(x => x.Id == booking_id);
                    if (exists != null && exists.Id > 0)
                    {
                        exists.Price = price;
                        exists.UpdatedBy = user_summit;
                        exists.UpdatedDate = DateTime.Now;
                        _DbContext.OtherBooking.Update(exists);
                        await _DbContext.SaveChangesAsync();
                    }
                    return 1;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateOtherBookingPrice - OtherBookingDAL: " + ex);
                return -1;
            }
        }
        public async Task<DataTable> GetDetailOtherBookingById(long OtherBookingId)
        {
            try
            {

                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@OtherBookingId ", OtherBookingId);
                return dbWorker.GetDataTable(StoreProcedureConstant.SP_GetDetailOtherBookingById, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetDetailOtherBookingById - OtherBookingDAL: " + ex);
            }
            return null;
        }
        public async Task<long> UpdateServiceOperator(long booking_id, int user_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var exists = await _DbContext.OtherBooking.AsNoTracking().Where(s => s.Id == booking_id).FirstOrDefaultAsync();
                    if (exists != null && exists.Id > 0 && exists.Status == (int)ServiceStatus.WaitingExcution)
                    {
                        exists.UpdatedDate = DateTime.Now;
                        exists.UpdatedBy = user_id;
                        exists.OperatorId = user_id;
                        exists.Status = (int)ServiceStatus.OnExcution;
                        _DbContext.OtherBooking.Update(exists);
                        await _DbContext.SaveChangesAsync();
                    }
                    return 1;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateServiceOperator - OtherBookingDAL: " + ex);
                return -2;
            }
        }
        public async Task<long> UpdateServiceStatus(int status, long booking_id, int user_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var exists = await _DbContext.OtherBooking.AsNoTracking().Where(s => s.Id == booking_id).FirstOrDefaultAsync();
                    if (exists != null && exists.Id > 0)
                    {
                        exists.StatusOld = exists.Status;
                        exists.UpdatedDate = DateTime.Now;
                        exists.UpdatedBy = user_id;
                        exists.Status = status;
                        _DbContext.OtherBooking.Update(exists);
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

        public DataTable GetOtherBookingPackagesOptionalByServiceId(long serviceId)
        {
            try
            {

                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@BookingId", serviceId);
                string procedure = StoreProcedureConstant.SP_GetListOtherBookingPackagesOptionalByBookingId;
                return dbWorker.GetDataTable(procedure, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetOtherBookingPackagesOptionalByServiceId - OtherBookingDAL: " + ex);
            }
            return null;
        }
        public async Task<List<OtherBooking>> ServiceCodeSuggesstion(string txt_search)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return await _DbContext.OtherBooking.AsNoTracking().Where(x => x.ServiceCode.ToLower().Contains(txt_search.ToLower())).Skip(0).Take(30).ToListAsync();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ServiceCodeSuggesstion - OtherBookingDAL: " + ex);
                return null;
            }
        }
        public async Task<List<OtherBooking>> getListOtherBookingByOrderId(long OrderId)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return await _DbContext.OtherBooking.AsNoTracking().Where(x => x.OrderId== OrderId).ToListAsync();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ServiceCodeSuggesstion - OtherBookingDAL: " + ex);
                return null;
            }
        }
        public async Task<OtherBooking> GetWaterSportBookingById(long booking_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var ws_servicetype= _DbContext.AllCode.AsNoTracking().FirstOrDefault(x =>  x.Type == AllCodeType.SERVICE_TYPE_OTHER_MAIN);
                    if(ws_servicetype!=null && ws_servicetype.Id > 0)
                    {
                        return await _DbContext.OtherBooking.AsNoTracking().FirstOrDefaultAsync(x => x.Id == booking_id && x.ServiceType == ws_servicetype.CodeValue);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetWaterSportBookingById - OtherBookingDAL: " + ex);
            }
            return null;

        }
        public async Task<List<OtherBookingPackages>> GetWaterSportPackagesByBookingId(long booking_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return await _DbContext.OtherBookingPackages.AsNoTracking().Where(x => x.BookingId == booking_id ).ToListAsync();

                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetWaterSportPackagesByBookingId - OtherBookingDAL: " + ex);
            }
            return null;

        }
        public async Task<long> RemoveNonExistsOtherBookingPackages(long booking_id,List<long> keep_ids)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var delete_packages = await _DbContext.OtherBookingPackages.AsNoTracking().Where(x => x.BookingId == booking_id && !keep_ids.Contains(x.Id)).ToListAsync();
                    if (delete_packages != null && delete_packages.Count > 0)
                    {
                        foreach (var p in delete_packages)
                        {
                            p.BookingId *= -1;
                            _DbContext.OtherBookingPackages.Update(p);
                            await _DbContext.SaveChangesAsync();
                        }
                    }

                }
                return booking_id;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetWaterSportPackagesByBookingId - OtherBookingDAL: " + ex);
            }
            return -1;

        }
        public async Task<long> CreateOtherBooking(OtherBooking booking)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    if (booking.Id > 0)
                    {
                        return booking.Id;

                    }
                    else
                    {
                        _DbContext.OtherBooking.Add(booking);
                        await _DbContext.SaveChangesAsync();
                        return booking.Id;
                    }

                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CreateOtherBooking - OtherBookingDAL: " + ex);
                return -2;
            }
        } 
        public async Task<long> UpdateOtherBooking(OtherBooking booking)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    if (booking.Id <= 0)
                    {
                        return booking.Id;

                    }
                    else
                    {
                        _DbContext.OtherBooking.Update(booking);
                        await _DbContext.SaveChangesAsync();
                        return booking.Id;
                    }

                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CreateOtherBooking - OtherBookingDAL: " + ex);
                return -2;
            }
        }
        
    }
}
