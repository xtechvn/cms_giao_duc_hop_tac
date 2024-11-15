using Entities.Models;
using Entities.ViewModels;
using Entities.ViewModels.HotelBooking;
using Entities.ViewModels.OrderManual;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.IRepositories
{
    public interface IHotelBookingRoomExtraPackageRepository
    {
        Task<List<HotelBookingRoomExtraPackages>> GetByBookingID(long hotel_booking_id);
        Task<List<HotelBookingRoomExtraPackagesViewModel>> Gethotelbookingroomextrapackagebyhotelbookingid(long HotelBookingId);
        Task<int> UpdateHotelBookingRoomExtraPackages(HotelBookingRoomExtraPackages modele);
    }
}
