using Entities.Models;
using Entities.ViewModels;
using Entities.ViewModels.OrderManual;
using Entities.ViewModels.SetServices;
using Entities.ViewModels.VinWonder;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.IRepositories
{
    public interface IVinWonderBookingRepository
    {
        public VinWonderBooking GetVinWonderBookingById(long booking_id);
        Task<List<VinWonderBookingTicket>> GetVinWonderTicketByBookingId(long booking_id);
        Task<List<VinWonderBookingTicket>> GetVinWonderTicketByBookingIdSP(long booking_id);
        Task<List<VinWonderBookingTicketCustomer>> GetVinWonderTicketCustomerByBookingIdSP(long booking_id);
        Task<long> SummitVinWonderServiceData(OrderManualVinWonderBookingServiceSummitModel data, int user_id);
        Task<long> DeleteVinWonderBookingByID(long id);
        Task<long> CancelVinWonderByID(long id, int user_id);
        Task<List<VinWonderDetailViewModel>> GetDetailVinWonderByBookingId(long BookingId);
        Task<GenericViewModel<VinWonderBookingSearchViewModel>> GetPagingList(SearchFlyBookingViewModel searchModel, int currentPage, int pageSize);
        Task<long> UpdateServiceOperator(long booking_id, int user_id, int user_commit);
        Task<long> UpdateVinWonderTicketOperatorPrice(List<VinWonderBookingTicket> data, int user_summit);
        Task<long> UpdateServiceOperator(long booking_id, int user_id);
        Task<long> UpdateServiceStatus(int status, long booking_id, int user_id);
        Task<string>  ExportDeposit(SearchFlyBookingViewModel searchModel, string FilePath);
        Task<List<ListVinWonderemialViewModel>> GetVinWonderBookingEmailByOrderID(long orderid);
        Task<List<VinWonderBookingTicket>> GetVinWonderBookingTicketByBookingID(long BookingId);
        Task<List<VinWonderBooking>> GetVinWonderBookingByOrderId(long order_id);
        Task<List<VinWonderBookingTicketCustomer>> GetVinWondeCustomerByBookingId(long BookingId);
    }
}
