using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HotelAvailabilityManager.Models
{
    public class Booking
    {
        [JsonPropertyName("hotelId")]
        public string HotelId { get; set; }
        [JsonPropertyName("arrival")]
        public string Arrival { get; set; }
        [JsonPropertyName("departure")]
        public string Departure { get; set; }
        [JsonPropertyName("roomType")]
        public string RoomType { get; set; }
        [JsonPropertyName("roomRate")]
        public string RoomRate { get; set; }
    }

}
