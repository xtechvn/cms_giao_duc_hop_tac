using System;
using System.Collections.Generic;

namespace Entities.Models
{
    public partial class HotelBookingCode
    {
        public long Id { get; set; }
        public long? OrderId { get; set; }
        public long? ServiceId { get; set; }
        public int? Type { get; set; }
        public string BookingCode { get; set; }
        public string Description { get; set; }
        public string AttactFile { get; set; }
        public bool? IsDelete { get; set; }
        public string Note { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
