using DAL.Generic;
using DAL.StoreProcedure;
using Entities.Models;
using Entities.ViewModels.HotelBookingCode;
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
   public class HotelBookingCodeDAL : GenericService<HotelBookingCode>
    {
        private DbWorker dbWorker;
        public HotelBookingCodeDAL(string connection) : base(connection)
        {
            dbWorker = new DbWorker(connection);
        }
        public async Task<DataTable> GetDetailBookingCodeByHotelBookingId(long HotelBookingId, int Type)
        {
            try
            {

                SqlParameter[] objParam = new SqlParameter[2];
                objParam[0] = new SqlParameter("@HotelBookingId", HotelBookingId);
                objParam[1] = new SqlParameter("@Type", Type);
     
                return dbWorker.GetDataTable(StoreProcedureConstant.sp_GetDetailBookingCodeByHotelBookingId, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetDetailBookingCodeByHotelBookingId - HotelBookingDAL: " + ex);
            }
            return null;
        }
        public async Task<DataTable> GetListHotelBookingCodeByOrderId(long OrderId)
        {
            try
            {

                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@OrderId", OrderId);
              
                return dbWorker.GetDataTable(StoreProcedureConstant.SP_GetListHotelBookingCodeByOrderId, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetListHotelBookingCodeByOrderId - HotelBookingDAL: " + ex);
            }
            return null;
        }
        public async Task<int> InsertHotelBookingCode(HotelBookingCodeViewModel model)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[9];
                objParam[0] = new SqlParameter("@ServiceId", model.HotelBookingId);
                objParam[1] = new SqlParameter("@Type", model.Type);
                objParam[2] = new SqlParameter("@BookingCode", model.BookingCode);
                objParam[3] = new SqlParameter("@Description", model.Description);
                objParam[4] = new SqlParameter("@AttactFile", model.AttactFile==null?"": model.AttactFile);
                objParam[5] = new SqlParameter("@IsDelete", model.IsDelete);
                objParam[6] = new SqlParameter("@OrderId", model.OrderId);
                objParam[7] = new SqlParameter("@CreatedBy", model.CreatedBy);
                objParam[8] = new SqlParameter("@Note", model.Note == null ? "" : model.Note);

                return dbWorker.ExecuteNonQuery(StoreProcedureConstant.Sp_InsertHotelBookingCode, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetDetailBookingCodeByHotelBookingId - HotelBookingDAL: " + ex);
            }
            return 0;
        }
        public async Task<int> UpdateHotelBookingCode(HotelBookingCodeViewModel model)
        {
            try
            {

                SqlParameter[] objParam = new SqlParameter[10];
                objParam[0] = new SqlParameter("@Id", model.Id);
                objParam[1] = new SqlParameter("@ServiceId", model.HotelBookingId);
                objParam[2] = new SqlParameter("@Type", model.Type);
                objParam[3] = new SqlParameter("@BookingCode", model.BookingCode);
                objParam[4] = new SqlParameter("@Description", model.Description);
                objParam[5] = new SqlParameter("@AttactFile", model.AttactFile == null ? "" : model.AttactFile);
                objParam[6] = new SqlParameter("@IsDelete", model.IsDelete);
                objParam[7] = new SqlParameter("@UpdatedBy", model.UpdatedBy);
                objParam[8] = new SqlParameter("@Note", model.Note == null ? "" : model.Note);
                objParam[9] = new SqlParameter("@OrderId", model.OrderId);
                return dbWorker.ExecuteNonQuery(StoreProcedureConstant.Sp_UpdateHotelBookingCode, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetDetailBookingCodeByHotelBookingId - HotelBookingDAL: " + ex);
            }
            return 0;
        }
        public async Task<DataTable> GetDetailBookingCodeById(long Id)
        {
            try
            {

                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@Id", Id);
               
                return dbWorker.GetDataTable(StoreProcedureConstant.SP_GetDetailBookingCodeById, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetDetailBookingCodeByHotelBookingId - HotelBookingDAL: " + ex);
            }
            return null;
        }
        public async Task<List<HotelBookingCode>> GetHotelBookingCodeByType( long service_id,int type)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var detail = await _DbContext.HotelBookingCode.AsNoTracking().Where(x => x.ServiceId==service_id && x.Type==type).ToListAsync();
                    return detail;
                }
            }
            catch (Exception ex)
            {
            }
            return null;

        }
        public HotelBookingCode GetHotelBookingCodeByDataIDandNote(long data_id,string note,string BookingCode)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {

                    return _DbContext.HotelBookingCode.AsNoTracking().FirstOrDefault(s => s.ServiceId == data_id && s.Note.ToLower().Contains(note.ToLower()) && s.BookingCode.ToLower().Equals(BookingCode.ToLower()));
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetHotelBookingCodeByDataIDandNote - HotelBookingDAL: " + ex);
                return null;
            }
        }
        public async Task DeleteBookingCodeByIdandNote(long data_id, string note)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {

                    var exists= await _DbContext.HotelBookingCode.AsNoTracking().Where(s => s.ServiceId == data_id && s.Note.ToLower().Contains(note.ToLower())).ToListAsync();
                    if(exists!=null && exists.Count > 0)
                    {
                        foreach(var detail in exists)
                        {
                            detail.ServiceId = detail.ServiceId * -1;
                            _DbContext.HotelBookingCode.Update(detail);
                            await _DbContext.SaveChangesAsync();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("DeleteBookingCodeByIdandNote - HotelBookingDAL: " + ex);
            }
        }
    }
}
