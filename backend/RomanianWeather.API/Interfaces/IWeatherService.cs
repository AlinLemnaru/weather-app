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
    }
}
