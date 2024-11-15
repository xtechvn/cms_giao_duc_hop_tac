using DAL;
using Entities.ConfigModels;
using Entities.Models;
using Entities.ViewModels;
using Microsoft.Extensions.Options;
using Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Utilities;

namespace Repositories.Repositories
{
    public class HotelBookingGuestRepository : IHotelBookingGuestRepository
    {
        
        private readonly HotelBookingGuestDAL _hotelBookingGuestDAL;

        public HotelBookingGuestRepository(IOptions<DataBaseConfig> dataBaseConfig)
        {

            _hotelBookingGuestDAL = new HotelBookingGuestDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
        }

        public Task<List<HotelGuest>> GetByHotelBookingID(long hotel_booking_id)
        {
            return _hotelBookingGuestDAL.GetByBookingId(hotel_booking_id);
        }
        public async Task<List<HotelGuestViewModel>> GetHotelGuestByHotelBookingId(long HotelBookingId)
        {
            var model = new List<HotelGuestViewModel>();
            try
            {
                DataTable dt = await _hotelBookingGuestDAL.GetHotelGuestByHotelBookingId(HotelBookingId);
                if (dt != null && dt.Rows.Count > 0)
                {
                    model = dt.ToList<HotelGuestViewModel>();
                }
                   
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetHotelGuestByHotelBookingId - HotelBookingGuestRepository: " + ex);
            }
            return model;
        }
    }
}
