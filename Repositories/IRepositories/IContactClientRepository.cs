using Entities.Models;
using Entities.ViewModels;
using Entities.ViewModels.Contract;
using Entities.ViewModels.CustomerManager;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.IRepositories
{
    public interface IContactClientRepository
    {
        Task<ContactClient> GetByBookingId(long hotel_booking_id);
        Task<long> UpdateContactClient(ContactClient client);
        ContactClient GetByContactClientId(long Id);
    }
}
