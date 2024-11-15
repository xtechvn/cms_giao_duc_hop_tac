using System;
using System.Collections.Generic;

namespace Entities.Models
{
    public partial class Notify
    {
        public int Id { get; set; }
        public int UserSend { get; set; }
        public string Message { get; set; }
        public DateTime SendDate { get; set; }
        public long DataId { get; set; }
        public short DataType { get; set; }
    }
}
