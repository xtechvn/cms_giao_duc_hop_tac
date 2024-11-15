using Entities.Models;
using Entities.ViewModels;
using Entities.ViewModels.OrderManual;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.IRepositories
{
    public interface IHotelBookingRoomRatesRepository
    {
        Task<List<HotelBookingRoomRates>> GetByHotelBookingID(long hotel_booking_id);



    }
}
