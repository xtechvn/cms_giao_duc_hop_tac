using Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.ViewModels.Tour
{
    public class TourPackagesOptionalViewModel : TourPackagesOptional
    {
        public string SupplierName { get; set; }
        public string PackageidName { get; set; }
        public string ShortName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public decimal TotalAmountPay { get; set; }
    }
}
