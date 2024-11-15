using Aspose.Cells;
using DAL;
using DAL.Programs;
using Entities.ConfigModels;
using Entities.Models;
using Entities.ViewModels;
using Entities.ViewModels.PricePolicy;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace Repositories.Repositories
{
    public class CampaignRepository : ICampaignRepository
    {
        private readonly CampaignDAL _campaignDAL;
        private readonly ProductRoomServiceDAL _productRoomServiceDAL;
        private readonly PriceDetailDAL _priceDetailDAL;
        private readonly ProgramsDAL _programsDAL;
        private readonly ProgramsPackageDAL _programsPackageDAL;
        private readonly HotelDAL _hotelDAL;


        public CampaignRepository(IOptions<DataBaseConfig> dataBaseConfig, IOptions<MailConfig> mailConfig)
        {
            _campaignDAL = new CampaignDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            _productRoomServiceDAL = new ProductRoomServiceDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            _priceDetailDAL = new PriceDetailDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            _programsDAL = new ProgramsDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            _hotelDAL = new HotelDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            _programsPackageDAL = new ProgramsPackageDAL(dataBaseConfig.Value.SqlServer.ConnectionString);

        }
        public GenericViewModel<PricePolicyListingModel> GetPagingList(PricePolicySearchModel searchModel, int currentPage, int pageSize)
        {
            var model = new GenericViewModel<PricePolicyListingModel>();
            try
            {
                DataTable dt = _campaignDAL.GetPagingList(searchModel, currentPage, pageSize);
                if (dt != null && dt.Rows.Count > 0)
                {
                    model.ListData = dt.ToList<PricePolicyListingModel>();

                    model.CurrentPage = currentPage;
                    model.PageSize = pageSize;
                    model.TotalRecord = Convert.ToInt32(dt.Rows[0]["TotalRow"]);
                    model.TotalPage = (int)Math.Ceiling((double)model.TotalRecord / pageSize);
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetPagingList - CampaignRepository: " + ex);
            }
            return model;
        }

        public async Task<Campaign> GetDetailByCode(string campaign_code)
        {
            return await _campaignDAL.GetDetailByCode(campaign_code);
        }
        public async Task<Campaign> GetDetailByID(int id)
        {
            return await _campaignDAL.GetDetailByID(id);
        }
        public async Task<int> AddNew(Campaign campaign)
        {
            return await _campaignDAL.AddNew(campaign);
        }

        public async Task<int> Update(Campaign campaign)
        {
            return await _campaignDAL.Update(campaign);
        }

        public int Delete(int campaignId)
        {
            return _campaignDAL.Delete(campaignId);
        }



        public async Task<HotelPricePolicyCampaignModel> GetPolicyDetailViewByCampaignID(int campaign_id, int hotel_id, DateTime? from_date, DateTime? to_date)
        {
            HotelPricePolicyCampaignModel result = new HotelPricePolicyCampaignModel()
            {
                Detail = new Campaign()
                {
                    Id = -1,
                    FromDate = from_date?? new DateTime(DateTime.Now.Year, 01, 01, 00, 00, 00),
                    ToDate = to_date?? new DateTime(DateTime.Now.Year + 1, 01, 01, 00, 00, 00),

                },
                PricePolicy = new List<HotelPricePolicyDetail>(),
                BasedProgram=new List<HotelPricePolicyDetail>()
                

            };
            try
            {

                DataTable dt_daily = await _programsDAL.GetHotelPricePolicyDailyByPrograms(hotel_id);
                if (dt_daily != null && dt_daily.Rows.Count > 0)
                {
                    result.BasedProgram = dt_daily.ToList<HotelPricePolicyDetail>();
                }

                result.Detail = await _campaignDAL.GetDetailByID(campaign_id);

                DataTable dt_price_daily = await _campaignDAL.GetHotelPricePolicyDailyByCampaignID(campaign_id);
                if (dt_price_daily != null && dt_price_daily.Rows.Count > 0)
                {
                    result.PricePolicy = dt_price_daily.ToList<HotelPricePolicyDetail>();
                }

                DataTable dt_2 = await _programsDAL.GetHotelPricePolicyByPrograms(hotel_id);
                if (dt_2 != null && dt_2.Rows.Count > 0)
                {
                    var list = dt_2.ToList<HotelPricePolicyDetail>();
                    var existed_program_id = result.BasedProgram.Select(x => new { x.ProgramId ,x.PackageId,x.RoomId});
                    foreach(var item in list)
                    {
                        if(!existed_program_id.Contains(new { item.ProgramId, item.PackageId, item.RoomId }))
                            {
                            result.BasedProgram.Add(item);
                        }
                    }
                   
                }
                DataTable dt_price = await _campaignDAL.GetHotelPricePolicyByCampaignID(campaign_id);
                if (dt_price != null && dt_price.Rows.Count > 0)
                {
                    var price_policy = dt_price.ToList<HotelPricePolicyDetail>();
                    var existed_price_policy_id = result.PricePolicy.Select(x => new { x.ProgramId, x.PackageId, x.RoomId });
                    foreach (var item in price_policy)
                    {
                        if (!existed_price_policy_id.Contains(new { item.ProgramId, item.PackageId, item.RoomId }))
                        {
                            result.PricePolicy.Add(item);
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetPolicyDetailViewByCampaignID - CampaignRepository: " + ex);
            }
            return result;
        }
        public async Task<ProductRoomService> GetFirstProductServiceRoombyCampaignID(int campaign_id)
        {
            var list = await _productRoomServiceDAL.GetByCampaignID(campaign_id);
            if (list != null && list.Count > 0)
            {
                return list.First();
            }
            return null;
        }
        public async Task<int> CreateOrUpdateHotelCampaign(HotelPricePolicyCampaignModel detail, int user_id)
        {
            int result = -1;
            try
            {
                if (detail.PricePolicy == null || detail.PricePolicy.Count <= 0) return result;
                detail.Detail.Description = detail.Detail.Description == null ? "" : detail.Detail.Description;
                if (detail.Detail.Id <= 0)
                {
                    detail.Detail.UserCreateId = user_id;
                    detail.Detail.UserUpdateId = user_id;
                    detail.Detail.CreateDate = DateTime.Now;
                    detail.Detail.UpdateLast = DateTime.Now;
                    var id = await _campaignDAL.AddNew(detail.Detail);
                }
                else
                {
                    detail.Detail.UserUpdateId = user_id;
                    detail.Detail.UpdateLast = DateTime.Now;
                    var id = await _campaignDAL.Update(detail.Detail);
                }
                List<HotelPricePolicySummitModel> summit = new List<HotelPricePolicySummitModel>();
                foreach (var policy in detail.PricePolicy)
                {
                    if (summit.Any(x => x.Detail.Id == policy.ProductServiceId))
                    {
                        summit.First(x => x.Detail.Id == policy.ProductServiceId).PriceDetail.Add(new PriceDetail()
                        {
                            Id = policy.PriceDetailId,
                            AmountLast = policy.Profit,
                            DayList = null,
                            FromDate = policy.FromDate,
                            Profit = policy.Profit,
                            MonthList = null,
                            Price = 0,
                            ProductServiceId = policy.ProductServiceId,
                            ServiceType = (int)ServiceType.BOOK_HOTEL_ROOM_VIN,
                            ToDate = policy.ToDate,
                            UnitId = policy.UnitId,
                            UserCreateId = user_id,
                            UserUpdateId = user_id
                        });
                    }
                    else
                    {
                        summit.Add(new HotelPricePolicySummitModel()
                        {
                            Detail = new ProductRoomService()
                            {
                                AllotmentsId = policy.AllotmentsId,
                                CampaignId = detail.Detail.Id,
                                HotelId = policy.HotelId,
                                Id = policy.ProductServiceId,
                                PackageCode = policy.PackageCode,
                                ProgramId = policy.ProgramId,
                                ProgramPackageId = policy.PackageId,
                                RoomId = policy.RoomId
                            },
                            PriceDetail = new List<PriceDetail>()
                            {
                                new PriceDetail()
                                {
                                    Id=policy.PriceDetailId,
                                    AmountLast=policy.Profit,
                                    DayList=null,
                                    FromDate=policy.FromDate,
                                    Profit=policy.Profit,
                                    MonthList=null,
                                    Price=0,
                                    ProductServiceId=policy.ProductServiceId,
                                    ServiceType=(int)ServiceType.BOOK_HOTEL_ROOM_VIN,
                                    ToDate=policy.ToDate,
                                    UnitId=policy.UnitId,
                                     UserCreateId=user_id,
                                     UserUpdateId=user_id
                                }
                            }
                        });
                    }


                }
                List<int> product_room_services = new List<int>();
                if (summit.Count > 0)
                {
                    foreach (var data in summit)
                    {
                        var id = await _productRoomServiceDAL.AddorUpdatePriceRoomPolicy(data);
                        product_room_services.Add(id);
                    }
                }
                await _productRoomServiceDAL.RemoveNonExistsProductServiceRoom(detail.Detail.Id, product_room_services);
                result = (int)ResponseType.SUCCESS;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CreateOrUpdateHotelCampaign - CampaignRepository: " + ex);
            }
            return result;
        }
        public async Task<int> CreateOrUpdateHotelPriceDetail(HotelPricePolicyCampaignModel detail, int user_id)
        {
            int result = -1;
            try
            {
                if (detail.PricePolicy == null || detail.PricePolicy.Count <= 0) return result;
                detail.Detail.ContractType = (int)ServicesType.VINHotelRent;
                if (detail.Detail.Description == null) detail.Detail.Description = "";
                if (detail.Detail.Id <= 0)
                {
                    return 0;
                }
                if (detail.PricePolicy.FirstOrDefault() == null) return result;
                foreach (var policy in detail.PricePolicy)
                {
                    var summit = new HotelPricePolicySummitModel()
                    {
                        Detail = new ProductRoomService()
                        {
                            AllotmentsId = policy.AllotmentsId,
                            CampaignId = detail.Detail.Id,
                            HotelId = policy.HotelId,
                            Id = policy.ProductServiceId,
                            PackageCode = policy.PackageCode,
                            ProgramId = policy.ProgramId,
                            ProgramPackageId = policy.PackageId,
                            RoomId = policy.RoomId
                        },
                        PriceDetail = new List<PriceDetail>()
                                {
                                    new PriceDetail()
                                    {
                                        Id=policy.PriceDetailId,
                                        AmountLast=policy.Profit,
                                        DayList=null,
                                        FromDate=policy.FromDate,
                                        Profit=policy.Profit,
                                        MonthList=null,
                                        Price=0,
                                        ProductServiceId=policy.ProductServiceId,
                                        ServiceType=(int)ServiceType.BOOK_HOTEL_ROOM_VIN,
                                        ToDate=policy.ToDate,
                                        UnitId=policy.UnitId,
                                         UserCreateId=user_id,
                                         UserUpdateId=user_id
                                    }
                                }
                    };
                    var summit_id = await _productRoomServiceDAL.AddorUpdatePriceDetail(summit);
                }

                result = (int)ResponseType.SUCCESS;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CreateOrUpdateHotelCampaign - CampaignRepository: " + ex);
            }
            return result;
        }
        public async Task<string> ExportCampaignExcel(GenericViewModel<PricePolicyListingModel> model, string file_path)
        {

            try
            {
                string full_path = Directory.GetCurrentDirectory() + file_path;

                try
                {
                    File.Delete(full_path);
                }
                catch { }
                if (model != null && model.ListData.Count > 0)
                {
                    Workbook wb = new Workbook();
                    Worksheet ws = wb.Worksheets[0];
                    ws.Name = "Chính sách giá";
                    Cells cell = ws.Cells;

                    var range = ws.Cells.CreateRange(0, 0, 1, 1);
                    StyleFlag st = new StyleFlag();
                    st.All = true;
                    Style style = ws.Cells["A1"].GetStyle();

                    #region Header
                    range = cell.CreateRange(0, 0, 2, 9);
                    style = ws.Cells["A1"].GetStyle();
                    style.Font.IsBold = true;
                    style.IsTextWrapped = true;
                    //style.ForegroundColor = Color.FromArgb(33, 88, 103);
                    //style.BackgroundColor = Color.FromArgb(33, 88, 103);
                    style.Pattern = BackgroundType.Solid;
                    //style.Font.Color = Color.White;
                    style.VerticalAlignment = TextAlignmentType.Center;
                    style.HorizontalAlignment = TextAlignmentType.Center;
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
                    cell.SetColumnWidth(1, 50);
                    cell.SetColumnWidth(2, 50);
                    cell.SetColumnWidth(3, 30);
                    cell.SetColumnWidth(4, 30);
                    cell.SetColumnWidth(5, 30);
                    cell.SetColumnWidth(6, 30);
                    cell.SetColumnWidth(7, 30);
                    cell.SetColumnWidth(8, 30);


                    // Set header value
                   

                    ws.Cells["A2"].PutValue("STT");
                    ws.Cells["B2"].PutValue("Mã chiến dịch");
                    ws.Cells["C2"].PutValue("Dịch vụ");
                    ws.Cells["D2"].PutValue("Nhà cung cấp");
                    ws.Cells["E2"].PutValue("Từ ngày");
                    ws.Cells["F2"].PutValue("Đến ngày");
                    ws.Cells["G2"].PutValue("Trạng thái");
                    ws.Cells["H2"].PutValue("Người tạo");
                    ws.Cells["I2"].PutValue("Thời gian tạo");

                    cell.Merge(0, 0, 2, 1);
                    cell.Merge(0, 1, 2, 1);
                    cell.Merge(0, 2, 2, 1);
                    cell.Merge(0, 3, 2, 1);
                    cell.Merge(0, 4, 2, 1);
                    cell.Merge(0, 5, 2, 1);
                    cell.Merge(0, 6, 2, 1);
                    cell.Merge(0, 7, 2, 1);
                    cell.Merge(0, 8, 2, 1);

                    #endregion

                    #region Body

                    range = cell.CreateRange(1, 0, model.ListData.Count + 1, 9);
                    style = ws.Cells["A3"].GetStyle();
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

                    Style alignCenterStyle = ws.Cells["A3"].GetStyle();
                    alignCenterStyle.HorizontalAlignment = TextAlignmentType.Center;

                    Style alignRightStyle = ws.Cells["A3"].GetStyle();
                    alignCenterStyle.HorizontalAlignment = TextAlignmentType.Right;
                    alignCenterStyle.VerticalAlignment = TextAlignmentType.Center;

                    Style numberStyle = ws.Cells["A3"].GetStyle();
                    numberStyle.Number = 3;
                    numberStyle.HorizontalAlignment = TextAlignmentType.Right;
                    numberStyle.VerticalAlignment = TextAlignmentType.Center;

                    int RowIndex = 2;

                    foreach (var item in model.ListData)
                    {

                        RowIndex++;
                        ws.Cells["A" + RowIndex].PutValue(RowIndex - 2);
                        ws.Cells["A" + RowIndex].SetStyle(alignCenterStyle);

                        ws.Cells["B" + RowIndex].PutValue(item.CampaignCode);
                        //Replace:
                        ws.Cells["C" + RowIndex].PutValue(item.CampaignDescription);
                        ws.Cells["D" + RowIndex].PutValue(item.ProviderName);
                        ws.Cells["E" + RowIndex].PutValue(item.FromDate == DateTime.MinValue ? "" : item.FromDate.ToString("dd/MM/yyyy HH:mm"));
                        ws.Cells["F" + RowIndex].PutValue(item.ToDate == DateTime.MinValue ? "" : item.ToDate.ToString("dd/MM/yyyy HH:mm"));


                        ws.Cells["G" + RowIndex].PutValue(item.StatusName);

                        ws.Cells["H" + RowIndex].PutValue(item.FullName);
                        ws.Cells["I" + RowIndex].PutValue(item.CreateDate.ToString("dd/MM/yyyy HH:mm"));

                    }

                    #endregion
                    wb.Save(full_path);
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ExportCampaignExcel - CampaignRepository: " + ex);
            }
            return file_path;
        }
    }
}
