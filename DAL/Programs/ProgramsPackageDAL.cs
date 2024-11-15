using DAL.Generic;
using DAL.StoreProcedure;
using Entities.Models;
using Entities.ViewModels.Programs;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace DAL.Programs
{
    public class ProgramsPackageDAL : GenericService<ProgramPackage>
    {
        private static DbWorker _DbWorker;
        public ProgramsPackageDAL(string connection) : base(connection)
        {
            _DbWorker = new DbWorker(connection);
        }
        public async Task<DataTable> GetPagingList(ProgramsPackageSearchViewModel searchModel)
        {
            try
            {

                SqlParameter[] objParam = new SqlParameter[12];
                objParam[0] = searchModel.ProgramId == null ? new SqlParameter("@ProgramId", DBNull.Value) : new SqlParameter("@ProgramId", searchModel.ProgramId);
                objParam[1] = searchModel.FromDate == null ? new SqlParameter("@FromDate", DBNull.Value) : new SqlParameter("@FromDate", DateUtil.StringToDate(searchModel.FromDate));
                objParam[2] = searchModel.ToDate == null ? new SqlParameter("@ToDate", DBNull.Value) : new SqlParameter("@ToDate", DateUtil.StringToDate(searchModel.ToDate));
                objParam[3] = new SqlParameter("@PageIndex", searchModel.PageIndex);
                objParam[4] = new SqlParameter("@PageSize", searchModel.PageSize);
                objParam[5] = searchModel.SupplierID == null ? new SqlParameter("@SupplierID", DBNull.Value) : new SqlParameter("@SupplierID", searchModel.SupplierID);
                objParam[6] = searchModel.HotelId == null ? new SqlParameter("@HotelId", DBNull.Value) : new SqlParameter("@HotelId", searchModel.HotelId);
                objParam[7] = searchModel.StayStartDateFrom == null ? new SqlParameter("@StayStartDateFrom", DBNull.Value) : new SqlParameter("@StayStartDateFrom", DateUtil.StringToDate(searchModel.StayStartDateFrom));
                objParam[8] = searchModel.StayStartDateTo == null ? new SqlParameter("@StayStartDateTo", DBNull.Value) : new SqlParameter("@StayStartDateTo", DateUtil.StringToDate(searchModel.StayStartDateTo));
                objParam[9] = searchModel.StayEndDateFrom == null ? new SqlParameter("@StayEndDateFrom", DBNull.Value) : new SqlParameter("@StayEndDateFrom", DateUtil.StringToDate(searchModel.StayEndDateFrom));
                objParam[10] = searchModel.StayEndDateTo == null ? new SqlParameter("@StayEndDateTo", DBNull.Value) : new SqlParameter("@StayEndDateTo", DateUtil.StringToDate(searchModel.StayEndDateTo));
                objParam[11] = searchModel.Status == null ? new SqlParameter("@Status", DBNull.Value) : new SqlParameter("@Status", searchModel.Status);


                return _DbWorker.GetDataTable(StoreProcedureConstant.SP_GetListProgramsPackage, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetPagingList - ProgramsPackageDAL: " + ex);
            }
            return null;
        }
        public async Task<int> InsertProgramPackage(InsertProgramsPackageViewModel Model, long Userid, long type2)
        {
            try
            {

                //Model.ApplyDate = GetDate(Model.FromDate, Model.ToDate, Model.WeekDay);
                SqlParameter[] objParam = new SqlParameter[15];
                objParam[0] = new SqlParameter("@PackageCode", Model.PackageCode);
                objParam[1] = new SqlParameter("@ProgramId", Model.ProgramId);
                objParam[2] = new SqlParameter("@RoomType", Model.RoomType);
                objParam[3] = new SqlParameter("@Amount", Model.Amount);
                objParam[4] = Model.FromDate == null ? new SqlParameter("@FromDate", DateUtil.StringToDate(Convert.ToDateTime(Model.FromDateStr).ToString("dd/MM/yyyy"))) : new SqlParameter("@FromDate", Model.FromDate);
                objParam[5] = Model.ToDate == null ? new SqlParameter("@ToDate", DateUtil.StringToDate(Convert.ToDateTime(Model.ToDateStr).ToString("dd/MM/yyyy"))) : new SqlParameter("@ToDate", Model.ToDate);
                objParam[6] = new SqlParameter("@WeekDay", Model.WeekDay);
                objParam[7] = Model.ApplyDate == null ? new SqlParameter("@ApplyDate", DBNull.Value) : new SqlParameter("@ApplyDate", Model.ApplyDate);
                objParam[8] = new SqlParameter("@OpenStatus", Model.OpenStatus);
                objParam[9] = new SqlParameter("@CreatedDate", DBNull.Value);
                objParam[10] = new SqlParameter("@CreatedBy", Userid);
                objParam[11] = new SqlParameter("@RoomTypeId", Model.RoomTypeId);
                objParam[12] = new SqlParameter("@PackageName", Model.PackageName);
                objParam[13] = new SqlParameter("@Price", Model.Price);
                objParam[14] = new SqlParameter("@Profit", Model.Profit);
                return _DbWorker.ExecuteNonQuery(StoreProcedureConstant.sp_InsertProgramPackage, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("InsertProgramPackage - ProgramsPackageDAL: " + ex);
                return 0;
            }

        }
        public async Task<int> checkProgramPackage(InsertProgramsPackageViewModel Model, long type2)
        {
            try
            {
                Model.ApplyDate = GetDate(Model.FromDate, Model.ToDate, Model.WeekDay);
                using (var _DbContext = new EntityDataContext(_connection))
                {

                    if (type2 == 0)
                    {
                        var data3 = _DbContext.ProgramPackage.Where(s => s.PackageCode.Equals(Model.PackageCode) && s.ProgramId == Model.ProgramId).ToList();
                        if (data3.Count > 0)
                        {
                            return -1;
                        }
                    }
                    if (type2 == 1)
                    {
                        var data3 = _DbContext.ProgramPackage.Where(s => s.PackageCode.Equals(Model.PackageCode) && s.RoomType == Model.RoomType && s.ProgramId == Model.ProgramId && s.ApplyDate == Model.ApplyDate).ToList();
                        if (data3.Count > 0)
                        {
                            return -1;
                        }
                    }
                    return 0;
                }
            }
            catch (Exception ex)
            {
                return -1;
                LogHelper.InsertLogTelegram("checkProgramPackage - ProgramsPackageDAL: " + ex);
            }

        }
        public async Task<int> CheckExistsProgramsPackageByDate(InsertProgramsPackageViewModel Model, long type2)
        {
            try
            {

                SqlParameter[] objParam = new SqlParameter[6];
                objParam[0] = new SqlParameter("@ProgramId", Model.ProgramId);
                objParam[1] = new SqlParameter("@PackageCode", Model.PackageCode);
                objParam[2] = new SqlParameter("@RoomType", Model.RoomType);
                objParam[3] = new SqlParameter("@FromDate", Model.FromDate);
                objParam[4] = new SqlParameter("@ToDate", Model.ToDate);
                objParam[5] = new SqlParameter("@RoomTypeId", Model.RoomTypeId);

                var dt = _DbWorker.GetDataTable(StoreProcedureConstant.SP_CheckExistsProgramsPackageByDate, objParam);
                if (dt != null && dt.Rows.Count > 0)
                {
                    var data = Convert.ToInt32(dt.Rows[0]["RESULT"]);
                    return data;
                }
                return 1;
            }
            catch (Exception ex)
            {
                return 1;
                LogHelper.InsertLogTelegram("CheckExistsProgramsPackageByDate - ProgramsPackageDAL: " + ex);
            }

        }
        public async Task<int> UpdateProgramPackage(InsertProgramsPackageViewModel Model, long userid)
        {
            try
            {

                SqlParameter[] objParam = new SqlParameter[14];
                objParam[0] = new SqlParameter("@Id", Model.id);
                objParam[1] = Model.PackageCode == null ? new SqlParameter("@PackageCode", DBNull.Value) : new SqlParameter("@PackageCode", Model.PackageCode);
                objParam[2] = Model.ProgramId == 0 ? new SqlParameter("@ProgramId", DBNull.Value) : new SqlParameter("@ProgramId", Model.ProgramId);
                objParam[3] = Model.RoomType == null ? new SqlParameter("@RoomType", DBNull.Value) : new SqlParameter("@RoomType", Model.RoomType);
                objParam[4] = new SqlParameter("@Amount", Model.Amount);
                objParam[5] = Model.FromDate == null ? new SqlParameter("@FromDate", DBNull.Value) : new SqlParameter("@FromDate", Model.FromDate);
                objParam[6] = Model.ToDate == null ? new SqlParameter("@ToDate", DBNull.Value) : new SqlParameter("@ToDate", Model.ToDate);
                objParam[7] = Model.WeekDay == null ? new SqlParameter("@WeekDay", DBNull.Value) : new SqlParameter("@WeekDay", Model.WeekDay);
                objParam[8] = Model.ApplyDate == null ? new SqlParameter("@ApplyDate", DBNull.Value) : new SqlParameter("@ApplyDate", Model.ApplyDate);
                objParam[9] = Model.OpenStatus == null ? new SqlParameter("@OpenStatus", DBNull.Value) : new SqlParameter("@OpenStatus", Model.OpenStatus);
                objParam[10] = new SqlParameter("@UpdatedBy ", userid);
                objParam[11] = new SqlParameter("@PackageName ", Model.PackageName);
                objParam[12] = new SqlParameter("@Price", Model.Price);
                objParam[13] = new SqlParameter("@Profit", Model.Profit);
                return _DbWorker.ExecuteNonQuery(StoreProcedureConstant.sp_UpdateProgramPackage, objParam);
            }
            catch (Exception ex)
            {
                return 0;
                LogHelper.InsertLogTelegram("UpdateProgramPackage - ProgramsPackageDAL: " + ex);
            }

        }
        public async Task<int> DeleteProgramPackagesbyProgramId(long id, string PackageCode, string RoomType, long Amount, DateTime? FromDate, DateTime? ApplyDate)
        {

            try
            {

                SqlParameter[] objParam = new SqlParameter[6];
                objParam[0] = new SqlParameter("@ProgramId", id);
                objParam[1] = new SqlParameter("@PackageCode", PackageCode.Trim());
                objParam[2] = new SqlParameter("@RoomType", RoomType.Trim());
                objParam[3] = Amount == 0 ? new SqlParameter("@Amount", DBNull.Value) : new SqlParameter("@Amount", Amount);
                objParam[4] = FromDate == null ? new SqlParameter("@FromDate", DBNull.Value) : new SqlParameter("@FromDate", FromDate);
                objParam[5] = ApplyDate == null ? new SqlParameter("@ApplyDate", DBNull.Value) : new SqlParameter("@ApplyDate", ApplyDate);


                return _DbWorker.ExecuteNonQuery(StoreProcedureConstant.Sp_DeleteProgramPackagesByProgramId, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("DeletebyProgramId - ProgramsDAL: " + ex);
            }
            return 0;
        }
        public async Task<ProgramPackage> GetProgramPackagesbyId(long id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var ProgramsBK = _DbContext.ProgramPackage.Where(s => s.Id == id).FirstOrDefault();
                    if (ProgramsBK != null)
                    {
                        return ProgramsBK;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetProgramPackagesbyId - ProgramsPackageDAL: " + ex.ToString());
                return null;
            }
        }

        public DateTime? GetDate(DateTime? FromDate, DateTime? ToDate, string weekday)
        {
            var Day = Convert.ToDateTime(FromDate).Day;
            var Day2 = Convert.ToDateTime(ToDate).Day;
            var Month = Convert.ToDateTime(FromDate).Month;
            var Year = Convert.ToDateTime(FromDate).Year;
            int weekday1 = Convert.ToInt32(weekday);
            if (weekday != "0")
            {
                weekday1 = weekday1 - 1;
            }
            for (int i = 0; i <= (Convert.ToDateTime(ToDate) - Convert.ToDateTime(FromDate)).Days; i++)
            {

                var dd = Day + i;

                var Week = (int)((Convert.ToDateTime(FromDate).AddDays(i)).DayOfWeek);

                if (Week == weekday1)
                {

                    var a = Convert.ToDateTime(FromDate).AddDays(i);
                    return a;
                }
            }
            return null;
        }
        public async Task<List<ProgramPackage>> GetListByProgramId(List<long> ids)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.ProgramPackage.Where(s => ids.Contains((long)s.ProgramId)).ToList();

                }
                return null;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetProgramPackagesbyId - ProgramsPackageDAL: " + ex.ToString());
                return null;
            }
        }
        public async Task<DataTable> GetListProgramsPackageGroupByRoom(ProgramsPackageSearchViewModel searchModel)
        {
            try
            {

                SqlParameter[] objParam = new SqlParameter[12];
                objParam[0] = searchModel.ProgramId == null ? new SqlParameter("@ProgramId", DBNull.Value) : new SqlParameter("@ProgramId", searchModel.ProgramId);
                objParam[1] = searchModel.FromDate == null ? new SqlParameter("@FromDate", DBNull.Value) : new SqlParameter("@FromDate", DateUtil.StringToDate(searchModel.FromDate));
                objParam[2] = searchModel.ToDate == null ? new SqlParameter("@ToDate", DBNull.Value) : new SqlParameter("@ToDate", DateUtil.StringToDate(searchModel.ToDate));
                objParam[3] = new SqlParameter("@PageIndex", searchModel.PageIndex);
                objParam[4] = new SqlParameter("@PageSize", searchModel.PageSize);
                objParam[5] = searchModel.SupplierID == null ? new SqlParameter("@SupplierID", DBNull.Value) : new SqlParameter("@SupplierID", searchModel.SupplierID);
                objParam[6] = searchModel.HotelId == null ? new SqlParameter("@HotelId", DBNull.Value) : new SqlParameter("@HotelId", searchModel.HotelId);
                objParam[7] = searchModel.StayStartDateFrom == null ? new SqlParameter("@StayStartDateFrom", DBNull.Value) : new SqlParameter("@StayStartDateFrom", DateUtil.StringToDate(searchModel.StayStartDateFrom));
                objParam[8] = searchModel.StayStartDateTo == null ? new SqlParameter("@StayStartDateTo", DBNull.Value) : new SqlParameter("@StayStartDateTo", DateUtil.StringToDate(searchModel.StayStartDateTo));
                objParam[9] = searchModel.StayEndDateFrom == null ? new SqlParameter("@StayEndDateFrom", DBNull.Value) : new SqlParameter("@StayEndDateFrom", DateUtil.StringToDate(searchModel.StayEndDateFrom));
                objParam[10] = searchModel.StayEndDateTo == null ? new SqlParameter("@StayEndDateTo", DBNull.Value) : new SqlParameter("@StayEndDateTo", DateUtil.StringToDate(searchModel.StayEndDateTo));
                objParam[11] = searchModel.Status == null ? new SqlParameter("@Status", DBNull.Value) : new SqlParameter("@Status", searchModel.Status);


                return _DbWorker.GetDataTable(StoreProcedureConstant.SP_GetListProgramsPackageGroupByRoom, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetListProgramsPackageGroupByRoom - ProgramsPackageDAL: " + ex);
            }
            return null;
        }
       
    }
}
