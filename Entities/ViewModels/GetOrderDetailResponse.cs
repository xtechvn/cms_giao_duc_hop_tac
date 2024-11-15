using Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.ViewModels
{
    public class GetOrderDetailResponse
    {
        public string bookingCode { get; set; }
        public int? paymentType { get; set; }
        public string paymentTypeName { get; set; }
        public string createTime { get; set; }
        public string name { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        
        public string startPoint { get; set; }
        public string startDistrict { get; set; }
        public string startime { get; set; }
        public string endPoint { get; set; }
        public string endDistrict { get; set; }
        public string endtime { get; set; }
        public string flightNumber { get; set; }
        public int clientId { get; set; }
        public string orderNo { get; set; }
        public bool? paymentStatus { get; set; }
        public string paymentStatusName { get; set; }
        public string bookingId { get; set; }
        public double amount { get; set; }
        public string airlineLogo { get; set; }
        public string airlineName_Vi { get; set; }
        public string sessionid { get; set; }
        public int leg { get; set; }
        public bool is_lock { get; set; }

        public int orderId { get; set; }
        public List<Passenger> passenger { get; set; }
        
    }
  
    public class OrderViewdetail
    {

        public long? orderId { get; set; }
        public long? clientId { get; set; }
        public long? accountClientId { get; set; }

        public string orderNo { get; set; }
        public string userName { get; set; }
        public string clientName { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public double? amount { get; set; }//tông tiền
        public double orderAmount { get; set; }//tông tiền
        public string order_status_name { get; set; }
        public byte? orderStatus { get; set; }

        public string? createTime { get; set; }
        public string? expriryDate { get; set; }
        public int? paymentStatus { get; set; }//đã thanh toán hay chưa 0-null / 1 rồi
        public string payment_Status_name { get; set; }//đã thanh toán hay chưa 0-null / 1 rồi
        public double? profit { get; set; }
        public string paymentNo { get; set; }//mã thanh toán
        public string paymentDate { get; set; }
        public int paymentAmount { get; set; }
        public long? UserUpdateId { get; set; }
        public long? UserCreateId { get; set; }
        public string? UpdateLast { get; set; }
        public double Price { get; set; }
        public double Discount { get; set; }
        public string voucher_code { get; set; }
        public int VoucherId { get; set; }
        public List<ListPayment> listPayment { get; set; }
        public List<ListPassenger> passenger { get; set; }
        public List<Bookingdetail> bookingdetail { get; set; }
        public OrderViewdetail()
        {
            listPayment = new List<ListPayment>();
            passenger = new List<ListPassenger>();
            bookingdetail = new List<Bookingdetail>();
        }
    }
   
    public class Bookingdetail
    {
        public long Id { get; set; }
        public long? OrderId { get; set; }
        public string FlightNumber { get; set; }//mã chuyến bay   
        public string StartPoint { get; set; }
        public string StartDistrict { get; set; }
        public string StartDistrict2 { get; set; }
        public string EndPoint { get; set; }
        public string EndDistrict { get; set; }
        public string EndDistrict2 { get; set; }
        public string BookingCode { get; set; }
        public string BookingCode2 { get; set; }
        public double Amount { get; set; }
        public double Discount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? Leg { get; set; }
        public int? Leg2 { get; set; }
        public string AirlineLogo { get; set; }
        public string AirlineName_Vi { get; set; }
        public string AirlineName_Vi2 { get; set; }
        public string GroupBookingId { get; set; }
        public string SalerIdName { get; set; }
        public string ServiceCode { get; set; }
        public int AdultNumber { get; set; }
        public int ChildNumber { get; set; }
        public int InfantNumber { get; set; }
        public int SalerId { get; set; }
        public int Status { get; set; }
    }
    public class ListPayment : Entities.Models.Payment
    {
        public string Payment_Type_Name { get; set; }//tên kiểu thanh toán
        public string PaymentDate { get; set; }//tên kiểu thanh toán
    }
    public class ListPassenger : Entities.Models.Passenger
    {
        public string Person_Type_Name { get; set; }
        public string Gender_Name { get; set; }
    }
}
