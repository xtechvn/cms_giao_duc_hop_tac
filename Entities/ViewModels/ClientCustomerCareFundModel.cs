using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ViewModels
{
    public class ClientCustomerCareFundModel
    {
        public long ClientId { get; set; }
        public string ClientName { get; set; }
        public string ClientCode { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public double TotalFundCustomerCare { get; set; }
   
    }
}
