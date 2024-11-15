﻿using System;
using System.Collections.Generic;

namespace Entities.Models
{
    public partial class ContactClient
    {
        public ContactClient()
        {
            Order = new HashSet<Order>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public long ClientId { get; set; }
        public long? OrderId { get; set; }
        public DateTime CreateDate { get; set; }

        public virtual ICollection<Order> Order { get; set; }
    }
}
