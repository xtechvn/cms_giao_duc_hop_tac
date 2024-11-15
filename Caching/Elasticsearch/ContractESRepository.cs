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
   public class ContractESRepository:ESRepository<ContractNoESViewModel>
    {
        public ContractESRepository(string Host) : base(Host) { }
        public async Task<List<ContractNoESViewModel>> GetContractNoSuggesstion(string txt_search, string index_name = "contract_store")
        {
            List<ContractNoESViewModel> result = new List<ContractNoESViewModel>();
            try
            {
                int top = 30;
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex(index_name);
                var elasticClient = new ElasticClient(connectionSettings);

                var search_response = elasticClient.Search<ContractNoESViewModel>(s => s
                          .Index(index_name + (_company_type.Trim() == "0" ? "" : "_" + _company_type.Trim()))
                          .Size(top)
                          .Query(q =>
                             q.QueryString(qs => qs
                               .DefaultField(s=>s.contractno)
                               .Query("*" + txt_search.ToUpper() + "*")
                            
                           )
                          ));

                if (!search_response.IsValid)
                {
                    return result;
                }
                else
                {
                    result = search_response.Documents as List<ContractNoESViewModel>;
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
