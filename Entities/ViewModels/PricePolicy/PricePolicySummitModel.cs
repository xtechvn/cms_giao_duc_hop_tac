using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.ViewModels.PricePolicy
{
    public class PricePolicySummitModel
    {
        public int CampaignId { get; set; }
        public string CampaignCode { get; set; }
        public int ClientTypeId { get; set; }
        public int GroupProductID { get; set; }
        public short ContractType { get; set; }
        public byte Status { get; set; }
        public string Description { get; set; }
        public DateTime FromDate { get; set; }
        public string FromDateStr { get; set; }
        public DateTime ToDate { get; set; }
        public string ToDateStr { get; set; }
        public double ServiceFee { get; set; }
        public short ServiceFeeType { get; set; }
        public List<string> Months { get; set; }
        public List<string> Days { get; set; }
        public string DaysString
        {
            get
            {
                var daysStr = new List<string>();
                foreach (var item in Days)
                {
                    switch (item)
                    {
                        case "Thứ Hai":
                            daysStr.Add(((int)DayOfWeek.Monday).ToString());
                            break;
                        case "Thứ Ba":
                            daysStr.Add(((int)DayOfWeek.Tuesday).ToString());
                            break;
                        case "Thứ Tư":
                            daysStr.Add(((int)DayOfWeek.Wednesday).ToString());
                            break;
                        case "Thứ Năm":
                            daysStr.Add(((int)DayOfWeek.Thursday).ToString());
                            break;
                        case "Thứ Sáu":
                            daysStr.Add(((int)DayOfWeek.Friday).ToString());
                            break;
                        case "Thứ Bảy":
                            daysStr.Add(((int)DayOfWeek.Saturday).ToString());
                            break;
                        case "Chủ Nhật":
                            daysStr.Add(((int)DayOfWeek.Sunday).ToString());
                            break;
                    }
                }
                return string.Join(",", daysStr);
            }
        }
        public List<PricePolicySummitDetail> PriceDetail { get; set; }
        public int Mode { get; set; }
    }
    public class PricePolicySummitDetail
    {
        public string ContractNo { get; set; }
        public string PackageId { get; set; }
        public string RoomID { get; set; }
        public string ProviderID { get; set; }
        public int? PriceDetailID { get; set; }
        public int CampaignID { get; set; }
        public double Price { get; set; }
        public double Profit { get; set; }
        public double AmountLast { get; set; }
        public short? UnitId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int ServiceType { get; set; }
        public int? ProductServiceId { get; set; }
    }
}
