using Entities.Models;
using Entities.ViewModels;
using Entities.ViewModels.Policy;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.IRepositories
{
    public interface IPolicyRepository
    {
        Task<GenericViewModel<PolicyViewModel>> GetPagingList(PolicySearchViewModel searchModel, int currentPage, int pageSize);
        Task<int> CreatePolicy(AddPolicyDtailViewModel data);
        PolicyDtailViewModel GetPolicyDetail(long PolicyId);
        Task<List<PolicyDtailViewModel>> DetailPolicy(long PolicyId);
        Task<int> UpdatePolicy(Policy model);
        Task<int> UpdatePolicyDetail(PolicyDetail model);
        Task<int> updatePolicy(AddPolicyDtailViewModel data);
        Task<int> DeletePolicy(int PolicyId);

        #region vin wonder
        int CreateCampaign(CampaignModel model);
        int UpdateCampaign(CampaignModel model);
        int InsertVinWonderPricePolicy(VinWonderPricePolicyModel model);
        int UpdateVinWonderPricePolicy(VinWonderPricePolicyModel model);
        void UpSertVinWonderPricePolicy(int campaign_id, IEnumerable<VinWonderPricePolicyModel> models);
        int UpdateVinWonderCommonProfit(VinWonderCommonProfitModel model);
        IEnumerable<VinWonderPricePolicyModel> GetVinWonderPricePolicyByCampaignId(int CampaignId);
        IEnumerable<VinWonderPricePolicyModel> GetVinWonderPricePolicyByServiceId(int ServiceId);
        Task<IEnumerable<VinWonderPricePolicyModel>> ImportExcelAsync(IFormFile file);
        #endregion
    }
}
