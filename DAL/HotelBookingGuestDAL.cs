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
    public class HotelBookingGuestDAL : GenericService<HotelBookingRoomRates>
    {
        private DbWorker dbWorker;
        public HotelBookingGuestDAL(string connection) : base(connection)
        {
            dbWorker = new DbWorker(connection);
        }
      
        public async Task<List<HotelGuest>> GetByBookingId(long hotel_booking_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                  //  var detail_room = await _DbContext.HotelBookingRooms.AsNoTracking().Where(x => x.HotelBookingId == hotel_booking_id).ToListAsync();
                  //  var detail_room_ids = detail_room.Select(x => x.Id);
                    var detail = await _DbContext.HotelGuest.AsNoTracking().Where(x => x.HotelBookingId==hotel_booking_id).ToListAsync();
                    return detail;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetByBookingId - HotelBookingGuestDAL. " + ex);
                return null;
            }
        }

        public async Task<DataTable> GetHotelGuestByHotelBookingId(long HotelBookingId)
        {
            try
            {

                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@HotelBookingId", HotelBookingId);
                
                return dbWorker.GetDataTable(StoreProcedureConstant.SP_GetHotelGuestByOrderId, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetHotelBookingRoomByHotelBookingID - HotelBookingGuestDAL: " + ex);
            }
            return null;
        }
    }
}
