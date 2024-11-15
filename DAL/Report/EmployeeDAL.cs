using DAL.Generic;
using DAL.StoreProcedure;
using Entities.ViewModels.Report;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using Utilities;

namespace DAL.Report
{
    public class EmployeeDAL : GenericService<Entities.Models.Invoice>
    {
        private static DbWorker _DbWorker;
        public EmployeeDAL(string connection) : base(connection)
        {
            _connection = connection;
            _DbWorker = new DbWorker(connection);
        }

        public DataTable GetReportEmployeeRevenue(SearchReportEmployeeViewModel searchModel, int currentPage, int pageSize, string proc)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[14];
                //if (string.IsNullOrEmpty(searchModel.InvoiceCode))
                //    objParam[0] = new SqlParameter("@InvoiceCode", DBNull.Value);
                //else
                //    objParam[0] = new SqlParameter("@InvoiceCode", searchModel.InvoiceCode);
                //if (string.IsNullOrEmpty(searchModel.InvoiceNo))
                //    objParam[1] = new SqlParameter("@InvoiceNo", DBNull.Value);
                //else
                //    objParam[1] = new SqlParameter("@InvoiceNo", searchModel.InvoiceNo);
                //if (string.IsNullOrEmpty(searchModel.InvoiceRequestNo))
                //    objParam[2] = new SqlParameter("@InvoiceRequestNo", DBNull.Value);
                //else
                //    objParam[2] = new SqlParameter("@InvoiceRequestNo", searchModel.InvoiceRequestNo);
                //if (searchModel.ClientId == 0)
                //    objParam[3] = new SqlParameter("@ClientId", DBNull.Value);
                //else
                //    objParam[3] = new SqlParameter("@ClientId", searchModel.ClientId);
                if (pageSize == -1)
                {
                    objParam[11] = new SqlParameter("@PageIndex", -1);
                    objParam[12] = new SqlParameter("@PageSize", DBNull.Value);
                }
                else
                {
                    objParam[11] = new SqlParameter("@PageIndex", currentPage);
                    objParam[12] = new SqlParameter("@PageSize", pageSize);
                }
                return _DbWorker.GetDataTable(proc, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetReportEmployeeRevenue - InvoiceDAL: " + ex);
            }
            return null;
        }
    }
}
