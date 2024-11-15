using DAL;
using Entities.ConfigModels;
using Entities.Models;
using Entities.ViewModels;
using Entities.ViewModels.PlaygroundDetai;
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
   public class PlaygroundDetaiRepository: IPlaygroundDetaiRepository
    {
        private readonly PlaygroundDetailDAL _playgroundDetailDAL;



        public PlaygroundDetaiRepository(IOptions<DataBaseConfig> dataBaseConfig)
        {

            _playgroundDetailDAL = new PlaygroundDetailDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
        }
        public async Task<GenericViewModel<PlaygroundDetaiViewModel>> GetListPlayground(PlaygroundDetaiSeachViewModel model, int currentPage, int pageSize )
        {
            var datamdoel = new GenericViewModel<PlaygroundDetaiViewModel>();
            try
            {
                DataTable dt =await _playgroundDetailDAL.GetListPlayground(model);
                if(dt != null && dt.Rows.Count > 0){

                    var data = dt.ToList<PlaygroundDetaiViewModel>();
                   
                    datamdoel.ListData = data;
                    datamdoel.CurrentPage = model.PageIndex;
                    datamdoel.PageSize = model.PageSize;
                    datamdoel.TotalRecord = Convert.ToInt32(dt.Rows[0]["TotalRow"]);
                    datamdoel.TotalPage = (int)Math.Ceiling((double)datamdoel.TotalRecord / datamdoel.PageSize);
                    return datamdoel;
                }
                
                return null;
                
               
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("PlaygroundDetailDAL - AllCodeDAL. " + ex);
                return null;
            }
        }
        public async Task<int> InsertPlaygroundDetail(PlaygroundDetaiViewModel model)
        {
            try
            {
               
                return await _playgroundDetailDAL.InsertPlaygroundDetail(model);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("PlaygroundDetailDAL - AllCodeDAL. " + ex);
                return 0;
            }
        }
        public async Task<int> UpdatePlaygroundDetail(PlaygroundDetaiViewModel model)
        {
            try
            {
                return await _playgroundDetailDAL.UpdatePlaygroundDetail(model);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("PlaygroundDetailDAL - PlaygroundDetaiRepository. " + ex);
                return 0;
            }
        }
        public async Task<PlaygroundDetaiViewModel> GetDetailPlaygroundDetail(long id)
        {
            try
            {
                DataTable dt = await _playgroundDetailDAL.GetDetailPlaygroundDetail(id);
                if (dt != null && dt.Rows.Count > 0)
                {
                    var data = dt.ToList<PlaygroundDetaiViewModel>();
                    return data[0];
                }

                    
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("PlaygroundDetailDAL - PlaygroundDetaiRepository. " + ex);
              
            }
            return null;
        }
        public async Task<List<PlaygroundDetail> > GetPlaygroundDetailbyCode(string Code)
        {
            try
            {
                return await _playgroundDetailDAL.GetPlaygroundDetailbyCode(Code);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetPlaygroundDetailbyCode - PlaygroundDetaiRepository. " + ex);

            }
            return null;
        }
        public async Task<int> DeletePlaygroundDetail(long id)
        {
            try
            {
                return await _playgroundDetailDAL.DeletePlaygroundDetail(id);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdatePlaygroundDetail - PlaygroundDetaiRepository. " + ex);
                return 0;
            }
        }
    }
}
