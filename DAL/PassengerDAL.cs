using DAL.Generic;
using DAL.StoreProcedure;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace DAL
{
    public class PassengerDAL : GenericService<Passenger>
    {
        private DbWorker dbWorker;
        public PassengerDAL(string connection) : base(connection)
        {
            dbWorker = new DbWorker(connection);
        }
      
        public async Task<List<Passenger>> GetByOrderId(long order_id, string group_fly)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var passengers = await _DbContext.Passenger.AsNoTracking().Where(x => x.OrderId == order_id && x.GroupBookingId.Trim()==group_fly.Trim()).ToListAsync();
                   
                    return passengers;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetByOrderId - PassengerDAL. " + ex.ToString());
                return new List<Passenger>();
            }
        }
        public async Task<List<Passenger>> GetPassengerByOrderId(long order_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var passengers = await _DbContext.Passenger.AsNoTracking().Where(x => x.OrderId == order_id ).ToListAsync();

                    return passengers;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetPassengerByOrderId - PassengerDAL. " + ex.ToString());
                return new List<Passenger>();
            }
        }
    }
}
