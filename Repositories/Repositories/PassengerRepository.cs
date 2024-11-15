using DAL;
using Entities.ConfigModels;
using Entities.Models;
using Microsoft.Extensions.Options;
using Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities;

namespace Repositories.Repositories
{
    public class PassengerRepository : IPassengerRepository
    {
        
        private readonly PassengerDAL _passengerDAL;

        public PassengerRepository(IOptions<DataBaseConfig> dataBaseConfig)
        {

            _passengerDAL = new PassengerDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
        }

        public async Task<List<Passenger>> GetByOrderID(long order_id, string group_fly)
        {
            try
            {
                if (group_fly == null || group_fly.Trim() == "") return new List<Passenger>();
                return await _passengerDAL.GetByOrderId(order_id, group_fly);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetByOrderID - PassengerRepository: " + ex.ToString());
            }
            return new List<Passenger>();
        }
        public async Task<List<Passenger>> GetPassengerByOrderId(long order_id)
        {
            try
            {
                return await _passengerDAL.GetPassengerByOrderId(order_id);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetPassengerByOrderId - PassengerDAL. " + ex.ToString());
                return new List<Passenger>();
            }
        }
    }
}
