using System;
using System.Collections.Generic;

namespace Entities.Models
{
    public partial class SportWaterGuests
    {
        public long Id { get; set; }
        public int? ClientId { get; set; }
        public string UserName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string RoomNo { get; set; }
        public int? National { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
