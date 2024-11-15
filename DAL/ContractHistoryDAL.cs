using DAL.Generic;
using DAL.StoreProcedure;
using Entities.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace DAL
{
   public class ContractHistoryDAL: GenericService<ContractHistory>
    {

        private static DbWorker _DbWorker;
        public ContractHistoryDAL(string connection) : base(connection)
        {
            _DbWorker = new DbWorker(connection);
        }
        public async Task<long> InsertContractHistory(ContractHistory model)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[6];
                objParam[0] = new SqlParameter("@ContractId", model.ContractId);
                objParam[1] = new SqlParameter("@Action", model.Action);
                objParam[2] = new SqlParameter("@ActionBy", model.ActionBy);
                objParam[3] = new SqlParameter("@ActionDate", model.ActionDate);
                objParam[4] = new SqlParameter("@CreatedBy", model.CreatedBy);
                objParam[5] = new SqlParameter("@CreatedDate", DBNull.Value);


                return _DbWorker.ExecuteNonQuery(StoreProcedureConstant.SP_InsertContractHistory, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("InsertContractHistory - ContractHistoryDAL. " + ex);
                return 0;
            }
        }
        public async Task<DataTable> GetContractHistory(long ContractId,long ActionBy)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[2];
                objParam[0] = new SqlParameter("@ContractId",ContractId);
                objParam[1] = new SqlParameter("@ActionBy", ActionBy);
                return _DbWorker.GetDataTable(StoreProcedureConstant.sp_GetDetailContractHistory, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetContractHistory - ContractHistoryDAL. " + ex);
                return null;
            }
        }
    }
}
