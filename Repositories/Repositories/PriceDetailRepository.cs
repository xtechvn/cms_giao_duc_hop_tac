using DAL;
using Entities.ConfigModels;
using Entities.Models;
using Entities.ViewModels;
using Entities.ViewModels.PricePolicy;
using Microsoft.Extensions.Options;
using Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace Repositories.Repositories
{
    public class PriceDetailRepository : IPriceDetailRepository
    {
        private readonly PriceDetailDAL _priceDetailDAL;

        public PriceDetailRepository(IOptions<DataBaseConfig> dataBaseConfig, IOptions<MailConfig> mailConfig)
        {
            _priceDetailDAL = new PriceDetailDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
        }
      
        public async Task<int> RemoveByID(int id)
        {

            return await _priceDetailDAL.RemoveByID(id);
        }

        public PriceDetail FindByProductServiceId(int productServiceId)
        {
            return _priceDetailDAL.FindByProductServiceId(productServiceId);
        }

        public async Task<string> AddOrUpdateSinglePriceDetail(ProductRoomService productRoomService, PriceDetail detail,int user_id)
        {
            return await _priceDetailDAL.AddOrUpdateSinglePriceDetail(productRoomService, detail, user_id);
        }
    }
}
