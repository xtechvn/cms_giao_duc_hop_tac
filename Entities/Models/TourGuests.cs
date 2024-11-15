using System;
using System.Collections.Generic;

namespace Entities.Models
{
    public partial class TourGuests
    {
        public long Id { get; set; }
        public long? TourId { get; set; }
        public string FullName { get; set; }
        public DateTime? Birthday { get; set; }
        public string Phone { get; set; }
        public string Note { get; set; }
        public string RoomNumber { get; set; }
        public string Cccd { get; set; }
        public short? Gender { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
