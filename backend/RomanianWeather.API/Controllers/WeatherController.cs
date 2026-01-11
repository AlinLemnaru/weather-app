using Microsoft.AspNetCore.Mvc;
using RomanianWeather.API.Services;
using System.Xml.Linq;
using RomanianWeather.API.Models;

namespace RomanianWeather.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly WeatherService _weatherService;
        private readonly WeatherAnalysisService _analysisService;

        public WeatherController(WeatherService weatherService, WeatherAnalysisService analysisService)
        {
            _weatherService = weatherService;
            _analysisService = analysisService;
        }

        [HttpGet("today")]
        public ActionResult<IEnumerable<WeatherSnapshot>> GetTodayWeather()
        {
            var data = _weatherService.GetTodayWeather();
            return Ok(data);
        }

        [HttpGet("today/{city}")]
        public ActionResult<WeatherSnapshot> GetCityWeather(string city)
        {
            var data = _weatherService.GetTodayWeather()
                                      .FirstOrDefault(w => w.City.ToLower() == city.ToLower());
            if (data == null)
                return NotFound($"Weather for city '{city}' not found.");

            return Ok(data);
        }

        [HttpGet("forecast/{city}")]
        public ActionResult<IEnumerable<ForecastDay>> Get5DayForecast(string city)
        {
            var forecast = _weatherService.Get5DayForecast(city);
            if (!forecast.Any())
                return NotFound($"No forecast found for city '{city}'");

            return Ok(forecast);
        }

        [HttpGet("average")]
        public ActionResult<double?> GetAverageTemperature([FromQuery] string city)
        {
            var avg = _analysisService.GetAverageTemperature(city);
            if (avg == null) return NotFound($"No forecast found for city '{city}'");
            return Ok(avg);
        }

        [HttpGet("average-min")]
        public ActionResult<double?> GetAverageMinTemperature([FromQuery] string city)
        {
            var avg = _analysisService.GetAverageMinTemperature(city);
            if (avg == null) return NotFound($"No forecast found for city '{city}'");
            return Ok(avg);
        }

        [HttpGet("average-max")]
        public ActionResult<double?> GetAverageMaxTemperature([FromQuery] string city)
        {
            var avg = _analysisService.GetAverageMaxTemperature(city);
            if (avg == null) return NotFound($"No forecast found for city '{city}'");
            return Ok(avg);
        }

        [HttpGet("hottest")]
        public ActionResult<ForecastDay?> GetHottestDay([FromQuery] string city)
        {
            var hottest = _analysisService.GetHottestDay(city);
            if (hottest == null) return NotFound($"No forecast found for city '{city}'");
            return Ok(hottest);
        }

        [HttpGet("coldest")]
        public ActionResult<ForecastDay?> GetColdestDay([FromQuery] string city)
        {
            var coldest = _analysisService.GetColdestDay(city);
            if (coldest == null) return NotFound($"No forecast found for city '{city}'");
            return Ok(coldest);
        }

        [HttpGet("sunny-days")]
        public ActionResult<IEnumerable<ForecastDay>> GetSunnyDays([FromQuery] string city)
        {
            var sunny = _analysisService.GetSunnyDays(city);
            if (sunny == null) return NotFound($"No forecast found for city '{city}'");
            return Ok(sunny);
        }

        [HttpGet("cloudy-days")]
        public ActionResult<IEnumerable<ForecastDay>> GetCloudyDays([FromQuery] string city)
        {
            var sunny = _analysisService.GetCloudyDays(city);
            if (sunny == null) return NotFound($"No forecast found for city '{city}'");
            return Ok(sunny);
        }

        [HttpGet("trend")]
        public ActionResult<string?> GetTemperatureTrend(string city, double threshold)
        {
            var trend = _analysisService.GetTemperatureTrend(city, threshold);
            if (trend == null) return NotFound($"No forecast found for city '{city}'");
            return Ok(trend);
        }

        [HttpGet("top-n-hottest-max")]
        public ActionResult<IEnumerable<ForecastDay>> GetTopNDaysByMaxTemperature(string city, int n)
        {
            var topNHottest = _analysisService.GetTopNDaysByMaxTemperature(city, n);
            if (topNHottest == null) return NotFound($"No forecast found for city '{city}'");
            return Ok(topNHottest);
        }

        [HttpGet("top-n-coldest-min")]
        public ActionResult<IEnumerable<ForecastDay>> GetTopNDaysByMinTemperature(string city, int n)
        {
            var topNColdest = _analysisService.GetTopNDaysByMinTemperature(city, n);
            if (topNColdest == null) return NotFound($"No forecast found for city '{city}'");
            return Ok(topNColdest);
        }

        [HttpGet("temperature-range")]
        public ActionResult<IEnumerable<ForecastDay>> GetDaysWithTemperatureRange(string city, int minTemp, int maxTemp)
        {
            var tempRange = _analysisService.GetDaysWithTemperatureRange(city, minTemp, maxTemp);
            if (tempRange == null) return NotFound($"No forecast found for city '{city}'");
            return Ok(tempRange);
        }
    }
}
