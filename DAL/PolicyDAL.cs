using DAL.Generic;
using DAL.StoreProcedure;
using Entities.Models;
using Entities.ViewModels;
using Entities.ViewModels.Policy;
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

    public class PolicyDAL : GenericService<Policy>
    {
        private static DbWorker _DbWorker;
        public PolicyDAL(string connection) : base(connection)
        {
            _DbWorker = new DbWorker(connection);
        }
        public async Task<DataTable> GetPagingList(PolicySearchViewModel searchModel, int currentPage, int pageSize, string proc)
        {
            try
            {

                SqlParameter[] objParam = new SqlParameter[9];
                objParam[0] = new SqlParameter("@PolicyName", searchModel.PolicyName);
                objParam[1] = new SqlParameter("@EffectiveDateFrom", searchModel.EffectiveDateFrom);
                objParam[2] = new SqlParameter("@EffectiveDateTo", searchModel.EffectiveDateTo);
                objParam[3] = new SqlParameter("@PermissionType", searchModel.PermissionType);
                objParam[4] = new SqlParameter("@UserCreate", searchModel.UserCreate);
                objParam[5] = new SqlParameter("@CreateDateFrom", searchModel.CreateDateFrom);
                objParam[6] = new SqlParameter("@CreateDateTo", searchModel.CreateDateTo);
                objParam[7] = new SqlParameter("@PageIndex", searchModel.PageIndex);
                objParam[8] = new SqlParameter("@PageSize", searchModel.PageSize);



                return _DbWorker.GetDataTable(proc, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetPagingList - PolicyDal: " + ex);
            }
            return null;
        }
        public async Task<int> CreatePolicy(Policy model)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[9];
                objParam[0] = new SqlParameter("@PolicyName", model.PolicyName);
                objParam[1] = new SqlParameter("@PolicyCode", model.PolicyCode);
                objParam[2] = new SqlParameter("@EffectiveDate", model.EffectiveDate);
                objParam[3] = new SqlParameter("@PermissionType", model.PermissionType);
                objParam[4] = new SqlParameter("@IsPrivate", model.IsPrivate);
                objParam[5] = new SqlParameter("@CreatedBy", model.CreatedBy);
                objParam[6] = new SqlParameter("@CreatedDate", model.CreatedDate);
                objParam[7] = new SqlParameter("@UpdatedBy", "");
                objParam[8] = new SqlParameter("@UpdatedDate", "");
                return _DbWorker.ExecuteNonQuery(StoreProcedureConstant.SP_InsertPolicy, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CreatePolicy - PolicyDal: " + ex);
            }
            return 0;
        }
        public DataTable GetPolicyDetail(long PolicyId)
        {
            try
            {

                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@PolicyId", PolicyId);

                return _DbWorker.GetDataTable(StoreProcedureConstant.sp_GetDetailPolicy, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetPolicyDetail - PolicyDal: " + ex);
            }
            return null;
        }
        public async Task<DataTable> DetailPolicy(long PolicyId)
        {
            try
            {

                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@PolicyId", PolicyId);

                return _DbWorker.GetDataTable(StoreProcedureConstant.sp_GetDetailPolicy, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("DetailPolicy - PolicyDal: " + ex);
            }
            return null;
        }
        public async Task<int> UpdatatPolicy(Policy model)
        {
            try
            {

                SqlParameter[] objParam = new SqlParameter[6];
                objParam[0] = new SqlParameter("@PolicyId", model.PolicyId);
                objParam[1] = new SqlParameter("@PolicyName", model.PolicyName);
                objParam[2] = new SqlParameter("@EffectiveDate", model.EffectiveDate);
                objParam[3] = new SqlParameter("@PermissionType", model.PermissionType);
                objParam[4] = new SqlParameter("@UpdatedBy", model.UpdatedBy);
                objParam[5] = new SqlParameter("@UpdatedDate", "");


                return _DbWorker.ExecuteNonQuery(StoreProcedureConstant.SP_UpdatePolicy, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("DetailPolicy - PolicyDal: " + ex);
            }
            return 0;
        }
        public async Task<int> DeletePolicy(int PolicyId)
        {
            try
            {

                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@PolicyId", PolicyId);
                return _DbWorker.ExecuteNonQuery(StoreProcedureConstant.SP_DeletePolicy, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("DetailPolicy - PolicyDal: " + ex);
            }
            return 0;
        }

        #region vin wonder
        public int CreateCampaign(CampaignModel model)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[]
                {
                    new SqlParameter("@CampaignCode",model.CampaignCode),
                    new SqlParameter("@FromDate",model.FromDate),
                    new SqlParameter("@ToDate",model.ToDate),
                    new SqlParameter("@ClientTypeId",model.ClientTypeId ?? 1),
                    new SqlParameter("@Description",model.Description ?? (object)DBNull.Value),
                    new SqlParameter("@Status",model.Status),
                    new SqlParameter("@CreateDate",DateTime.Now),
                    new SqlParameter("@UserCreateId",model.UserAction),
                    new SqlParameter("@ContractType",model.ContractType ?? 6),
                };
                return _DbWorker.ExecuteNonQuery(StoreProcedureConstant.sp_InsertCampaign, objParam);
            }
            catch
            {
                throw;
            }
        }

        public int UpdateCampaign(CampaignModel model)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[]
                {
                    new SqlParameter("@Id",model.Id),
                    new SqlParameter("@CampaignCode",model.CampaignCode),
                    new SqlParameter("@FromDate",model.FromDate),
                    new SqlParameter("@ToDate",model.ToDate),
                    new SqlParameter("@ClientTypeId",model.ClientTypeId ?? 1),
                    new SqlParameter("@Description",model.Description ?? (object)DBNull.Value),
                    new SqlParameter("@Status",model.Status),
                    new SqlParameter("@UpdateLast",DateTime.Now),
                    new SqlParameter("@UserUpdateId",model.UserAction),
                    new SqlParameter("@ContractType",model.ContractType ?? (object)DBNull.Value),
                };
                return _DbWorker.ExecuteNonQuery(StoreProcedureConstant.sp_UpdateCampaign, objParam);
            }
            catch
            {
                throw;
            }
        }

        public int InsertVinWonderPricePolicy(VinWonderPricePolicyModel model)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[]
                {
                     new SqlParameter("@CampaignId", model.CampaignId),
                     new SqlParameter("@ServiceId", model.ServiceId),
                     new SqlParameter("@BasePrice", model.BasePrice),
                     new SqlParameter("@WeekendRate", model.WeekendRate ?? (object)DBNull.Value),
                     new SqlParameter("@Profit", model.Profit),
                     new SqlParameter("@UnitType", model.UnitType),
                     new SqlParameter("@RateCode", model.RateCode),
                     new SqlParameter("@ServiceCode", model.ServiceCode),
                     new SqlParameter("@Name", model.Name),
                     new SqlParameter("@CreatedDate", DateTime.Now),
                     new SqlParameter("@CreatedBy", model.UserAction)
                };
                return _DbWorker.ExecuteNonQuery(StoreProcedureConstant.sp_InsertVinWonderPricePolicy, objParam);
            }
            catch
            {
                throw;
            }
        }

        public int UpdateVinWonderPricePolicy(VinWonderPricePolicyModel model)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[]
                {
                     new SqlParameter("@Id", model.Id),
                     new SqlParameter("@CampaignId", model.CampaignId),
                     new SqlParameter("@ServiceId", model.ServiceId),
                     new SqlParameter("@BasePrice", model.BasePrice),
                     new SqlParameter("@WeekendRate", model.WeekendRate),
                     new SqlParameter("@Profit", model.Profit),
                     new SqlParameter("@UnitType", model.UnitType),
                     new SqlParameter("@RateCode", model.RateCode),
                     new SqlParameter("@ServiceCode", model.ServiceCode),
                     new SqlParameter("@Name", model.Name),
                     new SqlParameter("@UpdatedBy", model.UserAction)
                };
                return _DbWorker.ExecuteNonQuery(StoreProcedureConstant.sp_UpdateVinWonderPricePolicy, objParam);
            }
            catch
            {
                throw;
            }
        }

        public void UpsertVinWonderPricePolicy(int campaign_id, IEnumerable<VinWonderPricePolicyModel> models, int user_action)
        {
            try
            {
                _DbWorker.ExecuteActionTransaction((connection, transaction) =>
                {
                    if (models != null && models.Any())
                    {
                        foreach (var model in models)
                        {
                            SqlCommand oCommand = new SqlCommand()
                            {
                                Connection = connection,
                                Transaction = transaction,
                                CommandType = CommandType.StoredProcedure
                            };

                            if (model.Id > 0)
                            {
                                oCommand.CommandText = StoreProcedureConstant.sp_UpdateVinWonderPricePolicy;
                                oCommand.Parameters.AddRange(new SqlParameter[] {
                                     new SqlParameter("@Id", model.Id),
                                     new SqlParameter("@CampaignId", campaign_id),
                                     new SqlParameter("@ServiceId", model.ServiceId),
                                     new SqlParameter("@BasePrice", model.BasePrice),
                                     new SqlParameter("@WeekendRate", model.WeekendRate),
                                     new SqlParameter("@Profit", model.Profit),
                                     new SqlParameter("@UnitType", model.UnitType),
                                     new SqlParameter("@RateCode", model.RateCode),
                                     new SqlParameter("@ServiceCode", model.ServiceCode),
                                     new SqlParameter("@Name", model.Name),
                                     new SqlParameter("@SiteId", model.SiteId),
                                     new SqlParameter("@SiteName", model.SiteName ?? (object)DBNull.Value),
                                     new SqlParameter("@UpdatedBy", user_action)
                                });
                            }
                            else
                            {
                                oCommand.CommandText = StoreProcedureConstant.sp_InsertVinWonderPricePolicy;
                                oCommand.Parameters.AddRange(new SqlParameter[]
                                {
                                     new SqlParameter("@CampaignId", campaign_id),
                                     new SqlParameter("@ServiceId", model.ServiceId),
                                     new SqlParameter("@BasePrice", model.BasePrice),
                                     new SqlParameter("@WeekendRate", model.WeekendRate ?? (object)DBNull.Value),
                                     new SqlParameter("@Profit", model.Profit),
                                     new SqlParameter("@UnitType", model.UnitType),
                                     new SqlParameter("@RateCode", model.RateCode),
                                     new SqlParameter("@ServiceCode", model.ServiceCode),
                                     new SqlParameter("@Name", model.Name),
                                     new SqlParameter("@SiteId", model.SiteId),
                                     new SqlParameter("@SiteName", model.SiteName ?? (object)DBNull.Value),
                                     new SqlParameter("@CreatedDate", DateTime.Now),
                                     new SqlParameter("@CreatedBy", user_action)
                                });
                            }

                            SqlParameter OuputParam = oCommand.Parameters.Add("@Identity", SqlDbType.Int);
                            OuputParam.Direction = ParameterDirection.Output;

                            oCommand.ExecuteNonQuery();
                        }
                    }
                });
            }
            catch
            {
                throw;
            }
        }

        public DataTable GetVinWonderPricePolicyByCampaignId(int CampaignId)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[]
                {
                     new SqlParameter("@CampaignId", CampaignId)
                };
                return _DbWorker.GetDataTable(StoreProcedureConstant.SP_GetVinWonderPricePolicyByCampaignId, objParam);
            }
            catch
            {
                throw;
            }
        }

        public int UpdateVinWonderCommonProfit(VinWonderCommonProfitModel model)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[]
                {
                    new SqlParameter("@Id",model.Id),
                    new SqlParameter("@Type",model.Type),
                    new SqlParameter("@CodeValue",model.CodeValue),
                    new SqlParameter("@Description",model.Description),
                    new SqlParameter("@OrderNo",DBNull.Value),
                    new SqlParameter("@UpdatedBy",model.UserAction),
                    new SqlParameter("@UpdateTime",DateTime.Now)
                };
                return _DbWorker.ExecuteNonQuery(StoreProcedureConstant.sp_UpdateAllCode, objParam);
            }
            catch
            {
                throw;
            }
        }

        public DataTable GetVinWonderPricePolicyByServiceId(int ServiceId)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[]
                {
                     new SqlParameter("@ServiceId", ServiceId)
                };
                return _DbWorker.GetDataTable(StoreProcedureConstant.SP_GetVinWonderPricePolicyByServiceId, objParam);
            }
            catch
            {
                throw;
            }
        }
        #endregion
    }
}
