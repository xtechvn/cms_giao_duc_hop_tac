using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.APPModels.PushHotel
{
    public class HotelRoomContract
    {
        public string date { get; set; }
        public int timestamp { get; set; }
        public int price { get; set; }
        public int allotment { get; set; }
    }

    public class HotelRoomPriceSummary
    {
        public List<HotelRoomContract> contract { get; set; }
        public List<HotelRoomPromotion> promotion { get; set; }
    }

    public class HotelRoomPromotion
    {
        public string date { get; set; }
        public int timestamp { get; set; }
        public int price { get; set; }
        public int allotment { get; set; }
    }

    public class HotelRoomFRViewModel
    {
        public int id { get; set; }
        public int mapping_code { get; set; }
        public string name { get; set; }
        public string vin_id { get; set; }
        public HotelRoomPriceSummary price_summary { get; set; }
    }
}
