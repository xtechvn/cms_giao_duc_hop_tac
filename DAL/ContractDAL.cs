using DAL.Generic;
using DAL.StoreProcedure;
using Entities.Models;
using Entities.ViewModels.Contract;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace DAL
{
   public class ContractDAL: GenericService<Contract>
    {

        private static DbWorker _DbWorker;
        public ContractDAL(string connection) : base(connection)
        {
            _DbWorker = new DbWorker(connection);
        }
        public async Task<DataTable> GetListByClientId(long ClientId )
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@ClientId", ClientId);
               
                
                return _DbWorker.GetDataTable(StoreProcedureConstant.sp_GetListContractByClientId, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetListByClientId - ContractDAL. " + ex);
                return null;
            }
        }

        public async Task<DataTable> GetPagingList(ContractSearchViewModel searchModel, int currentPage, int pageSize, string proc)
        {
            try
            {

                SqlParameter[] objParam = new SqlParameter[24];
                objParam[0] = new SqlParameter("@ClientId", searchModel.ClientId);
                objParam[1] = new SqlParameter("@ContractNo", searchModel.ContractNo);
                objParam[2] = new SqlParameter("@ClientName", searchModel.ClientName);
                objParam[3] = new SqlParameter("@Phone", searchModel.Phone);
                objParam[4] = new SqlParameter("@Email", searchModel.Email);
                objParam[5] = new SqlParameter("@ContractStatus", searchModel.ContractStatus);
                objParam[6] = new SqlParameter("@ContractExpire", searchModel.ContractExpire);
                objParam[7] = new SqlParameter("@ClientAgencyType", searchModel.ClientAgencyType);
                objParam[8] = new SqlParameter("@ClientType", searchModel.ClientType);
                objParam[9] = new SqlParameter("@PermissionType", searchModel.PermissionType);
                objParam[10] = new SqlParameter("@VerifyStatus", searchModel.VerifyStatus);
                objParam[11] = (CheckDate(searchModel.ExpireDateFrom) == DateTime.MinValue) ? new SqlParameter("@ExpireDateFrom", DBNull.Value) : new SqlParameter("@ExpireDateFrom", CheckDate(searchModel.ExpireDateFrom));
                objParam[12] = (CheckDate(searchModel.ExpireDateTo) == DateTime.MinValue) ? new SqlParameter("@ExpireDateTo", DBNull.Value) : new SqlParameter("@ExpireDateTo", CheckDate(searchModel.ExpireDateTo));
                objParam[13] = new SqlParameter("@DebtType", searchModel.DebtType);
                objParam[14] = new SqlParameter("@SalerId", searchModel.SalerId);
                objParam[15] = new SqlParameter("@UserCreate", searchModel.UserCreate);
                objParam[16] = (CheckDate(searchModel.CreateDateFrom) == DateTime.MinValue) ? new SqlParameter("@CreateDateFrom", DBNull.Value) : new SqlParameter("@CreateDateFrom", CheckDate(searchModel.CreateDateFrom));
                objParam[17] = (CheckDate(searchModel.CreateDateTo) == DateTime.MinValue) ? new SqlParameter("@CreateDateTo", DBNull.Value) : new SqlParameter("@CreateDateTo", CheckDate(searchModel.CreateDateTo));
                objParam[18] = new SqlParameter("@UserVerify", searchModel.UserVerify);
                objParam[19] = (CheckDate(searchModel.VerifyDateFrom) == DateTime.MinValue) ? new SqlParameter("@VerifyDateFrom", DBNull.Value) : new SqlParameter("@VerifyDateFrom", CheckDate(searchModel.VerifyDateFrom));             
                objParam[20] = (CheckDate(searchModel.VerifyDateTo) == DateTime.MinValue) ? new SqlParameter("@VerifyDateTo", DBNull.Value) : new SqlParameter("@VerifyDateTo", CheckDate(searchModel.VerifyDateTo));
                objParam[21] = new SqlParameter("@PageIndex", searchModel.PageIndex);
                objParam[22] = new SqlParameter("@PageSize", searchModel.PageSize);
                objParam[23] = new SqlParameter("@SalerPermission", searchModel.SalerPermission);
             
          
                
                return _DbWorker.GetDataTable(proc, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetPagingList - ContractDAL: " + ex);
            }
            return null;
        }

        public async Task<DataTable> TotalConTract(ContractSearchViewModel searchModel, int currentPage, int pageSize, string proc)
        {
            try
            {

                SqlParameter[] objParam = new SqlParameter[22];
                objParam[0] = new SqlParameter("@ClientId", searchModel.ClientId);
                objParam[1] = new SqlParameter("@ContractNo", searchModel.ContractNo);
                objParam[2] = new SqlParameter("@ClientName", searchModel.ClientName);
                objParam[3] = new SqlParameter("@Phone", searchModel.Phone);
                objParam[4] = new SqlParameter("@Email", searchModel.Email);
                objParam[5] = new SqlParameter("@ContractStatus", searchModel.ContractStatus);
                objParam[6] = new SqlParameter("@ContractExpire", searchModel.ContractExpire);
                objParam[7] = new SqlParameter("@ClientAgencyType", searchModel.ClientAgencyType);
                objParam[8] = new SqlParameter("@ClientType", searchModel.ClientType);
                objParam[9] = new SqlParameter("@PermissionType", searchModel.PermissionType);
                objParam[10] = new SqlParameter("@VerifyStatus", searchModel.VerifyStatus);
                objParam[11] = (CheckDate(searchModel.ExpireDateFrom) == DateTime.MinValue) ? new SqlParameter("@ExpireDateFrom", DBNull.Value) : new SqlParameter("@ExpireDateFrom", CheckDate(searchModel.ExpireDateFrom));
                objParam[12] = (CheckDate(searchModel.ExpireDateTo) == DateTime.MinValue) ? new SqlParameter("@ExpireDateTo", DBNull.Value) : new SqlParameter("@ExpireDateTo", CheckDate(searchModel.ExpireDateTo));
                objParam[13] = new SqlParameter("@DebtType", searchModel.DebtType);
                objParam[14] = new SqlParameter("@SalerId", searchModel.SalerId);
                objParam[15] = new SqlParameter("@UserCreate", searchModel.UserCreate);
                objParam[16] = (CheckDate(searchModel.CreateDateFrom) == DateTime.MinValue) ? new SqlParameter("@CreateDateFrom", DBNull.Value) : new SqlParameter("@CreateDateFrom", CheckDate(searchModel.CreateDateFrom));
                objParam[17] = (CheckDate(searchModel.CreateDateTo) == DateTime.MinValue) ? new SqlParameter("@CreateDateTo", DBNull.Value) : new SqlParameter("@CreateDateTo", CheckDate(searchModel.CreateDateTo));
                objParam[18] = new SqlParameter("@UserVerify", searchModel.UserVerify);
                objParam[19] = (CheckDate(searchModel.VerifyDateFrom) == DateTime.MinValue) ? new SqlParameter("@VerifyDateFrom", DBNull.Value) : new SqlParameter("@VerifyDateFrom", CheckDate(searchModel.VerifyDateFrom));
                objParam[20] = (CheckDate(searchModel.VerifyDateTo) == DateTime.MinValue) ? new SqlParameter("@VerifyDateTo", DBNull.Value) : new SqlParameter("@VerifyDateTo", CheckDate(searchModel.VerifyDateTo));
                objParam[21] = new SqlParameter("@SalerPermission", searchModel.SalerPermission);
                return _DbWorker.GetDataTable(proc, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("TotalConTract - ContractDAL: " + ex);
            }
            return null;
        }

        public long GetContractIDByClientId(long client_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {

                    var client = _DbContext.Client.FirstOrDefault(x=>x.Id==client_id);
                    if(client==null || client.Id <= 0)
                    {
                        return -1;
                    }
                    if (client.ClientType == (int)ClientType.kl) return 0;
                    var contract = _DbContext.Contract.FirstOrDefault(x=>x.ContractStatus ==0 && x.VerifyStatus==1);
                    if(contract==null || contract.ContractId <= 0)
                    {
                        return contract.ContractId;
                    }
                }
                return -1;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetContractIDByClientId - ContractDAL. " + ex);
                return -2;
            }
        }
        public async Task<long> CreateContact(Contract model)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    if (model.ContractId == 0)
                    {
                        //_DbContext.Contract.Add(model);
                        //await _DbContext.SaveChangesAsync();
                        //return 1;
                        SqlParameter[] objParam = new SqlParameter[18];
                        objParam[0] = model.ContractNo==null? new SqlParameter("@ContractNo", DBNull.Value):new SqlParameter("@ContractNo", model.ContractNo);
                        objParam[1] = model.ContractDate == null ? new SqlParameter("@ContractDate", DBNull.Value) : new SqlParameter("@ContractDate", model.ContractDate);
                        objParam[2] = model.ExpireDate == null ? new SqlParameter("@ExpireDate", DBNull.Value) : new SqlParameter("@ExpireDate", model.ExpireDate);
                        objParam[3] = model.ClientId == null ? new SqlParameter("@ClientId", DBNull.Value) : new SqlParameter("@ClientId", model.ClientId);
                        objParam[4] = model.SalerId == null ? new SqlParameter("@SalerId", DBNull.Value) : new SqlParameter("@SalerId", model.SalerId);
                        objParam[5] = model.CreateDate == null ? new SqlParameter("@CreateDate", DBNull.Value) : new SqlParameter("@CreateDate", model.CreateDate);
                        objParam[6] = model.VerifyStatus == null ? new SqlParameter("@VerifyStatus", DBNull.Value) : new SqlParameter("@VerifyStatus", model.VerifyStatus);
                        objParam[7] = model.UserIdCreate == null ? new SqlParameter("@UserIdCreate", DBNull.Value) : new SqlParameter("@UserIdCreate", model.UserIdCreate);
                        objParam[8] = model.UserIdVerify == null ? new SqlParameter("@UserIdVerify", DBNull.Value) : new SqlParameter("@UserIdVerify", model.UserIdVerify);
                        objParam[9] = model.VerifyDate == null ? new SqlParameter("@VerifyDate", DBNull.Value) : new SqlParameter("@VerifyDate", model.VerifyDate);
                        objParam[10] = model.TotalVerify == null ? new SqlParameter("@TotalVerify", DBNull.Value) : new SqlParameter("@TotalVerify", model.TotalVerify);
                        objParam[11] = model.ContractStatus == null ? new SqlParameter("@ContractStatus", DBNull.Value) : new SqlParameter("@ContractStatus", model.ContractStatus);
                        objParam[12] = model.DebtType == null ? new SqlParameter("@DebtType", DBNull.Value) : new SqlParameter("@DebtType", model.DebtType);
                        objParam[13] = model.ServiceType == null ? new SqlParameter("@ServiceType", DBNull.Value) : new SqlParameter("@ServiceType", model.ServiceType);
                        objParam[14] = model.PolicyId == null ? new SqlParameter("@PolicyId", DBNull.Value) : new SqlParameter("@PolicyId", model.PolicyId);
                        objParam[15] = model.Note == null ? new SqlParameter("@Note", DBNull.Value) : new SqlParameter("@Note", model.Note);
                        objParam[16] = model.ClientType == null ? new SqlParameter("@ClientType", DBNull.Value) : new SqlParameter("@ClientType", model.ClientType);
                        objParam[17] = model.PermisionType == null ? new SqlParameter("@PermisionType", DBNull.Value) : new SqlParameter("@PermisionType", model.PermisionType);
                       return _DbWorker.ExecuteNonQuery(StoreProcedureConstant.InsertContract, objParam);
                    }
                    else
                    {
                       var Cmodel= GetContractDtail((int)model.ContractId);
                        if(Cmodel!= null)
                        {
                            model.ContractDate = DateTime.ParseExact(Cmodel.ContractDate.ToString("dd/MM/yyyy hh:mm:ss"), "dd/MM/yyyy hh:mm:ss", CultureInfo.InvariantCulture);
                            model.VerifyDate = DateTime.ParseExact(((DateTime)Cmodel.VerifyDate).ToString("dd/MM/yyyy hh:mm:ss"), "dd/MM/yyyy hh:mm:ss", CultureInfo.InvariantCulture);
                            model.CreateDate = DateTime.ParseExact(Cmodel.CreateDate.ToString("dd/MM/yyyy hh:mm:ss"), "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                            model.UpdateLast = DateTime.Now;
                            model.ExpireDate = model.ExpireDate;

                            _DbContext.Contract.Update(model);
                            await _DbContext.SaveChangesAsync();
                            if (model.ContractStatus == ContractStatus.DA_DUYET)
                            {
                                var a = UpdataContactStatus(model.ContractId, (long)model.ContractStatus, "");
                            }
                            return 1;
                        }
                        return 0;
                    }
                        
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CreateContact - ContractDAL. " + ex.ToString());
                return 0;
            }
        }
        public async Task<DataTable> GetDetailContract( int ContractId)
        {
            try
            {

                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@ContractId", ContractId);
                
                return _DbWorker.GetDataTable(StoreProcedureConstant.sp_GetDetailContract, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetDetailContract - ContractDAL: " + ex);
            }
            return null;
        }
        public async Task<long> UpdataContactStatus(long ContractId,long ContractStatus,string Note)
        {
            try
            {
                 SqlParameter[] objParam = new SqlParameter[3];
                 objParam[0] = new SqlParameter("@ContractId", ContractId);
                 objParam[1] = new SqlParameter("@ContractStatus", ContractStatus);
                 objParam[2] = new SqlParameter("@Note", Note);
            
             
             _DbWorker.ExecuteNonQuery(StoreProcedureConstant.SP_UpdateContractStatus, objParam);
                 return 1;
               
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdataContactStatus - ContractDAL. " + ex);
                return 0;
            }
        }
        public async Task<long> DeleteContact(long ContractId)
        {
            try
            {
                
                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@ContractId", ContractId);
                _DbWorker.ExecuteNonQuery(StoreProcedureConstant.SP_DeleteContract, objParam);
                return 1;
                
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("DeleteContact - ContractDAL. " + ex);
                return 0;
            }
        }
        private DateTime CheckDate(string dateTime)
        {
            DateTime _date = DateTime.MinValue;
            if (!string.IsNullOrEmpty(dateTime))
            {
                _date = DateTime.ParseExact(dateTime, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }

            return _date != DateTime.MinValue ? _date : DateTime.MinValue;
        }
       
        public long CountContractInYear()
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.Contract.AsNoTracking().Where(x => (((DateTime?)x.ContractDate) ?? DateTime.Now).Year == DateTime.Now.Year).Count();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CountContractInYear - ContractDAL: " + ex.ToString());
                return -1;
            }
        }
        public  List<Contract> GetlistContract(int ClientId)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return  _DbContext.Contract.AsNoTracking().Where(x => (((DateTime?)x.ExpireDate)>= DateTime.Now) && x.ContractStatus == ContractStatus.DA_DUYET && x.ClientId== ClientId).ToList();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetlistContract - ContractDAL: " + ex.ToString());
                return null;
            }
        }
        public async Task<Contract> GetActiveContractByClientId(long ClientId)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return await _DbContext.Contract.AsNoTracking().Where(x => (((DateTime?)x.ExpireDate) >= DateTime.Now) && x.ContractStatus == ContractStatus.DA_DUYET && x.ClientId == ClientId).FirstOrDefaultAsync();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetActiveContractByClientId - ContractDAL: " + ex.ToString());
                return null;
            }
        }
        public Contract GetContractDtail(int ClientId)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.Contract.FirstOrDefault(s=>s.ContractId== ClientId);
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetContractDtail - ContractDAL: " + ex.ToString());
                return null;
            }
        }
    }
}
