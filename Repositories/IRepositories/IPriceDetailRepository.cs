using Entities.Models;
using Entities.ViewModels;
using Entities.ViewModels.PricePolicy;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.IRepositories
{
    public interface IPriceDetailRepository
    {

        public PriceDetail FindByProductServiceId(int productServiceId);
        public Task<string> AddOrUpdateSinglePriceDetail(ProductRoomService productRoomService, PriceDetail detail, int user_id);
        public Task<int> RemoveByID(int id);
    }
}
