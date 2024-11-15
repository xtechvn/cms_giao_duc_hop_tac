using System;
using System.Collections.Generic;

namespace Entities.Models
{
    public partial class HotelBookingRoomsOptional
    {
        public long Id { get; set; }
        public long HotelBookingId { get; set; }
        public long HotelBookingRoomId { get; set; }
        /// <summary>
        /// Giá đã trừ đi lợi nhuận
        /// </summary>
        public double Price { get; set; }
        /// <summary>
        /// Lợi nhuận
        /// </summary>
        public double Profit { get; set; }
        /// <summary>
        /// Tổng tiền dịch vụ khách phải trả
        /// </summary>
        public double TotalAmount { get; set; }
        public short? NumberOfRooms { get; set; }
        public int? SupplierId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string PackageName { get; set; }
        public bool? IsRoomFund { get; set; }
        public int? Status { get; set; }
    }
}
