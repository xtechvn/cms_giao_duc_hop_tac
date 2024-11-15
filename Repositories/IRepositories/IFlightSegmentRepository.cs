using Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories.IRepositories
{
    public interface IFlightSegmentRepository
    {
        FlightSegment GetByFlyBookingDetailId(long flyBookingDetailId);
        List<FlightSegment> GetByFlyBookingDetailIds(List<long> flyBookingDetailIds);
    }
}
