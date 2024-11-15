using DAL;
using Entities.ConfigModels;
using Entities.Models;
using Entities.ViewModels;
using Entities.ViewModels.HotelBookingRoom;
using Entities.ViewModels.OrderManual;
using Microsoft.Extensions.Options;
using Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Repositories.Repositories
{
   public class HotelBookingRoomsRepository : IHotelBookingRoomRepository
    {
        
        private readonly HotelBookingDAL _hotelBookingDAL;
        private readonly HotelBookingRoomDAL _hotelBookingRoomDAL;
        private readonly HotelBookingRoomRatesDAL _hotelBookingRoomRatesDAL;
        private readonly HotelBookingRoomExtraPackagesDAL _hotelBookingRoomExtraPackagesDAL;

        public HotelBookingRoomsRepository(IOptions<DataBaseConfig> dataBaseConfig)
        {

            _hotelBookingRoomDAL = new HotelBookingRoomDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            _hotelBookingRoomRatesDAL = new HotelBookingRoomRatesDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            _hotelBookingDAL = new HotelBookingDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            _hotelBookingRoomExtraPackagesDAL = new HotelBookingRoomExtraPackagesDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
        }

        public Task<List<HotelBookingRooms>> GetByHotelBookingID(long hotel_booking_id)
        {
            return _hotelBookingRoomDAL.GetByHotelBookingID(hotel_booking_id);
        }
        public async Task<List<HotelBookingRoomViewModel>> GetHotelBookingRoomByHotelBookingID(long HotelBookingId,long status)
        {
            var model = new List<HotelBookingRoomViewModel>();
            var model2 = new List<HotelBookingRoomRates>();
            try
            {
                DataTable dt = await _hotelBookingRoomDAL.GetHotelBookingRoomByHotelBookingID(HotelBookingId, status);
                if (dt != null && dt.Rows.Count > 0)
                {
                    model = dt.ToList<HotelBookingRoomViewModel>();

                }
                foreach(var item in model)
                {
                    DataTable dt2 = await _hotelBookingRoomRatesDAL.GetHotelBookingRateByHotelBookingRoomID(Convert.ToInt32(item.Id));
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        item.HotelBookingRoomRates = dt2.ToList<HotelBookingRoomRatesViewModel>();
                        item.SumAmount = item.HotelBookingRoomRates.Sum(x => x.TotalAmount);
                        item.SumUnitPrice = item.SumUnitPrice +Convert.ToDouble(item.HotelBookingRoomRates.Sum(x => x.UnitPrice));

                    }
                }
                var list = new List<HotelBookingRoomRatesViewModel>();
                foreach (var item in model)
                {
                    
                    list= item.HotelBookingRoomRates;
                    if (item.HotelBookingRoomRates!=null && item.HotelBookingRoomRates.Count > 1)
                    {
                        for(int i=0; i<item.HotelBookingRoomRates.Count;i++)
                        {
                            item.HotelBookingRoomRates[i].EndDate = item.HotelBookingRoomRates[i].StayDate;
                            for (int i2=1;i2 < item.HotelBookingRoomRates.Count;i2++)
                            {
                                if (item.HotelBookingRoomRates[i].RatePlanCode == item.HotelBookingRoomRates[i2].RatePlanCode && item.HotelBookingRoomRates[i].StayDate.Month == item.HotelBookingRoomRates[i2].StayDate.Month)
                                {
                                    if ((item.HotelBookingRoomRates[i].EndDate.Day + 1) == item.HotelBookingRoomRates[i2].StayDate.Day)
                                    {
                                        item.HotelBookingRoomRates[i].EndDate = item.HotelBookingRoomRates[i2].StayDate;
                                        item.HotelBookingRoomRates.Remove(item.HotelBookingRoomRates[i2]);
                                        i2--;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if(item.HotelBookingRoomRates != null && item.HotelBookingRoomRates.Count>0)
                            item.HotelBookingRoomRates[0].EndDate = item.HotelBookingRoomRates[0].StayDate;
                    }
                   
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetHotelBookingById - HotelBookingRepository: " + ex);
            }
            return model;
        }
      
       
        public async Task<long> UpdateHotelBookingUnitPrice(HotelBookingUnitPriceChangeSummitModel data,long hotel_booking_id,int user_update)
        {
            try
            {
                double total_unit_price = 0;
                List<long> list_keep_rate = new List<long>();
                List<long> list_keep_room = new List<long>();
                List<HotelBookingRoomRatesOptional> rate_optional = new List<HotelBookingRoomRatesOptional>();
                List<HotelBookingRoomsOptional> room_optional = new List<HotelBookingRoomsOptional>();
                List <HotelBookingRooms> room_sales = new List<HotelBookingRooms>();
                var sale_booking = await _hotelBookingDAL.GetHotelBookingByID(hotel_booking_id);
                int count = 0;
                foreach (var room in data.rooms)
                {
                    count++;
                    var exist_room = await _hotelBookingRoomDAL.GetHotelBookingRoomOptionalById(room.id);
                    var exists_room_sale = await _hotelBookingRoomDAL.GetByID(room.room_id);
                    room_sales.Add(exists_room_sale);
                    if(room.package_name==null || room.package_name.Trim() == "")
                    {
                        room.package_name = sale_booking.ServiceCode + "-" + count;
                    }
                    if (exist_room!=null && exist_room.Id > 0)
                    {
                        foreach (var rate in room.rates)
                        {
                            var exists_rate = await _hotelBookingRoomRatesDAL.GetOptionalByIdAndHotelBookingRoomId(rate.id, room.id);
                            var exists_rate_sale = await _hotelBookingRoomRatesDAL.GetByIdAndHotelBookingRoomId(rate.rate_id, exist_room.HotelBookingRoomId);
                            if (exists_rate != null && exists_rate.Id > 0)
                            {
                                exists_rate.Price = rate.operator_amount;
                                exists_rate.Profit = rate.profit;
                                exists_rate.TotalAmount = rate.operator_amount;
                                exists_rate.UpdatedBy = user_update;
                                exists_rate.UpdatedDate = DateTime.Now;
                                exists_rate.OperatorPrice = rate.operator_price;
                                _hotelBookingDAL.UpdateHotelBookingRoomsRateOptional(exists_rate);
                            }
                            else
                            {
                                exists_rate = new HotelBookingRoomRatesOptional()
                                {
                                    CreatedBy = user_update,
                                    CreatedDate = DateTime.Now,
                                    HotelBookingRoomOptionalId = exist_room.Id,
                                    HotelBookingRoomRatesId = exists_rate_sale.Id,
                                    Id = 0,
                                    Price = rate.operator_amount,
                                    Profit = rate.profit,
                                    TotalAmount = rate.operator_amount,
                                    OperatorPrice=rate.operator_price
                                };
                                _hotelBookingDAL.CreateHotelBookingRoomsRateOptional(exists_rate);

                            }
                            list_keep_rate.Add(exists_rate.Id);
                            rate_optional.Add(exists_rate);
                           
                        }
                        var operator_price = room.rates != null && room.rates.Count > 0 ? room.rates.Sum(x => x.operator_price) :0;
                        var profit = room.rates != null && room.rates.Count > 0 ? room.rates.Sum(x => x.profit):0;
                        var total_amout = room.rates != null && room.rates.Count > 0 ? room.rates.Sum(x => x.operator_amount) :0;

                        exist_room.Price = operator_price;
                        exist_room.Profit = profit;
                        exist_room.TotalAmount = total_amout;
                        exist_room.NumberOfRooms = (short)room.number_of_room;
                        exist_room.UpdatedBy = user_update;
                        exist_room.UpdatedDate = DateTime.Now;
                        exist_room.SupplierId = room.suplier_id;
                        exist_room.PackageName = room.package_name;
                        exist_room.IsRoomFund = room.is_room_fund;
                        _hotelBookingDAL.UpdateHotelBookingRoomsOptional(exist_room);
                        list_keep_room.Add(exist_room.Id);
                        room_optional.Add(exist_room);
                    }
                    else
                    {
                     
                        var operator_price = room.rates != null && room.rates.Count > 0 ? room.rates.Sum(x => x.operator_amount) : 0;
                        var profit = room.rates != null && room.rates.Count > 0 ? room.rates.Sum(x => x.profit) : 0;
                        var total_amout = room.rates != null && room.rates.Count > 0 ? room.rates.Sum(x => x.operator_amount) : 0;
                        exist_room = new HotelBookingRoomsOptional();
                        exist_room.Price = operator_price;
                        exist_room.Profit = profit;
                        exist_room.TotalAmount = total_amout;
                        exist_room.NumberOfRooms = (short)room.number_of_room;
                        exist_room.UpdatedBy = user_update;
                        exist_room.UpdatedDate = DateTime.Now;
                        exist_room.SupplierId = room.suplier_id;
                        exist_room.HotelBookingId = exists_room_sale.HotelBookingId;
                        exist_room.HotelBookingRoomId = exists_room_sale.Id;
                        exist_room.PackageName = room.package_name;
                        exist_room.IsRoomFund = room.is_room_fund;
                        _hotelBookingDAL.CreateHotelBookingRoomsOptional(exist_room);
                        list_keep_room.Add(exist_room.Id);
                        foreach (var rate in room.rates)
                        {
                            var exists_rate_sale = await _hotelBookingRoomRatesDAL.GetByIdAndHotelBookingRoomId(rate.rate_id, exists_room_sale.Id);
                            var exists_rate = new HotelBookingRoomRatesOptional()
                            {
                                CreatedBy = user_update,
                                CreatedDate = DateTime.Now,
                                HotelBookingRoomOptionalId = exist_room.Id,
                                HotelBookingRoomRatesId = exists_rate_sale.Id,
                                Id = 0,
                                Price = rate.operator_amount,
                                Profit = rate.profit,
                                TotalAmount = rate.operator_amount,
                                OperatorPrice = rate.operator_price,

                            };
                            _hotelBookingDAL.CreateHotelBookingRoomsRateOptional(exists_rate);
                            list_keep_rate.Add(exists_rate.Id);
                            rate_optional.Add(exists_rate);
                        }
                        room_optional.Add(exist_room);

                    }

                }
                await _hotelBookingDAL.DeleteNonExistsHotelOptionalData(list_keep_room, list_keep_rate, hotel_booking_id, user_update);

                var room_sale_list = room_optional.Select(x => x.HotelBookingRoomId).Distinct();
                if(room_sale_list != null && room_sale_list.Count() > 0)
                {
                    foreach(var room_sale_id in room_sale_list)
                    {
                        var sale_room = await _hotelBookingRoomDAL.GetByID(room_sale_id);
                        if(sale_room!=null && sale_room.Id > 0)
                        {
                            sale_room.TotalUnitPrice = room_optional.Where(x=>x.HotelBookingRoomId==sale_room.Id).Sum(x => x.TotalAmount);
                            sale_room.UpdatedBy = user_update;
                            sale_room.UpdatedDate = DateTime.Now;
                            await _hotelBookingDAL.UpdateHotelBookingRoom(sale_room);
                        }
                    }
                }
                var rate_sale_list = rate_optional.Select(x => x.HotelBookingRoomRatesId).Distinct();
                if (rate_sale_list != null && rate_sale_list.Count() > 0)
                {
                    foreach (var rate_sale_id in rate_sale_list)
                    {
                        var sale_rate = await _hotelBookingRoomRatesDAL.GetById(rate_sale_id);
                        if (sale_rate != null && sale_rate.Id > 0)
                        {
                            sale_rate.UnitPrice = rate_optional.Where(x => x.HotelBookingRoomRatesId == sale_rate.Id).Sum(x => x.TotalAmount);
                            sale_rate.UpdatedBy = user_update;
                            sale_rate.UpdatedDate = DateTime.Now;
                            await _hotelBookingDAL.UpdateHotelBookingRoomrate(sale_rate);
                        }
                    }
                }

                List<HotelBookingRoomExtraPackages> extra_list = new List<HotelBookingRoomExtraPackages>();
                if (data.extra_packages!=null && data.extra_packages.Count > 0)
                {
                    foreach (var extra in data.extra_packages)
                    {
                        var exist_extra = await _hotelBookingRoomExtraPackagesDAL.GetByIDAndBookingId(extra.id, hotel_booking_id);
                        if (exist_extra != null && exist_extra.Id > 0)
                        {
                            exist_extra.UnitPrice = extra.operator_price * exist_extra.Quantity * exist_extra.Nights;
                            exist_extra.UpdatedBy = user_update;
                            exist_extra.UpdatedDate = DateTime.Now;
                            exist_extra.SupplierId = extra.supplier_id;
                            await _hotelBookingDAL.UpdateHotelBookingExtraPackages(exist_extra);
                            extra_list.Add(exist_extra);
                        }
                    }
                }

                if (sale_booking != null && sale_booking.Id > 0)
                {
                    sale_booking.Price = room_optional.Where(x => x.IsRoomFund == false || x.IsRoomFund == null).Sum(x => x.TotalAmount) + extra_list.Sum(x=>x.UnitPrice) ;
                    sale_booking.UpdatedBy = user_update;
                    sale_booking.UpdatedDate = DateTime.Now;
                    await _hotelBookingDAL.UpdateHotelBooking(sale_booking);
                }
                return hotel_booking_id;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateHotelBookingUnitPrice - HotelBookingRepository: " + ex);
                return -1;
            }

        }
        public async Task<List<HotelBookingRoomRatesOptionalViewModel>> GetHotelBookingRoomRatesOptionalByBookingId(long HotelBookingId)
        {
            var model = new List<HotelBookingRoomRatesOptionalViewModel>();
            try
            {
                DataTable dt = await _hotelBookingDAL.GetHotelBookingRoomRatesOptionalByBookingId(HotelBookingId);
                if (dt != null && dt.Rows.Count > 0)
                {
                    model = dt.ToList<HotelBookingRoomRatesOptionalViewModel>();

                }
                
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetHotelBookingRoomRatesOptionalByBookingId - HotelBookingRepository: " + ex);
            }
            return model;
        }

    }
}
