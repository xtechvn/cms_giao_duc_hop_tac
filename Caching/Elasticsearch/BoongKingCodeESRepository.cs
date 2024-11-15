using Elasticsearch.Net;
using Entities.ViewModels.ElasticSearch;
using Nest;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Caching.Elasticsearch
{
   public class BoongKingCodeESRepository : ESRepository<BoongKingCodeESViewModel>
    {
        public BoongKingCodeESRepository(string Host) : base(Host) { }
        public async Task<List<BoongKingCodeESViewModel>> BoongKingCodeSuggesstion(string txt_search, string index_name = "hotel_booking_code")
        {
            List<BoongKingCodeESViewModel> result = new List<BoongKingCodeESViewModel>();
            try
            {
                int top = 30;
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex(index_name);
                var elasticClient = new ElasticClient(connectionSettings);
              
                var search_response = elasticClient.Search<BoongKingCodeESViewModel>(s => s
                         .Index(index_name + (_company_type.Trim() == "0" ? "" : "_" + _company_type.Trim()))
                          .Size(top)
                          .Query(q =>
                           q.Bool(
                               qb => qb.Must(
                                  q => q.Term("isdelete", false),
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
                    result = search_response.Documents as List<BoongKingCodeESViewModel>;
                    return result;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("BoongKingCodeSuggesstion - BoongKingCodeESRepository. " + ex);
                return null;
            }

        }
    }
}
