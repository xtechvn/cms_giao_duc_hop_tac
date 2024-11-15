using Aspose.Cells;
using DAL;
using DAL.StoreProcedure;
using Entities.ConfigModels;
using Entities.Models;
using Entities.ViewModels;
using Entities.ViewModels.HotelBookingRoom;
using Entities.ViewModels.OrderManual;
using Entities.ViewModels.SetServices;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Repositories.IRepositories;
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
    public class FlyBookingDetailRepository : IFlyBookingDetailRepository
    {
        private readonly FlyBookingDetailDAL flyBookingDetailDAL;
        private readonly AirPortCodeDAL airPortCodeDAL;
        private readonly AirLineDAL airLineDAL;
        private readonly ClientDAL clientDAL;
        private readonly FlyBookingExtraPackagesDAL flyBookingExtraPackagesDAL;
        private readonly PriceDetailDAL priceDetailDAL;
        private readonly OrderDAL orderDAL;
        private readonly FlyBookingPackagesOptionalDAL _flyBookingPackagesOptionalDAL;
        private readonly IOptions<DataBaseConfig> dataBaseConfig;
        private readonly int max_infant_age = 2;
        private readonly int max_child_age = 12;
        private readonly int min_adult_age = 13;
        private readonly OrderDAL _orderDAL;
        private readonly ClientDAL _clientDAL;
        public FlyBookingDetailRepository(IOptions<DataBaseConfig> _dataBaseConfig)
        {
            flyBookingDetailDAL = new FlyBookingDetailDAL(_dataBaseConfig.Value.SqlServer.ConnectionString);
            airPortCodeDAL = new AirPortCodeDAL(_dataBaseConfig.Value.SqlServer.ConnectionString);
            airLineDAL = new AirLineDAL(_dataBaseConfig.Value.SqlServer.ConnectionString);
            priceDetailDAL = new PriceDetailDAL(_dataBaseConfig.Value.SqlServer.ConnectionString);
            orderDAL = new OrderDAL(_dataBaseConfig.Value.SqlServer.ConnectionString);
            clientDAL = new ClientDAL(_dataBaseConfig.Value.SqlServer.ConnectionString);
            flyBookingExtraPackagesDAL = new FlyBookingExtraPackagesDAL(_dataBaseConfig.Value.SqlServer.ConnectionString);
            dataBaseConfig = _dataBaseConfig;
            _orderDAL = new OrderDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            _clientDAL = new ClientDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            _flyBookingPackagesOptionalDAL = new FlyBookingPackagesOptionalDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
        }

       
        public async Task<FlyBookingDetail> GetByFlyGroupAndLeg(long orderId,int leg, string group_fly)
        {
            try
            {
                if (group_fly == null || group_fly.Trim() == "") return new FlyBookingDetail();
                return flyBookingDetailDAL.GetDetailByLeg(orderId, leg, group_fly);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetByFlyGroupAndLeg - FlyBookingDetailRepository: " + ex.ToString());
            }
            return new FlyBookingDetail();
        }

        public List<FlyBookingDetail> GetListByOrderId(long orderId)
        {
            return flyBookingDetailDAL.GetListByOrderId(orderId);
        }
       
        public FlyBookingDetail GetByOrderId(long orderId)
        {
            return flyBookingDetailDAL.GetDetail(orderId);
        }
        public async Task<FlyBookingDetail> GetFlyBookingById(long fly_booking_id)
        {
            return  await flyBookingDetailDAL.GetFlyBookingById(fly_booking_id);

        }


        public async Task<List<FlyBookingExtraPackages>> GetExtraPackageByFlyBookingId(string group_fly)
        {
            try
            {
                return await flyBookingExtraPackagesDAL.GetByFlyBookingId(group_fly);

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetExtraPackageByFlyBookingId - FlyBookingDetailRepository: " + ex);
                return null;
            }
        }
        public async Task<List<FlyBookingDetail>> GetListByGroupFlyID(string group_fly)
        {
            return await flyBookingDetailDAL.GetListByGroupFlyID(group_fly);

        }
        public async Task<List<FlyBookingDetail>> GetListByGroupFlyID(long orderId, string group_fly)
        {
            try
            {
                if (group_fly == null || group_fly.Trim() == "") return new List<FlyBookingDetail>();
                return await flyBookingDetailDAL.GetListByGroupFlyID(orderId, group_fly);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetListByGroupFlyID - FlyBookingDetailRepository: " + ex);
            }
            return new List<FlyBookingDetail>();
        }
        public async Task<long> SummitFlyBookingServiceData(OrderManualFlyBookingServiceSummitModel data, int is_client_debt)
        {
            try
            {
                var order = _orderDAL.GetByOrderId(data.order_id);
                if (order == null || order.OrderId <= 0) return -1;
                var client = await _clientDAL.GetClientDetail((long)order.ClientId);
                var adt_object = data.extra_packages.Where(x => x.package_id == "adt_amount").FirstOrDefault();
                var chd_object = data.extra_packages.Where(x => x.package_id == "chd_amount").FirstOrDefault();
                var inf_object = data.extra_packages.Where(x => x.package_id == "inf_amount").FirstOrDefault();
                int adult_number = 0;
                int child_number = 0;
                int infant_number = 0;
                if (adt_object!=null && adt_object.package_id!=null && adt_object.package_id.Trim() != "")
                {
                    data.amount_adt = adt_object.amount;
                     adult_number = adt_object.quantity;
                 
                }
                else
                {
                    adult_number = 1;
                }
                if (chd_object != null && chd_object.package_id != null && chd_object.package_id.Trim() != "")
                {
                    data.amount_chd = chd_object.amount;
                     child_number = chd_object.quantity;
                }
                else
                {
                    child_number = 0;
                }
                if (inf_object != null && inf_object.package_id != null && inf_object.package_id.Trim() != "")
                {
                    data.amount_inf = inf_object.amount;
                     infant_number = inf_object.quantity;
                }
                else
                {
                    infant_number = 0;
                }
               
                var summit_object_sql = new OrderManualFlyBookingSQLServiceSummitModel()
                {
                    order_id = data.order_id,
                    total_amount= data.amount_adt + data.amount_chd + data.amount_inf,
                    go = new FlyBookingDetail(),
                    back = null,
                    passengers = new List<Passenger>(),
                    extra_packages = new List<FlyBookingExtraPackages>()
                };
               
                if (data.passenger!=null && data.passenger.Count > 0)
                {
                    TimeSpan range_infant_max = TimeSpan.FromSeconds((new DateTime(max_infant_age, 1, 1, 0, 0, 0)).Second);
                    TimeSpan range_child_max = TimeSpan.FromSeconds((new DateTime(max_child_age, 1, 1, 0, 0, 0)).Second);
                    TimeSpan range_adult_min = TimeSpan.FromSeconds((new DateTime(max_child_age, 1,1, 0, 0, 1)).Second);
                    foreach (var p in data.passenger)
                    {
                        string person_type = "INF";
                        var p_age = DateTime.Now - p.birthday;
                        if (p_age> range_infant_max && p_age <= range_child_max)
                        {
                            person_type = "CHD";
                        }
                        else if(p_age > range_adult_min)
                        {
                            person_type = "ADT";
                        }
                        var new_passenger = new Passenger()
                        {
                            Id=p.id,
                            Birthday = p.birthday,
                            Gender = p.genre == 0 ? true : false,
                            MembershipCard="",
                            Name=p.name ??"",
                            Note=p.note ?? "",
                            OrderId=data.order_id,
                            PersonType= person_type,
                        };
                        if (p.id > 0)
                        {
                            new_passenger.Id = p.id;
                        }
                        summit_object_sql.passengers.Add(new_passenger);
                    }
                }
                var fare_list_id = new List<string>() { "adt_amount", "chd_amount", "inf_amount" };
                var extrapackage_list = data.extra_packages.Where(x => !fare_list_id.Contains(x.package_id)).ToList();
                double total_package_list = 0;
                if (extrapackage_list != null && extrapackage_list.Count > 0)
                {
                    foreach (var ex in extrapackage_list)
                    {
                        total_package_list += ex.amount;
                        summit_object_sql.extra_packages.Add(new FlyBookingExtraPackages()
                        {
                            Amount = (decimal)ex.amount,
                            BasePrice = (decimal)ex.base_price,
                            CreatedBy = data.user_summit,
                            CreatedDate = DateTime.Now,
                            PackageCode = ex.package_code,
                            PackageId = ex.package_id == null ? "" : (ex.package_id.Substring(0, ex.package_id.Length <= 50 ? ex.package_id.Length : 50)),
                            Quantity = ex.quantity,
                            UpdatedBy = data.user_summit,
                            UpdatedDate = DateTime.Now,
                            Profit=ex.profit,
                            Price=ex.amount -ex.profit
                        });
                    }
                }
                //-- Calucate Profit:
                var ticket_profit = 0; /* (adt_object == null ? 0 : adt_object.profit) + (chd_object == null ? 0 : chd_object.profit) + (inf_object == null ? 0 : inf_object.profit);*/
                var ticket_profit_adt = 0; /*(adt_object == null ? 0 : adt_object.profit);*/
                var ticket_profit_chd = 0; /*(chd_object == null ? 0 : chd_object.profit);*/
                var ticket_profit_inf = 0; /*(inf_object == null ? 0 : inf_object.profit);*/
                int multi = data.back == null ? 1 : 2;
                Airlines airline = null;

                //-- flyBookingDetail:
                if (data.go != null)
                {
                    int status = 0;
                    var exists_fly = await flyBookingDetailDAL.GetFlyBookingById(data.go.id);
                    if(exists_fly!=null && exists_fly.Status != null)
                    {
                        status = (int)exists_fly.Status;
                        /*
                        if ((status == (int)ServiceStatus.Decline && is_client_debt == (int)DebtType.DEBT_ACCEPTED) || (status == (int)ServiceStatus.Decline && client.ClientType == (int)ClientType.kl))
                        {
                            status = (int)ServiceStatus.WaitingExcution;
                        }*/
                    }
                    airline = await airLineDAL.GetAirLineByCode(data.go.airline);
                    var route = new FlyBookingDetail()
                    {
                        Id = data.go.id,
                        AdultNumber = adult_number,
                        Airline = data.go.airline,
                        Amount = Math.Round((data.amount_adt + data.amount_chd + data.amount_inf + total_package_list) / (double)multi, 0),
                        AmountAdt = adt_object != null && adt_object.amount > 0 ? Math.Round((adt_object.amount - ticket_profit_adt) / (double)multi, 0) : 0,
                        AmountChd = chd_object != null && chd_object.amount > 0 ?  Math.Round((chd_object.amount - ticket_profit_chd) / (double)multi, 0) : 0,
                        AmountInf = inf_object != null && inf_object.amount > 0 ?  Math.Round((inf_object.amount - ticket_profit_inf) / (double)multi, 0) : 0,
                        BookingCode = data.go.booking_code == null || data.go.booking_code.Trim() == "" ? "" : data.go.booking_code,
                        BookingId = 0,
                        ChildNumber = child_number,
                        Currency = "VND",
                        Difference = 0,
                        EndDate = data.end_date,
                        EndPoint = data.end_point,
                        ExpiryDate = DateTime.Now.AddDays(1),
                        FareAdt = adt_object!=null && adt_object.base_price > 0 ? Math.Round((adt_object.base_price - ticket_profit_adt) / (double)multi / (double)(adult_number), 0) : 0,
                        FareChd = chd_object != null && chd_object.base_price > 0 ? Math.Round((chd_object.base_price - ticket_profit_chd) / (double)multi / (double)(child_number), 0) : 0,
                        FareInf = inf_object != null && inf_object.base_price > 0 ? Math.Round((inf_object.base_price - ticket_profit_inf) / (double)multi / (double)(infant_number), 0) : 0,
                        FeeAdt = 0,
                        FeeChd = 0,
                        FeeInf = 0,
                        Flight = data.go.fly_code==null || data.go.fly_code.Trim()==""?"":data.go.fly_code,
                        GroupClass = "",
                        InfantNumber = infant_number,
                        Leg = 0,
                        OrderId = data.order_id,
                        PriceId = 0,
                        Profit = data.profit / (double)multi,
                        SalerId = data.main_staff,
                        ServiceCode = data.service_code == null || data.service_code.Trim() == "" ? "" : data.service_code,
                        ServiceFeeAdt = 0,
                        ServiceFeeChd = 0,
                        ServiceFeeInf = 0,
                        Session = "Manual",
                        StartDate = data.start_date,
                        StartPoint = data.start_point,
                        Status = status,
                        TaxAdt = 0,
                        TaxChd = 0,
                        TaxInf = 0,
                        TotalBaggageFee = Math.Round(total_package_list / (double)multi, 0),
                        TotalCommission = 0,
                        TotalDiscount = 0,
                        TotalNetPrice = Math.Round((data.amount_adt + data.amount_chd + data.amount_inf + total_package_list - data.profit) / (double)multi, 0),
                        GroupBookingId = data.group_id,
                        UpdatedBy = data.user_summit,
                        UpdatedDate = DateTime.Now,
                        PriceAdt = adt_object != null && adt_object.amount > 0 ? Math.Round((adt_object.amount - ticket_profit_adt) / (double)multi, 0) : 0,
                        PriceChd = chd_object != null && chd_object.amount > 0 ? Math.Round((chd_object.amount - ticket_profit_chd) / (double)multi, 0) : 0,
                        PriceInf = inf_object != null && inf_object.amount > 0 ? Math.Round((inf_object.amount - ticket_profit_inf) / (double)multi, 0) : 0,
                        Price = Math.Round((data.amount_adt + data.amount_chd + data.amount_inf + total_package_list - data.profit) / (double)multi, 0),
                        SupplierId = airline != null ? airline.SupplierId : null,
                        Note=data.note==null?"":data.note,
                        ProfitAdt = adt_object != null ? adt_object.profit / (double)multi : 0,
                        ProfitChd = chd_object != null ? chd_object.profit / (double)multi : 0,
                        ProfitInf = inf_object != null ? inf_object.profit / (double)multi : 0,
                        Adgcommission=data.commission / (double)multi,
                        OthersAmount=data.others_amount / (double)multi
                    };
                    if (data.go.id > 0)
                    {
                        var package_optional = await _flyBookingPackagesOptionalDAL.GetBookingPackagesOptionalsByBookingId(data.go.id);
                        if (package_optional == null || package_optional.Count <= 0)
                        {
                            route.Price = Math.Round((data.amount_adt + data.amount_chd + data.amount_inf + total_package_list - data.profit) / (double)multi, 0);

                        }
                    }
                    else
                    {
                        route.Price = Math.Round((data.amount_adt + data.amount_chd + data.amount_inf + total_package_list - data.profit) / (double)multi, 0);
                    }
                    summit_object_sql.go = route;
                }
                if (data.back != null)
                {
                    airline = await airLineDAL.GetAirLineByCode(data.back.airline);

                    summit_object_sql.go.EndDate = data.start_date;
                    var route = new FlyBookingDetail()
                    {
                        Id = data.back.id,
                        AdultNumber = adult_number,
                        Airline = data.back.airline,
                        Amount = Math.Round((data.amount_adt + data.amount_chd + data.amount_inf + total_package_list) / (double)multi, 0),
                        AmountAdt = adt_object != null && adt_object.amount > 0 ? Math.Round((adt_object.amount - ticket_profit_adt) / (double)multi, 0) : 0,
                        AmountChd = chd_object != null && chd_object.amount > 0 ? Math.Round((chd_object.amount - ticket_profit_chd) / (double)multi, 0) : 0,
                        AmountInf = inf_object != null && inf_object.amount > 0 ? Math.Round((inf_object.amount - ticket_profit_inf) / (double)multi, 0) : 0,
                        BookingCode = data.back.booking_code == null || data.back.booking_code.Trim() == "" ? "" : data.back.booking_code,
                        BookingId = 0,
                        ChildNumber = child_number,
                        Currency = "VND",
                        Difference = 0,
                        EndDate = data.end_date,
                        EndPoint = data.start_point,
                        ExpiryDate = DateTime.Now.AddDays(1),
                        FareAdt = adt_object != null && adt_object.base_price > 0 ? Math.Round((adt_object.base_price - ticket_profit_adt) / (double)multi / (double)(adult_number), 0) : 0,
                        FareChd = chd_object != null && chd_object.base_price > 0 ? Math.Round((chd_object.base_price - ticket_profit_chd) / (double)multi / (double)(child_number), 0) : 0,
                        FareInf = inf_object != null && inf_object.base_price > 0 ? Math.Round((inf_object.base_price - ticket_profit_inf) / (double)multi / (double)(infant_number), 0) : 0,
                        FeeAdt = 0,
                        FeeChd = 0,
                        FeeInf = 0,
                        Flight = data.back.fly_code == null || data.back.fly_code.Trim() == "" ? "" : data.back.fly_code,
                        GroupClass = "",
                        InfantNumber = infant_number,
                        Leg = 1,
                        OrderId = data.order_id,
                        PriceId = 0,
                        Profit = data.profit / (double)multi,
                        SalerId = data.main_staff,
                        ServiceCode = data.service_code == null || data.service_code.Trim() == "" ? "" : data.service_code,
                        ServiceFeeAdt = 0,
                        ServiceFeeChd = 0,
                        ServiceFeeInf = 0,
                        Session = "Manual",
                        StartDate = data.end_date,
                        StartPoint = data.end_point,
                        Status = 0,
                        TaxAdt = 0,
                        TaxChd = 0,
                        TaxInf = 0,
                        TotalBaggageFee = Math.Round(total_package_list / (double)multi, 0),
                        TotalCommission = 0,
                        TotalDiscount = 0,
                        TotalNetPrice = Math.Round((data.amount_adt + data.amount_chd + data.amount_inf + total_package_list - data.profit) / (double)multi, 0),
                        GroupBookingId = data.group_id,
                        UpdatedBy = data.user_summit,
                        UpdatedDate = DateTime.Now,
                        PriceAdt = adt_object != null && adt_object.amount > 0 ? Math.Round((adt_object.amount - ticket_profit_adt) / (double)multi, 0) : 0,
                        PriceChd = chd_object != null && chd_object.amount > 0 ? Math.Round((chd_object.amount - ticket_profit_chd) / (double)multi, 0) : 0,
                        PriceInf = inf_object != null && inf_object.amount > 0 ? Math.Round((inf_object.amount - ticket_profit_inf) / (double)multi, 0) : 0,
                        
                        Price = Math.Round((data.amount_adt + data.amount_chd + data.amount_inf + total_package_list - data.profit) / (double)multi, 0),
                        SupplierId = airline != null ? airline.SupplierId : null,
                        Note = data.note == null ? "" : data.note,
                        ProfitAdt = adt_object != null ? adt_object.profit / (double)multi : 0,
                        ProfitChd = chd_object != null ? chd_object.profit / (double)multi : 0,
                        ProfitInf = inf_object != null ? inf_object.profit / (double)multi : 0,
                        Adgcommission = data.commission / (double)multi,
                        OthersAmount = data.others_amount / (double)multi
                    };
                    if (data.go.id > 0)
                    {
                        var package_optional = await _flyBookingPackagesOptionalDAL.GetBookingPackagesOptionalsByBookingId(data.go.id);
                        if (package_optional == null || package_optional.Count <= 0)
                        {
                            route.Price = Math.Round((data.amount_adt + data.amount_chd + data.amount_inf + total_package_list - data.profit) / (double)multi, 0);

                        }
                    }
                    else
                    {
                        route.Price = Math.Round((data.amount_adt + data.amount_chd + data.amount_inf + total_package_list - data.profit) / (double)multi, 0);
                    }
                    summit_object_sql.back = route;

                }
                if (summit_object_sql.go.Id <= 0)
                {
                    var packages = new FlyBookingPackagesOptional()
                    {
                        Amount = Math.Round((data.amount_adt + data.amount_chd + data.amount_inf + total_package_list - data.profit), 0),
                        BookingId = summit_object_sql.go.Id,
                        Note = "",
                        SuplierId = airline != null ? (int)airline.SupplierId : 0,
                        CreatedBy = data.user_summit,
                        CreatedDate = DateTime.Now,
                        UpdatedBy = data.user_summit,
                        UpdatedDate = DateTime.Now,
                        PackageName=""
                    };
                    var success = _flyBookingPackagesOptionalDAL.CreateFlyBookingPackagesOptional(packages);
                }
                var id = await flyBookingDetailDAL.UpdateFlyBooking(summit_object_sql);
                data.group_id = summit_object_sql.go.GroupBookingId;

                return id;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SummitFlyBookingServiceData - FlyBookingDetailRepository: " + ex);
                return -2;
            }
        }
        public async Task<List<Bookingdetail>> GetListBookingdetailByOrderId(long orderId)
        {
            return  flyBookingDetailDAL.GetListBookingdetailByOrderId(orderId);
        }

        public async Task<Bookingdetail> GetDetailFlyBookingDetailById(int FlyBookingId)
        {
            try
            {
                DataTable dt = await flyBookingDetailDAL.GetDetailFlyBookingDetailById(FlyBookingId);
                if (dt != null && dt.Rows.Count > 0)
                {
                    var ListData = dt.ToList<Bookingdetail>();
                    return ListData[0];
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetDetailFlyBookingDetailById- FlyBookingDetailRepository: " + ex);
            }
            return null;
        }
        public async Task<GenericViewModel<FlyBookingSearchViewModel>> GetPagingList(SearchFlyBookingViewModel searchModel, int currentPage, int pageSize)
        {
            var model = new GenericViewModel<FlyBookingSearchViewModel>();
            try
            {

                DataTable dt =  flyBookingDetailDAL.GetPagingList(searchModel, currentPage, pageSize);
                if (dt != null && dt.Rows.Count > 0)
                {
                    model.ListData = dt.ToList<FlyBookingSearchViewModel>();
                    model.CurrentPage = currentPage;
                    model.PageSize = pageSize;
                    model.TotalRecord = Convert.ToInt32(dt.Rows[0]["TotalRow"]);
                    model.TotalPage = (int)Math.Ceiling((double)model.TotalRecord / model.PageSize);
                    return model;
                }
                return null;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetPagingList - FlyBookingDetailRepository: " + ex);
                return null;
            }
        }
        public async Task<long> UpdateServiceCode(string group_booking_id, string service_code)
        {
            return await flyBookingDetailDAL.UpdateServiceCode(group_booking_id, service_code);
        }
        public async Task<PriceDetail> GetActiveFlyBookingPriceDetailByOrder(long order_id)
        {
            try
            {
                var order = orderDAL.GetByOrderId(order_id);
                int client_type = ClientType.kl;
                if(order!=null && order.OrderId > 0)
                {
                    var client = await clientDAL.GetClientDetail((long)order.ClientId);
                    if(client!=null && client.Id > 0)
                    {
                        client_type = client.ClientType==null ? ClientType.kl: (int)client.ClientType;
                    }
                }
                return await priceDetailDAL.GetActiveFlyBookingPriceDetail(client_type);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetActiveFlyBookingPriceDetailByOrder - FlyBookingDetailRepository: " + ex);
                return null;
            }
        }

        public async Task<long> UpdateOperatorOrderPrice(FlyOperatorOrderPriceSummitViewModel data)
        {
            try
            {
                var fly_booking_list = await flyBookingDetailDAL.GetListByGroupFlyID(data.group_booking_id);
                if(fly_booking_list!=null && fly_booking_list.Count > 0)
                {
                    int fly_count = fly_booking_list.Count;
                    var adt_price = data.list.Where(x => x.id == -1).FirstOrDefault();
                    var chd_price = data.list.Where(x => x.id == -2).FirstOrDefault();
                    var inf_price = data.list.Where(x => x.id == -3).FirstOrDefault();
                    FlyOperatorOrderPriceSummitSQLModel model = new FlyOperatorOrderPriceSummitSQLModel()
                    {
                        detail = new List<FlyBookingDetail>(),
                        extra = new List<FlyBookingExtraPackages>()
                    };
                    double extra_packages_price = 0;
                    var list_extra = data.list.Where(x => !(new List<long> { -1, -2, -3 }).Contains(x.id)).ToList();
                    if (list_extra != null && list_extra.Count > 0)
                    {
                        foreach (var extra in list_extra)
                        {
                            var ex_packages = await flyBookingExtraPackagesDAL.GetById(extra.id);
                            if (ex_packages != null)
                            {
                                extra_packages_price += extra.price;
                                ex_packages.Price = extra.price;
                                model.extra.Add(ex_packages);
                            }
                        }
                    }
                    foreach (var fly in fly_booking_list)
                    {
                        double total_price = Math.Round(extra_packages_price / (double)fly_count, 0);
                        if (adt_price != null)
                        {
                            total_price += Math.Round(adt_price.price / (double)fly_count, 0);
                            fly.PriceAdt = Math.Round(adt_price.price / (double)fly_count,0);
                        }
                        if (chd_price != null)
                        {
                            total_price += Math.Round(chd_price.price / (double)fly_count, 0);
                            fly.PriceChd = Math.Round(chd_price.price / (double)fly_count, 0);

                        }
                        if (inf_price != null)
                        {
                            total_price += Math.Round(inf_price.price / (double)fly_count, 0);
                            fly.PriceInf = Math.Round(inf_price.price / (double)fly_count, 0);
                        }
                        fly.Price = total_price;
                        model.detail.Add(fly);
                    }
                    
                    return await flyBookingDetailDAL.UpdateOperatorOrderPrice(model);
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetActiveFlyBookingPriceDetailByOrder - FlyBookingDetailRepository: " + ex);
                return -1;
            }
        }
        public async Task<long> UpdateServiceStatus(int status,string group_booking_id, int user_id)
        {
            return await flyBookingDetailDAL.UpdateServiceStatus(status,group_booking_id, user_id);
        }
        public async Task<long> UpdateServiceOperator( string group_booking_id, int user_id, int user_commit)
        {
            return await flyBookingDetailDAL.UpdateServiceOperator( group_booking_id, user_id, user_commit);
        }
        public async Task<long> DeleteFlyBookingByID(string group_booking_id)
        {
            try
            {
                return await flyBookingDetailDAL.DeleteFlyBookingByID(group_booking_id);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("DeleteFlyBookingByID - FlyBookingDetailRepository: " + ex);

            }
            return 0;
        }
        public async Task<long> CancelHotelBookingByID(string group_booking_id, int user_id)
        {
            try
            {
                return await flyBookingDetailDAL.CancelFlyBookingByID(group_booking_id, user_id);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CancelHotelBookingByID - FlyBookingDetailRepository: " + ex);

            }
            return 0;
        }
        public async Task<List<FlyBookingPackagesOptional>> GetBookingPackagesOptionalsByBookingId(long booking_id)
        {
            return await _flyBookingPackagesOptionalDAL.GetBookingPackagesOptionalsByBookingId(booking_id);
            
        }
        public async Task<long> UpdateFlyBookingOptional(List<FlyBookingPackagesOptional> data, string group_booking_id,int user_summit)
        {
            try
            {
                double price = 0;
               if(data!=null && data.Count > 0)
               {
                    var list_remain = new List<long>();
                    foreach(var item in data)
                    {
                        if(item.Note!=null && item.Note.Trim() != "")
                        {
                            item.Note = CommonHelper.RemoveSpecialCharacterExceptVietnameseCharacter(item.Note);
                        }
                        if(item.Status!= 1)
                        price += item.Amount > 0 ? item.Amount : 0;
                        item.CreatedBy = user_summit;
                        item.UpdatedBy = user_summit;
                        var id = await _flyBookingPackagesOptionalDAL.CreateOrUpdatePackageOptional(item);
                        list_remain.Add(item.Id);
                    }
                    var group_fly = await flyBookingDetailDAL.GetListByGroupFlyID(group_booking_id);
                    var go = new FlyBookingDetail();
                    foreach(var item in group_fly)
                    {
                        item.Price = Math.Round(price / (double)group_fly.Count);
                        item.UpdatedBy = user_summit;
                        item.UpdatedDate = DateTime.Now;
                        flyBookingDetailDAL.UpdateFlyBookingDetail(item);
                        if (item.Leg == 0) go = item;
                    }
                    if(go != null && go.Id > 0)
                    {
                        await flyBookingDetailDAL.RemoveNonExistsFlyBookingOpional(go.Id, list_remain);
                    }
                    return data[0].Id;
               }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateFlyBookingOptional - FlyBookingDetailRepository: " + ex);

            }
            return 0;

        }
        public async Task<string> ExportDeposit(SearchFlyBookingViewModel searchModel, string FilePath)
        {
            var pathResult = string.Empty;
            try
            {
                var data = new List<FlyBookingSearchViewModel>();


                DataTable dt = flyBookingDetailDAL.GetPagingList(searchModel, searchModel.PageIndex, searchModel.pageSize);
                if (dt != null && dt.Rows.Count > 0)
                {
                    data = dt.ToList<FlyBookingSearchViewModel>();
                  
                }
               
                if (data != null && data.Count > 0)
                {
                    Workbook wb = new Workbook();
                    Worksheet ws = wb.Worksheets[0];
                    ws.Name = "Danh sách đặt dịch vụ vé";
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
                       var leg_count= item.GroupBookingId.Split(",").Count();

                        RowIndex++;
                        ws.Cells["A" + RowIndex].PutValue(RowIndex - 1);
                        ws.Cells["A" + RowIndex].SetStyle(alignCenterStyle);
                        ws.Cells["B" + RowIndex].PutValue(item.ServiceCode);
                        ws.Cells["C" + RowIndex].PutValue("Hành trình: "+(leg_count > 1 ? "Khứ hồi" : "Một chiều")+"\n" + " Điểm đi: "+item.StartPoint + (leg_count > 1 ? "(" + item.StartDate.ToString("dd/MM/yyyy - HH:mm") + ")" : "")+"\n"
                            + "Điểm đến: "+item.EndPoint  +(leg_count > 1 ? "(" + item.EndDate.ToString("dd/MM/yyyy - HH:mm") + ")" : "")
                            + "\n Chiều đi: "+item.GoFlightNumber + " - " + item.GoAirLines +"\n Mã đặt chỗ: " +item.GoBookingCode+"\n"+
                            (leg_count > 1 ? " Chiều về: " + item.BackFlightNumber + " - " + item.BackAirLines + "\n "+"Mã đặt chỗ: " + item.BackBookingCode : "\n")+
                       "Số người: "+item.PassengerNumber);
                        ws.Cells["D" + RowIndex].PutValue(item.StartDate.ToString("dd/MM/yyyy"));
                        ws.Cells["E" + RowIndex].PutValue(item.EndDate.ToString("dd/MM/yyyy"));
                        ws.Cells["F" + RowIndex].PutValue(item.OrderNo);
                        ws.Cells["G" + RowIndex].PutValue(item.CreatedDate.ToString("dd/MM/yyyy"));
                        ws.Cells["H" + RowIndex].PutValue(item.SalerName );
                        ws.Cells["I" + RowIndex].PutValue(item.OperatorName);
                        ws.Cells["J" + RowIndex].PutValue(item.BookingCode);
                        ws.Cells["K" + RowIndex].PutValue(item.FlyBookingStatusName);
                        ws.Cells["L" + RowIndex].PutValue(((item.AmountGo == null ? 0 : (double)item.AmountGo) ));
                        ws.Cells["L" + RowIndex].SetStyle(numberStyle);
                        ws.Cells["M" + RowIndex].PutValue(((item.PriceGo == null ? 0 : (double)item.PriceGo) ));
                        ws.Cells["M" + RowIndex].SetStyle(numberStyle);
                        ws.Cells["N" + RowIndex].PutValue(((item.AmountGo == null ? 0 : (double)item.AmountGo)  - (item.PriceGo == null ? 0 : (double)item.PriceGo)));
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

        public async Task<List<FlyBookingPackagesOptionalViewModel>> GetFlyBookingPackagesOptionalsByBookingId(long hotelBookingId)
        {
            var model = new List<FlyBookingPackagesOptionalViewModel>();
            try
            {

                DataTable dt = await flyBookingDetailDAL.GetFlyBookingRoomsOptionalByBookingId(hotelBookingId);
                if (dt != null && dt.Rows.Count > 0)
                {
                    model = dt.ToList<FlyBookingPackagesOptionalViewModel>();
                    return model;
                }
                return new List<FlyBookingPackagesOptionalViewModel>();
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetHotelBookingOptionalListByHotelBookingId - HotelBookingRepository: " + ex);
                return new List<FlyBookingPackagesOptionalViewModel>();
            }
        }
    }
}
