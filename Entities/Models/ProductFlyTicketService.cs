using System;
using System.Collections.Generic;

namespace Entities.Models
{
    public partial class ProductFlyTicketService
    {
        public int Id { get; set; }
        public int CampaignId { get; set; }
        public string GroupProviderType { get; set; }

        public virtual Campaign Campaign { get; set; }
    }
}
