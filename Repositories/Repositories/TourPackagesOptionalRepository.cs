using DAL;
using Entities.ConfigModels;
using Entities.Models;
using Entities.ViewModels.Tour;
using Microsoft.Extensions.Options;
using Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Repositories.Repositories
{
    public class TourPackagesOptionalRepository: ITourPackagesOptionalRepository
    {

        private readonly TourPackagesOptionalDAL _tourPackagesOptionalDAL;

        public TourPackagesOptionalRepository(IOptions<DataBaseConfig> dataBaseConfig)
        {
            _tourPackagesOptionalDAL = new TourPackagesOptionalDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
        }
        public async Task<List<TourPackagesOptionalViewModel>> GetTourPackagesOptional(long tour_id)
        {
            try {
                DataTable dt = await _tourPackagesOptionalDAL.GetTourPackagesOptiona(tour_id);
                if (dt != null && dt.Rows.Count > 0)
                {
                    var data = dt.ToList<TourPackagesOptionalViewModel>();
                    return data;
                }
                return null;
            }
            catch(Exception ex)
            {
                LogHelper.InsertLogTelegram("GetTourPackagesOptional - TourPackagesOptionalRepository: " + ex.ToString());
                return null;
            }
        }
        public async Task<int> InsertTourPackagesOptional(TourPackagesOptional model)
        {
            try
            {
                return await _tourPackagesOptionalDAL.InsertTourPackagesOptional(model);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("InsertTourPackagesOptional - TourPackagesOptionalRepository: " + ex);
                return 0;
            }
        }
        public async Task<int> UpdateTourPackagesOptional(TourPackagesOptional model)
        {
            try
            {
                return await _tourPackagesOptionalDAL.UpdateTourPackagesOptional(model);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("InsertTourPackagesOptional - TourPackagesOptionalRepository: " + ex);
                return 0;
            }
        }
        public async Task<int> DeleteTourPackagesOptional(int Id)
        {
            try
            {

                return await _tourPackagesOptionalDAL.DeleteTourPackagesOptional(Id);

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("DeleteTourPackagesOptional - TourPackagesOptionalDAL: " + ex);
                return 0;
            }
        }
    }

}

