using DAL.Generic;
using DAL.StoreProcedure;
using Entities.Models;
using Entities.ViewModels.Hotel;
using Entities.ViewModels.HotelBooking;
using Entities.ViewModels.OrderManual;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
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
    public class HotelDAL : GenericService<Hotel>
    {
        private DbWorker _DbWorker;
        public HotelDAL(string connection) : base(connection)
        {
            _DbWorker = new DbWorker(connection);
        }

        public Hotel GetByHotelId(string hotel_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.Hotel.AsNoTracking().FirstOrDefault(s => s.HotelId.Trim().ToLower() == hotel_id.Trim().ToLower());
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetDetailByLeg - HotelDAL: " + ex);
                return null;
            }
        }
        public Hotel GetHotelById(int id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.Hotel.Find(id);
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetHotelById - HotelDAL: " + ex);
                return null;
            }
        }

        public DataTable GetHotelDetailDataTable(int hotel_id)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[]
                {
                    new SqlParameter("@Id", hotel_id)
                };

                return _DbWorker.GetDataTable(StoreProcedureConstant.SP_GetHotelDetailById, objParam);
            }
            catch
            {
                throw;
            }
        }

        public Hotel GetByName(string name)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.Hotel.AsNoTracking().FirstOrDefault(s => s.Name.Trim().ToLower() == name.Trim().ToLower());
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetByName - HotelDAL: " + ex);
                return null;
            }
        }

        public async Task<int> UpdateyHotel(Hotel model)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var add = _DbContext.Hotel.Update(model);
                    await _DbContext.SaveChangesAsync();
                    return 1;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetDetailByLeg - FlyBookingDetailDAL: " + ex);
                return 0;
            }
        }

        public int CreateHotel(Hotel model)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[]
                {
                    new SqlParameter("@HotelId", model.HotelId ?? (object)DBNull.Value),
                    new SqlParameter("@Name", model.Name ?? (object)DBNull.Value),
                    new SqlParameter("@Email", model.Email ?? (object)DBNull.Value),
                    new SqlParameter("@ImageThumb", model.ImageThumb ?? (object)DBNull.Value),
                    new SqlParameter("@NumberOfRoooms", model.NumberOfRoooms?? (object)DBNull.Value),
                    new SqlParameter("@Star", model.Star ?? (object)DBNull.Value),
                    new SqlParameter("@ReviewCount", model.ReviewCount ?? (object)DBNull.Value),
                    new SqlParameter("@ReviewRate", model.ReviewRate ?? (object)DBNull.Value),
                    new SqlParameter("@City", model.City?? (object)DBNull.Value),
                    new SqlParameter("@Country", model.Country ?? (object)DBNull.Value),
                    new SqlParameter("@Street", model.Street ?? (object)DBNull.Value),
                    new SqlParameter("@State", model.State?? (object)DBNull.Value),
                    new SqlParameter("@HotelType", model.HotelType ?? (object)DBNull.Value),
                    new SqlParameter("@TypeOfRoom", model.TypeOfRoom?? (object)DBNull.Value),
                    new SqlParameter("@IsRefundable", model.IsRefundable ?? (object)DBNull.Value),
                    new SqlParameter("@IsInstantlyConfirmed ", model.IsInstantlyConfirmed ?? (object)DBNull.Value),
                    new SqlParameter("@GroupName", model.GroupName?? (object)DBNull.Value),
                    new SqlParameter("@Telephone", model.Telephone?? (object)DBNull.Value),
                    new SqlParameter("@CheckinTime", DateTime.Now),
                    new SqlParameter("@CheckoutTime", DateTime.Now),
                    new SqlParameter("@SupplierId",model.SupplierId ?? (object)DBNull.Value),
                    new SqlParameter("@ProvinceId", model.ProvinceId ?? (object)DBNull.Value),
                    new SqlParameter("@TaxCode",  model.TaxCode ?? (object)DBNull.Value),
                    new SqlParameter("@EstablishedYear",  model.EstablishedYear ?? (object)DBNull.Value),
                    new SqlParameter("@RatingStar", model.RatingStar ?? (object)DBNull.Value),
                    new SqlParameter("@ChainBrands", model.ChainBrands ?? (object)DBNull.Value),
                    new SqlParameter("@VerifyDate", model.VerifyDate ?? (object)DBNull.Value),
                    new SqlParameter("@SalerId", model.SalerId ?? (object)DBNull.Value),
                    new SqlParameter("@IsDisplayWebsite", model.IsDisplayWebsite),
                    new SqlParameter("@ShortName", model.ShortName ?? (object)DBNull.Value),
                    new SqlParameter("@Description", model.Description ?? (object)DBNull.Value),
                    new SqlParameter("@CreatedBy", model.CreatedBy?? (object)DBNull.Value),
                    new SqlParameter("@CreatedDate", DateTime.Now),
                    new SqlParameter("@IsCommitFund",model.IsCommitFund?? (object)DBNull.Value),
                    new SqlParameter("@Position",model.Position?? (object)DBNull.Value)

                };
                return _DbWorker.ExecuteNonQuery(StoreProcedureConstant.SP_InsertHotel, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CreateHotel - HotelDAL. " + ex);
                throw;
            }
        }

        public int UpdateHotel(Hotel model)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[]
                {
                    new SqlParameter("@Id", model.Id),
                    new SqlParameter("@HotelId", model.HotelId ?? (object)DBNull.Value ),
                    new SqlParameter("@Name", model.Name ?? (object)DBNull.Value),
                    new SqlParameter("@Email", model.Email ?? (object)DBNull.Value),
                    new SqlParameter("@ImageThumb", model.ImageThumb ?? (object)DBNull.Value),
                    new SqlParameter("@NumberOfRoooms", model.NumberOfRoooms?? (object)DBNull.Value),
                    new SqlParameter("@Star", model.Star ?? (object)DBNull.Value),
                    new SqlParameter("@ReviewCount", model.ReviewCount ?? (object)DBNull.Value),
                    new SqlParameter("@ReviewRate", model.ReviewRate ?? (object)DBNull.Value),
                    new SqlParameter("@City", model.City?? (object)DBNull.Value),
                    new SqlParameter("@Country", model.Country ?? (object)DBNull.Value),
                    new SqlParameter("@Street", model.Street ?? (object)DBNull.Value),
                    new SqlParameter("@State", model.State?? (object)DBNull.Value),
                    new SqlParameter("@HotelType", model.HotelType ?? (object)DBNull.Value),
                    new SqlParameter("@TypeOfRoom", model.TypeOfRoom?? (object)DBNull.Value),
                    new SqlParameter("@IsRefundable", model.IsRefundable ?? (object)DBNull.Value),
                    new SqlParameter("@IsInstantlyConfirmed ", model.IsInstantlyConfirmed ?? (object)DBNull.Value),
                    new SqlParameter("@GroupName", model.GroupName?? (object)DBNull.Value),
                    new SqlParameter("@Telephone", model.Telephone?? (object)DBNull.Value),
                    new SqlParameter("@CheckinTime", DateTime.Now),
                    new SqlParameter("@CheckoutTime", DateTime.Now),
                    new SqlParameter("@SupplierId",model.SupplierId ?? (object)DBNull.Value),
                    new SqlParameter("@ProvinceId", model.ProvinceId ?? (object)DBNull.Value),
                    new SqlParameter("@TaxCode",  model.TaxCode ?? (object)DBNull.Value),
                    new SqlParameter("@EstablishedYear",  model.EstablishedYear ?? (object)DBNull.Value),
                    new SqlParameter("@RatingStar", model.RatingStar ?? (object)DBNull.Value),
                    new SqlParameter("@ChainBrands", model.ChainBrands ?? (object)DBNull.Value),
                    new SqlParameter("@VerifyDate", model.VerifyDate ?? (object)DBNull.Value),
                    new SqlParameter("@SalerId", model.SalerId ?? (object)DBNull.Value),
                    new SqlParameter("@IsDisplayWebsite", model.IsDisplayWebsite),
                    new SqlParameter("@ShortName", model.ShortName ?? (object)DBNull.Value),
                    new SqlParameter("@Description", model.Description ?? (object)DBNull.Value),
                    new SqlParameter("@UpdatedBy",model.UpdatedBy?? (object)DBNull.Value),
                    new SqlParameter("@IsCommitFund",model.IsCommitFund?? (object)DBNull.Value),
                    new SqlParameter("@Position",model.Position?? (object)DBNull.Value)
                };

                return _DbWorker.ExecuteNonQuery(StoreProcedureConstant.SP_UpdateHotel, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateHotel - HotelDAL. " + ex);
                throw;
            }
        }

        public DataTable GetHotelPagingList(HotelFilterModel model)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[]
                {
                    new SqlParameter("@FullName",model.FullName ?? (object)DBNull.Value),
                    new SqlParameter("@ProvinceId",model.ProvinceId ?? (object)DBNull.Value),
                    new SqlParameter("@RatingStar",model.RatingStar ?? (object)DBNull.Value),
                    new SqlParameter("@ChainBrands",model.ChainBrands ?? (object)DBNull.Value),
                    new SqlParameter("@SalerId",model.SalerId ?? (object)DBNull.Value),
                    new SqlParameter("@TypeFilter",model.TypeFilter==0?  (object)DBNull.Value:model.TypeFilter ),
                    new SqlParameter("@PageIndex",model.PageIndex),
                    new SqlParameter("@PageSize",model.PageSize)
                };

                return _DbWorker.GetDataTable(StoreProcedureConstant.SP_GetListHotel, objParam);
            }
            catch
            {
                throw;
            }
        }

        public DataTable GetSuggestHotelByName(string name, int? size)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[]
                {
                    new SqlParameter("@HotelName", name ?? (object)DBNull.Value),
                    new SqlParameter("@Size", size ?? 20),
                };

                return _DbWorker.GetDataTable(StoreProcedureConstant.SP_GetSuggestHotelByName, objParam);
            }
            catch
            {
                throw;
            }
        }
        public async Task<bool> UpdateHotelSurchargeNote(string body, int id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var exists = _DbContext.Hotel.AsNoTracking().FirstOrDefault(s => s.Id == id);
                    if (exists != null && exists.Id > 0)
                    {
                        exists.OtherSurcharge = body;
                        _DbContext.Update(exists);
                        await _DbContext.SaveChangesAsync();
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateHotelSurchargeNote - HotelDAL: " + ex);
            }
            return false;

        }

        #region banking account

        public DataTable GetListHotelBankingAccountByHotelId(int hotelId)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[]
                {
                new SqlParameter("@HotelId", hotelId)
                };

                return _DbWorker.GetDataTable(StoreProcedureConstant.SP_GetListHotelBankingAccountByHotelId, objParam);
            }
            catch
            {
                throw;
            }
        }
        public HotelBankingAccount GetHotelBankingAccountByName(string account_name, int hotel_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.HotelBankingAccount.AsNoTracking().Where(x => x.AccountName.Trim().ToLower() == account_name.ToLower() && x.HotelId == hotel_id).FirstOrDefault();
                }
            }
            catch
            {
                return null;
            }
        }
        public HotelBankingAccount GetHotelBankingAccountById(int Id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.HotelBankingAccount.Find(Id);
                }
            }
            catch
            {
                throw;
            }
        }
        public int InsertHotelBankingAccount(HotelBankingAccount model)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[]
                {
                    new SqlParameter("@BankId", model.BankId ?? (object)DBNull.Value),
                    new SqlParameter("@AccountNumber", model.AccountNumber ?? (object)DBNull.Value),
                    new SqlParameter("@AccountName", model.AccountName ?? (object)DBNull.Value),
                    new SqlParameter("@Branch", model.Branch ?? (object)DBNull.Value),
                    new SqlParameter("@HotelId", model.HotelId),
                    new SqlParameter("@CreatedBy", model.CreatedBy),
                    new SqlParameter("@CreatedDate",DateTime.Now)
                };

                return _DbWorker.ExecuteNonQuery(StoreProcedureConstant.SP_InsertHotelBankingAccount, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("InsertHotelBankingAccount - HotelDAL. " + ex);
                throw;
            }
        }

        public int UpdateHotelBankingAccount(HotelBankingAccount model)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[]
                {
                    new SqlParameter("@Id", model.Id),
                    new SqlParameter("@BankId", model.BankId ?? (object)DBNull.Value),
                    new SqlParameter("@AccountNumber", model.AccountNumber ?? (object)DBNull.Value),
                    new SqlParameter("@AccountName", model.AccountName ?? (object)DBNull.Value),
                    new SqlParameter("@Branch", model.Branch ?? (object)DBNull.Value),
                    new SqlParameter("@HotelId", model.HotelId),
                    new SqlParameter("@UpdatedBy", model.UpdatedBy)
                };

                return _DbWorker.ExecuteNonQuery(StoreProcedureConstant.SP_UpdateHotelBankingAccount, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateHotelBankingAccount - HotelDAL. " + ex);
                throw;
            }
        }

        public int DeleteHotelBankingAccountById(int Id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var model = _DbContext.HotelBankingAccount.Find(Id);
                    if (model != null)
                    {
                        _DbContext.HotelBankingAccount.Remove(model);
                        _DbContext.SaveChanges();
                    }
                    return Id;
                }
            }
            catch
            {
                throw;
            }
        }

        #endregion

        #region contact
        public DataTable GetHotelContactDataTable(int hotel_id)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[]
                {
                    new SqlParameter("@HotelId", hotel_id)
                };

                return _DbWorker.GetDataTable(StoreProcedureConstant.SP_GetListHotelContactByHotelId, objParam);
            }
            catch
            {
                throw;
            }
        }

        public HotelContact GetHotelContactById(int Id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.HotelContact.Find(Id);
                }
            }
            catch
            {
                throw;
            }
        }

        public int DeleteHotelContactById(int Id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var model = _DbContext.HotelContact.Find(Id);
                    if (model != null)
                    {
                        _DbContext.HotelContact.Remove(model);
                        _DbContext.SaveChanges();
                    }
                    return Id;
                }
            }
            catch
            {
                throw;
            }
        }

        public int InsertHotelContact(HotelContact model)
        {
            try
            {
                SqlParameter[] objParam_contractPay = new SqlParameter[]
                {
                    new SqlParameter("@HotelId", model.HotelId),
                    new SqlParameter("@Name", model.Name ?? (object)DBNull.Value),
                    new SqlParameter("@Mobile", model.Mobile ?? (object)DBNull.Value),
                    new SqlParameter("@Email", model.Email ?? (object)DBNull.Value),
                    new SqlParameter("@Position", model.Position ?? (object)DBNull.Value),
                    new SqlParameter("@CreatedBy", model.CreatedBy ?? (object)DBNull.Value),
                    new SqlParameter("@CreateDate ", DateTime.Now)
                };
                return _DbWorker.ExecuteNonQuery(StoreProcedureConstant.sp_InsertHotelContact, objParam_contractPay);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("InsertHotelContact - HotelDAL. " + ex);
                return -1;
            }
        }

        public int UpdateHotelContact(HotelContact model)
        {
            try
            {
                SqlParameter[] objParam_contractPay = new SqlParameter[]
                {
                    new SqlParameter("@Id", model.Id),
                    new SqlParameter("@HotelId", model.HotelId),
                    new SqlParameter("@Name", model.Name ?? (object)DBNull.Value),
                    new SqlParameter("@Mobile", model.Mobile ?? (object)DBNull.Value),
                    new SqlParameter("@Email", model.Email ?? (object)DBNull.Value),
                    new SqlParameter("@Position", model.Position ?? (object)DBNull.Value),
                    new SqlParameter("@UpdatedBy", model.UpdatedBy ?? (object)DBNull.Value)
                };
                return _DbWorker.ExecuteNonQuery(StoreProcedureConstant.sp_UpdateHotelContact, objParam_contractPay);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateHotelContact" + ex);
                return -1;
            }
        }
        #endregion

        #region surcharge
        public DataTable GetHotelSurchargeDataTable(int hotel_id, int page_index, int page_size)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[]
                {
                    new SqlParameter("@HotelId", hotel_id),
                    new SqlParameter("@PageIndex", page_index),
                    new SqlParameter("@PageSize", page_size)
                };

                return _DbWorker.GetDataTable(StoreProcedureConstant.SP_GetListHotelSurchargeByHotelId, objParam);
            }
            catch
            {
                throw;
            }
        }

        public HotelSurcharge GetHotelSurchargeById(int Id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.HotelSurcharge.Find(Id);
                }
            }
            catch
            {
                throw;
            }
        }

        public int DeleteHotelSurchargeById(int Id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var model = _DbContext.HotelSurcharge.Find(Id);
                    if (model != null)
                    {
                        _DbContext.HotelSurcharge.Remove(model);
                        _DbContext.SaveChanges();
                    }
                    return Id;
                }
            }
            catch
            {
                throw;
            }
        }

        public int InsertHotelSurcharge(HotelSurcharge model)
        {
            try
            {
                SqlParameter[] objParam_contractPay = new SqlParameter[]
                {
                    new SqlParameter("@HotelId", model.HotelId),
                    new SqlParameter("@Code", model.Code),
                    new SqlParameter("@Name", model.Name ?? (object)DBNull.Value),
                    new SqlParameter("@Description", model.Description ?? (object)DBNull.Value),
                    new SqlParameter("@Price", model.Price),
                    new SqlParameter("@Status", model.Status),
                    new SqlParameter("@CreatedBy", model.CreatedBy ?? (object)DBNull.Value),
                    new SqlParameter("@CreatedDate", DateTime.Now),
                    new SqlParameter("@FromDate", model.FromDate ?? (object)DBNull.Value),
                    new SqlParameter("@ToDate", model.ToDate ?? (object)DBNull.Value),

                };
                return _DbWorker.ExecuteNonQuery(StoreProcedureConstant.sp_InsertHotelSurcharge, objParam_contractPay);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("InsertHotelSurcharge - HotelDAL. " + ex);
                return -1;
            }
        }

        public int UpdateHotelSurcharge(HotelSurcharge model)
        {
            try
            {
                SqlParameter[] objParam_contractPay = new SqlParameter[]
                {
                    new SqlParameter("@Id", model.Id),
                    new SqlParameter("@HotelId", model.HotelId),
                    new SqlParameter("@Code", model.Code),
                    new SqlParameter("@Name", model.Name ?? (object)DBNull.Value),
                    new SqlParameter("@Description", model.Description ?? (object)DBNull.Value),
                    new SqlParameter("@Price", model.Price),
                    new SqlParameter("@Status", model.Status),
                    new SqlParameter("@UpdatedBy", model.UpdatedBy ?? (object)DBNull.Value),
                     new SqlParameter("@FromDate", model.FromDate ?? (object)DBNull.Value),
                    new SqlParameter("@ToDate", model.ToDate ?? (object)DBNull.Value)
                };
                return _DbWorker.ExecuteNonQuery(StoreProcedureConstant.sp_UpdateHotelSurcharge, objParam_contractPay);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateHotelSurcharge" + ex);
                return -1;
            }
        }
        #endregion

        #region room

        public int CreateHotelRoom(HotelRoom model)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[]
                {
                    new SqlParameter("@HotelId", model.HotelId),
                    new SqlParameter("@RoomId", model.RoomId ?? String.Empty),
                    new SqlParameter("@Name", model.Name ?? model.Name),
                    new SqlParameter("@Code", model.Code ?? String.Empty),
                    new SqlParameter("@Avatar", model.Avatar ?? (object)DBNull.Value),
                    new SqlParameter("@NumberOfBedRoom", model.NumberOfBedRoom ?? (object)DBNull.Value),
                    new SqlParameter("@Description", model.Description ?? (object)DBNull.Value),
                    new SqlParameter("@TypeOfRoom", model.TypeOfRoom ?? (object)DBNull.Value),
                    new SqlParameter("@Thumbnails", model.Thumbnails ?? (object)DBNull.Value),
                    new SqlParameter("@Extends", model.Extends ?? (object)DBNull.Value),
                    new SqlParameter("@BedRoomType", model.BedRoomType ?? (object)DBNull.Value),
                    new SqlParameter("@NumberOfAdult", model.NumberOfAdult ?? (object)DBNull.Value),
                    new SqlParameter("@NumberOfChild", model.NumberOfChild ?? (object)DBNull.Value),
                    new SqlParameter("@NumberOfRoom", model.NumberOfRoom ?? (object)DBNull.Value),
                    new SqlParameter("@RoomArea", model.RoomArea ?? (object)DBNull.Value),
                    new SqlParameter("@RoomAvatar", model.RoomAvatar ?? (object)DBNull.Value),
                    new SqlParameter("@IsActive", model.IsActive),
                    new SqlParameter("@IsDisplayWebsite", model.IsDisplayWebsite),
                    new SqlParameter("@CreatedBy", model.CreatedBy),
                    new SqlParameter("@CreatedDate", model.CreatedDate ?? (object)DBNull.Value),
                };

                return _DbWorker.ExecuteNonQuery(StoreProcedureConstant.SP_InsertHotelRoom, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CreateHotelRoom:" + ex);
                return -1;
            }
        }

        public int UpdateHotelRoom(HotelRoom model)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[]
                {
                    new SqlParameter("@Id", model.Id),
                     new SqlParameter("@HotelId", model.HotelId),
                     new SqlParameter("@RoomId", model.RoomId ?? (object)DBNull.Value),
                     new SqlParameter("@Name", model.Name ?? (object)DBNull.Value),
                     new SqlParameter("@Code", model.Code ?? (object)DBNull.Value),
                     new SqlParameter("@Avatar", model.Avatar ?? (object)DBNull.Value),
                     new SqlParameter("@NumberOfBedRoom", model.NumberOfBedRoom ?? (object)DBNull.Value),
                     new SqlParameter("@Description", model.Description ?? (object)DBNull.Value),
                     new SqlParameter("@TypeOfRoom", model.TypeOfRoom ?? (object)DBNull.Value),
                     new SqlParameter("@Thumbnails", model.Thumbnails ?? (object)DBNull.Value),
                     new SqlParameter("@Extends", model.Extends ?? (object)DBNull.Value),
                     new SqlParameter("@BedRoomType", model.BedRoomType ?? (object)DBNull.Value),
                     new SqlParameter("@NumberOfAdult", model.NumberOfAdult ?? (object)DBNull.Value),
                     new SqlParameter("@NumberOfChild", model.NumberOfChild ?? (object)DBNull.Value),
                     new SqlParameter("@NumberOfRoom", model.NumberOfRoom ?? (object)DBNull.Value),
                     new SqlParameter("@RoomArea", model.RoomArea ?? (object)DBNull.Value),
                     new SqlParameter("@RoomAvatar", model.RoomAvatar ?? (object)DBNull.Value),
                     new SqlParameter("@IsActive", model.IsActive),
                     new SqlParameter("@IsDisplayWebsite", model.IsDisplayWebsite ),
                     new SqlParameter("@UpdatedBy", model.UpdatedBy)
                };

                return _DbWorker.ExecuteNonQuery(StoreProcedureConstant.SP_UpdateHotelRoom, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateHotelRoom" + ex);
                return -1;
            }
        }

        public DataTable GetDetailHotelRoom(long room_id)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[]
                {
                    new SqlParameter("@RoomId",room_id),
                };

                return _DbWorker.GetDataTable(StoreProcedureConstant.SP_GetDetailHotelRoom, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CreateSupplierRoom - HotelDAL. " + ex);
                throw;
            }
        }

        public DataTable GetHotelRoomByHotelId(int hotel_id, int page_index, int page_size)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[]
                {
                    new SqlParameter("@HotelId",hotel_id),
                    new SqlParameter("@PageIndex",page_index),
                    new SqlParameter("@PageSize",page_size)
                };

                return _DbWorker.GetDataTable(StoreProcedureConstant.SP_GetHotelRoomByHotelId, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CreateSupplierRoom - HotelDAL. " + ex);
                throw;
            }
        }

        public int DeleteHotelRoom(long room_id)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[]
                {
                    new SqlParameter("@RoomId", room_id),
                };

                return _DbWorker.ExecuteNonQuery(StoreProcedureConstant.SP_DeleteHotelRoom, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("DeleteHotelRoom - HotelDAL. " + ex);
                return -1;
            }
        }

        public int CheckExistHotelRoomUsing(int hotel_id, int room_id)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[]
                {
                    new SqlParameter("@RoomTypeId", room_id),
                    new SqlParameter("@HotelId", hotel_id)
                };

                return _DbWorker.ExecuteNonQuery(StoreProcedureConstant.SP_CheckExistHotelRoomUsing, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CheckExistHotelRoomNameUsing - HotelDAL. " + ex);
                return -1;
            }
        }

        public bool CheckExistRoomName(long room_id, long hotel_id, string name)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[]
                {
                    new SqlParameter("@Id",room_id),
                    new SqlParameter("@TypeOfRoom",name),
                    new SqlParameter("@HotelId",hotel_id),
                };

                var result = _DbWorker.ExecuteNonQuery(StoreProcedureConstant.SP_CheckExistHotelRoomName, objParam);

                return result > 0 ? true : false;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CreateSupplierRoom - HotelDAL. " + ex);
                throw;
            }
        }
        #endregion
        #region Hotel_fe

        public DataTable GetFEHotelRoomByHotelId(int hotel_id, int page_index, int page_size)
        {
            try
            {
                SqlParameter[] objParams = new SqlParameter[]
                {
                    new SqlParameter("@HotelId", hotel_id),
                    new SqlParameter("@PageIndex", page_index),
                    new SqlParameter("@PageSize", page_size),
                };

                return _DbWorker.GetDataTable(StoreProcedureConstant.SP_fe_GetHotelRoomByHotelId, objParams);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SP_fe_GetHotelRoomByHotelId - HotelDAL. " + ex);
                return null;
            }
        }
        public DataTable GetHotelPricePolicy(string hotel_id, string room_ids, string client_types)
        {
            try
            {
                SqlParameter[] objParam_order = new SqlParameter[3];
                objParam_order[0] = new SqlParameter("@HotelID", hotel_id);
                if (room_ids == null || room_ids.Trim() == "")
                {
                    objParam_order[1] = new SqlParameter("@RoomIds", DBNull.Value);

                }
                else
                {
                    objParam_order[1] = new SqlParameter("@RoomIds", room_ids);

                }

                if (client_types == null || client_types.Trim() == "")
                {
                    objParam_order[2] = new SqlParameter("@ClientTypes", DBNull.Value);

                }
                else
                {
                    objParam_order[2] = new SqlParameter("@ClientTypes", client_types);

                }
                var rs = _DbWorker.GetDataTable(StoreProcedureConstant.SP_GetHotelPricePolicyActiveByHotelID, objParam_order);
                return rs;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetHotelPricePolicy - HotelDAL. " + ex);
                return null;
            }
        }
        public DataTable GetFERoomPackageDaiLyListByRoomId(int room_id, DateTime fromDate, DateTime toDate)
        {
            try
            {
                SqlParameter[] objParams = new SqlParameter[]
                {
                    new SqlParameter("@RoomId", room_id),
                    new SqlParameter("@FromDate", fromDate),
                    new SqlParameter("@ToDate", toDate),
                    //new SqlParameter("@PageIndex", 1),
                    //new SqlParameter("@PageSize", 1000),
                };

                return _DbWorker.GetDataTable(StoreProcedureConstant.SP_GetListProgramsPackageDailyByRoomId, objParams);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SP_GetListProgramsPackageByRoomId - HotelDAL. " + ex);
                return null;
            }
        }
        public DataTable GetFEHotelRoomPackageByRoomId(int room_id, DateTime fromDate, DateTime toDate)
        {
            try
            {
                SqlParameter[] objParams = new SqlParameter[]
                {
                    new SqlParameter("@RoomId", room_id),
                    new SqlParameter("@FromDate", fromDate),
                    new SqlParameter("@ToDate", toDate),
                    //new SqlParameter("@PageIndex", 1),
                    //new SqlParameter("@PageSize", 1000),
                };

                return _DbWorker.GetDataTable(StoreProcedureConstant.SP_GetListProgramsPackageByRoomId, objParams);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SP_GetListProgramsPackageByRoomId - HotelDAL. " + ex);
                return null;
            }
        }
        #endregion
        #region Hotel_Position
        public  int  InsertHotelPosition(HotelPosition model)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[]
                {
                    new SqlParameter("@HotelId", model.HotelId),
                    new SqlParameter("@PositionType", model.PositionType),
                    new SqlParameter("@Position", model.Position ),
                    new SqlParameter("@Status", model.Status),
                    new SqlParameter("@CreatedBy", model.CreatedBy ?? (object)DBNull.Value),
                    new SqlParameter("@CreatedDate",DBNull.Value),
               
                };

                return  _DbWorker.ExecuteNonQuery(StoreProcedureConstant.sp_InsertHotelPosition, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("InsertHotelPosition - HotelDAL. " + ex);
                throw;
            }
        }
        public int UpdateHotelPosition(HotelPosition model)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[]
                {
                    new SqlParameter("@Id", model.Id),
                    new SqlParameter("@HotelId", model.HotelId),
                  new SqlParameter("@PositionType", model.PositionType),
                    new SqlParameter("@Position", model.Position ),
                    new SqlParameter("@Status", model.Status),
                    new SqlParameter("@UpdatedBy", model.UpdatedBy ?? (object)DBNull.Value),
                };

                return _DbWorker.ExecuteNonQuery(StoreProcedureConstant.sp_UpdateHotelPosition, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateHotelPosition - HotelDAL. " + ex);
                throw;
            }
        }

        public HotelPosition GetDetailHotelPosition(long Position,long PositionType)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var model = _DbContext.HotelPosition.FirstOrDefault(s=>s.Position == Position && s.PositionType== PositionType);
                    
                    return model;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateHotelPosition - HotelDAL. " + ex);
                throw;
            }
        }
        public List<HotelPosition> GetListHotelPosition(long HotelId)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var model = _DbContext.HotelPosition.Where(s => s.HotelId == HotelId).ToList();

                    return model;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetListHotelPosition - HotelDAL. " + ex);
                throw;
            }
        }
        public List<HotelPosition> GetDetailHotelPositionByPosition(long Position, long PositionType)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var model = _DbContext.HotelPosition.Where(s => s.Position == Position && s.Status == PositionStatus.HD && s.PositionType == PositionType).ToList();

                    return model;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetDetailTourPositionByPosition - HotelDAL. " + ex);
                throw;
            }
        }
        #endregion
    }
}