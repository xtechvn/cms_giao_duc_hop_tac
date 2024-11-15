using Entities.Models;
using Entities.ViewModels;
using Entities.ViewModels.Programs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.IRepositories
{
  public interface  IProgramsPackageReprository
    {
        Task<GenericViewModel<ProgramsPackageModel>> ListProgramPackage(ProgramsPackageSearchViewModel searchModel);
        Task<GenericViewModel<ProgramsPackagePriceViewModel>> ListProgramPackagePrice(ProgramsPackageSearchViewModel searchModel);
        Task<int> InsertProgramPackage(List<InsertProgramsPackageViewModel> Model,long Userid,long type2);
        Task<ProgramsPackageModel> DetailProgramPackage(long id);
        Task<int> DeleteProgramPackagesbyProgramId(long id, string PackageCode,string RoomType, long Amount,DateTime? FromDate, DateTime? ApplyDate);
        Task<int> UpdateProgramPackage(List<InsertProgramsPackageViewModel> Model, long Userid,long type2);
        Task<List<ProgramsPackageViewModel>> ListDetaiProgramPackage(long id, string PackageCode);
        Task<List<ProgramsPackageViewModel>> ListdataProgramPackage(ProgramsPackageSearchViewModel searchModel);
        Task<int> EditProgramPackagesbyProgram(InsertProgramsPackageViewModel model, long Userid);
        Task<ProgramPackage> DetailPackagesbyProgramById(long id);
        Task<int> InsertProgramPackageDaily(List<InsertProgramsPackageViewModel> Model, List<InsertProgramsPackageViewModel> Model2, long Userid,long type2);
        Task<GenericViewModel<ProgramsPackageModel>> ListProgramsPackageDaily(ProgramsPackageSearchViewModel searchModel);
        Task<GenericViewModel<ProgramsPackagePriceViewModel>> GetListProgramPackageDaily(ProgramsPackageSearchViewModel searchModel);
        Task<int> UpdateProgramPackageDaily(List<InsertProgramsPackageViewModel> Model, List<InsertProgramsPackageViewModel> Model2, long Userid, long type2, long type);
        Task<int> DeleteProgramPackagesDailyByProgramId(long id, string PackageCode, string RoomType, long Amount, DateTime? FromDate,int WeekDay);
        Task<int> CheckProgramsPackageDaily(ProgramsPackageSearchViewModel searchModel, string packagecode, string roomtype);
        Task<GenericViewModel<ProgramsPackagePriceViewModel>> GetListProgramPriceHotel(ProgramsPackageSearchViewModel searchModel);
        Task<int> CheckProgramsPackage(ProgramsPackageSearchViewModel searchModel, string packagecode, string roomtype);
    }
}
