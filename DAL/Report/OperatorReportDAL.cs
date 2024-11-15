using DAL.Generic;
using DAL.StoreProcedure;
using Entities.ViewModels.Report;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Utilities;
using Utilities.Contants;

namespace DAL.Report
{
    public class OperatorReportDAL : GenericService<Entities.Models.Invoice>
    {
        private static DbWorker _DbWorker;
        public OperatorReportDAL(string connection) : base(connection)
        {
            _connection = connection;
            _DbWorker = new DbWorker(connection);
        }
        public DataTable GetOperatorReport(OperatorReportSearchModel searchModel, int currentPage, int pageSize)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[18];
                if (searchModel.FromDate != null)
                {
                    objParam[0] = new SqlParameter("@FromDate", searchModel.FromDate);
                }
                else
                {
                    objParam[0] = new SqlParameter("@FromDate", DBNull.Value);
                }

                if (searchModel.ToDate != null)
                {
                    objParam[1] = new SqlParameter("@ToDate", searchModel.ToDate);
                }
                else
                {
                    objParam[1] = new SqlParameter("@ToDate", DBNull.Value);
                }

                if (searchModel.StartDateFrom != null)
                {
                    objParam[2] = new SqlParameter("@StartDateFrom", searchModel.StartDateFrom);
                }
                else
                {
                    objParam[2] = new SqlParameter("@StartDateFrom", DBNull.Value);
                }

                if (searchModel.StartDateTo != null)
                {
                    objParam[3] = new SqlParameter("@StartDateTo", searchModel.StartDateTo);
                }
                else
                {
                    objParam[3] = new SqlParameter("@StartDateTo", DBNull.Value);
                }

                if (searchModel.EndDateFrom != null)
                {
                    objParam[4] = new SqlParameter("@EndDateFrom", searchModel.EndDateFrom);
                }
                else
                {
                    objParam[4] = new SqlParameter("@EndDateFrom", DBNull.Value);
                }

                if (searchModel.EndDateTo != null)
                {
                    objParam[5] = new SqlParameter("@EndDateTo", searchModel.EndDateTo);
                }
                else
                {
                    objParam[5] = new SqlParameter("@EndDateTo", DBNull.Value);
                }

                if (searchModel.OrderStatus != null)
                {
                    objParam[6] = new SqlParameter("@OrderStatus", searchModel.OrderStatus);
                }
                else
                {
                    objParam[6] = new SqlParameter("@OrderStatus", DBNull.Value);
                }
                if (searchModel.InvoiceStatus != null )
                {
                    objParam[7] = new SqlParameter("@InvoiceStatus", searchModel.InvoiceStatus);
                }
                else
                {
                    objParam[7] = new SqlParameter("@InvoiceStatus", DBNull.Value);
                }
                if (searchModel.ExportDateFrom != null)
                {
                    objParam[8] = new SqlParameter("@ExportDateFrom", searchModel.ExportDateFrom);
                }
                else
                {
                    objParam[8] = new SqlParameter("@ExportDateFrom", DBNull.Value);
                }
                if (searchModel.ExportDateTo != null)
                {
                    objParam[9] = new SqlParameter("@ExportDateTo", searchModel.ExportDateTo);
                }
                else
                {
                    objParam[9] = new SqlParameter("@ExportDateTo", DBNull.Value);
                }
                if (searchModel.SalerId != null && searchModel.SalerId > 0)
                {
                    objParam[10] = new SqlParameter("@SalerId", searchModel.SalerId);
                }
                else
                {
                    objParam[10] = new SqlParameter("@SalerId", DBNull.Value);
                }
                if (searchModel.SalerPermission != null)
                {
                    objParam[11] = new SqlParameter("@SalerPermission", searchModel.SalerPermission);
                }
                else
                {
                    objParam[11] = new SqlParameter("@SalerPermission", DBNull.Value);
                }
   
                if (pageSize == -1)
                {
                    objParam[12] = new SqlParameter("@PageIndex", -1);
                    objParam[13] = new SqlParameter("@PageSize", 20);
                }
                else
                {
                    objParam[12] = new SqlParameter("@PageIndex", currentPage);
                    objParam[13] = new SqlParameter("@PageSize", pageSize);
                }
                if (searchModel.DepartmentIdSearch != null&& searchModel.DepartmentIdSearch.Trim()!="")
                {
                    objParam[14] = new SqlParameter("@DepartmentId", searchModel.DepartmentIdSearch);
                }
                else
                {
                    objParam[14] = new SqlParameter("@DepartmentId", DBNull.Value);
                }
                if (searchModel.Branch != null && searchModel.Branch>0)
                {
                    objParam[15] = new SqlParameter("@Branch", searchModel.Branch);
                }
                else
                {
                    objParam[15] = new SqlParameter("@Branch", DBNull.Value);
                }
                if (searchModel.PaymentFromDate != null && searchModel.PaymentFromDate >DateTime.MinValue)
                {
                    objParam[16] = new SqlParameter("@FirstPayDateFrom", searchModel.PaymentFromDate); 
                }
                else
                {
                    objParam[16] = new SqlParameter("@FirstPayDateFrom",DBNull.Value);
                }
                if (searchModel.PaymentToDate != null && searchModel.PaymentToDate > DateTime.MinValue)
                {
                    objParam[17] = new SqlParameter("@FirstPayDateTo", searchModel.PaymentToDate);
                }
                else
                {
                    objParam[17] = new SqlParameter("@FirstPayDateTo", DBNull.Value);
                }
                return _DbWorker.GetDataTable(StoreProcedureConstant.Report_TotalRevenueByOrder, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetOperatorReport - OperatorReportDAL: " + ex);
            }
            return null;
        }
        public DataTable GetSumOperatorReport(OperatorReportSearchModel searchModel)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[13];
                if (searchModel.FromDate != null)
                {
                    objParam[0] = new SqlParameter("@FromDate", searchModel.FromDate);
                }
                else
                {
                    objParam[0] = new SqlParameter("@FromDate", DBNull.Value);
                }

                if (searchModel.ToDate != null)
                {
                    objParam[1] = new SqlParameter("@ToDate", searchModel.ToDate);
                }
                else
                {
                    objParam[1] = new SqlParameter("@ToDate", DBNull.Value);
                }

                if (searchModel.StartDateFrom != null)
                {
                    objParam[2] = new SqlParameter("@StartDateFrom", searchModel.StartDateFrom);
                }
                else
                {
                    objParam[2] = new SqlParameter("@StartDateFrom", DBNull.Value);
                }

                if (searchModel.StartDateTo != null)
                {
                    objParam[3] = new SqlParameter("@StartDateTo", searchModel.StartDateTo);
                }
                else
                {
                    objParam[3] = new SqlParameter("@StartDateTo", DBNull.Value);
                }

                if (searchModel.EndDateFrom != null)
                {
                    objParam[4] = new SqlParameter("@EndDateFrom", searchModel.EndDateFrom);
                }
                else
                {
                    objParam[4] = new SqlParameter("@EndDateFrom", DBNull.Value);
                }

                if (searchModel.EndDateTo != null)
                {
                    objParam[5] = new SqlParameter("@EndDateTo", searchModel.EndDateTo);
                }
                else
                {
                    objParam[5] = new SqlParameter("@EndDateTo", DBNull.Value);
                }

                if (searchModel.OrderStatus != null)
                {
                    objParam[6] = new SqlParameter("@OrderStatus", searchModel.OrderStatus);
                }
                else
                {
                    objParam[6] = new SqlParameter("@OrderStatus", DBNull.Value);
                }
                if (searchModel.InvoiceStatus != null)
                {
                    objParam[7] = new SqlParameter("@InvoiceStatus", searchModel.InvoiceStatus);
                }
                else
                {
                    objParam[7] = new SqlParameter("@InvoiceStatus", DBNull.Value);
                }
                if (searchModel.ExportDateFrom != null)
                {
                    objParam[8] = new SqlParameter("@ExportDateFrom", searchModel.ExportDateFrom);
                }
                else
                {
                    objParam[8] = new SqlParameter("@ExportDateFrom", DBNull.Value);
                }
                if (searchModel.ExportDateTo != null)
                {
                    objParam[9] = new SqlParameter("@ExportDateTo", searchModel.ExportDateTo);
                }
                else
                {
                    objParam[9] = new SqlParameter("@ExportDateTo", DBNull.Value);
                }
                if (searchModel.SalerId != null && searchModel.SalerId > 0)
                {
                    objParam[10] = new SqlParameter("@SalerId", searchModel.SalerId);
                }
                else
                {
                    objParam[10] = new SqlParameter("@SalerId", DBNull.Value);
                }
                if (searchModel.SalerPermission != null)
                {
                    objParam[11] = new SqlParameter("@SalerPermission", searchModel.SalerPermission);
                }
                else
                {
                    objParam[11] = new SqlParameter("@SalerPermission", DBNull.Value);
                }
                if (searchModel.DepartmentIdSearch != null && searchModel.DepartmentIdSearch.Trim() != "")
                {
                    objParam[12] = new SqlParameter("@DepartmentId", searchModel.DepartmentIdSearch);
                }
                else
                {
                    objParam[12] = new SqlParameter("@DepartmentId", DBNull.Value);
                }
                return _DbWorker.GetDataTable(StoreProcedureConstant.Report_SumTotalRevenueByOrder, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetSumOperatorReport - OperatorReportDAL: " + ex);
            }
            return null;
        }
        public DataTable GetTotalDebtRevenueByClient(ReportClientDebtSearchModel searchModel)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[6];
                if (searchModel.FromDate != null)
                {
                    objParam[0] = new SqlParameter("@FromDate", searchModel.FromDate);
                }
                else
                {
                    DateTime from_date = new DateTime(2020, 01, 01, 0, 0, 0);
                    objParam[0] = new SqlParameter("@FromDate", from_date);
                }

                if (searchModel.ToDate != null)
                {
                    objParam[1] = new SqlParameter("@ToDate", searchModel.ToDate);
                }
                else
                {
                    DateTime to_date = DateTime.Now;

                    objParam[1] = new SqlParameter("@ToDate", to_date);
                }

                if (searchModel.BranchCode != null && searchModel.BranchCode > 0)
                {
                    objParam[2] = new SqlParameter("@Branch", searchModel.BranchCode);
                }
                else
                {
                    objParam[2] = new SqlParameter("@Branch", DBNull.Value);
                }

                if (searchModel.ClientID != null && searchModel.ClientID > 0)
                {
                    objParam[3] = new SqlParameter("@ClientID", searchModel.ClientID);
                }
                else
                {
                    objParam[3] = new SqlParameter("@ClientID", DBNull.Value);
                }

                if (searchModel.PageIndex != null && searchModel.PageIndex > 0)
                {
                    objParam[4] = new SqlParameter("@PageIndex", searchModel.PageIndex);
                }
                else
                {
                    objParam[4] = new SqlParameter("@PageIndex", DBNull.Value);
                }

                if (searchModel.PageSize != null && searchModel.PageSize > 0)
                {
                    objParam[5] = new SqlParameter("@PageSize", searchModel.PageSize);
                }
                else
                {
                    objParam[5] = new SqlParameter("@PageSize", DBNull.Value);
                }

               
                return _DbWorker.GetDataTable(StoreProcedureConstant.SP_Report_TotalDebtRevenueByClient, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetTotalDebtRevenueByClient - OperatorReportDAL: " + ex);
            }
            return null;
        }
        public DataTable GetDetailDebtRevenueByClient(ReportDetailClientDebtSearchModel searchModel)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[5];
                if (searchModel.FromDate != null)
                {
                    objParam[0] = new SqlParameter("@FromDate", searchModel.FromDate);
                }
                else
                {
                    DateTime from_date = new DateTime(2020, 01, 01, 0, 0, 0);
                    objParam[0] = new SqlParameter("@FromDate", from_date);
                }

                if (searchModel.ToDate != null)
                {
                    objParam[1] = new SqlParameter("@ToDate", searchModel.ToDate);
                }
                else
                {
                    DateTime to_date = DateTime.Now;

                    objParam[1] = new SqlParameter("@ToDate", to_date);
                }

               
                if (searchModel.ClientID > 0)
                {
                    objParam[2] = new SqlParameter("@ClientID", searchModel.ClientID);
                }
                else
                {
                    objParam[2] = new SqlParameter("@ClientID", DBNull.Value);
                }

                if ( searchModel.PageIndex > 0)
                {
                    objParam[3] = new SqlParameter("@PageIndex", searchModel.PageIndex);
                }
                else
                {
                    objParam[3] = new SqlParameter("@PageIndex", DBNull.Value);
                }

                if ( searchModel.PageSize > 0)
                {
                    objParam[4] = new SqlParameter("@PageSize", searchModel.PageSize);
                }
                else
                {
                    objParam[4] = new SqlParameter("@PageSize", DBNull.Value);
                }


                return _DbWorker.GetDataTable(StoreProcedureConstant.SP_Report_DetailDebtRevenueByClientId, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetDetailDebtRevenueByClient - OperatorReportDAL: " + ex);
            }
            return null;
        }
    }
}
