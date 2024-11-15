using DAL;
using Entities.ConfigModels;
using Entities.Models;
using Entities.ViewModels.HotelBooking;
using Microsoft.Extensions.Options;
using Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Utilities;

namespace Repositories.Repositories
{
    public class HotelBookingRoomExtraPackageRepository : IHotelBookingRoomExtraPackageRepository
    {
        
        private readonly HotelBookingRoomExtraPackagesDAL _hotelBookingRoomExtraPackagesDAL;

        public HotelBookingRoomExtraPackageRepository(IOptions<DataBaseConfig> dataBaseConfig)
        {

            _hotelBookingRoomExtraPackagesDAL = new HotelBookingRoomExtraPackagesDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
        }

        public Task<List<HotelBookingRoomExtraPackages>> GetByBookingID(long hotel_booking_id)
        {
            return _hotelBookingRoomExtraPackagesDAL.GetByBookingId(hotel_booking_id);
        }
        public async Task<List<HotelBookingRoomExtraPackagesViewModel>> Gethotelbookingroomextrapackagebyhotelbookingid(long HotelBookingId)
        {
            var model = new List<HotelBookingRoomExtraPackagesViewModel>();
            try
            {
                DataTable dt = await _hotelBookingRoomExtraPackagesDAL.Gethotelbookingroomextrapackagebyhotelbookingid(HotelBookingId);
                if (dt != null && dt.Rows.Count > 0)
                {
                    model = dt.ToList<HotelBookingRoomExtraPackagesViewModel>();
                }

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Gethotelbookingroomextrapackagebyhotelbookingid - HotelBookingRoomExtraPackageRepository: " + ex);
            }
            return model;
        }

        public async Task<int> UpdateHotelBookingRoomExtraPackages(HotelBookingRoomExtraPackages modele)
        {
            try
            {
                return await _hotelBookingRoomExtraPackagesDAL.UpdateHotelBookingRoomExtraPackages(modele);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Gethotelbookingroomextrapackagebyhotelbookingid - HotelBookingRoomExtraPackagesDAL: " + ex);
            }
            return 0;
        }
    }
}
