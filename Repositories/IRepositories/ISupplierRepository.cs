using Entities.ViewModels.Funding;
using System.Collections.Generic;
using Entities.Models;
using System.Threading.Tasks;
using Entities.ViewModels.SupplierConfig;
using Entities.ViewModels;
using Entities.ViewModels.Hotel;

namespace Repositories.IRepositories
{
    public interface ISupplierRepository
    {
        List<SupplierViewModel> GetSuppliers(SupplierSearchModel searchModel);
        SupplierViewModel GetById(int supplierId);
        SupplierDetailViewModel GetDetailById(int supplierId);

        int Add(SupplierConfigUpsertModel model);
        int Update(SupplierConfigUpsertModel model);
        string ExportSuppliers(SupplierSearchModel searchModel, string FilePath);
        Task<List<Supplier>> GetSuggestionList(string name);

        SupplierRoomModel GetDetailSupplierRoom(long room_id);
        long AddSupplierRoom(SupplierRoomViewModel model);
        long UpdateSupplierRoom(SupplierRoomViewModel model);
        long DeleteSupplierRoom(long room_id);
        IEnumerable<SupplierRoomGridModel> GetRoomListOfSupplier(long supplier_id,string ServiceName, long HotelId, int page_index = 1, int page_size = 10);
        IEnumerable<SupplierHotelModel> GetHotelBySupplierId(int supplierId);
        public Supplier GetSuplierById(long supplierId);
        public IEnumerable<Supplier> GetSuggestSupplier(string text, int limit);
        public IEnumerable<Supplier> GetSuggestSupplierForHotel(int hotel_id, string text, int limit);
        int GetByIDOrName(int suplier_id, string name);

        // Contact
        public IEnumerable<SupplierContactViewModel> GetSupplierContactList(int supplier_id);
        public SupplierContact GetSupplierContactById(int Id);
        public int UpsertSupplierContact(SupplierContact model);
        public long DeleteSupplierContact(long id);

        // Banking account
        IEnumerable<SupplierPaymentViewModel> GetSupplierPaymentList(int supplier_id);
        BankingAccount GetSupplierPaymentById(int Id);
        int UpsertSupplierPayment(BankingAccount model);
        int DeleteSupplierPayment(int id);

        // Order history
        GenericViewModel<SupplierOrderGridViewModel> GetSupplierOrderList(SupplierOrderSearchModel model);

        // Service
        GenericViewModel<SupplierOrderGridViewModel> GetSupplierServiceList(SupplierServiceSearchModel model);
        int CreateBatchSupplierHotel(int supplier_id, string hotel_ids);
        IEnumerable<HotelViewModel> GetHotelListBySuplierId(int supplier_id);

        // Ticket
        GenericViewModel<SupplierTicketGridViewModel> GetSupplierTicketList(SupplierTicketSearchModel model);

        // Program
        GenericViewModel<SupplierProgramGridViewModel> GetSupplierProgramList(SupplierProgramSearchModel model);

    }
}
