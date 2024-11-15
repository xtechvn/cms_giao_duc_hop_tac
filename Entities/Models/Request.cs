using System;
using System.Collections.Generic;

namespace Entities.Models
{
    public partial class Request
    {
        public int RequestId { get; set; }
        public int? RoomTypeId { get; set; }
        public int? PackageId { get; set; }
        public int? HotelId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public double? Price { get; set; }
        public string Note { get; set; }
        public int? Status { get; set; }
        public int? SalerId { get; set; }
        public long? OrderId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
