namespace RomanianWeather.API.Models
{
    public class ForecastDay
    {
        public string Date { get; set; } = string.Empty;             // e.g., "2026-01-02"
        public double TemperatureMin { get; set; }                   // temp_min in Celsius
        public double TemperatureMax { get; set; }                   // temp_max in Celsius
        public string WeatherDescription { get; set; } = string.Empty; // fenomen_descriere
    }
}
