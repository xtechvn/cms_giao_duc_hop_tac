using System;
using System.Collections.Generic;

namespace Entities.Models
{
    public partial class AirPortCode
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string DistrictEn { get; set; }
        public string DistrictVi { get; set; }
        public int? CountryId { get; set; }
    }
}
