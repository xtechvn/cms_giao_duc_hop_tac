using System;
using System.Collections.Generic;

namespace Entities.Models
{
    public partial class Passenger
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string MembershipCard { get; set; }
        public string PersonType { get; set; }
        public DateTime? Birthday { get; set; }
        public bool Gender { get; set; }
        public long OrderId { get; set; }
        public string Note { get; set; }
        public string GroupBookingId { get; set; }

        public virtual Order Order { get; set; }
    }
}
