using Entities.Models;
using Entities.ViewModels;
using Entities.ViewModels.OrderManual;
using Entities.ViewModels.Tour;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.IRepositories
{
    public interface ITourRepository
    {
        public Task<Tour> GetTourById(long tour_id);
        Task<List<TourPackages>> GetTourPackagesByTourId(long tour_id);
        Task<List<TourGuests>> GetTourGuestsByTourId(long tour_id);
        Task<long> SummitTourServiceData(OrderManualTourBookingServiceSummitModel data, int user_id, int is_client_debt);
        Task<List<TourViewModel>> GetTourByOrderId(long OrderId);
        Task<TourViewModel> GetDetailTourByID(long TourId);
        Task<GenericViewModel<TourGetListViewModel>> GetListTour(TourSearchViewModel Tourmodel);
        Task<List<TourPackages>> ListTourPackagesByTourId(long TourId);
        Task<List<TourGuests>> GetListTourGuestsByTourId(long TourId);
        Task<int> UpdateTourStatus(long TourId, int Status);
        Task<int> UpdateTourPackages(List<TourPackages> model);
        Task<List<Tour>> GetAllTour();
        Task<List<CountTourviewModel>> CountTourByStatus(TourSearchViewModel model);
        Task<int> UpdateTour(Tour model);
        Task<int> UpdateTourTotalPrice(long tour_id);
        Task<List<TourProductViewModel>> TourProductSuggesstion(string txt_search);
        Task<TourProduct> GetTourProductById(long id);
        Task<List<TourDestination>> GetTourDestinationByTourProductId(long tour_id);
        int UpdateTourGuest(TourGuests model);
        Task<long> DeleteTourByID(long id);
        Task<long> CancelTourByID(long id, int user_id);

        IEnumerable<TourProductGridModel> GetPagingTourProduct(TourProductSearchModel searchModel);
        Task<long> UpsertTourProduct(TourProductUpsertModel model);
        int DeleteTourProduct(int Id);
        TourProductUpsertModel GetTourProductDetail(long tourProductId);
        Task<string> ExportDeposit(TourSearchViewModel searchModel, string FilePath);
        Task<List<TourProgramPackages>> GetListTourProgramPackagesByTourProductId(long tour_product_id);
        Task<bool> UpsertTourProductPrices(long tour_product_id, List<TourProgramPackages> model, int user_id);
        long DeleteTourProgramPackages(long id, int user_id);
        Task<List<TourPosition>> GetListTourPosition(long id);
    }
}
