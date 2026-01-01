using RomanianWeather.API.Models;
using System.Collections.Generic;

namespace RomanianWeather.API.Interfaces
{
    public interface IWeatherApiClient
    {
        IEnumerable<WeatherSnapshot> GetTodayWeather();

        IEnumerable<ForecastDay> Get5DayForecast(string city);
    }
}
