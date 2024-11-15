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
    public class InvoiceRequestDAL : GenericService<InvoiceRequest>
    {
        private static DbWorker _DbWorker;
        public InvoiceRequestDAL(string connection) : base(connection)
        {
            _connection = connection;
            _DbWorker = new DbWorker(connection);
        }

        public InvoiceRequest GetById(long InvoiceRequestId)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var detail = _DbContext.InvoiceRequest.AsNoTracking().FirstOrDefault(x => x.Id == InvoiceRequestId);
                    if (detail != null)
                    {
                        return detail;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetById - InvoiceRequestDAL: " + ex);
                return null;
            }
        }

        public InvoiceRequest GetByRequestNo(string InvoiceRequestNo)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var detail = _DbContext.InvoiceRequest.AsNoTracking().FirstOrDefault(x => x.InvoiceRequestNo == InvoiceRequestNo);
                    if (detail != null)
                    {
                        return detail;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetById - InvoiceRequestDAL: " + ex);
                return null;
            }
        }

        public DataTable GetPagingList(InvoiceRequestSearchModel searchModel, int currentPage, int pageSize, string proc)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[18];
                if (string.IsNullOrEmpty(searchModel.InvoiceRequestNo))
                    objParam[0] = new SqlParameter("@InvoiceRequestNo", DBNull.Value);
                else
                    objParam[0] = new SqlParameter("@InvoiceRequestNo", searchModel.InvoiceRequestNo);
                if (string.IsNullOrEmpty(searchModel.InvoiceNo))
                    objParam[1] = new SqlParameter("@InvoiceNo", DBNull.Value);
                else
                    objParam[1] = new SqlParameter("@InvoiceNo", searchModel.InvoiceNo);
                if (searchModel.ClientId == 0)
                    objParam[2] = new SqlParameter("@ClientId", DBNull.Value);
                else
                    objParam[2] = new SqlParameter("@ClientId", searchModel.ClientId);
                if (searchModel.PlanDateFrom == null)
                    objParam[3] = new SqlParameter("@PlanDateFrom", DBNull.Value);
                else
                    objParam[3] = new SqlParameter("@PlanDateFrom", searchModel.PlanDateFrom);
                if (searchModel.PlanDateTo == null)
                    objParam[4] = new SqlParameter("@PlanDateTo", DBNull.Value);
                else
                    objParam[4] = new SqlParameter("@PlanDateTo", searchModel.PlanDateTo);
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
                if (searchModel.IsHasBill == null)
                    objParam[13] = new SqlParameter("@IsHasBill", DBNull.Value);
                else
                    objParam[13] = new SqlParameter("@IsHasBill", searchModel.IsHasBill);
                if (searchModel.StatusMulti != null && searchModel.StatusMulti.Count > 0)
                    objParam[14] = new SqlParameter("@InvoiceRequestStatus", string.Join(",", searchModel.StatusMulti));
                else
                    objParam[14] = new SqlParameter("@InvoiceRequestStatus", DBNull.Value);

                if (searchModel.VerifyByIds == null || searchModel.VerifyByIds.Count == 0)
                {
                    objParam[15] = new SqlParameter("@UserVerify", DBNull.Value);
                }
                else
                {
                    objParam[15] = new SqlParameter("@UserVerify", string.Join(",", searchModel.VerifyByIds));
                }
                if (searchModel.CreateByIds == null || searchModel.CreateByIds.Count == 0)
                {
                    objParam[16] = new SqlParameter("@UserCreate", DBNull.Value);
                }
                else
                {
                    objParam[16] = new SqlParameter("@UserCreate", string.Join(",", searchModel.CreateByIds));
                }
                if (string.IsNullOrEmpty(searchModel.InvoiceCode))
                    objParam[17] = new SqlParameter("@InvoiceCode", DBNull.Value);
                else
                    objParam[17] = new SqlParameter("@InvoiceCode", searchModel.InvoiceCode);
                return _DbWorker.GetDataTable(proc, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetPagingList - InvoiceRequestDAL: " + ex);
            }
            return null;
        }

        public DataTable GetCountStatus(InvoiceRequestSearchModel searchModel, string proc)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[16];
                if (string.IsNullOrEmpty(searchModel.InvoiceRequestNo))
                    objParam[0] = new SqlParameter("@InvoiceRequestNo", DBNull.Value);
                else
                    objParam[0] = new SqlParameter("@InvoiceRequestNo", searchModel.InvoiceRequestNo);
                if (string.IsNullOrEmpty(searchModel.InvoiceNo))
                    objParam[1] = new SqlParameter("@InvoiceNo", DBNull.Value);
                else
                    objParam[1] = new SqlParameter("@InvoiceNo", searchModel.InvoiceNo);
                if (searchModel.ClientId == 0)
                    objParam[2] = new SqlParameter("@ClientId", DBNull.Value);
                else
                    objParam[2] = new SqlParameter("@ClientId", searchModel.ClientId);
                if (searchModel.PlanDateFrom == null)
                    objParam[3] = new SqlParameter("@PlanDateFrom", DBNull.Value);
                else
                    objParam[3] = new SqlParameter("@PlanDateFrom", searchModel.PlanDateFrom);
                if (searchModel.PlanDateTo == null)
                    objParam[4] = new SqlParameter("@PlanDateTo", DBNull.Value);
                else
                    objParam[4] = new SqlParameter("@PlanDateTo", searchModel.PlanDateTo);
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
                if (searchModel.IsHasBill == null)
                    objParam[11] = new SqlParameter("@IsHasBill", DBNull.Value);
                else
                    objParam[11] = new SqlParameter("@IsHasBill", searchModel.IsHasBill);
                if (searchModel.StatusMulti != null && searchModel.StatusMulti.Count > 0)
                    objParam[12] = new SqlParameter("@InvoiceRequestStatus", string.Join(",", searchModel.StatusMulti));
                else
                    objParam[12] = new SqlParameter("@InvoiceRequestStatus", DBNull.Value);
                if (searchModel.VerifyByIds == null || searchModel.VerifyByIds.Count == 0)
                {
                    objParam[13] = new SqlParameter("@UserVerify", DBNull.Value);
                }
                else
                {
                    objParam[13] = new SqlParameter("@UserVerify", string.Join(",", searchModel.VerifyByIds));
                }
                if (searchModel.CreateByIds == null || searchModel.CreateByIds.Count == 0)
                {
                    objParam[14] = new SqlParameter("@UserCreate", DBNull.Value);
                }
                else
                {
                    objParam[14] = new SqlParameter("@UserCreate", string.Join(",", searchModel.CreateByIds));
                }
                if (string.IsNullOrEmpty(searchModel.InvoiceCode))
                    objParam[15] = new SqlParameter("@InvoiceCode", DBNull.Value);
                else
                    objParam[15] = new SqlParameter("@InvoiceCode", searchModel.InvoiceCode);
                return _DbWorker.GetDataTable(proc, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetCountStatus - InvoiceRequestDAL: " + ex);
            }
            return null;
        }

        public int CreateInvoiceRequest(InvoiceRequestViewModel model)
        {
            int invoiceRequestId = 0;
            List<int> detailIds = new List<int>();
            try
            {
                SqlParameter[] objParam_InvoiceRequest = new SqlParameter[14];
                objParam_InvoiceRequest[0] = new SqlParameter("@InvoiceRequestNo", model.InvoiceRequestNo);
                objParam_InvoiceRequest[1] = new SqlParameter("@ClientId", model.ClientId);
                objParam_InvoiceRequest[2] = new SqlParameter("@OrderId", model.OrderId);
                objParam_InvoiceRequest[3] = new SqlParameter("@TaxNo", string.IsNullOrEmpty(model.TaxNo) ?
                    DBNull.Value.ToString() : model.TaxNo);
                objParam_InvoiceRequest[4] = new SqlParameter("@CompanyName", string.IsNullOrEmpty(model.CompanyName) ?
                    DBNull.Value.ToString() : model.CompanyName);
                objParam_InvoiceRequest[5] = new SqlParameter("@Address", string.IsNullOrEmpty(model.Address) ?
                    DBNull.Value.ToString() : model.Address);
                if (model.PlanDate != null)
                    objParam_InvoiceRequest[6] = new SqlParameter("@PlanDate", model.PlanDate);
                else
                    objParam_InvoiceRequest[6] = new SqlParameter("@PlanDate", DBNull.Value);
                objParam_InvoiceRequest[7] = new SqlParameter("@AttachFile", string.IsNullOrEmpty(model.AttachFile) ?
                   DBNull.Value.ToString() : model.AttachFile);
                objParam_InvoiceRequest[8] = new SqlParameter("@UserVerify", DBNull.Value);
                objParam_InvoiceRequest[9] = new SqlParameter("@VerifyDate", DBNull.Value);
                objParam_InvoiceRequest[10] = new SqlParameter("@CreatedBy", model.CreatedBy);
                objParam_InvoiceRequest[11] = new SqlParameter("@CreatedDate", DateTime.Now);
                if (model.isSend == 0)
                    objParam_InvoiceRequest[12] = new SqlParameter("@Status", Convert.ToInt32((int)INVOICE_REQUEST_STATUS.LUU_NHAP));
                else
                    objParam_InvoiceRequest[12] = new SqlParameter("@Status", (int)INVOICE_REQUEST_STATUS.CHO_TBP_DUYET);
                objParam_InvoiceRequest[13] = new SqlParameter("@Note", string.IsNullOrEmpty(model.Note) ?
                DBNull.Value.ToString() : model.Note);
                invoiceRequestId = _DbWorker.ExecuteNonQuery(StoreProcedureConstant.sp_InsertInvoiceRequest, objParam_InvoiceRequest);
                if (invoiceRequestId > 0 && model.InvoiceRequestDetails != null)
                {
                    foreach (var item in model.InvoiceRequestDetails)
                    {
                        var detailId = 0;
                        SqlParameter[] objParam_requestDetail = new SqlParameter[11];
                        objParam_requestDetail[0] = new SqlParameter("@InvoiceRequestId", invoiceRequestId);
                        objParam_requestDetail[1] = new SqlParameter("@ProductName", string.IsNullOrEmpty(item.ProductName) ?
                            DBNull.Value.ToString() : item.ProductName);
                        if (string.IsNullOrEmpty(item.Unit))
                            objParam_requestDetail[2] = new SqlParameter("@Unit", Convert.ToInt32(0));
                        else
                            objParam_requestDetail[2] = new SqlParameter("@Unit", item.Unit);
                        if (item.Quantity == 0)
                            objParam_requestDetail[3] = new SqlParameter("@Quantity", Convert.ToInt32(0));
                        else
                            objParam_requestDetail[3] = new SqlParameter("@Quantity", item.Quantity);
                        if (item.Price == 0)
                            objParam_requestDetail[4] = new SqlParameter("@Price", Convert.ToInt32(0));
                        else
                            objParam_requestDetail[4] = new SqlParameter("@Price", item.Price);
                        objParam_requestDetail[5] = new SqlParameter("@PriceVat", (item.Quantity * item.Price) * (model.VAT / 100));
                        if (item.PriceExtra == 0)
                            objParam_requestDetail[6] = new SqlParameter("@PriceExtra", Convert.ToInt32(0));
                        else
                            objParam_requestDetail[6] = new SqlParameter("@PriceExtra", item.PriceExtra);
                        if (model.VAT == 0)
                            objParam_requestDetail[7] = new SqlParameter("@VAT", Convert.ToInt32(0));
                        else
                            objParam_requestDetail[7] = new SqlParameter("@VAT", model.VAT);
                        if (item.PriceExtraExport == 0)
                            objParam_requestDetail[8] = new SqlParameter("@PriceExtraExport", Convert.ToInt32(0));
                        else
                            objParam_requestDetail[8] = new SqlParameter("@PriceExtraExport", item.PriceExtraExport);
                        objParam_requestDetail[9] = new SqlParameter("@CreatedBy", model.CreatedBy);
                        objParam_requestDetail[10] = new SqlParameter("@CreatedDate", DateTime.Now);
                        detailId = _DbWorker.ExecuteNonQuery(StoreProcedureConstant.sp_InsertInvoiceRequestDetail, objParam_requestDetail);
                        if (detailId > 0)
                            detailIds.Add(detailId);
                        if (detailId <= 0)
                        {
                            DeleteRequest(invoiceRequestId, detailIds);
                            return -1;
                        }
                    }
                }
                //insert history
                if (model.isSend == 1)// action Gửi đi
                {
                    InvoiceRequestHistory invoiceRequestHistory = new InvoiceRequestHistory();
                    invoiceRequestHistory.InvoiceRequestId = invoiceRequestId;
                    invoiceRequestHistory.CreatedBy = model.CreatedBy;
                    invoiceRequestHistory.CreatedDate = DateTime.Now;
                    invoiceRequestHistory.Actioin = "Gửi đi";
                    InsertHistory(invoiceRequestHistory);
                }
                return invoiceRequestId;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CreateInvoiceRequest - InvoiceRequestDAL. " + ex);
                DeleteRequest(invoiceRequestId, detailIds);
                return -1;
            }
        }

        private void DeleteRequest(int id, List<int> detailIds)
        {
            using (var _DbContext = new EntityDataContext(_connection))
            {
                var entity = _DbContext.InvoiceRequest.Find(id);
                _DbContext.InvoiceRequest.Remove(entity);
                foreach (var idDetail in detailIds)
                {
                    var detail = _DbContext.InvoiceRequestDetail.Find(idDetail);
                    _DbContext.InvoiceRequestDetail.Remove(detail);
                }
                _DbContext.SaveChanges();
            }
        }

        private void InsertHistory(InvoiceRequestHistory invoiceRequestHistory)
        {
            using (var _DbContext = new EntityDataContext(_connection))
            {
                var entity = _DbContext.InvoiceRequestHistory.Add(invoiceRequestHistory);
                _DbContext.SaveChanges();
            }
        }

        public int UpdateInvoiceRequest(InvoiceRequestViewModel model)
        {
            int invoiceRequestId = 0;
            List<int> detailIds = new List<int>();
            try
            {
                SqlParameter[] objParam_InvoiceRequest = new SqlParameter[15];
                objParam_InvoiceRequest[0] = new SqlParameter("@InvoiceRequestNo", model.InvoiceRequestNo);
                objParam_InvoiceRequest[1] = new SqlParameter("@ClientId", model.ClientId);
                objParam_InvoiceRequest[2] = new SqlParameter("@OrderId", model.OrderId);
                objParam_InvoiceRequest[3] = new SqlParameter("@TaxNo", string.IsNullOrEmpty(model.TaxNo) ?
                    DBNull.Value.ToString() : model.TaxNo);
                objParam_InvoiceRequest[4] = new SqlParameter("@CompanyName", string.IsNullOrEmpty(model.CompanyName) ?
                    DBNull.Value.ToString() : model.CompanyName);
                objParam_InvoiceRequest[5] = new SqlParameter("@Address", string.IsNullOrEmpty(model.Address) ?
                    DBNull.Value.ToString() : model.Address);
                if (model.PlanDate != null)
                    objParam_InvoiceRequest[6] = new SqlParameter("@PlanDate", model.PlanDate);
                else
                    objParam_InvoiceRequest[6] = new SqlParameter("@PlanDate", DBNull.Value);
                objParam_InvoiceRequest[7] = new SqlParameter("@AttachFile", string.IsNullOrEmpty(model.AttachFile) ?
                   DBNull.Value.ToString() : model.AttachFile);
                objParam_InvoiceRequest[8] = new SqlParameter("@UserVerify", DBNull.Value);
                objParam_InvoiceRequest[9] = new SqlParameter("@VerifyDate", DBNull.Value);
                objParam_InvoiceRequest[10] = new SqlParameter("@UpdatedBy", model.CreatedBy);
                if (model.isSend == 0)
                    objParam_InvoiceRequest[11] = new SqlParameter("@Status", Convert.ToInt32((int)INVOICE_REQUEST_STATUS.LUU_NHAP));
                else
                    objParam_InvoiceRequest[11] = new SqlParameter("@Status", (int)INVOICE_REQUEST_STATUS.CHO_TBP_DUYET);
                objParam_InvoiceRequest[12] = new SqlParameter("@Id", model.Id);
                objParam_InvoiceRequest[13] = new SqlParameter("@IsDelete", false);
                objParam_InvoiceRequest[14] = new SqlParameter("@Note", string.IsNullOrEmpty(model.Note) ?
                    DBNull.Value.ToString() : model.Note);
                invoiceRequestId = _DbWorker.ExecuteNonQuery(StoreProcedureConstant.sp_UpdateInvoiceRequest, objParam_InvoiceRequest);
                if (invoiceRequestId > 0 && model.InvoiceRequestDetails != null)
                {
                    foreach (var item in model.InvoiceRequestDetails)
                    {
                        var detailId = 0;
                        if (item.Id == 0)
                        {
                            SqlParameter[] objParam_requestDetail = new SqlParameter[11];
                            objParam_requestDetail[0] = new SqlParameter("@InvoiceRequestId", invoiceRequestId);
                            objParam_requestDetail[1] = new SqlParameter("@ProductName", string.IsNullOrEmpty(item.ProductName) ?
                                DBNull.Value.ToString() : item.ProductName);
                            if (string.IsNullOrEmpty(item.Unit))
                                objParam_requestDetail[2] = new SqlParameter("@Unit", Convert.ToInt32(0));
                            else
                                objParam_requestDetail[2] = new SqlParameter("@Unit", item.Unit);
                            if (item.Quantity == 0)
                                objParam_requestDetail[3] = new SqlParameter("@Quantity", Convert.ToInt32(0));
                            else
                                objParam_requestDetail[3] = new SqlParameter("@Quantity", item.Quantity);
                            if (item.Price == 0)
                                objParam_requestDetail[4] = new SqlParameter("@Price", Convert.ToInt32(0));
                            else
                                objParam_requestDetail[4] = new SqlParameter("@Price", item.Price);
                            if (item.PriceVat == 0)
                                objParam_requestDetail[5] = new SqlParameter("@PriceVat", Convert.ToInt32(0));
                            else
                                objParam_requestDetail[5] = new SqlParameter("@PriceVat", item.PriceVat);
                            if (item.PriceExtra == 0)
                                objParam_requestDetail[6] = new SqlParameter("@PriceExtra", Convert.ToInt32(0));
                            else
                                objParam_requestDetail[6] = new SqlParameter("@PriceExtra", item.PriceExtra);
                            if (item.Vat == 0)
                                objParam_requestDetail[7] = new SqlParameter("@VAT", Convert.ToInt32(0));
                            else
                                objParam_requestDetail[7] = new SqlParameter("@VAT", item.Vat);
                            if (item.PriceExtraExport == 0)
                                objParam_requestDetail[8] = new SqlParameter("@PriceExtraExport", Convert.ToInt32(0));
                            else
                                objParam_requestDetail[8] = new SqlParameter("@PriceExtraExport", item.PriceExtraExport);
                            objParam_requestDetail[9] = new SqlParameter("@CreatedBy", model.CreatedBy);
                            objParam_requestDetail[10] = new SqlParameter("@CreatedDate", DateTime.Now);
                            detailId = _DbWorker.ExecuteNonQuery(StoreProcedureConstant.sp_InsertInvoiceRequestDetail, objParam_requestDetail);
                            if (detailId > 0)
                                detailIds.Add(detailId);
                        }
                        else
                        {
                            SqlParameter[] objParam_requestDetail = new SqlParameter[11];
                            objParam_requestDetail[0] = new SqlParameter("@InvoiceRequestId", invoiceRequestId);
                            objParam_requestDetail[1] = new SqlParameter("@ProductName", string.IsNullOrEmpty(item.ProductName) ?
                                DBNull.Value.ToString() : item.ProductName);
                            if (string.IsNullOrEmpty(item.Unit))
                                objParam_requestDetail[2] = new SqlParameter("@Unit", Convert.ToInt32(0));
                            else
                                objParam_requestDetail[2] = new SqlParameter("@Unit", item.Unit);
                            if (item.Quantity == 0)
                                objParam_requestDetail[3] = new SqlParameter("@Quantity", Convert.ToInt32(0));
                            else
                                objParam_requestDetail[3] = new SqlParameter("@Quantity", item.Quantity);
                            if (item.Price == 0)
                                objParam_requestDetail[4] = new SqlParameter("@Price", Convert.ToInt32(0));
                            else
                                objParam_requestDetail[4] = new SqlParameter("@Price", item.Price);
                            if (item.PriceVat == 0)
                                objParam_requestDetail[5] = new SqlParameter("@PriceVat", Convert.ToInt32(0));
                            else
                                objParam_requestDetail[5] = new SqlParameter("@PriceVat", item.PriceVat);
                            if (item.PriceExtra == 0)
                                objParam_requestDetail[6] = new SqlParameter("@PriceExtra", Convert.ToInt32(0));
                            else
                                objParam_requestDetail[6] = new SqlParameter("@PriceExtra", item.PriceExtra);
                            if (item.Vat == 0)
                                objParam_requestDetail[7] = new SqlParameter("@VAT", Convert.ToInt32(0));
                            else
                                objParam_requestDetail[7] = new SqlParameter("@VAT", item.Vat);
                            if (item.PriceExtraExport == 0)
                                objParam_requestDetail[8] = new SqlParameter("@PriceExtraExport", Convert.ToInt32(0));
                            else
                                objParam_requestDetail[8] = new SqlParameter("@PriceExtraExport", item.PriceExtraExport);
                            objParam_requestDetail[9] = new SqlParameter("@UpdatedBy", model.UpdatedBy);
                            objParam_requestDetail[10] = new SqlParameter("@Id", item.Id);
                            detailId = _DbWorker.ExecuteNonQuery(StoreProcedureConstant.sp_UpdateInvoiceRequestDetail, objParam_requestDetail);
                            if (detailId > 0)
                                detailIds.Add(detailId);
                        }
                    }
                }
                var request = GetById(invoiceRequestId);
                var listDetail = GetByRequestId(model.Id);
                foreach (var item in listDetail)
                {
                    var exists = detailIds.FirstOrDefault(n => n == item.Id);
                    if (exists == 0)
                    {
                        DeleteInvoiceRequestDetail(item);
                    }
                }
                if (model.isSend == 1)
                {
                    InvoiceRequestHistory invoiceRequestHistory = new InvoiceRequestHistory();
                    invoiceRequestHistory.InvoiceRequestId = invoiceRequestId;
                    invoiceRequestHistory.CreatedBy = model.UpdatedBy;
                    invoiceRequestHistory.CreatedDate = DateTime.Now;
                    invoiceRequestHistory.Actioin = "Gửi đi";
                    InsertHistory(invoiceRequestHistory);
                }
                return invoiceRequestId;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateInvoiceRequest - InvoiceRequestDAL. " + ex);
                return -1;
            }
        }

        public int DeleteInvoiceRequestDetail(InvoiceRequestDetail model)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    _DbContext.InvoiceRequestDetail.Remove(model);
                    _DbContext.SaveChanges();
                    return 1;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("DeleteInvoiceRequestDetail - InvoiceRequestDAL: " + ex);
                return -1;
            }
        }

        public List<InvoiceRequestDetail> GetByRequestId(long requestId)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var contractPays = _DbContext.InvoiceRequestDetail.Where(x => x.InvoiceRequestId == requestId).ToList();
                    if (contractPays != null)
                    {
                        return contractPays;
                    }
                }
                return new List<InvoiceRequestDetail>();
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetByRequestId - InvoiceRequestDAL: " + ex);
                return new List<InvoiceRequestDetail>();
            }
        }

        public int ApproveRequest(int invoiceRequestId, int userId, int status)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[3];
                objParam[0] = new SqlParameter("@InvoiceRequestId", invoiceRequestId);
                objParam[1] = new SqlParameter("@Status", status);
                objParam[2] = new SqlParameter("@UserVerify", userId);
                InvoiceRequestHistory invoiceRequestHistory = new InvoiceRequestHistory();
                invoiceRequestHistory.InvoiceRequestId = invoiceRequestId;
                invoiceRequestHistory.CreatedBy = userId;
                invoiceRequestHistory.CreatedDate = DateTime.Now;
                invoiceRequestHistory.Actioin = status == (int)INVOICE_REQUEST_STATUS.DA_DUYET ? "Duyệt yêu cầu" : "Hoàn thành yêu cầu";
                InsertHistory(invoiceRequestHistory);
                return _DbWorker.ExecuteNonQuery(StoreProcedureConstant.SP_VerifyInvoiceRequest, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ApproveRequest - InvoiceRequestDAL. " + ex);
                return -1;
            }
        }

        public int RejectRequest(long requestId, string note, int userDelete)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var model = _DbContext.InvoiceRequest.Find(requestId);
                    model.Status = (int)INVOICE_REQUEST_STATUS.TU_CHOI;
                    model.DeclineReason = note;
                    model.UpdatedDate = DateTime.Now;
                    model.UpdatedBy = userDelete;
                    _DbContext.InvoiceRequest.Update(model);
                    InvoiceRequestHistory invoiceRequestHistory = new InvoiceRequestHistory();
                    invoiceRequestHistory.InvoiceRequestId = (int)requestId;
                    invoiceRequestHistory.CreatedBy = userDelete;
                    invoiceRequestHistory.CreatedDate = DateTime.Now;
                    invoiceRequestHistory.Actioin = "Từ chối [" + note + "]";
                    _DbContext.InvoiceRequestHistory.Add(invoiceRequestHistory);
                    _DbContext.SaveChanges();
                }
                return 1;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ApproveRequest - InvoiceRequestDAL. " + ex);
                return -1;
            }
        }

        public int DeleteRequest(long requestId, int userDelete)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var model = _DbContext.InvoiceRequest.Find(requestId);
                    model.IsDelete = true;
                    model.UpdatedDate = DateTime.Now;
                    model.UpdatedBy = userDelete;
                    _DbContext.InvoiceRequest.Update(model);
                    _DbContext.SaveChanges();
                }
                return 1;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ApproveRequest - InvoiceRequestDAL. " + ex);
                return -1;
            }
        }

        public DataTable GetRequestDetail(long requestId)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@InvoiceRequestId", requestId);
                return _DbWorker.GetDataTable(StoreProcedureConstant.sp_GetDetailInvoiceRequest, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetRequestDetail - InvoiceRequestDAL: " + ex);
            }
            return null;
        }

        public DataTable GetByClientId(long clientId)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@ClientId", clientId);
                return _DbWorker.GetDataTable(StoreProcedureConstant.sp_GetListInvoiceRequestByClientId, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetByClientId - InvoiceRequestDAL: " + ex);
            }
            return null;
        }

        public DataTable GetByInvoiceId(long invoiceId)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@InvoiceId", invoiceId);
                return _DbWorker.GetDataTable(StoreProcedureConstant.sp_GetListInvoiceRequestByInvoiceId, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetByInvoiceId - InvoiceRequestDAL: " + ex);
            }
            return null;
        }

        public List<InvoiceRequestViewModel> GetByOrderId(string orderId)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@OrderId", orderId.ToString());
                return _DbWorker.GetDataTable(StoreProcedureConstant.sp_GetListInvoiceRequestByOrderId, objParam).ToList<InvoiceRequestViewModel>();
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetByOrderId - InvoiceRequestDAL: " + ex);
                return new List<InvoiceRequestViewModel>();
            }
        }

        public List<InvoiceRequest> GetByInvoiceRequestNo(List<string> invoiceRequestNos)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var requests = _DbContext.InvoiceRequest.AsNoTracking().Where(x => invoiceRequestNos.Contains(x.InvoiceRequestNo)).ToList();
                    if (requests != null)
                    {
                        return requests;
                    }
                }
                return new List<InvoiceRequest>();
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetByPaymentCode - InvoiceRequestDAL: " + ex);
                return new List<InvoiceRequest>();
            }
        }

        public List<InvoiceRequestDetail> GetByInvoiceRequestIds(List<long> invoiceRequestIds)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var details = _DbContext.InvoiceRequestDetail.AsNoTracking().Where(x => invoiceRequestIds.Contains(x.InvoiceRequestId.Value)).ToList();
                    if (details != null)
                    {
                        return details;
                    }
                }
                return new List<InvoiceRequestDetail>();
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetByPaymentCode - InvoiceRequestDAL: " + ex);
                return new List<InvoiceRequestDetail>();
            }
        }

        public List<InvoiceRequestHistory> GetHistoriesByRequestId(long requestId)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var histories = _DbContext.InvoiceRequestHistory.AsNoTracking().Where(x => x.InvoiceRequestId == requestId).ToList();
                    if (histories != null)
                    {
                        return histories;
                    }
                }
                return new List<InvoiceRequestHistory>();
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetHistoriesByRequestId - InvoiceRequestDAL: " + ex);
                return new List<InvoiceRequestHistory>();
            }
        }
    }
}
