using System;
using System.Collections.Generic;

namespace Entities.Models
{
    public partial class GroupClassAirlines
    {
        public int Id { get; set; }
        public string Airline { get; set; }
        public string ClassCode { get; set; }
        public string FareType { get; set; }
        public string DetailVi { get; set; }
        public string DetailEn { get; set; }
        public string Description { get; set; }
    }
}
