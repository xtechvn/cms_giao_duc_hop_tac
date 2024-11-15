using Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.ViewModels.SetServices
{
    public class FlyOperatorOrderPriceSummitViewModel
    {
        public string group_booking_id { get; set; }
        public List<FlyOperatorOrderPriceSummitViewModelPackages> list { get; set; }

    }
    public class FlyOperatorOrderPriceSummitViewModelPackages
    {
        public long id { get; set; }
        public double price { get; set; }

    }
    public class FlyOperatorOrderPriceSummitSQLModel
    {
        public List<FlyBookingDetail> detail { get; set; }
        public List<FlyBookingExtraPackages> extra { get; set; }
    }
}
