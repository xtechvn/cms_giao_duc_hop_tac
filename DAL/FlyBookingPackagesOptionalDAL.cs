using DAL.Generic;
using DAL.StoreProcedure;
using Entities.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace DAL
{
    public class FlyBookingPackagesOptionalDAL : GenericService<FlyBookingPackagesOptional>
    {
        private static DbWorker _DbWorker;
        public FlyBookingPackagesOptionalDAL(string connection) : base(connection)
        {
            _DbWorker = new DbWorker(connection);
        }
        public async Task<long> CreateOrUpdatePackageOptional(FlyBookingPackagesOptional packages)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var exists = _DbContext.FlyBookingPackagesOptional.AsNoTracking().FirstOrDefault(s => s.Id == packages.Id);
                    if (exists != null && exists.Id > 0)
                    {
                        return UpdateFlyBookingPackagesOptional(packages);
                    }
                    else
                    {
                        return CreateFlyBookingPackagesOptional(packages);

                    }
                }

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CreateFlyBookingPackagesOptional - HotelBookingDAL. " + ex);
                return -1;
            }
        }
        public async Task<List<FlyBookingPackagesOptional>> GetBookingPackagesOptionalsByBookingId(long booking_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return await _DbContext.FlyBookingPackagesOptional.AsNoTracking().Where(s => s.BookingId == booking_id).ToListAsync();

                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetBookingPackagesOptionalsByBookingId - HotelBookingDAL. " + ex);
                return null;
            }
        }
        public int CreateFlyBookingPackagesOptional(FlyBookingPackagesOptional packages)
        {
            try
            {

                SqlParameter[] objParam_order = new SqlParameter[7];
                objParam_order[0] = new SqlParameter("@BookingId", packages.BookingId);
                objParam_order[1] = new SqlParameter("@SuplierID", packages.SuplierId);
                objParam_order[2] = new SqlParameter("@Amount", packages.Amount);
                if (packages.Note != null)
                {
                    objParam_order[3] = new SqlParameter("@Note", packages.Note);
                }
                else
                {
                    objParam_order[3] = new SqlParameter("@Note", DBNull.Value);

                }
                objParam_order[4] = new SqlParameter("@CreatedBy", packages.CreatedBy);
                objParam_order[5] = new SqlParameter("@PackageName", packages.PackageName);
                objParam_order[6] = new SqlParameter("@Status", packages.Status == null ? 0 : packages.Status);

                var id = _DbWorker.ExecuteNonQuery(StoreProcedureConstant.InsertFlyBookingPackagesOptional, objParam_order);
                packages.Id = id;
                return id;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CreateFlyBookingPackagesOptional - HotelBookingDAL. " + ex);
                return -1;
            }
        }
        public int UpdateFlyBookingPackagesOptional(FlyBookingPackagesOptional packages)
        {
            try
            {

                SqlParameter[] objParam_order = new SqlParameter[8];
                objParam_order[0] = new SqlParameter("@BookingId", packages.BookingId);
                objParam_order[1] = new SqlParameter("@SuplierID", packages.SuplierId);
                objParam_order[2] = new SqlParameter("@Amount", packages.Amount);
                if (packages.Note != null)
                {
                    objParam_order[3] = new SqlParameter("@Note", packages.Note);
                }
                else
                {
                    objParam_order[3] = new SqlParameter("@Note", DBNull.Value);

                }
                objParam_order[4] = new SqlParameter("@UpdatedBy", packages.UpdatedBy);
                objParam_order[5] = new SqlParameter("@Id", packages.Id);
                objParam_order[6] = new SqlParameter("@PackageName", packages.PackageName);
                objParam_order[7] = new SqlParameter("@Status", packages.Status);

                _DbWorker.ExecuteNonQueryNoIdentity(StoreProcedureConstant.UpdateFlyBookingPackagesOptional, objParam_order);
                return 1;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateFlyBookingPackagesOptional - HotelBookingDAL. " + ex);
                return -1;
            }
        }
    }
}
