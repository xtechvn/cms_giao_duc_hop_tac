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
   public class ProgramsESRepository : ESRepository<ProgramsViewModel>
    {
        public ProgramsESRepository(string Host) : base(Host) { }
        public async Task<List<ProgramsViewModel>> GetProgramsSuggesstion(string txt_search, string index_name = "program_store")
        {
            List<ProgramsViewModel> result = new List<ProgramsViewModel>();
            try
            {
                int top = 30;
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex(index_name);
                var elasticClient = new ElasticClient(connectionSettings);
                if (txt_search == null)
                {
                    var result_all = elasticClient.Search<CustomerESViewModel>(s => s
                          .Index(index_name + (_company_type.Trim() == "0" ? "" : "_" + _company_type.Trim()))
                          .Size(30)
                          .Query(q => q.MatchAll()

                           ));
                    if (!result_all.IsValid)
                    {
                        var debug = result_all.DebugInformation;
                        return result;
                    }
                    else
                    {
                        result = JsonConvert.DeserializeObject<List<ProgramsViewModel>>(JsonConvert.SerializeObject(result_all.Documents));
                        return result;
                    }
                }
                var search_response = elasticClient.Search<object>(s => s
                           .Index(index_name)
                           .Size(top)
                           .Query(q =>
                              q.QueryString(qs => qs
                                .Fields(new[] { "servicename","programname","programcode" })
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
                    result = JsonConvert.DeserializeObject<List<ProgramsViewModel>>(JsonConvert.SerializeObject(search_response.Documents));
                    return result;
                }
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public async Task<List<ProgramsViewModel>> GetProgramsSuggesstionByHotelid(string txt_search, string index_name = "program_store")
        {
            List<ProgramsViewModel> result = new List<ProgramsViewModel>();
            try
            {
                int top = 30;
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex(index_name);
                var elasticClient = new ElasticClient(connectionSettings);

                var search_response = elasticClient.Search<ProgramsViewModel>(s => s
                       .Index(index_name + (_company_type.Trim() == "0" ? "" : "_" + _company_type.Trim()))
                       .Size(top)
                       .Query(q => q
                        .Match(qs => qs
                            .Field(s => s.hotelid)
                            .Query(txt_search)
                           )
                          ));
                if (!search_response.IsValid)
                {
                    var debug = search_response.DebugInformation;
                    return result;
                }
                else
                {
                    result = JsonConvert.DeserializeObject<List<ProgramsViewModel>>(JsonConvert.SerializeObject(search_response.Documents));
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
