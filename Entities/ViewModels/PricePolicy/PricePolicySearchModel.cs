using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.ViewModels
{
    public class PricePolicySearchModel
    {
      public int  ServiceType { get; set; }
      public string CampaignCode { get; set; }
      public string CampaignDescription { get; set; }
      public DateTime FromDate { get; set; }
      public DateTime ToDate { get; set; }
      public string CampaginStatus { get; set; }
      public string ClientType { get; set; }
      public string ProviderName { get; set; }
    }
}
