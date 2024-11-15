using Entities.Models;
using Entities.ViewModels;
using Entities.ViewModels.HotelBookingRoom;
using Entities.ViewModels.OrderManual;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.IRepositories
{
    public interface IHotelBookingRoomRepository
    {
        Task<List<HotelBookingRooms>> GetByHotelBookingID(long hotel_booking_id);
        Task<List<HotelBookingRoomViewModel>> GetHotelBookingRoomByHotelBookingID(long HotelBookingId, long status);
        Task<long> UpdateHotelBookingUnitPrice(HotelBookingUnitPriceChangeSummitModel data, long hotel_booking_id, int user_update);
        Task<List<HotelBookingRoomRatesOptionalViewModel>> GetHotelBookingRoomRatesOptionalByBookingId(long HotelBookingId);

    }
}
