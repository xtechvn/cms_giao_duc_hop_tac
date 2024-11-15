using Entities.Models;
using Entities.ViewModels;
using Entities.ViewModels.PlaygroundDetai;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.IRepositories
{
    public interface IPlaygroundDetaiRepository
    {
        Task<GenericViewModel<PlaygroundDetaiViewModel>> GetListPlayground(PlaygroundDetaiSeachViewModel model, int currentPage , int pageSize );
        Task<int> InsertPlaygroundDetail(PlaygroundDetaiViewModel model);
        Task<int> UpdatePlaygroundDetail(PlaygroundDetaiViewModel model);
        Task<PlaygroundDetaiViewModel> GetDetailPlaygroundDetail(long Playground);
        Task<List<PlaygroundDetail>> GetPlaygroundDetailbyCode(string Code);
        Task<int> DeletePlaygroundDetail(long id);
    }
}
