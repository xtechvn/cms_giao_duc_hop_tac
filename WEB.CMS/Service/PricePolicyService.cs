using Entities.Models;
using Entities.ViewModels;
using Entities.ViewModels.FlyPricePolicy;
using Entities.ViewModels.Hotel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Contants;

namespace GDHT.CMS.Service
{
    public static class PricePolicyService
    {
        public static double CalucateMinProfit(List<HotelPricePolicyViewModel> policy_list, double amount, DateTime arrival_date, DateTime departure_date)
        {
            double profit = 0;
            try
            {
                if (policy_list != null && policy_list.Count > 0)
                {
                    int nights = Convert.ToInt32((departure_date - arrival_date).TotalDays < 1 ? 1 : (departure_date - arrival_date).TotalDays);
                    double price = amount / (double)nights;
                    List<double> actual_profit = new List<double>();
                    for (int d = 0; d < nights; d++)
                    {
                        var stay_date = arrival_date.AddDays(d);
                        var day_of_week = stay_date.DayOfWeek;
                        var selected_policy = policy_list/*.Where(x => x.FromDate <= stay_date && x.ToDate >= stay_date )*/;
                        List<double> profit_day = new List<double>();
                        foreach (var policy in selected_policy)
                        {
                            double item_profit = 0;
                            switch (policy.UnitId)
                            {
                                case (int)PriceUnitType.VND:
                                    {
                                        item_profit = policy.Profit;
                                    }
                                    break;
                                case (int)PriceUnitType.PERCENT:
                                    {
                                        item_profit = Math.Round((price * policy.Profit / (double)100), 0);
                                    }
                                    break;
                            }
                            profit_day.Add(item_profit);
                        }
                        if (profit_day.Count > 0)
                        {
                            actual_profit.Add(profit_day.OrderBy(x => x).First());
                        }
                    }
                    if (actual_profit.Count > 0)
                    {
                        profit = actual_profit.Sum(x => x);
                    }
                }

            }
            catch
            {

            }
            return profit;
        }

        public static FlightServicePriceModel GetFlyTicketProfit(List<FlyPricePolicyViewModel> policy_list, double amount)
        {
            try
            {
                List<PricePolicyServiceModel> data = new List<PricePolicyServiceModel>();
                if (policy_list != null && policy_list.Count > 0)
                {
                    foreach (var policy in policy_list)
                    {
                        double item_profit = 0;
                        switch (policy.UnitId)
                        {
                            case (int)PriceUnitType.VND:
                                {
                                    item_profit = policy.Profit;
                                }
                                break;
                            case (int)PriceUnitType.PERCENT:
                                {
                                    item_profit = Math.Round((amount * policy.Profit / (double)100), 0);
                                }
                                break;
                        }
                        data.Add(new PricePolicyServiceModel()
                        {
                            PriceID = policy.PriceDetailId,
                            profit = item_profit
                        });
                    }
                }
                if (data.Count > 0)
                {
                    var min_data = data.OrderBy(x => x.profit).First();
                    FlightServicePriceModel model = new FlightServicePriceModel()
                    {
                        price_id = min_data.PriceID,
                        profit = min_data.profit,
                        amount = min_data.profit + amount,
                        price = min_data.profit + amount
                    };
                    return model;
                }

            }
            catch (Exception)
            {

            }
            return new FlightServicePriceModel()
            {
                price_id = 0,
                amount = amount,
                price = amount,
                profit = 0
            };
        }
        public static RoomDetail GetRoomDetail(string room_id, DateTime arrival_date, DateTime departure_date, int nights, List<HotelFERoomPackageDataModel> room_packages_daily, List<HotelFERoomPackageDataModel> room_packages_special, List<HotelPricePolicyViewModel> profit_list, Hotel hotel, RoomDetail exist_detail = null)
        {
            RoomDetail result = new RoomDetail();
            try
            {
                List<RoomDetailRate> all_rates = new List<RoomDetailRate>();
                for (int d = 0; d < nights; d++)
                {
                    var stay_date = arrival_date.AddDays(d);
                    var day_of_week = (int)stay_date.DayOfWeek;

                    // Lấy ra rate từ ngày đặc biệt
                    var rate_special = room_packages_special.Where(x => x.ApplyDate == stay_date);
                    IEnumerable<string> codes = null;
                    if (rate_special != null && rate_special.Count() > 0)
                    {
                        codes = rate_special.Select(x => x.PackageCode).Distinct();
                        foreach (var c in codes)
                        {
                            var min_rate_inday_of_code = rate_special.Where(x => x.PackageCode == c).OrderBy(x => x.Amount).First();
                            var profit = profit_list.Where(x => x.RoomId.ToString() == room_id && x.PackageCode.ToString().Trim() == min_rate_inday_of_code.PackageCode && x.ProgramId == min_rate_inday_of_code.ProgramId).ToList();
                            var min_profit_value = CalucateProfitPerDay(profit, min_rate_inday_of_code.Amount);
                            all_rates.Add(new RoomDetailRate()
                            {
                                allotment_id = "",
                                apply_date = min_rate_inday_of_code.ApplyDate,
                                cancel_policy = new List<string>(),
                                code = min_rate_inday_of_code.PackageCode,
                                description = "",
                                guarantee_policy = "",
                                id = min_rate_inday_of_code.Id.ToString(),
                                name = min_rate_inday_of_code.PackageName,
                                total_price = min_rate_inday_of_code.Amount + min_profit_value,
                                total_profit = min_profit_value,
                                amount = min_rate_inday_of_code.Amount,
                            });
                        }
                    }
                    // Lấy ra rate từ ngày bình thường
                    var rate_daily = room_packages_daily.Where(x => x.FromDate.Date <= stay_date.Date && x.ToDate.Date >= stay_date.Date);
                    if (rate_daily != null && rate_daily.Count() > 0)
                    {
                        var codes_daily = rate_daily.Select(x => x.PackageCode).Distinct();
                        if (codes != null && codes.Count() > 0)
                        {
                            codes_daily = codes_daily.Where(x => !codes.Contains(x));
                        }
                        foreach (var c in codes_daily)
                        {
                            var min_rate_inday_of_code = rate_daily.Where(x => x.PackageCode == c && x.WeekDay == day_of_week).OrderBy(x => x.Amount).First();
                            var profit = profit_list.Where(x => x.RoomId == min_rate_inday_of_code.RoomTypeId && x.PackageCode.ToString().Trim() == min_rate_inday_of_code.PackageCode && x.ProgramId == min_rate_inday_of_code.ProgramId).ToList();
                            var min_profit_value = CalucateProfitPerDay(profit, min_rate_inday_of_code.Amount);
                            all_rates.Add(new RoomDetailRate()
                            {
                                allotment_id = "",
                                apply_date = stay_date,
                                cancel_policy = new List<string>(),
                                code = min_rate_inday_of_code.PackageCode,
                                description = "",
                                guarantee_policy = "",
                                id = min_rate_inday_of_code.Id.ToString(),
                                name = min_rate_inday_of_code.PackageName,
                                total_price = min_rate_inday_of_code.Amount + min_profit_value,
                                total_profit = min_profit_value,
                                amount = min_rate_inday_of_code.Amount,
                            });
                        }
                    }


                }
                //Combine
                result.rates = all_rates.Where(x => x.total_price > 0).GroupBy(s => new { s.code }).Select(s => new RoomDetailRate
                {
                    id = $"{room_id}-{s.Select(i => i.id).FirstOrDefault()}".ToLower(),
                    code = s.Key.code,
                    name = s.Select(i => i.name).FirstOrDefault(),
                    description = s.Select(i => i.description).FirstOrDefault(),
                    total_price = s.Average(k => k.total_price) * nights,
                    total_profit = s.Average(k => k.total_profit) * nights,
                    amount = s.Average(k => k.amount) * nights,

                }).ToList();
                // Tính min_price
                if (result.rates != null && result.rates.Count > 0)
                {

                    result.min_price = result.rates.Count > 0 ? result.rates.OrderBy(x => x.total_price).First().total_price : 0;
                }
                else
                {
                    result.rates = new List<RoomDetailRate>();
                    result.min_price = 0;
                }
            }
            catch (Exception)
            {

            }
            return result;
        }
        public static double CalucateProfitPerDay(List<HotelPricePolicyViewModel> policy_list, double amount)
        {
            double profit = 0;
            try
            {
                if (policy_list != null && policy_list.Count > 0)
                {
                    List<double> actual_profit = new List<double>();
                    foreach (var policy in policy_list)
                    {
                        double item_profit = 0;
                        switch (policy.UnitId)
                        {
                            case (int)PriceUnitType.VND:
                                {
                                    item_profit = policy.Profit;
                                }
                                break;
                            case (int)PriceUnitType.PERCENT:
                                {
                                    item_profit = Math.Round((amount * policy.Profit / (double)100), 0);
                                }
                                break;
                        }
                        actual_profit.Add(item_profit);
                    }
                    if (actual_profit.Count > 0)
                    {
                        profit = actual_profit.OrderBy(x => x).First();
                    }
                }

            }
            catch
            {

            }
            return profit;
        }
    }
}
