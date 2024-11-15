using DAL.Generic;
using DAL.StoreProcedure;
using Entities.Models;
using System.Data;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using Utilities;
using Microsoft.Data.SqlClient;
using Entities.ViewModels.PricePolicy;
using Utilities.Contants;

namespace DAL
{
    public class ProductFlyTicketServiceDAL : GenericService<ProductRoomService>
    {
        private static DbWorker _DbWorker;
        public ProductFlyTicketServiceDAL(string connection) : base(connection)
        {
            _DbWorker = new DbWorker(connection);
        }
        public ProductFlyTicketService GetByCampaignID(int campaign_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var flyTicket = _DbContext.ProductFlyTicketService.FirstOrDefault(x => x.CampaignId == campaign_id);
                    return flyTicket;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetByCampaignID - ProductFlyTicketServiceDAL: " + ex);
            }
            return null;
        }
        public int AddOrUpdateCampaginAndProduct(PricePolicySummitModel pricePolicyModel, int userId)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    //save Campaign
                    Campaign campaign = _DbContext.Campaign.FirstOrDefault(n => n.CampaignCode == pricePolicyModel.CampaignCode);
                    if (campaign != null && campaign.Id > 0)
                    {
                        campaign.FromDate = pricePolicyModel.FromDate;
                        campaign.ToDate = pricePolicyModel.ToDate;
                        campaign.ClientTypeId = pricePolicyModel.ClientTypeId;
                        campaign.Description = pricePolicyModel.Description;
                        campaign.Status = pricePolicyModel.Status;
                        campaign.UpdateLast = DateTime.Now;
                        campaign.UserUpdateId = userId;
                        campaign.ContractType = (int)ServiceType.PRODUCT_FLY_TICKET;
                        var result = _DbContext.Campaign.Update(campaign).Entity;
                    }
                    else
                    {
                        campaign = new Campaign();
                        campaign.CampaignCode = pricePolicyModel.CampaignCode;
                        campaign.UserCreateId = userId;
                        campaign.CreateDate = DateTime.Now;
                        campaign.FromDate = pricePolicyModel.FromDate;
                        campaign.ToDate = pricePolicyModel.ToDate;
                        campaign.ClientTypeId = pricePolicyModel.ClientTypeId;
                        campaign.Description = pricePolicyModel.Description;
                        campaign.Status = pricePolicyModel.Status;
                        campaign.UpdateLast = DateTime.Now;
                        campaign.ContractType = (int)ServiceType.PRODUCT_FLY_TICKET;
                        campaign.UserUpdateId = userId;
                        var result = _DbContext.Campaign.Add(campaign);
                    }
                    _DbContext.SaveChanges();

                    //save ProductFlyTicketService
                    var flyTickerService = _DbContext.ProductFlyTicketService.FirstOrDefault(n => n.CampaignId == campaign.Id);
                    var groupProviderType = _DbContext.AllCode.FirstOrDefault(n => n.Type == AllCodeType.GROUP_PROVIDER_TYPE
                    && n.CodeValue == 3);
                    int flyTickerServiceId = 0;
                    if (flyTickerService != null)
                    {
                        flyTickerService.CampaignId = campaign.Id;
                        flyTickerService.GroupProviderType = groupProviderType != null ? groupProviderType.CodeValue.ToString() : "0";
                        _DbContext.ProductFlyTicketService.Update(flyTickerService);
                    }
                    else
                    {
                        flyTickerService = new ProductFlyTicketService();
                        flyTickerService.CampaignId = campaign.Id;
                        flyTickerService.GroupProviderType = groupProviderType != null ? groupProviderType.CodeValue.ToString() : "0";
                        var result = _DbContext.ProductFlyTicketService.Add(flyTickerService);
                    }
                    _DbContext.SaveChanges();
                    flyTickerServiceId = flyTickerService.Id;
                    //save ProductDetail
                    PriceDetail priceDetail = _DbContext.PriceDetail.FirstOrDefault(n => n.ProductServiceId == flyTickerServiceId);
                    if (priceDetail == null)
                    {
                        priceDetail = new PriceDetail();
                    }
                    priceDetail.ProductServiceId = flyTickerServiceId;
                    priceDetail.FromDate = pricePolicyModel.FromDate;
                    priceDetail.ToDate = pricePolicyModel.ToDate;
                    priceDetail.Profit = pricePolicyModel.ServiceFee;
                    priceDetail.UnitId = pricePolicyModel.ServiceFeeType;
                    priceDetail.UserCreateId = userId;
                    priceDetail.ServiceType = (int)ServicesType.FlyingTicket;
                    priceDetail.DayList = pricePolicyModel.DaysString;
                    priceDetail.MonthList = string.Join(",", pricePolicyModel.Months.
                        Select(n =>
                        {
                            return n.Replace("Tháng ", "");
                        }));
                    if (priceDetail != null)
                    {
                        _DbContext.PriceDetail.Update(priceDetail);
                    }
                    else
                    {
                        _DbContext.PriceDetail.Add(priceDetail);
                    }
                    _DbContext.SaveChanges();
                    return campaign.Id;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("AddOrUpdateCampaginAndProduct - ProductFlyTicketServiceDAL: " + ex);
            }
            return -1;
        }
        public int Update(ProductFlyTicketService model)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var add = _DbContext.ProductFlyTicketService.Update(model);
                    _DbContext.SaveChangesAsync();
                    return model.Id;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Update - ProductFlyTicketServiceDAL: " + ex);
                return -1;
            }
        }
    }
}
