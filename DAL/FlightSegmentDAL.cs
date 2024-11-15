using DAL.Generic;
using DAL.StoreProcedure;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;

namespace DAL
{
    public class FlightSegmentDAL : GenericService<FlightSegment>
    {
        private static DbWorker _DbWorker;
        public FlightSegmentDAL(string connection) : base(connection)
        {
            _DbWorker = new DbWorker(connection);
        }

        public FlightSegment GetFlyBookingDetailId(long flyBookingDetailId)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.FlightSegment.AsNoTracking().FirstOrDefault(s => s.FlyBookingId == flyBookingDetailId);
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetFlyBookingDetailId - FlightSegmentDAL: " + ex);
                return null;
            }
        }

        public List<FlightSegment> GetFlyBookingDetailIds(List<long> flyBookingDetailIds)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.FlightSegment.AsNoTracking().Where(s => flyBookingDetailIds.Contains(s.FlyBookingId)).ToList();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetFlyBookingDetailIds - FlightSegmentDAL: " + ex);
                return new List<FlightSegment>();
            }
        }
    }
}
