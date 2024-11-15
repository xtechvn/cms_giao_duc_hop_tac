using DAL.Generic;
using DAL.StoreProcedure;
using Entities.Models;
using Entities.ViewModels;
using Entities.ViewModels.PricePolicy;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace DAL
{
    public class CampaignDAL : GenericService<Campaign>
    {
        private static DbWorker _DbWorker;
        public CampaignDAL(string connection) : base(connection)
        {
            _DbWorker = new DbWorker(connection);
        }
        public DataTable GetPagingList(PricePolicySearchModel searchModel, int currentPage, int pageSize)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[10];

                if (searchModel.CampaignCode != null && searchModel.CampaignCode.Trim() != "")
                    objParam[0] = new SqlParameter("@CampaignCode", searchModel.CampaignCode);
                else
                    objParam[0] = new SqlParameter("@CampaignCode", DBNull.Value);
                if (searchModel.CampaignDescription != null && searchModel.CampaignDescription.Trim() != "")
                    objParam[1] = new SqlParameter("@CampaignDescription", searchModel.CampaignDescription);
                else
                    objParam[1] = new SqlParameter("@CampaignDescription", DBNull.Value);

                if (searchModel.FromDate != DateTime.MinValue)
                    objParam[2] = new SqlParameter("@FromDate", searchModel.FromDate);
                else
                    objParam[2] = new SqlParameter("@FromDate", DBNull.Value);
                if (searchModel.ToDate != DateTime.MinValue)
                    objParam[3] = new SqlParameter("@ToDate", searchModel.ToDate);
                else
                    objParam[3] = new SqlParameter("@ToDate", DBNull.Value);

                objParam[4] = new SqlParameter("@CampaginStatus", searchModel.CampaginStatus);
                objParam[5] = new SqlParameter("@ClientType", searchModel.ClientType);
                objParam[6] = new SqlParameter("@ServiceType", searchModel.ServiceType);

                objParam[7] = new SqlParameter("@CurentPage", currentPage);
                objParam[8] = new SqlParameter("@PageSize", pageSize);
                if (searchModel.ProviderName !=  null && searchModel.ProviderName.Trim()!="")
                    objParam[9] = new SqlParameter("@ProviderName", searchModel.ProviderName);
                else
                    objParam[9] = new SqlParameter("@ProviderName", DBNull.Value);


                return _DbWorker.GetDataTable(ProcedureConstants.Campaign_Search, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetPagingList - CampaignDAL: " + ex.ToString());
            }
            return null;
        }

        public async Task<Campaign> GetDetailByCode(string campaign_code)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return await _DbContext.Campaign.AsNoTracking().FirstOrDefaultAsync(s => s.CampaignCode == campaign_code);
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetDetailByCode - CampaignDAL: " + ex.ToString());
                return null;
            }
        }
        public async Task<Campaign> GetDetailByID(int id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return await _DbContext.Campaign.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetDetailByID - CampaignDAL: " + ex.ToString());
                return null;
            }
        }
        
        public async Task<int> AddNew(Campaign campaign)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var add = _DbContext.Campaign.Add(campaign);
                    await _DbContext.SaveChangesAsync();
                    return campaign.Id;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("AddNew - CampaignDAL: " + ex.ToString());
                return -1;
            }
        }
        public async Task<int> Update(Campaign campaign)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var exists= _DbContext.Campaign.FirstOrDefault(n => n.Id == campaign.Id);
                    if(exists!=null && exists.Id > 0)
                    {

                        exists. CampaignCode =campaign.CampaignCode;
                        exists.UpdateLast =DateTime.Now;
                        exists.ClientTypeId =campaign.ClientTypeId;
                        exists.ContractType =1;
                        exists.Description =campaign.Description;
                        exists.FromDate =campaign.FromDate;
                        exists.Status =campaign.Status;
                        exists.ToDate =campaign.ToDate;
                        exists.UserUpdateId =campaign.UserUpdateId;
                        var add = _DbContext.Campaign.Update(exists);
                        await _DbContext.SaveChangesAsync();
                        return campaign.Id;

                    }
                    return campaign.Id;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Update - CampaignDAL: " + ex.ToString());
                return -1;
            }
        }
        public int Delete(int campaignId)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var entity = _DbContext.Campaign.FirstOrDefault(n => n.Id == campaignId);
                    _DbContext.Campaign.Remove(entity);
                    _DbContext.SaveChanges();
                    return 0;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Delete - CampaignDAL: " + ex.ToString());
                return -1;
            }
        }
        public async Task<DataTable> GetHotelPricePolicyByCampaignID(int campaign_id)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@CampaignId", campaign_id);

                return _DbWorker.GetDataTable(StoreProcedureConstant.GetHotelPricePolicyByCampaignID, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetListPriceDetailByListPackagesId - PriceDetailDAL: " + ex);
            }
            return null;
        }
        public async Task<DataTable> GetHotelPricePolicyDailyByCampaignID(int campaign_id)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@CampaignId", campaign_id);

                return _DbWorker.GetDataTable(StoreProcedureConstant.GetHotelPricePolicyDailyByCampaignID, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetListPriceDetailByListPackagesId - PriceDetailDAL: " + ex);
            }
            return null;
        }
    }
}
