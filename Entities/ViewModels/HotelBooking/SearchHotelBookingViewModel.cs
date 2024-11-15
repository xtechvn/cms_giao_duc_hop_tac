using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.ViewModels.HotelBooking
{
	public class SearchHotelBookingViewModel
	{
		public string ServiceCode { get; set; }
		public string OrderCode { get; set; }
		public string StatusBooking { get; set; }
		public string CheckinDateFrom { get; set; }
		public string CheckinDateTo { get; set; }
		public string CheckoutDateFrom { get; set; }
		public string CheckoutDateTo { get; set; }
		public string UserCreate { get; set; }
		public string CreateDateFrom { get; set; }
		public string CreateDateTo { get; set; }
		public string SalerPermission { get; set; }
		public string BookingCode { get; set; }
		public int SalerId { get; set; }
		public int OperatorId { get; set; }
		public int PageIndex { get; set; } = -1;
		public int PageSize { get; set; } = -1;

	}
	public class SearchHotelBookingModel
	{
		public string ServiceCode { get; set; }
		public string Id { get; set; }
		public string OrderNo { get; set; }
		public string OrderStatus { get; set; }
		public DateTime CheckinTime { get; set; }
		public DateTime CheckoutTime { get; set; }
		public DateTime CreatedDate { get; set; }
		public DateTime ArrivalDate { get; set; }
		public DateTime DepartureDate { get; set; }
		public string SalerName { get; set; }
		public string OperatorName { get; set; }
		public int Status { get; set; }
		public string BookingCode { get; set; }
		public string StatusName { get; set; }
		public string OrderId { get; set; }
		public string HotelName { get; set; }
		public double Amount { get; set; }
		public double Price { get; set; }
		public List<HotelBookingDetailModel> HotelBookingDetai { get; set; }


	}
	public class HotelBookingDetailModel
	{
		public long Id { get; set; }
		public string HotelName { get; set; }
		public string RoomTypeName { get; set; }
		public int TotalRooms { get; set; }
		public int TotalDays { get; set; }


	}
	public class HotelBookingDetailViewModel : Entities.Models.HotelBooking
	{
		public string UserUpdate { get; set; }
		public string UserCreate { get; set; }
		public string SalerName { get; set; }
		public string StatusName { get; set; }
		public string ContactClientId { get; set; }
		public string OrderNo { get; set; }
		public double Amount { get; set; }
		public double OrderPrice { get; set; }
		public double TotalAmountPaymentRequest { get; set; }
		public string SalerPhone { get; set; }
		public string ContactClientEmail { get; set; }
		public string ContactClientPhone { get; set; }
		public string ContactClientName { get; set; }
		public DateTime UpdatedDate { get; set; }
		public string SalerEmail { get; set; }
		public int OrderStatus { get; set; }
		public string SuplierName { get; set; }

	}
	public class TotalHotelBookingViewModel
	{
		public string Total { get; set; }
		public string Status { get; set; }
		public string StatusName { get; set; }
	}
}
