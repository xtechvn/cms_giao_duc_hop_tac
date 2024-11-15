using DAL;
using Entities.ConfigModels;
using Entities.Models;
using Microsoft.Extensions.Options;
using Repositories.IRepositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories.Repositories
{
    public class HotelBookingRoomRatesRepository : IHotelBookingRoomRatesRepository
    {
        
        private readonly HotelBookingRoomRatesDAL _hotelBookingRoomRatesDAL;

        public HotelBookingRoomRatesRepository(IOptions<DataBaseConfig> dataBaseConfig)
        {

            _hotelBookingRoomRatesDAL = new HotelBookingRoomRatesDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
        }

        public Task<List<HotelBookingRoomRates>> GetByHotelBookingID(long hotel_booking_id)
        {
            return _hotelBookingRoomRatesDAL.GetByBookingRoomsRateByHotelBookingID(hotel_booking_id);
        }
    }
}
