using DAL.Generic;
using DAL.StoreProcedure;
using Entities.Models;
using Entities.ViewModels;
using Entities.ViewModels.Request;
using Microsoft.Data.SqlClient;
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
    public class RequestDAL : GenericService<Request>
    {
        private static DbWorker _DbWorker;
        public RequestDAL(string connection) : base(connection)
        {
            _DbWorker = new DbWorker(connection);

        }
        public async Task<DataTable> GetPagingList(RequestSearchModel searchModel)
        {
            try
            {

                SqlParameter[] objParam = new SqlParameter[7];
                objParam[0] = new SqlParameter("@RequestId", searchModel.RequestId);
                objParam[1] = new SqlParameter("@CreateDateFrom", DBNull.Value);
                objParam[2] = new SqlParameter("@CreateDateTo", DBNull.Value);
                objParam[3] = new SqlParameter("@SalerId", searchModel.SalerId);
                objParam[4] = new SqlParameter("@ClientId", searchModel.ClientId);
                objParam[5] = new SqlParameter("@PageIndex", searchModel.PageIndex);
                objParam[6] = new SqlParameter("@PageSize", searchModel.PageSize);

                return _DbWorker.GetDataTable(StoreProcedureConstant.SP_GetListRequest, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetPagingList - RequestDAL: " + ex);
            }
            return null;
        }     
        public async Task<long> UpdateRequest(Request Model)
        {
            try
            {

                SqlParameter[] objParam = new SqlParameter[12];
                objParam[0] = new SqlParameter("@RequestId", Model.RequestId);
                objParam[1] = new SqlParameter("@RoomTypeId", Model.RoomTypeId);
                objParam[2] = new SqlParameter("@PackageId", Model.PackageId);
                objParam[3] = new SqlParameter("@HotelId", Model.HotelId);
                objParam[4] = new SqlParameter("@FromDate", DBNull.Value);
                objParam[5] = new SqlParameter("@ToDate", DBNull.Value);
                objParam[6] = new SqlParameter("@Price", Model.Price);
                objParam[7] = new SqlParameter("@Note", Model.Note);
                objParam[8] = new SqlParameter("@Status", Model.Status);
                objParam[9] = new SqlParameter("@SalerId", Model.SalerId);
                objParam[10] = new SqlParameter("@OrderId", Model.OrderId);
                objParam[11] = new SqlParameter("@UpdatedBy", Model.UpdatedBy);

                return _DbWorker.ExecuteNonQuery(StoreProcedureConstant.sp_UpdateRequest, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateRequest - RequestDAL: " + ex);
            }
            return -1;
        }
        public async Task<Request> GetDetailRequest(long RequestId)
        {
            try
            {

                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@RequestId", RequestId);
                DataTable dt= _DbWorker.GetDataTable(StoreProcedureConstant.Sp_GetDetailRequest, objParam);
                if (dt != null && dt.Rows.Count > 0)
                {
                    var data = dt.ToList<Request>();
                    return data[0];
                }

                  
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateRequest - RequestDAL: " + ex);
            }
            return null;
        }
    }
}
