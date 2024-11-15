using System;
using System.Collections.Generic;

namespace Entities.Models
{
    public partial class RoomPackage
    {
        public RoomPackage()
        {
            ServicePiceRoom = new HashSet<ServicePiceRoom>();
        }

        public int Id { get; set; }
        public string PackageId { get; set; }
        public string Code { get; set; }
        public int? RoomFunId { get; set; }
        public string Description { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateLast { get; set; }

        public virtual RoomFun RoomFun { get; set; }
        public virtual ICollection<ServicePiceRoom> ServicePiceRoom { get; set; }
    }
}
