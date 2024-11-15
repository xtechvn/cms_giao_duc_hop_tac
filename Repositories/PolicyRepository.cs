using DAL;
using DAL.StoreProcedure;
using Entities.ConfigModels;
using Entities.Models;
using Entities.ViewModels;
using Entities.ViewModels.Policy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using OfficeOpenXml;
using Repositories.IRepositories;
using Repositories.Repositories.BaseRepos;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Repositories
{
    public class PolicyRepository : BaseRepository, IPolicyRepository
    {

        private readonly PolicyDAL _PolicyDAL;
        private readonly PolicyDetailDAL _PolicyDetailDAL;

        public PolicyRepository(IHttpContextAccessor context, IOptions<DataBaseConfig> dataBaseConfig, IUserRepository userRepository, IConfiguration configuration) : base(context, dataBaseConfig, configuration, userRepository)
        {
            _PolicyDAL = new PolicyDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            _PolicyDetailDAL = new PolicyDetailDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
        }

        public async Task<GenericViewModel<PolicyViewModel>> GetPagingList(PolicySearchViewModel searchModel, int currentPage, int pageSize)
        {
            var model = new GenericViewModel<PolicyViewModel>();
            try
            {

                DataTable dt = await _PolicyDAL.GetPagingList(searchModel, currentPage, pageSize, ProcedureConstants.SP_GetListPolicy);
                if (dt != null && dt.Rows.Count > 0)
                {
                    var data = (from row in dt.AsEnumerable()
                                select new PolicyViewModel
                                {

                                    PolicyName = !row["PolicyName"].Equals(DBNull.Value) ? row["PolicyName"].ToString() : "",
                                    EffectiveDate = !row["EffectiveDate"].Equals(DBNull.Value) ? row["EffectiveDate"].ToString() : "",
                                    PermissionType_Name = !row["PermissionTypeName"].Equals(DBNull.Value) ? row["PermissionTypeName"].ToString() : "",
                                    Create_Name = !row["FullName"].Equals(DBNull.Value) ? row["FullName"].ToString() : "",
                                    CreatedDate = !row["CreatedDate"].Equals(DBNull.Value) ? row["CreatedDate"].ToString() : "",
                                    PolicyId = Convert.ToInt32(!row["PolicyId"].Equals(DBNull.Value) ? row["PolicyId"].ToString() : ""),
                                    PolicyCode = !row["PolicyCode"].Equals(DBNull.Value) ? row["PolicyCode"].ToString() : "",

                                }).ToList();


                    model.ListData = data;
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
                LogHelper.InsertLogTelegram("GetPagingList - PolicyRepository: " + ex);
                return null;
            }
        }
        public async Task<int> CreatePolicy(AddPolicyDtailViewModel data)
        {
            try
            {
                var DataModel = new Policy();
                DataModel.CreatedBy = data.CreatedBy;
                DataModel.CreatedDate = DateTime.Now;
                DataModel.IsPrivate = false;
                DataModel.PermissionType = data.PermissionType;
                DataModel.PolicyName = data.PolicyName;
                DataModel.PolicyCode = data.PolicyCode;
                DataModel.EffectiveDate = Convert.ToDateTime(data.EffectiveDate);
                var poliId = await _PolicyDAL.CreatePolicy(DataModel);
                var a = await _PolicyDetailDAL.InsertPolicyDetail(data, poliId);
                return 1;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CreatePolicy - PolicyRepository: " + ex);
                return 0;
            }

        }
        public async Task<int> updatePolicy(AddPolicyDtailViewModel data)
        {
            try
            {
                var PolicyDetail = await _PolicyDAL.DetailPolicy(Convert.ToInt32(data.PolicyId));
                var PolicyDetail2 = PolicyDetail.ToList<PolicyDtailViewModel>();
                var DataModel = new Policy();
                DataModel.UpdatedBy = data.CreatedBy;
                DataModel.CreatedDate = Convert.ToDateTime(PolicyDetail2[0].CreatedDate);
                DataModel.PolicyCode = PolicyDetail2[0].PolicyCode;
                DataModel.IsPrivate = false;
                DataModel.PermissionType = data.PermissionType;
                DataModel.PolicyName = data.PolicyName;
                DataModel.EffectiveDate = Convert.ToDateTime(data.EffectiveDate);
                DataModel.PolicyId = data.PolicyId;

                if (data.extra_policy != null)
                {
                    foreach (var item in data.extra_policy)
                    {
                        var DataModel2 = new PolicyDetail();
                        DataModel2.Id = item.Id;
                        DataModel2.PolicyId = data.PolicyId;
                        DataModel2.ClientType = Convert.ToInt32(item.ClientType);
                        DataModel2.DebtType = Convert.ToInt32(item.DebtType);
                        DataModel2.ProductFlyTicketDebtAmount = Convert.ToInt32(item.ProductFlyTicketDebtAmount);
                        DataModel2.HotelDebtAmout = Convert.ToInt32(item.HotelDebtAmout);
                        DataModel2.ProductFlyTicketDepositAmount = Convert.ToInt32(item.ProductFlyTicketDepositAmount);
                        DataModel2.HotelDepositAmout = Convert.ToInt32(item.HotelDepositAmout);

                        DataModel2.TourDebtAmount = Convert.ToInt32(item.TourDebtAmount);
                        DataModel2.TourDepositAmount = Convert.ToInt32(item.TourDepositAmount);
                        DataModel2.TouringCarDebtAmount = Convert.ToInt32(item.TouringCarDebtAmount);
                        DataModel2.TouringCarDepositAmount = Convert.ToInt32(item.TouringCarDepositAmount);
                        DataModel2.VinWonderDebtAmount = Convert.ToInt32(item.VinWonderDebtAmount);
                        DataModel2.VinWonderDepositAmount = Convert.ToInt32(item.VinWonderDepositAmount);
                        DataModel2.UpdatedBy = data.CreatedBy;
                        var a = await _PolicyDetailDAL.UpdatePolicyDetail(DataModel2);
                        if (a == 0 || a == -1)
                        {
                            return 0;
                        }
                    }
                }

                var poliId = await _PolicyDAL.UpdatatPolicy(DataModel);

                return 1;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CreatePolicy - PolicyRepository: " + ex);
                return 0;
            }

        }
        public PolicyDtailViewModel GetPolicyDetail(long PolicyId)
        {
            try
            {

                var data = _PolicyDAL.GetPolicyDetail(PolicyId).ToList<PolicyDtailViewModel>();
                return data[0];
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetPolicyDetail - PolicyRepository: " + ex);
                return null;
            }

        }
        public async Task<List<PolicyDtailViewModel>> DetailPolicy(long PolicyId)
        {
            try
            {
                var data = await _PolicyDAL.DetailPolicy(PolicyId);
                return data.ToList<PolicyDtailViewModel>();
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("DetailPolicy - PolicyDal: " + ex);
            }
            return null;
        }
        public async Task<int> UpdatePolicy(Policy model)
        {
            try
            {
                return await _PolicyDAL.UpdatatPolicy(model);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("DetailPolicy - PolicyDal: " + ex);
            }
            return 0;
        }
        public async Task<int> UpdatePolicyDetail(PolicyDetail model)
        {
            try
            {
                return await _PolicyDetailDAL.UpdatePolicyDetail(model);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdatePolicyDetail - PolicyDal: " + ex);
            }
            return 0;
        }
        public async Task<int> DeletePolicy(int PolicyId)
        {
            try
            {
                return await _PolicyDAL.DeletePolicy(PolicyId);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("DetailPolicy - PolicyDal: " + ex);
            }
            return 0;
        }

        #region vin wonder
        public int CreateCampaign(CampaignModel model)
        {
            try
            {
                model.CampaignCode = $"VIN_WONDER_{model.FromDate.ToString("dd/MM/yyyy")}_{model.ToDate.ToString("dd/MM/yyyy")}";
                model.UserAction = _SysUserModel.Id;
                return _PolicyDAL.CreateCampaign(model);
            }
            catch
            {
                throw;
            }
        }

        public int UpdateCampaign(CampaignModel model)
        {
            try
            {
                model.CampaignCode = $"VIN_WONDER_{model.FromDate.ToString("dd/MM/yyyy")}_{model.ToDate.ToString("dd/MM/yyyy")}";
                model.UserAction = _SysUserModel.Id;
                return _PolicyDAL.UpdateCampaign(model);
            }
            catch
            {
                throw;
            }
        }

        public int InsertVinWonderPricePolicy(VinWonderPricePolicyModel model)
        {
            try
            {
                model.UserAction = _SysUserModel.Id;
                return _PolicyDAL.InsertVinWonderPricePolicy(model);
            }
            catch
            {
                throw;
            }
        }

        public int UpdateVinWonderPricePolicy(VinWonderPricePolicyModel model)
        {
            try
            {
                return _PolicyDAL.UpdateVinWonderPricePolicy(model);
            }
            catch
            {
                throw;
            }
        }

        public void UpSertVinWonderPricePolicy(int campaign_id, IEnumerable<VinWonderPricePolicyModel> models)
        {
            try
            {
                _PolicyDAL.UpsertVinWonderPricePolicy(campaign_id, models, _SysUserModel.Id);
            }
            catch
            {
                throw;
            }
        }

        public int UpdateVinWonderCommonProfit(VinWonderCommonProfitModel model)
        {
            try
            {
                model.UserAction = _SysUserModel.Id;
                return _PolicyDAL.UpdateVinWonderCommonProfit(model);
            }
            catch
            {
                throw;
            }
        }

        public IEnumerable<VinWonderPricePolicyModel> GetVinWonderPricePolicyByCampaignId(int campaignId)
        {
            try
            {
                var dataTable = _PolicyDAL.GetVinWonderPricePolicyByCampaignId(campaignId);
                return dataTable.ToList<VinWonderPricePolicyModel>();
            }
            catch
            {
                throw;
            }
        }

        public IEnumerable<VinWonderPricePolicyModel> GetVinWonderPricePolicyByServiceId(int serviceId)
        {
            try
            {
                var dataTable = _PolicyDAL.GetVinWonderPricePolicyByServiceId(serviceId);
                return dataTable.ToList<VinWonderPricePolicyModel>();
            }
            catch
            {
                throw;
            }
        }

        public async Task<IEnumerable<VinWonderPricePolicyModel>> ImportExcelAsync(IFormFile file)
        {
            try
            {
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet ws = package.Workbook.Worksheets.FirstOrDefault();

                        var endRow = ws.Cells.End.Row;
                        var startRow = 2;

                        var data_list = new List<VinWonderPricePolicyModel>();

                        for (int row = startRow; row <= endRow; row++)
                        {
                            var cellRange = ws.Cells[row, 1, row, 11];
                            var isRowEmpty = cellRange.All(c => c.Value == null);
                            if (isRowEmpty)
                            {
                                break;
                            }

                            var data_site_id = ws.Cells[row, 2].Value;
                            var data_site_name = ws.Cells[row, 3].Value;
                            var data_rate_code = ws.Cells[row, 4].Value;
                            var data_service_id = ws.Cells[row, 5].Value;
                            var data_service_code = ws.Cells[row, 6].Value;
                            var data_name = ws.Cells[row, 7].Value;
                            var data_base_price = ws.Cells[row, 9].Value;
                            var data_weekend_rate = ws.Cells[row, 10].Value;

                            var data = new VinWonderPricePolicyModel
                            {
                                SiteId = int.TryParse((data_site_id ?? string.Empty).ToString(), out int site_id) ? site_id : 0,
                                SiteName = data_site_name.ToString(),
                                RateCode = int.TryParse((data_rate_code ?? string.Empty).ToString(), out int rate) ? rate : 0,
                                ServiceId = int.TryParse((data_service_id ?? string.Empty).ToString(), out int service) ? service : 0,
                                ServiceCode = (data_service_code ?? string.Empty).ToString(),
                                Name = (data_name ?? string.Empty).ToString(),
                                BasePrice = decimal.TryParse((data_base_price ?? string.Empty).ToString(), out decimal base_price) ? base_price : 0,
                                WeekendRate = decimal.TryParse((data_weekend_rate ?? string.Empty).ToString(), out decimal wk_price) ? wk_price : 0
                            };

                            data_list.Add(data);
                        }

                        return data_list;
                    }
                }
            }
            catch
            {
                throw;
            }
        }


        #endregion
    }
}
