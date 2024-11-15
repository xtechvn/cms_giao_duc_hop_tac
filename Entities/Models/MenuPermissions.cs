using System;
using System.Collections.Generic;

namespace Entities.Models
{
    public partial class MenuPermissions
    {
        public int Id { get; set; }
        public int MenuId { get; set; }
        public int PermissionId { get; set; }
    }
}
