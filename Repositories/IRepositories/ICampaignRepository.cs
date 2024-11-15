using Entities.Models;
using Entities.ViewModels;
using Entities.ViewModels.PricePolicy;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.IRepositories
{
    public interface ICampaignRepository
    {
        public GenericViewModel<PricePolicyListingModel> GetPagingList(PricePolicySearchModel searchModel, int currentPage, int pageSize);

        public Task<Campaign> GetDetailByCode(string campaign_code);
        public Task<int> AddNew(Campaign campaign);
        public Task<int> Update(Campaign campaign);
        public int Delete(int campaignId);
        public Task<int> CreateOrUpdateHotelCampaign(HotelPricePolicyCampaignModel detail, int user_id);
        public Task<Campaign> GetDetailByID(int id);
        public Task<HotelPricePolicyCampaignModel> GetPolicyDetailViewByCampaignID(int campaign_id,int hotel_id, DateTime? from_date, DateTime? to_date);
        public Task<ProductRoomService> GetFirstProductServiceRoombyCampaignID(int campaign_id);
        Task<int> CreateOrUpdateHotelPriceDetail(HotelPricePolicyCampaignModel detail, int user_id);
        Task<string> ExportCampaignExcel(GenericViewModel<PricePolicyListingModel> model, string file_path);
    }
}
