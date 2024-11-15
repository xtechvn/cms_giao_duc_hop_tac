using System;
using System.Collections.Generic;

namespace Entities.Models
{
    public partial class HotelContact
    {
        public int Id { get; set; }
        public int? HotelId { get; set; }
        public string Name { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Position { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
