using Elasticsearch.Net;
using Entities.ViewModels.ElasticSearch;
using Nest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Caching.Elasticsearch
{
    public class FlyBookingESRepository : ESRepository<FlyBookingESViewModel>
    {
        public FlyBookingESRepository(string Host) : base(Host) { }

        public async Task<List<FlyBookingESViewModel>> GetFlyBookingSuggesstion(string txt_search, string index_name = "fly_booking_detail_store")
        {
            List<FlyBookingESViewModel> result = new List<FlyBookingESViewModel>();
            try
            {
                int top = 30;
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex(index_name);
                var elasticClient = new ElasticClient(connectionSettings);
                var search_response = elasticClient.Search<object>(s => s
                           .Index(index_name + (_company_type.Trim() == "0" ? "" : "_" + _company_type.Trim()))
                           .Size(top)
                           .Query(q =>
                              q.QueryString(qs => qs
                                .Fields(new[] { "servicecode" })
                                .Query("*" + txt_search.ToUpper() + "*")
                                .Analyzer("standard")
                            )
                           ));
                if (!search_response.IsValid)
                {
                    var debug = search_response.DebugInformation;
                    return result;
                }
                else
                {
                    result = JsonConvert.DeserializeObject<List<FlyBookingESViewModel>>(JsonConvert.SerializeObject(search_response.Documents));
                    return result;
                }
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        
    }
}
