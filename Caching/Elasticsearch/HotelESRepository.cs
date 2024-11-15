using Elasticsearch.Net;
using Entities.ViewModels;
using Nest;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Caching.Elasticsearch
{
    public class HotelESRepository : ESRepository<HotelESViewModel>
    {
        private string index_name = "hotel_store";
        public HotelESRepository(string Host) : base(Host) { }

        public async Task<List<HotelESViewModel>> GetListProduct(string txtsearch)
        {
            List<HotelESViewModel> result = new List<HotelESViewModel>();
            try
            {
                int top = 4000;
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex(index_name);
                var elasticClient = new ElasticClient(connectionSettings);
                if (txtsearch == null) txtsearch = "";
                var search_response = elasticClient.Search<HotelESViewModel>(s => s
                          .Index(index_name + (_company_type.Trim() == "0" ? "" : "_" + _company_type.Trim()))
                          .From(0)
                          .Size(top)
                          .Query(q => 
                            q.Bool(
                                qb=>qb.Should(
                                    sh => sh.QueryString(m => m
                                    .DefaultField(f => f.name)
                                    .Query("*"+txtsearch+ "*")),
                                    sh => sh.QueryString(m => m
                                    .DefaultField(f => f.city)
                                    .Query("*" + txtsearch + "*")),
                                    sh => sh.QueryString(m => m
                                    .DefaultField(f => f.street)
                                    .Query("*" + txtsearch + "*"))

                                ))
                           )
                          );
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
        public async Task<HotelESViewModel> GetHotelByID(string id)
        {
            List<HotelESViewModel> result = new List<HotelESViewModel>();
            try
            {
                int top = 4000;
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex(index_name);
                var elasticClient = new ElasticClient(connectionSettings);

                var search_response = elasticClient.Search<HotelESViewModel>(s => s
                          .Index(index_name + (_company_type.Trim() == "0" ? "" : "_" + _company_type.Trim()))
                          .Size(top)
                          .Query(q => q
                           .Match(qs => qs
                               .Field(s => s.hotelid)
                               .Query(id)

                           )
                          ));
                if (!search_response.IsValid)
                {
                    return null;
                }
                else
                {
                    result = search_response.Documents as List<HotelESViewModel>;
                    return result.Count>0? result[0]: null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public async Task<HotelESViewModel> GetHotelByID(int hotel_id)
        {
            List<HotelESViewModel> result = new List<HotelESViewModel>();
            try
            {
                int top = 4000;
                var nodes = new Uri[] { new Uri(_ElasticHost) };
                var connectionPool = new StaticConnectionPool(nodes);
                var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming().DefaultIndex(index_name);
                var elasticClient = new ElasticClient(connectionSettings);

                var search_response = elasticClient.Search<HotelESViewModel>(s => s
                          .Index(index_name + (_company_type.Trim() == "0" ? "" : "_" + _company_type.Trim()))
                          .Size(top)
                          .Query(q => q
                           .Match(qs => qs
                               .Field(s => s.id)
                               .Query(hotel_id.ToString())

                           )
                          ));
                if (!search_response.IsValid)
                {
                    return null;
                }
                else
                {
                    result = search_response.Documents as List<HotelESViewModel>;
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
