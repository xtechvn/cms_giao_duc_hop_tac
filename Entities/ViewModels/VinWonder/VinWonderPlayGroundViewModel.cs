using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.ViewModels.VinWonder
{
    public class ListVinWonderemialViewModel
    {
        public string SiteName { get; set; }
        public DateTime DateUsed { get; set; }
        public int adt { get; set; }
        public int child { get; set; }
        public int old { get; set; }
        public string Name { get; set; }
        public string typeCode { get; set; }
        public long BookingId { get; set; }
        public long BookingTicketId { get; set; }
    }
}
