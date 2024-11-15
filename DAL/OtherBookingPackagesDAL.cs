using DAL.Generic;
using DAL.StoreProcedure;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace DAL
{

    public class OtherBookingPackagesDAL : GenericService<OtherBookingPackages>
    {
        private DbWorker dbWorker;

        public OtherBookingPackagesDAL(string connection) : base(connection)
        {
            dbWorker = new DbWorker(connection);

        }
        public List<OtherBookingPackages> GetOtherBookingPackagesByBookingId(long booking_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.OtherBookingPackages.Where(x => x.BookingId == booking_id).ToList();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetOtherBookingPackagesByBookingId - OtherBookingPackagesDAL: " + ex);
                return null;
            }
        }
        public async Task<long> CreateOtherBookingPackages(OtherBookingPackages booking)
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
                        _DbContext.OtherBookingPackages.Add(booking);
                        await _DbContext.SaveChangesAsync();
                        return booking.Id;
                    }

                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CreateOtherBookingPackages - OtherBookingPackagesDAL: " + ex);
                return -2;
            }
        }
    }
}
