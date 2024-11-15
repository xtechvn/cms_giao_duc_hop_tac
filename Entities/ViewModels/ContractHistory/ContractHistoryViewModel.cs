using System;
using System.Collections.Generic;
using System.Text;
using Entities.Models;

namespace Entities.ViewModels.ContractHistory
{
   public class ContractHistoryViewModel : Entities.Models.ContractHistory
    {
        public string Fullname { get; set; }
        public string ActionName { get; set; }
    }
}
