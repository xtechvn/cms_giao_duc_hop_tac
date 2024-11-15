using DAL;
using Entities.ConfigModels;
using Entities.Models;
using Microsoft.Extensions.Options;
using Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Repositories.Repositories
{
    public class AirlinesRepository : IAirlinesRepository
    {
        private readonly AirPortCodeDAL airPortCodeDAL;
        private readonly AirLineDAL airLineDAL;
        private readonly IOptions<DataBaseConfig> dataBaseConfig;

        public AirlinesRepository(IOptions<DataBaseConfig> _dataBaseConfig)
        {
            airPortCodeDAL = new AirPortCodeDAL(_dataBaseConfig.Value.SqlServer.ConnectionString);
            airLineDAL = new AirLineDAL(_dataBaseConfig.Value.SqlServer.ConnectionString);
            dataBaseConfig = _dataBaseConfig;

        }

        public async Task<List<AirPortCode>> GetAirportCode(string txt_search)
        {
            try
            {
                if (txt_search == null || txt_search.Trim() == "")
                {
                    return airPortCodeDAL.GetAll();

                }
                else
                {
                    return await airPortCodeDAL.SearchAirPortCode(txt_search);

                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetAirportCode - FlyBookingDetailRepository: " + ex);
                return null;
            }
        }
        
        public async Task<AirPortCode> GetAirportByCode(string code)
        {
            try
            {
                return await airPortCodeDAL.GetAirPortByCode(code);

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetAirportByCode - FlyBookingDetailRepository: " + ex);
                return null;
            }
        }
        public async Task<List<Airlines>> SearchAirlines(string txt_search)
        {
            try
            {
                if (txt_search == null || txt_search.Trim() == "")
                {
                    return airLineDAL.GetAll();
                }
                else
                {
                    return await airLineDAL.SearchAirLines(txt_search);

                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SearchAirlines - FlyBookingDetailRepository: " + ex);
                return null;
            }
        }
        public async Task<Airlines> GetAirLineByCode(string code)
        {
            try
            {
                return await airLineDAL.GetAirLineByCode(code);

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetAirLineByCode - FlyBookingDetailRepository: " + ex);
                return null;
            }
        }

        public async Task<List<AirPortCode>> getAllAirportCode()
        {
            try
            {
                return await airPortCodeDAL.getAllAirportCode();
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("getAllAirportCode - AirportCodeRepository: " + ex);
                return null;
            }
        }
        public async Task<List<Airlines>> getAllAirlines()
        {
            try
            {
                return await airPortCodeDAL.getAllAirlines();
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("getAllAirportCode - AirportCodeRepository: " + ex);
                return null;
            }
        }
       
    public async Task<GroupClassAirlines> getGroupClassAirlines(string classCode, string airline, string fairtype)
        {
            try
            {
                DataTable dt =await airLineDAL.getGroupClassAirlines(classCode, airline, fairtype);
                var data = dt.ToList<GroupClassAirlines>().FirstOrDefault();
                return data;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("getAllAirportCode - AirportCodeRepository: " + ex);
                return null;
            }
        }
    }
}
