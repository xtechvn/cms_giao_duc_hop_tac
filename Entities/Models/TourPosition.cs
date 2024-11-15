using System;
using System.Collections.Generic;

namespace Entities.Models
{
    public partial class TourPosition
    {
        public long Id { get; set; }
        public int? TourId { get; set; }
        public short? PositionType { get; set; }
        public int? Position { get; set; }
        public int? Status { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
