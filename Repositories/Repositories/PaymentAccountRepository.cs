using DAL;
using Entities.ConfigModels;
using Entities.Models;
using Entities.ViewModels;
using Entities.ViewModels.UserAgent;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Repositories.Repositories
{
   public class PaymentAccountRepository : IPaymentAccountRepository
    {
        private readonly PaymentAccountDAL _PaymentAccountDAL;     
        private readonly UserAgentDAL _UserAgentDAL;     

        public PaymentAccountRepository(IOptions<DataBaseConfig> dataBaseConfig)
        {
            
            _PaymentAccountDAL = new PaymentAccountDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            _UserAgentDAL = new UserAgentDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
        }
        public int CreatePaymentAccount(PaymentAccount model)
        {
            try
            {
                return _PaymentAccountDAL.CreatePaymentAccount(model);
            }catch(Exception ex)
            {
                LogHelper.InsertLogTelegram("CreatePaymentAccount - PaymentAccountRepository: " + ex);
                return 0;
            }
        }
        public GenericViewModel<PaymentAccount> GetAllByClientId(long id, int currentPage,int pageSize)
        {
            var model = new GenericViewModel<PaymentAccount>();
            try
            {
                var data = _PaymentAccountDAL.GetAllByClientId(id);
                model.ListData = data.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
                model.PageSize = pageSize;
                model.CurrentPage = currentPage;
                model.TotalRecord = data.Count;
                model.TotalPage = (int)Math.Ceiling((double)data.Count / pageSize);
                return model;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetAllByClientId - PaymentAccountRepository: " + ex);
                return null;
            }
        }
        public int Setup(PaymentAccount model)
        {
            try
            {
                return _PaymentAccountDAL.Setup(model);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Setup - PaymentAccountRepository: " + ex);
                return 0;
            }
        }
        public int Delete(int Id)
        {
            try
            {
                return _PaymentAccountDAL.Delete(Id);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Delete - PaymentAccountRepository: " + ex);
                return 0;
            }
        }
        public Task<PaymentAccount> getPaymentAccountById(int Id)
        {
            try
            {
                return _PaymentAccountDAL.getPaymentAccountById(Id);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("getPaymentAccountById - PaymentAccountRepository: " + ex);
                return null;
            }
        }
        public int UpdataUserAgent(int ClientId, int UserId,int create_id)
        {
            try
            {
               
                return _UserAgentDAL.UpdataUserAgent(ClientId, UserId, create_id);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdataUserAgent - PaymentAccountRepository: " + ex);
                return 0;
            }
        }
        public UserAgentViewModel UserAgentByClient(int ClientId)
        {
            try
            {
                return _UserAgentDAL.UserAgentByClient(ClientId);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UserAgentByClient - PaymentAccountRepository: " + ex);
                return null;
            }
        }
    }
}
