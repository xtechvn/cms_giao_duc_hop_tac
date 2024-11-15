using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ViewModels.Request
{
    public class RequestSearchModel
    {
        public string RequestId { get; set; }
        public string SalerId { get; set; }
        public long ClientId { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
