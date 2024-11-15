using System;
using System.Collections.Generic;
using System.Text;
using Repositories.IRepositories;
using Entities.Models;
using DAL;
using Entities.ConfigModels;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Entities.ViewModels;
using Utilities;

namespace Repositories.Repositories
{
    public class TelegramRepository : ITelegramRepository
    {
        private readonly TelegramDAL _telegramDAL;
        public TelegramRepository(IOptions<DataBaseConfig> dataBaseConfig)
        {
            _telegramDAL = new TelegramDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
        }
        public GenericViewModel<TelegramDetail> GetTelegramPagingList(string TokenName, int Projectmodel, int statusmodel, int currentPage, int pageSize)
        {
            var model = new GenericViewModel<TelegramDetail>();
            try
            {
                model.ListData = _telegramDAL.GetTelegramPagingList(TokenName, Projectmodel, statusmodel, currentPage, pageSize, out int totalRecord);
                model.PageSize = pageSize;
                model.CurrentPage = currentPage;
                model.TotalRecord = totalRecord;
                model.TotalPage = (int)Math.Ceiling((double)totalRecord / pageSize);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetTelegramPagingList - TelegramRepository: " + ex);
            }

            return model;
        }
        public List<TelegramDetail> GetAllcodeTelegram()
        {
            return _telegramDAL.GetAllTelegram();
        }

        public Task<int> AddTelegram(TelegramDetail telegrammodel)
        {
            var a= _telegramDAL.AddTelegram(telegrammodel);
            return a;
        }
       
        public TelegramDetail GetTelegrambyid(int id)
        {
            return _telegramDAL.GetTelegrambyid(id);
        }
        
    }
}
