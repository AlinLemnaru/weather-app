using RomanianWeather.API.Interfaces;
using RomanianWeather.API.Models;

namespace RomanianWeather.API.Services
{
    public class WeatherAnalysisService : IWeatherAnalysisService
    {
        private readonly IWeatherService _weatherService;

        public WeatherAnalysisService(IWeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        public double? GetAverageTemperature(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
                throw new ArgumentException("City cannot be null or empty", nameof(city));

            var data = _weatherService.Get5DayForecast(city);
            if (!data.Any()) return null;

            return data.Average(d => (d.TemperatureMin + d.TemperatureMax) / 2.0);
        }

        public double? GetAverageMinTemperature(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
                throw new ArgumentException("City cannot be null or empty", nameof(city));

            var data = _weatherService.Get5DayForecast(city);
            if (!data.Any()) return null;

            return data.Average(d => d.TemperatureMin);
        }

        public double? GetAverageMaxTemperature(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
                throw new ArgumentException("City cannot be null or empty", nameof(city));

            var data = _weatherService.Get5DayForecast(city);
            if (!data.Any()) return null;

            return data.Average(d => d.TemperatureMax);
        }

        public ForecastDay? GetHottestDay(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
                throw new ArgumentException("City cannot be null or empty", nameof(city));

            var data = _weatherService.Get5DayForecast(city);
            if (!data.Any()) return null;

            return data.OrderByDescending(d => d.TemperatureMax).First();
        }

        public ForecastDay? GetColdestDay(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
                throw new ArgumentException("City cannot be null or empty", nameof(city));

            var data = _weatherService.Get5DayForecast(city);
            if (!data.Any()) return null;

            return data.OrderBy(d => d.TemperatureMin).First();
        }

        public IEnumerable<ForecastDay> GetSunnyDays(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
                throw new ArgumentException("City cannot be null or empty", nameof(city));

            var forecast = _weatherService.Get5DayForecast(city);

            if (forecast == null || !forecast.Any())
                return new List<ForecastDay>();

            string[] sunnyKeywords = { "CER SENIN", "CER VARIABIL", "CER PARTIAL NOROS", "CER TEMPORAR NOROS" };

            return forecast.Where(f =>
                sunnyKeywords.Any(k => f.WeatherDescription.Contains(k, StringComparison.OrdinalIgnoreCase))
            ).ToList();
        }

        public IEnumerable<ForecastDay> GetCloudyDays(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
                throw new ArgumentException("City cannot be null or empty", nameof(city));

            var forecast = _weatherService.Get5DayForecast(city);

            if (forecast == null || !forecast.Any())
                return new List<ForecastDay>();

            string[] cloudyKeywords = { "CER MAI NOROS", "CER MAI MULT NOROS" };

            return forecast.Where(f =>
                cloudyKeywords.Any(k => f.WeatherDescription.Contains(k, StringComparison.OrdinalIgnoreCase))
            ).ToList();
        }

        public string GetTemperatureTrend(string city, double threshold)
        {
            if (string.IsNullOrWhiteSpace(city))
                throw new ArgumentException("City cannot be null or empty", nameof(city));

            if (threshold < 0)
                throw new ArgumentOutOfRangeException(nameof(threshold), "Threshold cannot be negative");

            var forecast = _weatherService.Get5DayForecast(city);
            if (forecast == null || !forecast.Any())
                return "No data";

            // Calculate average temperature per day
            var dailyAverages = forecast
                .Select(f => (f.TemperatureMin + f.TemperatureMax) / 2.0)
                .ToList();

            bool rising = true;
            bool falling = true;

            for (int i = 1; i < dailyAverages.Count; i++)
            {
                if (dailyAverages[i] - dailyAverages[i - 1] > threshold)
                    falling = false;
                else if (dailyAverages[i] - dailyAverages[i - 1] < -threshold)
                    rising = false;
                else
                {
                    rising = false;
                    falling = false;
                }
            }

            if (rising) return "Rising";
            if (falling) return "Falling";
            
            return "Stable/Mixed";
        }

        public IEnumerable<ForecastDay> GetTopNDaysByMaxTemperature(string city, int n)
        {
            if (string.IsNullOrWhiteSpace(city))
                throw new ArgumentException("City cannot be null or empty", nameof(city));

            var forecast = _weatherService.Get5DayForecast(city);
            if (forecast == null || !forecast.Any())
                throw new InvalidOperationException($"No forecast found for city '{city}'");

            if (n < 1)
                throw new ArgumentOutOfRangeException(nameof(n), "n must be at least 1");

            // Limit n to available forecast days
            n = Math.Min(n, forecast.Count());

            return forecast
                .OrderByDescending(f => f.TemperatureMax)
                .Take(n);
        }

        public IEnumerable<ForecastDay> GetTopNDaysByMinTemperature(string city, int n)
        {
            if (string.IsNullOrWhiteSpace(city))
                throw new ArgumentException("City cannot be null or empty", nameof(city));

            var forecast = _weatherService.Get5DayForecast(city);
            if (forecast == null || !forecast.Any())
                throw new InvalidOperationException($"No forecast found for city '{city}'");

            if (n < 1)
                throw new ArgumentOutOfRangeException(nameof(n), "n must be at least 1");

            // Limit n to available forecast days
            n = Math.Min(n, forecast.Count());

            return forecast
                .OrderBy(f => f.TemperatureMin)
                .Take(n);
        }

        public IEnumerable<ForecastDay> GetDaysWithTemperatureRange(string city, int minTemp, int maxTemp)
        {
            if (string.IsNullOrWhiteSpace(city))
                throw new ArgumentException("City cannot be null or empty", nameof(city));

            if (minTemp > maxTemp)
                throw new ArgumentException("minTemp cannot be greater than maxTemp");

            var forecast = _weatherService.Get5DayForecast(city);
            if (forecast == null || !forecast.Any())
                return Enumerable.Empty<ForecastDay>();

            return forecast.Where(f =>
                f.TemperatureMin >= minTemp &&
                f.TemperatureMax <= maxTemp);
        }
    }
}
