using RomanianWeather.API.Models;

namespace RomanianWeather.API.Services
{
    public interface IWeatherAnalysisService
    {
        double? GetAverageTemperature(string city);
        double? GetAverageMinTemperature(string city);
        double? GetAverageMaxTemperature(string city);
        ForecastDay? GetHottestDay(string city);
        ForecastDay? GetColdestDay(string city);
        IEnumerable<ForecastDay> GetSunnyDays(string city);
        IEnumerable<ForecastDay> GetCloudyDays(string city);
        string GetTemperatureTrend(string city, double threshold);
        IEnumerable<ForecastDay> GetTopNDaysByMaxTemperature(string city, int n);
        IEnumerable<ForecastDay> GetTopNDaysByMinTemperature(string city, int n);
        IEnumerable<ForecastDay> GetDaysWithTemperatureRange(string city, int minTemp, int maxTemp);
    }
}
