using DAL.Generic;
using DAL.StoreProcedure;
using Entities.Models;
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
   public class TourPackagesOptionalDAL : GenericService<TourPackagesOptional>
    {

        private DbWorker dbWorker;

        public TourPackagesOptionalDAL(string connection) : base(connection)
        {
            dbWorker = new DbWorker(connection);

        }
        public async Task<DataTable> GetTourPackagesOptiona(long TourId)
        {
            try
            {
                SqlParameter[] objParam_order = new SqlParameter[1];
                objParam_order[0] = new SqlParameter("@TourId", TourId);
                return dbWorker.GetDataTable(StoreProcedureConstant.SP_GetListTourPackagesOptionalByTourId, objParam_order);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetTourPackagesOptiona - TourPackagesOptionalDAL: " + ex);
                return null;
            }
        }
        public async Task<int> InsertListTourPackagesOptional(List<TourPackages> model)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {

                    var data = _DbContext.TourPackagesOptional.FirstOrDefault(x => x.Id == model[0].TourId);
                    if(data!=null && data.Id > 0)
                    {
                        return -2;
                    }
                    else
                    {
                        foreach (var item in model)
                        {
                            var insert_item = new TourPackagesOptional()
                            {
                                CreatedBy = item.CreatedBy,
                                Amount = item.UnitPrice,
                                CreatedDate = item.CreatedDate,
                                Id = 0,
                                Packageid = 0,
                                PackageName = item.PackageName,
                                SupplierId = 0,
                                TourId = item.TourId,
                                UpdatedBy = item.UpdatedBy,
                                UpdatedDate = item.UpdatedDate
                            };
                            var id = await InsertTourPackagesOptional(insert_item);
                        }
                    }
                }
                return -1;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("InsertListTourPackagesOptional - TourPackagesOptionalDAL: " + ex);
                return -2;
            }
        }
        public async Task<int> InsertTourPackagesOptional(TourPackagesOptional model)
        {
            try
            {
                SqlParameter[] objParam_order = new SqlParameter[12];
                objParam_order[0] = model.TourId == null ? new SqlParameter("@TourId", DBNull.Value) : new SqlParameter("@TourId", model.TourId);
                objParam_order[1] = model.SupplierId == null ? new SqlParameter("@SupplierId", DBNull.Value) : new SqlParameter("@SupplierId", model.SupplierId);
                objParam_order[2] = model.Packageid == null ? new SqlParameter("@PackageId", DBNull.Value) : new SqlParameter("@PackageId", model.Packageid);
                objParam_order[3] = model.Amount == null ? new SqlParameter("@Amount", DBNull.Value) : new SqlParameter("@Amount", model.Amount);
                objParam_order[4] = model.CreatedBy == null ? new SqlParameter("@NoCreatedByte", DBNull.Value) : new SqlParameter("@CreatedBy", model.CreatedBy);
                objParam_order[5] = model.CreatedDate == null ? new SqlParameter("@CreatedDate", DBNull.Value) : new SqlParameter("@CreatedDate",DBNull.Value);
                objParam_order[6] = model.PackageName == null ? new SqlParameter("@PackageName", DBNull.Value) : new SqlParameter("@PackageName", model.PackageName);
                objParam_order[7] = model.Quantity == null ? new SqlParameter("@Quantity", DBNull.Value) : new SqlParameter("@Quantity", model.Quantity);
                objParam_order[8] = model.Times == null ? new SqlParameter("@Times", DBNull.Value) : new SqlParameter("@Times", model.Times);
                objParam_order[9] = model.Price == null ? new SqlParameter("@Price", DBNull.Value) : new SqlParameter("@Price", model.Price);
                objParam_order[10] = model.Note == null ? new SqlParameter("@Note", DBNull.Value) : new SqlParameter("@Note", model.Note);
                objParam_order[11] = model.Status == null ? new SqlParameter("@Status", DBNull.Value) : new SqlParameter("@Status", model.Status);
                return dbWorker.ExecuteNonQuery(StoreProcedureConstant.SP_InsertTourPackagesOptional, objParam_order);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetTourPackagesOptiona - TourPackagesOptionalDAL: " + ex);
                return 0;
            }
        }
        public async Task<int> UpdateTourPackagesOptional(TourPackagesOptional model)
        {
            try
            {
                SqlParameter[] objParam_order = new SqlParameter[11];
                objParam_order[0] = model.TourId==null ? new SqlParameter("@TourId", DBNull.Value): new SqlParameter("@TourId", model.TourId);
                objParam_order[1] = model.SupplierId == null ? new SqlParameter("@SupplierId", DBNull.Value) : new SqlParameter("@SupplierId", model.SupplierId);
                objParam_order[2] = model.Packageid == null ? new SqlParameter("@PackageId", DBNull.Value) : new SqlParameter("@PackageId", model.Packageid);
                objParam_order[3] = model.Amount  == null ? new SqlParameter("@Amount", DBNull.Value) :new SqlParameter("@Amount", model.Amount);
                objParam_order[4] = model.UpdatedBy == null ? new SqlParameter("@UpdatedBy", DBNull.Value) : new SqlParameter("@UpdatedBy", model.UpdatedBy);
                objParam_order[5] = new SqlParameter("@Id", model.Id);
                objParam_order[6] = model.Quantity == null ? new SqlParameter("@Quantity", DBNull.Value) : new SqlParameter("@Quantity", model.Quantity);
                objParam_order[7] = model.Times == null ? new SqlParameter("@Times", DBNull.Value) : new SqlParameter("@Times", model.Times);
                objParam_order[8] = model.Price == null ? new SqlParameter("@Price", DBNull.Value) : new SqlParameter("@Price", model.Price);
                objParam_order[9] = model.Note == null ? new SqlParameter("@Note", DBNull.Value) : new SqlParameter("@Note", model.Note);
                objParam_order[10] = model.Status == null ? new SqlParameter("@Status", DBNull.Value) : new SqlParameter("@Status", model.Status);
                return dbWorker.ExecuteNonQuery(StoreProcedureConstant.SP_UpdateTourPackagesOptional, objParam_order);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateTourPackagesOptional - TourPackagesOptionalDAL: " + ex);
                return 0;
            }
        }
        public async Task<int> DeleteTourPackagesOptional(int Id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {

                    var data = _DbContext.TourPackagesOptional.FirstOrDefault(x => x.Id == Id);

                    if(data!=null && data.Id > 0)
                    {
                        var data2 = _DbContext.TourPackagesOptional.Remove(data);
                        _DbContext.SaveChanges();
                    }
                    return 1;

                }


            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateTourPackagesOptional - TourPackagesOptionalDAL: " + ex);
                return 0;
            }
        }
        public async Task<List<TourPackagesOptional>> GetTourOptionalByTourId(long booking_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return await _DbContext.TourPackagesOptional.AsNoTracking().Where(s => s.TourId == booking_id).ToListAsync();

                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetTourOptionalByTourId - TourPackagesOptionalDAL. " + ex);
                return null;
            }
        }
    }
}
