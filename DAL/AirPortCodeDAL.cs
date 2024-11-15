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
    public class AirPortCodeDAL : GenericService<AirPortCode>
    {
        private DbWorker _dbWorker;
        public AirPortCodeDAL(string connection) : base(connection)
        {
            _dbWorker = new DbWorker(connection);
        }
        public async Task<List<AirPortCode>> SearchAirPortCode(string txt_search = "")
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return await _DbContext.AirPortCode.Where(x => x.Code.ToLower().Contains(txt_search.ToLower()) || x.Description.ToLower().Contains(txt_search.ToLower()) || x.DistrictEn.ToLower().Contains(txt_search.ToLower())).ToListAsync();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SearchAirPortCode - AirPortCodeDAL: " + ex);
                return null;
            }
        }
        public async Task<AirPortCode> GetAirPortByCode(string air_port_code)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return await _DbContext.AirPortCode.Where(x => x.Code.ToLower()==air_port_code.ToLower()).FirstOrDefaultAsync();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetAirPortByCode - AirPortCodeDAL: " + ex);
                return null;
            }
        }
       
        public async Task<List<AirPortCode>> getAllAirportCode()
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return await _DbContext.AirPortCode.AsNoTracking().ToListAsync();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetAirPortByCode - AirPortCodeDAL: " + ex);
                return null;
            }
        }
        public async Task<List<Airlines>> getAllAirlines()
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return await _DbContext.Airlines.AsNoTracking().ToListAsync();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("getAllAirlines - AirPortCodeDAL: " + ex);
                return null;
            }
        }
        
    }
}
