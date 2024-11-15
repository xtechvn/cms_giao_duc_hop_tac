using System;
using System.Collections.Generic;

namespace Entities.Models
{
    public partial class DepositHistory
    {
        public int Id { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateLast { get; set; }
        public long? UserId { get; set; }
        public string TransNo { get; set; }
        public string Title { get; set; }
        public double? Price { get; set; }
        public short? TransType { get; set; }
        public short? PaymentType { get; set; }
        public int? Status { get; set; }
        public string ImageScreen { get; set; }
        public short? ServiceType { get; set; }
        public string BankName { get; set; }
        public long? UserVerifyId { get; set; }
        public DateTime? VerifyDate { get; set; }
        public string NoteReject { get; set; }
        public long? ClientId { get; set; }
        public string BankAccount { get; set; }
        public bool? IsFinishPayment { get; set; }
    }
}
