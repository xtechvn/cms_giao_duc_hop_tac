using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.ViewModels.HotelBooking
{
    public class HotelBookingCreateBookingModel
    {
        public string propertyId { get; set; }
        public string arrivalDate { get; set; }
        public string departureDate { get; set; }
        public string distributionChannel { get; set; }
        public List<HotelBookingCreateBookingModelreservations> reservations { get; set; }
    }
    public class HotelBookingCreateBookingModelreservations
    {
        public HotelBookingCreateBookingModelroomOccupancy roomOccupancy { get; set; }
        public HotelBookingCreateBookingModeltotalAmount totalAmount { get; set; }
        public bool isSpecialRequestSpecified { get; set; }
        public List<HotelBookingCreateBookingModelspecialRequests> specialRequests { get; set; }
        public bool isProfilesSpecified { get; set; }
        public List<HotelBookingCreateBookingModelprofiles> profiles { get; set; }  
        public bool isRoomRatesSpecified { get; set; }
        public List<HotelBookingCreateBookingModelroomRates> roomRates { get; set; } 
        public bool isPackagesSpecified { get; set; }
        public List<HotelBookingCreateBookingModelpackages> packages { get; set; }

    }
    public class HotelBookingCreateBookingModelpackages
    {
        public string usedDate { get; set; }
        public string packageRefId { get; set; }
        public string ratePlanId { get; set; }
        public int quantity { get; set; }

    }  
    public class HotelBookingCreateBookingModelroomRates
    {
        public string stayDate { get; set; }
        public string roomTypeRefID { get; set; }
        public string allotmentId { get; set; }
        public string ratePlanRefID { get; set; }

    } 
    public class HotelBookingCreateBookingModelprofiles
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string phoneNumber { get; set; }
        public string profileType { get; set; }

    }
    public class HotelBookingCreateBookingModelspecialRequests
    {
        public string requestType { get; set; }
        public string requestContent { get; set; }

    }
    public class HotelBookingCreateBookingModelroomOccupancy
    {
        public int numberOfAdult { get; set; }
        public List<HotelBookingCreateBookingModelroomOccupancyotherOccupancies> otherOccupancies { get; set; }

    } 
    public class HotelBookingCreateBookingModeltotalAmount
    {
        public string amount { get; set; }
        public string currencyCode { get; set; }

    }
    public class HotelBookingCreateBookingModelroomOccupancyotherOccupancies
    {
        public string otherOccupancyRefID { get; set; }
        public string otherOccupancyRefCode { get; set; }
        public int quantity { get; set; }

    }
}
