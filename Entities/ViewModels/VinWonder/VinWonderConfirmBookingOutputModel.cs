using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.ViewModels.VinWonder
{
    public  class VinWonderConfirmBookingOutputModel
    {
        public int status { get; set; }
        public List<VinWonderConfirmBookingOutputData?> data { get; set; }
        public string email { get; set; }


    }

    public class VinWonderConfirmBookingOutputData
    {
        public VinWonderConfirmBookingOutputDataResult Result { get; set; }
        public VinWonderConfirmBookingOutputDataData? Data { get; set; }
    }

    public class VinWonderConfirmBookingOutputDataData
    {
        public string BookingCode { get; set; }
        public string ReferenceCode { get; set; }
        public string InvoiceCode { get; set; }
        public double TotalAmount { get; set; }
        public List<VinWonderConfirmBookingOutputDataDataTickets>? Tickets { get; set; }

    }

    public class VinWonderConfirmBookingOutputDataDataTickets
    {
        public string CardID { get; set; }
        public string QrCode { get; set; }
        public string QrCodeUrl { get; set; }
        public string ServiceCode { get; set; }
        public string ServiceName { get; set; }
        public DateTimeUsed? DateTimeUsed { get; set; }

    }
    public class VinWonderConfirmBookingOutputDataDataTicketsSummitModel : VinWonderConfirmBookingOutputDataDataTickets
    {
        public string BookingCode { get; set; }

    }
    public class DateTimeUsed
    {
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public string GateCode { get; set; }
        public string GateName { get; set; }
        public string WeekDays { get; set; }
        public int? NumberOfUses { get; set; }
        public int? DateUsed { get; set; }
        public string TimeStart { get; set; }
        public string MinuteStart { get; set; }
        public string TimeEnd { get; set; }
        public string MinuteEnd { get; set; }
    }

    public class VinWonderConfirmBookingOutputDataResult
    {
        public string Code { get; set; }
        public string Message { get; set; }
    }
}
