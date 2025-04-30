using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelAvailabilityManager.Services
{
    public interface IAvailabilityService
    {
        int GetAvailability(string hotelId, string roomType, DateTime start, DateTime end);
        List<string> SearchAvailability(string hotelId, int daysAhead, string roomType, DateTime today);
    }
}
