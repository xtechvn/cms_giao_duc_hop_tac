using Entities.Models;
using Entities.ViewModels.HotelBookingCode;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.IRepositories
{
   public interface IHotelBookingCodeRepository
    {
        Task<List<HotelBookingCodeModel>> GetListlBookingCodeByHotelBookingId(long HotelBookingId,int Type);
        Task<HotelBookingCode> GetDetailBookingCodeById(long id);
        Task<int> InsertHotelBookingCode(HotelBookingCodeViewModel model);
        Task<int> UpdateHotelBookingCode(HotelBookingCodeViewModel model);
        Task<List<HotelBookingCodeModel>> GetListHotelBookingCodeByOrderId(long OrderId);
        Task<List<HotelBookingCode>> GetHotelBookingCodeByType(long service_id, int type);
        Task DeleteBookingCodeByIdandNote(long data_id, string note);
    }
}
