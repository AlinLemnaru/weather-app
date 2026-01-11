using NUnit.Framework;
using RomanianWeather.API.Services;
using RomanianWeather.API.Providers;
using RomanianWeather.API.Interfaces;
using RomanianWeather.Tests.Stubs;
using RomanianWeather.API.Models;

namespace RomanianWeather.Tests.ServiceTests
{
    [TestFixture]
    public class PreliminaryTests
    {
        private IWeatherApiClient _stubClient;

        [SetUp]
        public void Setup()
        {
            // Initialize the stub before each test
            _stubClient = new MeteoApiClientStub();
        }

        /// <summary>
        /// P1: Constructor Test for WeatherService and WeatherAnalysisService
        /// Ensures that both services can be instantiated with a stub client without exceptions.
        /// </summary>
        [Test]
        public void P1_Constructor_ShouldInstantiateServices()
        {
            // Act
            var weatherService = new WeatherService(_stubClient);
            var analysisService = new WeatherAnalysisService(weatherService);

            // Assert
            Assert.IsNotNull(weatherService, "WeatherService should not be null after instantiation.");
            Assert.IsNotNull(analysisService, "WeatherAnalysisService should not be null after instantiation.");
        }

        /// <summary>
        /// P2: Get5DayForecast with Stub
        /// Ensures that the WeatherService returns a non-null list of forecasts for a known city
        /// and that the list contains 5 ForecastDay items by default from the stub.
        /// </summary>
        [Test]
        public void P2_Get5DayForecast_ShouldReturn5Days()
        {
            // Arrange
            var weatherService = new WeatherService(_stubClient);
            string testCity = "Iasi";

            // Act
            var forecast = weatherService.Get5DayForecast(testCity);

            // Assert
            Assert.IsNotNull(forecast, "Forecast list should not be null.");
            Assert.That(forecast.Count(), Is.EqualTo(5), "Forecast list should contain 5 items by default.");
        }

        /// <summary>
        /// P3: GetTodayWeather with Stub
        /// Ensures that the WeatherService returns a non-null list of today's weather snapshots
        /// and that the default stub contains the expected cities.
        /// </summary>
        [Test]
        public void P3_GetTodayWeather_ShouldReturnDefaultCities()
        {
            // Arrange
            var weatherService = new WeatherService(_stubClient);

            // Act
            var todayWeather = weatherService.GetTodayWeather();

            // Assert
            Assert.IsNotNull(todayWeather, "TodayWeather list should not be null.");
            Assert.IsTrue(todayWeather.Any(), "TodayWeather list should contain at least one item.");

            var cities = todayWeather.Select(w => w.City).ToList();
            CollectionAssert.Contains(cities, "Iasi", "TodayWeather should contain 'Iasi'.");
            CollectionAssert.Contains(cities, "Botosani", "TodayWeather should contain 'Botosani'.");
        }

        /// <summary>
        /// P4: Get5DayForecast with unknown city
        /// Ensures that requesting a forecast for a city not in the stub returns an empty list.
        /// </summary>
        [Test]
        public void P4_Get5DayForecast_UnknownCity_ShouldReturnEmptyList()
        {
            // Arrange
            var weatherService = new WeatherService(_stubClient);
            string unknownCity = "UnknownCity";

            // Act
            var forecast = weatherService.Get5DayForecast(unknownCity);

            // Assert
            Assert.IsNotNull(forecast, "Forecast list should not be null.");
            Assert.IsEmpty(forecast, "Forecast list should be empty for an unknown city.");
        }

        /// <summary>
        /// P5: GetTodayWeather with default stub
        /// Verifies that GetTodayWeather() returns a non-empty list of WeatherSnapshot objects.
        /// </summary>
        [Test]
        [Category("Preliminary")]
        [Description("P5 - Verify that GetTodayWeather() returns a non-empty list of WeatherSnapshot objects.")]
        public void P5_GetTodayWeather_ShouldReturnNonEmptyList()
        {
            // Arrange
            var stub = new MeteoApiClientStub();

            // Act
            var result = stub.GetTodayWeather();

            // Assert
            Assert.IsNotNull(result, "The returned list should not be null.");
            Assert.IsNotEmpty(result, "The returned list should contain at least one WeatherSnapshot.");
            foreach (var snapshot in result)
            {
                Assert.IsInstanceOf<WeatherSnapshot>(snapshot, "Each item should be of type WeatherSnapshot.");
                Assert.IsNotNull(snapshot.City, "City should not be null.");
                Assert.IsNotNull(snapshot.Temperature, "Temperature should not be null.");
                Assert.IsNotNull(snapshot.Humidity, "Humidity should not be null.");
                Assert.IsNotNull(snapshot.WeatherDescription, "WeatherDescription should not be null.");
            }
        }

        /// <summary>
        /// P6: Custom Stub Forecast and TodayWeather
        /// Ensures that the MeteoApiClientStub can return custom forecast and today weather values
        /// when they are provided via the constructor.
        /// This verifies that we can dynamically control stub output for more complex tests.
        /// </summary>
        [Test]
        [Category("Preliminary")]
        [Description("P6 - Verify that custom data passed to the stub constructor is returned correctly.")]
        public void P6_CustomStub_ShouldReturnProvidedData()
        {
            // Arrange
            var customForecast = new Dictionary<string, List<ForecastDay>>
            {
                { 
                    "Cluj", new List<ForecastDay>
                    {
                        new ForecastDay 
                        { 
                            Date = "2026-01-12", 
                            TemperatureMin = -5, 
                            TemperatureMax = 3, 
                            WeatherDescription = "CER SENIN" 
                        }
                    }
                }
            };

            var customToday = new List<WeatherSnapshot>
            {
                new WeatherSnapshot 
                { 
                    City = "Cluj", 
                    Temperature = 4, 
                    Humidity = 60, 
                    WeatherDescription = "CER PARTIAL NOROS" 
                }
            };

            var stub = new MeteoApiClientStub(customForecast, customToday);

            // Act
            var forecastResult = stub.Get5DayForecast("Cluj"); // match the key exactly
            var todayResult = stub.GetTodayWeather();

            // Assert
            var firstForecastList = customForecast.Values.First();

            Assert.That(forecastResult.Count(), Is.EqualTo(firstForecastList.Count), 
                "Forecast list count should match the custom data.");
            Assert.That(forecastResult.First().Date, Is.EqualTo(firstForecastList.First().Date), 
                "Forecast date should match the custom data.");
            Assert.That(todayResult.Count(), Is.EqualTo(customToday.Count), 
                "TodayWeather list count should match the custom data.");
            Assert.That(todayResult.First().City, Is.EqualTo(customToday.First().City), 
                "TodayWeather city should match the custom data.");
        }

        /// <summary>
        /// P7: Real API Integration Test for Get5DayForecast (Integration Test)
        /// Ensures that the WeatherService correctly fetches a 5-day forecast for a real city
        /// using the live API. This verifies end-to-end connectivity and parsing logic.
        /// </summary>
        [Test]
        [Category("Preliminary")]
        [Description("P7 - Verify that Get5DayForecast retrieves actual forecast data from the live API.")]
        public void P7_RealApi_Get5DayForecast_ShouldReturnNonEmptyList()
        {
            // Arrange
            var httpClient = new HttpClient(); // real HttpClient for live API
            var apiClient = new MeteoApiClient(httpClient);
            var weatherService = new WeatherService(apiClient);
            string testCity = "Iasi"; // a city we know exists in the API

            // Act
            var forecast = weatherService.Get5DayForecast(testCity);

            // Assert
            Assert.IsNotNull(forecast, "Forecast list should not be null.");
            Assert.IsNotEmpty(forecast, "Forecast list should contain at least one item from the real API.");
            Assert.That(forecast.Count(), Is.LessThanOrEqualTo(5), "Forecast list should contain up to 5 items.");

            foreach (var day in forecast)
            {
                Assert.IsNotNull(day.Date, "Forecast day should have a Date.");
                Assert.IsInstanceOf<double>(day.TemperatureMin, "TemperatureMin should be a double.");
                Assert.IsInstanceOf<double>(day.TemperatureMax, "TemperatureMax should be a double.");
                Assert.IsNotNull(day.WeatherDescription, "WeatherDescription should not be null.");
            }
        }
    }
}
