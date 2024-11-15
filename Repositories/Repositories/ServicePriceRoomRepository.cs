using DAL;
using Entities.ConfigModels;
using Entities.Models;
using Microsoft.Extensions.Options;
using Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories
{
    public class ServicePriceRoomRepository : IServicePriceRoomRepository
    {
        private readonly ServicePiceRoomDAL _servicePiceRoomDAL;

        public ServicePriceRoomRepository(IOptions<DataBaseConfig> dataBaseConfig)
        {
            _servicePiceRoomDAL = new ServicePiceRoomDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
        }

      
    }
}
