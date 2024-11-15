using DAL.Generic;
using DAL.StoreProcedure;
using Entities.Models;
using Entities.ViewModels.PricePolicy;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace DAL
{
    public class ProductRoomServiceDAL : GenericService<ProductRoomService>
    {
        private static DbWorker _DbWorker;
        public ProductRoomServiceDAL(string connection) : base(connection)
        {
            _DbWorker = new DbWorker(connection);
        }
     
        public async Task<List<ProductRoomService>> GetByCampaignID(int campaign_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var find = _DbContext.ProductRoomService.Where(x=>x.CampaignId==campaign_id).ToList();
                    return find;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetByCampaignID - ProductRoomServiceDAL: " + ex);
                return null;
            }
        }
        public async Task<DataTable> GetHotelPricePolicyByPrograms(int hotel_id, DateTime from_date, DateTime to_date)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[3];
                objParam[0] = new SqlParameter("@HotelID", hotel_id);
                objParam[1] = new SqlParameter("@ArrivalDate", from_date);
                objParam[2] = new SqlParameter("@DepartureDate", to_date);

                return _DbWorker.GetDataTable(StoreProcedureConstant.GetHotelPricePolicyByPrograms, objParam);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetListPriceDetailByListPackagesId - ProductRoomServiceDAL: " + ex);
            }
            return null;
        }
        public async Task<int> AddorUpdatePriceRoomPolicy(HotelPricePolicySummitModel model)
        {
            try
            {
                if (model.Detail.Id <= 0)
                {
                    var new_product_price_room = new ProductRoomService()
                    {
                        AllotmentsId = model.Detail.AllotmentsId,
                        CampaignId = model.Detail.CampaignId,
                        HotelId = model.Detail.HotelId,
                        PackageCode = model.Detail.PackageCode,
                        ProgramId = model.Detail.ProgramId,
                        ProgramPackageId = model.Detail.ProgramPackageId,
                        RoomId = model.Detail.RoomId,

                    };
                    var id = await AddNew(new_product_price_room);
                    model.Detail.Id = id;

                }
                //else
                //{
                //    var id = await Update(model.Detail);
                //    model.Detail.Id = id;
                //}
                if (model.Detail.Id <= 0)
                {
                    LogHelper.InsertLogTelegram("AddNew - ProductRoomServiceDAL: Cannot insert ProductRoomService with" + JsonConvert.SerializeObject(model.Detail));
                    return -1;
                }
                List<int> exists_price_detail = new List<int>();
                if(model.PriceDetail!=null && model.PriceDetail.Count > 0)
                {
                    foreach(var policy in model.PriceDetail)
                    {
                        policy.ProductServiceId = model.Detail.Id;
                        if (policy.Id <= 0)
                        {
                            var new_pricedetail = new PriceDetail()
                            {
                                AmountLast = policy.Profit,
                                DayList = policy.DayList,
                                FromDate = policy.FromDate,
                                Profit = policy.Profit,
                                MonthList = policy.MonthList,
                                Price = 0,
                                ProductServiceId = model.Detail.Id,
                                ServiceType = (int)ServiceType.BOOK_HOTEL_ROOM_VIN,
                                ToDate = policy.ToDate,
                                UnitId = policy.UnitId,
                                UserCreateId = policy.UserCreateId,
                                UserUpdateId = policy.UserUpdateId
                            };
                            var id = await AddNewPriceDetail(new_pricedetail);
                            policy.Id = id;
                            exists_price_detail.Add(id);
                        }
                        else
                        {
                           
                            var id = await UpdatePriceDetail(policy);
                            exists_price_detail.Add(policy.Id);
                        }
                    }
                }
                await RemoveNonExistsPriceDetail(model.Detail.Id, exists_price_detail);
                return model.Detail.Id;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("AddorUpdatePriceRoomPolicy - ProductRoomServiceDAL: " + ex.ToString());
            }
            return -1;

        }
        public async Task<int> AddorUpdatePriceDetail(HotelPricePolicySummitModel model)
        {
            try
            {
                if (model.PriceDetail != null && model.PriceDetail.Count > 0)
                {
                    foreach (var policy in model.PriceDetail)
                    {
                        if (policy.Id <= 0)
                        {
                            var new_pricedetail = new PriceDetail()
                            {
                                AmountLast = policy.Profit,
                                DayList = policy.DayList,
                                FromDate = policy.FromDate,
                                Profit = policy.Profit,
                                MonthList = policy.MonthList,
                                Price = 0,
                                ProductServiceId = policy.ProductServiceId,
                                ServiceType = (int)ServiceType.BOOK_HOTEL_ROOM_VIN,
                                ToDate = policy.ToDate,
                                UnitId = policy.UnitId,
                                UserCreateId = policy.UserCreateId,
                                UserUpdateId = policy.UserUpdateId
                            };
                            var id = await AddNewPriceDetail(new_pricedetail);
                            policy.Id = id;
                        }
                        else
                        {

                            var id = await UpdatePriceDetail(policy);
                            policy.Id = id;
                        }
                    }
                }
                return model.Detail.Id;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("AddorUpdatePriceDetail - ProductRoomServiceDAL: " + ex.ToString());
            }
            return -1;

        }
        public async Task<int> AddNew(ProductRoomService model)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var add = _DbContext.ProductRoomService.Add(model);
                    await _DbContext.SaveChangesAsync();
                    return model.Id;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("AddNew - ProductRoomServiceDAL: " + ex.ToString());
                return -1;
            }
        }
        public async Task<int> Update(ProductRoomService model)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var add = _DbContext.ProductRoomService.Update(model);
                    await _DbContext.SaveChangesAsync();
                    return model.Id;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Update - ProductRoomServiceDAL: " + ex.ToString());
                return -1;
            }
        }
        public async Task<int> RemoveNonExistsProductServiceRoom(int campaign_id, List<int> list_productserviceroom_exists)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var list = await _DbContext.ProductRoomService.Where(x => x.CampaignId == campaign_id  && !list_productserviceroom_exists.Contains(x.Id)).ToListAsync();
                    if (list != null && list.Count > 0)
                    {
                        foreach (var item in list)
                        {
                            var list_detail = await _DbContext.PriceDetail.Where(x => x.ProductServiceId == item.Id ).ToListAsync();
                            if (list != null && list.Count > 0)
                            {
                                foreach (var item_2 in list_detail)
                                {
                                    item_2.ProductServiceId *= -1;
                                    var add_2 = _DbContext.PriceDetail.Update(item_2);
                                    await _DbContext.SaveChangesAsync();
                                }
                            }
                            item.CampaignId *= -1;
                            item.HotelId *= -1;
                            var add = _DbContext.ProductRoomService.Update(item);
                            await _DbContext.SaveChangesAsync();
                        }
                    }
                }
                return 0;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Update - ProductRoomServiceDAL: " + ex.ToString());
                return -1;
            }
        }
        public async Task<int> RemoveNonExistsPriceDetail(int room_service_id, List<int> price_detail_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var list = await _DbContext.PriceDetail.Where(x=>x.ProductServiceId==room_service_id && x.ServiceType==(int)ServiceType.BOOK_HOTEL_ROOM_VIN && !price_detail_id.Contains(x.Id)).ToListAsync();
                    if(list!=null && list.Count > 0)
                    {
                        foreach(var item in list)
                        {
                            item.ProductServiceId *= -1;
                            var add = _DbContext.PriceDetail.Update(item);
                            await _DbContext.SaveChangesAsync();
                        }
                    }
                }
                return 0;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Update - ProductRoomServiceDAL: " + ex.ToString());
                return -1;
            }
        }
        public async Task<int> AddNewPriceDetail(PriceDetail model)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var add = _DbContext.PriceDetail.Add(model);
                    await _DbContext.SaveChangesAsync();
                    return model.Id;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("AddNewPriceDetail - ProductRoomServiceDAL: " + ex.ToString());
                return -1;
            }
        }
        public async Task<int> UpdatePriceDetail(PriceDetail model)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var exists = _DbContext.PriceDetail.FirstOrDefault(n => n.Id == model.Id);
                    if (exists != null && exists.Id > 0)
                    {
                        exists. AmountLast=model.AmountLast;
                        exists.ToDate =model.ToDate;
                        exists.DayList =model.DayList;
                        exists.FromDate =model.FromDate;
                        exists.MonthList =model.MonthList;
                        exists.Price =model.Price;
                        exists.ProductServiceId =model.ProductServiceId;
                        exists.Profit =model.Profit;
                        exists.ServiceType =model.ServiceType;
                        exists.UnitId =model.UnitId;
 
                        var add = _DbContext.PriceDetail.Update(exists);
                        await _DbContext.SaveChangesAsync();

                    }
                    
                    return model.Id;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdatePriceDetail - ProductRoomServiceDAL: " + ex.ToString());
                return -1;
            }
        }
    }
}
