using DAL.Generic;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace DAL
{
   public class AccountClientDAL : GenericService<AccountClient>
    {
        public AccountClientDAL(string connection) : base(connection) { }
        public int CreateAccountClient(AccountClient model)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {

                    var deta = _DbContext.AccountClient.Add(model);
                    _DbContext.SaveChanges();


                }
                return 1;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CreateAccountClient - AccountClientDAL: " + ex);
                return 0;
            }
        }
        public long GetMainAccountClientByClientId(long client_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {

                    var main_account =  _DbContext.AccountClient.FirstOrDefault(x => x.ClientId == client_id);
                    if (main_account != null)
                    {
                        return main_account.Id;
                    }


                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetMainAccountClientByClientId - AccountClientDAL: " + ex.ToString());

            }
            return -1;

        }
        public AccountClient AccountClientByClientId(long client_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {

                    var main_account = _DbContext.AccountClient.FirstOrDefault(x => x.ClientId == client_id);
                    if (main_account != null)
                    {
                        return main_account;
                    }


                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("AccountClientByClientId - AccountClientDAL: " + ex.ToString());

            }
            return null;

        }
        public async Task<int> UpdataAccountClient(AccountClient model)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {

                 
                    _DbContext.AccountClient.Update(model);
                     await _DbContext.SaveChangesAsync();

                    return 1;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdataAccountClient - AccountClientDAL: " + ex.ToString());

            }
            return 0;

        }
        
    }
}
