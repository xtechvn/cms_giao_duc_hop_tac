using Entities.Models;
using Entities.ViewModels;
using Entities.ViewModels.UserAgent;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.IRepositories
{
   public interface IPaymentAccountRepository
    {
        GenericViewModel<PaymentAccount> GetAllByClientId(long id, int currentPage , int pageSize );
        int CreatePaymentAccount(PaymentAccount model);
        int Setup(PaymentAccount model);
        int Delete(int Id);
        Task<PaymentAccount> getPaymentAccountById(int Id);
        int UpdataUserAgent(int ClientId, int UserId, int create_id);
        UserAgentViewModel UserAgentByClient(int ClientId);
    }
}
