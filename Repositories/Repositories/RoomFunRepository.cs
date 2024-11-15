using DAL;
using Entities.ConfigModels;
using Entities.ViewModels.PricePolicy;
using ENTITIES.APPModels.PushHotel;
using Microsoft.Extensions.Options;
using Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Repositories.Repositories
{
    public class RoomFunRepository : IRoomFunRepository
    {
        private readonly RoomFunDAL _roomFunDAL;
        private readonly RoomPackageDAL _roomPackageDAL;
        private readonly ServicePiceRoomDAL _servicePiceRoomDAL;

        public RoomFunRepository(IOptions<DataBaseConfig> dataBaseConfig, IOptions<MailConfig> mailConfig)
        {
            _roomFunDAL = new RoomFunDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            _roomPackageDAL = new RoomPackageDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            _servicePiceRoomDAL = new ServicePiceRoomDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
        }

        public async Task<int> CreateOrUpdateRoomFun(HotelContract detail)
        {
            return await _roomFunDAL.CreateOrUpdateRoomFun(detail);
        }

        public string GetNameByID(string allotment_id)
        {
            return _roomFunDAL.GetNameByID(allotment_id);
        }

        public async Task<List<HotelPricePolicyDetail>> GetPolicyByProvider( int campaign_id, string hotel_id, int contract_type)
        {
            List<HotelPricePolicyDetail> result = new List<HotelPricePolicyDetail>();
            try
            {
                List<HotelContract> contract_list = new List<HotelContract>();
                var room_fun_list = _roomFunDAL.GetByContractAndHotel(hotel_id, contract_type);
                var packages_all_list = _roomPackageDAL.GetAll();
                var room_list = _servicePiceRoomDAL.GetAll();
                if (room_fun_list != null && room_fun_list.Count > 0)
                {
                    foreach (var room_fun in room_fun_list)
                    {
                        HotelContract contract = new HotelContract();
                        contract.contract = room_fun;
                        contract.packages_list = new List<RoomPackagesDetail>();
                        var packages_list = packages_all_list.Where(x => x.RoomFunId == contract.contract.Id).ToList();
                        if (packages_list != null && packages_list.Count > 0)
                        {
                            foreach (var package in packages_list)
                            {
                                contract.packages_list.Add(new RoomPackagesDetail()
                                {
                                    package = package,
                                    room_list = room_list.Where(x => x.RoomPackageId == package.Id && x.HotelId == hotel_id).ToList()
                                });
                            }
                        }
                        contract_list.Add(contract);
                    }
                }
                if (contract_list.Count > 0)
                {
                    foreach (var contract in contract_list)
                    {
                        if (contract != null && contract.packages_list != null && contract.packages_list.Count > 0)
                            foreach (var package in contract.packages_list)
                            {

                                foreach (var room in package.room_list)
                                {
                                  
                                }
                            }
                    }
                }


            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetPolicyByProvider - RoomFunRepository: " + ex);
            }
            return result;
        }
    }
}
