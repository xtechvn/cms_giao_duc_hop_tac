using Elasticsearch.Net;
using Entities.ViewModels.ElasticSearch;
using Nest;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Caching.Elasticsearch
{
    public class NationalESRepository : ESRepository<NationalESViewModel>
    {
        private string index_name = "national_store";
        public NationalESRepository(string Host) : base(Host) { }

        public async Task<List<NationalESViewModel>> SearchNational(string txt_search)
        {
            List<NationalESViewModel> result = new List<NationalESViewModel>();
            try
            {
                if (txt_search == null) txt_search = " ";
                int top = 4000;
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex(index_name);
                var elasticClient = new ElasticClient(connectionSettings);

                var search_response = elasticClient.Search<NationalESViewModel>(s => s
                          .Index(index_name + (_company_type.Trim() == "0" ? "" : "_" + _company_type.Trim()))
                          .Size(top)
                          .Query(q => 
                           q.Bool(
                                qb => qb.Should(
                                    sh => sh.QueryString(m => m
                                    .DefaultField(f => f.name)
                                    .Query("*" + txt_search + "*")),
                                    sh => sh.QueryString(m => m
                                    .DefaultField(f => f.code)
                                    .Query("*" + txt_search + "*"))
                                ))
                          ));

                if (!search_response.IsValid)
                {
                    return result;
                }
                else
                {
                    result = search_response.Documents as List<NationalESViewModel>;
                    return result;
                }
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public async Task<NationalESViewModel> GetNationalByID(string id)
        {
            List<NationalESViewModel> result = new List<NationalESViewModel>();
            try
            {
                int top = 4000;
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex(index_name);
                var elasticClient = new ElasticClient(connectionSettings);

                var search_response = elasticClient.Search<NationalESViewModel>(s => s
                          .Index(index_name + (_company_type.Trim() == "0" ? "" : "_" + _company_type.Trim()))
                          .Size(top)
                          .Query(q => q
                           .Match(qs => qs
                               .Field(s => s.id)
                               .Query(id.Trim())

                           )
                          ));
                if (!search_response.IsValid)
                {
                    return null;
                }
                else
                {
                    result = search_response.Documents as List<NationalESViewModel>;
                    return result.Count > 0 ? result[0] : null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }

        }
    }
}
