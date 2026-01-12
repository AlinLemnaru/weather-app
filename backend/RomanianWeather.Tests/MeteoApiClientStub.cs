using RomanianWeather.API.Interfaces;
using RomanianWeather.API.Models;

public class MeteoApiClientStub : IWeatherApiClient
{
    private readonly Dictionary<string, List<ForecastDay>> _cityForecasts;
    private readonly List<WeatherSnapshot> _todayWeather;

    public MeteoApiClientStub(
        Dictionary<string, List<ForecastDay>>? customForecasts = null,
        List<WeatherSnapshot>? customTodayWeather = null)
    {
        // Use provided city forecasts or default
        _cityForecasts = customForecasts ?? new Dictionary<string, List<ForecastDay>>(StringComparer.OrdinalIgnoreCase)
        {
            { "Iasi", new List<ForecastDay>
                {
                    new ForecastDay { Date = DateTime.Now.ToString("yyyy-MM-dd"), TemperatureMin = 0, TemperatureMax = 10, WeatherDescription = "CER SENIN" },
                    new ForecastDay { Date = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"), TemperatureMin = 2, TemperatureMax = 12, WeatherDescription = "CER PARTIAL NOROS" },
                    new ForecastDay { Date = DateTime.Now.AddDays(2).ToString("yyyy-MM-dd"), TemperatureMin = 1, TemperatureMax = 11, WeatherDescription = "CER VARIABIL" },
                    new ForecastDay { Date = DateTime.Now.AddDays(3).ToString("yyyy-MM-dd"), TemperatureMin = -1, TemperatureMax = 9, WeatherDescription = "CER MAI NOROS" },
                    new ForecastDay { Date = DateTime.Now.AddDays(4).ToString("yyyy-MM-dd"), TemperatureMin = -2, TemperatureMax = 8, WeatherDescription = "CER MAI MULT NOROS" }
                }
            }
        };

        _todayWeather = customTodayWeather ?? new List<WeatherSnapshot>
        {
            new WeatherSnapshot { City = "Iasi", Temperature = 5, Humidity = 70, WeatherDescription = "CER SENIN" },
            new WeatherSnapshot { City = "Botosani", Temperature = 3, Humidity = 65, WeatherDescription = "CER PARTIAL NOROS" }
        };
    }

    public IEnumerable<ForecastDay> Get5DayForecast(string city)
    {
        return _cityForecasts.TryGetValue(city, out var forecast) ? forecast : new List<ForecastDay>();
    }

    public IEnumerable<WeatherSnapshot> GetTodayWeather()
    {
        return _todayWeather;
    }
}
