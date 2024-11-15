using DAL;
using DAL.StoreProcedure;
using Entities.ConfigModels;
using Entities.Models;
using Entities.ViewModels;
using Entities.ViewModels.Contract;
using Entities.ViewModels.ContractHistory;
using Entities.ViewModels.Policy;
using Microsoft.Extensions.Options;
using Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace Repositories.Repositories
{
    public class ContractRepository : IContractRepository
    {

        private readonly ContractDAL _ContractDAL;
        private readonly AllCodeDAL _AllCodeDAL;
        private readonly PolicyDetailDAL _PolicyDetailDAL;
        private readonly ContractHistoryDAL _ContractHistoryDAL;
        private readonly PolicyDAL _PolicyDAL;



        public ContractRepository(IOptions<DataBaseConfig> dataBaseConfig)
        {

            _ContractDAL = new ContractDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            _AllCodeDAL = new AllCodeDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            _PolicyDetailDAL = new PolicyDetailDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            _ContractHistoryDAL = new ContractHistoryDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            _PolicyDAL = new PolicyDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
        }
        public async Task<GenericViewModel<ContractViewModel>> GetListByType(long ClientId, int currentPage, int pageSize)
        {
            try
            {
                var model = new GenericViewModel<ContractViewModel>();
                DataTable dt = await _ContractDAL.GetListByClientId(ClientId);
                if (dt != null && dt.Rows.Count > 0)
                {
                    var data = (from row in dt.AsEnumerable()
                                select new ContractViewModel
                                {

                                    ContractId = Convert.ToInt32(!row["ContractId"].Equals(DBNull.Value) ? row["ContractId"] : 0),
                                    ContractNo = !row["ContractNo"].Equals(DBNull.Value) ? row["ContractNo"].ToString() : "",
                                    ExpireDate = Convert.ToDateTime(!row["ExpireDate"].Equals(DBNull.Value) ? row["ExpireDate"].ToString() : ""),
                                    CreateDate = Convert.ToDateTime(!row["CreateDate"].Equals(DBNull.Value) ? row["CreateDate"].ToString() : ""),
                                    UpdateLast = Convert.ToDateTime(!row["UpdateLast"].Equals(DBNull.Value) ? row["UpdateLast"].ToString() : ""),
                                    PolicyId= Convert.ToInt32(!row["PolicyId"].Equals(DBNull.Value) ? row["PolicyId"] : 0),
                                    ServiceType = !row["ServiceType"].Equals(DBNull.Value) ? row["ServiceType"].ToString() : "",
                                    SalerId_Name = !row["SalerName"].Equals(DBNull.Value) ? row["SalerName"].ToString() : "",
                                    UserIdCreate_Name = !row["CreateName"].Equals(DBNull.Value) ? row["CreateName"].ToString() : "",
                                    UserIdUpdate_Name = !row["UpdataName"].Equals(DBNull.Value) ? row["UpdataName"].ToString() : "",
                                    ContractStatus = Convert.ToInt32(!row["ContractStatus"].Equals(DBNull.Value) ? row["ContractStatus"] : 0),
                                    ContractDate = Convert.ToDateTime(!row["ContractDate"].Equals(DBNull.Value) ? row["ContractDate"].ToString() : ""),

                                    ServiceTypeName = !row["ServiceTypeName"].Equals(DBNull.Value) ? row["ServiceTypeName"].ToString() : "",
                                    ContractStatus_Name = !row["ContractStatusName"].Equals(DBNull.Value) ? row["ContractStatusName"].ToString() : "",



                                }).OrderByDescending(s => s.CreateDate).ToList();

                    model.ListData = data;
                    model.PageSize = pageSize;
                    model.CurrentPage = currentPage;
                    return model;
                }
                return model;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<GenericViewModel<ContractViewModel>> GetPagingList(ContractSearchViewModel searchModel, int currentPage, int pageSize)
        {
            var model = new GenericViewModel<ContractViewModel>();
            try
            {

                DataTable dt = await _ContractDAL.GetPagingList(searchModel, currentPage, pageSize, ProcedureConstants.SP_GetListContract);
                if (dt != null && dt.Rows.Count > 0)
                {
                    var data = (from row in dt.AsEnumerable()
                                select new ContractViewModel
                                {

                                    ContractNo = !row["ContractNo"].Equals(DBNull.Value) ? row["ContractNo"].ToString() : "",
                                    ExpireDate = Convert.ToDateTime(!row["ExpireDate"].Equals(DBNull.Value) ? row["ExpireDate"].ToString() : ""),
                                    ClientId_Name = !row["ClientName"].Equals(DBNull.Value) ? row["ClientName"].ToString() : "",
                                    ClienType_Name = !row["ClienType"].Equals(DBNull.Value) ? row["ClienType"].ToString() : "",
                                    AgencyType_Name = !row["AgencyType"].Equals(DBNull.Value) ? row["AgencyType"].ToString() : "",
                                    PermisionType_Name = !row["PermisionType"].Equals(DBNull.Value) ? row["PermisionType"].ToString() : "",
                                    asDebtType_Name = !row["DebtType"].Equals(DBNull.Value) ? row["DebtType"].ToString() : "",
                                    UserIdCreate_Name = !row["FullName"].Equals(DBNull.Value) ? row["FullName"].ToString() : "",
                                    UserIdUpdate_Name = !row["UserIdUpdateName"].Equals(DBNull.Value) ? row["UserIdUpdateName"].ToString() : "",
                                    CreateDate = Convert.ToDateTime(!row["CreateDate"].Equals(DBNull.Value) ? row["CreateDate"].ToString() : ""),
                                    UpdateLast = Convert.ToDateTime(!row["UpdateLast"].Equals(DBNull.Value) ? row["UpdateLast"].ToString() : ""),
                                    SalerId_Name = !row["SalerIdName"].Equals(DBNull.Value) ? row["SalerIdName"].ToString() : "",
                                    ServiceType = !row["ServiceType"].Equals(DBNull.Value) ? row["ServiceType"].ToString() : "",
                                    ContractStatus_Name = !row["ContractStatusName"].Equals(DBNull.Value) ? row["ContractStatusName"].ToString() : "",
                                    ContractStatus = Convert.ToInt32(!row["ContractStatus"].Equals(DBNull.Value) ? row["ContractStatus"] : 0),
                                    ContractId = Convert.ToInt32(!row["ContractId"].Equals(DBNull.Value) ? row["ContractId"] : 0),
                                    Client_Email = !row["Email"].Equals(DBNull.Value) ? row["Email"].ToString() : "",
                                    Client_Phone = !row["Phone"].Equals(DBNull.Value) ? row["Phone"].ToString() : "",
                                    ServiceTypeName = !row["ServiceTypeName"].Equals(DBNull.Value) ? row["ServiceTypeName"].ToString() : "",
                                    ContractExpireStatus = !row["ContractExpireStatus"].Equals(DBNull.Value) ? row["ContractExpireStatus"].ToString() : "",
                                    ContractExpire = Convert.ToInt32(!row["ContractExpire"].Equals(DBNull.Value) ? row["ContractExpire"] : 0),
                                    ClientId = Convert.ToInt32(!row["ClientId"].Equals(DBNull.Value) ? row["ClientId"] : 0),


                                }).ToList();
                    var alicode = _AllCodeDAL.GetListByType(AllCodeType.SERVICE_TYPE);

                    foreach (var item in data)
                    {
                        List<string> ServiceType_name = new List<string>();
                        var price_range = Array.ConvertAll(item.ServiceTypeName.Split(','), s => s.ToString());

                        item.ServiceType_Name = price_range;
                    }


                    model.ListData = data;
                    model.CurrentPage = currentPage;
                    model.PageSize = pageSize;
                    model.TotalRecord = Convert.ToInt32(dt.Rows[0]["TotalRow"]);
                    model.TotalPage = (int)Math.Ceiling((double)model.TotalRecord / model.PageSize);
                    return model;
                }
                return null;

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetPagingList - ContractRepository: " + ex);
                return null;
            }

        }
        public async Task<List<TotalConTract>> TotalConTract(ContractSearchViewModel searchModel, int currentPage, int pageSize)
        {
            var model = new GenericViewModel<TotalConTract>();
            try
            {

                DataTable dt = await _ContractDAL.TotalConTract(searchModel, currentPage, pageSize, ProcedureConstants.Sp_CountTotalContractByStatus);
                if (dt != null && dt.Rows.Count > 0)
                {
                    var data = (from row in dt.AsEnumerable()
                                select new TotalConTract
                                {
                                    Total = Convert.ToInt32(!row["Total"].Equals(DBNull.Value) ? row["Total"] : 0),
                                    ContractStatus = Convert.ToInt32(!row["ContractStatus"].Equals(DBNull.Value) ? row["ContractStatus"] : 0),
                                    ContractStatusName = !row["ContractStatusName"].Equals(DBNull.Value) ? row["ContractStatusName"].ToString() : "",
                                }).ToList();
                    return data;
                }
                return null;

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetPagingList - ContractRepository: " + ex);
                return null;
            }

        }
        public PolicyDetail GetPolicyDetailByType(int ClientType, int PermisionType)
        {
            try
            {

                return _PolicyDetailDAL.GetPolicyDetailByType(ClientType, PermisionType);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetPolicyDetailByType - PolicyDetailDAL. " + ex);
                return null;
            }
        }
        public async Task<long> CreateContact(ContractViewModel model)
        {
            try
            {
                var datetime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                var Contract_model = new Contract();
                Contract_model.ContractDate = Convert.ToDateTime(datetime);
                Contract_model.ClientId = model.ClientId;
                Contract_model.UserIdCreate = model.UserIdCreate;
                Contract_model.CreateDate = Convert.ToDateTime(datetime);
                Contract_model.UpdateLast = Convert.ToDateTime(datetime);
                Contract_model.VerifyDate = Convert.ToDateTime(datetime);
                Contract_model.VerifyStatus = (byte)VerifyStatus.CHUA_DUYET;
                Contract_model.ContractStatus = model.ContractStatus;
                Contract_model.UserIdVerify = model.UserIdCreate;
                Contract_model.DebtType = model.DebtType;
                Contract_model.ServiceType = model.ServiceType;
                Contract_model.TotalVerify = 0;
                Contract_model.ClientType = Convert.ToInt32(model.ClientId_Name);
                Contract_model.PermisionType = Convert.ToInt32(model.PermisionType_Name);
                Contract_model.ContractId = model.ContractId;
                Contract_model.PolicyId = model.PolicyId;
                Contract_model.IsDelete = ContractIsDelete.HD;
                Contract_model.Note = model.Note;
                Contract_model.ContractNo = model.ContractNo;
                Contract_model.ExpireDate = model.ExpireDate;
                Contract_model.SalerId = model.SalerId;

                if (model.IsPrivate == 1 && model.PolicyId == 0)
                {
                    var DataModel = new Policy();
                    DataModel.CreatedBy = Contract_model.UserIdCreate;
                    DataModel.PolicyCode = "CSHT";
                    DataModel.CreatedDate = DateTime.Now;
                    DataModel.IsPrivate = true;
                    DataModel.PermissionType = Contract_model.PermisionType;
                    DataModel.PolicyName = "Chính sách riêng" + DateTime.Now;
                    DataModel.EffectiveDate = DateTime.Now;
                    var poliId = await _PolicyDAL.CreatePolicy(DataModel);
                    if (poliId <= 0)
                    {
                        LogHelper.InsertLogTelegram("CreateContact-CreatePolicy- PolicyDetailDAL. ");
                        return 0;
                    }
                    Contract_model.PolicyId = poliId;

                    PolicyDtailViewModel PolicyDtail = new PolicyDtailViewModel();
                    AddPolicyDtailViewModel List_PolicyDtail = new AddPolicyDtailViewModel();
                    List_PolicyDtail.extra_policy = new List<PolicyDtailViewModel>();

                    List_PolicyDtail.CreatedBy = (int)Contract_model.UserIdCreate;
                    PolicyDtail.ClientType = Contract_model.ClientType.ToString();
                    PolicyDtail.DebtType = model.DebtType.ToString();
                    PolicyDtail.ProductFlyTicketDebtAmount = model.ProductFlyTicketDebtAmount;
                    PolicyDtail.HotelDebtAmout = model.HotelDebtAmout;
                    PolicyDtail.ProductFlyTicketDepositAmount =  model.ProductFlyTicketDepositAmount;
                    PolicyDtail.HotelDepositAmout = model.HotelDepositAmout;
                    PolicyDtail.VinWonderDebtAmount = model.VinWonderDebtAmount;
                    PolicyDtail.VinWonderDepositAmount = model.VinWonderDepositAmount;
                    PolicyDtail.TourDebtAmount = model.TourDebtAmount;
                    PolicyDtail.TourDepositAmount = model.TourDepositAmount;
                    PolicyDtail.TouringCarDebtAmount = model.TouringCarDebtAmount;
                    PolicyDtail.TouringCarDepositAmount = model.TouringCarDepositAmount;
                    
                    PolicyDtail.PolicyId = poliId;
                    List_PolicyDtail.extra_policy.Add(PolicyDtail);
                    var addPolicyDtail = await _PolicyDetailDAL.InsertPolicyDetail(List_PolicyDtail, poliId);
                    if (addPolicyDtail <= 0)
                    {
                        LogHelper.InsertLogTelegram("CreateContact-InsertPolicyDetail - PolicyDetailDAL. ");
                        return 0;
                    }
                }
                if (model.IsPrivate == 1 && model.PolicyId != 0)
                {
                    var DataModel = new PolicyDetail();


                    DataModel.Id = (int)model.Id;
                    DataModel.PolicyId = (int)model.PolicyId;
                    DataModel.ClientType = Convert.ToInt32(model.ClientId_Name);
                    DataModel.DebtType = model.DebtType==null? 0: (int)model.DebtType;
                    DataModel.ProductFlyTicketDebtAmount = (int)model.ProductFlyTicketDebtAmount;
                    DataModel.HotelDebtAmout = (int)model.HotelDebtAmout;
                    DataModel.ProductFlyTicketDepositAmount = (int)model.ProductFlyTicketDepositAmount;
                    DataModel.HotelDepositAmout = (int)model.HotelDepositAmout;
                    DataModel.UpdatedBy = (int)model.UserIdCreate;

                    DataModel.VinWonderDebtAmount = (int)model.VinWonderDebtAmount;
                    DataModel.VinWonderDepositAmount = (int)model.VinWonderDepositAmount;
                    DataModel.TourDebtAmount = (int)model.TourDebtAmount;
                    DataModel.TourDepositAmount = (int)model.TourDepositAmount;
                    DataModel.TouringCarDebtAmount = (int)model.TouringCarDebtAmount;
                    DataModel.TouringCarDepositAmount = (int)model.TouringCarDepositAmount;



                    var addPolicyDtail = await _PolicyDetailDAL.UpdatePolicyDetail(DataModel);
                    if (addPolicyDtail <= 0)
                    {
                        LogHelper.InsertLogTelegram("CreateContact-InsertPolicyDetail - PolicyDetailDAL. ");
                        return 0;
                    }
                }

                var mode = new ContractHistory();
                mode.CreatedDate = DateTime.Now;
                mode.ActionDate = DateTime.Now;
                mode.CreatedBy = model.UserIdCreate;
                mode.ContractId = model.ContractId;
                mode.ActionBy = model.ContractStatus;
                mode.Action = "";

                var a = await _ContractHistoryDAL.InsertContractHistory(mode);

                var data = await _ContractDAL.CreateContact(Contract_model);

                return data;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CreateContact - PolicyDetailDAL. " + ex);
                return 0;
            }
        }
        public async Task<long> CreateContact2(Contract model, string Note)
        {
            try
            {

                var mode = new ContractHistory();
                mode.CreatedDate = DateTime.Now;
                mode.ActionDate = DateTime.Now;
                mode.CreatedBy = model.UserIdUpdate;
                mode.ContractId = model.ContractId;
                mode.ActionBy = model.ContractStatus;
                if (Note == null) { Note = ""; }
                mode.Action = Note;
                var a = await _ContractHistoryDAL.InsertContractHistory(mode);
                if (model.ContractStatus == ContractStatus.DA_DUYET)
                {
                    var list = _ContractDAL.GetlistContract(model.ClientId);
                    if (list != null)
                    {
                        foreach (var item in list)
                        {
                            item.ContractStatus = ContractStatus.HUY_BO;
                            var update = await _ContractDAL.CreateContact(item);
                        }
                    }
                }
                var data = await _ContractDAL.CreateContact(model);

                return data;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CreateContact - PolicyDetailDAL. " + ex);
                return 0;
            }
        }
        public async Task<List<ContractViewModel>> GetDetailContract(int ContractId)
        {

            try
            {

                DataTable dt = await _ContractDAL.GetDetailContract(ContractId);
                if (dt != null && dt.Rows.Count > 0)
                {
                    var data = (from row in dt.AsEnumerable()
                                select new ContractViewModel
                                {

                                    ContractId = Convert.ToInt32(!row["ContractId"].Equals(DBNull.Value) ? row["ContractId"] : 0),
                                    ContractNo = !row["ContractNo"].Equals(DBNull.Value) ? row["ContractNo"].ToString() : "",
                                    ExpireDate = Convert.ToDateTime(!row["ExpireDate"].Equals(DBNull.Value) ? row["ExpireDate"].ToString() : ""),
                                    CreateDate = Convert.ToDateTime(!row["CreateDate"].Equals(DBNull.Value) ? row["CreateDate"].ToString() : ""),
                                    UpdateLast = Convert.ToDateTime(!row["UpdateLast"].Equals(DBNull.Value) ? row["UpdateLast"].ToString() : ""),
                                    VerifyDate = Convert.ToDateTime(!row["VerifyDate"].Equals(DBNull.Value) ? row["VerifyDate"].ToString() : ""),
                                    ServiceType = !row["ServiceType"].Equals(DBNull.Value) ? row["ServiceType"].ToString() : "",
                                    SalerId_Name = !row["FullName"].Equals(DBNull.Value) ? row["FullName"].ToString() : "",
                                    ProductFlyTicketDebtAmount = Convert.ToDouble(!row["ProductFlyTicketDebtAmount"].Equals(DBNull.Value) ? row["ProductFlyTicketDebtAmount"] : 0),
                                    HotelDebtAmout = Convert.ToDouble(!row["HotelDebtAmout"].Equals(DBNull.Value) ? row["HotelDebtAmout"] : 0),
                                    ProductFlyTicketDepositAmount = Convert.ToDouble(!row["ProductFlyTicketDepositAmount"].Equals(DBNull.Value) ? row["ProductFlyTicketDepositAmount"] : 0),
                                    HotelDepositAmout = Convert.ToDouble(!row["HotelDepositAmout"].Equals(DBNull.Value) ? row["HotelDepositAmout"] : 0),

                                    TourDebtAmount = Convert.ToDouble(!row["TourDebtAmount"].Equals(DBNull.Value) ? row["TourDebtAmount"] : 0),
                                    TourDepositAmount = Convert.ToDouble(!row["TourDepositAmount"].Equals(DBNull.Value) ? row["TourDepositAmount"] : 0),
                                    TouringCarDebtAmount = Convert.ToDouble(!row["TouringCarDebtAmount"].Equals(DBNull.Value) ? row["TouringCarDebtAmount"] : 0),
                                    TouringCarDepositAmount = Convert.ToDouble(!row["TouringCarDepositAmount"].Equals(DBNull.Value) ? row["TouringCarDepositAmount"] : 0),
                                    VinWonderDebtAmount = Convert.ToDouble(!row["VinWonderDebtAmount"].Equals(DBNull.Value) ? row["VinWonderDebtAmount"] : 0),
                                    VinWonderDepositAmount = Convert.ToDouble(!row["VinWonderDepositAmount"].Equals(DBNull.Value) ? row["VinWonderDepositAmount"] : 0),


                                    UserIdCreate_Name = !row["CreateName"].Equals(DBNull.Value) ? row["CreateName"].ToString() : "",
                                    UserIdUpdate_Name = !row["UpdataName"].Equals(DBNull.Value) ? row["UpdataName"].ToString() : "",
                                    ClientId_Name = !row["ClientName"].Equals(DBNull.Value) ? row["ClientName"].ToString() : "",

                                    ClientNote = !row["ClientNote"].Equals(DBNull.Value) ? row["ClientNote"].ToString() : "",
                                    ContractStatus_Name = !row["ContractStatusName"].Equals(DBNull.Value) ? row["ContractStatusName"].ToString() : "",

                                    PermisionType_Name = !row["PermisionTypeName"].Equals(DBNull.Value) ? row["PermisionTypeName"].ToString() : "",
                                    ClienType_Name = !row["ClientTypeName"].Equals(DBNull.Value) ? row["ClientTypeName"].ToString() : "",
                                    asDebtType_Name = !row["DebtTypeName"].Equals(DBNull.Value) ? row["DebtTypeName"].ToString() : "",
                                    PermisionType = Convert.ToInt32(!row["PermisionType"].Equals(DBNull.Value) ? row["PermisionType"] : 0),
                                    ContractStatus = Convert.ToInt32(!row["ContractStatus"].Equals(DBNull.Value) ? row["ContractStatus"] : 0),
                                    ClientId = Convert.ToInt32(!row["ClientId"].Equals(DBNull.Value) ? row["ClientId"] : 0),

                                    Client_Email = !row["Email"].Equals(DBNull.Value) ? row["Email"].ToString() : "",
                                    Client_Phone = !row["Phone"].Equals(DBNull.Value) ? row["Phone"].ToString() : "",
                                    ClientType = Convert.ToInt32(!row["ClientType"].Equals(DBNull.Value) ? row["ClientType"] : 0),
                                    ContractDate = Convert.ToDateTime(!row["VerifyDate"].Equals(DBNull.Value) ? row["VerifyDate"].ToString() : ""),
                                    UserIdCreate = Convert.ToInt32(!row["UserIdCreate"].Equals(DBNull.Value) ? row["UserIdCreate"] : 0),
                                    ServiceTypeName = !row["ServiceTypeName"].Equals(DBNull.Value) ? row["ServiceTypeName"].ToString() : "",
                                    Note = !row["Note"].Equals(DBNull.Value) ? row["Note"].ToString() : "",
                                    SalerId = Convert.ToInt32(!row["SalerId"].Equals(DBNull.Value) ? row["SalerId"] : 0),
                                    PolicyId = Convert.ToInt32(!row["PolicyId"].Equals(DBNull.Value) ? row["PolicyId"] : 0),
                                    IsPrivate = Convert.ToInt32(!row["IsPrivate"].Equals(DBNull.Value) ? row["IsPrivate"] : 0),
                                    DebtType = Convert.ToInt32(!row["DebtType"].Equals(DBNull.Value) ? row["DebtType"] : 0),
                                    Id = Convert.ToInt32(!row["Id"].Equals(DBNull.Value) ? row["Id"] : 0),


                                }).ToList();

                    return data;
                }
                return null;

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetDetailContract - ContractRepository: " + ex);
                return null;
            }

        }
        public async Task<long> UpdataContactStatus(long ContractId, long ContractStatus, string Note, long UserIdUpdate)
        {
            try
            {

                var mode = new ContractHistory();
                mode.CreatedDate = DateTime.Now;
                mode.ActionDate = DateTime.Now;
                mode.CreatedBy = (int?)UserIdUpdate;
                mode.ContractId = ContractId;
                mode.ActionBy = (int?)ContractStatus;
                if (Note == null) { Note = ""; }
                mode.Action = Note;
                var a = await _ContractHistoryDAL.InsertContractHistory(mode);
                var data = await _ContractDAL.UpdataContactStatus(ContractId, ContractStatus, Note);
                return data;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdataContactStatus - PolicyDetailDAL. " + ex);
                return 0;
            }
        }
        public async Task<long> DeleteContact(long ContractId)
        {
            try
            {
                return await _ContractDAL.DeleteContact(ContractId);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdataContactStatus - PolicyDetailDAL. " + ex);
                return 0;
            }
        }
        public async Task<List<ContractHistoryViewModel>> GetPagingListContractHistory(long ContractId, long ActionBy)
        {
            var model = new List<ContractHistoryViewModel>();
            try
            {

                DataTable dt = await _ContractHistoryDAL.GetContractHistory(ContractId, ActionBy);
                if (dt != null && dt.Rows.Count > 0)
                {
                    var data = (from row in dt.AsEnumerable()
                                select new ContractHistoryViewModel
                                {

                                    ContractId = Convert.ToInt32(!row["ContractId"].Equals(DBNull.Value) ? row["ContractId"].ToString() : ""),
                                    Action = !row["Action"].Equals(DBNull.Value) ? row["Action"].ToString() : "",
                                    ActionDate = Convert.ToDateTime(!row["ActionDate"].Equals(DBNull.Value) ? row["ActionDate"].ToString() : ""),
                                    CreatedDate = Convert.ToDateTime(!row["CreatedDate"].Equals(DBNull.Value) ? row["CreatedDate"].ToString() : ""),
                                    Fullname = !row["FullName"].Equals(DBNull.Value) ? row["FullName"].ToString() : "",
                                    ActionName = !row["ActionName"].Equals(DBNull.Value) ? row["ActionName"].ToString() : "",



                                }).ToList();
                    model = data;
                    return model;
                }
                return null;

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetPagingListContractHistory - ContractRepository: " + ex);
                return null;
            }

        }
        public async Task<Contract> GetActiveContractByClientId(long ClientId)
        {
            return await _ContractDAL.GetActiveContractByClientId(ClientId);

        }
        public async Task<int> CheckContractbyStatus(long ClientId, int Status)
        {
            try
            {
                DataTable dt = await _ContractDAL.GetListByClientId(ClientId);
                if (dt != null && dt.Rows.Count > 0)
                {
                    var data = dt.ToList<ContractViewModel>();
                    data = data.Where(s => s.ContractStatus == Status).ToList();
                    if (data.Count > 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return 1;
                    }
                }
                return 1;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetPagingListContractHistory - ContractRepository: " + ex);
                return 1;
            }

        }
    }
}
