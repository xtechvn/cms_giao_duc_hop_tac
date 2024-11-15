using DAL.Generic;
using DAL.StoreProcedure;
using Entities.Models;
using Entities.ViewModels.PricePolicy;
using ENTITIES.APPModels.PushHotel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace DAL
{
    public class RoomFunDAL : GenericService<RoomFun>
    {
        private static DbWorker _DbWorker;
        public RoomFunDAL(string connection) : base(connection)
        {
            _DbWorker = new DbWorker(connection);
        }
        public async Task<int> CreateOrUpdateRoomFun(HotelContract detail)
        {
            int result = (int)ResponseType.FAILED;
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var exists = _DbContext.RoomFun.FirstOrDefault(x => x.AllotmentId == detail.contract.AllotmentId && x.HotelId == detail.contract.HotelId);
                    if (exists != null && exists.Id > 0)
                    {
                        detail.contract.Id = exists.Id;
                    }
                    else
                    {
                        _DbContext.RoomFun.Add(detail.contract);
                        _DbContext.SaveChanges();
                    }
                    if (detail.contract.Id > 0 && detail.packages_list != null && detail.packages_list.Count > 0)
                    {
                        foreach (var package in detail.packages_list)
                        {
                            var exists_package = _DbContext.RoomPackage.FirstOrDefault(x => x.PackageId == package.package.PackageId && x.Code == package.package.Code && x.RoomFunId == detail.contract.Id);
                            if (exists_package != null && exists_package.Id > 0)
                            {
                                package.package.Id = exists_package.Id;
                            }
                            else
                            {
                                package.package.RoomFunId = detail.contract.Id;
                                _DbContext.RoomPackage.Add(package.package);
                                _DbContext.SaveChanges();
                            }
                            if (package.package.Id > 0 && package.room_list != null && package.room_list.Count > 0)
                            {
                                foreach (var room in package.room_list)
                                {
                                    room.RoomPackageId = package.package.Id;
                                    var exists_room = _DbContext.ServicePiceRoom.FirstOrDefault(x => x.RoomId == room.RoomId && x.RoomPackageId == package.package.Id);
                                    if (exists_room != null && exists_room.Id > 0)
                                    {
                                        exists_room.RoomName = room.RoomName;
                                        _DbContext.ServicePiceRoom.Update(exists_room);
                                    }
                                    else
                                    {
                                        room.RoomPackageId = package.package.Id;
                                        _DbContext.ServicePiceRoom.Add(room);
                                    }
                                }
                                _DbContext.SaveChanges();
                            }
                        }
                    }
                }
                result = (int)ResponseType.SUCCESS;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CreateOrUpdateRoomFun - RoomFunDAL: " + ex);
                result = (int)ResponseType.ERROR;
            }
            return result;
        }
        public List<RoomFun> GetByContractAndHotel(string hotel_id, int contract_type)
        {
            List<RoomFun> result = new List<RoomFun>();
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    result = _DbContext.RoomFun.Where(x => x.ContractType == contract_type && x.HotelId == hotel_id).ToList();

                }

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetByContractAndHotel - RoomFunDAL: " + ex);
            }
            return result;
        }
        public string GetNameByID(string allotment_id)
        {
            using (var _DbContext = new EntityDataContext(_connection))
            {
                var find = _DbContext.RoomFun.FirstOrDefault(x => x.AllotmentId == allotment_id);
                if (find != null && find.Id > 0)
                {
                    return find.AllotmentName;
                }
                else return "";
            }
        }

    }
}
