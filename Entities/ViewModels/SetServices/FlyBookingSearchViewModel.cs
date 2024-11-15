using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.ViewModels.SetServices
{
    public class FlyBookingSearchViewModel
    {
        public string ServiceCode { get; set; }
        public string OrderNo { get; set; }
        public string OrderStatusName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public string SalerName { get; set; }
        public string OperatorName { get; set; }
        public int OrderStatus  { get; set; }
        public long TotalRow { get; set; }
        public long FlyBookingDetailId { get; set; }
        public string GroupBookingId { get; set; }
        public string StartPoint { get; set; }
        public string EndPoint { get; set; }
        public string GoFlightNumber { get; set; }
        public string GoAirLines { get; set; }
        public string GoBookingCode { get; set; }
        public string BackFlightNumber { get; set; }
        public string BackAirLines { get; set; }
        public string BackBookingCode { get; set; }
        public int PassengerNumber { get; set; }
        public long OrderId { get; set; }
        public string DepartmentName { get; set; }
        public string FlyBookingStatusName { get; set; }
        public int FlyBookingStatus { get; set; }
        public string BookingCode { get; set; }

        public string SalerFullName { get; set; }
        public string SalerEmail { get; set; }
        public double? AmountGo { get; set; }
        public double? AmountBack { get; set; }
        public double? PriceGo { get; set; }
        public double? PriceBack { get; set; }

    }
}
