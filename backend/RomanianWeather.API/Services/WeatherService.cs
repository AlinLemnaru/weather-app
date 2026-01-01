using RomanianWeather.API.Interfaces;
using RomanianWeather.API.Models;
using System.Collections.Generic;
using System.Linq;

namespace RomanianWeather.API.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly IWeatherApiClient _weatherApiClient;

        public WeatherService(IWeatherApiClient weatherApiClient)
        {
            _weatherApiClient = weatherApiClient;
        }

        public IEnumerable<WeatherSnapshot> GetTodayWeather()
        {
            return _weatherApiClient.GetTodayWeather();
        }

        public WeatherSnapshot? GetWeatherByCity(string cityName)
        {
            return _weatherApiClient.GetTodayWeather()
                                    .FirstOrDefault(w => w.City.ToLower() == cityName.ToLower());
        }

        public IEnumerable<ForecastDay> Get5DayForecast(string city)
        {
            return _weatherApiClient.Get5DayForecast(city);
        }
    }
}
