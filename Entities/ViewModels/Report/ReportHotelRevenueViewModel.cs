using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.ViewModels.Report
{
    public class ReportHotelRevenueViewModel
    {
        public string HotelName { get; set; }
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
        public int RoomNights { get; set; }
        public int TotalRow { get; set; }
    }
}
