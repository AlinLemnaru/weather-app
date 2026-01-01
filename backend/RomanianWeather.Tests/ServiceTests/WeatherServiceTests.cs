using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using RomanianWeather.API.Models;
using RomanianWeather.API.Services;
using RomanianWeather.Tests.Stubs;
using System.Linq;

namespace RomanianWeather.Tests.ServiceTests
{
    [TestFixture]
    public class WeatherServiceTests
    {
        private WeatherService _weatherService;

        [SetUp]
        public void Setup()
        {
            _weatherService = new WeatherService(new MeteoApiClientStub());
        }

        [Test]
        public void GetTodayWeather_ShouldReturnStubData()
        {
            var result = _weatherService.GetTodayWeather().ToList();

            Assert.That(result.Count, Is.EqualTo(2), "Expected 2 cities in today's weather data.");
            Assert.That(result[0].City, Is.EqualTo("Bucharest"), "First city should be Bucharest.");
            Assert.That(result[1].City, Is.EqualTo("Cluj-Napoca"), "Second city should be Cluj-Napoca.");
            Assert.That(result[0].WeatherDescription, Is.EqualTo("Cer senin"), "First city's weather description mismatch.");
            Assert.That(result[1].WeatherDescription, Is.EqualTo("Zapada usoara"), "Second city's weather description mismatch.");

            TestContext.WriteLine("GetTodayWeather test passed successfully!");
        }

        [Test]
        public void GetWeatherByCity_ShouldReturnCorrectCity()
        {
            var bucharest = _weatherService.GetWeatherByCity("Bucharest");
            var cluj = _weatherService.GetWeatherByCity("Cluj-Napoca");
            var unknown = _weatherService.GetWeatherByCity("Timisoara");

            Assert.That(bucharest, Is.Not.Null, "Bucharest should not be null.");
            Assert.That(bucharest!.City, Is.EqualTo("Bucharest"), "City name mismatch for Bucharest.");

            Assert.That(cluj, Is.Not.Null, "Cluj-Napoca should not be null.");
            Assert.That(cluj!.City, Is.EqualTo("Cluj-Napoca"), "City name mismatch for Cluj-Napoca.");

            Assert.That(unknown, Is.Null, "Unknown city should return null.");

            TestContext.WriteLine("GetWeatherByCity test passed successfully!");
        }

        [Test]
        public void Get5DayForecast_ShouldReturnStubData()
        {
            var forecast = _weatherService.Get5DayForecast("Bucharest").ToList();

            Assert.That(forecast.Count, Is.EqualTo(5), "Forecast should have 5 days.");

            Assert.That(forecast[0].Date, Is.EqualTo("2026-01-01"), "First day date mismatch.");
            Assert.That(forecast[0].WeatherDescription, Is.EqualTo("Cer senin"), "First day weather mismatch.");

            Assert.That(forecast[4].Date, Is.EqualTo("2026-01-05"), "Last day date mismatch.");
            Assert.That(forecast[4].WeatherDescription, Is.EqualTo("Zapada"), "Last day weather mismatch.");

            TestContext.WriteLine("Get5DayForecast test passed successfully!");
        }

        [Test]
        public void Get5DayForecast_InvalidCity_ShouldReturnEmptyList()
        {
            // Act
            var result = _weatherService.Get5DayForecast("UnknownCity").ToList();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(0), "Forecast list should be empty for an unknown city");
        }

    }
}
