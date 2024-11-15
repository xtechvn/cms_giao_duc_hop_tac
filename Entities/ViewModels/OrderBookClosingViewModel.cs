using System;
using System.Collections.Generic;
using System.Text;
using Utilities;

namespace Entities.ViewModels
{
   public class OrderBookClosingViewModel
    {
        public string FromDateStr { get; set; }
        public string ToDateStr { get; set; }
        public DateTime? FromDate
        {
            get
            {
                return DateUtil.StringToDate(FromDateStr);
            }
        } 
        public DateTime ToDate { get; set; }

        public long UserFinalize { get; set; }
    }
}
