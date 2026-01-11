using NUnit.Framework;
using RomanianWeather.API.Services;
using RomanianWeather.API.Providers;
using RomanianWeather.API.Interfaces;
using RomanianWeather.Tests.Stubs;
using RomanianWeather.API.Models;

namespace RomanianWeather.Tests.ServiceTests
{
    [TestFixture]
    public class DomainTests
    {
        private IWeatherApiClient _stubClient;

        [SetUp]
        public void Setup()
        {
            // Initialize the stub before each test
            _stubClient = new MeteoApiClientStub();
        }

        /// <summary>
        /// D1: Get5DayForecast - Minimum/Maximum Temperature Boundary Test
        /// Ensures that the WeatherService correctly returns forecasts containing expected
        /// min and max temperatures from the stub. This allows us to test calculations in
        /// WeatherAnalysisService later (averages, trends, etc.).
        /// Boundary: The stub has min temp = -2 and max temp = 12 in default forecast.
        /// </summary>
        [Test]
        [Category("Domain")]
        [Description("D1 - Verify forecast contains correct min and max temperature boundaries.")]
        public void D1_WeatherServiceGet5DayForecast_TemperatureBoundaries()
        {
            // Arrange
            var stub = new MeteoApiClientStub();
            var weatherService = new WeatherService(stub);
            string city = "Iasi";

            // Act
            var forecast = weatherService.Get5DayForecast(city).ToList();

            // Assert
            Assert.IsNotNull(forecast, "Forecast list should not be null.");
            Assert.IsTrue(forecast.Any(), "Forecast list should contain at least one day.");

            // Verify min/max temperature boundaries from stub
            double minTemp = forecast.Min(f => f.TemperatureMin);
            double maxTemp = forecast.Max(f => f.TemperatureMax);

            Assert.That(minTemp, Is.EqualTo(-2).Within(0.001), "Minimum temperature should match stub boundary.");
            Assert.That(maxTemp, Is.EqualTo(12).Within(0.001), "Maximum temperature should match stub boundary.");
        }

        /// <summary>
        /// D2_WeatherService: GetTodayWeather - Temperature & Humidity Boundaries
        /// Ensures that the WeatherService correctly returns today's weather snapshots
        /// with expected temperature and humidity ranges from the stub.
        /// Boundary: The stub has temperatures between 3 and 5, humidity between 65 and 70.
        /// </summary>
        [Test]
        [Category("Domain")]
        [Description("D2_WeatherService - Verify GetTodayWeather returns correct temperature and humidity boundaries.")]
        public void D2_WeatherService_GetTodayWeather_Boundaries()
        {
            // Arrange
            var stub = new MeteoApiClientStub();
            var weatherService = new WeatherService(stub);

            // Act
            var todayWeather = weatherService.GetTodayWeather().ToList();

            // Assert
            Assert.IsNotNull(todayWeather, "TodayWeather list should not be null.");
            Assert.IsTrue(todayWeather.Any(), "TodayWeather list should contain at least one snapshot.");

            // Verify temperature boundaries
            double minTemp = todayWeather.Min(w => w.Temperature);
            double maxTemp = todayWeather.Max(w => w.Temperature);

            Assert.That(minTemp, Is.EqualTo(3).Within(0.001), "Minimum temperature should match stub boundary.");
            Assert.That(maxTemp, Is.EqualTo(5).Within(0.001), "Maximum temperature should match stub boundary.");

            // Verify humidity boundaries
            int minHum = todayWeather.Min(w => w.Humidity);
            int maxHum = todayWeather.Max(w => w.Humidity);

            Assert.That(minHum, Is.EqualTo(65), "Minimum humidity should match stub boundary.");
            Assert.That(maxHum, Is.EqualTo(70), "Maximum humidity should match stub boundary.");
        }

        /// <summary>
        /// D3_WeatherService: Get5DayForecast with null/empty city
        /// Ensures that calling Get5DayForecast with a null or empty city
        /// throws an ArgumentException with the correct message.
        /// </summary>
        [Test]
        [Category("Domain")]
        [Description("D3_WeatherService - Verify Get5DayForecast throws ArgumentException for null or empty city.")]
        public void D3_WeatherService_Get5DayForecast_NullOrEmptyCity_ShouldThrow()
        {
            // Arrange
            var stub = new MeteoApiClientStub();
            var weatherService = new WeatherService(stub);

            // Act & Assert
            var exEmpty = Assert.Throws<ArgumentException>(() => weatherService.Get5DayForecast(""));
            Assert.That(exEmpty.Message, Does.Contain("City cannot be null or empty"));
            Assert.That(exEmpty.ParamName, Is.EqualTo("city"));
        }

        /// <summary>
        /// D4_WeatherService: Get5DayForecast with unknown city
        /// Ensures that calling Get5DayForecast for a city not in the stub
        /// returns an empty list instead of throwing an exception.
        /// </summary>
        [Test]
        [Category("Domain")]
        [Description("D4_WeatherService - Verify Get5DayForecast returns empty list for unknown city.")]
        public void D4_WeatherService_Get5DayForecast_UnknownCity_ShouldReturnEmpty()
        {
            // Arrange
            var stub = new MeteoApiClientStub();
            var weatherService = new WeatherService(stub);
            string unknownCity = "Atlantis";

            // Act
            var forecast = weatherService.Get5DayForecast(unknownCity).ToList();

            // Assert
            Assert.IsNotNull(forecast, "Forecast list should not be null.");
            Assert.IsEmpty(forecast, "Forecast list should be empty for an unknown city.");
        }

        /// <summary>
        /// D5_WeatherService: GetTodayWeather with empty stub
        /// Ensures that GetTodayWeather returns an empty list gracefully
        /// when the stub provides no data.
        /// This tests the service behavior for edge cases where no weather snapshots are available.
        /// </summary>
        [Test]
        [Category("Domain")]
        [Description("D5_WeatherService - Verify GetTodayWeather returns empty list if stub has no data.")]
        public void D5_WeatherService_GetTodayWeather_EmptyStub_ShouldReturnEmptyList()
        {
            // Arrange
            var emptyStub = new MeteoApiClientStub(
                customForecasts: new Dictionary<string, List<ForecastDay>>(),
                customTodayWeather: new List<WeatherSnapshot>()
            );
            var weatherService = new WeatherService(emptyStub);

            // Act
            var todayWeather = weatherService.GetTodayWeather().ToList();

            // Assert
            Assert.IsNotNull(todayWeather, "TodayWeather list should not be null even if stub is empty.");
            Assert.IsEmpty(todayWeather, "TodayWeather list should be empty when stub provides no data.");
        }

    }
}
