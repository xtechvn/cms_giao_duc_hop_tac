using Entities.Models;
using Entities.ViewModels.Hotel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.IRepositories
{
    public interface IHotelRepository
    {
        IEnumerable<HotelGridModel> GetHotelPagingList(HotelFilterModel model);

        IEnumerable<HotelViewModel> GetSuggestionHotelList(string name, int size);

        int SaveHotel(HotelUpsertViewModel model);
        Hotel GetHotelById(int id);

        HotelDetailViewModel GetHotelDetailById(int id);

        // Contact
        public IEnumerable<HotelContactGridModel> GetHotelContactList(int hotel_id);
        public HotelContact GetHotelContactById(int id);
        public int UpsertHotelContact(HotelContact model);
        public int DeleteHotelContact(int id);

        // Banking account
        IEnumerable<HotelBankingAccountGridModel> GetHotelBankingAccountList(int hotel_id);
        HotelBankingAccount GetHotelBankingAccountById(int Id);
        int UpsertHotelBankingAccount(HotelBankingAccount model);
        int DeleteHotelBankingAccount(int id);

        // Surcharge
        IEnumerable<HotelSurchargeGridModel> GetHotelSurchargeList(int hotel_id, int page_index, int page_size);
        HotelSurcharge GetHotelSurchargeById(int id);
        int UpsertHotelSurcharge(HotelSurcharge model);
        int DeleteHotelSurcharge(int id);

        // Room
        IEnumerable<HotelRoomGridModel> GetHotelRoomList(int hotel_id, int page_index, int page_size);
        HotelRoom GetHotelRoomById(int id);
        int UpsertHotelRoom(HotelRoomUpsertModel model);
        int DeleteHotelRoom(int id);

        // Ultilities
        Task<int> UpsertHotelUltilities(int hotel_id, string extends);
        int UpsertHotelBankingAccountByName(HotelBankingAccount model);
        public Hotel GetHotelByHotelID(string hotel_id);
        Task<bool> UpdateHotelSurchargeNote(string body, int id);
        //fe
        List<HotelPricePolicyViewModel> GetHotelRoomPricePolicy(string hotel_id, string room_ids,  string client_types);
        List<HotelPosition> GetListHotelPosition(long hotelid);

    }
}
