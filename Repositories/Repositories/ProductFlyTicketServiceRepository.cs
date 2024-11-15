using DAL;
using Entities.ConfigModels;
using Entities.Models;
using Entities.ViewModels.PricePolicy;
using Microsoft.Extensions.Options;
using Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories.Repositories
{
    public class ProductFlyTicketServiceRepository : IProductFlyTicketServiceRepository
    {
        private readonly ProductFlyTicketServiceDAL _productFlyTicketServiceDAL;

        public ProductFlyTicketServiceRepository(IOptions<DataBaseConfig> dataBaseConfig)
        {
            _productFlyTicketServiceDAL = new ProductFlyTicketServiceDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
        }

        public int AddCampaginAndProduct(PricePolicySummitModel model,int userId)
        {
            return _productFlyTicketServiceDAL.AddOrUpdateCampaginAndProduct(model, userId);
        }

        public ProductFlyTicketService GetByCampaignID(int campaignId)
        {
            return _productFlyTicketServiceDAL.GetByCampaignID(campaignId);
        }

        public int Update(ProductFlyTicketService model)
        {
            return _productFlyTicketServiceDAL.Update(model);
        }
    }
}
