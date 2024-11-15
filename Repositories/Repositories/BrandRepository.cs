using DAL;
using Entities.ConfigModels;
using Entities.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories
{
    public class BrandRepository : IBrandRepository
    {
        private readonly ILogger<AttachFileRepository> _logger;
        private readonly BrandDAL _BrandDAL;

        public BrandRepository(IOptions<DataBaseConfig> dataBaseConfig, ILogger<AttachFileRepository> logger)
        {
            _logger = logger;
            _BrandDAL = new BrandDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
        }

        public async Task<IEnumerable<Brand>> GetAll()
        {
            try
            {
                return await _BrandDAL.GetAllAsync();
            }
            catch
            {
                throw;
            }
        }
    }
}
