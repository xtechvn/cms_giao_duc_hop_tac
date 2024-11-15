using DAL;
using Entities.ConfigModels;
using Entities.Models;
using Entities.ViewModels;
using Entities.ViewModels.Request;
using Microsoft.Extensions.Options;
using Repositories.IRepositories;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Utilities;

namespace Repositories.Repositories
{
    public class RequestRepository: IRequestRepository
    {

        private readonly RequestDAL requestDAL;
       
        public RequestRepository(IOptions<DataBaseConfig> _dataBaseConfig)
        {
            requestDAL = new RequestDAL(_dataBaseConfig.Value.SqlServer.ConnectionString);
        }
        public async Task<GenericViewModel<RequestViewModel>> GetPagingList(RequestSearchModel searchModel)
        {
            var model = new GenericViewModel<RequestViewModel>();
            model.CurrentPage = searchModel.PageIndex;
            model.PageSize = searchModel.PageSize;
            try
            {
                DataTable dt = await requestDAL.GetPagingList(searchModel);
                if (dt != null && dt.Rows.Count > 0)
                {
                    model.ListData = dt.ToList<RequestViewModel>();
                    model.TotalRecord = Convert.ToInt32(dt.Rows[0]["TotalRow"]);
                    model.TotalPage = (int)Math.Ceiling((double)model.TotalRecord / model.PageSize);
                }
                return model;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetPagingList - RequestRepository: " + ex);
            }
            return null;
        }
        public async Task<long> UpdateRequest(Request Model)
        {
  
            try
            {
                return await requestDAL.UpdateRequest(Model);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetPagingList - RequestRepository: " + ex);
            }
            return -1;
        } 
        public async Task<Request> GetDetailRequest(long RequestId)
        {
  
            try
            {
                return await requestDAL.GetDetailRequest(RequestId);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetPagingList - RequestRepository: " + ex);
            }
            return null;
        }
    }
}
