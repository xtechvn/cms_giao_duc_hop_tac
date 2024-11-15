using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.ViewModels
{
   public class CloseTheBookViewModel
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public bool Status { get; set; }
     
    }
}
