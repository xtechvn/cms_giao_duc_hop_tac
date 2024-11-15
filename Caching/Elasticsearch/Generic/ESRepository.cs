using Elasticsearch.Net;
using Entities.ViewModels;
using Entities.ViewModels.ElasticSearch;
using Nest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Utilities;

namespace Caching.Elasticsearch
{
    //https://www.steps2code.com/post/how-to-use-elasticsearch-in-csharp
    public class ESRepository<TEntity> : IESRepository<TEntity> where TEntity : class
    {
        public static string _ElasticHost;
        public static string  _company_type;
        public ESRepository(string Host)
        {
            _ElasticHost = Host;
            AppSettings _appconfig = new AppSettings();
            using (StreamReader r = new StreamReader("appsettings.json"))
            {
                string json = r.ReadToEnd();
                _appconfig = JsonConvert.DeserializeObject<AppSettings>(json);
                 _company_type = _appconfig.CompanyType;
            }
               
        }

       

        /// <summary>

        /// <summary>

        /// </summary>
        /// <param name="indexName"></param>
        /// <param name="value">Giá trị cần tìm kiếm</param>
        /// <param name="field_name">Tên cột cần search</param>
        /// <returns></returns>
        public TEntity FindById(string indexName, object value, string field_name = "id")
        {
            try
            {
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex("hotel");
                var elasticClient = new ElasticClient(connectionSettings);

                var searchResponse = elasticClient.Search<object>(s => s
                    .Index(indexName+ (_company_type.Trim() == "0" ? "" : "_" + _company_type.Trim()))
                    .Query(q => q.Term(field_name, value))
                );

                var JsonObject = JsonConvert.SerializeObject(searchResponse.Documents);
                var ListObject = JsonConvert.DeserializeObject<List<TEntity>>(JsonObject);
                return ListObject.FirstOrDefault();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Lấy ra chi tiết sản phẩm theo code
        /// </summary>
        /// <param name="index_name"></param>
        /// <param name="value_search"></param>
        /// <returns></returns>


        public int UpSert(TEntity entity, string indexName,string Type)
        {
            try
            {
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex(Type);
                var elasticClient = new ElasticClient(connectionSettings);
                var indexResponse = elasticClient.Index(new IndexRequest<TEntity>(entity, indexName + (_company_type.Trim() == "0" ? "" : "_" + _company_type.Trim())));

                if (!indexResponse.IsValid)
                {
                    
                    return 0;
                }

                return 1;
            }
            catch
            {
                return 0;
            }
        }

        public async Task<int> UpSertAsync(TEntity entity, string indexName,string Type)
        {
            try
            {
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex(Type);
                var elasticClient = new ElasticClient(connectionSettings);

                var indexResponse = elasticClient.Index(entity, i => i.Index(indexName + (_company_type.Trim() == "0" ? "" : "_" + _company_type.Trim())));
                if (!indexResponse.IsValid)
                {
                    // If the request isn't valid, we can take action here
                }

                var indexResponseAsync = await elasticClient.IndexDocumentAsync(entity);
            }
            catch
            {

            }
            return 0;
        }

        public async Task<List<HotelESViewModel>> GetListProduct(string index_name,string txtsearch,string Type)
        {
            List<HotelESViewModel> result = new List<HotelESViewModel>();
            try
            {
                int top = 4000;
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex(Type);
                var elasticClient = new ElasticClient(connectionSettings);

                var search_response = elasticClient.Search<HotelESViewModel>(s => s
                          .Index(index_name + (_company_type.Trim() == "0" ? "" : "_" + _company_type.Trim()))
                          .Size(top)
                          .Query(q => q
                           .QueryString(qs => qs
                               .Fields(new[] { "name", "state", "street", "city" })
                               .Query("*"+txtsearch+"*")
                               .Analyzer("standard")
                           )
                          ));

                if (!search_response.IsValid)
                {
                        return result;
                }
                else
                {
                    result = search_response.Documents as List<HotelESViewModel>;
                    return result;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            
        }
       
        public async Task<List<HotelESViewModel>> GetListHotelByCity(string index_name, string city,string Type)
        {
            List<HotelESViewModel> result = new List<HotelESViewModel>();
            try
            {
                int top = 4000;
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex(Type);
                var elasticClient = new ElasticClient(connectionSettings);
                city = CommonHelper.RemoveUnicode(city);
                var search_response = elasticClient.Search<HotelESViewModel>(s => s
                          .Index(index_name + (_company_type.Trim() == "0" ? "" : "_" + _company_type.Trim()))
                          .Size(top)
                          .Query(q => q
                           .QueryString(qs => qs
                               .Fields(new[] { "city", "state","street" })
                               .Query("*" + city + "*")
                               .Analyzer("standard")
                           )
                          ));

                if (!search_response.IsValid)
                {
                    return result;
                }
                else
                {
                    result = search_response.Documents as List<HotelESViewModel>;
                    return result;
                }
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public async Task<List<HotelESViewModel>> GetListHotelByGroupName(string index_name, string group_name,string Type)
        {
            List<HotelESViewModel> result = new List<HotelESViewModel>();
            try
            {
                int top = 4000;
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex(Type);
                var elasticClient = new ElasticClient(connectionSettings);

                var search_response = elasticClient.Search<HotelESViewModel>(s => s
                          .Index(index_name + (_company_type.Trim() == "0" ? "" : "_" + _company_type.Trim()))
                          .Size(top)
                          .Query(q => q
                           .Match(qs => qs
                               .Field(s=>s.hotelid)
                               .Query("*" + group_name + "*")
                               .Analyzer("standard")
                           )
                          ));

                if (!search_response.IsValid)
                {
                    return result;
                }
                else
                {
                    result = search_response.Documents as List<HotelESViewModel>;
                    return result;
                }
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public bool DeleteHotelByID( string hotel_id, string index_name,string Type)
        {
            string key_es_id = string.Empty;
            try
            {
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex(Type);
                var elasticClient = new ElasticClient(connectionSettings);
                var result = elasticClient.DeleteByQuery<object>(sd => sd
                              .Index(index_name + (_company_type.Trim() == "0" ? "" : "_" + _company_type.Trim()))
                              .Query(q => q
                                  .Match(m => m.Field("hotelid").Query(hotel_id)
                                  )));
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("DeleteHotelByID key_es_id= " + key_es_id + " hotelid" + hotel_id + " Exception" + ex.ToString());
                return false;
            }
        }
        public bool DeleteOrderID(string hotel_id, string index_name, string Type)
        {
            string key_es_id = string.Empty;
            try
            {
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex(Type);
                var elasticClient = new ElasticClient(connectionSettings);
                var result = elasticClient.DeleteByQuery<object>(sd => sd
                              .Index(index_name + (_company_type.Trim() == "0" ? "" : "_" + _company_type.Trim()))
                              .Query(q => q
                                  .Match(m => m.Field("orderId").Query(hotel_id)
                                  )));
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("DeleteOrderID key_es_id= " + key_es_id + "OrderId" + hotel_id + " Exception" + ex.ToString());
                return false;
            }
        }
        public bool DeleteDepositHistoryID(string hotel_id, string index_name, string Type)
        {
            string key_es_id = string.Empty;
            try
            {
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex(Type);
                var elasticClient = new ElasticClient(connectionSettings);
                var result = elasticClient.DeleteByQuery<object>(sd => sd
                              .Index(index_name + (_company_type.Trim() == "0" ? "" : "_" + _company_type.Trim()))
                              .Query(q => q
                                  .Match(m => m.Field("id").Query(hotel_id)
                                  )));
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("DeleteDepositHistoryID key_es_id= " + key_es_id + " id" + hotel_id + " Exception" + ex.ToString());
                return false;
            }
        }
      
        
        
    }

}
