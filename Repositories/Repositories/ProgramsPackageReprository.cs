using DAL.Programs;
using Entities.ConfigModels;
using Entities.Models;
using Entities.ViewModels;
using Entities.ViewModels.Programs;
using Microsoft.Extensions.Options;
using Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace Repositories.Repositories
{
    public class ProgramsPackageReprository : IProgramsPackageReprository
    {
        private readonly ProgramsPackageDAL _programsPackageDAL;
        private readonly ProgramsDAL _programsDAL;
        private readonly ProgramPackageDailyDAL _programPackageDailyDAL;
        public ProgramsPackageReprository(IOptions<DataBaseConfig> dataBaseConfig)
        {
            _programsPackageDAL = new ProgramsPackageDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            _programsDAL = new ProgramsDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            _programPackageDailyDAL = new ProgramPackageDailyDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
        }

        public async Task<GenericViewModel<ProgramsPackageModel>> ListProgramPackage(ProgramsPackageSearchViewModel searchModel)
        {
            var model = new GenericViewModel<ProgramsPackageModel>();
            int TotalRow = 0;
            var PageIndex = searchModel.PageIndex;
            try
            {
                var ProgramsPackage = new List<ProgramsPackageModel>();

                var listProgram = new List<Programs>();
                if (searchModel.ProgramId != null)
                {
                    listProgram = _programsDAL.GetProgramsbyProgramId(Convert.ToInt32(searchModel.ProgramId), searchModel.PageIndex == -1 ? 1 : searchModel.PageIndex, searchModel.PageSize);
                }
                else
                {
                    listProgram = _programsDAL.GetAll(searchModel.PageIndex, searchModel.PageSize);
                }
                if (listProgram != null)
                {
                    TotalRow = listProgram.Count;
                    foreach (var item in listProgram)
                    {

                        searchModel.ProgramId = item.Id.ToString();
                        searchModel.PageIndex = -1;
                        DataTable dt = await _programsPackageDAL.GetPagingList(searchModel);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            var data = dt.ToList<ProgramsPackageViewModel>();
                            var data3 = data.GroupBy(s => new { s.PackageCode }).Select(i => i.First()).ToList();

                            foreach (var i in data3)
                            {
                                var ProgramsPackageDetail = new ProgramsPackageModel();
                                ProgramsPackageDetail.Id = item.Id;
                                ProgramsPackageDetail.ProgramsPackageId = i.id;
                                ProgramsPackageDetail.Description = item.Description;
                                ProgramsPackageDetail.ProgramName = item.ProgramName;
                                ProgramsPackageDetail.ProgramName = i.PackageCode;
                                var data2 = data.Where(s => s.PackageCode == i.PackageCode).GroupBy(s => new { s.Price, s.RoomType, s.FromDate }).Select(i => i.First()).ToList();
                                foreach (var item2 in data2)
                                {
                                    var listWeekDay = "";
                                    var listWeekDay2 = data.Where(s => s.Price == item2.Price && s.RoomType == item2.RoomType && s.PackageCode == i.PackageCode).Select(s => s.WeekDay).Distinct().ToList();
                                    foreach (var item3 in listWeekDay2)
                                    {
                                        if (item3 == "0")
                                        {
                                            string WeekDayCN = "CN, ";
                                            listWeekDay += WeekDayCN;
                                        }
                                        else
                                        {
                                            string WeekDay = "Thứ " + item3 + ", ";
                                            listWeekDay += WeekDay;
                                        }

                                    }
                                    item2.WeekDay = listWeekDay;
                                }
                                ProgramsPackageDetail.ProgramsPackage = data2;
                                ProgramsPackageDetail.FromDate = ProgramsPackageDetail.ProgramsPackage[0].FromDate;
                                ProgramsPackageDetail.ToDate = ProgramsPackageDetail.ProgramsPackage[0].ToDate;
                                ProgramsPackageDetail.SupplierId = item.SupplierId;
                                ProgramsPackage.Add(ProgramsPackageDetail);
                            }
                        }

                    }
                }
                model.ListData = ProgramsPackage;
                model.CurrentPage = PageIndex;
                model.PageSize = searchModel.PageSize;
                model.TotalRecord = TotalRow;
                model.TotalPage = (int)Math.Ceiling((double)model.TotalRecord / model.PageSize);
                return model;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ListProgramPackage- ProgramsPackageReprository: " + ex);
            }
            return null;
        }
        public async Task<GenericViewModel<ProgramsPackagePriceViewModel>> ListProgramPackagePrice(ProgramsPackageSearchViewModel searchModel)
        {
            var model = new GenericViewModel<ProgramsPackagePriceViewModel>();
            int TotalRow = 0;
            try
            {
                var ProgramsPackage = new List<ProgramsPackagePriceViewModel>();
                var listProgram = new List<Programs>();
                if (searchModel.ProgramId != null)
                {
                    listProgram = _programsDAL.GetProgramsbyProgramId(Convert.ToInt32(searchModel.ProgramId), searchModel.PageIndex, searchModel.PageSize);
                }
                else
                {
                    listProgram = _programsDAL.GetAll(searchModel.PageIndex, searchModel.PageSize);
                }
                var PageIndex = searchModel.PageIndex;
                if (listProgram != null)
                {
                    TotalRow = listProgram.Count;
                    foreach (var item in listProgram)
                    {



                        searchModel.ProgramId = item.Id.ToString();
                        searchModel.PageIndex = -1;
                        DataTable dt = await _programsPackageDAL.GetPagingList(searchModel);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            var data = dt.ToList<ProgramsPackageViewModel>();
                            var data2 = data.GroupBy(s => new { s.PackageCode, s.RoomType }).Select(i => i.First()).ToList();

                            foreach (var item3 in data2)
                            {
                                var ProgramsPackageDetail = new ProgramsPackagePriceViewModel();
                                var ProgramsPackagePrice = new List<ProgramsPackageModel>();
                                var ProgramsPackagePriceDetail = new ProgramsPackageModel();


                                ProgramsPackagePriceDetail.ProgramName = item3.PackageCode;
                                ProgramsPackagePriceDetail.RoomType = item3.RoomType;
                                ProgramsPackagePriceDetail.ProgramsPackage = data.Where(s => s.PackageCode == item3.PackageCode && s.RoomType == item3.RoomType /*&& s.OpenStatus == 0*/).ToList();

                                ProgramsPackageDetail.Id = item.Id;
                                ProgramsPackageDetail.Description = item.Description;
                                ProgramsPackagePrice.Add(ProgramsPackagePriceDetail);

                                ProgramsPackageDetail.ProgramsPackagePrice = ProgramsPackagePrice;
                                ProgramsPackageDetail.ProgramName = ProgramsPackagePrice[0].ProgramName;
                                ProgramsPackageDetail.FromDate = data[0].FromDate;
                                ProgramsPackageDetail.ToDate = data[0].ToDate;
                                ProgramsPackage.Add(ProgramsPackageDetail);
                            }

                        }

                    }
                }
                model.ListData = ProgramsPackage;
                model.CurrentPage = PageIndex;
                model.PageSize = searchModel.PageSize;
                model.TotalRecord = TotalRow;
                model.TotalPage = (int)Math.Ceiling((double)model.TotalRecord / model.PageSize);
                return model;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ListProgramPackage- ProgramsPackageReprository: " + ex);
            }
            return null;
        }
        public async Task<List<ProgramsPackageViewModel>> ListDetaiProgramPackage(long id, string PackageCode)
        {

            try
            {
                var searchModel = new ProgramsPackageSearchViewModel();
                searchModel.ProgramId = id.ToString();
                searchModel.PageIndex = -1;
                DataTable dt = await _programsPackageDAL.GetPagingList(searchModel);
                if (dt != null && dt.Rows.Count > 0)
                {
                    //var data = (from row in dt.AsEnumerable()
                    //            select new ProgramsPackageViewModel
                    //            {
                    //                id =Convert.ToInt32( row["id"].ToString()),
                    //                PackageCode = row["PackageCode"].ToString(),
                    //                RoomType = row["RoomType"].ToString(),
                    //                Amount =Convert.ToDouble(row["Amount"].ToString()),
                    //                WeekDay = row["WeekDay"].ToString(),
                    //                ApplyDate = row["ApplyDate"].ToString(),
                    //                ToDate = row["ToDate"].ToString(),
                    //                OpenStatus =Convert.ToInt32( row["OpenStatus"].ToString()),
                    //                Description = row["Description"].ToString(),
                    //            }).ToList();
                    var data = dt.ToList<ProgramsPackageViewModel>();
                    var model = data.Where(s => s.PackageCode.ToLower().Trim() == PackageCode.ToLower().Trim()).ToList();
                    return model;
                }



            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ListDetaiProgramPackage - ProgramsPackageReprository: " + ex);
            }
            return null;
        }
        public async Task<int> InsertProgramPackage(List<InsertProgramsPackageViewModel> Model, long Userid, long type2)
        {
            try
            {
                var id = -3;



                foreach (var item in Model)
                {
                    var dateWeek = DateTime.ParseExact(item.ApplyDateStr, "dd/MM/yyyy", null);
                    var Week = (int)dateWeek.DayOfWeek;
                    item.WeekDay = Week.ToString();
                    item.ApplyDate = DateTime.ParseExact(item.ApplyDateStr, "dd/MM/yyyy", null);
                    switch (Week)
                    {
                        case 0:
                            {

                                if (item.WeekDay.Contains("0"))
                                {
                                    var WeekDay2 = item.WeekDay;
                                    var item2 = item;
                                    item2.WeekDay = "0";

                                    var Insert = await _programsPackageDAL.InsertProgramPackage(item2, Userid, type2);
                                    item.WeekDay = WeekDay2;
                                    id = Insert;
                                    if (Insert <= 0)
                                    {
                                        return Insert;
                                    }
                                }


                                break;
                            }
                        case 1:
                            {

                                if (item.WeekDay.Contains("1"))
                                {
                                    var item2 = item;
                                    var WeekDay2 = item.WeekDay;
                                    item2.WeekDay = "2";

                                    var Insert = await _programsPackageDAL.InsertProgramPackage(item2, Userid, type2);
                                    item.WeekDay = WeekDay2;
                                    id = Insert;
                                    if (Insert <= 0)
                                    {
                                        return Insert;
                                    }
                                }

                                break;
                            }
                        case 2:
                            {

                                if (item.WeekDay.Contains("2"))
                                {
                                    var WeekDay2 = item.WeekDay;
                                    var item2 = item;
                                    item2.WeekDay = "3";

                                    var Insert = await _programsPackageDAL.InsertProgramPackage(item2, Userid, type2);
                                    item.WeekDay = WeekDay2;
                                    id = Insert;
                                    if (Insert <= 0)
                                    {
                                        return Insert;
                                    }
                                }


                                break;
                            }
                        case 3:
                            {

                                if (item.WeekDay.Contains("3"))
                                {
                                    var WeekDay2 = item.WeekDay;
                                    var item2 = item;
                                    item2.WeekDay = "4";

                                    var Insert = await _programsPackageDAL.InsertProgramPackage(item2, Userid, type2);
                                    item.WeekDay = WeekDay2;
                                    id = Insert;
                                    if (Insert <= 0)
                                    {
                                        return Insert;
                                    }
                                }

                                break;
                            }
                        case 4:
                            {

                                if (item.WeekDay.Contains("4"))
                                {
                                    var WeekDay2 = item.WeekDay;
                                    var item2 = item;
                                    item2.WeekDay = "5";

                                    var Insert = await _programsPackageDAL.InsertProgramPackage(item2, Userid, type2);
                                    item.WeekDay = WeekDay2;
                                    id = Insert;
                                    if (Insert <= 0)
                                    {
                                        return Insert;
                                    }
                                }

                                break;
                            }
                        case 5:
                            {

                                if (item.WeekDay.Contains("5"))
                                {
                                    var WeekDay2 = item.WeekDay;
                                    var item2 = item;
                                    item2.WeekDay = "6";

                                    var Insert = await _programsPackageDAL.InsertProgramPackage(item2, Userid, type2);
                                    item.WeekDay = WeekDay2;
                                    id = Insert;
                                    if (Insert <= 0)
                                    {
                                        return Insert;
                                    }
                                }

                                break;
                            }
                        case 6:
                            {

                                if (item.WeekDay.Contains("6"))
                                {
                                    var WeekDay2 = item.WeekDay;
                                    var item2 = item;
                                    item2.WeekDay = "7";

                                    var Insert = await _programsPackageDAL.InsertProgramPackage(item2, Userid, type2);
                                    item.WeekDay = WeekDay2;
                                    id = Insert;
                                    if (Insert <= 0)
                                    {
                                        return Insert;
                                    }
                                }

                                break;
                            }
                    }

                }
                return id;
            }
            catch (Exception ex)
            {
                return -1;
                LogHelper.InsertLogTelegram("InsertProgramPackage- ProgramsPackageReprository: " + ex);
            }

        }
        public async Task<ProgramsPackageModel> DetailProgramPackage(long id)
        {
            try
            {
                var data = new ProgramsPackageModel();
                var Model = new ProgramsPackageSearchViewModel();

                //var detail = await _programsPackageDAL.GetProgramPackagesbyId(id);
                //if(detail !=null)
                Model.ProgramId = id.ToString();
                Model.PageIndex = -1;
                var Program = _programsDAL.GetProgramsbyProgramId(id, 1, 10);
                DataTable dt = await _programsPackageDAL.GetPagingList(Model);
                if (dt != null && dt.Rows.Count > 0)
                {
                    var data2 = dt.ToList<ProgramsPackageViewModel>();

                    //var data4 = data2.Where(s => s.PackageCode == detail.PackageCode && s.RoomType == detail.RoomType).ToList();
                    var data3 = data2.GroupBy(s => new { s.Price, s.RoomType, s.FromDate }).Select(i => i.First()).ToList();

                    foreach (var item2 in data3)
                    {
                        var listWeekDay = "";
                        var listWeekDay2 = data2.Where(s => s.Price == item2.Price && s.RoomType == item2.RoomType).Select(s => s.WeekDay).ToList();
                        foreach (var item3 in listWeekDay2)
                        {

                            listWeekDay += item3 + ",";

                        }
                        item2.WeekDay = listWeekDay.TrimEnd(',', ' ');
                    }

                    data.ProgramsPackage = data2;
                    data.Id = Program[0].Id;
                    data.Description = Program[0].Description;
                    data.ProgramName = Program[0].ProgramName;
                    data.FromDate = data3[0].FromDate;
                    data.ToDate = data3[0].ToDate;
                    data.StartDate = Program[0].StayStartDate.ToString();
                    data.EndDate = Program[0].StayEndDate.ToString();
                    data.PackageCode = data3[0].PackageCode;
                    data.Status = (int)Program[0].Status;
                    data.ProgramsPackageStatus = (int)data3[0].OpenStatus;
                    data.SupplierId = (int)Program[0].SupplierId;
                    data.RoomType = data3[0].RoomType;
                    data.RoomTypeId = data3[0].RoomTypeId;
                    data.PackageName = data3[0].PackageName;
                }
                return data;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("DetailProgramPackage- ProgramsPackageReprository: " + ex);
            }
            return null;
        }
        public async Task<int> DeleteProgramPackagesbyProgramId(long id, string PackageCode, string RoomType, long Amount, DateTime? FromDate, DateTime? ApplyDate)
        {
            try
            {
                return await _programsPackageDAL.DeleteProgramPackagesbyProgramId(id, PackageCode, RoomType, Amount, FromDate, ApplyDate);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("DeleteProgramPackagesbyProgramId - ProgramsPackageReprository: " + ex);
            }
            return 0;
        }
        public async Task<int> UpdateProgramPackage(List<InsertProgramsPackageViewModel> Model, long Userid, long type2)
        {
            try
            {
                var id = -3;
                var detail = await _programsPackageDAL.GetProgramPackagesbyId(Model[0].id);
                var Insertpk = 1;
                if (detail.RoomType == Model[0].RoomType)
                {

                    var delete = await DeleteProgramPackagesbyProgramId(Model[0].ProgramId, detail.PackageCode, Model[0].RoomType, 0, Model[0].FromDate, null);
                    for (int i = 0; i <= (Convert.ToDateTime(Model[0].ToDate) - Convert.ToDateTime(Model[0].FromDate)).Days; i++)
                    {
                        var dateWeek = (Convert.ToDateTime(Model[0].FromDate).AddDays(i));
                        var Week = (int)dateWeek.DayOfWeek;

                        switch (Week)
                        {
                            case 0:
                                {
                                    foreach (var item in Model)
                                    {
                                        if (item.WeekDay.Contains("0"))
                                        {
                                            var WeekDay2 = item.WeekDay;
                                            var item2 = item;
                                            item2.WeekDay = "0";
                                            item2.ApplyDate = dateWeek;
                                            var Insert = await _programsPackageDAL.InsertProgramPackage(item2, Userid, type2);
                                            item.WeekDay = WeekDay2;
                                            id = Insert;
                                            if (Insert <= 0)
                                            {
                                                return Insert;
                                            }
                                        }
                                    }

                                    break;
                                }
                            case 1:
                                {
                                    foreach (var item in Model)
                                    {
                                        if (item.WeekDay.Contains("2"))
                                        {
                                            var item2 = item;
                                            var WeekDay2 = item.WeekDay;
                                            item2.WeekDay = "2";
                                            item2.ApplyDate = dateWeek;
                                            var Insert = await _programsPackageDAL.InsertProgramPackage(item2, Userid, type2);
                                            item.WeekDay = WeekDay2;
                                            id = Insert;
                                            if (Insert <= 0)
                                            {
                                                return Insert;
                                            }
                                        }
                                    }
                                    break;
                                }
                            case 2:
                                {
                                    foreach (var item in Model)
                                    {
                                        if (item.WeekDay.Contains("3"))
                                        {
                                            var WeekDay2 = item.WeekDay;
                                            var item2 = item;
                                            item2.WeekDay = "3";
                                            item2.ApplyDate = dateWeek;
                                            var Insert = await _programsPackageDAL.InsertProgramPackage(item2, Userid, type2);
                                            item.WeekDay = WeekDay2;
                                            id = Insert;
                                            if (Insert <= 0)
                                            {
                                                return Insert;
                                            }
                                        }
                                    }

                                    break;
                                }
                            case 3:
                                {
                                    foreach (var item in Model)
                                    {
                                        if (item.WeekDay.Contains("4"))
                                        {
                                            var WeekDay2 = item.WeekDay;
                                            var item2 = item;
                                            item2.WeekDay = "4";
                                            item2.ApplyDate = dateWeek;
                                            var Insert = await _programsPackageDAL.InsertProgramPackage(item2, Userid, type2);
                                            item.WeekDay = WeekDay2;
                                            id = Insert;
                                            if (Insert <= 0)
                                            {
                                                return Insert;
                                            }
                                        }
                                    }
                                    break;
                                }
                            case 4:
                                {
                                    foreach (var item in Model)
                                    {
                                        if (item.WeekDay.Contains("5"))
                                        {
                                            var WeekDay2 = item.WeekDay;
                                            var item2 = item;
                                            item2.WeekDay = "5";
                                            item2.ApplyDate = dateWeek;
                                            var Insert = await _programsPackageDAL.InsertProgramPackage(item2, Userid, type2);
                                            item.WeekDay = WeekDay2;
                                            id = Insert;
                                            if (Insert <= 0)
                                            {
                                                return Insert;
                                            }
                                        }
                                    }
                                    break;
                                }
                            case 5:
                                {
                                    foreach (var item in Model)
                                    {
                                        if (item.WeekDay.Contains("6"))
                                        {
                                            var WeekDay2 = item.WeekDay;
                                            var item2 = item;
                                            item2.WeekDay = "6";
                                            item2.ApplyDate = dateWeek;
                                            var Insert = await _programsPackageDAL.InsertProgramPackage(item2, Userid, type2);
                                            item.WeekDay = WeekDay2;
                                            id = Insert;
                                            if (Insert <= 0)
                                            {
                                                return Insert;
                                            }
                                        }
                                    }
                                    break;
                                }
                            case 6:
                                {
                                    foreach (var item in Model)
                                    {
                                        if (item.WeekDay.Contains("7"))
                                        {
                                            var WeekDay2 = item.WeekDay;
                                            var item2 = item;
                                            item2.WeekDay = "7";
                                            item2.ApplyDate = dateWeek;
                                            var Insert = await _programsPackageDAL.InsertProgramPackage(item2, Userid, type2);
                                            item.WeekDay = WeekDay2;
                                            id = Insert;
                                            if (Insert <= 0)
                                            {
                                                return Insert;
                                            }
                                        }
                                    }
                                    break;
                                }
                        }

                    }
                    return id;
                }
                else
                {
                    var delete = await DeleteProgramPackagesbyProgramId((long)detail.ProgramId, detail.PackageCode, detail.RoomType, 0, detail.FromDate, null);
                    for (int i = 0; i <= (Convert.ToDateTime(Model[0].ToDate) - Convert.ToDateTime(Model[0].FromDate)).Days; i++)
                    {
                        var dateWeek = (Convert.ToDateTime(Model[0].FromDate).AddDays(i));
                        var Week = (int)dateWeek.DayOfWeek;

                        switch (Week)
                        {
                            case 0:
                                {
                                    foreach (var item in Model)
                                    {
                                        if (item.WeekDay.Contains("0"))
                                        {
                                            var WeekDay2 = item.WeekDay;
                                            var item2 = item;
                                            item2.WeekDay = "0";
                                            item2.ApplyDate = dateWeek;
                                            var Insert = await _programsPackageDAL.InsertProgramPackage(item2, Userid, type2);
                                            item.WeekDay = WeekDay2;
                                            id = Insert;
                                            if (Insert <= 0)
                                            {
                                                return Insert;
                                            }
                                        }
                                    }

                                    break;
                                }
                            case 1:
                                {
                                    foreach (var item in Model)
                                    {
                                        if (item.WeekDay.Contains("2"))
                                        {
                                            var item2 = item;
                                            var WeekDay2 = item.WeekDay;
                                            item2.WeekDay = "2";
                                            item2.ApplyDate = dateWeek;
                                            var Insert = await _programsPackageDAL.InsertProgramPackage(item2, Userid, type2);
                                            item.WeekDay = WeekDay2;
                                            id = Insert;
                                            if (Insert <= 0)
                                            {
                                                return Insert;
                                            }
                                        }
                                    }
                                    break;
                                }
                            case 2:
                                {
                                    foreach (var item in Model)
                                    {
                                        if (item.WeekDay.Contains("3"))
                                        {
                                            var WeekDay2 = item.WeekDay;
                                            var item2 = item;
                                            item2.WeekDay = "3";
                                            item2.ApplyDate = dateWeek;
                                            var Insert = await _programsPackageDAL.InsertProgramPackage(item2, Userid, type2);
                                            item.WeekDay = WeekDay2;
                                            id = Insert;
                                            if (Insert <= 0)
                                            {
                                                return Insert;
                                            }
                                        }
                                    }

                                    break;
                                }
                            case 3:
                                {
                                    foreach (var item in Model)
                                    {
                                        if (item.WeekDay.Contains("4"))
                                        {
                                            var WeekDay2 = item.WeekDay;
                                            var item2 = item;
                                            item2.WeekDay = "4";
                                            item2.ApplyDate = dateWeek;
                                            var Insert = await _programsPackageDAL.InsertProgramPackage(item2, Userid, type2);
                                            item.WeekDay = WeekDay2;
                                            id = Insert;
                                            if (Insert <= 0)
                                            {
                                                return Insert;
                                            }
                                        }
                                    }
                                    break;
                                }
                            case 4:
                                {
                                    foreach (var item in Model)
                                    {
                                        if (item.WeekDay.Contains("5"))
                                        {
                                            var WeekDay2 = item.WeekDay;
                                            var item2 = item;
                                            item2.WeekDay = "5";
                                            item2.ApplyDate = dateWeek;
                                            var Insert = await _programsPackageDAL.InsertProgramPackage(item2, Userid, type2);
                                            item.WeekDay = WeekDay2;
                                            id = Insert;
                                            if (Insert <= 0)
                                            {
                                                return Insert;
                                            }
                                        }
                                    }
                                    break;
                                }
                            case 5:
                                {
                                    foreach (var item in Model)
                                    {
                                        if (item.WeekDay.Contains("6"))
                                        {
                                            var WeekDay2 = item.WeekDay;
                                            var item2 = item;
                                            item2.WeekDay = "6";
                                            item2.ApplyDate = dateWeek;
                                            var Insert = await _programsPackageDAL.InsertProgramPackage(item2, Userid, type2);
                                            item.WeekDay = WeekDay2;
                                            id = Insert;
                                            if (Insert <= 0)
                                            {
                                                return Insert;
                                            }
                                        }
                                    }
                                    break;
                                }
                            case 6:
                                {
                                    foreach (var item in Model)
                                    {
                                        if (item.WeekDay.Contains("7"))
                                        {
                                            var WeekDay2 = item.WeekDay;
                                            var item2 = item;
                                            item2.WeekDay = "7";
                                            item2.ApplyDate = dateWeek;
                                            var Insert = await _programsPackageDAL.InsertProgramPackage(item2, Userid, type2);
                                            item.WeekDay = WeekDay2;
                                            id = Insert;
                                            if (Insert <= 0)
                                            {
                                                return Insert;
                                            }
                                        }
                                    }
                                    break;
                                }
                        }

                    }
                    return id;
                }


                return Insertpk;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("InsertProgramPackage- ProgramsPackageReprository: " + ex);
            }
            return 0;
        }
        public async Task<List<ProgramsPackageViewModel>> ListdataProgramPackage(ProgramsPackageSearchViewModel searchModel)
        {

            try
            {

                searchModel.PageIndex = -1;
                DataTable dt = await _programsPackageDAL.GetPagingList(searchModel);
                if (dt != null && dt.Rows.Count > 0)
                {
                    var data = dt.ToList<ProgramsPackageViewModel>();
                    return data;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ListdataProgramPackage - ProgramsPackageReprository: " + ex);
            }
            return null;
        }
        public async Task<int> DeleteProgramPackage(long ProgramId, string PackageCode, string RoomType, long Amount, DateTime? FromDate)
        {
            try
            {
                var delete = await DeleteProgramPackagesbyProgramId(ProgramId, PackageCode, RoomType, Amount, FromDate, null);


                return 1;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("DeleteProgramPackage- ProgramsPackageReprository: " + ex);
            }
            return 0;
        }
        public async Task<int> EditProgramPackagesbyProgram(InsertProgramsPackageViewModel model, long Userid)
        {
            try
            {
                var delete = await _programsPackageDAL.UpdateProgramPackage(model, Userid);


                return delete;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("EditProgramPackagesbyProgram- ProgramsPackageReprository: " + ex);

                return 0;
            }

        }
        public Task<ProgramPackage> DetailPackagesbyProgramById(long id)
        {
            try
            {
                var Detail = _programsPackageDAL.GetProgramPackagesbyId(id);


                return Detail;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("EditProgramPackagesbyProgram- ProgramsPackageReprository: " + ex);

                return null;
            }

        }
        public async Task<int> InsertProgramPackageDaily(List<InsertProgramsPackageViewModel> Model, List<InsertProgramsPackageViewModel> Model2, long Userid, long type2)
        {
            try
            {
                var id = -3;

                foreach (var i in Model)
                {

                    var listWeekDay = i.WeekDay.Split(',');
                    var listWeekDay2 = i.WeekDay;
                    var data2 = await _programPackageDailyDAL.CheckExistsProgramsPackageDailyByDate(i);
                    if (data2 != 0)
                    {
                        return -1;
                    }
                    if (listWeekDay.Count() > 1)
                    {
                        foreach (var item3 in listWeekDay)
                        {
                            i.WeekDay = item3;
                            var Insert = await _programPackageDailyDAL.CheckExistsProgramsPackageDailyByDate(i);

                            if (Insert != 0)
                            {
                                return -1;
                            }
                            else
                            {
                                i.WeekDay = listWeekDay2;
                            }
                        }
                        continue;
                    }
                    else
                    {
                        var data3 = await _programPackageDailyDAL.CheckExistsProgramsPackageDailyByDate(i);
                        if (data3 != 0)
                        {
                            return -1;
                        }

                    }

                }
                foreach (var i in Model)
                {
                    var listWeekDay = i.WeekDay.Split(',');
                    var listWeekDay2 = i.WeekDay;
                    foreach (var week in listWeekDay)
                    {
                        var WeekDay2 = i.WeekDay;
                        var item2 = i;
                        item2.WeekDay = week;

                        var Insert = await _programPackageDailyDAL.InsertProgramPackageDaily(item2, Userid);
                        i.WeekDay = WeekDay2;
                        id = Insert;
                        if (Insert <= 0)
                        {
                            return Insert;
                        }
                    }


                }

                if (Model2 != null && Model2.Count > 0)
                {
                    InsertProgramPackage(Model2, Userid, type2);
                }
                return id;
            }
            catch (Exception ex)
            {
                return 0;
                LogHelper.InsertLogTelegram("InsertProgramPackage- ProgramsPackageReprository: " + ex);
            }

        }
        public async Task<GenericViewModel<ProgramsPackageModel>> ListProgramsPackageDaily(ProgramsPackageSearchViewModel searchModel)
        {
            var model = new GenericViewModel<ProgramsPackageModel>();
            int TotalRow = 0;
            var PageIndex = searchModel.PageIndex;
            try
            {
                var ProgramsPackage = new List<ProgramsPackageModel>();

                var listProgram = new List<Programs>();
                if (searchModel.ProgramId != null)
                {
                    listProgram = _programsDAL.GetProgramsbyProgramId(Convert.ToInt32(searchModel.ProgramId), searchModel.PageIndex, searchModel.PageSize);
                }
                else
                {
                    listProgram = _programsDAL.GetAll(searchModel.PageIndex, searchModel.PageSize);
                }
                if (listProgram != null)
                {
                    TotalRow = listProgram.Count;
                    foreach (var item in listProgram)
                    {

                        searchModel.ProgramId = item.Id.ToString();
                        searchModel.PageIndex = -1;
                        DataTable dt = await _programPackageDailyDAL.GetListProgramsPackageDaily(searchModel);
                        DataTable dt2 = await _programsPackageDAL.GetPagingList(searchModel);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            var data_programsPackage = new List<ProgramsPackageViewModel>();
                            if (dt2 != null && dt2.Rows.Count > 0)
                            {
                                data_programsPackage = dt2.ToList<ProgramsPackageViewModel>();
                            }
                            var data = dt.ToList<ProgramsPackageViewModel>();
                            var data3 = data.GroupBy(s => new { s.PackageCode }).Select(i => i.First()).ToList();
                            var List_data_programsPackage = data_programsPackage.GroupBy(s => new { s.PackageCode }).Select(i => i.First()).ToList();
                            if(List_data_programsPackage.Count > data3.Count)
                            {
                                data3.AddRange(List_data_programsPackage);
                                data3= data3.GroupBy(s => new { s.PackageCode}).Select(i => i.First()).ToList();
                            }
                            foreach (var i in data3)
                            {
                                var ProgramsPackageDetail = new ProgramsPackageModel();
                                ProgramsPackageDetail.Id = item.Id;
                                ProgramsPackageDetail.Status = (int)item.Status;
                                ProgramsPackageDetail.ProgramsPackageId = i.id;
                                ProgramsPackageDetail.Description = item.Description;
                                ProgramsPackageDetail.ProgramName = item.ProgramName;
                                ProgramsPackageDetail.ProgramName = i.PackageCode;
                                var data2 = data.Where(s => s.PackageCode == i.PackageCode).GroupBy(s => new { s.Price, s.RoomType, s.FromDate }).Select(i => i.First()).ToList();
                                var data2_programsPackage = data_programsPackage != null ? data_programsPackage.Where(s => s.PackageCode == i.PackageCode).GroupBy(s => new { s.Price, s.RoomType, s.FromDate, s.ApplyDate ,s.WeekDay}).Select(i => i.First()).ToList() : null;
                                foreach (var item2 in data2)
                                {
                                    var listWeekDay = "";
                                    var listWeekDay2 = data.Where(s => s.Price == item2.Price && s.RoomType == item2.RoomType && s.PackageCode == i.PackageCode).Select(s => s.WeekDay).Distinct().ToList();
                                    foreach (var item3 in listWeekDay2)
                                    {
                                        if (item3 == "0")
                                        {
                                            string WeekDayCN = "CN, ";
                                            listWeekDay += WeekDayCN;
                                        }
                                        else
                                        {
                                            if (item3 != "" && (item3 == "2" || item3 == "3" || item3 == "4" || item3 == "5" || item3 == "6" || item3 == "7"))
                                            {
                                                string WeekDay = "Thứ " + item3 + ", ";
                                                listWeekDay += WeekDay;
                                            }
                                        }

                                    }
                                    item2.WeekDay = listWeekDay;
                                }
                                if (data2_programsPackage != null)
                                {
                                    foreach (var item2 in data2_programsPackage)
                                    {
                                        var listWeekDay = "";
                                        var listprogramsPackagedaily = data2.Where(s => s.PackageCode == item2.PackageCode && s.RoomType == item2.RoomType && s.FromDate == item2.FromDate).ToList();
                                        if (listprogramsPackagedaily.Count == 0) { item2.Type = 1; }
                                        if (item2.Type == 0)
                                        {
                                            var listWeekDay2 = data_programsPackage.Where(s => s.Price == item2.Price && s.RoomType == item2.RoomType && s.PackageCode == i.PackageCode && s.ApplyDate == item2.ApplyDate).Select(s => s.WeekDay).Distinct().ToList();
                                            foreach (var item3 in listWeekDay2)
                                            {
                                                if (item3 == "0")
                                                {
                                                    string WeekDayCN = "CN, ";
                                                    listWeekDay += WeekDayCN;
                                                }
                                                else
                                                {
                                                    if (item3 != "" && (item3 == "2" || item3 == "3" || item3 == "4" || item3 == "5" || item3 == "6" || item3 == "7"))
                                                    {
                                                        string WeekDay = "Thứ " + item3 + ", ";
                                                        listWeekDay += WeekDay;
                                                    }
                                                }

                                            }
                                            item2.WeekDay = listWeekDay;
                                        }
                                        else
                                        {
                                            var listWeekDay2 = data_programsPackage.Where(s => s.Price == item2.Price && s.RoomType == item2.RoomType && s.PackageCode == i.PackageCode).Select(s => s.WeekDay).Distinct().ToList();
                                            foreach (var item3 in listWeekDay2)
                                            {
                                                if (item3 == "0")
                                                {
                                                    string WeekDayCN = "CN, ";
                                                    listWeekDay += WeekDayCN;
                                                }
                                                else
                                                {
                                                    if (item3 != "" && (item3 == "2" || item3 == "3" || item3 == "4" || item3 == "5" || item3 == "6" || item3 == "7"))
                                                    {
                                                        string WeekDay = "Thứ " + item3 + ", ";
                                                        listWeekDay += WeekDay;
                                                    }
                                                }

                                            }
                                            item2.WeekDay = listWeekDay;
                                        }

                                    }
                                    data2.AddRange(data2_programsPackage);

                                }
                                ProgramsPackageDetail.ProgramsPackage = data2;
                                ProgramsPackageDetail.FromDate = data2.Min(s => s.FromDate);
                                ProgramsPackageDetail.ToDate = data2.Max(s => s.ToDate);
                                ProgramsPackageDetail.SupplierId = item.SupplierId;
                                ProgramsPackage.Add(ProgramsPackageDetail);

                            }

                        }
                        else
                        {
                            if (dt2 != null && dt2.Rows.Count > 0)
                            {
                                var data = dt2.ToList<ProgramsPackageViewModel>();
                                var data3 = data.GroupBy(s => new { s.PackageCode }).Select(i => i.First()).ToList();

                                foreach (var i in data3)
                                {

                                    var ProgramsPackageDetail = new ProgramsPackageModel();
                                    ProgramsPackageDetail.Id = item.Id;
                                    ProgramsPackageDetail.Status = (int)item.Status;
                                    ProgramsPackageDetail.ProgramsPackageId = i.id;
                                    ProgramsPackageDetail.Description = item.Description;
                                    ProgramsPackageDetail.ProgramName = item.ProgramName;
                                    ProgramsPackageDetail.ProgramName = i.PackageCode;
                                    var data2 = data.Where(s => s.PackageCode == i.PackageCode).GroupBy(s => new { s.Price, s.RoomType, s.RoomTypeId, s.FromDate }).Select(i => i.First()).ToList();
                                    foreach (var item2 in data2)
                                    {
                                        var listWeekDay = "";
                                        item2.Type = 1;
                                        var listWeekDay2 = data.Where(s => s.Price == item2.Price && s.RoomType == item2.RoomType && s.PackageCode == i.PackageCode ).Select(s => s.WeekDay).Distinct().ToList();
                                        foreach (var item3 in listWeekDay2)
                                        {
                                            if (item3 == "0")
                                            {
                                                string WeekDayCN = "CN, ";
                                                listWeekDay += WeekDayCN;
                                            }
                                            else
                                            {
                                                if (item3 != "" && (item3 == "2"|| item3 == "3" || item3 == "4" || item3 == "5" || item3 == "6" || item3 == "7"))
                                                {
                                                    string WeekDay = "Thứ " + item3 + ", ";
                                                    listWeekDay += WeekDay;
                                                }
                                                
                                            }

                                        }
                                        item2.WeekDay = listWeekDay;
                                    }
                                    ProgramsPackageDetail.ProgramsPackage = data2;
                                    ProgramsPackageDetail.FromDate = data2.Min(s => s.FromDate);
                                    ProgramsPackageDetail.ToDate = data2.Max(s => s.ToDate);
                                    ProgramsPackageDetail.SupplierId = item.SupplierId;
                                    ProgramsPackage.Add(ProgramsPackageDetail);
                                }
                            }

                        }
                    }
                }
                model.ListData = ProgramsPackage;
                model.CurrentPage = PageIndex;
                model.PageSize = searchModel.PageSize;
                model.TotalRecord = TotalRow;
                model.TotalPage = (int)Math.Ceiling((double)model.TotalRecord / model.PageSize);
                return model;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ListProgramsPackageDaily- ProgramsPackageReprository: " + ex);
            }
            return null;
        }
        public async Task<GenericViewModel<ProgramsPackagePriceViewModel>> GetListProgramPackageDaily(ProgramsPackageSearchViewModel searchModel)
        {
            var model = new GenericViewModel<ProgramsPackagePriceViewModel>();
            int TotalRow = 0;
            try
            {
                var ProgramsPackage = new List<ProgramsPackagePriceViewModel>();
                var listProgram = new List<Programs>();
                if (searchModel.ProgramId != null)
                {
                    listProgram = _programsDAL.GetProgramsbyProgramId(Convert.ToInt32(searchModel.ProgramId), searchModel.PageIndex, searchModel.PageSize);
                }
                else
                {
                    listProgram = _programsDAL.GetAll(searchModel.PageIndex, searchModel.PageSize);
                }
                var PageIndex = searchModel.PageIndex;
                if (listProgram != null)
                {
                    TotalRow = listProgram.Count;
                    foreach (var item in listProgram)
                    {
                        searchModel.ProgramId = item.Id.ToString();
                        searchModel.PageIndex = -1;
                        DataTable dt = await _programsPackageDAL.GetPagingList(searchModel);
                        DataTable dt2 = await _programPackageDailyDAL.GetListProgramsPackageDaily(searchModel);
                        if ((dt != null && dt.Rows.Count > 0) || (dt2 != null && dt2.Rows.Count > 0))
                        {
                            var data = dt.ToList<ProgramsPackageViewModel>();
                            var ListProgramsPackageDaily = dt2.ToList<ProgramsPackageViewModel>();
                            var data2 = data.GroupBy(s => new { s.PackageCode, s.RoomType }).Select(i => i.First()).ToList();
                            var list_ProgramsPackageDaily = ListProgramsPackageDaily.GroupBy(s => new { s.PackageCode, s.RoomType }).Select(i => i.First()).ToList();


                            foreach (var item3 in data2)
                            {
                                var ProgramsPackageDetail = new ProgramsPackagePriceViewModel();
                                var ProgramsPackagePrice = new List<ProgramsPackageModel>();
                                var ProgramsPackagePriceDetail = new ProgramsPackageModel();


                                ProgramsPackagePriceDetail.ProgramName = item3.PackageCode;
                                ProgramsPackagePriceDetail.RoomType = item3.RoomType;
                                ProgramsPackagePriceDetail.RoomTypeId = item3.RoomTypeId;
                                ProgramsPackagePriceDetail.ProgramsPackage = data.Where(s => s.PackageCode == item3.PackageCode && s.RoomType == item3.RoomType /*&& s.OpenStatus == 0*/).ToList();
                                ProgramsPackagePriceDetail.ProgramsPackageDaily = ListProgramsPackageDaily.Where(s => s.PackageCode == item3.PackageCode && s.RoomType == item3.RoomType /*&& s.OpenStatus == 0*/).ToList();
                                ProgramsPackageDetail.Id = item.Id;
                                ProgramsPackageDetail.RoomType = item3.RoomType;
                                ProgramsPackageDetail.RoomTypeId = item3.RoomTypeId;
                                ProgramsPackageDetail.Description = item.Description;
                                ProgramsPackagePrice.Add(ProgramsPackagePriceDetail);

                                ProgramsPackageDetail.ProgramsPackagePrice = ProgramsPackagePrice;
                                ProgramsPackageDetail.ProgramName = ProgramsPackagePrice[0].ProgramName;
                                ProgramsPackageDetail.FromDate = searchModel.FromDate != null ? searchModel.FromDate : (ProgramsPackagePriceDetail.ProgramsPackageDaily.Count > 0 ? ProgramsPackagePriceDetail.ProgramsPackageDaily.Min(s => Convert.ToDateTime(s.FromDate)).ToString("dd/MM/yyyy") : ProgramsPackagePriceDetail.ProgramsPackage.Min(s => Convert.ToDateTime(s.FromDate)).ToString("dd/MM/yyyy"));
                                ProgramsPackageDetail.ToDate = searchModel.ToDate != null ? searchModel.ToDate : (ProgramsPackagePriceDetail.ProgramsPackageDaily.Count > 0 ? ProgramsPackagePriceDetail.ProgramsPackageDaily.Max(s => Convert.ToDateTime(s.ToDate)).ToString("dd/MM/yyyy") : ProgramsPackagePriceDetail.ProgramsPackage.Min(s => Convert.ToDateTime(s.ToDate)).ToString("dd/MM/yyyy"));
                                ProgramsPackage.Add(ProgramsPackageDetail);
                            }
                            if (list_ProgramsPackageDaily != null && list_ProgramsPackageDaily.Count > 0)
                                foreach (var item3 in list_ProgramsPackageDaily)
                                {
                                    var ProgramsPackageDetail = new ProgramsPackagePriceViewModel();
                                    var ProgramsPackagePrice = new List<ProgramsPackageModel>();
                                    var ProgramsPackagePriceDetail = new ProgramsPackageModel();
                                    ProgramsPackagePriceDetail.Id = item.Id;
                                    ProgramsPackagePriceDetail.ProgramName = item3.PackageCode;
                                    ProgramsPackagePriceDetail.RoomType = item3.RoomType;

                                    ProgramsPackagePriceDetail.ProgramsPackageDaily = ListProgramsPackageDaily.Where(s => s.PackageCode == item3.PackageCode && s.RoomType == item3.RoomType /*&& s.OpenStatus == 0*/).ToList();

                                    ProgramsPackageDetail.Id = item.Id;
                                    ProgramsPackageDetail.RoomType = item3.RoomType;
                                    ProgramsPackageDetail.RoomTypeId = item3.RoomTypeId;
                                    ProgramsPackageDetail.Description = item.Description;
                                    ProgramsPackagePrice.Add(ProgramsPackagePriceDetail);


                                    ProgramsPackageDetail.ProgramsPackagePrice = ProgramsPackagePrice;
                                    ProgramsPackageDetail.ProgramName = ProgramsPackagePrice[0].ProgramName;
                                    ProgramsPackageDetail.FromDate = searchModel.FromDate != null ? searchModel.FromDate : ProgramsPackagePriceDetail.ProgramsPackageDaily.Min(s => Convert.ToDateTime(s.FromDate)).ToString("dd/MM/yyyy"); ;
                                    ProgramsPackageDetail.ToDate = searchModel.ToDate != null ? searchModel.ToDate : ProgramsPackagePriceDetail.ProgramsPackageDaily.Max(s => Convert.ToDateTime(s.ToDate)).ToString("dd/MM/yyyy");
                                    ProgramsPackage.Add(ProgramsPackageDetail);
                                }
                        }

                    }
                }
                model.ListData = ProgramsPackage.GroupBy(s => new { s.RoomType, s.ProgramName }).Select(i => i.First()).ToList();
                model.CurrentPage = PageIndex;
                model.PageSize = searchModel.PageSize;
                model.TotalRecord = TotalRow;
                model.TotalPage = (int)Math.Ceiling((double)model.TotalRecord / model.PageSize);
                return model;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ListProgramPackage- ProgramsPackageReprository: " + ex);
            }
            return null;
        }
        public async Task<int> UpdateProgramPackageDaily(List<InsertProgramsPackageViewModel> Model, List<InsertProgramsPackageViewModel> Model2, long Userid, long type2, long type)
        {
            try
            {
                var id = -3;
                var detail = await _programPackageDailyDAL.GetProgramPackageDailybyId(Model[0].ProgramId,Convert.ToInt32(Model[0].RoomTypeId));

                if (detail == null || ((type!=3? detail.RoomType : Model[0].RoomType) == Model[0].RoomType))
                {
                    if(type == 3)
                    foreach (var i in Model)
                    {

                        var listWeekDay = i.WeekDay.Split(',');
                        var listWeekDay2 = i.WeekDay;
                        var data2 = await _programPackageDailyDAL.CheckExistsProgramsPackageDailyByDate(i);
                        if (data2 != 0)
                        {
                            return -1;
                        }
                        if (listWeekDay.Count() > 1)
                        {
                            foreach (var item3 in listWeekDay)
                            {
                                i.WeekDay = item3;
                                var Insert = await _programPackageDailyDAL.CheckExistsProgramsPackageDailyByDate(i);

                                if (Insert != 0)
                                {
                                    return -1;
                                }
                                else
                                {
                                    i.WeekDay = listWeekDay2;
                                }
                            }
                            continue;
                        }
                        else
                        {
                            var data3 = await _programPackageDailyDAL.CheckExistsProgramsPackageDailyByDate(i);
                            if (data3 != 0)
                            {
                                return -1;
                            }

                        }

                    }
                    if(detail == null && type ==1) {
                        detail = await _programPackageDailyDAL.GetProgramPackageDailybyId(Model[0].ProgramId, Convert.ToInt32(Model[0].RoomTypeIdV1));
                        await _programPackageDailyDAL.DeleteProgramPackagesDailyByProgramId(Model[0].ProgramId, Model[0].PackageCode, detail.RoomType, 0, Model[0].FromDate, 9);
                        await DeleteProgramPackagesbyProgramId(Model[0].ProgramId, Model[0].PackageCode, detail.RoomType, 0, Model[0].FromDate, null);
                    }
                    if (Model != null)
                    {
                        await _programPackageDailyDAL.DeleteProgramPackagesDailyByProgramId(Model[0].ProgramId, Model[0].PackageCode, Model[0].RoomType, 0, Model[0].FromDate, 9);
                    }

                    await DeleteProgramPackagesbyProgramId(Model[0].ProgramId, Model[0].PackageCode, Model[0].RoomType, 0, Model[0].FromDate, null);

                   
                    foreach (var i in Model)
                    {
                        var listWeekDay = i.WeekDay.Split(',');
                        var listWeekDay2 = i.WeekDay;
                        foreach (var week in listWeekDay)
                        {
                            var WeekDay2 = i.WeekDay;
                            var item2 = i;
                            item2.WeekDay = week;

                            var Insert = await _programPackageDailyDAL.InsertProgramPackageDaily(item2, Userid);
                            i.WeekDay = WeekDay2;
                            id = Insert;
                            if (Insert <= 0)
                            {
                                return Insert;
                            }
                        }


                    }

                    if (Model2 != null && Model2.Count > 0)
                    {
                        InsertProgramPackage(Model2, Userid, type2);
                    }
                    return id;
                }


                return id;
            }
            catch (Exception ex)
            {
                return 0;
                LogHelper.InsertLogTelegram("InsertProgramPackage- ProgramsPackageReprository: " + ex);
            }

        }
        public async Task<int> DeleteProgramPackagesDailyByProgramId(long id, string PackageCode, string RoomType, long Amount, DateTime? FromDate, int WeekDay)
        {
            try
            {
                return await _programPackageDailyDAL.DeleteProgramPackagesDailyByProgramId(id, PackageCode, RoomType, Amount, FromDate, WeekDay);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("DeleteProgramPackagesDailyByProgramId - ProgramsPackageReprository: " + ex);
            }
            return 0;
        }
        public async Task<int> CheckProgramsPackageDaily(ProgramsPackageSearchViewModel searchModel, string packagecode, string roomtype)
        {
            try
            {
                DataTable dt = await _programPackageDailyDAL.GetListProgramsPackageDaily(searchModel);
                if (dt != null && dt.Rows.Count > 0)
                {
                    var data = dt.ToList<ProgramsPackageViewModel>();
                    if (data != null && data.Count > 0)
                    {
                        data = data.Where(s => s.PackageCode == packagecode && s.RoomType == roomtype).ToList();
                        data = data.GroupBy(s => new { s.Price, s.RoomType, s.FromDate }).Select(i => i.First()).ToList();
                        if (data.Count > 0)
                        {
                            return data.Count;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("DeleteProgramPackagesDailyByProgramId - ProgramsPackageReprository: " + ex);
            }
            return 0;
        }
        public async Task<int> CheckProgramsPackage(ProgramsPackageSearchViewModel searchModel, string packagecode, string roomtype)
        {
            try
            {
                DataTable dt = await _programsPackageDAL.GetPagingList(searchModel);
                if (dt != null && dt.Rows.Count > 0)
                {
                    var data = dt.ToList<ProgramsPackageViewModel>();
                    if (data != null && data.Count > 0)
                    {
                        data = data.Where(s => s.PackageCode == packagecode && s.RoomType == roomtype).ToList();
                        if (data.Count > 0)
                        {
                            return data.Count;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("DeleteProgramPackagesDailyByProgramId - ProgramsPackageReprository: " + ex);
            }
            return 0;
        }
        public async Task<GenericViewModel<ProgramsPackagePriceViewModel>> GetListProgramPriceHotel(ProgramsPackageSearchViewModel searchModel)
        {
            var model = new GenericViewModel<ProgramsPackagePriceViewModel>();
            int TotalRow = 0;
            try
            {
                var ProgramsPackage = new List<ProgramsPackagePriceViewModel>();
                var listProgram = new List<Programs>();

                searchModel.PageIndex = -1;
                DataTable dt = await _programsPackageDAL.GetPagingList(searchModel);
                DataTable dt2 = await _programPackageDailyDAL.GetListProgramsPackageDaily(searchModel);
                if ((dt != null && dt.Rows.Count > 0) || (dt2 != null && dt2.Rows.Count > 0))
                {
                    var data = dt.ToList<ProgramsPackageViewModel>();
                    var ListProgramsPackageDaily = dt2.ToList<ProgramsPackageViewModel>();
                    var data2 = data.GroupBy(s => new { s.PackageCode, s.RoomType }).Select(i => i.First()).ToList();
                    var list_ProgramsPackageDaily = ListProgramsPackageDaily.GroupBy(s => new { s.PackageCode, s.RoomType }).Select(i => i.First()).ToList();


                    foreach (var item3 in data2)
                    {
                        var ProgramsPackageDetail = new ProgramsPackagePriceViewModel();
                        var ProgramsPackagePrice = new List<ProgramsPackageModel>();
                        var ProgramsPackagePriceDetail = new ProgramsPackageModel();


                        ProgramsPackagePriceDetail.ProgramName = item3.PackageCode;
                        ProgramsPackagePriceDetail.RoomType = item3.RoomType;
                        ProgramsPackagePriceDetail.ProgramsPackage = data.Where(s => s.PackageCode == item3.PackageCode && s.RoomType == item3.RoomType /*&& s.OpenStatus == 0*/).ToList();
                        ProgramsPackagePriceDetail.ProgramsPackageDaily = ListProgramsPackageDaily.Where(s => s.PackageCode == item3.PackageCode && s.RoomType == item3.RoomType /*&& s.OpenStatus == 0*/).ToList();

                        ProgramsPackageDetail.RoomType = item3.RoomType;
                        ProgramsPackageDetail.ProgramsPackageName = item3.PackageCode;

                        ProgramsPackagePrice.Add(ProgramsPackagePriceDetail);

                        ProgramsPackageDetail.ProgramsPackagePrice = ProgramsPackagePrice;
                        ProgramsPackageDetail.ProgramName = item3.ProgramName;
                        ProgramsPackageDetail.FromDate = searchModel.FromDate != null ? searchModel.FromDate : (ProgramsPackagePriceDetail.ProgramsPackageDaily.Count > 0 ? ProgramsPackagePriceDetail.ProgramsPackageDaily.Min(s => Convert.ToDateTime(s.FromDate)).ToString("dd/MM/yyyy") : ProgramsPackagePriceDetail.ProgramsPackage.Min(s => Convert.ToDateTime(s.FromDate)).ToString("dd/MM/yyyy"));
                        ProgramsPackageDetail.ToDate = searchModel.ToDate != null ? searchModel.ToDate : (ProgramsPackagePriceDetail.ProgramsPackageDaily.Count > 0 ? ProgramsPackagePriceDetail.ProgramsPackageDaily.Max(s => Convert.ToDateTime(s.ToDate)).ToString("dd/MM/yyyy") : ProgramsPackagePriceDetail.ProgramsPackage.Min(s => Convert.ToDateTime(s.ToDate)).ToString("dd/MM/yyyy"));
                        ProgramsPackage.Add(ProgramsPackageDetail);
                    }
                    if (list_ProgramsPackageDaily != null && list_ProgramsPackageDaily.Count > 0)
                        foreach (var item3 in list_ProgramsPackageDaily)
                        {
                            var ProgramsPackageDetail = new ProgramsPackagePriceViewModel();
                            var ProgramsPackagePrice = new List<ProgramsPackageModel>();
                            var ProgramsPackagePriceDetail = new ProgramsPackageModel();

                            ProgramsPackagePriceDetail.ProgramName = item3.PackageCode;
                            ProgramsPackagePriceDetail.RoomType = item3.RoomType;

                            ProgramsPackagePriceDetail.ProgramsPackageDaily = ListProgramsPackageDaily.Where(s => s.PackageCode == item3.PackageCode && s.RoomType == item3.RoomType /*&& s.OpenStatus == 0*/).ToList();

                            ProgramsPackageDetail.RoomType = item3.RoomType;
                            ProgramsPackageDetail.ProgramsPackageName = item3.PackageCode;
                            ProgramsPackagePrice.Add(ProgramsPackagePriceDetail);


                            ProgramsPackageDetail.ProgramsPackagePrice = ProgramsPackagePrice;
                            ProgramsPackageDetail.ProgramName = item3.ProgramName;
                            ProgramsPackageDetail.FromDate = searchModel.FromDate != null ? searchModel.FromDate : ProgramsPackagePriceDetail.ProgramsPackageDaily.Min(s => Convert.ToDateTime(s.FromDate)).ToString("dd/MM/yyyy"); ;
                            ProgramsPackageDetail.ToDate = searchModel.ToDate != null ? searchModel.ToDate : ProgramsPackagePriceDetail.ProgramsPackageDaily.Max(s => Convert.ToDateTime(s.ToDate)).ToString("dd/MM/yyyy");
                            ProgramsPackage.Add(ProgramsPackageDetail);
                        }
                }

                model.ListData = ProgramsPackage.GroupBy(s => new { s.RoomType, s.ProgramName,s.ProgramsPackageName }).Select(i => i.First()).ToList();
                model.CurrentPage = searchModel.PageIndex;
                model.PageSize = searchModel.PageSize;
                model.TotalRecord = TotalRow;
                model.TotalPage = (int)Math.Ceiling((double)model.TotalRecord / model.PageSize);
                return model;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ListProgramPackage- ProgramsPackageReprository: " + ex);
            }
            return null;
        }
    }

}
