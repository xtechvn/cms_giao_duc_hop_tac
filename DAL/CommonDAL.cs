using DAL.StoreProcedure;
using Entities.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace DAL
{
    public class CommonDAL
    {
        private readonly string _connection;
        private DbWorker dbWorker;
        public CommonDAL(string connection)
        {
            _connection = connection;
            dbWorker = new DbWorker(connection);
        }

        public async Task<List<National>> GetNationalList()
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return await _DbContext.National.AsNoTracking().ToListAsync();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetProvinceList - CommonDAL: " + ex);
                return null;
            }
        }

        public async Task<List<Province>> GetProvinceList()
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return await _DbContext.Province.AsNoTracking().ToListAsync();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetProvinceList - CommonDAL: " + ex);
                return null;
            }
        }
        public async Task<Province> GetProvinceDetail(long id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return await _DbContext.Province.Where(s => s.Id == id).FirstOrDefaultAsync();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetProvinceDetail - CommonDAL: " + ex);
                return null;
            }
        }
        public async Task<List<District>> District(string ProvinceId)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return await _DbContext.District.Where(s => s.ProvinceId == ProvinceId).ToListAsync();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetProvinceList - CommonDAL: " + ex);
                return null;
            }
        }
        public async Task<Supplier> GetSupplierById(int id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return await _DbContext.Supplier.FindAsync(id);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<District>> GetDistrictListByProvinceId(string provinceId)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return await _DbContext.District.Where(s => s.ProvinceId == provinceId).ToListAsync();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetDistrictListByProvinceId - CommonDAL: " + ex);
                return null;
            }
        }

        public async Task<List<Ward>> GetWardListByDistrictId(string districtId)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return await _DbContext.Ward.Where(s => s.DistrictId == districtId).ToListAsync();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetWardListByDistrictId - CommonDAL: " + ex);
                return null;
            }
        }

        public async Task<List<AllCode>> GetAllCodeListByType(string type)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return await _DbContext.AllCode.Where(s => s.Type == type).ToListAsync();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetAllCodeListByType - CommonDAL: " + ex);
                return null;
            }
        }

        public void InsertAttachFiles(IEnumerable<AttachFile> models)
        {
            try
            {
                dbWorker.ExecuteActionTransaction((connection, transaction) =>
                {
                    if (models != null && models.Any())
                    {
                        foreach (var model in models)
                        {
                            SqlCommand oCommand = new SqlCommand()
                            {
                                Connection = connection,
                                Transaction = transaction,
                                CommandType = CommandType.StoredProcedure
                            };

                            oCommand.CommandText = StoreProcedureConstant.SP_InsertAttachFile;
                            oCommand.Parameters.AddRange(new SqlParameter[] {
                                new SqlParameter("@DataId", model.DataId),
                                new SqlParameter("@Type", model.Type),
                                new SqlParameter("@UserId", model.UserId),
                                new SqlParameter("@Path", model.Path),
                                new SqlParameter("@Ext", model.Ext ?? (object)DBNull.Value),
                                new SqlParameter("@Capacity", model.Capacity ?? (object)DBNull.Value)
                            });

                            SqlParameter OuputParam = oCommand.Parameters.Add("@Identity", SqlDbType.Int);
                            OuputParam.Direction = ParameterDirection.Output;

                            oCommand.ExecuteNonQuery();
                        }
                    }
                });
            }
            catch
            {
                throw;
            }
        }

        public void DeleteAttachFilesByDataId(long dataId, int type)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[]
                {
                    new SqlParameter("@DataId",dataId),
                    new SqlParameter("@Type",type),
                };
                dbWorker.ExecuteNonQueryNoIdentity(StoreProcedureConstant.SP_DeleteAttachFile, objParam);
            }
            catch
            {
                throw;
            }
        }

        public DataTable GetAttachFilesByDataIdAndType(long dataId, int type)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[]
                {
                    new SqlParameter("@DataId",dataId),
                    new SqlParameter("@Type",type),
                };
                return dbWorker.GetDataTable(StoreProcedureConstant.SP_GetAttachFileByDataIdAndType, objParam);
            }
            catch
            {
                throw;
            }
        }

    }
}
