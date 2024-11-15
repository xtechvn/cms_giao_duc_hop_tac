using Entities.Models;
using Entities.ViewModels;
using Entities.ViewModels.Contract;
using Entities.ViewModels.ContractHistory;
using Entities.ViewModels.CustomerManager;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.IRepositories
{
    public interface IContractRepository
    {
        Task<GenericViewModel<ContractViewModel>> GetListByType(long ClientId, int currentPage, int pageSize);
        Task<GenericViewModel<ContractViewModel>> GetPagingList(ContractSearchViewModel searchModel, int currentPage, int pageSize);
        PolicyDetail GetPolicyDetailByType(int ClientType, int PermisionType);
        Task<long> CreateContact(ContractViewModel model);
        Task<List<ContractViewModel>> GetDetailContract(int ContractId);
        Task<long> UpdataContactStatus(long ContractId, long ContractStatus, string Note,long UserIdUpdate);
        Task<long> DeleteContact(long ContractId);
        Task<long> CreateContact2(Contract model, string Note);
        Task<List<ContractHistoryViewModel>> GetPagingListContractHistory(long ContractId, long ActionBy);
        Task<List<TotalConTract>> TotalConTract(ContractSearchViewModel searchModel, int currentPage, int pageSize);
        public Task<Contract> GetActiveContractByClientId(long ClientId);
        Task<int> CheckContractbyStatus(long ClientId, int Status);

    }
}
