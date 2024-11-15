using System;
using System.Collections.Generic;

namespace Entities.Models
{
    public partial class OrderBookClosing
    {
        public int Id { get; set; }
        public long OrderId { get; set; }
        public double? Amount { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public DateTime? FinalizeDate { get; set; }
        public int? Status { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
