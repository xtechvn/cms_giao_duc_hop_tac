using DAL;
using Entities.ConfigModels;
using Entities.Models;
using Microsoft.Extensions.Options;
using Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories.Repositories
{
    public class FlightSegmentRepository : IFlightSegmentRepository
    {
        private readonly FlightSegmentDAL flightSegmentDAL;
        private readonly IOptions<DataBaseConfig> dataBaseConfig;

        public FlightSegmentRepository(IOptions<DataBaseConfig> _dataBaseConfig)
        {
            flightSegmentDAL = new FlightSegmentDAL(_dataBaseConfig.Value.SqlServer.ConnectionString);
            dataBaseConfig = _dataBaseConfig;
        }

        public FlightSegment GetByFlyBookingDetailId(long flyBookingDetailId)
        {
            return flightSegmentDAL.GetFlyBookingDetailId(flyBookingDetailId);
        }

        public List<FlightSegment> GetByFlyBookingDetailIds(List<long> flyBookingDetailIds)
        {
            return flightSegmentDAL.GetFlyBookingDetailIds(flyBookingDetailIds);
        }
    }
}
