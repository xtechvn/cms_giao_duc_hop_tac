using Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.ViewModels.SetServices
{
    public class FlyBookingPackagesOptionalViewModel: FlyBookingPackagesOptional
    {
        public Supplier supplier { get; set; }
        public decimal TotalAmountPay { get; set; }
        public string SupplierName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int SupplierId { get; set; }

    }
}
