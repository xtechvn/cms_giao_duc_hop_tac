using Entities.ViewModels.PricePolicy;
using ENTITIES.APPModels.PushHotel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.IRepositories
{
    public interface IRoomFunRepository
    {
        public Task<int> CreateOrUpdateRoomFun(HotelContract detail);
        public Task<List<HotelPricePolicyDetail>> GetPolicyByProvider(int campaign_id, string hotel_id, int contract_type);
        public string GetNameByID(string allotment_id);
    }
}
