using Entities.Models;
using Entities.ViewModels;
using Entities.ViewModels.OrderManual;
using Entities.ViewModels.SetServices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.IRepositories
{
    public interface IFlyBookingDetailRepository
    {
        FlyBookingDetail GetByOrderId(long orderId);
        List<FlyBookingDetail> GetListByOrderId(long orderId);
        Task<FlyBookingDetail> GetByFlyGroupAndLeg(long orderId, int leg, string group_fly);
        Task<List<FlyBookingExtraPackages>> GetExtraPackageByFlyBookingId(string group_fly);
        Task<List<FlyBookingDetail>> GetListByGroupFlyID(long orderId, string group_fly);
        Task<long> SummitFlyBookingServiceData(OrderManualFlyBookingServiceSummitModel data,int is_client_debt);
        Task<List<Bookingdetail>> GetListBookingdetailByOrderId(long orderId);
        Task<Bookingdetail> GetDetailFlyBookingDetailById(int FlyBookingId);
        Task<GenericViewModel<FlyBookingSearchViewModel>> GetPagingList(SearchFlyBookingViewModel searchModel, int currentPage, int pageSize);
        Task<long> UpdateServiceCode(string group_booking_id, string service_code);
        Task<PriceDetail> GetActiveFlyBookingPriceDetailByOrder(long order_id);
        Task<List<FlyBookingDetail>> GetListByGroupFlyID(string group_fly);
        Task<long> UpdateOperatorOrderPrice(FlyOperatorOrderPriceSummitViewModel data);
        Task<long> UpdateServiceStatus(int status, string group_booking_id, int user_id);
        Task<long> UpdateServiceOperator(string group_booking_id, int user_id, int user_commit);
        Task<long> DeleteFlyBookingByID(string group_booking_id);
        Task<long> CancelHotelBookingByID(string group_booking_id, int user_id);
        Task<List<FlyBookingPackagesOptional>> GetBookingPackagesOptionalsByBookingId(long booking_id);
        Task<List<FlyBookingPackagesOptionalViewModel>> GetFlyBookingPackagesOptionalsByBookingId(long booking_id);
        Task<long> UpdateFlyBookingOptional(List<FlyBookingPackagesOptional> data,string group_booking_id,int user_summit);
        Task<FlyBookingDetail> GetFlyBookingById(long fly_booking_id);
        Task<string> ExportDeposit(SearchFlyBookingViewModel searchModel, string FilePath);
    }
}
