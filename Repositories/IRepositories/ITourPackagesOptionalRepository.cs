using Entities.Models;
using Entities.ViewModels.Tour;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.IRepositories
{
    public interface ITourPackagesOptionalRepository
    {
        Task<List<TourPackagesOptionalViewModel>> GetTourPackagesOptional(long tour_id);
        Task<int> InsertTourPackagesOptional(TourPackagesOptional model);
        Task<int> UpdateTourPackagesOptional(TourPackagesOptional model);
        Task<int> DeleteTourPackagesOptional(int Id);
    }
}
