using Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.ViewModels.Hotel
{
    public class HotelViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class HotelDetailViewModel : Entities.Models.Hotel
    {
        public string ChainBrandsName { get; set; }
        public string VerifyDateName { get; set; }
    }

    public class HotelUpsertViewModel : Entities.Models.Hotel
    {
        public string SupplierName { get; set; }
        public int PositionB2C { get; set; }
        public int PositionB2B { get; set; }
    }


    public class HotelFilterModel
    {
        public string FullName { get; set; }
        public string ProvinceId { get; set; }
        public string RatingStar { get; set; }
        public string ChainBrands { get; set; }
        public string SalerId { get; set; }
        public int TypeFilter { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }

    public class HotelGridModel
    {
        public int Id { get; set; }
        public int SupplierId { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Street { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string GroupName { get; set; }
        public int? EstablishedYear { get; set; }
        public string ProvinceName { get; set; }
        public string Email { get; set; }
        public string Country { get; set; }
        public string ResidenceName { get; set; }
        public string SalerName { get; set; }
        public string CreatedName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? RatingStar { get; set; }
        public int? Star { get; set; }
        public string UpdatedName { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? Todate { get; set; }
        public string VerifyDateName { get; set; }
        public bool IsDisplayWebsite { get; set; }
        public string Mobile { get; set; }
        public string ContactName { get; set; }
        public int TotalRow { get; set; }
        public string ImageThumb { get; set; }
    }

    public class HotelSurchargeGridModel : HotelSurcharge
    {
        public string UserName { get; set; }
        public int TotalRow { get; set; }
    }

    public class HotelContactGridModel : HotelContact
    {
        public string UserCreate { get; set; }
        public int TotalRow { get; set; }
    }

    public class HotelBankingAccountGridModel : HotelBankingAccount
    {
        public string UserCreate { get; set; }
        public int TotalRow { get; set; }
    }

    public class HotelRoomGridModel : HotelRoom
    {
        public string BedRoomTypeName { get; set; }
        public int TotalRow { get; set; }
    }

    public class HotelRoomUpsertModel : HotelRoom
    {
        public IEnumerable<string> OtherImages { get; set; }
    }
    public class VietQRBankDetail
    {
        public int id { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public string bin { get; set; }
        public string shortName { get; set; }
        public string logo { get; set; }
        public string short_name { get; set; }
        public string swift_code { get; set; }
    }
}
