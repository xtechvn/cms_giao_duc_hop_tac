using Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.ViewModels
{
   public class HotelBookingRoomRatesViewModel: HotelBookingRoomRates
    {
        public DateTime EndDate { get; set; }
    }
}
