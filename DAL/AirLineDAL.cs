using DAL.Generic;
using DAL.StoreProcedure;
using Entities.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace DAL
{
    public class AirLineDAL : GenericService<Airlines>
    {
        private DbWorker _dbWorker;
        public AirLineDAL(string connection) : base(connection)
        {
            _dbWorker = new DbWorker(connection);
        }
        public async Task<List<Airlines>> SearchAirLines(string txt_search = "")
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return await _DbContext.Airlines.Where(x => x.NameVi.ToLower().Contains(txt_search.ToLower()) || x.NameEn.ToLower().Contains(txt_search.ToLower())).ToListAsync();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("SearchAirLines - AirLineDAL: " + ex);
                return null;
            }
        }
        public async Task<Airlines> GetAirLineByCode(string airline)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    if (airline == null) airline="";
                    return await _DbContext.Airlines.Where(x => x.Code.ToLower().Contains(airline.ToLower())).FirstOrDefaultAsync();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetAirLineByCode - AirLineDAL: " + ex);
                return null;
            }
        }
        public async Task<DataTable> getGroupClassAirlines(string classCode, string airline, string fairtype)
        {
            try
            {
                if (classCode.Contains("_ECO")) classCode = "_ECO";
                if (classCode.Contains("_DLX")) classCode = "_DLX";
                if (classCode.Contains("_BOSS")) classCode = "_BOSS";
                if (classCode.Contains("_SBOSS")) classCode = "_SBOSS";
                if (classCode.Contains("_Combo")) classCode = "_Combo";
                if (airline.ToLower().Equals("vu")) classCode = "";
                SqlParameter[] objParam = new SqlParameter[3];
                objParam[0] = new SqlParameter("@classCode", classCode);
                objParam[1] = new SqlParameter("@airline", airline);
                objParam[2] = new SqlParameter("@fairtype", fairtype);
                return _dbWorker.GetDataTable(StoreProcedureConstant.SP_GetGroupClassAirlines, objParam);

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("AirLineDAL getAllAirportCode" + ex);
                return null;
            }
        }
    }
}
