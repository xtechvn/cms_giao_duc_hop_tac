using System;
using System.Collections.Generic;

namespace Entities.Models
{
    public partial class TourPackages
    {
        public long Id { get; set; }
        public long? TourId { get; set; }
        public string PackageName { get; set; }
        public string PackageCode { get; set; }
        public double? BasePrice { get; set; }
        public int? Quantity { get; set; }
        public double? AmountBeforeVat { get; set; }
        public double? AmountVat { get; set; }
        public double? Amount { get; set; }
        public double? Vat { get; set; }
        public double? UnitPrice { get; set; }
        public double? Profit { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
