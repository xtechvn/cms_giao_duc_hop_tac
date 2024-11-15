using Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.ViewModels
{
    public class ProductFlyTicketViewModel
    {
        public Campaign Campaign { get; set; }
        public PriceDetail PriceDetail { get; set; }
        public List<string> MonthList { get; set; }
        public List<string> DayList { get; set; }
    }
}
