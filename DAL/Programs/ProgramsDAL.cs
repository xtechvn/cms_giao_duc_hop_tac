using DAL.Generic;
using DAL.StoreProcedure;
using Entities.Models;
using Entities.ViewModels.Programs;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
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
    public class ProgramsDAL : GenericService<Entities.Models.Programs>
    {
        private static DbWorker _DbWorker;
        public ProgramsDAL(string connection) : base(connection)
        {
            _DbWorker = new DbWorker(connection);
        }
        public List<Entities.Models.Programs> GetAll(int PageIndex, int PageSize)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var Programs = _DbContext.Programs.AsNoTracking().OrderByDescending(s => s.CreatedDate).Skip((PageIndex - 1) * PageSize).Take(PageSize).ToList();
                    if (Programs != null)
                    {
                        return Programs;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetAll - ProgramsDAL: " + ex.ToString());
                return null;
            }
        }
        public List<Entities.Models.Programs> GetProgramsbyProgramId(long id,int PageIndex, int PageSize)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var Programs = _DbContext.Programs.AsNoTracking().Where(s => s.Id == id).Skip((PageIndex - 1) * PageSize).Take(PageSize).ToList();
                    if (Programs != null)
                    {
                        return Programs;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetAll - ProgramsDAL: " + ex.ToString());
                return null;
            }
        }
        public async Task<DataTable> GetPagingList(ProgramsSearchViewModel searchModel)
        {

            try
            {

                SqlParameter[] objParam = new SqlParameter[19];
                objParam[0] = new SqlParameter("@ProgramCode", searchModel.ProgramCode);
                objParam[1] = new SqlParameter("@Description", searchModel.Description);
                objParam[2] = new SqlParameter("@ProgramStatus", searchModel.ProgramStatus);
                objParam[3] = searchModel.ServiceType==""? new SqlParameter("@ServiceType", DBNull.Value) : new SqlParameter("@ServiceType", searchModel.ServiceType);
                objParam[4] = new SqlParameter("@SupplierID", searchModel.SupplierID);
                objParam[5] = new SqlParameter("@ClientId", searchModel.ClientId);
                objParam[6] = searchModel.StartDateFrom == null ? new SqlParameter("@StartDateFrom", DBNull.Value) : new SqlParameter("@StartDateFrom", DateUtil.StringToDate(searchModel.StartDateFrom));
                objParam[7] = searchModel.StartDateTo == null ? new SqlParameter("@StartDateTo", DBNull.Value) : new SqlParameter("@StartDateTo", DateUtil.StringToDate(searchModel.StartDateTo));
                objParam[8] = searchModel.EndDateFrom == null? new SqlParameter("@EndDateFrom", DBNull.Value): new SqlParameter("@EndDateFrom", DateUtil.StringToDate(searchModel.EndDateFrom));
                objParam[9] = searchModel.EndDateTo == null ? new SqlParameter("@EndDateTo", DBNull.Value) : new SqlParameter("@EndDateTo", DateUtil.StringToDate(searchModel.EndDateTo));
                objParam[10] = new SqlParameter("@UserCreate", searchModel.UserCreate);
                objParam[11] = searchModel.CreateDateFrom == null ? new SqlParameter("@CreateDateFrom", DBNull.Value) : new SqlParameter("@CreateDateFrom", DateUtil.StringToDate(searchModel.CreateDateFrom));
                objParam[12] = searchModel.CreateDateTo == null ? new SqlParameter("@CreateDateTo", DBNull.Value) : new SqlParameter("@CreateDateTo", DateUtil.StringToDate(searchModel.CreateDateTo));
                objParam[13] = new SqlParameter("@UserVerify", searchModel.UserVerify);
                objParam[14] = searchModel.VerifyDateFrom == null ? new SqlParameter("@VerifyDateFrom", DBNull.Value) : new SqlParameter("@VerifyDateFrom", DateUtil.StringToDate(searchModel.VerifyDateFrom));
                objParam[15] = searchModel.VerifyDateTo == null ? new SqlParameter("@VerifyDateTo", DBNull.Value) : new SqlParameter("@VerifyDateTo", DateUtil.StringToDate(searchModel.VerifyDateTo));
                objParam[16] = new SqlParameter("@PageIndex", searchModel.PageIndex);
                objParam[17] = new SqlParameter("@PageSize", searchModel.PageSize);
                objParam[18] = new SqlParameter("@SalerPermission", searchModel.SalerPermission);


                return  _DbWorker.GetDataTable(StoreProcedureConstant.SP_GetListPrograms, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetPagingList - ProgramsDAL: " + ex);
            }
            return null;
        }
        public async Task<int> InsertPrograms(ProgramsModel Model)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[16];
                objParam[0] = new SqlParameter("@ProgramCode", Model.ProgramCode);
                objParam[1] = new SqlParameter("@ProgramName", Model.ProgramName);
                objParam[2] = new SqlParameter("@SupplierId", Model.SupplierId);
                objParam[3] = new SqlParameter("@ServiceType", Model.ServiceType);
                objParam[4] = new SqlParameter("@ServiceName", Model.ServiceName);
                objParam[5] = new SqlParameter("@StartDate", Model.StartDate);
                objParam[6] = new SqlParameter("@EndDate", Model.EndDate);
                objParam[7] = Model.Description== null? new SqlParameter("@Description",DBNull.Value): new SqlParameter("@Description", Model.Description);
                objParam[8] = new SqlParameter("@Status", Model.Status);
                objParam[9] = new SqlParameter("@HotelId", Model.HotelId);
                objParam[10] = new SqlParameter("@UserVerify", DBNull.Value);
                objParam[11] = new SqlParameter("@StayStartDate", Model.StayStartDate);
                objParam[12] = new SqlParameter("@StayEndDate", Model.StayEndDate);
                objParam[13] = new SqlParameter("@VerifyDate", DBNull.Value);
                objParam[14] = new SqlParameter("@CreatedBy", Model.CreatedBy);
                objParam[15] = new SqlParameter("@CreatedDate", DBNull.Value);
               
  
                return _DbWorker.ExecuteNonQuery(StoreProcedureConstant.SP_InsertPrograms, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("InsertPrograms - ProgramsDAL: " + ex);
                return 0;
            }

        }
        public async Task<int> UpdatePrograms(ProgramsModel Model)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[16];
                objParam[0] = new SqlParameter("@Id", Model.Id);
                objParam[1] = new SqlParameter("@ProgramCode", Model.ProgramCode);
                objParam[2] = new SqlParameter("@ProgramName", Model.ProgramName);
                objParam[3] = new SqlParameter("@SupplierId", Model.SupplierId);
                objParam[4] = new SqlParameter("@ServiceType", Model.ServiceType);
                objParam[5] = new SqlParameter("@ServiceName", Model.ServiceName);
                objParam[6] = Model.StartDate == null? new SqlParameter("@StartDate", DBNull.Value) :new SqlParameter("@StartDate", Model.StartDate);
                objParam[7] = Model.EndDate == null ? new SqlParameter("@EndDate", DBNull.Value) :new SqlParameter("@EndDate", Model.EndDate);
                objParam[8] = Model.Description==null ? new SqlParameter("@Description", DBNull.Value): new SqlParameter("@Description", Model.Description);
                objParam[9] = new SqlParameter("@Status", Model.Status);
                objParam[10] = new SqlParameter("@HotelId", Model.HotelId);
                objParam[11] = new SqlParameter("@UserVerify", Model.UserVerify);
                objParam[12] = new SqlParameter("@StayStartDate", Model.StayStartDate);
                objParam[13] = new SqlParameter("@StayEndDate", Model.StayEndDate);
                objParam[14] = new SqlParameter("@VerifyDate", DBNull.Value);
                objParam[15] = new SqlParameter("@UpdatedBy", Model.UpdatedBy);
   
            
                return _DbWorker.ExecuteNonQuery(StoreProcedureConstant.SP_UpdatePrograms, objParam);
            }
            catch (Exception ex)
            {
                return 0;
                LogHelper.InsertLogTelegram("UpdatePrograms - ProgramsDAL: " + ex);
            }

        }
        public async Task<DataTable> getDetailById(long id)
        {

            try
            {

                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@ProgramId", id);
                return _DbWorker.GetDataTable(StoreProcedureConstant.sp_GetDetailProgram, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("getDetailById - ProgramsDAL: " + ex);
            }
            return null;
        }
        public async Task<int> UpdateProgramsStatus(int Status,long id ,long userid)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[16];
                objParam[0] = new SqlParameter("@Id", id);
                objParam[1] = new SqlParameter("@ProgramCode", DBNull.Value);
                objParam[2] = new SqlParameter("@ProgramName", DBNull.Value);
                objParam[3] = new SqlParameter("@SupplierId", DBNull.Value);
                objParam[4] = new SqlParameter("@ServiceType", DBNull.Value);
                objParam[5] = new SqlParameter("@ServiceName", DBNull.Value);
                objParam[6] = new SqlParameter("@StartDate", DBNull.Value);
                objParam[7] = new SqlParameter("@EndDate", DBNull.Value);
                objParam[8] = new SqlParameter("@Description", DBNull.Value);
                objParam[9] = new SqlParameter("@Status", Status);
                objParam[10] = new SqlParameter("@HotelId", DBNull.Value);
                objParam[11] = new SqlParameter("@UserVerify", userid);
                objParam[12] = new SqlParameter("@StayStartDate", DBNull.Value);
                objParam[13] = new SqlParameter("@StayEndDate", DBNull.Value);
                objParam[14] = new SqlParameter("@VerifyDate", DBNull.Value);
                objParam[15] = new SqlParameter("@UpdatedBy", userid);
     
            
                return _DbWorker.ExecuteNonQuery(StoreProcedureConstant.SP_UpdatePrograms, objParam);
            }
            catch (Exception ex)
            {
                return 0;
                LogHelper.InsertLogTelegram("UpdatePrograms - ProgramsDAL: " + ex);
            }

        }
        public async Task<List<HotelModel>> GetlistHotelBySupplierId( long id)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@SupplierId", id);
          
                DataTable dt= _DbWorker.GetDataTable(StoreProcedureConstant.sp_GetlistHotelBySupplierId, objParam);
                if (dt != null && dt.Rows.Count > 0)
                {
                    var data = dt.ToList<HotelModel>();
                    return data;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
           
                LogHelper.InsertLogTelegram("GetlistHotelBySupplierId - ProgramsDAL: " + ex);
                return null;
            }

        }
        public async Task<List<SupplierModel>> GetlistSupplierByHotel(long id)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[1];
                objParam[0] = new SqlParameter("@HotelId", id);

                DataTable dt = _DbWorker.GetDataTable(StoreProcedureConstant.SP_GetListSupplierByHotel, objParam);
                if (dt != null && dt.Rows.Count > 0)
                {
                    var data = dt.ToList<SupplierModel>();
                    return data;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {

                LogHelper.InsertLogTelegram("GetlistHotelBySupplierId - ProgramsDAL: " + ex);
                return null;
            }

        }
        public async Task<DataTable> GetlistHotelBySupplierId(ProgramsSearchSupplierId model)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[4];
                objParam[0] = new SqlParameter("@SupplierID", model.SupplierID);
                objParam[1] = new SqlParameter("@PageIndex", model.PageIndex);
                objParam[2] = new SqlParameter("@PageSize", model.PageSize);
                objParam[3] = new SqlParameter("@ServiceName", model.ServiceName);

                return _DbWorker.GetDataTable(StoreProcedureConstant.SP_GetListProgramsBySupplierId, objParam);
            }
            catch (Exception ex)
            {

                LogHelper.InsertLogTelegram("GetlistHotelBySupplierId - ProgramsDAL: " + ex);
                return null;
            }

        }
        public async Task<DataTable> GetHotelPricePolicyByPrograms(int hotel_id)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[] {
                    new SqlParameter("@HotelID", hotel_id)

                };
                //SqlParameter[] objParam = new SqlParameter[3];
                //objParam[0] = new SqlParameter("@HotelID", hotel_id);
                //objParam[1] = new SqlParameter("@ArrivalDate", from_date);
                //objParam[2] = new SqlParameter("@DepartureDate", to_date);

                return _DbWorker.GetDataTable(StoreProcedureConstant.GetHotelPricePolicyByPrograms, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetListPriceDetailByListPackagesId - PriceDetailDAL: " + ex);
            }
            return null;
        }
        public async Task<DataTable> GetHotelPricePolicyDailyByPrograms(int hotel_id)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[] {
                    new SqlParameter("@HotelID", hotel_id)

                };
                //objParam[0] = new SqlParameter("@HotelID", hotel_id);
                //objParam[1] = new SqlParameter("@ArrivalDate", from_date);
                //objParam[2] = new SqlParameter("@DepartureDate", to_date);

                return _DbWorker.GetDataTable(StoreProcedureConstant.GetHotelPricePolicyDailyByPrograms, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetListPriceDetailByListPackagesId - PriceDetailDAL: " + ex);
            }
            return null;
        }
    }
}
