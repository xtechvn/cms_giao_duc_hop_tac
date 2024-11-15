using Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.ViewModels
{
   public class PassengerViewModel : Passenger
    {
        public string luggageGo { get; set; }
        public string luggageBack { get; set; }
    }
}
