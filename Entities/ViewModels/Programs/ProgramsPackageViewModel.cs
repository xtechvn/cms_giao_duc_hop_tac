using System;
using System.Collections.Generic;
using System.Text;
using Utilities;

namespace Entities.ViewModels.Programs
{
    public class ProgramsPackageViewModel
    {
        public long id { get; set; }
        public string PackageCode { get; set; }
        public string PackageName { get; set; }
        public string RoomType { get; set; }
        public string RoomTypeId { get; set; }
        public double Amount { get; set; }
        public double Price { get; set; }
        public double Profit { get; set; }
        public string WeekDay { get; set; }
        public string ApplyDate { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public int OpenStatus { get; set; }
        public string Description { get; set; }
        public string StatusName { get; set; }
        public string ProgramName { get; set; }
        public string ProgramId { get; set; }
        public int Type { get; set; }
        
    }
    public class ProgramsPackagePriceViewModel
    {
        public long Id { get; set; }
        public long ProgramsPackageId { get; set; }
        public string ProgramName { get; set; }
        public string ProgramsPackageName { get; set; }
        public string RoomType { get; set; }
        public string RoomTypeId { get; set; }
        public string Description { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public List<ProgramsPackageModel> ProgramsPackagePrice { get; set; }

    }
    public class ProgramsPackageModel
    {
        public long Id { get; set; }
        public long SupplierId { get; set; }
        public long ProgramsPackageId{ get; set; }
        public string ProgramName { get; set; }
        public string PackageName { get; set; }
        public string PackageCode { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }
        public int ProgramsPackageStatus { get; set; }
        public string RoomType { get; set; }
        public string RoomTypeId { get; set; }
        public double Amount { get; set; }
        public string WeekDay { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; } 
        public string StatusName { get; set; } 
        public List<ProgramsPackageViewModel> ProgramsPackage { get; set; }
        public List<ProgramsPackageViewModel> ProgramsPackageDaily { get; set; }
    }
    public class ProgramsPackagePriceModel
    {
        public long Id { get; set; }

        public string ProgramName { get; set; }
        public string Description { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public List<ProgramsPackagePriceViewModel> ProgramsPackage { get; set; }
    }
    public class ProgramsPackageSearchViewModel
    {
        public string ProgramId { get; set; }
        public string Packageid { get; set; }
        public string ProgramName { get; set; }
        public string FromDate { get; set; }
        public string RoomType { get; set; }
        public string PackageCode { get; set; }
        public string ToDate { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int Type { get; set; }
        public int ClientType { get; set; }
        public string SupplierID { get; set; } = null;
        public string HotelId { get; set; } = null;
        public string StayStartDateFrom { get; set; } = null;
        public string StayStartDateTo { get; set; } = null;
        public string StayEndDateFrom { get; set; } = null;
        public string StayEndDateTo { get; set; } = null;
        public string Status { get; set; } = null;
    }
    public class InsertProgramsPackageViewModel
    {
        public string PackageCode { get; set; }
        public string PackageName { get; set; }
        public int ProgramId { get; set; }
        public int id { get; set; }
        public string RoomType { get; set; }
        public string RoomTypeId { get; set; }
        public string RoomTypeIdV1 { get; set; }
        public double Amount { get; set; }
        public double Price { get; set; }
        public double Profit { get; set; }
        public string FromDateStr { get; set; }
        public string ToDateStr { get; set; }
        public string WeekDay { get; set; }
        public int OpenStatus { get; set; }
        public string ApplyDateStr
        {
            get; set;
        }
        public DateTime? ApplyDate  { get; set; }
        public DateTime? FromDate
        {
            get
            {
                return DateUtil.StringToDate(FromDateStr);
            }
        }
        public DateTime? ToDate
        {
            get
            {
                return DateUtil.StringToDate(ToDateStr);
            }
        }

        public class WeekDayModel
        {
            public string name { get; set; }
            public string id { get; set; }

        }
    }
}
