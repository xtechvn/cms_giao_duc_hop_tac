using System;
using System.Collections.Generic;

namespace Entities.Models
{
    public partial class PaymentAccount
    {
        public int Id { get; set; }
        public string AccountNumb { get; set; }
        public string AccountName { get; set; }
        public string BankName { get; set; }
        public string Branch { get; set; }
        public long ClientId { get; set; }
    }
}
