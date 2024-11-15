using DAL.Generic;
using DAL.StoreProcedure;
using Entities.Models;
using Entities.ViewModels.OrderManual;
using Entities.ViewModels.SetServices;
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

    public class VinWonderBookingTicketDAL : GenericService<VinWonderBookingTicket>
    {
        private DbWorker dbWorker;

        public VinWonderBookingTicketDAL(string connection) : base(connection)
        {
            dbWorker = new DbWorker(connection);

        }
        public async Task<List<VinWonderBookingTicket>> GetVinWonderTicketByBookingId(long booking_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return await _DbContext.VinWonderBookingTicket.Where(x => x.BookingId == booking_id).ToListAsync();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetVinWonderBookingById - VinWonderBookingDAL: " + ex);
                return null;
            }
        }
        public async Task<DataTable> GetVinWonderTicketByBookingIdSP(long booking_id)
        {
            try
            {
                SqlParameter[] objParam_order = new SqlParameter[1];
                objParam_order[0] = new SqlParameter("@BookingId", booking_id);

                return dbWorker.GetDataTable(StoreProcedureConstant.GetVinWonderBookingTicketByBookingID, objParam_order);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetListTour - TourPackagesDAL: " + ex);
                return null;
            }
        }
        public async Task<long> UpdateVinWonderTicketOperatorPrice(List<VinWonderBookingTicket> data,int user_summit)
        {
            try
            {
                if (data.Count <= 0) return -1;
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    foreach (var item in data)
                    {
                        var exists_ticket = await _DbContext.VinWonderBookingTicket.AsNoTracking().FirstOrDefaultAsync(s => s.Id == item.Id);
                        if (exists_ticket != null && exists_ticket.Id > 0 && item.UnitPrice >= 0)
                        {
                            exists_ticket.UnitPrice = item.UnitPrice;
                            exists_ticket.UpdatedBy = user_summit;
                            exists_ticket.UpdatedDate = DateTime.Now;
                            _DbContext.VinWonderBookingTicket.Update(exists_ticket);
                            await _DbContext.SaveChangesAsync();
                        }

                    }
                    return 1;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateServiceOperator - VinWonderBookingDAL: " + ex);
                return -2;
            }
        }
    }
}
