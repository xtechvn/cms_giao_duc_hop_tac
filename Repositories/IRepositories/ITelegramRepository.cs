using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Entities.Models;
using Entities.ViewModels;

namespace Repositories.IRepositories
{
    public interface  ITelegramRepository
    {
        List<TelegramDetail> GetAllcodeTelegram();
        GenericViewModel<TelegramDetail> GetTelegramPagingList(string TokenName, int Projectmodel, int statusmodel, int currentPage, int pageSize);
        Task<int> AddTelegram(TelegramDetail telegrammodel);
      
        TelegramDetail GetTelegrambyid(int id);
        
    }
}
