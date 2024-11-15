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
    public class TourProductDAL : GenericService<TourProduct>
    {
        private DbWorker _DbWorker;

        public TourProductDAL(string connection) : base(connection)
        {
            _DbWorker = new DbWorker(connection);

        }
        public async Task<TourProduct> GetTourProductById(long id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return await _DbContext.TourProduct.FindAsync(id);
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetTourProductById - TourProductDAL: " + ex);
                return null;
            }
        }
        public DataTable GetListTourProgramPackagesByTourProductId(long tour_product_id)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[]
                {
                    new SqlParameter("@TourProductId", tour_product_id),
                    new SqlParameter("@ClientType", DBNull.Value)
                };

                return _DbWorker.GetDataTable(StoreProcedureConstant.SP_GetListTourProgramPackagesByTourProductId, objParam);
            }
            catch
            {
                throw;
            }
        }
        public int CreateTourPackages(TourProgramPackages model)
        {
            try
            {

                SqlParameter[] objParam_order = new SqlParameter[]
                {

                    new SqlParameter("@TourProductId", model.TourProductId),
                    new SqlParameter("@FromDate", model.FromDate),
                    new SqlParameter("@ToDate", model.ToDate),
                    new SqlParameter("@IsDaily", model.IsDaily),
                    new SqlParameter("@AdultPrice", model.AdultPrice),
                    new SqlParameter("@ChildPrice", model.ChildPrice),
                    new SqlParameter("@ClientType", model.ClientType),
                    new SqlParameter("@CreatedBy", model.CreatedBy),
                    new SqlParameter("@CreatedDate", model.CreatedDate)
                };
                var id = _DbWorker.ExecuteNonQuery(StoreProcedureConstant.InsertTourProgramPackages, objParam_order);
                model.Id = id;
                return id;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CreateTourPackages - TourProductDAL. " + ex);

                return -1;
            }
        }
        public long DeleteTourProgramPackages(long id,int user_id)
        {
            try
            {

                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var exists = _DbContext.TourProgramPackages.FirstOrDefault(x => x.Id == id);
                    if(exists!=null && exists.Id > 0)
                    {
                        exists.TourProductId *= -1;
                        exists.UpdatedBy = user_id;
                        exists.UpdatedDate =DateTime.Now;
                        UpdateTourPackages(exists);
                    }
                }
                return id;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateTourPackages - TourDAL. " + ex);

                return -1;
            }
        }
        public int UpdateTourPackages(TourProgramPackages model)
        {
            try
            {

                SqlParameter[] objParam_order = new SqlParameter[]
                {
                    new SqlParameter("@Id", model.Id),
                    new SqlParameter("@TourProductId", model.TourProductId),
                    new SqlParameter("@FromDate", model.FromDate),
                    new SqlParameter("@ToDate", model.ToDate),
                    new SqlParameter("@IsDaily", model.IsDaily),
                    new SqlParameter("@AdultPrice", model.AdultPrice),
                    new SqlParameter("@ChildPrice", model.ChildPrice),
                    new SqlParameter("@ClientType", model.ClientType),
                    new SqlParameter("@UpdatedBy", model.UpdatedBy),

                };
                var id = _DbWorker.ExecuteNonQuery(StoreProcedureConstant.UpdateTourProgramPackages, objParam_order);
                return id;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateTourPackages - TourDAL. " + ex);

                return -1;
            }
        }
    }
}