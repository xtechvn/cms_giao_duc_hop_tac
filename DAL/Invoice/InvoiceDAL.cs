using DAL.Generic;
using DAL.StoreProcedure;
using Entities.Models;
using Entities.ViewModels.Invoice;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utilities;
using Utilities.Contants;
namespace DAL.Invoice
{
    public class InvoiceDAL : GenericService<Entities.Models.Invoice>
    {
        private static DbWorker _DbWorker;
        public InvoiceDAL(string connection) : base(connection)
        {
            _connection = connection;
            _DbWorker = new DbWorker(connection);
        }

        public Entities.Models.Invoice GetById(long invoiceId)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var detail = _DbContext.Invoice.AsNoTracking().FirstOrDefault(x => x.Id == invoiceId);
                    if (detail != null)
                    {
                        return detail;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetById - InvoiceDAL: " + ex);
                return null;
            }
        }

        public Entities.Models.Invoice GetByRequestNo(string invoiceCode)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var detail = _DbContext.Invoice.AsNoTracking().FirstOrDefault(x => x.InvoiceCode == invoiceCode);
                    if (detail != null)
                    {
                        return detail;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetByRequestNo - InvoiceDAL: " + ex);
                return null;
            }
        }

        public DataTable GetPagingList(InvoiceSearchModel searchModel, int currentPage, int pageSize, string proc)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[15];
                if (string.IsNullOrEmpty(searchModel.InvoiceCode))
                    objParam[0] = new SqlParameter("@InvoiceCode", DBNull.Value);
                else
                    objParam[0] = new SqlParameter("@InvoiceCode", searchModel.InvoiceCode);
                if (string.IsNullOrEmpty(searchModel.InvoiceNo))
                    objParam[1] = new SqlParameter("@InvoiceNo", DBNull.Value);
                else
                    objParam[1] = new SqlParameter("@InvoiceNo", searchModel.InvoiceNo);
                if (string.IsNullOrEmpty(searchModel.InvoiceRequestNo))
                    objParam[2] = new SqlParameter("@InvoiceRequestNo", DBNull.Value);
                else
                    objParam[2] = new SqlParameter("@InvoiceRequestNo", searchModel.InvoiceRequestNo);
                if (searchModel.ClientId == 0)
                    objParam[3] = new SqlParameter("@ClientId", DBNull.Value);
                else
                    objParam[3] = new SqlParameter("@ClientId", searchModel.ClientId);
                if (searchModel.VerifyByIds == null || searchModel.VerifyByIds.Count == 0)
                {
                    objParam[4] = new SqlParameter("@UserVerify", DBNull.Value);
                }
                else
                {
                    objParam[4] = new SqlParameter("@UserVerify", string.Join(",", searchModel.VerifyByIds));
                }
                if (searchModel.ExportDateFrom == null)
                    objParam[5] = new SqlParameter("@ExportDateFrom", DBNull.Value);
                else
                    objParam[5] = new SqlParameter("@ExportDateFrom", searchModel.ExportDateFrom);
                if (searchModel.ExportDateTo == null)
                    objParam[6] = new SqlParameter("@ExportDateTo", DBNull.Value);
                else
                    objParam[6] = new SqlParameter("@ExportDateTo", searchModel.ExportDateTo);
                if (searchModel.CreateDateFrom == null)
                    objParam[7] = new SqlParameter("@CreateDateFrom", DBNull.Value);
                else
                    objParam[7] = new SqlParameter("@CreateDateFrom", searchModel.CreateDateFrom);
                if (searchModel.CreateDateTo == null)
                    objParam[8] = new SqlParameter("@CreateDateTo", DBNull.Value);
                else
                    objParam[8] = new SqlParameter("@CreateDateTo", searchModel.CreateDateTo);
                if (searchModel.VerifyDateFrom == null)
                    objParam[9] = new SqlParameter("@VerifyDateFrom", DBNull.Value);
                else
                    objParam[9] = new SqlParameter("@VerifyDateFrom", searchModel.VerifyDateFrom);
                if (searchModel.VerifyDateTo == null)
                    objParam[10] = new SqlParameter("@VerifyDateTo", DBNull.Value);
                else
                    objParam[10] = new SqlParameter("@VerifyDateTo", searchModel.VerifyDateTo);
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

                if (searchModel.CreateByIds == null || searchModel.CreateByIds.Count == 0)
                {
                    objParam[13] = new SqlParameter("@UserCreate", DBNull.Value);
                }
                else
                {
                    objParam[13] = new SqlParameter("@UserCreate", string.Join(",", searchModel.CreateByIds));
                }
                if (string.IsNullOrEmpty(searchModel.OrderNo))
                    objParam[14] = new SqlParameter("@OrderNo", DBNull.Value);
                else
                    objParam[14] = new SqlParameter("@OrderNo", searchModel.OrderNo);
                return _DbWorker.GetDataTable(proc, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetPagingList - InvoiceDAL: " + ex);
            }
            return null;
        }

        public int CreateInvoice(InvoiceViewModel model)
        {
            int invoiceId = 0;
            int invoiceFormNoId = 0;
            int invoiceSignId = 0;
            List<int> detailIds = new List<int>();
            bool isSuccess = true;
            try
            {
                SqlParameter[] objParam_Invoice = new SqlParameter[16];
                objParam_Invoice[0] = new SqlParameter("@PayType", model.PayType);
                objParam_Invoice[1] = new SqlParameter("@ClientId", model.ClientId);
                objParam_Invoice[2] = new SqlParameter("@InvoiceFromId", string.IsNullOrEmpty(model.InvoiceFromId) ?
                    DBNull.Value.ToString() : model.InvoiceFromId);
                objParam_Invoice[3] = new SqlParameter("@InvoiceSignId", string.IsNullOrEmpty(model.InvoiceSignId) ?
                    DBNull.Value.ToString() : model.InvoiceSignId);
                objParam_Invoice[4] = new SqlParameter("@InvoiceCode", string.IsNullOrEmpty(model.InvoiceCode) ?
                    DBNull.Value.ToString() : model.InvoiceCode);
                objParam_Invoice[5] = new SqlParameter("@InvoiceNo", string.IsNullOrEmpty(model.InvoiceNo) ?
                   DBNull.Value.ToString() : model.InvoiceNo);
                if (model.BankingAccountId != 0)
                    objParam_Invoice[6] = new SqlParameter("@BankingAccountId", model.BankingAccountId);
                else
                    objParam_Invoice[6] = new SqlParameter("@BankingAccountId", DBNull.Value);
                if (model.ExportDate != null)
                    objParam_Invoice[7] = new SqlParameter("@ExportDate", model.ExportDate);
                else
                    objParam_Invoice[7] = new SqlParameter("@ExportDate", DBNull.Value);
                objParam_Invoice[8] = new SqlParameter("@UserVerify", DBNull.Value);
                objParam_Invoice[9] = new SqlParameter("@VerifyDate", DBNull.Value);
                objParam_Invoice[10] = new SqlParameter("@IsDelete ", false);
                objParam_Invoice[11] = new SqlParameter("@Status", model.Status);
                objParam_Invoice[12] = new SqlParameter("@CreatedBy", model.CreatedBy);
                objParam_Invoice[13] = new SqlParameter("@CreatedDate", DateTime.Now);
                objParam_Invoice[14] = new SqlParameter("@Note", string.IsNullOrEmpty(model.Note) ?
                    DBNull.Value.ToString() : model.Note);
                objParam_Invoice[15] = new SqlParameter("@AttactFile", string.IsNullOrEmpty(model.AttactFile) ?
                    DBNull.Value.ToString() : model.AttactFile);
                invoiceId = _DbWorker.ExecuteNonQuery(StoreProcedureConstant.sp_InsertInvoice, objParam_Invoice);
                if (invoiceId > 0 && model.InvoiceDetails != null)
                {
                    foreach (var item in model.InvoiceDetails)
                    {
                        var detailId = 0;
                        SqlParameter[] objParam_detail = new SqlParameter[4];
                        objParam_detail[0] = new SqlParameter("@InvoiceId", invoiceId);
                        objParam_detail[1] = new SqlParameter("@InvoiceRequestId", item.Id);
                        objParam_detail[2] = new SqlParameter("@CreatedBy", model.CreatedBy);
                        objParam_detail[3] = new SqlParameter("@CreatedDate", DateTime.Now);
                        detailId = _DbWorker.ExecuteNonQuery(StoreProcedureConstant.sp_InsertInvoiceDetail, objParam_detail);
                        if (detailId > 0)
                            detailIds.Add(detailId);
                        else
                            isSuccess = false;
                        if (detailId <= 0)
                        {
                            DeleteInvoiceAndDetail(invoiceId, detailIds, invoiceFormNoId, invoiceSignId);
                            return -1;
                        }
                    }
                }
                else
                {
                    isSuccess = false;
                }
                if (isSuccess)
                {
                    using (var _DbContext = new EntityDataContext(_connection))
                    {
                        foreach (var item in model.InvoiceDetails)
                        {
                            var entity = _DbContext.InvoiceRequest.Find(item.Id);
                            entity.Status = (int)INVOICE_REQUEST_STATUS.HOAN_THANH;
                            entity.UpdatedDate = DateTime.Now;
                            entity.UpdatedBy = model.CreatedBy;
                            _DbContext.InvoiceRequest.Update(entity);
                            _DbContext.SaveChanges();
                        }
                    }
                }
                return invoiceId;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CreateRequest - InvoiceDAL. " + ex);
                DeleteInvoiceAndDetail(invoiceId, detailIds, invoiceFormNoId, invoiceSignId);
                return -1;
            }
        }

        private void DeleteInvoiceAndDetail(int invoiceId, List<int> detailIds, int invoiceFromId, int invoiceSignId)
        {
            using (var _DbContext = new EntityDataContext(_connection))
            {
                var entity = _DbContext.Invoice.Find(invoiceId);
                _DbContext.Invoice.Remove(entity);
                var invoiceFormNo = _DbContext.InvoiceFormNo.Find(invoiceFromId);
                _DbContext.InvoiceFormNo.Remove(invoiceFormNo);
                var invoiceSign = _DbContext.InvoiceSign.Find(invoiceSignId);
                _DbContext.InvoiceSign.Remove(invoiceSign);
                foreach (var idDetail in detailIds)
                {
                    var detail = _DbContext.InvoiceDetail.Find(idDetail);
                    _DbContext.InvoiceDetail.Remove(detail);
                }
                _DbContext.SaveChanges();
            }
        }

        public int UpdateInvoice(InvoiceViewModel model)
        {
            int invoiceId = 0;
            bool isSuccess = true;
            List<int> detailIds = new List<int>();
            try
            {
                SqlParameter[] objParam_Invoice = new SqlParameter[16];
                objParam_Invoice[0] = new SqlParameter("@PayType", model.PayType);
                objParam_Invoice[1] = new SqlParameter("@ClientId", model.ClientId);
                objParam_Invoice[2] = new SqlParameter("@InvoiceFromId", string.IsNullOrEmpty(model.InvoiceFromId) ?
                  DBNull.Value.ToString() : model.InvoiceFromId);
                objParam_Invoice[3] = new SqlParameter("@InvoiceSignId", string.IsNullOrEmpty(model.InvoiceSignId) ?
                    DBNull.Value.ToString() : model.InvoiceSignId);
                objParam_Invoice[4] = new SqlParameter("@InvoiceCode", string.IsNullOrEmpty(model.InvoiceCode) ?
                    DBNull.Value.ToString() : model.InvoiceCode);
                objParam_Invoice[5] = new SqlParameter("@InvoiceNo", string.IsNullOrEmpty(model.InvoiceNo) ?
                   DBNull.Value.ToString() : model.InvoiceNo);
                if (model.BankingAccountId != 0)
                    objParam_Invoice[6] = new SqlParameter("@BankingAccountId", model.BankingAccountId);
                else
                    objParam_Invoice[6] = new SqlParameter("@BankingAccountId", DBNull.Value);
                if (model.ExportDate != null)
                    objParam_Invoice[7] = new SqlParameter("@ExportDate", model.ExportDate);
                else
                    objParam_Invoice[7] = new SqlParameter("@ExportDate", DBNull.Value);
                objParam_Invoice[8] = new SqlParameter("@UserVerify", DBNull.Value);
                objParam_Invoice[9] = new SqlParameter("@VerifyDate", DBNull.Value);
                objParam_Invoice[10] = new SqlParameter("@IsDelete ", false);
                objParam_Invoice[11] = new SqlParameter("@Status", model.Status);
                objParam_Invoice[12] = new SqlParameter("@UpdatedBy", model.UpdatedBy);
                objParam_Invoice[13] = new SqlParameter("@Id ", model.Id);
                objParam_Invoice[14] = new SqlParameter("@Note", string.IsNullOrEmpty(model.Note) ?
                   DBNull.Value.ToString() : model.Note);
                objParam_Invoice[15] = new SqlParameter("@AttactFile", string.IsNullOrEmpty(model.AttactFile) ?
                    DBNull.Value.ToString() : model.AttactFile);
                invoiceId = _DbWorker.ExecuteNonQuery(StoreProcedureConstant.sp_UpdateInvoice, objParam_Invoice);
                if (invoiceId > 0 && model.InvoiceDetails != null)
                {
                    foreach (var item in model.InvoiceDetails)
                    {
                        var detailId = 0;
                        if (item.InvoiceDetailId == 0)
                        {
                            SqlParameter[] objParam_Detail = new SqlParameter[4];
                            objParam_Detail[0] = new SqlParameter("@InvoiceId", invoiceId);
                            objParam_Detail[1] = new SqlParameter("@InvoiceRequestId", item.Id);
                            objParam_Detail[2] = new SqlParameter("@CreatedBy", model.UpdatedBy);
                            objParam_Detail[3] = new SqlParameter("@CreatedDate", DateTime.Now);
                            detailId = _DbWorker.ExecuteNonQuery(StoreProcedureConstant.sp_InsertInvoiceDetail, objParam_Detail);
                            if (detailId > 0)
                                detailIds.Add(detailId);
                            else
                                isSuccess = false;
                        }
                        else
                        {
                            detailIds.Add((int)item.InvoiceDetailId);
                        }
                    }
                }
                else
                {
                    isSuccess = false;
                }
                var request = GetById(invoiceId);
                var listDetail = GetByInvoiceId(model.Id);
                foreach (var item in listDetail)
                {
                    var exists = detailIds.FirstOrDefault(n => n == item.Id);
                    if (exists == 0)
                    {
                        DeleteInvoiceRequestDetail(item);
                    }
                }
                if (isSuccess)
                {
                    using (var _DbContext = new EntityDataContext(_connection))
                    {
                        foreach (var item in model.InvoiceDetails)
                        {
                            var entity = _DbContext.InvoiceRequest.Find(item.Id);
                            entity.Status = (int)INVOICE_REQUEST_STATUS.HOAN_THANH;
                            entity.UpdatedDate = DateTime.Now;
                            entity.UpdatedBy = model.UpdatedBy;
                            _DbContext.InvoiceRequest.Update(entity);
                            _DbContext.SaveChanges();
                        }
                    }
                }
                return invoiceId;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateInvoiceRequest - InvoiceDAL. " + ex);
                return -1;
            }
        }

        public int DeleteInvoiceRequestDetail(InvoiceDetail model)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    _DbContext.InvoiceDetail.Remove(model);
                    var entity = _DbContext.InvoiceRequest.Find(model.InvoiceRequestId);
                    if (entity != null)
                    {
                        entity.Status = (int)INVOICE_REQUEST_STATUS.DA_DUYET;
                        entity.UpdatedDate = DateTime.Now;
                        entity.UpdatedBy = model.UpdatedBy;
                        _DbContext.InvoiceRequest.Update(entity);
                    }
                    _DbContext.SaveChanges();
                    return 1;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("DeleteInvoiceRequestDetail - InvoiceDAL: " + ex);
                return -1;
            }
        }

        public List<InvoiceDetail> GetByInvoiceId(long invoiceId)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var details = _DbContext.InvoiceDetail.Where(x => x.InvoiceId == invoiceId).ToList();
                    if (details != null)
                    {
                        return details;
                    }
                }
                return new List<InvoiceDetail>();
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetByRequestId - InvoiceDAL: " + ex);
                return new List<InvoiceDetail>();
            }
        }

        public DataTable GetInvoiceInfo(long invoiceId)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@InvoiceId", invoiceId);
                return _DbWorker.GetDataTable(StoreProcedureConstant.sp_GetDetailInvoice, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetInvoiceInfo - InvoiceDAL: " + ex);
            }
            return null;
        }

        public int DeleteInvoice(long invoiceId, int userDelete)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var model = _DbContext.Invoice.Find(invoiceId);
                    model.IsDelete = true;
                    model.UpdatedDate = DateTime.Now;
                    model.UpdatedBy = userDelete;
                    _DbContext.Invoice.Update(model);
                    var listDetail = _DbContext.InvoiceDetail.Where(n => n.InvoiceId == invoiceId).ToList();
                    var requestIds = listDetail.Select(n => n.InvoiceRequestId).ToList();
                    var listInvoiceRequest = _DbContext.InvoiceRequest.Where(n => requestIds.Contains(n.Id)).ToList();
                    foreach (var item in listInvoiceRequest)
                    {
                        item.Status = (int)INVOICE_REQUEST_STATUS.DA_DUYET;
                    }
                    _DbContext.SaveChanges();
                }
                return 1;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("DeleteInvoice - InvoiceDAL. " + ex);
                return -1;
            }
        }
        public DataTable GetListInvoiceCodebyOrderId(string order_ids)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@OrderId", order_ids);
                return _DbWorker.GetDataTable(StoreProcedureConstant.GetListInvoiceCodebyOrderId, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetInvoiceInfo - InvoiceDAL: " + ex);
            }
            return null;
        }
    }
}
