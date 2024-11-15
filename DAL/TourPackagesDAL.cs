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
    public class TourPackagesDAL : GenericService<TourPackages>
    {
        private DbWorker dbWorker;

        public TourPackagesDAL(string connection) : base(connection)
        {
            dbWorker = new DbWorker(connection);

        }
        public async Task<List<TourPackages>> GetTourPackagesByTourId(long tour_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return await _DbContext.TourPackages.Where(x => x.TourId == tour_id).ToListAsync();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetTourPackagesByTourId - TourPackagesDAL: " + ex.ToString());
                return null;
            }
        }
        public async Task<DataTable> ListTourPackagesByTourId(long TourId)
        {
            try
            {
                SqlParameter[] objParam_order = new SqlParameter[1];
                objParam_order[0] = new SqlParameter("@TourId", TourId);
               
                return dbWorker.GetDataTable(StoreProcedureConstant.SP_GetListTourPackagesByTourId, objParam_order);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetListTour - TourPackagesDAL: " + ex);
                return null;
            }
        }
        public async Task<int> UpdateTourPackages(TourPackages model)
        {
            try
            {
                SqlParameter[] objParam_order = new SqlParameter[13];
                objParam_order[0] = new SqlParameter("@TourId", model.TourId);
                objParam_order[1] = new SqlParameter("@PackageName", model.PackageName);
                objParam_order[2] = new SqlParameter("@BasePrice", model.BasePrice);
                objParam_order[3] = new SqlParameter("@Quantity", model.Quantity);

                objParam_order[4] = new SqlParameter("@AmountBeforeVat", model.AmountBeforeVat);
                objParam_order[5] = new SqlParameter("@AmountVat", model.AmountVat);
                objParam_order[6] = new SqlParameter("@Amount", model.Amount);
                objParam_order[7] = new SqlParameter("@VAT", model.Vat);

                objParam_order[8] = new SqlParameter("@UnitPrice", model.UnitPrice);

                objParam_order[9] = new SqlParameter("@UpdatedBy", model.UpdatedBy);
                objParam_order[10] = new SqlParameter("@PackageCode", model.PackageCode);
                objParam_order[11] = new SqlParameter("@Id", model.Id);
                objParam_order[12] = new SqlParameter("@Profit", model.Profit);


                var id = dbWorker.ExecuteNonQuery(StoreProcedureConstant.UpdateTourPackages, objParam_order);
                model.Id = id;
                return id;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateTourPackages - TourPackagesDAL. " + ex);
                return -1;
            }
        }
        public async Task<int> UpdateTourPackagesUnitPrice(TourPackages model)
        {
            try
            {
                SqlParameter[] objParam_order = new SqlParameter[13];
                objParam_order[0] = new SqlParameter("@TourId", DBNull.Value);
                objParam_order[1] = new SqlParameter("@PackageName", DBNull.Value);
                objParam_order[2] = new SqlParameter("@BasePrice", DBNull.Value);
                objParam_order[3] = new SqlParameter("@Quantity", DBNull.Value);

                objParam_order[4] = new SqlParameter("@AmountBeforeVat", DBNull.Value);
                objParam_order[5] = new SqlParameter("@AmountVat", DBNull.Value);
                objParam_order[6] = new SqlParameter("@Amount", DBNull.Value);
                objParam_order[7] = new SqlParameter("@VAT", DBNull.Value);

                objParam_order[8] = new SqlParameter("@UnitPrice", model.UnitPrice);

                objParam_order[9] = new SqlParameter("@UpdatedBy", model.UpdatedBy);
                objParam_order[10] = new SqlParameter("@PackageCode", DBNull.Value);
                objParam_order[11] = new SqlParameter("@Id", model.Id);
                objParam_order[12] = new SqlParameter("@Profit", DBNull.Value);


                var id = dbWorker.ExecuteNonQuery(StoreProcedureConstant.UpdateTourPackages, objParam_order);
                model.Id = id;
                return id;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateTourPackages - TourPackagesDAL. " + ex);
                return -1;
            }
        }
    }
}