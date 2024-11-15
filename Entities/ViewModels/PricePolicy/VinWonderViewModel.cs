using Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.ViewModels
{
    public class CampaignModel : Campaign
    {
        public int UserAction { get; set; }
    }

    public class CampaignViewModel : CampaignModel
    {
        public IEnumerable<VinWonderPricePolicyModel> PricePolycies { get; set; }
    }

    public class VinWonderPricePolicyModel
    {
        public int Id { get; set; }
        public int CampaignId { get; set; }
        public int ServiceId { get; set; }
        public string ServiceCode { get; set; }
        public string Name { get; set; }
        public decimal BasePrice { get; set; }
        public decimal? WeekendRate { get; set; }
        public int SiteId { get; set; }
        public string SiteName { get; set; }
        public int UnitType { get; set; }
        public decimal Profit { get; set; }
        public int RateCode { get; set; }
        public int UserAction { get; set; }
    }

    public class VinWonderCommonProfitModel : AllCode
    {
        public int UserAction { get; set; }
    }
}
