using System;
using System.Collections.Generic;

namespace Entities.Models
{
    public partial class VinWonderBookingTicketCustomer
    {
        public long Id { get; set; }
        public long? BookingId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime? Birthday { get; set; }
        public string Genre { get; set; }
        public string OtherDetail { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string Note { get; set; }
    }
}
