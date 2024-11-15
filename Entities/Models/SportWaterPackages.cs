using System;
using System.Collections.Generic;

namespace Entities.Models
{
    public partial class SportWaterPackages
    {
        public long Id { get; set; }
        public int? SportWaterId { get; set; }
        public double? Price { get; set; }
        public DateTime? EffectDate { get; set; }
        public DateTime? ExpireDate { get; set; }
        public int? Status { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
