using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.ViewModels.PricePolicy
 {
    public class HotelPriceDetailSummitModel
     {
        public int CampaignID  { get; set; }
        public string ContractNo  { get; set; }
        public string PackageId  { get; set; }
        public string PackageName  { get; set; }
        public int PriceDetailID { get; set; }
        public double Price { get; set; }
        public double AmountLast { get; set; }
        public double Profit { get; set; }
        public int UnitId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string ProviderID  { get; set; }
        public List<string> DayList { get; set; }
        public List<string> MonthList { get; set; }
        public string DaysString
        {
            get
            {
                var daysStr = new List<string>();
                foreach (var item in DayList)
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
        public int ServiceType { get; set; }
        public string RoomID { get; set; }
        public int? GroupProviderType { get; set; }
        public string AllotmentsId { get; set; }
        public int ProductServiceId { get; set; }

    }
}
