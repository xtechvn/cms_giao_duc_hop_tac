using DAL.Generic;
using DAL.StoreProcedure;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace DAL
{
    public class ServicePiceRoomDAL : GenericService<ServicePiceRoom>
    {
        private static DbWorker _DbWorker;
        public ServicePiceRoomDAL(string connection) : base(connection)
        {
            _DbWorker = new DbWorker(connection);
        }
        public async Task<List<ServicePiceRoom>> GetHotelRoomList()
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var find = _DbContext.ServicePiceRoom.ToList();
                    return find;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetHotelRoomList - ServicePiceRoomDAL: " + ex);
                return null;
            }
        }

        public string GetCodeByID(string room_id)
        {
            using (var _DbContext = new EntityDataContext(_connection))
            {
                var find = _DbContext.ServicePiceRoom.FirstOrDefault(x => x.RoomId == room_id);
                if (find != null && find.Id > 0)
                {
                    return find.RoomCode;
                }
                else return "";
            }
        }

        public async Task<int> AddOrUpdateHotelRoomList(List<ServicePiceRoom> room_list)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                   foreach(var room in room_list)
                   {
                        var find = _DbContext.ServicePiceRoom.FirstOrDefault(x=>x.RoomId==room.RoomId && x.HotelId==room.HotelId);
                        if(find!=null && find.Id > 0)
                        {
                            find.Price = room.Price;
                            find.CreateDate = room.CreateDate;
                            find.RoomName = room.RoomName;
                            _DbContext.ServicePiceRoom.Update(find);
                        }
                        else
                        {
                            _DbContext.ServicePiceRoom.AddAsync(room);
                        }

                   }
                    _DbContext.SaveChanges();
                    return (int)ResponseType.SUCCESS;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("AddOrUpdateHotelRoomList - ServicePiceRoomDAL: " + ex);
                return (int)ResponseType.ERROR;
            }
        }
        public string GetNameByID(string room_id)
        {
            using (var _DbContext = new EntityDataContext(_connection))
            {
                var find = _DbContext.ServicePiceRoom.FirstOrDefault(x => x.RoomId == room_id);
                if (find != null && find.Id > 0)
                {
                    return find.RoomName;
                }
                else return "";
            }
        }
        public double GetPriceByRoomID(string room_id, string hotel_id)
        {
            using (var _DbContext = new EntityDataContext(_connection))
            {
                var find = _DbContext.ServicePiceRoom.FirstOrDefault(x => x.RoomId == room_id && x.HotelId==hotel_id);
                if (find != null && find.Id > 0)
                {
                    return (double)find.Price;
                }
                else return 0;
            }
        }
        public async Task<List<ServicePiceRoom>> GetByRoomPackageIds(List<int> packages_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return await _DbContext.ServicePiceRoom.Where(x => packages_id.Contains((int)x.RoomPackageId)).ToListAsync();


                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetByRoomPackageIds - ServicePiceRoomDAL: " + ex);
            }
            return null;
        }
    }
}
