using DAL.Generic;
using DAL.StoreProcedure;
using Entities.Models;
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
    public class HotelBookingRoomExtraPackagesDAL : GenericService<HotelBookingRoomExtraPackages>
    {
        private DbWorker dbWorker;
        public HotelBookingRoomExtraPackagesDAL(string connection) : base(connection)
        {
            dbWorker = new DbWorker(connection);
        }
      
        public async Task<List<HotelBookingRoomExtraPackages>> GetByBookingId(long hotel_booking_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var detail = await _DbContext.HotelBookingRoomExtraPackages.AsNoTracking().Where(x => x.HotelBookingId==hotel_booking_id).ToListAsync();
                    return detail;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetByBookingId - HotelBookingRoomExtraPackagesDAL. " + ex);
                return null;
            }
        }
        public async Task<HotelBookingRoomExtraPackages> GetByIDAndBookingId(long id,long hotel_booking_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var detail = await _DbContext.HotelBookingRoomExtraPackages.AsNoTracking().Where(x => x.Id==id && x.HotelBookingId == hotel_booking_id).FirstOrDefaultAsync();
                    return detail;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetByIDAndBookingId - HotelBookingRoomExtraPackagesDAL. " + ex);
                return null;
            }
        }
        public async Task<DataTable> Gethotelbookingroomextrapackagebyhotelbookingid(long HotelBookingId)
        {
            try
            {

                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@HotelBookingID", HotelBookingId);

                return dbWorker.GetDataTable(StoreProcedureConstant.sp_gethotelbookingroomextrapackagebyhotelbookingid, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Gethotelbookingroomextrapackagebyhotelbookingid - HotelBookingRoomExtraPackagesDAL: " + ex);
            }
            return null;
        }
        public async Task<int> UpdateHotelBookingRoomExtraPackages(HotelBookingRoomExtraPackages modele)
        {
            try
            {

                SqlParameter[] objParam = new SqlParameter[8];
                objParam[0] = new SqlParameter("@Id", modele.Id);
                objParam[1] = new SqlParameter("@PackageId", DBNull.Value);
                objParam[2] = new SqlParameter("@PackageCode", DBNull.Value);
                objParam[3] = new SqlParameter("@HotelBookingId", DBNull.Value);
                objParam[4] = new SqlParameter("@HotelBookingRoomID", DBNull.Value);
                objParam[5] = new SqlParameter("@Amount", DBNull.Value);
                objParam[6] = new SqlParameter("@UnitPrice", modele.UnitPrice);
                objParam[7] = new SqlParameter("@UpdatedBy", modele.UpdatedBy);

                return dbWorker.ExecuteNonQuery(StoreProcedureConstant.SP_UpdateHotelBookingRoomExtraPackages, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Gethotelbookingroomextrapackagebyhotelbookingid - HotelBookingRoomExtraPackagesDAL: " + ex);
            }
            return 0;
        }
    }
}
