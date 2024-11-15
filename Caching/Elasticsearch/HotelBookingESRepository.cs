using Elasticsearch.Net;
using Entities.ViewModels;
using Entities.ViewModels.ElasticSearch;
using Nest;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Caching.Elasticsearch
{
   public class HotelBookingESRepository : ESRepository<HotelBookingESViewModel>
    {
        private string index_name = "hotel_booking_store";
        public HotelBookingESRepository(string Host) : base(Host) { }

        public async Task<List<HotelBookingESViewModel>> GetListProduct(string txt_search)
        {
            List<HotelBookingESViewModel> result = new List<HotelBookingESViewModel>();
            try
            {
                int top = 4000;
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex(index_name);
                var elasticClient = new ElasticClient(connectionSettings);

                var search_response = elasticClient.Search<HotelBookingESViewModel>(s => s
                          .Index(index_name + (_company_type.Trim() == "0" ? "" : "_" + _company_type.Trim()))
                          .Size(top)
                          .Query(q => q
                           .QueryString(qs => qs
                               .Fields(new[] { "servicecode"})
                               .Query("*" + txt_search + "*")
                               .Analyzer("standard")
                           )
                          ));

                if (!search_response.IsValid)
                {
                    return result;
                }
                else
                {
                    result = search_response.Documents as List<HotelBookingESViewModel>;
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
