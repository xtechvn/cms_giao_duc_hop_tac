using System;
using System.Collections.Generic;

namespace Entities.Models
{
    public partial class KeyType
    {
        public int Id { get; set; }
        public int? Type { get; set; }
        public string Code { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
