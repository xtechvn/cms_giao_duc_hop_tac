using DAL.Generic;
using DAL.StoreProcedure;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace DAL
{
    public class PriceDetailDAL : GenericService<PriceDetail>
    {
        private static DbWorker _DbWorker;
        public PriceDetailDAL(string connection) : base(connection)
        {
            _DbWorker = new DbWorker(connection);
        }
        public async Task<PriceDetail> FindByDetail(int price_detail_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var find = _DbContext.PriceDetail.FirstOrDefault(x => x.Id == price_detail_id);
                    return find;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("FindByDetail - PriceDetailDAL: " + ex);
                return null;
            }
        }
        public PriceDetail FindByProductServiceId(int productServiceId)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var find = _DbContext.PriceDetail.FirstOrDefault(x => x.ProductServiceId == productServiceId);
                    return find;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("FindByProductServiceId - PriceDetailDAL: " + ex);
                return null;
            }
        }
        
       
        public async Task<string> AddOrUpdateSinglePriceDetail(ProductRoomService productRoomService, PriceDetail detail,int user_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var update_service = productRoomService;
                    var update_detail = detail;
                    //-- Check campaign
                    Campaign campaign = _DbContext.Campaign.Where(x => x.Id == productRoomService.CampaignId).FirstOrDefault();
                    if(campaign==null || campaign.Id <= 0)
                    {
                        return "Campaign not found";
                    }
                    //-- check & add product_room_service:
                    var productRoomServiceExists = _DbContext.ProductRoomService.Where(x => x.Id == update_service.Id).FirstOrDefault();
                    if(productRoomServiceExists!=null && productRoomServiceExists.Id > 0)
                    {
                        update_detail.ProductServiceId = productRoomServiceExists.Id;
                        update_service = productRoomServiceExists;
                    }
                    else
                    {
                        productRoomServiceExists = _DbContext.ProductRoomService.Where(x => x.CampaignId == update_service.CampaignId && x.ProgramId == update_service.ProgramId && x.ProgramPackageId == update_service.ProgramPackageId && x.RoomId == update_service.RoomId).FirstOrDefault();
                        if (productRoomServiceExists != null && productRoomServiceExists.Id > 0)
                        {
                            update_detail.ProductServiceId = productRoomServiceExists.Id;
                            update_service = productRoomServiceExists;
                        }
                        else
                        {
                            var result = _DbContext.ProductRoomService.Add(update_service);
                            _DbContext.SaveChanges();
                        }
                    }
                    // Check & add/update PriceDetail
                    PriceDetail priceDetail_exists = _DbContext.PriceDetail.Where(x => x.Id == update_detail.Id).FirstOrDefault();
                    if(priceDetail_exists!=null && priceDetail_exists.Id > 0)
                    {
                        update_detail = priceDetail_exists;
                        update_detail.Price = detail.Price;
                        update_detail.Profit = detail.Profit;
                        update_detail.UnitId = detail.UnitId;
                        update_detail.UserUpdateId = detail.UserUpdateId;
                        update_detail.MonthList = detail.MonthList;
                        update_detail.DayList = detail.DayList;
                        update_detail.AmountLast = detail.AmountLast;
                        update_detail.FromDate = detail.FromDate;
                        update_detail.ToDate = detail.ToDate;
                        update_detail.ProductServiceId = detail.ProductServiceId;
                        update_detail.ServiceType = detail.ServiceType;
                        _DbContext.PriceDetail.Update(update_detail);
                        _DbContext.SaveChanges();
                        detail.Id = update_detail.Id;
                    }
                    else
                    {
                        var add_detail = new PriceDetail()
                        {
                            AmountLast = update_detail.AmountLast,
                            DayList = update_detail.DayList,
                            MonthList = update_detail.MonthList,
                            UserUpdateId = user_id,
                            FromDate = update_detail.FromDate,
                            Price = update_detail.Price,
                            ProductServiceId = update_detail.ProductServiceId,
                            Profit = update_detail.Profit,
                            ServiceType = update_detail.ServiceType,
                            ToDate = update_detail.ToDate,
                            UnitId = update_detail.UnitId,
                            UserCreateId = user_id
                        };
                        var result = _DbContext.PriceDetail.Add(add_detail);
                        _DbContext.SaveChanges();
                        detail.Id = add_detail.Id;
                    }

                    return "";
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("AddOrUpdateSinglePriceDetail - PriceDetailDAL: " + ex.ToString());
                return ex.ToString();
            }
        }
        public async Task<int> RemoveByID(int id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var find = _DbContext.PriceDetail.FirstOrDefault(x=>x.Id==id);
                    if(find!=null && find.Id > 0)
                    {
                        find.ProductServiceId *= -1;
                        var remove = _DbContext.PriceDetail.Update(find);
                        await _DbContext.SaveChangesAsync();
                        return (int)ResponseType.SUCCESS;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("RemoveByID - PriceDetailDAL: " + ex);
                return (int)ResponseType.ERROR;
            }
            return (int)ResponseType.FAILED;
        }
        public async Task<PriceDetail> GetActiveFlyBookingPriceDetail(int client_type)
        {
            try
            {
                List<int> client_b2b = new List<int>() { ClientType.DALC1, ClientType.DALC2, ClientType.DLC3,ClientType.DL };
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var price_detail = await (from p in _DbContext.PriceDetail.AsNoTracking()
                                       join b in _DbContext.ProductFlyTicketService.AsNoTracking() on p.ProductServiceId equals b.Id
                                       join a in _DbContext.Campaign.AsNoTracking().OrderByDescending(x=>x.UpdateLast).Where(s => s.Status == (int)Status.HOAT_DONG ) on b.CampaignId equals a.Id
                                       where a.ClientTypeId == client_type 
                                       select new PriceDetail
                                       {
                                           Id = p.Id,
                                           FromDate=p.FromDate,
                                           ToDate=p.ToDate,
                                           ServiceType=p.ServiceType,
                                           Profit=p.Profit,
                                           UnitId=p.UnitId
                                       }).FirstOrDefaultAsync();
                        if (price_detail == null && client_b2b.Contains(client_type))
                        {
                            price_detail = await (from p in _DbContext.PriceDetail.AsNoTracking()
                                                  join b in _DbContext.ProductFlyTicketService.AsNoTracking() on p.ProductServiceId equals b.Id
                                                  join a in _DbContext.Campaign.AsNoTracking().OrderByDescending(x => x.UpdateLast).Where(s => s.Status == (int)Status.HOAT_DONG) on b.CampaignId equals a.Id
                                                  where a.ClientTypeId == ClientType.DL
                                                  select new PriceDetail
                                                  {
                                                      Id = p.Id,
                                                      FromDate = p.FromDate,
                                                      ToDate = p.ToDate,
                                                      ServiceType = p.ServiceType,
                                                      Profit = p.Profit,
                                                      UnitId = p.UnitId
                                                  }).FirstOrDefaultAsync();
                        }
                    return price_detail;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("RemoveByID - PriceDetailDAL: " + ex);
            }
            return null;
        }
        public async Task<List<PriceDetail>> GetListPriceDetailByProductRoomServiceId(List<int> packages_id, int service_type)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return await _DbContext.PriceDetail.Where(x => packages_id.Contains(x.ProductServiceId) && x.ServiceType==service_type).ToListAsync();
                   
                
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetListPriceDetailByListPackagesId - PriceDetailDAL: " + ex);
            }
            return null;
        }
    }
}
