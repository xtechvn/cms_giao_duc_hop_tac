using Entities.Models;
using Entities.ViewModels.CustomerManager;
using Entities.ViewModels;
using Entities.ViewModels.OrderManual;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.IRepositories
{
    public interface IClientRepository
    {
        Task<Client> GetClientDetailAsync(long clientId);
        Task<List<Client>> GetClientType(int Type);
        Task<List<AccountClient>> GetUserCreateSuggest(string name);
        Task<Client> GetClientDetailByClientId(long clientId);
        List<Client> GetAllClient();
        Client GetClientByEmail(string email);
        Client GetClientByTaxNo(string Maso_id);
        int SetUpClient(Client model);
        Task<Client> GetClientByClientCode(string client_code);
        Task<GenericViewModel<ClientCustomerCareFundModel>> GetListClientCustomerCareFund(CustomerManagerViewSearchModel searchModel);
    }
}
