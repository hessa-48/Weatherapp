using System;

namespace WEATHERAPP.Models
{
    public class WeatherResult
    {
        public int ResultId { get; set; }       // PK
        public int? UserId { get; set; }        // FK, nullable
        public string City { get; set; }
        public int? Humidity { get; set; }
        public double? TempMin { get; set; }
        public double? TempMax { get; set; }
        public DateTime? DateAdded { get; set; }
        public DateTime DateRecorded { get; set; } // not null
    }
}
