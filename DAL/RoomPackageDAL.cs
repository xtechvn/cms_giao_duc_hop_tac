using DAL.Generic;
using DAL.StoreProcedure;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;

namespace DAL
{
    public class RoomPackageDAL : GenericService<RoomPackage>
    {
        private static DbWorker _DbWorker;
        public RoomPackageDAL(string connection) : base(connection)
        {
            _DbWorker = new DbWorker(connection);
        }
        public string GetPackageNameByID(string package_id)
        {
            try
            {
                if (package_id == "-1")
                {
                    return "Default Package";
                }
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var find = _DbContext.RoomPackage.FirstOrDefault(x => x.PackageId == package_id);
                    if (find != null && find.Id > 0)
                    {
                        return find.Description;
                    }
                    else return "";
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetPackageNameByID - RoomPackageDAL: " + ex);
                return null;
            }
        }
        public string GetNameByID(string package_id)
        {
            using (var _DbContext = new EntityDataContext(_connection))
            {
                var find = _DbContext.RoomPackage.FirstOrDefault(x => x.PackageId == package_id);
                if (find != null && find.Id > 0)
                {
                    return find.Code;
                }
                else return "";
            }
        }
        public List<RoomPackage> GetListByListRoomFunID(List<int> roomfun_ids)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.RoomPackage.Where(x => roomfun_ids.Contains((int)x.RoomFunId)).ToList();

                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetListByListRoomFunID - RoomPackageDAL: " + ex);
                return null;
            }
        }
        
    }
}
