using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.ViewModels.OrderManual
{
   public class TotalCustomerCareFundViewModel
    {
        public double TotalFundCustomerCare { get; set; }
        public double TotalAmountPaymentRequestComplete { get; set; }
        public double TotalAmountPaymentRequestPending { get; set; }
    }
}
