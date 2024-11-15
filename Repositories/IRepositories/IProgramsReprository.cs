using Entities.ViewModels;
using Entities.ViewModels.Programs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.IRepositories
{
    public interface IProgramsReprository
    {
        Task<GenericViewModel<ProgramsViewModel>> SearchPrograms(ProgramsSearchViewModel searchModel);
        Task<ProgramsViewModel> DetailPrograms(long id);
        Task<int> InsertPrograms(ProgramsModel Model);
        Task<int> UpdatePrograms(ProgramsModel Model);
        Task<int> UpdateProgramsStatus(int status, long id,long userid);
        Task<List<Entities.ViewModels.Programs.HotelModel>> GetlistHotelBySupplierId(long id);
        Task<GenericViewModel<ProgramsViewModel>> GetlistHotelBySupplierId(ProgramsSearchSupplierId searchModel);
        Task<string> ExportDeposit(ProgramsSearchViewModel searchModel, string FilePath, FieldPrograms field);
        Task<List<Entities.ViewModels.Programs.SupplierModel>> GetlistSupplierByHotelId(long id);
     }
}
