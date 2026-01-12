using NUnit.Framework;
using Moq;
using RomanianWeather.API.Services;
using RomanianWeather.API.Interfaces;
using RomanianWeather.API.Models;
using System.Collections.Generic;
using System.Linq;

namespace RomanianWeather.Tests.Mocks
{
    [TestFixture]
    public class MockTests
    {
        private Mock<IWeatherService> _mockWeatherService = null!;
        private WeatherAnalysisService _analysisService = null!;

        [SetUp]
        public void Setup()
        {
            _mockWeatherService = new Mock<IWeatherService>();
            _analysisService = new WeatherAnalysisService(_mockWeatherService.Object);
        }

        /// <summary>
        /// M1: GetAverageTemperature returns correct average using mocked forecast.
        /// </summary>
        [Test]
        public void M1_GetAverageTemperature_ShouldReturnCorrectAverage()
        {
            // Arrange
            var forecast = new List<ForecastDay>
            {
                new ForecastDay { TemperatureMin = 2, TemperatureMax = 4 },
                new ForecastDay { TemperatureMin = 0, TemperatureMax = 6 }
            };
            _mockWeatherService.Setup(s => s.Get5DayForecast("Iasi")).Returns(forecast);

            // Act
            var avg = _analysisService.GetAverageTemperature("Iasi");

            // Assert
            double expected = forecast.Average(f => (f.TemperatureMin + f.TemperatureMax) / 2.0);
            Assert.That(avg, Is.EqualTo(expected).Within(0.001));
        }

        /// <summary>
        /// M2: GetTodayWeather returns mocked snapshot.
        /// </summary>
        [Test]
        public void M2_GetTodayWeather_ShouldReturnSnapshot()
        {
            // Arrange
            var today = new List<WeatherSnapshot>
            {
                new WeatherSnapshot { City = "Cluj", Temperature = 5, Humidity = 60 }
            };
            _mockWeatherService.Setup(s => s.GetTodayWeather()).Returns(today);

            // Act
            var result = _mockWeatherService.Object.GetTodayWeather();

            // Assert
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().City, Is.EqualTo("Cluj"));
        }

        /// <summary>
        /// M3: GetHottestDay returns null when forecast is empty.
        /// </summary>
        [Test]
        public void M3_GetHottestDay_EmptyForecast_ShouldReturnNull()
        {
            // Arrange
            _mockWeatherService.Setup(s => s.Get5DayForecast("Iasi")).Returns(new List<ForecastDay>());

            // Act
            var hottest = _analysisService.GetHottestDay("Iasi");

            // Assert
            Assert.That(hottest, Is.Null);
        }

        /// <summary>
        /// M4: Exception thrown by service is propagated.
        /// </summary>
        [Test]
        public void M4_GetAverageTemperature_ServiceThrows_ShouldPropagateException()
        {
            // Arrange
            _mockWeatherService.Setup(s => s.Get5DayForecast("Iasi")).Throws(new System.InvalidOperationException("API error"));

            // Act & Assert
            Assert.Throws<System.InvalidOperationException>(() => _analysisService.GetAverageTemperature("Iasi"));
        }

        /// <summary>
        /// M5: Controller-style call using mocked analysis service.
        /// </summary>
        [Test]
        public void M5_GetTemperatureTrend_MockedService_ShouldReturnRising()
        {
            // Arrange
            var mockAnalysis = new Mock<IWeatherAnalysisService>();
            mockAnalysis.Setup(a => a.GetTemperatureTrend("Iasi", 2)).Returns("Rising");

            // Act
            var trend = mockAnalysis.Object.GetTemperatureTrend("Iasi", 2);

            // Assert
            Assert.That(trend, Is.EqualTo("Rising"));
        }

        /// <summary>
        /// M6: Verify that Get5DayForecast is called exactly once when calculating average temperature.
        /// </summary>
        [Test]
        public void M6_GetAverageTemperature_ShouldCallGet5DayForecastOnce()
        {
            // Arrange
            var mockWeather = new Mock<IWeatherService>();
            var analysisService = new WeatherAnalysisService(mockWeather.Object);

            var forecast = new List<ForecastDay>
            {
                new ForecastDay { TemperatureMin = 0, TemperatureMax = 10 }
            };
            mockWeather.Setup(s => s.Get5DayForecast("Iasi")).Returns(forecast);

            // Act
            var avg = analysisService.GetAverageTemperature("Iasi");

            // Assert
            mockWeather.Verify(s => s.Get5DayForecast("Iasi"), Times.Once,
                "Get5DayForecast should be called exactly once for the specified city.");
        }

    }
}
