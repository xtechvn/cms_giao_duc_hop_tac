using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.ViewModels.PlaygroundDetai
{
   public class PlaygroundDetaiViewModel
    {
       public long  Id { get; set; }
       public long  Code { get; set; }
       public long NewsId { get; set; }
       public long ServiceType { get; set; }
       public string LocationName { get; set; }
       public string ServiceName { get; set; }
       public string Title { get; set; }
       public DateTime CreateDate { get; set; }
       public DateTime UpdateTime { get; set; }
       public long Status { get; set; }
       public string StatusName { get; set; }
       public string Description { get; set; }
       public string NewsIdName { get; set; }
    }
    public class PlaygroundDetaiSeachViewModel
    {
        public string LocationName { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }


    }
}
