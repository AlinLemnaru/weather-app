using RomanianWeather.API.Models;
using RomanianWeather.API.Interfaces;
using System.Collections.Generic;

namespace RomanianWeather.Tests.Stubs
{
    public class MeteoApiClientStub : IWeatherApiClient
    {
        public IEnumerable<WeatherSnapshot> GetTodayWeather()
        {
            return new List<WeatherSnapshot>
            {
                new WeatherSnapshot { City = "Bucharest", Temperature = 5, Humidity = 80, WeatherDescription = "Cer senin" },
                new WeatherSnapshot { City = "Cluj-Napoca", Temperature = -1, Humidity = 70, WeatherDescription = "Zapada usoara" }
            };
        }

        public IEnumerable<ForecastDay> Get5DayForecast(string city)
        {
            if (city == "Bucharest" || city == "Cluj-Napoca")
            {
                return new List<ForecastDay>
            {
                new ForecastDay { Date = "2026-01-01", WeatherDescription = "Cer senin" },
                new ForecastDay { Date = "2026-01-02", WeatherDescription = "Cer variabil" },
                new ForecastDay { Date = "2026-01-03", WeatherDescription = "Ploaie slaba" },
                new ForecastDay { Date = "2026-01-04", WeatherDescription = "Ninsorica" },
                new ForecastDay { Date = "2026-01-05", WeatherDescription = "Zapada" }
            };
            }

            // Return empty for unknown cities
            return new List<ForecastDay>();
        }
    }
}
