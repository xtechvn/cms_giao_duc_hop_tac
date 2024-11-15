using DAL;
using Entities.ConfigModels;
using Entities.Models;
using Microsoft.Extensions.Options;
using Repositories.IRepositories;
using System;
using Utilities;

namespace Repositories.Repositories
{
    public class UserAgentRepository: IUserAgentRepository
    {
  
        private readonly UserAgentDAL _userAgentDAL;
     

        public UserAgentRepository(IOptions<DataBaseConfig> dataBaseConfig)
        {
            _userAgentDAL=new UserAgentDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
        }
        public UserAgent GetUserAgentClient(int ClientId)
        {
            try
            {
              
                return _userAgentDAL.GetUserAgentClient(ClientId);
          

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetUserAgentClient - UserAgentRepository: " + ex);
                return null;
            }
        }
    }
}
