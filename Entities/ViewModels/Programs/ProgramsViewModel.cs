using System;
using System.Collections.Generic;
using System.Text;
using Utilities;

namespace Entities.ViewModels.Programs
{
    public class ProgramsViewModel
    {
        public long ProgramId { get; set; }
        public string ProgramCode { get; set; }
        public string ProgramName { get; set; }
        //public string ServiceTypeName { get; set; }
        public string ServiceName { get; set; }
        public string FullName { get; set; }
        public string CreatedDate { get; set; }
        public string ServiceType { get; set; }
        public string Description { get; set; }
        public string UserName { get; set; }
        public string SupplierId { get; set; }
        public string ProgramStatus { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string UpdatedDate { get; set; }
        public string UserVerifyName { get; set; }
        public string VerifyDate { get; set; }
        public string UserUpdateName { get; set; }
        public string UserCreateName { get; set; }
        public string UserCreatedFullName { get; set; }
        public string UserVerifyNameFullName { get; set; }
        public string StayStartDate { get; set; }
        public string StayEndDate { get; set; }
        public int Status { get; set; }
        public long HotelId { get; set; }

        public string TotalRow { get; set; }
       
    }
    public class ProgramsSearchViewModel
    {
        public string ProgramCode { get; set; }
        public string Description { get; set; }
        public string ProgramStatus { get; set; }
        public string ServiceType { get; set; }
        public string SupplierID { get; set; }
        public string ClientId { get; set; }
        public string StartDateFrom { get; set; }
        public string StartDateTo { get; set; }
        public string EndDateFrom { get; set; }
        public string EndDateTo { get; set; }
        public string UserCreate { get; set; }
        public string CreateDateFrom { get; set; }
        public string CreateDateTo { get; set; }
        public string UserVerify { get; set; }
        public string VerifyDateFrom { get; set; }
        public string VerifyDateTo { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string SalerPermission { get; set; }
    }
    public class ProgramsModel
    {
        public long Id { get; set; }
        public string ProgramCode { get; set; }
        public string ProgramName { get; set; }
        public string ServiceName { get; set; }
        public int SupplierId { get; set; }
        public int ServiceType { get; set; }
        public string StartDateStr { get; set; }
        public DateTime? StartDate
        {
            get
            {
                return DateUtil.StringToDate(StartDateStr);
            }
        }
        public string EndDateStr { get; set; }
        public DateTime? EndDate
        {
            get
            {
                return DateUtil.StringToDate(EndDateStr);
            }
        }
        public string StayStartDateStr { get; set; }
        public DateTime? StayStartDate
        {
            get
            {
                return DateUtil.StringToDate(StayStartDateStr);
            }
        }
        public string StayEndDateStr { get; set; }
        public DateTime? StayEndDate
        {
            get
            {
                return DateUtil.StringToDate(StayEndDateStr);
            }
        }
        public string Description { get; set; }
        public int? Status { get; set; }
        public int? UserVerify { get; set; }
 
        public DateTime? VerifyDate { get; set; }

        public int? CreatedBy { get; set; }
       
        public DateTime? CreatedDate { get; set; }
       
        public int? UpdatedBy { get; set; }
        public int HotelId { get; set; }
        public DateTime? UpdatedDate{ get; set; }
    }
    public class HotelModel
    {
        public string HotelId { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
    }
    public class SupplierModel
    {
        public string SupplierId { get; set; }
        public string FullName { get; set; }
      
    }
    public class ProgramsSearchSupplierId
    {
        public string SupplierID { get; set; }
        public string ServiceName { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }

    public class FieldPrograms
    {
        public bool STT { get; set; }
        public bool ProgramCode { get; set; }
        public bool ProgramNamef { get; set; }
        public bool ServiceType { get; set; }
        public bool ServiceNamef { get; set; }
        public bool SupplierNamef { get; set; }
        public bool ApplyDate { get; set; }
        public bool Descriptionf { get; set; }
        public bool UserCreate { get; set; }
        public bool CreateDatef { get; set; }
        public bool UserVerify { get; set; }
        public bool VerifyDate { get; set; }
    }
}
