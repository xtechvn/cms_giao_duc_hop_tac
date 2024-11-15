using Entities.Models;
using Entities.ViewModels;
using Entities.ViewModels.Funding;
using Entities.ViewModels.Hotel;
using Entities.ViewModels.HotelBooking;
using Entities.ViewModels.HotelBookingRoom;
using Entities.ViewModels.OrderManual;
using Entities.ViewModels.Report;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.IRepositories
{
    public interface IHotelBookingRepositories
    {
        Task<List<HotelBookingViewModel>> GetListByOrderId(long orderId);
        Task<HotelBooking> GetHotelBookingByID(long id);
        public Task<long> UpdateHotelBooking(OrderManualHotelSerivceSummitHotel data, HotelESViewModel hotel_detail, int user_id, int is_debt_able);
        Task<GenericViewModel<SearchHotelBookingModel>> GetPagingList(SearchHotelBookingViewModel searchModel, int currentPage, int pageSize);
        Task<List<HotelBookingDetailViewModel>> GetHotelBookingById(long HotelBookingId);
        Task<List<HotelBooking>> GetHotelBookingByOrderID(long orderid);
        Task<int> UpdateHotelBookingStatus(long HotelBookingId, int Status);
        Task<List<HotelBookingViewModel>> GetDetailHotelBookingByID(long HotelBookingId);
        Task<List<TotalHotelBookingViewModel>> TotalHotelBooking(SearchHotelBookingViewModel searchModel);
        Task<int> UpdateHotelBooking(HotelBooking model);
        Task<int> InsertServiceDeclines(ServiceDeclines model);
        Task<ServiceDeclinesViewModel> GetServiceDeclinesByServiceId(string ServiceId, int type);
        Task<List<HotelBookingsRoomOptionalViewModel>> GetHotelBookingOptionalListByHotelBookingId(long hotelBookingId);
        Task<List<HotelBookingsRoomOptionalViewModel>> GetListHotelBookingRoomExtraPackagesHotelBookingId(long hotelBookingId);
        Task<long> DeleteHotelBookingByID(long id);
        Task<long> CancelHotelBookingByID(long id, int user_id);
        Task<int> CreateHotel(Hotel model);
        Task<string> ExportDeposit(SearchHotelBookingViewModel searchModel, string FilePath);
        List<ReportHotelRevenueViewModel> GetHotelBookingRevenue(ReportClientDebtSearchModel searchModel, out long total);
        string ExportHotelRevenue(ReportClientDebtSearchModel searchModel, string FilePath);
    }
}
