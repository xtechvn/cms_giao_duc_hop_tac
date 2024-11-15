using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.ViewModels
{
   public class OtherBookingViewModel : Entities.Models.OtherBooking
    {
        public string ServiceName { get; set; }
        public string OperatorName { get; set; }
    }
}
