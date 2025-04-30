using HotelAvailabilityManager.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HotelAvailabilityManager.Services
{
    public class AvailabilityService : IAvailabilityService
    {
        private List<Hotel> hotels;
        private List<Booking> bookings;
        
        public AvailabilityService(string HotelFilePath, string BookingFilePath) {
        
            hotels = GetItemsByFilePath<Hotel>(HotelFilePath);
            bookings = GetItemsByFilePath<Booking>(BookingFilePath);
        }

        public List<T> GetItemsByFilePath<T>(string dataFilePath)
        {
            return JsonSerializer.Deserialize<List<T>>(File.ReadAllText(dataFilePath));
        }
        public int GetAvailability(string hotelId, string roomType, DateTime start, DateTime end)
        {
            var hotel = hotels.FirstOrDefault(h => h.Id == hotelId);
            if (hotel == null) return 0;

            int totalRooms = hotel.Rooms.Count(r => r.RoomType == roomType);

            var dates = Enumerable.Range(0, (end - start).Days)
                                   .Select(offset => start.AddDays(offset));

            int minAvailability = int.MaxValue;

            foreach (var date in dates)
            {
                int bookedCount = bookings.Count(b => b.HotelId == hotelId && b.RoomType == roomType &&
                    DateTime.ParseExact(b.Arrival, "yyyyMMdd", CultureInfo.InvariantCulture) < date &&
                    DateTime.ParseExact(b.Departure, "yyyyMMdd", CultureInfo.InvariantCulture) > date);

                int available = totalRooms - bookedCount;
                minAvailability = Math.Min(minAvailability, available);
            }

            return minAvailability;

        }
        public List<string> SearchAvailability(string hotelId, int daysAhead, string roomType, DateTime today)
        {
            DateTime endDate = today.AddDays(daysAhead);
            List<string> results = new List<string>();

            DateTime? rangeStart = null;
            DateTime? rangeEnd = null;
            int? currentAvailability = null;

            for (DateTime date = today; date < endDate; date = date.AddDays(1))
            {
                int availability = GetAvailability(hotelId, roomType, date, date.AddDays(1));

                if (availability > 0)
                {
                    if (rangeStart == null)
                    {
                        rangeStart = date;
                        currentAvailability = availability;
                    }
                    else if (availability != currentAvailability)
                    {
                        results.Add($"({rangeStart:yyyyMMdd}-{rangeEnd:yyyyMMdd}, {currentAvailability})");
                        rangeStart = date;
                        currentAvailability = availability;
                    }
                    rangeEnd = date.AddDays(1);
                }
                else if (rangeStart != null)
                {
                    results.Add($"({rangeStart:yyyyMMdd}-{rangeEnd:yyyyMMdd}, {currentAvailability})");
                    rangeStart = null;
                    rangeEnd = null;
                    currentAvailability = null;
                }
            }

            if (rangeStart != null && rangeEnd != null)
            {
                results.Add($"({rangeStart:yyyyMMdd}-{rangeEnd:yyyyMMdd}, {currentAvailability})");
            }

            return results;

        }


    }
}
