using System;
using System.Collections.Generic;

namespace Entities.Models
{
    public partial class PaymentRequestSupplier
    {
        public long Id { get; set; }
        public int RequestId { get; set; }
        public long? SupplierId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
