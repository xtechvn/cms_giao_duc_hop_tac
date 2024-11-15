using Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.IRepositories
{
    public interface IAirlinesRepository
    {
        Task<List<AirPortCode>> GetAirportCode(string txt_search);
        Task<AirPortCode> GetAirportByCode(string code);
        Task<List<Airlines>> SearchAirlines(string txt_search);
        Task<Airlines> GetAirLineByCode(string code);
        Task<List<AirPortCode>> getAllAirportCode();
        Task<List<Airlines>> getAllAirlines();
        Task<GroupClassAirlines> getGroupClassAirlines(string classCode, string airline, string fairtype);
    }
}
