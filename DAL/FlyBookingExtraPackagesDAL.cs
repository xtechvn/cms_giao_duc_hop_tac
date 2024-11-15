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
    public class FlyBookingExtraPackagesDAL : GenericService<FlyBookingExtraPackages>
    {
        private static DbWorker _DbWorker;
        public FlyBookingExtraPackagesDAL(string connection) : base(connection)
        {
            _DbWorker = new DbWorker(connection);
        }
        public async Task<List<FlyBookingExtraPackages>> GetByFlyBookingId(string group_fly)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return await _DbContext.FlyBookingExtraPackages.AsNoTracking().Where(s => s.GroupFlyBookingId == group_fly).ToListAsync();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetByFlyBookingId - FlyBookingExtraPackagesDAL: " + ex);
                return null;
            }
        }
        public async Task<FlyBookingExtraPackages> GetById(long id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return await _DbContext.FlyBookingExtraPackages.AsNoTracking().Where(s => s.Id == id).FirstOrDefaultAsync();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetById - FlyBookingExtraPackagesDAL: " + ex);
                return null;
            }
        }

    }
}
