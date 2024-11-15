using System;
using System.Collections.Generic;

namespace Entities.Models
{
    public partial class HotelGuest
    {
        public long Id { get; set; }
        public long HotelBookingRoomsId { get; set; }
        public string Name { get; set; }
        public DateTime? Birthday { get; set; }
        public long HotelBookingId { get; set; }
        public string Note { get; set; }
        public short? Type { get; set; }
    }
}
