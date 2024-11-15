using System;
using System.Collections.Generic;

namespace Entities.Models
{
    public partial class PriceLimitedSetting
    {
        public int Id { get; set; }
        public int Offset { get; set; }
        public int Limit { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
        public int? UnitId { get; set; }
        public int? ServiceType { get; set; }
    }
}
