using System;
using System.Collections.Generic;
using System.Text;
using Utilities;

namespace Entities.ViewModels.Report
{
    public class ReportClientDebtSearchModel
    {
        public int? BranchCode { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public long? ClientID { get; set; }
        public int? PageIndex { get; set; }
        public int? PageSize { get; set; }
        public List<string> HotelIds { get; set; }
        public DateTime? CreateDateFrom
        {
            get
            {
                return DateUtil.StringToDate(CreateDateFromStr);
            }
        }
        public string CreateDateFromStr { get; set; }
        public DateTime? CreateDateTo
        {
            get
            {
                return DateUtil.StringToDate(CreateDateToStr);
            }
        }
        public string CreateDateToStr { get; set; }
        public DateTime? CheckInDateFrom
        {
            get
            {
                return DateUtil.StringToDate(CheckInDateFromStr);
            }
        }
        public string CheckInDateFromStr { get; set; }
        public DateTime? CheckInDateTo
        {
            get
            {
                return DateUtil.StringToDate(CheckInDateToStr);
            }
        }
        public string CheckInDateToStr { get; set; }
        public DateTime? CheckOutDateFrom
        {
            get
            {
                return DateUtil.StringToDate(CheckOutDateFromStr);
            }
        }
        public string CheckOutDateFromStr { get; set; }
        public DateTime? CheckOutDateTo
        {
            get
            {
                return DateUtil.StringToDate(CheckOutDateToStr);
            }
        }
        public string CheckOutDateToStr { get; set; }
    }
}
