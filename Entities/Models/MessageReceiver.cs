using System;
using System.Collections.Generic;

namespace Entities.Models
{
    public partial class MessageReceiver
    {
        public long Id { get; set; }
        public int? ReceiverId { get; set; }
        public short? SeenStatus { get; set; }
        public int? NotifyId { get; set; }
        public DateTime? SeenDate { get; set; }
    }
}
