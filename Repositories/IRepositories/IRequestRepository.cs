using Entities.Models;
using Entities.ViewModels;
using Entities.ViewModels.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.IRepositories
{
    public interface  IRequestRepository
    {
        Task<GenericViewModel<RequestViewModel>> GetPagingList(RequestSearchModel searchModel);
        Task<long> UpdateRequest(Request Model);
        Task<Request> GetDetailRequest(long RequestId);
    }
}
