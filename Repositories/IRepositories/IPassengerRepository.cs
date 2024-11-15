using Entities.Models;
using Entities.ViewModels;
using Entities.ViewModels.OrderManual;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.IRepositories
{
    public interface IPassengerRepository
    {
        Task<List<Passenger>> GetByOrderID(long order_id, string group_fly);
        Task<List<Passenger>> GetPassengerByOrderId(long order_id);
    }
}
