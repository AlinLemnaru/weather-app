using NUnit.Framework;
using RomanianWeather.API.Providers;
using RomanianWeather.API.Models;
using System.Collections.Generic;
using System.Linq;

namespace RomanianWeather.Tests.Stubs
{
    [TestFixture]
    public class StubTests
    {
        private MeteoApiClientStub _defaultStub;
        private MeteoApiClientStub _customStub;

        [SetUp]
        public void Setup()
        {
            // Default stub
            _defaultStub = new MeteoApiClientStub();

            // Custom stub with one short city and one long city
            var customForecast = new Dictionary<string, List<ForecastDay>>
            {
                {
                    "A", new List<ForecastDay>
                    {
                        new ForecastDay { Date = "2026-01-12", TemperatureMin = 0, TemperatureMax = 10, WeatherDescription = "CER SENIN" }
                    }
                },
                {
                    "Abcdefghijklmnopqrstuv", new List<ForecastDay>
                    {
                        new ForecastDay { Date = "2026-01-12", TemperatureMin = 1, TemperatureMax = 11, WeatherDescription = "CER PARTIAL NOROS" }
                    }
                }
            };

            var customToday = new List<WeatherSnapshot>
            {
                new WeatherSnapshot { City = "A", Temperature = 5, Humidity = 60, WeatherDescription = "CER SENIN" },
                new WeatherSnapshot { City = "Abcdefghijklmnopqrstuv", Temperature = 6, Humidity = 65, WeatherDescription = "CER PARTIAL NOROS" }
            };

            _customStub = new MeteoApiClientStub(customForecast, customToday);
        }

        /// <summary>
        /// S1: Default stub Get5DayForecast
        /// Ensures that the default stub returns 5 forecast days for a known city.
        /// </summary>
        [Test]
        [Category("Stub")]
        [Description("S1 - Verify default stub Get5DayForecast returns 5 items for known city.")]
        public void S1_DefaultStub_Get5DayForecast_ShouldReturn5Days()
        {
            var forecast = _defaultStub.Get5DayForecast("Iasi").ToList();
            Assert.IsNotNull(forecast);
            Assert.That(forecast.Count, Is.EqualTo(5));
        }

        /// <summary>
        /// S2: Default stub GetTodayWeather
        /// Ensures that the default stub returns non-empty today weather list with expected cities.
        /// </summary>
        [Test]
        [Category("Stub")]
        [Description("S2 - Verify default stub GetTodayWeather returns non-empty list.")]
        public void S2_DefaultStub_GetTodayWeather_ShouldReturnNonEmpty()
        {
            var todayWeather = _defaultStub.GetTodayWeather().ToList();
            Assert.IsNotNull(todayWeather);
            Assert.IsNotEmpty(todayWeather);

            var cities = todayWeather.Select(w => w.City).ToList();
            CollectionAssert.Contains(cities, "Iasi");
            CollectionAssert.Contains(cities, "Botosani");
        }

        /// <summary>
        /// S3: Default stub Get5DayForecast for unknown city
        /// Ensures that requesting forecast for unknown city returns empty list.
        /// </summary>
        [Test]
        [Category("Stub")]
        [Description("S3 - Verify default stub Get5DayForecast returns empty for unknown city.")]
        public void S3_DefaultStub_Get5DayForecast_UnknownCity_ShouldReturnEmpty()
        {
            var forecast = _defaultStub.Get5DayForecast("Atlantis").ToList();
            Assert.IsNotNull(forecast);
            Assert.IsEmpty(forecast);
        }

        /// <summary>
        /// S4: Custom stub Get5DayForecast
        /// Ensures that the custom stub returns correct forecast for custom cities.
        /// </summary>
        [Test]
        [Category("Stub")]
        [Description("S4 - Verify custom stub Get5DayForecast returns correct forecast for custom city.")]
        public void S4_CustomStub_Get5DayForecast_ShouldReturnCustomData()
        {
            var forecastShort = _customStub.Get5DayForecast("A").ToList();
            Assert.IsNotNull(forecastShort);
            Assert.That(forecastShort.Count, Is.EqualTo(1));
            Assert.That(forecastShort[0].TemperatureMin, Is.EqualTo(0));
            Assert.That(forecastShort[0].TemperatureMax, Is.EqualTo(10));

            var forecastLong = _customStub.Get5DayForecast("Abcdefghijklmnopqrstuv").ToList();
            Assert.IsNotNull(forecastLong);
            Assert.That(forecastLong.Count, Is.EqualTo(1));
            Assert.That(forecastLong[0].TemperatureMin, Is.EqualTo(1));
            Assert.That(forecastLong[0].TemperatureMax, Is.EqualTo(11));
        }

        /// <summary>
        /// S5: Custom stub with empty data
        /// Ensures that initializing the stub with empty forecast and today weather returns empty lists.
        /// </summary>
        [Test]
        [Category("Stub")]
        [Description("S5 - Verify custom stub with empty data returns empty lists.")]
        public void S5_CustomStub_EmptyData_ShouldReturnEmptyLists()
        {
            var emptyStub = new MeteoApiClientStub(
                customForecasts: new Dictionary<string, List<ForecastDay>>(),
                customTodayWeather: new List<WeatherSnapshot>()
            );

            var forecast = emptyStub.Get5DayForecast("AnyCity").ToList();
            var today = emptyStub.GetTodayWeather().ToList();

            Assert.IsNotNull(forecast);
            Assert.IsEmpty(forecast);

            Assert.IsNotNull(today);
            Assert.IsEmpty(today);
        }
    }
}
