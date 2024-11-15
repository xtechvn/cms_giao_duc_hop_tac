using System;
using System.Collections.Generic;

namespace Entities.Models
{
    public partial class UserAgent
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public long ClientId { get; set; }
        public short? MainFollow { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateLast { get; set; }
        public DateTime? VerifyDate { get; set; }
        public int? VerifyStatus { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
