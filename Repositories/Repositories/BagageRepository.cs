using DAL;
using Entities.ConfigModels;
using Entities.Models;
using Microsoft.Extensions.Options;
using Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories.Repositories
{
    public class BagageRepository : IBagageRepository
    {
        private readonly BaggageDAL baggageDAL;
        private readonly IOptions<DataBaseConfig> dataBaseConfig;

        public BagageRepository(IOptions<DataBaseConfig> _dataBaseConfig)
        {
            baggageDAL = new BaggageDAL(_dataBaseConfig.Value.SqlServer.ConnectionString);
            dataBaseConfig = _dataBaseConfig;
        }
        public List<Baggage> GetBaggages(List<int> passengerIdList)
        {
            return baggageDAL.GetBaggages(passengerIdList);
        }
    }
}
