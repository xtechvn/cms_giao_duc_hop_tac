using DAL.Generic;
using DAL.StoreProcedure;
using Entities.Models;
using Entities.ViewModels;
using Entities.ViewModels.OrderManual;
using Entities.ViewModels.Tour;
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
    public class TourDestinationDAL : GenericService<TourDestination>
    {
        private DbWorker dbWorker;

        public TourDestinationDAL(string connection) : base(connection)
        {
            dbWorker = new DbWorker(connection);

        }
        public async Task<List<TourDestination>> GetByTourId(long tour_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return await _DbContext.TourDestination.Where(x => x.TourId == tour_id).ToListAsync();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetByTourId - TourDestinationDAL: " + ex);
                return null;
            }
        }
       
    }
}