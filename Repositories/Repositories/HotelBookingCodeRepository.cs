using DAL;
using Entities.ConfigModels;
using Entities.Models;
using Entities.ViewModels.HotelBookingCode;
using Microsoft.Extensions.Options;
using Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Repositories.Repositories
{
   public class HotelBookingCodeRepository : IHotelBookingCodeRepository
    {
        private readonly HotelBookingCodeDAL HotelBookingCodeDAL;
        public HotelBookingCodeRepository(IOptions<DataBaseConfig> dataBaseConfig )
        {

            HotelBookingCodeDAL = new HotelBookingCodeDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            
        }
        public async Task<List<HotelBookingCodeModel>> GetListlBookingCodeByHotelBookingId(long HotelBookingId, int Type)
        {
            try
            {

                DataTable dt = await HotelBookingCodeDAL.GetDetailBookingCodeByHotelBookingId(HotelBookingId,Type);
                if (dt != null && dt.Rows.Count > 0)
                {
                    var data = dt.ToList<HotelBookingCodeModel>();
                    return data;
                }
               
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetDetailBookingCodeByHotelBookingId - HotelBookingCodeRepository: " + ex);
            }
            return null;
        }
        public async Task<HotelBookingCode> GetDetailBookingCodeById(long id)
        {
            try
            {

                DataTable dt = await HotelBookingCodeDAL.GetDetailBookingCodeById(id);
                if (dt != null && dt.Rows.Count > 0)
                {
                    var data = dt.ToList<HotelBookingCode>();
                    return data[0];
                }
               
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetDetailBookingCodeByHotelBookingId - HotelBookingCodeRepository: " + ex);
            }
            return null;
        }
        public async Task<int> InsertHotelBookingCode(HotelBookingCodeViewModel model)
        {
            try
            {
                var exists = HotelBookingCodeDAL.GetHotelBookingCodeByDataIDandNote(model.HotelBookingId, model.Note ?? "",model.BookingCode ?? "");
                if (exists != null && exists.Id > 0)
                {
                    model.Id = exists.Id;
                    return await HotelBookingCodeDAL.UpdateHotelBookingCode(model);
                }
                return await HotelBookingCodeDAL.InsertHotelBookingCode(model);
            }
            catch(Exception ex)
            {
                LogHelper.InsertLogTelegram("GetDetailBookingCodeByHotelBookingId - HotelBookingCodeRepository: " + ex);
            }
            return 0;
        }
        public async Task<int> UpdateHotelBookingCode(HotelBookingCodeViewModel model)
        {
            try
            {

                return await HotelBookingCodeDAL.UpdateHotelBookingCode(model);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetDetailBookingCodeByHotelBookingId - HotelBookingCodeRepository: " + ex);
            }
            return 0;
        }
        public async Task<List<HotelBookingCodeModel>> GetListHotelBookingCodeByOrderId(long OrderId)
        {
            try
            {

                DataTable dt = await HotelBookingCodeDAL.GetListHotelBookingCodeByOrderId(OrderId);
                if (dt != null && dt.Rows.Count > 0)
                {
                    var data = dt.ToList<HotelBookingCodeModel>();
                    return data;
                }

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetDetailBookingCodeByHotelBookingId - HotelBookingCodeRepository: " + ex);
            }
            return null;
        }
        public async Task<List<HotelBookingCode>> GetHotelBookingCodeByType(long service_id, int type)
        {
            try
            {

                return await HotelBookingCodeDAL.GetHotelBookingCodeByType(service_id,type);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetHotelBookingCodeByName - HotelBookingCodeRepository: " + ex.ToString());
            }
            return null;
        }
        public async Task DeleteBookingCodeByIdandNote(long data_id, string note)
        {
            try
            {
                 HotelBookingCodeDAL.DeleteBookingCodeByIdandNote(data_id, note);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("DeleteBookingCodeByIdandNote - HotelBookingCodeRepository: " + ex);
            }
        }
    }
}
