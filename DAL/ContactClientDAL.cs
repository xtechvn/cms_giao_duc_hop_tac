using DAL.Generic;
using DAL.StoreProcedure;
using Entities.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace DAL
{
    public class ContactClientDAL : GenericService<Client>
    {
        private static DbWorker _DbWorker;
        public ContactClientDAL(string connection) : base(connection)
        {
            _DbWorker = new DbWorker(connection);
        }

        public ContactClient GetByClientId(long clientId)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.ContactClient.AsNoTracking().FirstOrDefault(s => s.ClientId == clientId);
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetByClientId - ContactClientDAL: " + ex);
                return null;
            }
        }
        public async Task<ContactClient> GetByBookingId(long hotel_booking_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var booking= await _DbContext.ContactClient.AsNoTracking().FirstOrDefaultAsync(s => s.Id == hotel_booking_id);
                    if(booking !=null && booking.Id > 0)
                    {
                        var order = await _DbContext.Order.AsNoTracking().FirstOrDefaultAsync(s => s.OrderId == booking.OrderId);
                        return _DbContext.ContactClient.AsNoTracking().FirstOrDefault(s => s.ClientId == order.ContactClientId);
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetByBookingId - ContactClientDAL: " + ex);
                return null;
            }
        }
        public async Task<long> UpdateContactClient (ContactClient client)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    //---- Contact Client:
                    if (client.Id > 0)
                    {
                        var contact_client = await _DbContext.ContactClient.AsNoTracking().FirstOrDefaultAsync(x => x.Id == client.Id);
                        if (contact_client != null && contact_client.Id > 0)
                        {
                            client.Id = contact_client.Id;
                            _DbContext.ContactClient.Update(client);
                            await _DbContext.SaveChangesAsync();
                        }
                       
                        return client.Id;
                    }
                    var client_exists = await _DbContext.AccountClient.AsNoTracking().FirstOrDefaultAsync(x => x.ClientId == client.ClientId);
                    if (client_exists != null)
                    {
                        CreateContactClients(client, client_exists.Id);
                       
                    }
                    return client.Id;
                }

            }
            catch(Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateContactClient - ContactClientDAL: " + ex);
                return -1;
            }
        }
        public int CreateContactClients(ContactClient obj_contact_client, long account_client_id)
        {
            try
            {

                SqlParameter[] objParam_cclient = new SqlParameter[6];
                objParam_cclient[0] = new SqlParameter("@Name", obj_contact_client.Name);
                objParam_cclient[1] = new SqlParameter("@Mobile", obj_contact_client.Mobile);
                objParam_cclient[2] = new SqlParameter("@Email", obj_contact_client.Email);
                objParam_cclient[3] = new SqlParameter("@CreateDate", obj_contact_client.CreateDate);
                objParam_cclient[4] = new SqlParameter("@AccountClientId", account_client_id);
                objParam_cclient[5] = new SqlParameter("@OrderId", obj_contact_client.OrderId);
                var id = _DbWorker.ExecuteNonQuery(StoreProcedureConstant.CreateContactClients, objParam_cclient);
                obj_contact_client.Id = id;
                return id;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CreateContactClients - ContactClientDAL. " + ex);
                return -1;
            }
        }
        public ContactClient GetByContactClientId(long Id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return _DbContext.ContactClient.AsNoTracking().FirstOrDefault(s => s.Id == Id);
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetByContactClientId - ContactClientDAL: " + ex);
                return null;
            }
        }
    }
}
