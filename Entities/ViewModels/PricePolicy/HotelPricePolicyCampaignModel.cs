using Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.ViewModels.PricePolicy
{
    public class HotelPricePolicyCampaignModel
    {
        public Campaign Detail { get; set; }
        public List<HotelPricePolicyDetail> PricePolicy { get; set; }
        public List<HotelPricePolicyDetail> BasedProgram { get; set; }

    }
    public class HotelPricePolicyDetail
    {
        public int PriceDetailId { get; set; }
        public int ProductServiceId { get; set; }
        public string ProgramName { get; set; }
        public int HotelId { get; set; }
        public int ProgramId { get; set; }
        public string PackageCode { get; set; }
        public string PackageName { get; set; }
        public int PackageId { get; set; }
        public int RoomId { get; set; }

        public string RoomName { get; set; }
        public string AllotmentsId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public double Profit { get; set; }
        public short UnitId { get; set; }

    }
    public class HotelPricePolicySummitModel
    {
        public ProductRoomService Detail { get; set; }
        public List<PriceDetail> PriceDetail { get; set; }

    }

}
