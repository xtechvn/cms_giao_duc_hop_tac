using System;
using System.Collections.Generic;

namespace Entities.Models
{
    public partial class RoomFacilities
    {
        public int Id { get; set; }
        public long? HotelRoomId { get; set; }
        public int? FacilityId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
