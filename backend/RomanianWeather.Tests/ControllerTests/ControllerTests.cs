using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;
using RomanianWeather.API.Controllers;
using RomanianWeather.API.Services;
using RomanianWeather.API.Models;
using RomanianWeather.Tests.Stubs;
using System.Collections.Generic;
using System.Linq;

namespace RomanianWeather.Tests.ControllerTests
{
    [TestFixture]
    public class ControllerTests
    {
        private MeteoApiClientStub _stubClientCustom = null!;
        private WeatherService _weatherService = null!;
        private WeatherAnalysisService _analysisService = null!;
        private WeatherController _controller = null!;

        [SetUp]
        public void Setup()
        {
            // Custom stub for controller tests
            var customForecast = new Dictionary<string, List<ForecastDay>>
            {
                {
                    "A", new List<ForecastDay>
                    {
                        new ForecastDay { Date = "2026-01-12", TemperatureMin = 0, TemperatureMax = 10, WeatherDescription = "CER SENIN" }
                    }
                }
            };

            var customToday = new List<WeatherSnapshot>
            {
                new WeatherSnapshot
                {
                    City = "A",
                    Temperature = 5,
                    Humidity = 60,
                    WeatherDescription = "CER SENIN"
                }
            };

            _stubClientCustom = new MeteoApiClientStub(customForecast, customToday);
            _weatherService = new WeatherService(_stubClientCustom);
            _analysisService = new WeatherAnalysisService(_weatherService);
            _controller = new WeatherController(_weatherService, _analysisService);
        }

        /// <summary>
        /// C1: Verifies that GetTodayWeather returns the expected data.
        /// </summary>
        [Test]
        public void C1_GetTodayWeather_ShouldReturnOk()
        {
            var result = _controller.GetTodayWeather();
            var ok = result.Result as OkObjectResult;
            Assert.That(ok, Is.Not.Null, "Result should be OkObjectResult");

            var data = ok!.Value as IEnumerable<WeatherSnapshot>;
            Assert.That(data, Is.Not.Null, "OkObjectResult.Value should not be null");
            Assert.That(data!.Count(), Is.EqualTo(1));
        }

        /// <summary>
        /// C2: Verifies that GetCityWeather returns the expected data for an existing city.
        /// </summary>
        [Test]
        public void C2_GetCityWeather_ExistingCity_ShouldReturnOk()
        {
            var result = _controller.GetCityWeather("A");
            var ok = result.Result as OkObjectResult;
            Assert.That(ok, Is.Not.Null, "Result should be OkObjectResult");

            var data = ok!.Value as WeatherSnapshot;
            Assert.That(data, Is.Not.Null, "OkObjectResult.Value should not be null");
            Assert.That(data!.City, Is.EqualTo("A"));
        }

        /// <summary>
        /// C3: Verifies that GetCityWeather returns NotFound for an unknown city.
        /// </summary>
        [Test]
        public void C3_GetCityWeather_UnknownCity_ShouldReturnNotFound()
        {
            var result = _controller.GetCityWeather("Atlantis");
            var notFound = result.Result as NotFoundObjectResult;
            Assert.That(notFound, Is.Not.Null, "Result should be NotFoundObjectResult");

            var value = notFound!.Value;
            Assert.That(value, Is.Not.Null, "NotFoundObjectResult.Value should not be null");
            Assert.That(value!.ToString(), Does.Contain("Atlantis"));
        }

        /// <summary>
        /// C4: Verifies that Get5DayForecast returns the expected data for an existing city.
        /// </summary>
        [Test]
        public void C4_Get5DayForecast_ExistingCity_ShouldReturnOk()
        {
            var result = _controller.Get5DayForecast("A");
            var ok = result.Result as OkObjectResult;
            Assert.That(ok, Is.Not.Null, "Result should be OkObjectResult");

            var data = ok!.Value as IEnumerable<ForecastDay>;
            Assert.That(data, Is.Not.Null, "OkObjectResult.Value should not be null");
            Assert.That(data!.Count(), Is.EqualTo(1));
        }

        /// <summary>
        /// C5: Verifies that Get5DayForecast returns NotFound for an unknown city.
        /// </summary>
        [Test]
        public void C5_Get5DayForecast_UnknownCity_ShouldReturnNotFound()
        {
            var result = _controller.Get5DayForecast("Atlantis");
            var notFound = result.Result as NotFoundObjectResult;
            Assert.That(notFound, Is.Not.Null, "Result should be NotFoundObjectResult");

            var value = notFound!.Value;
            Assert.That(value, Is.Not.Null, "NotFoundObjectResult.Value should not be null");
            Assert.That(value!.ToString(), Does.Contain("Atlantis"));
        }
    }
}
