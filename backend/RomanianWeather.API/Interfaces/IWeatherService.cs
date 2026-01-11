using RomanianWeather.API.Models;
using System.Collections.Generic;

namespace RomanianWeather.API.Interfaces
{
    public interface IWeatherService
    {
        /// <summary>
        /// Returns all cities with today's weather
        /// </summary>
        IEnumerable<WeatherSnapshot> GetTodayWeather();

        /// <summary>
        /// Returns weather for a specific city, or null if not found
        /// </summary>
        WeatherSnapshot? GetWeatherByCity(string cityName);

        /// <summary>
        /// Returns 5-day weather forecast for a specific city
        /// </summary>
        public IEnumerable<ForecastDay> Get5DayForecast(string city);
    }
}
