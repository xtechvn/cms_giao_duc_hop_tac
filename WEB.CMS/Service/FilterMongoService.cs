using Entities.ViewModels.Funding;
using Entities.ViewModels.TransferSms;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Utilities;

namespace GDHT.CMS.Service
{
    public class FilterMongoService
    {
        public List<SystemLog> SearchHistoryBackend(long keyId, List<string> objectType)
        {
            var listDebtBrickLog = new List<SystemLog>();
            try
            {
                var db = MongodbService.GetDatabase();

                var collection = db.GetCollection<SystemLog>("BACKEND");
                var filter = Builders<SystemLog>.Filter.Empty;
                filter &= Builders<SystemLog>.Filter.Eq(n => n.KeyID, keyId.ToString());
                filter &= Builders<SystemLog>.Filter.In(n => n.ObjectType, objectType);
                filter &= Builders<SystemLog>.Filter.Eq(n => n.CompanyType, LogHelper.CompanyTypeInt);
                var S = Builders<SystemLog>.Sort.Descending("_id");
                listDebtBrickLog = collection.Find(filter).Sort(S).ToList();
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SearchHistoryBackend - FilterMongoService. " + JsonConvert.SerializeObject(ex));
            }
            return listDebtBrickLog;
        }

    }
}
