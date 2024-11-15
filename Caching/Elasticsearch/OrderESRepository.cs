using Elasticsearch.Net;
using Entities.ViewModels.ElasticSearch;
using ENTITIES.ViewModels.ElasticSearch;
using Nest;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Caching.Elasticsearch
{
   public class OrderESRepository :ESRepository<OrderElasticsearchViewModel>
    {
        public OrderESRepository(string Host) : base(Host) { }
        public async Task<List<OrderElasticsearchViewModel>> GetOrderNoSuggesstion(string txt_search, string index_name = "order_store")
        {
            List<OrderElasticsearchViewModel> result = new List<OrderElasticsearchViewModel>();
            try
            {
                int top = 30;
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex(index_name);
                var elasticClient = new ElasticClient(connectionSettings);

                var search_response = elasticClient.Search<OrderElasticsearchViewModel>(s => s
                          .Index(index_name + (_company_type.Trim() == "0" ? "" : "_" + _company_type.Trim()))
                          .Size(top)
                          .Query(q =>
                             q.QueryString(qs => qs
                               .Fields(new[] { "orderno" })
                               .Query("*" + txt_search.ToUpper() + "*")
                               .Analyzer("standard")
                           )
                          ));

                if (!search_response.IsValid)
                {
                    return result;
                }
                else
                {
                    result = search_response.Documents as List<OrderElasticsearchViewModel>;
                    return result;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetOrderNoSuggesstion - OrderESRepository. " + ex);
                return null;
            }

        }
        public async Task<List<OrderElasticsearchViewModel>> GetOrderNoSuggesstion2(string txt_search, int SysTemType, string index_name = "order_store")
        {
            List<OrderElasticsearchViewModel> result = new List<OrderElasticsearchViewModel>();
            try
            {
                int top = 30;
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex(index_name);
                var elasticClient = new ElasticClient(connectionSettings);

                var search_response = elasticClient.Search<OrderElasticsearchViewModel>(s => s
                          .Index(index_name + (_company_type.Trim() == "0" ? "" : "_" + _company_type.Trim()))
                          .Size(top)
                          .Query(q =>
                           q.Bool(
                               qb => qb.Must(
                                  q => q.Term("systemtype", SysTemType),
                                   sh => sh.QueryString(qs => qs
                                   .Fields(new[] { "orderno" })
                                   .Query("*" + txt_search.ToUpper() + "*")
                                   .Analyzer("standard")

                            )
                           )
                          )));

                if (!search_response.IsValid)
                {
                    return result;
                }
                else
                {
                    result = search_response.Documents as List<OrderElasticsearchViewModel>;
                    return result;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetOrderNoSuggesstion - OrderESRepository. " + ex);
                return null;
            }

        }
        public async Task<List<ESHotelBookingCodeViewModel>> GetHotelBookingCode(string txt_search, int Type, string index_name = "hotel_booking_code_store")
        {
            List<ESHotelBookingCodeViewModel> result = new List<ESHotelBookingCodeViewModel>();
            try
            {
                int top = 30;
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex(index_name);
                var elasticClient = new ElasticClient(connectionSettings);

                var search_response = elasticClient.Search<ESHotelBookingCodeViewModel>(s => s
                          .Index(index_name + (_company_type.Trim() == "0" ? "" : "_" + _company_type.Trim()))
                          .Size(top)
                          .Query(q =>
                           q.Bool(
                               qb => qb.Must(
                                  q => q.Term("hotel_type", Type),
                                   q => q.Term("isdelete", "false"),
                                   sh => sh.QueryString(qs => qs
                                   .Fields(new[] { "bookingcode" })
                                   .Query("*" + txt_search.ToUpper() + "*")
                                   .Analyzer("standard")

                            )
                           )
                          )));

                if (!search_response.IsValid)
                {
                    return result;
                }
                else
                {
                    result = search_response.Documents as List<ESHotelBookingCodeViewModel>;
                    return result;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetOrderNoSuggesstion - OrderESRepository. " + ex);
                return null;
            }

        }
    }
}
