using Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.ViewModels.Tour
{
    public class TourProductViewModel
    {
        public long id { get; set; }
        public string TourName { get; set; }
        public string ServiceCode { get; set; }
        public string OrganizingTypeName { get; set; }
        public string TourTypeName { get; set; }
        public string TourType { get; set; }
        public string StartPoint1 { get; set; }
        public string StartPoint2 { get; set; }
        public string StartPoint3 { get; set; }
        public string SupplierId { get; set; }
        public string FullName { get; set; }
        public string DateDeparture { get; set; }
        public string GroupEndPoint { get; set; }
        public DateTime CreatedDate { get; set; }
        public long TotalRow { get; set; }
    }

    public class TourProductGridModel
    {
        public long id { get; set; }
        public string TourName { get; set; }
        public string ServiceCode { get; set; }
        public string OrganizingTypeName { get; set; }
        public string TourTypeName { get; set; }
        public string TourType { get; set; }
        public string StartPoint { get; set; }
        public string SupplierId { get; set; }
        public string FullName { get; set; }
        public string DateDeparture { get; set; }
        public string StartPoint1 { get; set; }
        public string StartPoint2 { get; set; }
        public string StartPoint3 { get; set; }
        public string GroupEndPoint1 { get; set; }
        public string GroupEndPoint2 { get; set; }
        public string GroupEndPoint3 { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UserCreate { get; set; }
        public string UserUpdate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int IsDisplayWeb { get; set; }
        public int Status { get; set; }
        public int TotalRow { get; set; }
    }

    public class TourProductUpsertModel : TourProduct
    {
        public IEnumerable<TourProductScheduleModel> TourSchedule { get; set; }
        public IEnumerable<string> OtherImages { get; set; }
        public IEnumerable<int> EndPoints { get; set; }
        public string SupplierName { get; set; }
        public int PositionB2C { get; set; }
        public int PositionB2B { get; set; }
    }

    public class TourProductScheduleModel
    {
        public int day_num { get; set; }
        public string day_title { get; set; }
        public string day_description { get; set; }
    }

    public class TourProductSearchModel
    {
        public string ServiceCode { get; set; }
        public string TourName { get; set; }
        public string TourType { get; set; }
        public string OrganizingType { get; set; }
        public int? Days { get; set; }
        public int? Star { get; set; }
        public string SupplierIds { get; set; }
        public int? IsDisplayWeb { get; set; }
        public string StartPoint { get; set; }
        public string Endpoint { get; set; }
        public bool? IsSelfDesign { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
