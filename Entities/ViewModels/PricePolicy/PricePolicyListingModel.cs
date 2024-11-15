using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.ViewModels
{
    public class PricePolicyListingModel
    {

        public int Id { get; set; }
        public string CampaignCode { get; set; }
        public string CampaignDescription { get; set; }
        public double Profit { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public DateTime UpdateLast { get; set; }
        public DateTime CreateDate { get; set; }
        public string ClientTypeName { get; set; }
        public string ServiceTypeName { get; set; }
        public string UnitName { get; set; }
        public string StatusName { get; set; }
        public int CampaginStatus { get; set; }
        public int ClientTypeId { get; set; }
        public int ServiceType { get; set; }
        public string ProviderID { get; set; }
        public string ProviderName { get; set; }
        public string FullName { get; set; }

    }
}
