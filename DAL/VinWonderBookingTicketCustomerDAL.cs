using DAL.Generic;
using DAL.StoreProcedure;
using Entities.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace DAL
{

    public class VinWonderBookingTicketCustomerDAL : GenericService<VinWonderBookingTicketCustomer>
    {
        private DbWorker dbWorker;

        public VinWonderBookingTicketCustomerDAL(string connection) : base(connection)
        {
            dbWorker = new DbWorker(connection);

        }
        public async Task<DataTable> GetVinWonderTicketCustomerByBookingIdSP(long booking_id)
        {
            try
            {
                SqlParameter[] objParam_order = new SqlParameter[1];
                objParam_order[0] = new SqlParameter("@BookingId", booking_id);

                return dbWorker.GetDataTable(StoreProcedureConstant.GetVinWonderBookingCustomerByBookingId, objParam_order);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetListTour - TourPackagesDAL: " + ex);
                return null;
            }
        }

    }
}
