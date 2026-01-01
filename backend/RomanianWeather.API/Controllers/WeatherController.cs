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

        public WeatherController(WeatherService weatherService)
        {
            _weatherService = weatherService;
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
        public IActionResult Get5DayForecast(string city)
        {
            var forecast = _weatherService.Get5DayForecast(city);
            if (!forecast.Any())
                return NotFound($"No forecast found for city '{city}'");

            return Ok(forecast);
        }

    }
}
