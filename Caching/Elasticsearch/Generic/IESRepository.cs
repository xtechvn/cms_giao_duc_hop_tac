using Entities.Models;
using Entities.ViewModels;
using Entities.ViewModels.ElasticSearch;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Caching.Elasticsearch
{
    public interface IESRepository<TEntity> where TEntity : class
    {
        /// <summary>
        /// string StrEsConfig = $"{elasticConfig.Host}:{elasticConfig.Port}";
        /// IESRepository<EsProductViewModel> _ESRepository = new ESRepository<EsProductViewModel>(StrEsConfig);
        /// var id = "0134190440_1";
        /// var Model = _ESRepository.FindById("product", id);
        /// </summary>
        /// <param name="indexName"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        TEntity FindById(string indexName, object value, string field_name);

        int UpSert(TEntity entity, string indexName, string Type);
        Task<int> UpSertAsync(TEntity entity, string indexName, string Type);
        Task<List<HotelESViewModel>> GetListProduct(string index_name, string txtsearch, string Type);
        
        bool DeleteHotelByID(string hotel_id, string index_name, string Type);
        bool DeleteOrderID(string hotel_id, string index_name, string Type);
        bool DeleteDepositHistoryID(string hotel_id, string index_name, string Type);
        Task<List<HotelESViewModel>> GetListHotelByCity(string index_name, string city, string Type);
        Task<List<HotelESViewModel>> GetListHotelByGroupName(string index_name, string group_name, string Type);
    }
}
