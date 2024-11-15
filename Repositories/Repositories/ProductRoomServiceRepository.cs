using DAL;
using Entities.ConfigModels;
using Entities.Models;
using Entities.ViewModels.PricePolicy;
using Microsoft.Extensions.Options;
using Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories
{
    public class ProductRoomServiceRepository : IProductRoomServiceRepository
    {
        private readonly ProductRoomServiceDAL _productRoomServiceDAL;
        private readonly PriceDetailDAL _priceDetailDAL;

        public ProductRoomServiceRepository(IOptions<DataBaseConfig> dataBaseConfig, IOptions<MailConfig> mailConfig)
        {
            _productRoomServiceDAL = new ProductRoomServiceDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            _priceDetailDAL = new PriceDetailDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
        }

    }
}
