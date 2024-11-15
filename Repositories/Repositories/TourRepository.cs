using Aspose.Cells;
using DAL;
using Entities.ConfigModels;
using Entities.Models;
using Entities.ViewModels;
using Entities.ViewModels.OrderManual;
using Entities.ViewModels.Tour;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Repositories.IRepositories;
using Repositories.Repositories.BaseRepos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace Repositories.Repositories
{
    public class TourRepository : BaseRepository, ITourRepository
    {
        private readonly TourDAL tourDAL;
        private readonly TourPackagesDAL tourPackagesDAL;
        private readonly TourGuestDAL tourGuestDAL;
        private readonly TourProductDAL tourProductDAL;
        private readonly TourDestinationDAL tourDestinationDAL;
        private readonly TourPackagesOptionalDAL _tourPackagesOptionalDAL;
        private readonly OrderDAL _orderDAL;
        private readonly ClientDAL _clientDAL;
        private readonly CommonDAL _CommonDAL;
        private readonly string _UrlStaticImage;

        public TourRepository(IHttpContextAccessor context, IOptions<DataBaseConfig> dataBaseConfig, IOptions<DomainConfig> domainConfig, IUserRepository userRepository, IConfiguration configuration) : base(context, dataBaseConfig, configuration, userRepository)
        {
            tourDAL = new TourDAL(_SqlServerConnectString);
            tourPackagesDAL = new TourPackagesDAL(_SqlServerConnectString);
            tourGuestDAL = new TourGuestDAL(_SqlServerConnectString);
            tourProductDAL = new TourProductDAL(_SqlServerConnectString);
            tourDestinationDAL = new TourDestinationDAL(_SqlServerConnectString);
            _tourPackagesOptionalDAL = new TourPackagesOptionalDAL(_SqlServerConnectString);
            _orderDAL = new OrderDAL(_SqlServerConnectString);
            _clientDAL = new ClientDAL(_SqlServerConnectString);
            _CommonDAL = new CommonDAL(_SqlServerConnectString);
            _UrlStaticImage = domainConfig.Value.ImageStatic;
        }

        //public TourRepository(IOptions<DataBaseConfig> dataBaseConfig, IOptions<DomainConfig> domainConfig)
        //{
        //    tourDAL = new TourDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
        //    tourPackagesDAL = new TourPackagesDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
        //    tourGuestDAL = new TourGuestDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
        //    tourProductDAL = new TourProductDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
        //    tourDestinationDAL = new TourDestinationDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
        //    _tourPackagesOptionalDAL = new TourPackagesOptionalDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
        //    _orderDAL = new OrderDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
        //    _clientDAL = new ClientDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
        //    _CommonDAL = new CommonDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
        //    _UrlStaticImage = domainConfig.Value.ImageStatic;
        //}


        public async Task<Tour> GetTourById(long tour_id)
        {
            return await tourDAL.GetTourById(tour_id);
        }
        public async Task<List<TourPackages>> GetTourPackagesByTourId(long tour_id)
        {
            return await tourPackagesDAL.GetTourPackagesByTourId(tour_id);
        }
        public async Task<List<TourGuests>> GetTourGuestsByTourId(long tour_id)
        {
            return await tourGuestDAL.GetTourGuestByTourId(tour_id);
        }

        public async Task<long> SummitTourServiceData(OrderManualTourBookingServiceSummitModel data, int user_id, int is_client_debt)
        {

            try
            {
                var order = _orderDAL.GetByOrderId(data.order_id);
                if (order == null || order.OrderId <= 0) return -1;
                var client = await _clientDAL.GetClientDetail((long)order.ClientId);
                OrderManualTourSQLServiceSummitModel summit_model = new OrderManualTourSQLServiceSummitModel()
                {
                    extra_packages = new List<TourPackages>(),
                    guest = new List<TourGuests>(),
                    detail = new Tour()
                };
                double amount = 0;
                double profit = 0;
                double price = 0;
                if (data.extra_packages.Count > 0)
                {
                    foreach (var p in data.extra_packages)
                    {
                        amount += p.amount;
                        profit += p.profit;
                        price += p.price;
                        summit_model.extra_packages.Add(new TourPackages()
                        {
                            Amount = p.amount,
                            AmountBeforeVat = p.amount,
                            AmountVat = 0,
                            BasePrice = p.base_price,
                            CreatedBy = data.user_summit,
                            CreatedDate = DateTime.Now,
                            Id = p.id,
                            PackageCode = p.package_id ?? "",
                            PackageName = p.package_code ?? "",
                            Quantity = p.quantity,
                            TourId = data.tour_id,
                            UpdatedBy = data.user_summit,
                            UpdatedDate = DateTime.Now,
                            Vat = 0,
                            UnitPrice = p.price,
                            Profit = p.profit,

                        });
                    }
                }
                int status = 0;
                var exists_tour = await tourDAL.GetTourById(data.tour_id);
                if (exists_tour != null && exists_tour.Status != null)
                {
                    status = (int)exists_tour.Status;
                    /*
                    if ((status == (int)ServiceStatus.Decline && is_client_debt == (int)DebtType.DEBT_ACCEPTED) || (status == (int)ServiceStatus.Decline && client.ClientType == (int)ClientType.kl))
                    {
                        status = (int)ServiceStatus.WaitingExcution;
                    }*/
                }
                TourProduct tour_product = new TourProduct()
                {
                    Id = 0,
                    AdditionInfo = "",
                    Avatar = "",
                    CreatedBy = user_id,
                    CreatedDate = DateTime.Now,
                    DateDeparture = data.start_date.ToString(),
                    Days = (data.end_date - data.start_date).Days > 0 ? (data.end_date - data.start_date).Days : 1,
                    Image = "",
                    IsDelete = false,
                    IsDisplayWeb = true,
                    OldPrice = amount,
                    OrganizingType = data.organizing_type,
                    Price = amount,
                    Schedule = "",
                    ServiceCode = "",
                    Star = 5,
                    StartPoint = data.start_point,
                    Status = 0,
                    SupplierId = 0,
                    TourName = data.tour_product_name,
                    TourType = data.tour_type,
                    UpdatedBy = data.user_summit,
                    UpdatedDate = DateTime.Now,
                    IsSelfDesigned = data.is_self_designed > 0 ? true : false
                };
                if (data.tour_product_id > 0)
                {
                    tour_product = await tourProductDAL.GetTourProductById(data.tour_product_id);
                    if (data.is_self_designed <= 0)
                    {
                        tour_product.Price = amount;
                        tour_product.StartPoint = data.start_point;

                        tour_product.TourName = data.tour_product_name;
                        tour_product.TourType = data.tour_type;
                        tour_product.OrganizingType = data.organizing_type;
                        tour_product.DateDeparture = data.start_date.ToString();
                        tour_product.Days = (data.end_date - data.start_date).Days > 0 ? (data.end_date - data.start_date).Days : 1;
                    }
                }
                tour_product.UpdatedBy = data.user_summit;
                tour_product.UpdatedDate = DateTime.Now;
                summit_model.product = tour_product;
                if (data.end_point != null && data.end_point.Trim() != "" && data.is_self_designed == 1)
                {
                    var list_destination = data.end_point.Split(",");
                    summit_model.destinations = new List<TourDestination>();
                    foreach (var dest in list_destination)
                    {
                        try
                        {
                            summit_model.destinations.Add(new TourDestination()
                            {
                                CreatedBy = user_id,
                                CreatedDate = DateTime.Now,
                                UpdatedBy = user_id,
                                UpdatedDate = DateTime.Now,
                                Id = 0,
                                LocationId = Convert.ToInt32(dest),
                                TourId = summit_model.product.Id,
                                Type = data.tour_type
                            });
                        }
                        catch { }
                    }
                }
                summit_model.detail = new Tour()
                {
                    Amount = amount,
                    CreatedBy = data.user_summit,
                    CreatedDate = DateTime.Now,
                    UpdatedBy = data.user_summit,
                    UpdatedDate = DateTime.Now,
                    EndDate = data.end_date,
                    OrderId = data.order_id,
                    Id = data.tour_id,
                    OrganizingType = data.organizing_type,
                    SalerId = data.main_staff,
                    ServiceCode = data.service_code,
                    StartDate = data.start_date,
                    TourType = data.tour_type,
                    AdditionInfo = "",
                    Avatar = "",
                    TourProductId = data.tour_product_id,
                    SupplierId = tour_product.SupplierId,
                    Price = price,
                    Status = status,
                    Profit = profit - data.commission - data.other_amount,
                    Days = (data.end_date - data.start_date).Days > 0 ? (data.end_date - data.start_date).Days : 1,
                    Image = "",
                    IsDisplayWeb = false,
                    Schedule = "",
                    Star = 5,
                    Note = data.note == null ? "" : data.note,
                    Commission = data.commission,
                    OthersAmount = data.other_amount,
                    FundCustomerCare = data.fundCustomerCare,

                };
                if (data.tour_id > 0)
                {
                    var package_optional = await _tourPackagesOptionalDAL.GetTourOptionalByTourId(data.tour_id);
                    if (package_optional != null && package_optional.Count >= 0)
                    {
                        summit_model.detail.Price = package_optional.Sum(x => x.Amount);

                    }
                }
                if (data.guest != null && data.guest.Count > 0)
                {
                    foreach (var g in data.guest)
                    {
                        summit_model.guest.Add(new TourGuests()
                        {
                            Birthday = g.birthday,
                            CreatedBy = data.user_summit,
                            CreatedDate = DateTime.Now,
                            UpdatedBy = data.user_summit,
                            UpdatedDate = DateTime.Now,
                            FullName = g.name ?? "",
                            Note = g.note ?? "",
                            Phone = "",
                            TourId = summit_model.detail.OrderId,
                            Id = g.id,
                            Cccd = g.cccd ?? "",
                            Gender = g.gender,
                            RoomNumber = g.room_number == null ? "" : g.room_number.ToString()

                        });
                    }

                }
                var id = await tourDAL.SummitTourData(summit_model);
                // await _tourPackagesOptionalDAL.InsertListTourPackagesOptional(summit_model.extra_packages);
                return id;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SummitTourServiceData - TourRepository: " + ex.ToString());
                return -2;
            }
        }
        public async Task<List<TourViewModel>> GetTourByOrderId(long OrderId)
        {
            try
            {
                DataTable dt = await tourDAL.GetTourByOrderId(OrderId);
                if (dt != null && dt.Rows.Count > 0)
                {
                    var data = dt.ToList<TourViewModel>();
                    return data;
                }
                return null;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetTourByOrderId - TourRepository: " + ex.ToString());
                return null;
            }

        }
        public async Task<TourViewModel> GetDetailTourByID(long TourId)
        {
            try
            {
                DataTable dt = await tourDAL.GetDetailTourByID(TourId);
                if (dt != null && dt.Rows.Count > 0)
                {
                    var data = dt.ToList<TourViewModel>();
                    data[0].TotalAmountPaymentRequest = data.Sum(s => s.TotalAmountPaymentRequest);
                    return data[0];
                }
                return null;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetDetailTourByID - TourRepository: " + ex.ToString());
                return null;
            }

        }
        public async Task<GenericViewModel<TourGetListViewModel>> GetListTour(TourSearchViewModel Tourmodel)
        {
            var model = new GenericViewModel<TourGetListViewModel>();
            try
            {
                DataTable dt = await tourDAL.GetListTour(Tourmodel);
                if (dt != null && dt.Rows.Count > 0)
                {
                    var data = dt.ToList<TourGetListViewModel>();


                    model.ListData = data;
                    model.CurrentPage = Tourmodel.PageIndex;
                    model.PageSize = Tourmodel.PageSize;
                    model.TotalRecord = Convert.ToInt32(dt.Rows[0]["TotalRow"]);
                    model.TotalPage = (int)Math.Ceiling((double)model.TotalRecord / model.PageSize);
                    return model;
                }
                return null;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetDetailTourByID - TourRepository: " + ex.ToString());
                return null;
            }

        }
        public async Task<List<TourPackages>> ListTourPackagesByTourId(long TourId)
        {
            try
            {
                DataTable dt = await tourPackagesDAL.ListTourPackagesByTourId(TourId);
                if (dt != null && dt.Rows.Count > 0)
                {
                    var data = dt.ToList<TourPackages>();
                    return data;
                }
                return null;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetTourPackagesByTourId - TourRepository: " + ex.ToString());
                return null;
            }

        }
        public async Task<List<TourGuests>> GetListTourGuestsByTourId(long TourId)
        {
            try
            {
                DataTable dt = await tourGuestDAL.GetListTourGuestsByTourId(TourId);
                if (dt != null && dt.Rows.Count > 0)
                {
                    var data = dt.ToList<TourGuests>();
                    return data;
                }
                return null;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetListTourGuestsByTourId - TourRepository: " + ex.ToString());
                return null;
            }
        }
        public async Task<int> UpdateTourStatus(long TourId, int Status)
        {
            try
            {

                return await tourDAL.UpdateTourStatus(TourId, Status);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetListTourGuestsByTourId - TourRepository: " + ex.ToString());
                return 0;
            }

        }
        public async Task<int> UpdateTourPackages(List<TourPackages> model)
        {
            try
            {
                foreach (var item in model)
                {
                    var update = tourPackagesDAL.UpdateTourPackagesUnitPrice(item);
                }
                return 0;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateTourPackages - TourRepository. " + ex);
                return -1;
            }
        }
        public async Task<int> UpdateTourTotalPrice(long tour_id)
        {
            try
            {
                var tour = await tourDAL.GetTourById(tour_id);
                var list_data = await _tourPackagesOptionalDAL.GetTourPackagesOptiona(tour_id);
                var list_tour = list_data.ToList<TourPackagesOptionalViewModel>();
                if (list_tour != null && tour != null && tour.Id > 0 && list_tour.Count > 0)
                {
                    double price = (double)list_tour.Where(s => s.Status != 1).Sum(x => x.Amount);
                    tour.Price = price;
                    tourDAL.UpdateTour(tour);
                }
                else
                {
                    tour.Price = 0;
                    tourDAL.UpdateTour(tour);
                }
                return 0;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateTourTotalPrice - TourRepository. " + ex);
                return -1;
            }
        }
        public async Task<List<Tour>> GetAllTour()
        {
            try
            {
                return await tourDAL.GetAllTour();
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetAllTour - TourRepository: " + ex);
                return null;
            }
        }
        public async Task<List<CountTourviewModel>> CountTourByStatus(TourSearchViewModel model)
        {
            try
            {
                DataTable dt = await tourDAL.CountTourByStatus(model);
                if (dt != null && dt.Rows.Count > 0)
                {
                    var data = dt.ToList<CountTourviewModel>();
                    return data;
                }
                return null;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetAllTour - TourRepository: " + ex);
                return null;
            }
        }
        public async Task<int> UpdateTour(Tour model)
        {
            try
            {
                return tourDAL.UpdateTour(model);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateTourStatus - TourDAL: " + ex);
                return 0;
            }
        }
        public async Task<List<TourProductViewModel>> TourProductSuggesstion(string txt_search)
        {
            try
            {
                DataTable dt = await tourDAL.TourProductSuggesstion(txt_search);
                if (dt != null && dt.Rows.Count > 0)
                {
                    var data = dt.ToList<TourProductViewModel>();
                    return data;
                }
                return null;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetListTourProduct - TourRepository: " + ex);
                return null;
            }
        }
        public async Task<TourProduct> GetTourProductById(long id)
        {
            try
            {
                return await tourProductDAL.GetTourProductById(id);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetTourProductById - TourRepository: " + ex);
                return null;
            }
        }
        public async Task<List<TourDestination>> GetTourDestinationByTourProductId(long tour_id)
        {
            try
            {
                return await tourDestinationDAL.GetByTourId(tour_id);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetTourProductById - TourRepository: " + ex);
                return null;
            }
        }
        public int UpdateTourGuest(TourGuests model)
        {
            try
            {
                var update = tourGuestDAL.UpdateTourGuest(model);
                return update;

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateTourPackages - TourRepository. " + ex.ToString());
                return -1;
            }
        }
        public async Task<long> DeleteTourByID(long id)
        {
            try
            {
                return await tourDAL.DeleteTourByID(id);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("DeleteFlyBookingByID - TourRepository: " + ex);

            }
            return 0;
        }
        public async Task<long> CancelTourByID(long id, int user_id)
        {
            try
            {
                return await tourDAL.CancelTourByID(id, user_id);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CancelTourByID - TourRepository: " + ex);

            }
            return 0;
        }

        public IEnumerable<TourProductGridModel> GetPagingTourProduct(TourProductSearchModel searchModel)
        {
            try
            {
                return tourDAL.GetListTourProduct(searchModel).ToList<TourProductGridModel>();
            }
            catch
            {
                throw;
            }
        }
        public async Task<long> UpsertTourProduct(TourProductUpsertModel model)
        {
            try
            {
                var tour_id = 0;
                if (!string.IsNullOrEmpty(model.Avatar))
                    model.Avatar = UpLoadHelper.UploadBase64Src(model.Avatar, _UrlStaticImage).Result;

                if (model.Id > 0)
                {
                    model.UpdatedBy = _SysUserModel.Id;
                    model.UpdatedDate = DateTime.Now;
                    tour_id = tourDAL.UpdateTourProduct(model);
                    if (model.IsDisplayWeb == true)
                    {


                        var modelPosition = new TourPosition();
                        modelPosition.TourId = (int?)model.Id;


                        modelPosition.CreatedBy = model.UpdatedBy;
                        modelPosition.UpdatedBy = model.UpdatedBy;
                        modelPosition.Status = 1;
                        var list = tourDAL.GetListTourPosition(model.Id);
                        if (list != null && list.Count > 0)
                        {
                            if (model.PositionB2B > 0)
                            {
                                var ListTourPosition = tourDAL.GetDetailTourPositionByPosition(model.PositionB2C, PositionType.B2C);

                                if (ListTourPosition != null && ListTourPosition.Count > 0)
                                {
                                    foreach (var item in ListTourPosition)
                                    {
                                        var DetailTour = await GetTourProductById((long)item.TourId);
                                        if (model.TourType == DetailTour.TourType)
                                        {
                                            item.Status = PositionStatus.khoa;
                                            tourDAL.UpdateTourPosition(item);
                                        }
                                    }

                                }
                                var list_B2B = list.Where(s => s.PositionType == (short?)PositionType.B2B).ToList();
                                modelPosition.Position = model.PositionB2B;
                                modelPosition.PositionType = (short?)PositionType.B2B;
                                if (list_B2B != null && list_B2B.Count > 0)
                                {
                                    modelPosition.Id = list_B2B[0].Id;
                                    tourDAL.UpdateTourPosition(modelPosition);
                                }
                                else
                                {
                                    tourDAL.InsertTourPosition(modelPosition);
                                }
                            }

                            if (model.PositionB2C > 0)
                            {
                                var ListTourPosition = tourDAL.GetDetailTourPositionByPosition(model.PositionB2C, PositionType.B2C);

                                if (ListTourPosition != null && ListTourPosition.Count > 0)
                                {
                                    foreach (var item in ListTourPosition)
                                    {
                                        var DetailTour = await GetTourProductById((long)item.TourId);
                                        if (model.TourType == DetailTour.TourType)
                                        {
                                            item.Status = PositionStatus.khoa;
                                            tourDAL.UpdateTourPosition(item);
                                        }
                                    }

                                }
                                var list_B2C = list.Where(s => s.PositionType == (short?)PositionType.B2C).ToList();
                                modelPosition.Position = model.PositionB2C;
                                modelPosition.PositionType = (short?)PositionType.B2C;
                                if (list_B2C != null && list_B2C.Count > 0)
                                {
                                    list_B2C = list_B2C.Where(s => s.PositionType == PositionType.B2C).ToList();
                                    modelPosition.Id = list_B2C[0].Id;
                                    tourDAL.UpdateTourPosition(modelPosition);
                                }
                                else
                                {
                                    tourDAL.InsertTourPosition(modelPosition);
                                }

                            }
                        }
                        else
                        {
                            if (model.PositionB2B > 0)
                            {
                                var ListTourPosition = tourDAL.GetDetailTourPositionByPosition(model.PositionB2C, PositionType.B2C);

                                if (ListTourPosition != null && ListTourPosition.Count > 0)
                                {
                                    foreach (var item in ListTourPosition)
                                    {
                                        var DetailTour = await GetTourProductById((long)item.TourId);
                                        if (model.TourType == DetailTour.TourType)
                                        {
                                            item.Status = PositionStatus.khoa;
                                            tourDAL.UpdateTourPosition(item);
                                        }
                                    }

                                }
                                modelPosition.Position = model.PositionB2B;
                                modelPosition.PositionType = (short?)PositionType.B2B;
                                tourDAL.InsertTourPosition(modelPosition);
                            }
                            if (model.PositionB2C > 0)
                            {
                                var ListTourPosition = tourDAL.GetDetailTourPositionByPosition(model.PositionB2C, PositionType.B2C);

                                if (ListTourPosition != null && ListTourPosition.Count > 0)
                                {
                                    foreach (var item in ListTourPosition)
                                    {
                                        var DetailTour = await GetTourProductById((long)item.TourId);
                                        if (model.TourType == DetailTour.TourType)
                                        {
                                            item.Status = PositionStatus.khoa;
                                            tourDAL.UpdateTourPosition(item);
                                        }
                                    }

                                }
                                modelPosition.Position = model.PositionB2C;
                                modelPosition.PositionType = (short?)PositionType.B2C;
                                tourDAL.InsertTourPosition(modelPosition);
                            }

                        }
                    }
                }
                else
                {
                    model.CreatedBy = _SysUserModel.Id;
                    model.UpdatedBy = _SysUserModel.Id;
                    model.UpdatedDate = DateTime.Now;
                    tour_id = tourDAL.CreateTourProduct(model);
                    if (model.IsDisplayWeb == true)
                    {


                        var modelPosition = new TourPosition();
                        modelPosition.TourId = tour_id;

                        modelPosition.CreatedBy = model.CreatedBy;
                        modelPosition.Status = 1;
                        if (model.PositionB2B > 0)
                        {
                            var ListTourPosition = tourDAL.GetDetailTourPositionByPosition(model.PositionB2C, PositionType.B2C);

                            if (ListTourPosition != null && ListTourPosition.Count > 0)
                            {
                                foreach (var item in ListTourPosition)
                                {
                                    var DetailTour = await GetTourProductById((long)item.TourId);
                                    if (model.TourType == DetailTour.TourType)
                                    {
                                        item.Status = PositionStatus.khoa;
                                        tourDAL.UpdateTourPosition(item);
                                    }
                                }

                            }
                            modelPosition.Position = model.PositionB2B;
                            modelPosition.PositionType = (short?)PositionType.B2B;
                            tourDAL.InsertTourPosition(modelPosition);
                        }
                        if (model.PositionB2C > 0)
                        {
                            var ListTourPosition = tourDAL.GetDetailTourPositionByPosition(model.PositionB2C, PositionType.B2C);

                            if (ListTourPosition != null && ListTourPosition.Count > 0)
                            {
                                foreach (var item in ListTourPosition)
                                {
                                    var DetailTour = await GetTourProductById((long)item.TourId);
                                    if (model.TourType == DetailTour.TourType)
                                    {
                                        item.Status = PositionStatus.khoa;
                                        tourDAL.UpdateTourPosition(item);
                                    }
                                }

                            }
                            modelPosition.Position = model.PositionB2C;
                            modelPosition.PositionType = (short?)PositionType.B2B;
                            tourDAL.InsertTourPosition(modelPosition);
                        }
                    }
                }

                // Delete all image
                _CommonDAL.DeleteAttachFilesByDataId(tour_id, (int)AttachmentType.TOUR_PRODUCT);

                // Add image again
                if (model.OtherImages != null && model.OtherImages.Count() > 0)
                {
                    var AttachFiles = new List<AttachFile>();
                    foreach (var image in model.OtherImages)
                    {
                        var str_path = UpLoadHelper.UploadBase64Src(image, _UrlStaticImage).Result;
                        if (!string.IsNullOrEmpty(str_path))
                        {
                            AttachFiles.Add(new AttachFile
                            {
                                DataId = tour_id,
                                Path = str_path,
                                Type = (int)AttachmentType.TOUR_PRODUCT,
                                UserId = _SysUserModel.Id
                            });
                        }
                    }
                    _CommonDAL.InsertAttachFiles(AttachFiles);
                }

                if (model.TourType != null)
                {
                    // DeleteTourDestination
                    tourDAL.DeleteTourDestination(tour_id, model.TourType.Value);

                    // AddTourDestination
                    if (model.EndPoints != null && model.EndPoints.Count() > 0)
                    {
                        var TourDestinations = new List<TourDestination>();
                        foreach (var location in model.EndPoints)
                        {
                            TourDestinations.Add(new TourDestination
                            {
                                TourId = tour_id,
                                LocationId = location,
                                Type = model.TourType,
                                CreatedBy = _SysUserModel.Id,
                                CreatedDate = DateTime.Now
                            });
                        }

                        tourDAL.CreateMultipleTourDestination(TourDestinations);
                    }
                }

                return tour_id;
            }
            catch
            {
                throw;
            }
        }
        public async Task<string> ExportDeposit(TourSearchViewModel searchModel, string FilePath)
        {
            var pathResult = string.Empty;
            try
            {
                var data = new List<TourGetListViewModel>();

                DataTable dt = await tourDAL.GetListTour(searchModel);
                if (dt != null && dt.Rows.Count > 0)
                {
                    data = dt.ToList<TourGetListViewModel>();

                }
                if (data != null && data.Count > 0)
                {
                    Workbook wb = new Workbook();
                    Worksheet ws = wb.Worksheets[0];
                    ws.Name = "Danh sách đặt dịch vụ tour";
                    Cells cell = ws.Cells;

                    var range = ws.Cells.CreateRange(0, 0, 1, 1);
                    StyleFlag st = new StyleFlag();
                    st.All = true;
                    Style style = ws.Cells["A1"].GetStyle();

                    #region Header
                    range = cell.CreateRange(0, 0, 1, 14);
                    style = ws.Cells["A1"].GetStyle();
                    style.Font.IsBold = true;
                    style.IsTextWrapped = true;
                    style.ForegroundColor = Color.FromArgb(33, 88, 103);
                    style.BackgroundColor = Color.FromArgb(33, 88, 103);
                    style.Pattern = BackgroundType.Solid;
                    style.Font.Color = Color.White;
                    style.VerticalAlignment = TextAlignmentType.Center;
                    style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                    style.Borders[BorderType.TopBorder].Color = Color.Black;
                    style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
                    style.Borders[BorderType.BottomBorder].Color = Color.Black;
                    style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                    style.Borders[BorderType.LeftBorder].Color = Color.Black;
                    style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                    style.Borders[BorderType.RightBorder].Color = Color.Black;
                    range.ApplyStyle(style, st);

                    // Set column width
                    cell.SetColumnWidth(0, 8);
                    cell.SetColumnWidth(1, 20);
                    cell.SetColumnWidth(2, 40);
                    cell.SetColumnWidth(3, 20);
                    cell.SetColumnWidth(4, 20);
                    cell.SetColumnWidth(5, 30);
                    cell.SetColumnWidth(6, 30);
                    cell.SetColumnWidth(7, 25);
                    cell.SetColumnWidth(8, 25);
                    cell.SetColumnWidth(9, 25);
                    cell.SetColumnWidth(10, 25);
                    cell.SetColumnWidth(11, 25);
                    cell.SetColumnWidth(12, 25);
                    cell.SetColumnWidth(13, 25);


                    // Set header value
                    ws.Cells["A1"].PutValue("STT");
                    ws.Cells["B1"].PutValue("Mã dịch vụ");
                    ws.Cells["C1"].PutValue("Chi tiết dịch vụ");
                    ws.Cells["D1"].PutValue("Ngày check in");
                    ws.Cells["E1"].PutValue("Ngày check out");
                    ws.Cells["F1"].PutValue("Mã đơn hàng");
                    ws.Cells["G1"].PutValue("Ngày tạo");
                    ws.Cells["H1"].PutValue("Nhân viên bán");
                    ws.Cells["I1"].PutValue("Điều hành");
                    ws.Cells["J1"].PutValue("Mã code");
                    ws.Cells["K1"].PutValue("Trạng thái");
                    ws.Cells["L1"].PutValue("Doanh thu dịch vụ");
                    ws.Cells["M1"].PutValue("Giá NET thực tế");
                    ws.Cells["N1"].PutValue("Lợi nhuận thực tế");
                    #endregion

                    #region Body

                    range = cell.CreateRange(1, 0, data.Count, 14);
                    style = ws.Cells["A2"].GetStyle();
                    style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                    style.Borders[BorderType.TopBorder].Color = Color.Black;
                    style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
                    style.Borders[BorderType.BottomBorder].Color = Color.Black;
                    style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                    style.Borders[BorderType.LeftBorder].Color = Color.Black;
                    style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                    style.Borders[BorderType.RightBorder].Color = Color.Black;
                    style.VerticalAlignment = TextAlignmentType.Center;
                    range.ApplyStyle(style, st);

                    Style alignCenterStyle = ws.Cells["A2"].GetStyle();
                    alignCenterStyle.HorizontalAlignment = TextAlignmentType.Center;

                    Style numberStyle = ws.Cells["I2"].GetStyle();
                    numberStyle.Number = 3;
                    numberStyle.HorizontalAlignment = TextAlignmentType.Right;
                    numberStyle.VerticalAlignment = TextAlignmentType.Center;

                    int RowIndex = 1;

                    foreach (var item in data)
                    {
                        RowIndex++;
                        ws.Cells["A" + RowIndex].PutValue(RowIndex - 1);
                        ws.Cells["A" + RowIndex].SetStyle(alignCenterStyle);
                        ws.Cells["B" + RowIndex].PutValue(item.ServiceCode);
                        ws.Cells["C" + RowIndex].PutValue("Tour " + item.TourTypeName + ":" + item.StartPoint1 + item.StartPoint2 + item.StartPoint3 + " - " + item.GroupEndPoint1 + item.GroupEndPoint2 + item.GroupEndPoint3);
                        ws.Cells["D" + RowIndex].PutValue(item.StartDate.ToString("dd/MM/yyyy"));
                        ws.Cells["E" + RowIndex].PutValue(item.EndDate.ToString("dd/MM/yyyy"));
                        ws.Cells["F" + RowIndex].PutValue(item.OrderNo);
                        ws.Cells["G" + RowIndex].PutValue(item.CreatedDate.ToString("dd/MM/yyyy"));
                        ws.Cells["H" + RowIndex].PutValue(item.OperatorName);
                        ws.Cells["I" + RowIndex].PutValue(item.SalerName);
                        ws.Cells["J" + RowIndex].PutValue(item.BookingCode);
                        ws.Cells["K" + RowIndex].PutValue(item.StatusName);
                        ws.Cells["L" + RowIndex].PutValue((item.Amount == null ? 0 : (double)item.Amount));
                        ws.Cells["L" + RowIndex].SetStyle(numberStyle);
                        ws.Cells["M" + RowIndex].PutValue((item.Price == null ? 0 : (double)item.Price));
                        ws.Cells["M" + RowIndex].SetStyle(numberStyle);
                        ws.Cells["N" + RowIndex].PutValue(((item.Amount == null ? 0 : (double)item.Amount) - (item.Price == null ? 0 : (double)item.Price)));
                        ws.Cells["N" + RowIndex].SetStyle(numberStyle);
                    }
                    ws.Cells.InsertColumn(5);
                    ws.Cells.CopyColumn(ws.Cells, ws.Cells.Columns[12].Index, ws.Cells.Columns[5].Index);
                    ws.Cells.DeleteColumn(12);
                    ws.Cells.InsertColumn(6);
                    ws.Cells.CopyColumn(ws.Cells, ws.Cells.Columns[13].Index, ws.Cells.Columns[6].Index);
                    ws.Cells.DeleteColumn(13);
                    ws.Cells.InsertColumn(7);
                    ws.Cells.CopyColumn(ws.Cells, ws.Cells.Columns[14].Index, ws.Cells.Columns[7].Index);
                    ws.Cells.DeleteColumn(14);
                    #endregion
                    wb.Save(FilePath);
                    pathResult = FilePath;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ExportPaymentRequest - tourRepositories: " + ex);
            }
            return pathResult;
        }

        public TourProductUpsertModel GetTourProductDetail(long tourProductId)
        {
            try
            {
                var dataTable = tourDAL.GetTourProduct(tourProductId);
                return null;
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
                return tourDAL.DeleteTourProduct(Id);
            }
            catch
            {
                throw;
            }
        }
        public async Task<List<TourProgramPackages>> GetListTourProgramPackagesByTourProductId(long tour_product_id)
        {
            try
            {
                DataTable dt = tourProductDAL.GetListTourProgramPackagesByTourProductId(tour_product_id);
                if (dt != null && dt.Rows.Count > 0)
                {
                    var data = dt.ToList<TourProgramPackages>();
                    return data;
                }
                return null;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetListTourProgramPackagesByTourProductId - TourRepository: " + ex);
                return null;
            }
        }
        public async Task<bool> UpsertTourProductPrices(long tour_product_id, List<TourProgramPackages> model, int user_id)
        {
            try
            {
                List<long> product_id_keep = new List<long>();
                if (model != null && model.Count > 0)
                {
                    double min_price = (double)model.Where(x => x.AdultPrice != null).OrderBy(x => (double)x.AdultPrice).First().AdultPrice;
                    var tour = await tourDAL.GetTourById(tour_product_id);
                    if (tour != null && tour.Id > 0)
                    {
                        tour.Amount = min_price;
                        tourDAL.UpdateTour(tour);
                    }
                    foreach (var item in model)
                    {
                        if (item.Id <= 0)
                        {
                            item.CreatedBy = user_id;
                            item.CreatedDate = DateTime.Now;
                            item.UpdatedBy = user_id;
                            item.UpdatedDate = DateTime.Now;
                            tourProductDAL.CreateTourPackages(item);
                            product_id_keep.Add(item.Id);
                        }
                        else
                        {
                            item.UpdatedBy = user_id;
                            item.UpdatedDate = DateTime.Now;
                            tourProductDAL.UpdateTourPackages(item);
                            product_id_keep.Add(item.Id);

                        }
                    }
                }
                //var list = await GetListTourProgramPackagesByTourProductId(tour_product_id);
                //if(list!=null && list.Count > 0)
                //{
                //    foreach(var item in list)
                //    {
                //        if (!product_id_keep.Contains(item.Id))
                //        {
                //             tourProductDAL.DeleteTourProgramPackages(item.Id, user_id);
                //        }
                //    }
                //}
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpsertTourProductPrices - TourRepository: [" + tour_product_id + "] [" + JsonConvert.SerializeObject(model) + "]" + ex);
            }
            return false;

        }
        public long DeleteTourProgramPackages(long id, int user_id)
        {
            return tourProductDAL.DeleteTourProgramPackages(id, user_id);
        }
        public async Task<List<TourPosition>> GetListTourPosition(long id)
        {
            try
            {
                return tourDAL.GetListTourPosition(id);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetListTourPosition - TourRepository: " + ex);
                return null;
            }
        }
    }
}
