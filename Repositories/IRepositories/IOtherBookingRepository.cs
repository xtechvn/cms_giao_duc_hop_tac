using Entities.Models;
using Entities.ViewModels;
using Entities.ViewModels.OrderManual;

using Entities.ViewModels.SetServices;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.IRepositories
{
    public interface IOtherBookingRepository
    {
        public Task<List<OtherBookingPackages>> GetOtherBookingPackagesByBookingId(long booking_id);
        public Task<OtherBooking> GetOtherBookingById(long booking_id);
        Task<long> SummitOtherBooking(OrderManualOtherBookingServiceSummitModel data, int user_summit);
        Task<GenericViewModel<OtherBookingSearchViewModel>> GetPagingList(SearchFlyBookingViewModel searchModel, int currentPage, int pageSize);
        Task<long> DeleteOtherBookingById(long id);
        Task<long> CancelOtherBookingById(long id, int user_id);
        Task<List<OtherBookingPackagesOptional>> GetOtherBookingPackagesOptionalByBookingId(long booking_id);
        List<OtherBookingPackagesOptionalViewModel> GetOtherBookingPackagesOptionalByServiceId(long serviceId);
        Task<long> UpdateServiceOperator(long booking_id, int user_id, int user_commit);
        Task<long> UpdateOtherBookingOptional(List<OtherBookingPackagesOptional> data, long booking_id, int user_summit);
        Task<List<OtherBookingViewModel>> GetDetailOtherBookingById(int OtherBookingId);

        Task<string> ExportDeposit(SearchFlyBookingViewModel searchModel, string FilePath);

        Task<long> UpdateServiceOperator(long booking_id, int user_id);
        Task<long> UpdateServiceStatus(int status, long booking_id, int user_id);
        Task<List<OtherBooking>> ServiceCodeSuggesstion(string txt_search = "");
        Task<List<OtherBooking>> getListOtherBookingByOrderId(long OrderId);

      
    }
}
