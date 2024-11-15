using DAL.Generic;
using DAL.StoreProcedure;
using Entities.Models;
using Entities.ViewModels;
using Entities.ViewModels.OrderManual;
using Entities.ViewModels.Tour;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace DAL
{
    public class TourDAL : GenericService<Tour>
    {
        private DbWorker dbWorker;

        public TourDAL(string connection) : base(connection)
        {
            dbWorker = new DbWorker(connection);

        }
        public async Task<Tour> GetTourById(long tour_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return await _DbContext.Tour.Where(x => x.Id == tour_id).FirstOrDefaultAsync();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetTourById - TourDAL: " + ex);
                return null;
            }
        }
        public async Task<long> SummitTourData(OrderManualTourSQLServiceSummitModel model)
        {
            try
            {

                using (var _DbContext = new EntityDataContext(_connection))
                {
                    List<long> tour_guest_new_id = new List<long>();
                    List<long> tour_packages_new_id = new List<long>();
                    var order = await _DbContext.Order.AsNoTracking().FirstOrDefaultAsync(x => x.OrderId == model.detail.OrderId);
                    if (order == null || order.OrderId <= 0)
                    {
                        return -1;
                    }
                    if (model.product.Id <= 0)
                    {
                        var tour_product_id = CreateTourProduct(model.product);
                        foreach (var dest in model.destinations)
                        {
                            dest.TourId = tour_product_id;
                            var id = CreateTourDestination(dest);
                        }
                        model.detail.TourProductId = tour_product_id;
                    }
                    else if (model.product.IsSelfDesigned == true)
                    {
                        var tour_product_id = UpdateTourProduct(model.product);
                        var exists_destination_list = _DbContext.TourDestination.AsNoTracking().Where(x => x.TourId == model.product.Id);
                        if (exists_destination_list != null && exists_destination_list.Count() > 0)
                        {
                            var remain_destination_ids = new List<long>();
                            if (model.destinations != null)
                            {
                                foreach (var dest in model.destinations)
                                {
                                    var match_destination = exists_destination_list.FirstOrDefault(x => x.Type == dest.Type && x.LocationId == dest.LocationId);
                                    if (match_destination != null)
                                    {
                                        remain_destination_ids.Add(match_destination.Id);
                                        continue;
                                    }
                                    else
                                    {
                                        var id = CreateTourDestination(dest);
                                        remain_destination_ids.Add(dest.Id);
                                    }
                                }
                                await DeleteNonExistsTourDestination(remain_destination_ids, model.product.Id, model.detail.CreatedBy);
                            }
                        }
                        else
                        {
                            foreach (var dest in model.destinations)
                            {
                                var id = CreateTourDestination(dest);
                            }
                        }
                    }
                    var exists_tour = await _DbContext.Tour.AsNoTracking().FirstOrDefaultAsync(x => x.Id == model.detail.Id);
                    if (exists_tour != null && exists_tour.Id > 0)
                    {
                        model.detail.CreatedBy = exists_tour.CreatedBy;
                        model.detail.CreatedDate = exists_tour.CreatedDate;
                        UpdateTour(model.detail);
                        List<long> new_extra_package_id = new List<long>();
                        foreach (var package in model.extra_packages)
                        {
                            package.TourId = model.detail.Id;
                            var exists_tour_package = await _DbContext.TourPackages.AsNoTracking().FirstOrDefaultAsync(x => x.Id == package.Id);
                            if (exists_tour_package != null && exists_tour_package.Id > 0)
                            {
                                package.CreatedBy = exists_tour_package.CreatedBy;
                                package.CreatedDate = exists_tour_package.CreatedDate;
                                UpdateTourPackages(package);
                            }
                            else
                            {
                                CreateTourPackages(package);
                            }
                            tour_packages_new_id.Add(package.Id);
                        }
                        if (model.guest != null && model.guest.Count > 0)
                        {
                            foreach (var guest in model.guest)
                            {
                                guest.TourId = model.detail.Id;
                                var exists_tour_guest = await _DbContext.TourGuests.AsNoTracking().FirstOrDefaultAsync(x => x.Id == guest.Id);
                                if (exists_tour_guest != null && exists_tour_guest.Id > 0)
                                {
                                    guest.CreatedBy = exists_tour_guest.CreatedBy;
                                    guest.CreatedDate = exists_tour_guest.CreatedDate;
                                    UpdateTourGuest(guest);
                                }
                                else
                                {
                                    CreateTourGuest(guest);
                                }
                                tour_guest_new_id.Add(guest.Id);
                            }
                        }
                        await DeleteNonExistsTourData(tour_guest_new_id, tour_packages_new_id, model.detail.Id, model.detail.UpdatedBy);

                    }
                    else
                    {
                        CreateTour(model.detail);
                        foreach (var package in model.extra_packages)
                        {
                            package.TourId = model.detail.Id;
                            CreateTourPackages(package);
                            tour_packages_new_id.Add(package.Id);
                        }
                        if (model.guest != null && model.guest.Count > 0)
                        {
                            foreach (var guest in model.guest)
                            {
                                guest.TourId = model.detail.Id;
                                CreateTourGuest(guest);
                                tour_guest_new_id.Add(guest.Id);
                            }
                        }

                    }


                    return model.detail.Id;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SummitTourData - TourDAL: " + ex);
                return -2;
            }
        }
        private async Task<bool> DeleteNonExistsTourData(List<long> remain_guests, List<long> remain_packages, long tour_id, int? updated_user)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var delete_guest = await _DbContext.TourGuests.AsNoTracking().Where(x => x.TourId == tour_id && !remain_guests.Contains(x.Id)).ToListAsync();
                    if (delete_guest != null && delete_guest.Count > 0)
                    {
                        foreach (var guest in delete_guest)
                        {
                            guest.TourId = guest.TourId * -1;
                            guest.UpdatedBy = updated_user;
                            UpdateTourGuest(guest);
                        }
                    }
                    var delete_packages = await _DbContext.TourPackages.AsNoTracking().Where(x => x.TourId == tour_id && !remain_packages.Contains(x.Id)).ToListAsync();
                    if (delete_packages != null && delete_packages.Count > 0)
                    {
                        foreach (var package in delete_packages)
                        {
                            package.TourId = package.TourId * -1;
                            package.UpdatedBy = updated_user;
                            if (package.UnitPrice == null) package.UnitPrice = 0;
                            UpdateTourPackages(package);
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("DeleteNonExistsTourData - TourDAL. " + ex);
                return false;
            }
        }
        private async Task<bool> DeleteNonExistsTourDestination(List<long> remain_destination, long tour_id, int? updated_user)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var delete_destination = await _DbContext.TourDestination.AsNoTracking().Where(x => x.TourId == tour_id && !remain_destination.Contains(x.Id)).ToListAsync();
                    if (delete_destination != null && delete_destination.Count > 0)
                    {
                        _DbContext.TourDestination.RemoveRange(delete_destination);
                        await _DbContext.SaveChangesAsync();

                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("DeleteNonExistsTourData - TourDAL. " + ex);
                return false;
            }
        }
        public async Task<long> DeleteTourByID(long id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var tour = await _DbContext.Tour.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                    if (tour != null && tour.Id > 0)
                    {
                        var tour_product = await _DbContext.TourProduct.AsNoTracking().FirstOrDefaultAsync(x => x.Id == tour.TourProductId);
                        var another_tour_same_tour_product = await _DbContext.Tour.AsNoTracking().FirstOrDefaultAsync(x => x.TourProductId == tour.TourProductId && x.Id != tour.Id);
                        if (tour_product != null && tour_product.Id > 0 && (tour_product.IsSelfDesigned == null || (bool)tour_product.IsSelfDesigned) && (another_tour_same_tour_product == null || another_tour_same_tour_product.Id <= 0))
                        {
                            var tour_product_destination = await _DbContext.TourDestination.AsNoTracking().Where(x => x.TourId == tour.TourProductId).ToListAsync();
                            if (tour_product_destination != null && tour_product_destination.Count > 0)
                            {
                                foreach (var destination in tour_product_destination)
                                {
                                    destination.TourId *= -1;
                                    _DbContext.TourDestination.Update(destination);
                                    await _DbContext.SaveChangesAsync();
                                }
                                //_DbContext.TourDestination.RemoveRange(tour_product_destination);
                                //await _DbContext.SaveChangesAsync();
                            }
                            tour_product.IsDelete =true;
                            //_DbContext.TourProduct.Remove(tour_product);
                            _DbContext.TourProduct.Update(tour_product);
                            await _DbContext.SaveChangesAsync();
                        }

                        var extra = await _DbContext.TourPackages.AsNoTracking().Where(x => x.TourId == tour.Id).ToListAsync();
                        if (extra != null && extra.Count > 0)
                        {
                            foreach(var tour_extra in extra)
                            {
                                tour_extra.TourId *= -1;
                            }
                            _DbContext.TourPackages.UpdateRange(extra);
                            await _DbContext.SaveChangesAsync();
                            //_DbContext.TourPackages.RemoveRange(extra);
                            //await _DbContext.SaveChangesAsync();
                        }

                        var passengers = await _DbContext.TourGuests.AsNoTracking().Where(x => x.TourId == tour.Id).ToListAsync();
                        if (passengers != null && passengers.Count > 0)
                        {
                            foreach (var pass in passengers)
                            {
                                pass.TourId *= -1;
                            }
                            _DbContext.TourGuests.UpdateRange(passengers);
                            //_DbContext.TourGuests.RemoveRange(passengers);
                            await _DbContext.SaveChangesAsync();
                        }
                        tour.OrderId *= -1;
                        tour.TourProductId *= -1;
                        _DbContext.Tour.Update(tour);
                        //_DbContext.Tour.Remove(tour);
                        await _DbContext.SaveChangesAsync();
                    }
                    return 1;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("DeleteTourByID - TourDAL. " + ex);
                return -1;
            }
        }
        public async Task<long> CancelTourByID(long id, int user_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var tour = await _DbContext.Tour.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                    if (tour != null && tour.Id > 0)
                    {
                        if (tour.Status == (int)ServiceStatus.Decline)
                        {
                            tour.Status = (int)ServiceStatus.Cancel;
                            tour.UpdatedBy = user_id;
                            tour.UpdatedDate = DateTime.Now;
                            _DbContext.Tour.Update(tour);
                            await _DbContext.SaveChangesAsync();
                        }
                    }
                    return 1;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("DeleteTourByID - TourDAL. " + ex);
                return -1;
            }
        }
        public int CreateTour(Tour model)
        {
            try
            {
                SqlParameter[] objParam_order = new SqlParameter[18];
                objParam_order[0] = new SqlParameter("@TourType", model.TourType);
                objParam_order[1] = new SqlParameter("@OrganizingType", model.OrganizingType);

                objParam_order[2] = new SqlParameter("@StartDate", model.StartDate);
                objParam_order[3] = new SqlParameter("@EndDate", model.EndDate);
                objParam_order[4] = new SqlParameter("@OrderId", model.OrderId);
                objParam_order[5] = new SqlParameter("@SalerId", model.SalerId);
                objParam_order[6] = new SqlParameter("@Amount", model.Amount);
                if (model.ServiceCode != null)
                {
                    objParam_order[7] = new SqlParameter("@ServiceCode", model.ServiceCode);
                }
                else
                {
                    objParam_order[7] = new SqlParameter("@ServiceCode", DBNull.Value);
                }
                objParam_order[8] = new SqlParameter("@SupplierId", model.SupplierId);
                objParam_order[9] = new SqlParameter("@Status", model.Status);
                objParam_order[10] = new SqlParameter("@TourProductId", model.TourProductId);

                objParam_order[11] = new SqlParameter("@CreatedBy", model.CreatedBy);
                objParam_order[12] = new SqlParameter("@CreatedDate", model.CreatedDate);
                objParam_order[13] = new SqlParameter("@Profit", model.Profit);
                objParam_order[14] = new SqlParameter("@Note", model.Note);
                objParam_order[15] = new SqlParameter("@Commission", model.Commission);
                objParam_order[16] = new SqlParameter("@OthersAmount", model.OthersAmount);
                objParam_order[17] = new SqlParameter("@FundCustomerCare", model.FundCustomerCare);


                var id = dbWorker.ExecuteNonQuery(StoreProcedureConstant.InsertTour, objParam_order);

                model.Id = id;
                return id;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CreateTour - TourDAL. " + ex);

                return -1;
            }
        }

        public int CreateTourPackages(TourPackages model)
        {
            try
            {

                SqlParameter[] objParam_order = new SqlParameter[12];
                objParam_order[0] = new SqlParameter("@TourId", model.TourId);
                objParam_order[1] = new SqlParameter("@PackageName", model.PackageName);
                objParam_order[2] = new SqlParameter("@BasePrice", model.BasePrice);
                objParam_order[3] = new SqlParameter("@Quantity", model.Quantity);

                objParam_order[4] = new SqlParameter("@AmountBeforeVat", model.AmountBeforeVat);
                objParam_order[5] = new SqlParameter("@AmountVat", model.AmountVat);
                objParam_order[6] = new SqlParameter("@Amount", model.Amount);
                objParam_order[7] = new SqlParameter("@VAT", model.Vat);

                objParam_order[8] = new SqlParameter("@CreatedBy", model.CreatedBy);
                objParam_order[9] = new SqlParameter("@CreatedDate", model.CreatedDate);
                objParam_order[10] = new SqlParameter("@PackageCode", model.PackageCode);
                objParam_order[11] = new SqlParameter("@Profit", model.Profit);


                var id = dbWorker.ExecuteNonQuery(StoreProcedureConstant.InsertTourPackages, objParam_order);
                model.Id = id;
                return id;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CreateTourPackages - TourDAL. " + ex);

                return -1;
            }
        }
        public int UpdateTour(Tour model)
        {
            try
            {

                SqlParameter[] objParam_order = new SqlParameter[19];
                objParam_order[0] = model.TourType == null ? new SqlParameter("@TourType", DBNull.Value) : new SqlParameter("@TourType", model.TourType);


                objParam_order[1] = model.OrganizingType == null ? new SqlParameter("@OrganizingType", DBNull.Value) : new SqlParameter("@OrganizingType", model.OrganizingType);

                objParam_order[2] = model.StartDate == null ? new SqlParameter("@StartDate", DBNull.Value) : new SqlParameter("@StartDate", model.StartDate);
                objParam_order[3] = model.EndDate == null ? new SqlParameter("@EndDate", DBNull.Value) : new SqlParameter("@EndDate", model.EndDate);

                objParam_order[4] = model.OrderId == null ? new SqlParameter("@OrderId", DBNull.Value) : new SqlParameter("@OrderId", model.OrderId);
                objParam_order[5] = model.SalerId == null ? new SqlParameter("@SalerId", DBNull.Value) : new SqlParameter("@SalerId", model.SalerId);
                objParam_order[6] = model.Amount == null ? new SqlParameter("@Amount", DBNull.Value) : new SqlParameter("@Amount", model.Amount);
                if (model.ServiceCode != null)
                {
                    objParam_order[7] = new SqlParameter("@ServiceCode", model.ServiceCode);
                }
                else
                {
                    objParam_order[7] = new SqlParameter("@ServiceCode", DBNull.Value);
                }
                objParam_order[8] = model.UpdatedBy == null ? new SqlParameter("@UpdatedBy", DBNull.Value) : new SqlParameter("@UpdatedBy", model.UpdatedBy);
                objParam_order[9] = new SqlParameter("@Id", model.Id);
                objParam_order[10] = model.SupplierId == null ? new SqlParameter("@SupplierId", DBNull.Value) : new SqlParameter("@SupplierId", model.SupplierId);
                objParam_order[11] = model.Status == null ? new SqlParameter("@Status", DBNull.Value) : new SqlParameter("@Status", model.Status);
                objParam_order[12] = model.TourProductId == null ? new SqlParameter("@TourProductId", DBNull.Value) : new SqlParameter("@TourProductId", model.TourProductId);
                objParam_order[13] = model.Price == null ? new SqlParameter("@Price", DBNull.Value) : new SqlParameter("@Price", model.Price);
                objParam_order[14] = model.Profit == null ? new SqlParameter("@Profit", DBNull.Value) : new SqlParameter("@Profit", model.Profit);
                objParam_order[15] = model.Note == null ? new SqlParameter("@Note", DBNull.Value) : new SqlParameter("@Note", model.Note);

                objParam_order[16] = model.Commission == null ? new SqlParameter("@Commission", DBNull.Value) : new SqlParameter("@Commission", model.Commission);
                objParam_order[17] = model.OthersAmount == null ? new SqlParameter("@OthersAmount", DBNull.Value) : new SqlParameter("@OthersAmount", model.OthersAmount);
                objParam_order[18] = model.FundCustomerCare == null ? new SqlParameter("@FundCustomerCare", DBNull.Value) : new SqlParameter("@FundCustomerCare", model.FundCustomerCare);

                var id = dbWorker.ExecuteNonQuery(StoreProcedureConstant.UpdateTour, objParam_order);
                model.Id = id;
                return id;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateTour - TourDAL. " + ex);
                return -1;
            }
        }

        private int UpdateTourPackages(TourPackages model)
        {
            try
            {

                SqlParameter[] objParam_order = new SqlParameter[13];
                objParam_order[0] = new SqlParameter("@TourId", model.TourId);
                objParam_order[1] = new SqlParameter("@PackageName", model.PackageName);
                objParam_order[2] = new SqlParameter("@BasePrice", model.BasePrice);
                objParam_order[3] = new SqlParameter("@Quantity", model.Quantity);

                objParam_order[4] = new SqlParameter("@AmountBeforeVat", model.AmountBeforeVat);
                objParam_order[5] = new SqlParameter("@AmountVat", model.AmountVat);
                objParam_order[6] = new SqlParameter("@Amount", model.Amount);
                objParam_order[7] = new SqlParameter("@VAT", model.Vat);

                objParam_order[8] = new SqlParameter("@UnitPrice", model.UnitPrice);

                objParam_order[9] = new SqlParameter("@UpdatedBy", model.UpdatedBy);
                objParam_order[10] = new SqlParameter("@PackageCode", model.PackageCode);
                objParam_order[11] = new SqlParameter("@Id", model.Id);
                objParam_order[12] = new SqlParameter("@Profit", model.Profit);


                var id = dbWorker.ExecuteNonQuery(StoreProcedureConstant.UpdateTourPackages, objParam_order);
                model.Id = id;
                return id;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateTourPackages - TourDAL. " + ex);
                return -1;
            }
        }
        private int CreateTourGuest(TourGuests model)
        {
            try
            {

                SqlParameter[] objParam_order = new SqlParameter[10];
                objParam_order[0] = new SqlParameter("@TourId", model.TourId);
                objParam_order[1] = new SqlParameter("@FullName", model.FullName);
                if (model.Birthday != null)
                {
                    objParam_order[2] = new SqlParameter("@Birthday", model.Birthday);
                }
                else
                {
                    objParam_order[2] = new SqlParameter("@Birthday", DBNull.Value);

                }
                if (model.Phone != null && model.Phone.Trim() != "")
                {
                    objParam_order[3] = new SqlParameter("@Phone", model.Phone);
                }
                else
                {
                    objParam_order[3] = new SqlParameter("@Phone", DBNull.Value);

                }

                if (model.Note != null)
                {
                    objParam_order[4] = new SqlParameter("@Note", model.Note);
                }
                else
                {
                    objParam_order[4] = new SqlParameter("@Note", DBNull.Value);

                }


                objParam_order[5] = new SqlParameter("@CreatedBy", model.CreatedBy);
                objParam_order[6] = new SqlParameter("@CreatedDate", model.CreatedDate);
                objParam_order[7] = new SqlParameter("@Gender", model.Gender);
                objParam_order[8] = new SqlParameter("@RoomNumber", model.RoomNumber);
                if (model.Cccd != null)
                {
                    objParam_order[9] = new SqlParameter("@CCCD", model.Cccd);
                }
                else
                {
                    objParam_order[9] = new SqlParameter("@CCCD", DBNull.Value);

                }
                var id = dbWorker.ExecuteNonQuery(StoreProcedureConstant.InsertTourGuests, objParam_order);
                model.Id = id;
                return id;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CreateTourGuest - TourDAL. " + ex.ToString());
                return -1;
            }
        }
        private int UpdateTourGuest(TourGuests model)
        {
            try
            {

                SqlParameter[] objParam_order = new SqlParameter[10];
                objParam_order[0] = new SqlParameter("@TourId", model.TourId);
                objParam_order[1] = new SqlParameter("@FullName", model.FullName);
                if (model.Birthday != null)
                {
                    objParam_order[2] = new SqlParameter("@Birthday", model.Birthday);
                }
                else
                {
                    objParam_order[2] = new SqlParameter("@Birthday", DBNull.Value);

                }
                if (model.Phone != null && model.Phone.Trim() != "")
                {
                    objParam_order[3] = new SqlParameter("@Phone", model.Phone);
                }
                else
                {
                    objParam_order[3] = new SqlParameter("@Phone", DBNull.Value);

                }

                if (model.Note != null)
                {
                    objParam_order[4] = new SqlParameter("@Note", model.Note);
                }
                else
                {
                    objParam_order[4] = new SqlParameter("@Note", DBNull.Value);

                }
                objParam_order[5] = new SqlParameter("@UpdatedBy", model.UpdatedBy);


                objParam_order[6] = new SqlParameter("@Id", model.Id);
                if (model.Gender != null)
                {
                    objParam_order[7] = new SqlParameter("@Gender", model.Gender);
                }
                else
                {
                    objParam_order[7] = new SqlParameter("@Gender", DBNull.Value);

                }
                objParam_order[8] = new SqlParameter("@RoomNumber", model.RoomNumber);
                if (model.Cccd != null)
                {
                    objParam_order[9] = new SqlParameter("@CCCD", model.Cccd);
                }
                else
                {
                    objParam_order[9] = new SqlParameter("@CCCD", DBNull.Value);

                }


                var id = dbWorker.ExecuteNonQuery(StoreProcedureConstant.UpdateTourGuests, objParam_order);
                model.Id = id;
                return id;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateTourGuest - TourDAL. " + ex.ToString());
                return -1;
            }
        }
        public async Task<DataTable> GetTourByOrderId(long OrderId)
        {
            try
            {
                SqlParameter[] objParam_order = new SqlParameter[1];
                objParam_order[0] = new SqlParameter("@OrderID", OrderId);
                return dbWorker.GetDataTable(StoreProcedureConstant.Sp_GetTourByOrderId, objParam_order);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetTourById - TourDAL: " + ex);
                return null;
            }
        }
        public async Task<DataTable> GetDetailTourByID(long TourId)
        {
            try
            {
                SqlParameter[] objParam_order = new SqlParameter[1];
                objParam_order[0] = new SqlParameter("@TourId", TourId);
                return dbWorker.GetDataTable(StoreProcedureConstant.SP_GetDetailTourByID, objParam_order);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetTourById - TourDAL: " + ex);
                return null;
            }
        }
        public async Task<DataTable> GetListTour(TourSearchViewModel model)
        {
            try
            {

                SqlParameter[] objParam_order = new SqlParameter[23];
                objParam_order[0] = new SqlParameter("@ServiceCode", model.ServiceCode);
                objParam_order[1] = new SqlParameter("@OrderCode", model.OrderCode);
                objParam_order[2] = new SqlParameter("@StatusBooking", model.StatusBooking);
                objParam_order[3] = new SqlParameter("@TourType", model.TourType);
                objParam_order[4] = new SqlParameter("@OrganizingType", model.OrganizingType);
                objParam_order[5] = (CheckDate(model.StartDateFrom) == DateTime.MinValue) ? new SqlParameter("@StartDateFrom", DBNull.Value) : new SqlParameter("@StartDateFrom", CheckDate(model.StartDateFrom));
                objParam_order[6] = (CheckDate(model.StartDateTo) == DateTime.MinValue) ? new SqlParameter("@StartDateTo", DBNull.Value) : new SqlParameter("@StartDateTo", CheckDate(model.StartDateTo));
                objParam_order[7] = (CheckDate(model.EndDateFrom) == DateTime.MinValue) ? new SqlParameter("@EndDateFrom", DBNull.Value) : new SqlParameter("@EndDateFrom", CheckDate(model.EndDateFrom));
                objParam_order[8] = (CheckDate(model.EndDateTo) == DateTime.MinValue) ? new SqlParameter("@EndDateTo", DBNull.Value) : new SqlParameter("@EndDateTo", CheckDate(model.EndDateTo));
                objParam_order[9] = new SqlParameter("@UserCreate", model.UserCreate);
                objParam_order[10] = (CheckDate(model.CreateDateFrom) == DateTime.MinValue) ? new SqlParameter("@CreateDateFrom", DBNull.Value) : new SqlParameter("@CreateDateFrom", CheckDate(model.CreateDateFrom));
                objParam_order[11] = (CheckDate(model.CreateDateTo) == DateTime.MinValue) ? new SqlParameter("@CreateDateTo", DBNull.Value) : new SqlParameter("@CreateDateTo", CheckDate(model.CreateDateTo));
                objParam_order[12] = new SqlParameter("@SalerId", model.SalerId);
                objParam_order[13] = new SqlParameter("@OperatorId", model.OperatorId);
                objParam_order[14] = new SqlParameter("@Days", model.Days);
                objParam_order[15] = new SqlParameter("@Star", model.Star);
                objParam_order[16] = new SqlParameter("@IsDisplayWeb", model.IsDisplayWeb);
                objParam_order[17] = new SqlParameter("@StartPoint", model.StartPoint);
                objParam_order[18] = new SqlParameter("@Endpoint", model.Endpoint);
                objParam_order[19] = new SqlParameter("@PageIndex", model.PageIndex);
                objParam_order[20] = new SqlParameter("@PageSize", model.PageSize);
                objParam_order[21] = new SqlParameter("@SalerPermission", model.SalerPermission);
                objParam_order[22] = new SqlParameter("@BookingCode", model.BookingCode);
                return dbWorker.GetDataTable(StoreProcedureConstant.SP_GetListTour, objParam_order);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetListTour - TourDAL: " + ex);
                return null;
            }
        }
        public async Task<int> UpdateTourStatus(long TourId, int Status)
        {
            try
            {
                SqlParameter[] objParam_order = new SqlParameter[2];
                objParam_order[0] = new SqlParameter("@TourId", TourId);
                objParam_order[1] = new SqlParameter("@Status", Status);
                return dbWorker.ExecuteNonQuery(StoreProcedureConstant.SP_UpdateTourStatus, objParam_order);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateTourStatus - TourDAL: " + ex);
                return 0;
            }
        }
        public async Task<DataTable> CountTourByStatus(TourSearchViewModel model)
        {
            try
            {
                SqlParameter[] objParam_order = new SqlParameter[21];
                objParam_order[0] = new SqlParameter("@ServiceCode", model.ServiceCode);
                objParam_order[1] = new SqlParameter("@OrderCode", model.OrderCode);
                objParam_order[2] = new SqlParameter("@StatusBooking", model.StatusBooking);
                objParam_order[3] = new SqlParameter("@TourType", model.TourType);
                objParam_order[4] = new SqlParameter("@OrganizingType", model.OrganizingType);
                objParam_order[5] = (CheckDate(model.StartDateFrom) == DateTime.MinValue) ? new SqlParameter("@StartDateFrom", DBNull.Value) : new SqlParameter("@StartDateFrom", CheckDate(model.StartDateFrom));
                objParam_order[6] = (CheckDate(model.StartDateTo) == DateTime.MinValue) ? new SqlParameter("@StartDateTo", DBNull.Value) : new SqlParameter("@StartDateTo", CheckDate(model.StartDateTo));
                objParam_order[7] = (CheckDate(model.EndDateFrom) == DateTime.MinValue) ? new SqlParameter("@EndDateFrom", DBNull.Value) : new SqlParameter("@EndDateFrom", CheckDate(model.EndDateFrom));
                objParam_order[8] = (CheckDate(model.EndDateTo) == DateTime.MinValue) ? new SqlParameter("@EndDateTo", DBNull.Value) : new SqlParameter("@EndDateTo", CheckDate(model.EndDateTo));
                objParam_order[9] = new SqlParameter("@UserCreate", model.UserCreate);
                objParam_order[10] = (CheckDate(model.CreateDateFrom) == DateTime.MinValue) ? new SqlParameter("@CreateDateFrom", DBNull.Value) : new SqlParameter("@CreateDateFrom", CheckDate(model.CreateDateFrom));
                objParam_order[11] = (CheckDate(model.CreateDateTo) == DateTime.MinValue) ? new SqlParameter("@CreateDateTo", DBNull.Value) : new SqlParameter("@CreateDateTo", CheckDate(model.CreateDateTo));
                objParam_order[12] = new SqlParameter("@SalerId", model.SalerId);
                objParam_order[13] = new SqlParameter("@OperatorId", model.OperatorId);
                objParam_order[14] = new SqlParameter("@Days", model.OperatorId);
                objParam_order[15] = new SqlParameter("@Star", model.OperatorId);
                objParam_order[16] = new SqlParameter("@IsDisplayWeb", model.OperatorId);
                objParam_order[17] = new SqlParameter("@StartPoint", model.OperatorId);
                objParam_order[18] = new SqlParameter("@Endpoint", model.OperatorId);
                objParam_order[19] = new SqlParameter("@SalerPermission", model.SalerPermission);
                objParam_order[20] = new SqlParameter("@BookingCode", model.BookingCode);


                return dbWorker.GetDataTable(StoreProcedureConstant.SP_CountTourByStatus, objParam_order);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateTourStatus - TourDAL: " + ex);
                return null;
            }
        }
        public async Task<List<Tour>> GetAllTour()
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return await _DbContext.Tour.AsNoTracking().ToListAsync();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetAllTour - TourDAL: " + ex);
                return null;
            }
        }
        private DateTime CheckDate(string dateTime)
        {
            DateTime _date = DateTime.MinValue;
            if (!string.IsNullOrEmpty(dateTime))
            {
                _date = DateTime.ParseExact(dateTime, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }

            return _date != DateTime.MinValue ? _date : DateTime.MinValue;
        }


        public async Task<DataTable> TourProductSuggesstion(string txt_search)
        {
            try
            {
                int page = 1;
                int size = 30;
                bool is_self_designed = false;
                SqlParameter[] objParam_order = new SqlParameter[12];
                objParam_order[0] = new SqlParameter("@ServiceCode", DBNull.Value);
                if (txt_search != null && txt_search.Trim() != "")
                {
                    objParam_order[1] = new SqlParameter("@TourName", txt_search);
                }
                else
                {
                    objParam_order[1] = new SqlParameter("@TourName", DBNull.Value);

                }
                objParam_order[2] = new SqlParameter("@TourType", DBNull.Value);
                objParam_order[3] = new SqlParameter("@OrganizingType", DBNull.Value);
                objParam_order[4] = new SqlParameter("@Days", DBNull.Value);
                objParam_order[5] = new SqlParameter("@Star", DBNull.Value);
                objParam_order[6] = new SqlParameter("@IsDisplayWeb", DBNull.Value);
                objParam_order[7] = new SqlParameter("@StartPoint", DBNull.Value);
                objParam_order[8] = new SqlParameter("@Endpoint", DBNull.Value);
                objParam_order[9] = new SqlParameter("@PageIndex", page);
                objParam_order[10] = new SqlParameter("@PageSize", size);
                objParam_order[11] = new SqlParameter("@IsSelfDesign", is_self_designed);

                return dbWorker.GetDataTable(StoreProcedureConstant.SP_GetListTourProduct, objParam_order);

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("TourProductSuggesstion - TourDAL. " + ex.ToString());
                return null;
            }
        }

        public DataTable GetListTourProduct(TourProductSearchModel model)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[]
                {
                    new SqlParameter("@ServiceCode",model.ServiceCode ?? (object)DBNull.Value),
                    new SqlParameter("@TourName",model.TourName ?? (object)DBNull.Value),
                    new SqlParameter("@TourType",model.TourType ?? (object)DBNull.Value),
                    new SqlParameter("@OrganizingType",model.OrganizingType ?? (object)DBNull.Value),
                    new SqlParameter("@Days",model.Days ?? (object)DBNull.Value),
                    new SqlParameter("@Star",model.Star ?? (object)DBNull.Value),
                    new SqlParameter("@IsDisplayWeb",model.IsDisplayWeb ?? (object)DBNull.Value),
                    new SqlParameter("@StartPoint",model.StartPoint ?? (object)DBNull.Value),
                    new SqlParameter("@Endpoint",model.Endpoint ?? (object)DBNull.Value),
                    new SqlParameter("@IsSelfDesign",model.IsSelfDesign ?? (object)DBNull.Value),
                    new SqlParameter("@SupplierId",model.SupplierIds ?? (object)DBNull.Value),
                    new SqlParameter("@PageIndex",model.PageIndex),
                    new SqlParameter("@PageSize",model.PageSize)
                };

                return dbWorker.GetDataTable(StoreProcedureConstant.SP_GetListTourProduct, objParam);

            }
            catch
            {
                throw;
            }
        }

        public DataTable GetTourProduct(long tour_product_id)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[]
                {
                    new SqlParameter("@TourProductId", tour_product_id)
                };

                return dbWorker.GetDataTable(StoreProcedureConstant.SP_GetDetailTourProductByID, objParam);

            }
            catch
            {
                throw;
            }
        }

        public int CreateTourProduct(TourProduct model)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[]
                {
                    new SqlParameter("@TourName", model.TourName),
                    new SqlParameter("@TourType", model.TourType ?? (object) DBNull.Value),
                    new SqlParameter("@OrganizingType", model.OrganizingType ?? (object) DBNull.Value),
                    new SqlParameter("@DateDeparture", model.DateDeparture ?? (object) DBNull.Value),
                    new SqlParameter("@Price", model.Price),
                    new SqlParameter("@StartPoint", model.StartPoint ?? (object) DBNull.Value),
                    new SqlParameter("@ServiceCode", model.ServiceCode ?? String.Empty),
                    new SqlParameter("@SupplierId", model.SupplierId ?? (object) DBNull.Value),
                    new SqlParameter("@Days", model.Days ?? (object) DBNull.Value),
                    new SqlParameter("@OldPrice", model.OldPrice ?? (object) DBNull.Value),
                    new SqlParameter("@Star", model.Star ?? (object) DBNull.Value),
                    new SqlParameter("@Avatar", model.Avatar ?? (object) DBNull.Value),
                    new SqlParameter("@IsDisplayWeb", model.IsDisplayWeb),
                    new SqlParameter("@Image", model.Image ?? (object) DBNull.Value),
                    new SqlParameter("@Schedule", model.Schedule ?? (object) DBNull.Value),
                    new SqlParameter("@AdditionInfo", model.AdditionInfo ?? (object) DBNull.Value),
                    new SqlParameter("@IsDelete", model.IsDelete ?? false),
                    new SqlParameter("@IsSelfDesigned", model.IsSelfDesigned ?? false),
                    new SqlParameter("@Transportation", model.Transportation ?? (object) DBNull.Value),
                    new SqlParameter("@Description", model.Description ?? (object) DBNull.Value),
                    new SqlParameter("@Include", model.Include ?? (object) DBNull.Value),
                    new SqlParameter("@Exclude", model.Exclude ?? (object) DBNull.Value),
                    new SqlParameter("@Refund", model.Refund ?? (object) DBNull.Value),
                    new SqlParameter("@Surcharge", model.Surcharge ?? (object) DBNull.Value),
                    new SqlParameter("@Note", model.Note ?? (object) DBNull.Value),
                    new SqlParameter("@Status", model.Status ?? (object) DBNull.Value),
                    new SqlParameter("@CreatedBy", model.CreatedBy),
                    new SqlParameter("@CreatedDate", model.CreatedDate ?? DateTime.Now)
                };

                return dbWorker.ExecuteNonQuery(StoreProcedureConstant.SP_InsertTourProduct, objParam);
            }
            catch
            {
                throw;
            }
        }

        public int UpdateTourProduct(TourProduct model)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[]
                {
                    new SqlParameter("@Id", model.Id),
                    new SqlParameter("@TourName", model.TourName),
                    new SqlParameter("@TourType", model.TourType ?? (object) DBNull.Value),
                    new SqlParameter("@OrganizingType", model.OrganizingType ?? (object) DBNull.Value),
                    new SqlParameter("@DateDeparture", model.DateDeparture ?? (object) DBNull.Value),
                    new SqlParameter("@Price", model.Price),
                    new SqlParameter("@StartPoint", model.StartPoint ?? (object) DBNull.Value),
                    new SqlParameter("@ServiceCode", model.ServiceCode ?? String.Empty),
                    new SqlParameter("@SupplierId", model.SupplierId ?? (object) DBNull.Value),
                    new SqlParameter("@Days", model.Days ?? (object) DBNull.Value),
                    new SqlParameter("@OldPrice", model.OldPrice ?? (object) DBNull.Value),
                    new SqlParameter("@Star", model.Star ?? (object) DBNull.Value),
                    new SqlParameter("@Avatar", model.Avatar ?? (object) DBNull.Value),
                    new SqlParameter("@IsDisplayWeb", model.IsDisplayWeb),
                    new SqlParameter("@Image", model.Image ?? (object) DBNull.Value),
                    new SqlParameter("@Schedule", model.Schedule ?? (object) DBNull.Value),
                    new SqlParameter("@AdditionInfo", model.AdditionInfo ?? (object) DBNull.Value),
                    new SqlParameter("@IsDelete", model.IsDelete ?? false),
                    new SqlParameter("@IsSelfDesigned", model.IsSelfDesigned ?? false),
                    new SqlParameter("@Transportation", model.Transportation ?? (object) DBNull.Value),
                    new SqlParameter("@Description", model.Description ?? (object) DBNull.Value),
                    new SqlParameter("@Include", model.Include ?? (object) DBNull.Value),
                    new SqlParameter("@Exclude", model.Exclude ?? (object) DBNull.Value),
                    new SqlParameter("@Refund", model.Refund ?? (object) DBNull.Value),
                    new SqlParameter("@Surcharge", model.Surcharge ?? (object) DBNull.Value),
                    new SqlParameter("@Note", model.Note ?? (object) DBNull.Value),
                    new SqlParameter("@Status", model.Status ?? (object) DBNull.Value),
                    new SqlParameter("@UpdatedBy", model.UpdatedBy)
                };

                return dbWorker.ExecuteNonQuery(StoreProcedureConstant.SP_UpdateTourProduct, objParam);
            }
            catch
            {
                throw;
            }
        }

        public void CreateMultipleTourDestination(IEnumerable<TourDestination> models)
        {
            try
            {
                dbWorker.ExecuteActionTransaction((connection, transaction) =>
                {
                    if (models != null && models.Any())
                    {
                        foreach (var model in models)
                        {
                            SqlCommand oCommand = new SqlCommand()
                            {
                                Connection = connection,
                                Transaction = transaction,
                                CommandType = CommandType.StoredProcedure
                            };

                            oCommand.CommandText = StoreProcedureConstant.SP_InsertTourDestination;
                            oCommand.Parameters.AddRange(new SqlParameter[] {
                                new SqlParameter("@TourId", model.TourId),
                                new SqlParameter("@LocationId", model.LocationId),
                                new SqlParameter("@Type", model.Type),
                                new SqlParameter("@CreatedBy", model.CreatedBy),
                                new SqlParameter("@CreatedDate", model.CreatedDate)
                            });

                            SqlParameter OuputParam = oCommand.Parameters.Add("@Identity", SqlDbType.Int);
                            OuputParam.Direction = ParameterDirection.Output;

                            oCommand.ExecuteNonQuery();
                        }
                    }
                });
            }
            catch
            {
                throw;
            }
        }

        public int DeleteTourProduct(int Id)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[]
                {
                    new SqlParameter("@Id", Id),
                };
                dbWorker.ExecuteNonQueryNoIdentity(StoreProcedureConstant.SP_DeleteTourProduct, objParam);

                return Id;
            }
            catch
            {
                throw;
            }
        }

        public int CreateTourDestination(TourDestination model)
        {
            try
            {
                SqlParameter[] objParam_order = new SqlParameter[5];
                objParam_order[0] = new SqlParameter("@TourId", model.TourId);
                objParam_order[1] = new SqlParameter("@LocationId", model.LocationId);
                objParam_order[2] = new SqlParameter("@Type", model.Type);
                objParam_order[3] = new SqlParameter("@CreatedBy", model.CreatedBy);
                objParam_order[4] = new SqlParameter("@CreatedDate", model.CreatedDate);

                var id = dbWorker.ExecuteNonQuery(StoreProcedureConstant.SP_InsertTourDestination, objParam_order);
                model.Id = id;
                return id;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CreateTourProduct - TourDAL. " + ex);
                return -1;
            }
        }
        private int UpdateTourDestination(TourDestination model)
        {
            try
            {
                SqlParameter[] objParam_order = new SqlParameter[5];
                objParam_order[0] = new SqlParameter("@Id", model.Id);
                objParam_order[1] = new SqlParameter("@TourId", model.TourId);
                objParam_order[2] = new SqlParameter("@Type", model.Type);
                objParam_order[3] = new SqlParameter("@LocationId", model.LocationId);
                objParam_order[4] = new SqlParameter("@UpdatedBy", model.UpdatedBy);


                var id = dbWorker.ExecuteNonQuery(StoreProcedureConstant.SP_UpdateTourDestination, objParam_order);
                model.Id = id;
                return id;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CreateTourProduct - TourDAL. " + ex);
                return -1;
            }
        }

        public void DeleteTourDestination(int TourId, int type)
        {
            try
            {
                SqlParameter[] objParam_order = new SqlParameter[]
                {
                    new SqlParameter("@TourId", TourId),
                    new SqlParameter("@Type", type)
                };
                dbWorker.ExecuteNonQueryNoIdentity(StoreProcedureConstant.SP_DeleteTourDestination, objParam_order);
            }
            catch
            {
                throw;
            }
        }
        #region Tour_Position
        public int InsertTourPosition(TourPosition model)
        {
            try
            {
                SqlParameter[] objParam_order = new SqlParameter[]
                {
                  new SqlParameter("@TourId", model.TourId),
                new SqlParameter("@PositionType", model.PositionType),
                    new SqlParameter("@Position", model.Position ),
                    new SqlParameter("@Status", model.Status),
                    new SqlParameter("@CreatedBy", model.CreatedBy ?? (object)DBNull.Value),
                };
                return dbWorker.ExecuteNonQuery(StoreProcedureConstant.sp_InsertTourPosition, objParam_order);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("InsertTourPosition - HotelDAL. " + ex);
                throw;
            }
        }
        public int UpdateTourPosition(TourPosition model)
        {
            try
            {
                SqlParameter[] objParam_order = new SqlParameter[]
                {
                   new SqlParameter("@Id", model.Id),
                   new SqlParameter("@TourId", model.TourId),
                    new SqlParameter("@PositionType", model.PositionType),
                    new SqlParameter("@Position", model.Position ),
                    new SqlParameter("@Status", model.Status),
                    new SqlParameter("@UpdatedBy", DBNull.Value),
                };
                return dbWorker.ExecuteNonQuery(StoreProcedureConstant.sp_UpdateTourPosition, objParam_order);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateTourPosition - HotelDAL. " + ex);
                throw;
            }
        }
        public List<TourPosition> GetListTourPosition(long TourId)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var model = _DbContext.TourPosition.Where(s => s.TourId == TourId).ToList();

                    return model;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetListTourPosition - HotelDAL. " + ex);
                throw;
            }
        } 
        public List<TourPosition> GetDetailTourPositionByPosition(long Position, long PositionType)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var model = _DbContext.TourPosition.Where(s => s.Position == Position && s.Status== PositionStatus.HD && s.PositionType== PositionType).ToList();

                    return model;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetDetailTourPositionByPosition - HotelDAL. " + ex);
                throw;
            }
        }
        #endregion
    }
}