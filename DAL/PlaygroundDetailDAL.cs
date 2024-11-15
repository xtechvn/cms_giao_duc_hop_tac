using DAL.Generic;
using DAL.StoreProcedure;
using Entities.Models;
using Entities.ViewModels.PlaygroundDetai;
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
  public  class PlaygroundDetailDAL : GenericService<PlaygroundDetail>
    {
        private static DbWorker _DbWorker;
        public PlaygroundDetailDAL(string connection) : base(connection)
        {
            _DbWorker = new DbWorker(connection);
        }

        
        public async Task<DataTable> GetListPlayground(PlaygroundDetaiSeachViewModel model)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[3];
                objParam[0] = new SqlParameter("@LocationName", model.LocationName);
                objParam[1] = new SqlParameter("@PageIndex", model.PageIndex);
                objParam[2] = new SqlParameter("@PageSize", model.PageSize);
                return _DbWorker.GetDataTable(StoreProcedureConstant.SP_GetListPlayGroundDetail, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("PlaygroundDetailDAL - GetListPlayground. " + ex);
                return null;
            }
        }
        public async Task<int> InsertPlaygroundDetail(PlaygroundDetaiViewModel model)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[6];
                objParam[0] = new SqlParameter("@Code", model.Code);
                objParam[1] = new SqlParameter("@ServiceType", model.ServiceType);
                objParam[2] = new SqlParameter("@Status", model.Status);
                objParam[3] = new SqlParameter("@NewsId", model.NewsId);
                objParam[4] = model.LocationName==null? new SqlParameter("@LocationName",DBNull.Value) : new SqlParameter("@LocationName", model.LocationName);
                objParam[5] = model.Description == null? new SqlParameter("@Description", DBNull.Value) : new SqlParameter("@Description", model.Description);
                return _DbWorker.ExecuteNonQuery(StoreProcedureConstant.SP_InsertPlaygroundDetail, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("PlaygroundDetailDAL - InsertPlaygroundDetail. " + ex);
                return 0;
            }
        }
        public async Task<int> UpdatePlaygroundDetail(PlaygroundDetaiViewModel model)
        { 
            try
            {
                SqlParameter[] objParam = new SqlParameter[7];
                objParam[0] = new SqlParameter("@Id", model.Id);
                objParam[1] = new SqlParameter("@Code", model.Code);
                objParam[2] = new SqlParameter("@ServiceType", model.ServiceType);
                objParam[3] = new SqlParameter("@NewsId", model.NewsId);
                objParam[4] = new SqlParameter("@Status", model.Status);
                objParam[5] = model.LocationName == null ? new SqlParameter("@LocationName", DBNull.Value) : new SqlParameter("@LocationName", model.LocationName);
                objParam[6] = model.Description == null ? new SqlParameter("@Description", DBNull.Value) : new SqlParameter("@Description", model.Description);
                return _DbWorker.ExecuteNonQuery(StoreProcedureConstant.SP_UpdatePlaygroundDetail, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("PlaygroundDetailDAL - UpdatePlaygroundDetail. " + ex);
                return 0;
            }
        }
        public async Task<DataTable> GetDetailPlaygroundDetail(long Playground)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@Playground", Playground);
                return _DbWorker.GetDataTable(StoreProcedureConstant.Sp_GetDetailPlayground, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("PlaygroundDetailDAL - GetDetailPlaygroundDetail. " + ex);
                return null;
            }
        }
        public async Task<List<PlaygroundDetail>> GetPlaygroundDetailbyCode(string Code)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var detail =_DbContext.PlaygroundDetail.AsNoTracking().Where(x => x.Code == Code).ToList();
                   
                    if (detail != null)
                    {
                        return detail;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetPlaygroundDetailbyCode - GetDetailPlaygroundDetail. " + ex);

            }
            return null;
        }
        public async Task<int>  DeletePlaygroundDetail(long id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var detail =await _DbContext.PlaygroundDetail.FirstOrDefaultAsync(x => x.Id == id);
                    _DbContext.PlaygroundDetail.Remove(detail);
                    await _DbContext.SaveChangesAsync();
                    if (detail != null)
                    {
                        return 1;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetPlaygroundDetailbyCode - GetDetailPlaygroundDetail. " + ex);

            }
            return 0;
        }
    }
}
