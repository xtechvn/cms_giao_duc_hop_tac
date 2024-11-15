using Elasticsearch.Net;
using Entities.ViewModels.ElasticSearch;
using Nest;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Caching.Elasticsearch
{
   public class ArticleESRepository : ESRepository<ArticleESViewModel>
    {
        public ArticleESRepository(string Host) : base(Host) { }
        public async Task<List<ArticleESViewModel>> GetArticleSuggesstion(string txt_search, string index_name = "news_store")
        {
            List<ArticleESViewModel> result = new List<ArticleESViewModel>();
            try
            {
                int top = 40;
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex(index_name);
                var elasticClient = new ElasticClient(connectionSettings);
                if (txt_search == null)
                {
                    var result_all = elasticClient.Search<ArticleESViewModel>(s => s

                         .Index(index_name + (_company_type.Trim() == "0" ? "" : "_" + _company_type.Trim()))
                          .Size(top)
                          .Query(q => q
                           .QueryString(qs => qs
                               .Fields(new[] { "title" })
                               .Query("*" + txt_search + "*")
                               .Analyzer("standard")
                           ))
                           .Sort(sort => sort.Field(c => c.createdon, SortOrder.Descending))
                          );
                    result = result_all.Documents as List<ArticleESViewModel>;
                    return result;
                }

                var search_response = elasticClient.Search<ArticleESViewModel>(s => s
                        .Index(index_name)
                        .Size(top)
                        .Query(q => q
                         .Bool(qb => qb
                         .Should(m => m
                              .MatchPhrase(m => m
                                  .Field(f => f.title)
                                  .Query("*" + txt_search + "*"))
                         ))
                        ));
                if (!search_response.IsValid)
                {
                    return result;
                }
                else
                {
                    result = search_response.Documents as List<ArticleESViewModel>;
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
