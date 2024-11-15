using DAL.Generic;
using DAL.StoreProcedure;
using Entities.Models;
using Entities.ViewModels;
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
    public class TourGuestDAL : GenericService<TourGuests>
    {
        private DbWorker dbWorker;

        public TourGuestDAL(string connection) : base(connection)
        {
            dbWorker = new DbWorker(connection);

        }
        public async Task<List<TourGuests>> GetTourGuestByTourId(long tour_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return await _DbContext.TourGuests.Where(x => x.TourId == tour_id).ToListAsync();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetTourGuestByTourId - TourGuestDAL: " + ex.ToString());
                return null;
            }
        }
        private int CreateTourGuest(TourGuests model)
        {
            try
            {

                SqlParameter[] objParam_order = new SqlParameter[7];
                objParam_order[0] = new SqlParameter("@TourId", model.TourId);
                objParam_order[1] = new SqlParameter("@FullName", model.FullName);
                objParam_order[2] = new SqlParameter("@Birthday", model.Birthday);
                if (model.Phone != null && model.Phone.Trim() != "")
                {
                    objParam_order[3] = new SqlParameter("@Phone", model.Phone);
                }
                else
                {
                    objParam_order[3] = new SqlParameter("@Phone", DBNull.Value);

                }
                objParam_order[4] = new SqlParameter("@Note", model.Note);


                objParam_order[5] = new SqlParameter("@CreatedBy", model.CreatedBy);
                objParam_order[6] = new SqlParameter("@CreatedDate", model.CreatedDate);


                var id = dbWorker.ExecuteNonQuery(StoreProcedureConstant.InsertTourGuests, objParam_order);
                model.Id = id;
                return id;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CreateTourGuest - TourGuestDAL. " + ex.ToString());
                return -1;
            }
        }
        public int UpdateTourGuest(TourGuests model)
        {
            try
            {

                SqlParameter[] objParam_order = new SqlParameter[10];
                objParam_order[0] = model.TourId==null? new SqlParameter("@TourId", DBNull.Value) :new SqlParameter("@TourId", model.TourId);
                objParam_order[1] = model.FullName == null ? new SqlParameter("@FullName", DBNull.Value) : new SqlParameter("@FullName", model.FullName);
                objParam_order[2] = model.Birthday == null ? new SqlParameter("@Birthday", DBNull.Value) : new SqlParameter("@Birthday", model.Birthday);
                objParam_order[3] = model.Phone == null ? new SqlParameter("@Phone", DBNull.Value) : new SqlParameter("@Phone", model.Phone);
                objParam_order[4] = model.Note == null ? new SqlParameter("@Note", DBNull.Value) : new SqlParameter("@Note", model.Note);
                objParam_order[5] = model.CreatedBy == null ? new SqlParameter("@UpdatedBy", DBNull.Value) : new SqlParameter("@UpdatedBy", model.CreatedBy);
                objParam_order[6] = model.RoomNumber == null ? new SqlParameter("@RoomNumber", DBNull.Value) : new SqlParameter("@RoomNumber", model.RoomNumber);
                objParam_order[7] = model.Cccd == null ? new SqlParameter("@CCCD", DBNull.Value) : new SqlParameter("@CCCD", model.Cccd);
                objParam_order[8] = model.Gender == null ? new SqlParameter("@Gender", DBNull.Value) : new SqlParameter("@Gender", model.Gender);
                objParam_order[9] = new SqlParameter("@Id", model.Id);

                var id = dbWorker.ExecuteNonQuery(StoreProcedureConstant.UpdateTourGuests, objParam_order);
                model.Id = id;
                return id;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateTourPackages - TourGuestDAL. " + ex.ToString());
                return -1;
            }
        }
        public async Task<DataTable> GetListTourGuestsByTourId(long TourId)
        {
            try
            {
                SqlParameter[] objParam_order = new SqlParameter[1];
                objParam_order[0] = new SqlParameter("@TourId", TourId);

                return dbWorker.GetDataTable(StoreProcedureConstant.SP_GetListTourGuestsByTourId, objParam_order);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetListTour - TourDAL: " + ex);
                return null;
            }
        }
    }
}