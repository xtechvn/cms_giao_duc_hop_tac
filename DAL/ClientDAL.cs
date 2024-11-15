using DAL.Generic;
using DAL.StoreProcedure;
using Entities.Models;
using Entities.ViewModels.CustomerManager;
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
    public class ClientDAL : GenericService<Client>
    {
        private static DbWorker _DbWorker;
        public ClientDAL(string connection) : base(connection)
        {
            _DbWorker = new DbWorker(connection);
        }

        public async Task<AccountClient> GetAccountClientDetai(long clientId)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var detail = await _DbContext.AccountClient.AsNoTracking().FirstOrDefaultAsync(x => x.Id == clientId);
                    if (detail != null)
                    {
                        return detail;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetAccountClientDetai - ClientDAL: " + ex.ToString());
                return null;
            }
        }
        public async Task<AccountClient> GetAccountClientIdDetai(long clientId)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var detail = await _DbContext.AccountClient.AsNoTracking().FirstOrDefaultAsync(x => x.ClientId == clientId);
                    if (detail != null)
                    {
                        return detail;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetAccountClientIdDetai - ClientDAL: " + ex.ToString());
                return null;
            }
        }

        public async Task<AccountClient> GetAccountClientByID(long id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var detail = await _DbContext.AccountClient.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                    if (detail != null)
                    {
                        return detail;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetAccountClientByID - ClientDAL: " + ex.ToString());
                return null;
            }
        }
        public async Task<List<AccountClient>> GetAccountClientAsync()
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return await _DbContext.AccountClient.AsNoTracking().ToListAsync();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetAccountClientAsync - ClientDAL: " + ex.ToString());
                return new List<AccountClient>();
            }
        }

        public async Task<Client> GetClientDetail(long clientId)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var detail = await _DbContext.Client.AsNoTracking().FirstOrDefaultAsync(x => x.Id == clientId);
                    if (detail != null)
                    {
                        return detail;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetClientDetail - ClientDAL: " + ex.ToString());
                return null;
            }
        }

        public List<Client> GetClientByIds(List<long> clientIds)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var clients = _DbContext.Client.AsNoTracking().Where(x => clientIds.Contains(x.Id)).ToList();
                    if (clients != null)
                    {
                        return clients;
                    }
                }
                return new List<Client>();
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetClientByIds - ClientDAL: " + ex.ToString());
                return new List<Client>();
            }
        }

        public async Task<List<Client>> GetClientType(int Type)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var clients = _DbContext.Client.AsNoTracking().Where(x => x.ClientType == Type).ToList();
                    if (clients != null)
                    {
                        return clients;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetClientType - ClientDAL: " + ex.ToString());
                return null;
            }
        }
        public List<Client> GetAllClient()
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var clients = _DbContext.Client.AsNoTracking().OrderByDescending(s => s.JoinDate).ToList();
                    if (clients != null)
                    {
                        return clients;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetAllClient - ClientDAL: " + ex.ToString());
                return null;
            }
        }

        public async Task<List<Client>> GetClientInfo(List<long> listIdAccountClient)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var listAccountClient = _DbContext.AccountClient.AsNoTracking().Where(n => listIdAccountClient.Contains(n.Id)).ToList();
                    var listClientId = listAccountClient.Select(n => n.ClientId).ToList();
                    var listClient = _DbContext.Client.AsNoTracking().Where(n => listClientId.Contains(n.Id)).ToList();
                    foreach (var item in listClient)
                    {
                        var accountClient = listAccountClient.FirstOrDefault(n => n.ClientId == item.Id);
                        item.ClientMapId = accountClient?.Id;
                    }
                    return listClient;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetClientInfo - ClientDAL: " + ex);
                return new List<Client>();
            }
        }
        public CustomerManagerViewModel GetUserAgentClientId(long ClientId)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var deta = (from a in _DbContext.UserAgent.AsNoTracking().Where(s => s.ClientId == ClientId)
                                join b in _DbContext.Client.AsNoTracking() on a.ClientId equals b.Id
                                join h in _DbContext.User.AsNoTracking() on a.UserId equals h.Id
                                join k in _DbContext.User.AsNoTracking() on a.CreatedBy equals k.Id
                                join i in _DbContext.User.AsNoTracking() on a.UpdatedBy equals i.Id
                                join c in _DbContext.AllCode.AsNoTracking().Where(s => s.Type == AllCodeType.CLIENT_TYPE) on b.ClientType equals c.CodeValue
                                join d in _DbContext.AllCode.AsNoTracking().Where(s => s.Type == AllCodeType.AGENCY_TYPE) on b.AgencyType equals d.CodeValue
                                join e in _DbContext.AllCode.AsNoTracking().Where(s => s.Type == AllCodeType.PERMISION_TYPE) on b.PermisionType equals e.CodeValue
                                join g in _DbContext.AllCode.AsNoTracking().Where(s => s.Type == AllCodeType.VERIFY_STATUS) on a.VerifyStatus equals g.CodeValue
                                select new CustomerManagerViewModel
                                {
                                    UserId_name = h.FullName,
                                    client_type_name = c.Description,
                                    AgencyType_name = d.Description,
                                    PermisionType_name = e.Description,
                                    CreateDate_UserAgent = ((DateTime)a.CreateDate).ToString("dd/MM/yyyy"),
                                    VerifyDate = ((DateTime)a.VerifyDate).ToString("dd/MM/yyyy"),
                                    UpdateLast = ((DateTime)a.UpdateLast).ToString("dd/MM/yyyy"),
                                    VerifyStatus_name = g.Description,
                                    TaxNo = b.TaxNo,
                                    Create_name = k.FullName,
                                    Update_Name=i.FullName
                                    
                                }).FirstOrDefault();

                    if (deta != null)
                    {
                        return deta;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetUserAgentClientId - ClientDAL: " + ex.ToString());
                return null;
            }
        }
        public int SetUpClient(Client model)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    if (model.Id == 0)
                    {
                        var check = _DbContext.Client.Where(s => s.ClientCode == model.ClientCode).ToList();
                        if(check!=null && check.Count > 0)
                        {
                            return 2;
                        }
                        else
                        {
                            var deta = _DbContext.Client.Add(model);
                            _DbContext.SaveChanges();
                        }
                       
                    }
                    else
                    {
                        var data2 = _DbContext.Client.Where(s => s.Email.Equals(model.Email) && s.Id != model.Id).ToList();
                      

                        if (data2.Count == 0 && data2 != null  )
                        {
                           
                           
                                var deta = _DbContext.Client.Update(model);
                                _DbContext.SaveChanges();
                                return 1;
                            
                            
                        }
                        else
                        {
                            return 2;
                        }

                    }

                }
                return 1;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SetUpClient - ClientDAL: " + ex.ToString());
                return 0;
            }
        }
        public Client GetClientByEmail(string email)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var data2 = _DbContext.Client.AsQueryable();
                    var a = data2.FirstOrDefault(s => s.Email.Equals(email));
                    return a;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetClientByEmail - ClientDAL: " + ex);
                return null;
            }
        }
        public Client GetClientByTaxNo(string TaxNo)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var data2 = _DbContext.Client.AsQueryable();
                    var a = data2.FirstOrDefault(s => s.TaxNo.Equals(TaxNo));
                    return a;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetClientByTaxNo - ClientDAL: " + ex);
                return null;
            }
        }
        public async Task<DataTable> GetPagingList(CustomerManagerViewSearchModel searchModel, int currentPage, int pageSize, string proc)
        {
            try
            {

                SqlParameter[] objParam = new SqlParameter[16];
                objParam[0] = new SqlParameter("@MaKH", searchModel.MaKH);
                objParam[1] = new SqlParameter("@TenKH", searchModel.TenKH);
                objParam[2] = new SqlParameter("@Email", searchModel.Email);
                objParam[3] = new SqlParameter("@Phone", searchModel.Phone);
                objParam[4] = new SqlParameter("@AgencyType", searchModel.AgencyType);
                objParam[5] = new SqlParameter("@ClientType", searchModel.ClientType);
                objParam[6] = new SqlParameter("@PermissionType", searchModel.PermissionType);
                objParam[7] = new SqlParameter("@PageIndex", currentPage);
                objParam[8] = new SqlParameter("@PageSize", pageSize);
                objParam[9] = new SqlParameter("@UserId", searchModel.UserId);
                objParam[10] = (CheckDate(searchModel.CreateDate) == DateTime.MinValue) ? new SqlParameter("@CreateDate", DBNull.Value) : new SqlParameter("@CreateDate", CheckDate(searchModel.CreateDate));
                objParam[11] = (CheckDate(searchModel.EndDate) == DateTime.MinValue) ? new SqlParameter("@EndDate", DBNull.Value) : new SqlParameter("@EndDate", CheckDate(searchModel.EndDate));
                objParam[12] = new SqlParameter("@MinAmount", searchModel.MinAmount);
                objParam[13] = new SqlParameter("@MaxAmount", searchModel.MaxAmount);
                objParam[14] = new SqlParameter("@CreatedBy", searchModel.CreatedBy);
                objParam[15] = new SqlParameter("@SalerPermission", searchModel.SalerPermission);
                return _DbWorker.GetDataTable(proc, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetPagingList - ClientDAL: " + ex);
            }
            return null;
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
        public async Task<List<Client>> GetClientSuggesstion(string txt_search)
        {
            try
            {
                List<Client> result = new List<Client>();

                using (var _DbContext = new EntityDataContext(_connection))
                {
                    if (txt_search == null || txt_search.Trim() == "")
                    {
                        result = await _DbContext.Client.AsNoTracking().OrderByDescending(x => x.JoinDate).Take(20).ToListAsync();
                    }
                    else
                    {
                        txt_search = txt_search.Trim();
                        result = await _DbContext.Client.AsNoTracking().Where(x => x.Email.Contains(txt_search) || x.ClientName.Contains(txt_search) || x.Phone.Contains(txt_search)).OrderByDescending(x => x.JoinDate).Take(20).ToListAsync();
                    }

                }
                return result;

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetClientSuggesstion - ClientDAL: " + ex);
                return new List<Client>();
            }
        }
        public async Task<DataTable> getClientid(long Client)
        {
            try
            {

                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@ClientID", Client);
                
                return  _DbWorker.GetDataTable(StoreProcedureConstant.GetClientByID, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("getClientid - ClientDal: " + ex);
            }
            return null;
        }
        public async Task<DataTable> GetAmountRemainOfContractByClientId(long Client)
        {
            try
            {

                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@ClientId", Client);

                return _DbWorker.GetDataTable(StoreProcedureConstant.Sp_GetAmountRemainOfContractPayByClientId, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetAmountRemainOfContractByClientId - ClientDal: " + ex);
            }
            return null;
        }
        public async Task<Client> GetClientByClientCode(string client_code)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return await _DbContext.Client.AsNoTracking().FirstOrDefaultAsync(x => x.ClientCode.ToLower() == client_code);
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetClientByClientCode - ClientDAL: " + ex);
                return null;
            }
        }
        public async Task<DataTable> GetListClientCustomerCareFund(CustomerManagerViewSearchModel searchModel)
        {
            try
            {

                SqlParameter[] objParam = new SqlParameter[4];
               
                objParam[0] = new SqlParameter("@SalerPermission", searchModel.SalerPermission);
                objParam[1] = new SqlParameter("@ClientId", searchModel.MaKH);
                objParam[2] = new SqlParameter("@PageIndex", searchModel.PageIndex);
                objParam[3] = new SqlParameter("@PageSize", searchModel.PageSize);
          
   
                return _DbWorker.GetDataTable(StoreProcedureConstant.SP_GetListClientCustomerCareFund, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetPagingList - ClientDAL: " + ex);
            }
            return null;
        }
    }
}