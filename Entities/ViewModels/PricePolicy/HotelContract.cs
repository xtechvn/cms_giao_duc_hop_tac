using Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ENTITIES.APPModels.PushHotel
{
    public class HotelContract
    {
        public RoomFun contract { get; set; }
        public List<RoomPackagesDetail> packages_list { get; set; }
    }
    public class RoomPackagesDetail
    {
        public RoomPackage package { get; set; }
        public List<ServicePiceRoom> room_list { get; set; }
    }
}
