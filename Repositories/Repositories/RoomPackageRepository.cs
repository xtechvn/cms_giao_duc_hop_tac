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
    public class RoomPackageRepository : IRoomPackageRepository
    {
        private readonly RoomPackageDAL _roomPackageDAL;

        public RoomPackageRepository(IOptions<DataBaseConfig> dataBaseConfig)
        {
            _roomPackageDAL = new RoomPackageDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
        }

       
    }
}
