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
    public class ProgramPackageDailyDAL : GenericService<ProgramPackageDaily>
    {
        private static DbWorker _DbWorker;
        public ProgramPackageDailyDAL(string connection) : base(connection)
        {
            _DbWorker = new DbWorker(connection);
        }
        public async Task<int> InsertProgramPackageDaily(InsertProgramsPackageViewModel Model, long Userid)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[13];
                objParam[0] = new SqlParameter("@PackageCode", Model.PackageCode);
                objParam[1] = new SqlParameter("@PackageName", Model.PackageName);
                objParam[2] = new SqlParameter("@ProgramId", Model.ProgramId);
                objParam[3] = new SqlParameter("@RoomType", Model.RoomType);
                objParam[4] = new SqlParameter("@RoomTypeId", Model.RoomTypeId);
                objParam[5] = new SqlParameter("@FromDate", Model.FromDate);
                objParam[6] = new SqlParameter("@ToDate", Model.ToDate);
                objParam[7] = new SqlParameter("@WeekDay", Model.WeekDay);
                objParam[8] = new SqlParameter("@ApplyDate", Model.FromDate);
                objParam[9] = new SqlParameter("@OpenStatus", Model.OpenStatus);
                objParam[10] = new SqlParameter("@Price", Model.Price);
                objParam[11] = new SqlParameter("@CreatedDate", DBNull.Value);
                objParam[12] = new SqlParameter("@CreatedBy", Userid);
                return _DbWorker.ExecuteNonQuery(StoreProcedureConstant.sp_InsertProgramPackageDaily, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("InsertPrograms - ProgramPackageDailyDAL: " + ex);
                return 0;
            }

        }
        public async Task<int> CheckExistsProgramPackageDailyDate(InsertProgramsPackageViewModel Model)
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
                LogHelper.InsertLogTelegram("CheckExistsProgramsPackageByDate - ProgramPackageDailyDAL: " + ex);
            }

        }
        public async Task<int> CheckExistsProgramsPackageDailyByDate(InsertProgramsPackageViewModel Model)
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

                var dt = _DbWorker.GetDataTable(StoreProcedureConstant.SP_CheckExistsProgramsPackageDailyByDate, objParam);
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
                LogHelper.InsertLogTelegram("CheckExistsProgramsPackageByDate - ProgramPackageDailyDAL: " + ex);
            }

        }
        public async Task<DataTable> GetListProgramsPackageDaily(ProgramsPackageSearchViewModel searchModel)
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



                return _DbWorker.GetDataTable(StoreProcedureConstant.SP_GetListProgramsPackageDaily, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetPagingList - ProgramPackageDailyDAL: " + ex);
            }
            return null;
        }
        public async Task<int> DeleteProgramPackagesDailyByProgramId(long id, string PackageCode, string RoomType, long Amount, DateTime? FromDate,int WeekDay)
        {

            try
            {

                SqlParameter[] objParam = new SqlParameter[6];
                objParam[0] = new SqlParameter("@ProgramId", id);
                objParam[1] = new SqlParameter("@PackageCode", PackageCode.Trim());
                objParam[2] = new SqlParameter("@RoomType", RoomType.Trim());
                objParam[3] = Amount == 0 ? new SqlParameter("@Amount", DBNull.Value) : new SqlParameter("@Amount", Amount);
                objParam[4] = new SqlParameter("@FromDate", FromDate);
                objParam[5] = WeekDay == 9 ? new SqlParameter("@WeekDay", DBNull.Value) :new SqlParameter("@WeekDay", WeekDay);


                return _DbWorker.ExecuteNonQuery(StoreProcedureConstant.SP_DeleteProgramPackagesDailyByProgramId, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("DeletebyProgramId - ProgramPackageDailyDAL: " + ex);
            }
            return 0;
        }
        public async Task<ProgramPackageDaily> GetProgramPackageDailybyId(long id,long RoomTypeId)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var ProgramsBK = _DbContext.ProgramPackageDaily.Where(s => s.ProgramId == id && s.RoomTypeId== RoomTypeId).FirstOrDefault();
                    if (ProgramsBK != null)
                    {
                        return ProgramsBK;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetProgramPackageDailybyId - ProgramPackageDailyDAL: " + ex.ToString());
                return null;
            }
        }
        public async Task<DataTable> GetListProgramsPackageDailyGroupByRoom(ProgramsPackageSearchViewModel searchModel)
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


                return _DbWorker.GetDataTable(StoreProcedureConstant.SP_GetListProgramsPackageDailyGroupByRoomId, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetListProgramsPackageDailyGroupByRoom - ProgramsPackageDAL: " + ex);
            }
            return null;
        }
    }
}
