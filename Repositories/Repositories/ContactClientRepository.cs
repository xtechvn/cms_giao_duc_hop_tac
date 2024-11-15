using DAL;
using DAL.StoreProcedure;
using Entities.ConfigModels;
using Entities.Models;
using Entities.ViewModels;
using Entities.ViewModels.Contract;
using Microsoft.Extensions.Options;
using Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace Repositories.Repositories
{
    public class ContactClientRepository : IContactClientRepository
    {
        
        private readonly ContactClientDAL _contactClientDAL;
        private readonly OrderDAL _OrderDAL;


        public ContactClientRepository(IOptions<DataBaseConfig> dataBaseConfig)
        {

            _contactClientDAL = new ContactClientDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
            _OrderDAL = new OrderDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
           
        }

        public Task<ContactClient> GetByBookingId(long hotel_booking_id)
        {
            return _contactClientDAL.GetByBookingId(hotel_booking_id);
        }

        public async Task<long> UpdateContactClient(ContactClient client)
        {
            try
            {
                var contactClient_Id = _contactClientDAL.UpdateContactClient(client).Result;
                if (contactClient_Id == 0) { return -1; }
                var model = _OrderDAL.GetByOrderId((long)client.OrderId);
                model.ContactClientId = Convert.ToInt32(contactClient_Id.ToString());
                var a =await _OrderDAL.UpdataOrder(model);
                return a;
            }
            catch(Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateContactClient - ContactClientRepository: " + ex.ToString());
                return 0;
            }
           
        }
        public ContactClient GetByContactClientId(long Id)
        {
            try
            {
                return _contactClientDAL.GetByContactClientId(Id);


            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetByContactClientId - ContactClientRepository: " + ex);
                return null;
            }
        }
    }
}
