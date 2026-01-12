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
        private IWeatherApiClient _stubClientCustom;

        [SetUp]
        public void Setup()
        {
            // Initialize the stub before each test
            _stubClient = new MeteoApiClientStub();

            // Custom boundary stub data
            // Custom boundary stub data
            var customForecast = new Dictionary<string, List<ForecastDay>>
            {
                {
                    "A", new List<ForecastDay>
                    {
                        new ForecastDay { Date = DateTime.Now.ToString("yyyy-MM-dd"), TemperatureMin = 0, TemperatureMax = 5, WeatherDescription = "CER SENIN" },
                        new ForecastDay { Date = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"), TemperatureMin = -1, TemperatureMax = 6, WeatherDescription = "CER PARTIAL NOROS" },
                        new ForecastDay { Date = DateTime.Now.AddDays(2).ToString("yyyy-MM-dd"), TemperatureMin = -2, TemperatureMax = 7, WeatherDescription = "CER VARIABIL" },
                        new ForecastDay { Date = DateTime.Now.AddDays(3).ToString("yyyy-MM-dd"), TemperatureMin = -3, TemperatureMax = 8, WeatherDescription = "CER MAI NOROS" },
                        new ForecastDay { Date = DateTime.Now.AddDays(4).ToString("yyyy-MM-dd"), TemperatureMin = -4, TemperatureMax = 9, WeatherDescription = "CER MAI MULT NOROS" }
                    }
                },
                {
                    "Abcdefghijklmnopqrstuv", new List<ForecastDay>
                    {
                        new ForecastDay { Date = DateTime.Now.ToString("yyyy-MM-dd"), TemperatureMin = 1, TemperatureMax = 10, WeatherDescription = "CER SENIN" },
                        new ForecastDay { Date = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"), TemperatureMin = 2, TemperatureMax = 11, WeatherDescription = "CER PARTIAL NOROS" },
                        new ForecastDay { Date = DateTime.Now.AddDays(2).ToString("yyyy-MM-dd"), TemperatureMin = 3, TemperatureMax = 12, WeatherDescription = "CER VARIABIL" },
                        new ForecastDay { Date = DateTime.Now.AddDays(3).ToString("yyyy-MM-dd"), TemperatureMin = 4, TemperatureMax = 13, WeatherDescription = "CER MAI NOROS" },
                        new ForecastDay { Date = DateTime.Now.AddDays(4).ToString("yyyy-MM-dd"), TemperatureMin = 5, TemperatureMax = 14, WeatherDescription = "CER MAI MULT NOROS" }
                    }
                }
            };
            
            var customToday = new List<WeatherSnapshot>
            {
                new WeatherSnapshot
                {
                    City = "A",
                    Temperature = 0,
                    Humidity = 50,
                    WeatherDescription = "CER SENIN"
                },
                new WeatherSnapshot
                {
                    City = "Abcdefghijklmnopqrstuv",
                    Temperature = 10,
                    Humidity = 60,
                    WeatherDescription = "CER PARTIAL NOROS"
                }
            };

            // Initialize the custom stub
            _stubClientCustom = new MeteoApiClientStub(customForecast, customToday);

        }

        //----------------------------------------------------------------
        // WeatherService Domain Tests
        //----------------------------------------------------------------

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
            var weatherService = new WeatherService(_stubClient);
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
            var weatherService = new WeatherService(_stubClient);

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
            var weatherService = new WeatherService(_stubClient);

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

        /// <summary>
        /// D6_WeatherService: Get5DayForecast with empty stub
        /// Ensures that Get5DayForecast returns an empty list gracefully
        /// when the stub provides no data.
        /// This tests the service behavior for edge cases where no forecasts are available.
        /// </summary>
        [Test]
        [Category("Domain")]
        [Description("D6_WeatherService - Verify Get5DayForecast returns empty list if stub has no data.")]
        public void D6_WeatherService_Get5DayForecast_EmptyStub_ShouldReturnEmptyList()
        {
            // Arrange
            var emptyStub = new MeteoApiClientStub(
                customForecasts: new Dictionary<string, List<ForecastDay>>(),
                customTodayWeather: new List<WeatherSnapshot>()
            );
            var weatherService = new WeatherService(emptyStub);

            // Act
            var forecast = weatherService.Get5DayForecast("Iasi").ToList();

            // Assert
            Assert.IsNotNull(forecast, "forecast list should not be null even if stub is empty.");
            Assert.IsEmpty(forecast, "forecast list should be empty when stub provides no data.");
        }

        /// <summary>
        /// D7_WeatherService: Get5DayForecast with different casing
        /// Ensures that Get5DayForecast is case-insensitive regarding the city name.
        /// We use the default stub data and request "IASI" instead of "Iasi".
        /// The service should still return the forecast instead of treating it as unknown.
        /// </summary>
        [Test]
        [Category("Domain")]
        [Description("D7_WeatherService - Verify Get5DayForecast is case-insensitive for city input.")]
        public void D7_WeatherService_Get5DayForecast_CaseInsensitiveCity_ShouldReturnForecast()
        {
            // Arrange
            var weatherService = new WeatherService(_stubClient);
            string city = "IASI";

            // Act
            var forecast = weatherService.Get5DayForecast(city).ToList();

            // Assert
            Assert.IsNotNull(forecast, "Forecast list should not be null.");
            Assert.IsTrue(forecast.Any(), "Forecast should not be empty when using uppercase city name.");

            // ensure the boundaries still match the stub defaults
            double minTemp = forecast.Min(f => f.TemperatureMin);
            double maxTemp = forecast.Max(f => f.TemperatureMax);

            Assert.That(minTemp, Is.EqualTo(-2).Within(0.001), "Minimum temperature should match stub boundary.");
            Assert.That(maxTemp, Is.EqualTo(12).Within(0.001), "Maximum temperature should match stub boundary.");
        }

        //----------------------------------------------------------------
        // WeatherAnalysisService Domain Tests
        //----------------------------------------------------------------

        /// <summary>
        /// D8_WeatherAnalysisService: GetAverageTemperature - Single-letter city boundary
        /// Ensures that GetAverageTemperature correctly calculates the average temperature
        /// for a city with a single-character name ("A") using the custom boundary stub.
        /// </summary>
        [Test]
        [Category("Domain")]
        [Description("D8_WeatherAnalysisService - Verify GetAverageTemperature for single-letter city.")]
        public void D8_WeatherAnalysisService_GetAverageTemperature_SingleLetterCity()
        {
            // Arrange
            var weatherService = new WeatherService(_stubClientCustom);
            var analysisService = new WeatherAnalysisService(weatherService);
            string city = "A"; // single-letter city

            // Act
            var avgTemp = analysisService.GetAverageTemperature(city);

            // Assert
            Assert.IsNotNull(avgTemp, "Average temperature should not be null for a valid city.");

            // Calculate expected average manually from stub for verification
            var forecast = _stubClientCustom.Get5DayForecast(city);
            double expected = forecast.Average(f => (f.TemperatureMin + f.TemperatureMax) / 2.0);

            Assert.That(avgTemp, Is.EqualTo(expected).Within(0.001), "Average temperature should match expected value from stub.");
        }

        /// <summary>
        /// D9_WeatherAnalysisService: GetAverageTemperature - Long city name boundary
        /// Ensures that GetAverageTemperature correctly calculates the average temperature
        /// for a city with a long name ("Abcdefghijklmnopqrstuv") using the custom boundary stub.
        /// </summary>
        [Test]
        [Category("Domain")]
        [Description("D9_WeatherAnalysisService - Verify GetAverageTemperature for long city name.")]
        public void D9_WeatherAnalysisService_GetAverageTemperature_LongCityName()
        {
            // Arrange
            var weatherService = new WeatherService(_stubClientCustom);
            var analysisService = new WeatherAnalysisService(weatherService);
            string city = "Abcdefghijklmnopqrstuv"; // long city name

            // Act
            var avgTemp = analysisService.GetAverageTemperature(city);

            // Assert
            Assert.IsNotNull(avgTemp, "Average temperature should not be null for a valid city.");

            // Calculate expected average manually from stub for verification
            var forecast = _stubClientCustom.Get5DayForecast(city);
            double expected = forecast.Average(f => (f.TemperatureMin + f.TemperatureMax) / 2.0);

            Assert.That(avgTemp, Is.EqualTo(expected).Within(0.001), "Average temperature should match expected value from stub.");
        }

        /// <summary>
        /// D10_WeatherAnalysisService: GetAverageTemperature - Unknown city
        /// Ensures that GetAverageTemperature returns null when the city is not found
        /// in the forecast stub.
        /// </summary>
        [Test]
        [Category("Domain")]
        [Description("D10_WeatherAnalysisService - Verify GetAverageTemperature returns null for unknown city.")]
        public void D10_WeatherAnalysisService_GetAverageTemperature_UnknownCity()
        {
            // Arrange
            var weatherService = new WeatherService(_stubClientCustom);
            var analysisService = new WeatherAnalysisService(weatherService);
            string unknownCity = "Atlantis"; // city not in stub

            // Act
            var avgTemp = analysisService.GetAverageTemperature(unknownCity);

            // Assert
            Assert.IsNull(avgTemp, "Average temperature should be null for a city not present in the stub.");
        }

        /// <summary>
        /// D11_WeatherAnalysisService: GetAverageTemperature - Empty city string
        /// Ensures that calling GetAverageTemperature with an empty string
        /// throws an ArgumentException.
        /// </summary>
        [Test]
        [Category("Domain")]
        [Description("D11_WeatherAnalysisService - Verify GetAverageTemperature throws ArgumentException for empty city.")]
        public void D11_WeatherAnalysisService_GetAverageTemperature_EmptyCity_ShouldThrow()
        {
            // Arrange
            var weatherService = new WeatherService(_stubClientCustom);
            var analysisService = new WeatherAnalysisService(weatherService);
            string emptyCity = "";

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => analysisService.GetAverageTemperature(emptyCity));
            Assert.That(ex.Message, Does.Contain("City cannot be null or empty"));
            Assert.That(ex.ParamName, Is.EqualTo("city"));
        }

        /// <summary>
        /// D12_WeatherAnalysisService: GetAverageTemperature - Whitespace-only city string
        /// Ensures that calling GetAverageTemperature with a string containing only whitespace
        /// throws an ArgumentException.
        /// </summary>
        [Test]
        [Category("Domain")]
        [Description("D12_WeatherAnalysisService - Verify GetAverageTemperature throws ArgumentException for whitespace-only city.")]
        public void D12_WeatherAnalysisService_GetAverageTemperature_WhitespaceCity_ShouldThrow()
        {
            // Arrange
            var weatherService = new WeatherService(_stubClientCustom);
            var analysisService = new WeatherAnalysisService(weatherService);
            string whitespaceCity = "     ";

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => analysisService.GetAverageTemperature(whitespaceCity));
            Assert.That(ex.Message, Does.Contain("City cannot be null or empty"));
            Assert.That(ex.ParamName, Is.EqualTo("city"));
        }

        /// <summary>
        /// D13_WeatherAnalysisService: GetAverageMinTemperature - Single-letter city boundary
        /// Ensures that GetAverageMinTemperature correctly calculates the average minimum temperature
        /// for a city with a single-character name ("A") using the custom boundary stub.
        /// </summary>
        [Test]
        [Category("Domain")]
        [Description("D13_WeatherAnalysisService - Verify GetAverageMinTemperature for single-letter city.")]
        public void D13_WeatherAnalysisService_GetAverageMinTemperature_SingleLetterCity()
        {
            // Arrange
            var weatherService = new WeatherService(_stubClientCustom);
            var analysisService = new WeatherAnalysisService(weatherService);
            string city = "A";

            // Act
            var avgMinTemp = analysisService.GetAverageMinTemperature(city);

            // Assert
            Assert.IsNotNull(avgMinTemp, "Average minimum temperature should not be null for a valid city.");
            var forecast = _stubClientCustom.Get5DayForecast(city);
            double expected = forecast.Average(f => f.TemperatureMin);
            Assert.That(avgMinTemp.Value, Is.EqualTo(expected).Within(0.001),
                "Average minimum temperature should match expected value from stub.");
        }

        /// <summary>
        /// D14_WeatherAnalysisService: GetAverageMinTemperature - Long city name boundary
        /// Ensures that GetAverageMinTemperature correctly calculates the average minimum temperature
        /// for a city with a long name ("Abcdefghijklmnopqrstuv") using the custom boundary stub.
        /// </summary>
        [Test]
        [Category("Domain")]
        [Description("D14_WeatherAnalysisService - Verify GetAverageMinTemperature for long city name.")]
        public void D14_WeatherAnalysisService_GetAverageMinTemperature_LongCityName()
        {
            // Arrange
            var weatherService = new WeatherService(_stubClientCustom);
            var analysisService = new WeatherAnalysisService(weatherService);
            string city = "Abcdefghijklmnopqrstuv";

            // Act
            var avgMinTemp = analysisService.GetAverageMinTemperature(city);

            // Assert
            Assert.IsNotNull(avgMinTemp, "Average minimum temperature should not be null for a valid city.");
            var forecast = _stubClientCustom.Get5DayForecast(city);
            double expected = forecast.Average(f => f.TemperatureMin);
            Assert.That(avgMinTemp.Value, Is.EqualTo(expected).Within(0.001),
                "Average minimum temperature should match expected value from stub.");
        }

        /// <summary>
        /// D15_WeatherAnalysisService: GetAverageMinTemperature - Unknown city
        /// Ensures that GetAverageMinTemperature returns null when the city is not found
        /// in the forecast stub.
        /// </summary>
        [Test]
        [Category("Domain")]
        [Description("D15_WeatherAnalysisService - Verify GetAverageMinTemperature returns null for unknown city.")]
        public void D15_WeatherAnalysisService_GetAverageMinTemperature_UnknownCity()
        {
            // Arrange
            var weatherService = new WeatherService(_stubClientCustom);
            var analysisService = new WeatherAnalysisService(weatherService);
            string unknownCity = "Atlantis";

            // Act
            var avgMinTemp = analysisService.GetAverageMinTemperature(unknownCity);

            // Assert
            Assert.IsNull(avgMinTemp, "Average minimum temperature should be null for a city not present in the stub.");
        }

        /// <summary>
        /// D16_WeatherAnalysisService: GetAverageMinTemperature - Empty city string
        /// Ensures that calling GetAverageMinTemperature with an empty string
        /// throws an ArgumentException.
        /// </summary>
        [Test]
        [Category("Domain")]
        [Description("D16_WeatherAnalysisService - Verify GetAverageMinTemperature throws ArgumentException for empty city.")]
        public void D16_WeatherAnalysisService_GetAverageMinTemperature_EmptyCity_ShouldThrow()
        {
            // Arrange
            var weatherService = new WeatherService(_stubClientCustom);
            var analysisService = new WeatherAnalysisService(weatherService);
            string emptyCity = "";

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => analysisService.GetAverageMinTemperature(emptyCity));
            Assert.That(ex.Message, Does.Contain("City cannot be null or empty"));
            Assert.That(ex.ParamName, Is.EqualTo("city"));
        }

        /// <summary>
        /// D17_WeatherAnalysisService: GetAverageMinTemperature - Whitespace-only city string
        /// Ensures that calling GetAverageMinTemperature with a string containing only whitespace
        /// throws an ArgumentException.
        /// </summary>
        [Test]
        [Category("Domain")]
        [Description("D17_WeatherAnalysisService - Verify GetAverageMinTemperature throws ArgumentException for whitespace-only city.")]
        public void D17_WeatherAnalysisService_GetAverageMinTemperature_WhitespaceCity_ShouldThrow()
        {
            // Arrange
            var weatherService = new WeatherService(_stubClientCustom);
            var analysisService = new WeatherAnalysisService(weatherService);
            string whitespaceCity = "     ";

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => analysisService.GetAverageMinTemperature(whitespaceCity));
            Assert.That(ex.Message, Does.Contain("City cannot be null or empty"));
            Assert.That(ex.ParamName, Is.EqualTo("city"));
        }

        /// <summary>
        /// D18_WeatherAnalysisService: GetAverageMaxTemperature - Single-letter city boundary
        /// Ensures that GetAverageMaxTemperature correctly calculates the average maximum temperature
        /// for a city with a single-character name ("A") using the custom boundary stub.
        /// </summary>
        [Test]
        [Category("Domain")]
        [Description("D18_WeatherAnalysisService - Verify GetAverageMaxTemperature for single-letter city.")]
        public void D18_WeatherAnalysisService_GetAverageMaxTemperature_SingleLetterCity()
        {
            // Arrange
            var weatherService = new WeatherService(_stubClientCustom);
            var analysisService = new WeatherAnalysisService(weatherService);
            string city = "A";

            // Act
            var avgMaxTemp = analysisService.GetAverageMaxTemperature(city);

            // Assert
            Assert.IsNotNull(avgMaxTemp, "Average maximum temperature should not be null for a valid city.");
            var forecast = _stubClientCustom.Get5DayForecast(city);
            double expected = forecast.Average(f => f.TemperatureMax);
            Assert.That(avgMaxTemp.Value, Is.EqualTo(expected).Within(0.001),
                "Average maximum temperature should match expected value from stub.");
        }

        /// <summary>
        /// D19_WeatherAnalysisService: GetAverageMaxTemperature - Long city name boundary
        /// Ensures that GetAverageMaxTemperature correctly calculates the average maximum temperature
        /// for a city with a long name ("Abcdefghijklmnopqrstuv") using the custom boundary stub.
        /// </summary>
        [Test]
        [Category("Domain")]
        [Description("D19_WeatherAnalysisService - Verify GetAverageMaxTemperature for long city name.")]
        public void D19_WeatherAnalysisService_GetAverageMaxTemperature_LongCityName()
        {
            // Arrange
            var weatherService = new WeatherService(_stubClientCustom);
            var analysisService = new WeatherAnalysisService(weatherService);
            string city = "Abcdefghijklmnopqrstuv";

            // Act
            var avgMaxTemp = analysisService.GetAverageMaxTemperature(city);

            // Assert
            Assert.IsNotNull(avgMaxTemp, "Average maximum temperature should not be null for a valid city.");
            var forecast = _stubClientCustom.Get5DayForecast(city);
            double expected = forecast.Average(f => f.TemperatureMax);
            Assert.That(avgMaxTemp.Value, Is.EqualTo(expected).Within(0.001),
                "Average maximum temperature should match expected value from stub.");
        }

        /// <summary>
        /// D20_WeatherAnalysisService: GetAverageMaxTemperature - Unknown city
        /// Ensures that GetAverageMaxTemperature returns null when the city is not found
        /// in the forecast stub.
        /// </summary>
        [Test]
        [Category("Domain")]
        [Description("D20_WeatherAnalysisService - Verify GetAverageMaxTemperature returns null for unknown city.")]
        public void D20_WeatherAnalysisService_GetAverageMaxTemperature_UnknownCity()
        {
            // Arrange
            var weatherService = new WeatherService(_stubClientCustom);
            var analysisService = new WeatherAnalysisService(weatherService);
            string unknownCity = "Atlantis";

            // Act
            var avgMaxTemp = analysisService.GetAverageMaxTemperature(unknownCity);

            // Assert
            Assert.IsNull(avgMaxTemp, "Average maximum temperature should be null for a city not present in the stub.");
        }

        /// <summary>
        /// D21_WeatherAnalysisService: GetAverageMaxTemperature - Empty city string
        /// Ensures that calling GetAverageMaxTemperature with an empty string
        /// throws an ArgumentException.
        /// </summary>
        [Test]
        [Category("Domain")]
        [Description("D21_WeatherAnalysisService - Verify GetAverageMaxTemperature throws ArgumentException for empty city.")]
        public void D21_WeatherAnalysisService_GetAverageMaxTemperature_EmptyCity_ShouldThrow()
        {
            // Arrange
            var weatherService = new WeatherService(_stubClientCustom);
            var analysisService = new WeatherAnalysisService(weatherService);
            string emptyCity = "";

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => analysisService.GetAverageMaxTemperature(emptyCity));
            Assert.That(ex.Message, Does.Contain("City cannot be null or empty"));
            Assert.That(ex.ParamName, Is.EqualTo("city"));
        }

        /// <summary>
        /// D22_WeatherAnalysisService: GetAverageMaxTemperature - Whitespace-only city string
        /// Ensures that calling GetAverageMaxTemperature with a string containing only whitespace
        /// throws an ArgumentException.
        /// </summary>
        [Test]
        [Category("Domain")]
        [Description("D22_WeatherAnalysisService - Verify GetAverageMaxTemperature throws ArgumentException for whitespace-only city.")]
        public void D22_WeatherAnalysisService_GetAverageMaxTemperature_WhitespaceCity_ShouldThrow()
        {
            // Arrange
            var weatherService = new WeatherService(_stubClientCustom);
            var analysisService = new WeatherAnalysisService(weatherService);
            string whitespaceCity = "     ";

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => analysisService.GetAverageMaxTemperature(whitespaceCity));
            Assert.That(ex.Message, Does.Contain("City cannot be null or empty"));
            Assert.That(ex.ParamName, Is.EqualTo("city"));
        }

        /// <summary>
        /// D23_WeatherAnalysisService: GetHottestDay - Single-letter city boundary
        /// Ensures that GetHottestDay correctly returns the day with the highest maximum temperature
        /// for a city with a single-character name ("A") using the custom boundary stub.
        /// </summary>
        [Test]
        [Category("Domain")]
        [Description("D23_WeatherAnalysisService - Verify GetHottestDay for single-letter city.")]
        public void D23_WeatherAnalysisService_GetHottestDay_SingleLetterCity()
        {
            // Arrange
            var weatherService = new WeatherService(_stubClientCustom);
            var analysisService = new WeatherAnalysisService(weatherService);
            string city = "A";

            // Act
            var hottestDay = analysisService.GetHottestDay(city);

            // Assert
            Assert.IsNotNull(hottestDay, "Hottest day should not be null for a valid city.");
            var forecast = _stubClientCustom.Get5DayForecast(city);
            var expected = forecast.OrderByDescending(f => f.TemperatureMax).First();
            Assert.That(hottestDay.TemperatureMax, Is.EqualTo(expected.TemperatureMax).Within(0.001),
                "Hottest day's max temperature should match expected value from stub.");
        }

        /// <summary>
        /// D24_WeatherAnalysisService: GetHottestDay - Long city name boundary
        /// Ensures that GetHottestDay correctly returns the day with the highest maximum temperature
        /// for a city with a long name ("Abcdefghijklmnopqrstuv") using the custom boundary stub.
        /// </summary>
        [Test]
        [Category("Domain")]
        [Description("D24_WeatherAnalysisService - Verify GetHottestDay for long city name.")]
        public void D24_WeatherAnalysisService_GetHottestDay_LongCityName()
        {
            // Arrange
            var weatherService = new WeatherService(_stubClientCustom);
            var analysisService = new WeatherAnalysisService(weatherService);
            string city = "Abcdefghijklmnopqrstuv";

            // Act
            var hottestDay = analysisService.GetHottestDay(city);

            // Assert
            Assert.IsNotNull(hottestDay, "Hottest day should not be null for a valid city.");
            var forecast = _stubClientCustom.Get5DayForecast(city);
            var expected = forecast.OrderByDescending(f => f.TemperatureMax).First();
            Assert.That(hottestDay.TemperatureMax, Is.EqualTo(expected.TemperatureMax).Within(0.001),
                "Hottest day's max temperature should match expected value from stub.");
        }

        /// <summary>
        /// D25_WeatherAnalysisService: GetHottestDay - Unknown city
        /// Ensures that GetHottestDay returns null when the city is not found in the forecast stub.
        /// </summary>
        [Test]
        [Category("Domain")]
        [Description("D25_WeatherAnalysisService - Verify GetHottestDay returns null for unknown city.")]
        public void D25_WeatherAnalysisService_GetHottestDay_UnknownCity()
        {
            // Arrange
            var weatherService = new WeatherService(_stubClientCustom);
            var analysisService = new WeatherAnalysisService(weatherService);
            string unknownCity = "Atlantis";

            // Act
            var hottestDay = analysisService.GetHottestDay(unknownCity);

            // Assert
            Assert.IsNull(hottestDay, "Hottest day should be null for a city not present in the stub.");
        }

        /// <summary>
        /// D26_WeatherAnalysisService: GetHottestDay - Empty city string
        /// Ensures that calling GetHottestDay with an empty string throws an ArgumentException.
        /// </summary>
        [Test]
        [Category("Domain")]
        [Description("D26_WeatherAnalysisService - Verify GetHottestDay throws ArgumentException for empty city.")]
        public void D26_WeatherAnalysisService_GetHottestDay_EmptyCity_ShouldThrow()
        {
            // Arrange
            var weatherService = new WeatherService(_stubClientCustom);
            var analysisService = new WeatherAnalysisService(weatherService);
            string emptyCity = "";

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => analysisService.GetHottestDay(emptyCity));
            Assert.That(ex.Message, Does.Contain("City cannot be null or empty"));
            Assert.That(ex.ParamName, Is.EqualTo("city"));
        }

        /// <summary>
        /// D27_WeatherAnalysisService: GetHottestDay - Whitespace-only city string
        /// Ensures that calling GetHottestDay with a string containing only whitespace throws an ArgumentException.
        /// </summary>
        [Test]
        [Category("Domain")]
        [Description("D27_WeatherAnalysisService - Verify GetHottestDay throws ArgumentException for whitespace-only city.")]
        public void D27_WeatherAnalysisService_GetHottestDay_WhitespaceCity_ShouldThrow()
        {
            // Arrange
            var weatherService = new WeatherService(_stubClientCustom);
            var analysisService = new WeatherAnalysisService(weatherService);
            string whitespaceCity = "     ";

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => analysisService.GetHottestDay(whitespaceCity));
            Assert.That(ex.Message, Does.Contain("City cannot be null or empty"));
            Assert.That(ex.ParamName, Is.EqualTo("city"));
        }

        /// <summary>
        /// D28_WeatherAnalysisService: GetColdestDay - Single-letter city boundary
        /// Ensures that GetColdestDay correctly returns the day with the lowest minimum temperature
        /// for a city with a single-character name ("A") using the custom boundary stub.
        /// </summary>
        [Test]
        [Category("Domain")]
        [Description("D28_WeatherAnalysisService - Verify GetColdestDay for single-letter city.")]
        public void D28_WeatherAnalysisService_GetColdestDay_SingleLetterCity()
        {
            // Arrange
            var weatherService = new WeatherService(_stubClientCustom);
            var analysisService = new WeatherAnalysisService(weatherService);
            string city = "A";

            // Act
            var coldestDay = analysisService.GetColdestDay(city);

            // Assert
            Assert.IsNotNull(coldestDay, "Coldest day should not be null for a valid city.");
            var forecast = _stubClientCustom.Get5DayForecast(city);
            var expected = forecast.OrderBy(f => f.TemperatureMin).First();
            Assert.That(coldestDay.TemperatureMin, Is.EqualTo(expected.TemperatureMin).Within(0.001),
                "Coldest day's min temperature should match expected value from stub.");
        }

        /// <summary>
        /// D29_WeatherAnalysisService: GetColdestDay - Long city name boundary
        /// Ensures that GetColdestDay correctly returns the day with the lowest minimum temperature
        /// for a city with a long name ("Abcdefghijklmnopqrstuv") using the custom boundary stub.
        /// </summary>
        [Test]
        [Category("Domain")]
        [Description("D29_WeatherAnalysisService - Verify GetColdestDay for long city name.")]
        public void D29_WeatherAnalysisService_GetColdestDay_LongCityName()
        {
            // Arrange
            var weatherService = new WeatherService(_stubClientCustom);
            var analysisService = new WeatherAnalysisService(weatherService);
            string city = "Abcdefghijklmnopqrstuv";

            // Act
            var coldestDay = analysisService.GetColdestDay(city);

            // Assert
            Assert.IsNotNull(coldestDay, "Coldest day should not be null for a valid city.");
            var forecast = _stubClientCustom.Get5DayForecast(city);
            var expected = forecast.OrderBy(f => f.TemperatureMin).First();
            Assert.That(coldestDay.TemperatureMin, Is.EqualTo(expected.TemperatureMin).Within(0.001),
                "Coldest day's min temperature should match expected value from stub.");
        }

        /// <summary>
        /// D30_WeatherAnalysisService: GetColdestDay - Unknown city
        /// Ensures that GetColdestDay returns null when the city is not found in the forecast stub.
        /// </summary>
        [Test]
        [Category("Domain")]
        [Description("D30_WeatherAnalysisService - Verify GetColdestDay returns null for unknown city.")]
        public void D30_WeatherAnalysisService_GetColdestDay_UnknownCity()
        {
            // Arrange
            var weatherService = new WeatherService(_stubClientCustom);
            var analysisService = new WeatherAnalysisService(weatherService);
            string unknownCity = "Atlantis";

            // Act
            var coldestDay = analysisService.GetColdestDay(unknownCity);

            // Assert
            Assert.IsNull(coldestDay, "Coldest day should be null for a city not present in the stub.");
        }

        /// <summary>
        /// D31_WeatherAnalysisService: GetColdestDay - Empty city string
        /// Ensures that calling GetColdestDay with an empty string throws an ArgumentException.
        /// </summary>
        [Test]
        [Category("Domain")]
        [Description("D31_WeatherAnalysisService - Verify GetColdestDay throws ArgumentException for empty city.")]
        public void D31_WeatherAnalysisService_GetColdestDay_EmptyCity_ShouldThrow()
        {
            // Arrange
            var weatherService = new WeatherService(_stubClientCustom);
            var analysisService = new WeatherAnalysisService(weatherService);
            string emptyCity = "";

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => analysisService.GetColdestDay(emptyCity));
            Assert.That(ex.Message, Does.Contain("City cannot be null or empty"));
            Assert.That(ex.ParamName, Is.EqualTo("city"));
        }

        /// <summary>
        /// D32_WeatherAnalysisService: GetColdestDay - Whitespace-only city string
        /// Ensures that calling GetColdestDay with a string containing only whitespace throws an ArgumentException.
        /// </summary>
        [Test]
        [Category("Domain")]
        [Description("D32_WeatherAnalysisService - Verify GetColdestDay throws ArgumentException for whitespace-only city.")]
        public void D32_WeatherAnalysisService_GetColdestDay_WhitespaceCity_ShouldThrow()
        {
            // Arrange
            var weatherService = new WeatherService(_stubClientCustom);
            var analysisService = new WeatherAnalysisService(weatherService);
            string whitespaceCity = "     ";

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => analysisService.GetColdestDay(whitespaceCity));
            Assert.That(ex.Message, Does.Contain("City cannot be null or empty"));
            Assert.That(ex.ParamName, Is.EqualTo("city"));
        }

        /// <summary>
        /// D33_WeatherAnalysisService: GetSunnyDays - Single-letter city boundary
        /// Ensures that GetSunnyDays correctly returns all sunny forecast days
        /// for a city with a single-character name ("A") using the custom boundary stub.
        /// </summary>
        [Test]
        [Category("Domain")]
        [Description("D33_WeatherAnalysisService - Verify GetSunnyDays for single-letter city.")]
        public void D33_WeatherAnalysisService_GetSunnyDays_SingleLetterCity()
        {
            // Arrange
            var weatherService = new WeatherService(_stubClientCustom);
            var analysisService = new WeatherAnalysisService(weatherService);
            string city = "A";

            // Act
            var sunnyDays = analysisService.GetSunnyDays(city).ToList();

            // Assert
            Assert.IsNotNull(sunnyDays, "SunnyDays list should not be null for a valid city.");
            var forecast = _stubClientCustom.Get5DayForecast(city);
            var expected = forecast.Where(f =>
                new[] { "CER SENIN", "CER VARIABIL", "CER PARTIAL NOROS", "CER TEMPORAR NOROS" }
                .Any(k => f.WeatherDescription.Contains(k, StringComparison.OrdinalIgnoreCase)))
                .ToList();
            Assert.That(sunnyDays.Count, Is.EqualTo(expected.Count), "SunnyDays count should match expected value from stub.");
        }

        /// <summary>
        /// D34_WeatherAnalysisService: GetSunnyDays - Long city name boundary
        /// Ensures that GetSunnyDays correctly returns all sunny forecast days
        /// for a city with a long name ("Abcdefghijklmnopqrstuv") using the custom boundary stub.
        /// </summary>
        [Test]
        [Category("Domain")]
        [Description("D34_WeatherAnalysisService - Verify GetSunnyDays for long city name.")]
        public void D34_WeatherAnalysisService_GetSunnyDays_LongCityName()
        {
            // Arrange
            var weatherService = new WeatherService(_stubClientCustom);
            var analysisService = new WeatherAnalysisService(weatherService);
            string city = "Abcdefghijklmnopqrstuv";

            // Act
            var sunnyDays = analysisService.GetSunnyDays(city).ToList();

            // Assert
            Assert.IsNotNull(sunnyDays, "SunnyDays list should not be null for a valid city.");
            var forecast = _stubClientCustom.Get5DayForecast(city);
            var expected = forecast.Where(f =>
                new[] { "CER SENIN", "CER VARIABIL", "CER PARTIAL NOROS", "CER TEMPORAR NOROS" }
                .Any(k => f.WeatherDescription.Contains(k, StringComparison.OrdinalIgnoreCase)))
                .ToList();
            Assert.That(sunnyDays.Count, Is.EqualTo(expected.Count), "SunnyDays count should match expected value from stub.");
        }

        /// <summary>
        /// D35_WeatherAnalysisService: GetSunnyDays - Unknown city
        /// Ensures that GetSunnyDays returns an empty list when the city is not found in the forecast stub.
        /// </summary>
        [Test]
        [Category("Domain")]
        [Description("D35_WeatherAnalysisService - Verify GetSunnyDays returns empty list for unknown city.")]
        public void D35_WeatherAnalysisService_GetSunnyDays_UnknownCity()
        {
            // Arrange
            var weatherService = new WeatherService(_stubClientCustom);
            var analysisService = new WeatherAnalysisService(weatherService);
            string unknownCity = "Atlantis";

            // Act
            var sunnyDays = analysisService.GetSunnyDays(unknownCity).ToList();

            // Assert
            Assert.IsNotNull(sunnyDays, "SunnyDays list should not be null for unknown city.");
            Assert.IsEmpty(sunnyDays, "SunnyDays list should be empty for a city not present in the stub.");
        }

        /// <summary>
        /// D36_WeatherAnalysisService: GetSunnyDays - Empty city string
        /// Ensures that calling GetSunnyDays with an empty string throws an ArgumentException.
        /// </summary>
        [Test]
        [Category("Domain")]
        [Description("D36_WeatherAnalysisService - Verify GetSunnyDays throws ArgumentException for empty city.")]
        public void D36_WeatherAnalysisService_GetSunnyDays_EmptyCity_ShouldThrow()
        {
            // Arrange
            var weatherService = new WeatherService(_stubClientCustom);
            var analysisService = new WeatherAnalysisService(weatherService);
            string emptyCity = "";

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => analysisService.GetSunnyDays(emptyCity));
            Assert.That(ex.Message, Does.Contain("City cannot be null or empty"));
            Assert.That(ex.ParamName, Is.EqualTo("city"));
        }

        /// <summary>
        /// D37_WeatherAnalysisService: GetSunnyDays - Whitespace-only city string
        /// Ensures that calling GetSunnyDays with a string containing only whitespace throws an ArgumentException.
        /// </summary>
        [Test]
        [Category("Domain")]
        [Description("D37_WeatherAnalysisService - Verify GetSunnyDays throws ArgumentException for whitespace-only city.")]
        public void D37_WeatherAnalysisService_GetSunnyDays_WhitespaceCity_ShouldThrow()
        {
            // Arrange
            var weatherService = new WeatherService(_stubClientCustom);
            var analysisService = new WeatherAnalysisService(weatherService);
            string whitespaceCity = "     ";

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => analysisService.GetSunnyDays(whitespaceCity));
            Assert.That(ex.Message, Does.Contain("City cannot be null or empty"));
            Assert.That(ex.ParamName, Is.EqualTo("city"));
        }

        /// <summary>
        /// D38_WeatherAnalysisService: GetCloudyDays - Single-letter city boundary
        /// Ensures that GetCloudyDays correctly returns all cloudy forecast days
        /// for a city with a single-character name ("A") using the custom boundary stub.
        /// </summary>
        [Test]
        [Category("Domain")]
        [Description("D38_WeatherAnalysisService - Verify GetCloudyDays for single-letter city.")]
        public void D38_WeatherAnalysisService_GetCloudyDays_SingleLetterCity()
        {
            // Arrange
            var weatherService = new WeatherService(_stubClientCustom);
            var analysisService = new WeatherAnalysisService(weatherService);
            string city = "A";

            // Act
            var cloudyDays = analysisService.GetCloudyDays(city).ToList();

            // Assert
            Assert.IsNotNull(cloudyDays, "CloudyDays list should not be null for a valid city.");
            var forecast = _stubClientCustom.Get5DayForecast(city);
            var expected = forecast.Where(f =>
                new[] { "CER MAI NOROS", "CER MAI MULT NOROS" }
                .Any(k => f.WeatherDescription.Contains(k, StringComparison.OrdinalIgnoreCase)))
                .ToList();
            Assert.That(cloudyDays.Count, Is.EqualTo(expected.Count), "CloudyDays count should match expected value from stub.");
        }

        /// <summary>
        /// D39_WeatherAnalysisService: GetCloudyDays - Long city name boundary
        /// Ensures that GetCloudyDays correctly returns all cloudy forecast days
        /// for a city with a long name ("Abcdefghijklmnopqrstuv") using the custom boundary stub.
        /// </summary>
        [Test]
        [Category("Domain")]
        [Description("D39_WeatherAnalysisService - Verify GetCloudyDays for long city name.")]
        public void D39_WeatherAnalysisService_GetCloudyDays_LongCityName()
        {
            // Arrange
            var weatherService = new WeatherService(_stubClientCustom);
            var analysisService = new WeatherAnalysisService(weatherService);
            string city = "Abcdefghijklmnopqrstuv";

            // Act
            var cloudyDays = analysisService.GetCloudyDays(city).ToList();

            // Assert
            Assert.IsNotNull(cloudyDays, "CloudyDays list should not be null for a valid city.");
            var forecast = _stubClientCustom.Get5DayForecast(city);
            var expected = forecast.Where(f =>
                new[] { "CER MAI NOROS", "CER MAI MULT NOROS" }
                .Any(k => f.WeatherDescription.Contains(k, StringComparison.OrdinalIgnoreCase)))
                .ToList();
            Assert.That(cloudyDays.Count, Is.EqualTo(expected.Count), "CloudyDays count should match expected value from stub.");
        }

        /// <summary>
        /// D40_WeatherAnalysisService: GetCloudyDays - Unknown city
        /// Ensures that GetCloudyDays returns an empty list when the city is not found in the forecast stub.
        /// </summary>
        [Test]
        [Category("Domain")]
        [Description("D40_WeatherAnalysisService - Verify GetCloudyDays returns empty list for unknown city.")]
        public void D40_WeatherAnalysisService_GetCloudyDays_UnknownCity()
        {
            // Arrange
            var weatherService = new WeatherService(_stubClientCustom);
            var analysisService = new WeatherAnalysisService(weatherService);
            string unknownCity = "Atlantis";

            // Act
            var cloudyDays = analysisService.GetCloudyDays(unknownCity).ToList();

            // Assert
            Assert.IsNotNull(cloudyDays, "CloudyDays list should not be null for unknown city.");
            Assert.IsEmpty(cloudyDays, "CloudyDays list should be empty for a city not present in the stub.");
        }

        /// <summary>
        /// D41_WeatherAnalysisService: GetCloudyDays - Empty city string
        /// Ensures that calling GetCloudyDays with an empty string throws an ArgumentException.
        /// </summary>
        [Test]
        [Category("Domain")]
        [Description("D41_WeatherAnalysisService - Verify GetCloudyDays throws ArgumentException for empty city.")]
        public void D41_WeatherAnalysisService_GetCloudyDays_EmptyCity_ShouldThrow()
        {
            // Arrange
            var weatherService = new WeatherService(_stubClientCustom);
            var analysisService = new WeatherAnalysisService(weatherService);
            string emptyCity = "";

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => analysisService.GetCloudyDays(emptyCity));
            Assert.That(ex.Message, Does.Contain("City cannot be null or empty"));
            Assert.That(ex.ParamName, Is.EqualTo("city"));
        }

        /// <summary>
        /// D42_WeatherAnalysisService: GetCloudyDays - Whitespace-only city string
        /// Ensures that calling GetCloudyDays with a string containing only whitespace throws an ArgumentException.
        /// </summary>
        [Test]
        [Category("Domain")]
        [Description("D42_WeatherAnalysisService - Verify GetCloudyDays throws ArgumentException for whitespace-only city.")]
        public void D42_WeatherAnalysisService_GetCloudyDays_WhitespaceCity_ShouldThrow()
        {
            // Arrange
            var weatherService = new WeatherService(_stubClientCustom);
            var analysisService = new WeatherAnalysisService(weatherService);
            string whitespaceCity = "     ";

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => analysisService.GetCloudyDays(whitespaceCity));
            Assert.That(ex.Message, Does.Contain("City cannot be null or empty"));
            Assert.That(ex.ParamName, Is.EqualTo("city"));
        }

        /// <summary>
        /// D43_WeatherAnalysisService: GetTemperatureTrend - Threshold = 0
        /// Ensures that GetTemperatureTrend returns a valid trend string when threshold is at its lower limit (0).
        /// Uses the custom boundary stub for deterministic results.
        /// </summary>
        [Test]
        [Category("Domain")]
        [Description("D43_WeatherAnalysisService - Verify GetTemperatureTrend for threshold = 0.")]
        public void D43_WeatherAnalysisService_GetTemperatureTrend_ThresholdZero()
        {
            // Arrange
            var weatherService = new WeatherService(_stubClientCustom);
            var analysisService = new WeatherAnalysisService(weatherService);
            string city = "A";
            double threshold = 0;

            // Act
            var trend = analysisService.GetTemperatureTrend(city, threshold);

            // Assert
            Assert.IsNotNull(trend, "Trend string should not be null for threshold = 0.");
            Assert.That(trend, Is.TypeOf<string>(), "Trend should be a string.");
            // We can verify it's one of the valid return values
            Assert.Contains(trend, new[] { "Rising", "Falling", "Stable/Mixed" });
        }

        /// <summary>
        /// D44_WeatherAnalysisService: GetTemperatureTrend - Threshold = 1
        /// Ensures that GetTemperatureTrend returns a valid trend string when threshold is just above the lower limit (1).
        /// </summary>
        [Test]
        [Category("Domain")]
        [Description("D44_WeatherAnalysisService - Verify GetTemperatureTrend for threshold = 1.")]
        public void D44_WeatherAnalysisService_GetTemperatureTrend_ThresholdOne()
        {
            // Arrange
            var weatherService = new WeatherService(_stubClientCustom);
            var analysisService = new WeatherAnalysisService(weatherService);
            string city = "A";
            double threshold = 1;

            // Act
            var trend = analysisService.GetTemperatureTrend(city, threshold);

            // Assert
            Assert.IsNotNull(trend, "Trend string should not be null for threshold = 1.");
            Assert.That(trend, Is.TypeOf<string>(), "Trend should be a string.");
            Assert.Contains(trend, new[] { "Rising", "Falling", "Stable/Mixed" });
        }

        /// <summary>
        /// D45_WeatherAnalysisService: GetTemperatureTrend - Threshold = 1000
        /// Ensures that GetTemperatureTrend returns a valid trend string when threshold is very large (1000).
        /// </summary>
        [Test]
        [Category("Domain")]
        [Description("D45_WeatherAnalysisService - Verify GetTemperatureTrend for a very large threshold (1000).")]
        public void D45_WeatherAnalysisService_GetTemperatureTrend_ThresholdLarge()
        {
            // Arrange
            var weatherService = new WeatherService(_stubClientCustom);
            var analysisService = new WeatherAnalysisService(weatherService);
            string city = "A";
            double threshold = 1000;

            // Act
            var trend = analysisService.GetTemperatureTrend(city, threshold);

            // Assert
            Assert.IsNotNull(trend, "Trend string should not be null for large threshold.");
            Assert.That(trend, Is.TypeOf<string>(), "Trend should be a string.");
            Assert.Contains(trend, new[] { "Rising", "Falling", "Stable/Mixed" });
        }

        /// <summary>
        /// D46_WeatherAnalysisService: GetTemperatureTrend - Threshold = -1
        /// Ensures that GetTemperatureTrend throws ArgumentOutOfRangeException when threshold is negative (-1).
        /// </summary>
        [Test]
        [Category("Domain")]
        [Description("D46_WeatherAnalysisService - Verify GetTemperatureTrend throws ArgumentOutOfRangeException for negative threshold (-1).")]
        public void D46_WeatherAnalysisService_GetTemperatureTrend_ThresholdNegative()
        {
            // Arrange
            var weatherService = new WeatherService(_stubClientCustom);
            var analysisService = new WeatherAnalysisService(weatherService);
            string city = "A";
            double threshold = -1;

            // Act & Assert
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => analysisService.GetTemperatureTrend(city, threshold));
            Assert.That(ex.ParamName, Is.EqualTo("threshold"));
        }

        /// <summary>
        /// D47_WeatherAnalysisService: GetTemperatureTrend - Threshold = -0.001
        /// Ensures that GetTemperatureTrend throws ArgumentOutOfRangeException when threshold is slightly below zero (-0.001).
        /// </summary>
        [Test]
        [Category("Domain")]
        [Description("D47_WeatherAnalysisService - Verify GetTemperatureTrend throws ArgumentOutOfRangeException for threshold slightly below zero (-0.001).")]
        public void D47_WeatherAnalysisService_GetTemperatureTrend_ThresholdSlightlyBelowZero()
        {
            // Arrange
            var weatherService = new WeatherService(_stubClientCustom);
            var analysisService = new WeatherAnalysisService(weatherService);
            string city = "A";
            double threshold = -0.001;

            // Act & Assert
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => analysisService.GetTemperatureTrend(city, threshold));
            Assert.That(ex.ParamName, Is.EqualTo("threshold"));
        }

        /// <summary>
        /// D48_WeatherAnalysisService: GetTopNDaysByMaxTemperature - n = 1
        /// Ensures that GetTopNDaysByMaxTemperature returns exactly 1 day when n is at the lower limit.
        /// </summary>
        [Test]
        [Category("Domain")]
        [Description("D48_WeatherAnalysisService - Verify GetTopNDaysByMaxTemperature returns 1 day when n = 1.")]
        public void D48_WeatherAnalysisService_GetTopNDaysByMaxTemperature_NEqualsOne()
        {
            // Arrange
            var weatherService = new WeatherService(_stubClientCustom);
            var analysisService = new WeatherAnalysisService(weatherService);
            string city = "A";
            int n = 1;

            // Act
            var result = analysisService.GetTopNDaysByMaxTemperature(city, n).ToList();

            // Assert
            Assert.IsNotNull(result, "Result should not be null.");
            Assert.That(result.Count, Is.EqualTo(1), "Should return exactly 1 day.");
        }

        /// <summary>
        /// D49_WeatherAnalysisService: GetTopNDaysByMaxTemperature - n = 5 (upper bound)
        /// Ensures that GetTopNDaysByMaxTemperature returns all available forecast days when n equals the total forecast count.
        /// </summary>
        [Test]
        [Category("Domain")]
        [Description("D49_WeatherAnalysisService - Verify GetTopNDaysByMaxTemperature returns 5 days when n = 5.")]
        public void D49_WeatherAnalysisService_GetTopNDaysByMaxTemperature_NEqualsFive()
        {
            // Arrange
            var weatherService = new WeatherService(_stubClientCustom);
            var analysisService = new WeatherAnalysisService(weatherService);
            string city = "A";
            int n = 5;

            // Act
            var result = analysisService.GetTopNDaysByMaxTemperature(city, n).ToList();

            // Assert
            Assert.IsNotNull(result, "Result should not be null.");
            Assert.That(result.Count, Is.EqualTo(5), "Should return exactly 5 days.");
        }

        /// <summary>
        /// D50_WeatherAnalysisService: GetTopNDaysByMaxTemperature - n inside boundaries (n = 2)
        /// Ensures that GetTopNDaysByMaxTemperature returns the expected number of days when n is inside valid range.
        /// </summary>
        [Test]
        [Category("Domain")]
        [Description("D50_WeatherAnalysisService - Verify GetTopNDaysByMaxTemperature returns 2 days when n = 2.")]
        public void D50_WeatherAnalysisService_GetTopNDaysByMaxTemperature_NInside()
        {
            // Arrange
            var weatherService = new WeatherService(_stubClientCustom);
            var analysisService = new WeatherAnalysisService(weatherService);
            string city = "A";
            int n = 2;

            // Act
            var result = analysisService.GetTopNDaysByMaxTemperature(city, n).ToList();

            // Assert
            Assert.IsNotNull(result, "Result should not be null.");
            Assert.That(result.Count, Is.EqualTo(2), "Should return exactly 2 days.");
        }

        /// <summary>
        /// D51_WeatherAnalysisService: GetTopNDaysByMaxTemperature - n below lower bound (n = 0)
        /// Ensures that GetTopNDaysByMaxTemperature throws ArgumentOutOfRangeException when n is below the valid limit.
        /// </summary>
        [Test]
        [Category("Domain")]
        [Description("D51_WeatherAnalysisService - Verify GetTopNDaysByMaxTemperature throws for n = 0.")]
        public void D51_WeatherAnalysisService_GetTopNDaysByMaxTemperature_NBelowLowerBound_ShouldThrow()
        {
            // Arrange
            var weatherService = new WeatherService(_stubClientCustom);
            var analysisService = new WeatherAnalysisService(weatherService);
            string city = "A";
            int n = 0;

            // Act & Assert
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => analysisService.GetTopNDaysByMaxTemperature(city, n));
            Assert.That(ex.ParamName, Is.EqualTo("n"));
        }

        /// <summary>
        /// D52_WeatherAnalysisService: GetTopNDaysByMaxTemperature - n above upper bound (n = 10)
        /// Ensures that GetTopNDaysByMaxTemperature caps n to available forecast count if n exceeds it.
        /// </summary>
        [Test]
        [Category("Domain")]
        [Description("D52_WeatherAnalysisService - Verify GetTopNDaysByMaxTemperature caps n to forecast count when n = 10.")]
        public void D52_WeatherAnalysisService_GetTopNDaysByMaxTemperature_NAboveUpperBound()
        {
            // Arrange
            var weatherService = new WeatherService(_stubClientCustom);
            var analysisService = new WeatherAnalysisService(weatherService);
            string city = "A";
            int n = 10;

            // Act
            var result = analysisService.GetTopNDaysByMaxTemperature(city, n).ToList();

            // Assert
            Assert.IsNotNull(result, "Result should not be null.");
            Assert.That(result.Count, Is.EqualTo(5), "Should return maximum available days (5) when n exceeds forecast count.");
        }

        /// <summary>
        /// D53_WeatherAnalysisService: GetTopNDaysByMinTemperature - n = 1
        /// Ensures that GetTopNDaysByMinTemperature returns exactly 1 day when n is at the lower limit.
        /// </summary>
        [Test]
        [Category("Domain")]
        [Description("D53_WeatherAnalysisService - Verify GetTopNDaysByMinTemperature returns 1 day when n = 1.")]
        public void D53_WeatherAnalysisService_GetTopNDaysByMinTemperature_NEqualsOne()
        {
            // Arrange
            var weatherService = new WeatherService(_stubClientCustom);
            var analysisService = new WeatherAnalysisService(weatherService);
            string city = "A";
            int n = 1;

            // Act
            var result = analysisService.GetTopNDaysByMinTemperature(city, n).ToList();

            // Assert
            Assert.IsNotNull(result, "Result should not be null.");
            Assert.That(result.Count, Is.EqualTo(1), "Should return exactly 1 day.");
        }

        /// <summary>
        /// D54_WeatherAnalysisService: GetTopNDaysByMinTemperature - n = 5 (upper bound)
        /// Ensures that GetTopNDaysByMinTemperature returns all available forecast days when n equals the total forecast count.
        /// </summary>
        [Test]
        [Category("Domain")]
        [Description("D54_WeatherAnalysisService - Verify GetTopNDaysByMinTemperature returns 5 days when n = 5.")]
        public void D54_WeatherAnalysisService_GetTopNDaysByMinTemperature_NEqualsFive()
        {
            // Arrange
            var weatherService = new WeatherService(_stubClientCustom);
            var analysisService = new WeatherAnalysisService(weatherService);
            string city = "A";
            int n = 5;

            // Act
            var result = analysisService.GetTopNDaysByMinTemperature(city, n).ToList();

            // Assert
            Assert.IsNotNull(result, "Result should not be null.");
            Assert.That(result.Count, Is.EqualTo(5), "Should return exactly 5 days.");
        }

        /// <summary>
        /// D55_WeatherAnalysisService: GetTopNDaysByMinTemperature - n inside boundaries (n = 2)
        /// Ensures that GetTopNDaysByMinTemperature returns the expected number of days when n is inside valid range.
        /// </summary>
        [Test]
        [Category("Domain")]
        [Description("D55_WeatherAnalysisService - Verify GetTopNDaysByMinTemperature returns 2 days when n = 2.")]
        public void D55_WeatherAnalysisService_GetTopNDaysByMinTemperature_NInside()
        {
            // Arrange
            var weatherService = new WeatherService(_stubClientCustom);
            var analysisService = new WeatherAnalysisService(weatherService);
            string city = "A";
            int n = 2;

            // Act
            var result = analysisService.GetTopNDaysByMinTemperature(city, n).ToList();

            // Assert
            Assert.IsNotNull(result, "Result should not be null.");
            Assert.That(result.Count, Is.EqualTo(2), "Should return exactly 2 days.");
        }

        /// <summary>
        /// D56_WeatherAnalysisService: GetTopNDaysByMinTemperature - n below lower bound (n = 0)
        /// Ensures that GetTopNDaysByMinTemperature throws ArgumentOutOfRangeException when n is below the valid limit.
        /// </summary>
        [Test]
        [Category("Domain")]
        [Description("D56_WeatherAnalysisService - Verify GetTopNDaysByMinTemperature throws for n = 0.")]
        public void D56_WeatherAnalysisService_GetTopNDaysByMinTemperature_NBelowLowerBound_ShouldThrow()
        {
            // Arrange
            var weatherService = new WeatherService(_stubClientCustom);
            var analysisService = new WeatherAnalysisService(weatherService);
            string city = "A";
            int n = 0;

            // Act & Assert
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => analysisService.GetTopNDaysByMinTemperature(city, n));
            Assert.That(ex.ParamName, Is.EqualTo("n"));
        }

        /// <summary>
        /// D57_WeatherAnalysisService: GetTopNDaysByMinTemperature - n above upper bound (n = 10)
        /// Ensures that GetTopNDaysByMinTemperature caps n to available forecast count if n exceeds it.
        /// </summary>
        [Test]
        [Category("Domain")]
        [Description("D57_WeatherAnalysisService - Verify GetTopNDaysByMinTemperature caps n to forecast count when n = 10.")]
        public void D57_WeatherAnalysisService_GetTopNDaysByMinTemperature_NAboveUpperBound()
        {
            // Arrange
            var weatherService = new WeatherService(_stubClientCustom);
            var analysisService = new WeatherAnalysisService(weatherService);
            string city = "A";
            int n = 10;

            // Act
            var result = analysisService.GetTopNDaysByMinTemperature(city, n).ToList();

            // Assert
            Assert.IsNotNull(result, "Result should not be null.");
            Assert.That(result.Count, Is.EqualTo(5), "Should return maximum available days (5) when n exceeds forecast count.");
        }

        /// <summary>
        /// D58_WeatherAnalysisService: GetDaysWithTemperatureRange - Regular valid range
        /// Ensures that GetDaysWithTemperatureRange returns days within a valid min/max range.
        /// </summary>
        [Test]
        [Category("Domain")]
        [Description("D58_WeatherAnalysisService - Verify GetDaysWithTemperatureRange for a valid range.")]
        public void D58_WeatherAnalysisService_GetDaysWithTemperatureRange_ValidRange()
        {
            var weatherService = new WeatherService(_stubClientCustom);
            var analysisService = new WeatherAnalysisService(weatherService);
            string city = "A";
            int minTemp = 0;
            int maxTemp = 5;

            var result = analysisService.GetDaysWithTemperatureRange(city, minTemp, maxTemp).ToList();

            Assert.IsNotNull(result, "Result should not be null.");
            Assert.IsTrue(result.All(f => f.TemperatureMin >= minTemp && f.TemperatureMax <= maxTemp),
                "All returned days should have temperatures within the specified range.");
        }

        /// <summary>
        /// D59_WeatherAnalysisService: GetDaysWithTemperatureRange - Narrow valid range
        /// Ensures that method works when min and max are very close but valid.
        /// </summary>
        [Test]
        [Category("Domain")]
        [Description("D59_WeatherAnalysisService - Verify GetDaysWithTemperatureRange for a narrow valid range.")]
        public void D59_WeatherAnalysisService_GetDaysWithTemperatureRange_NarrowValidRange()
        {
            var weatherService = new WeatherService(_stubClientCustom);
            var analysisService = new WeatherAnalysisService(weatherService);
            string city = "A";
            double minTemp = 1.99;
            double maxTemp = 2;

            var result = analysisService.GetDaysWithTemperatureRange(city, (int)Math.Floor(minTemp), (int)Math.Ceiling(maxTemp)).ToList();

            Assert.IsNotNull(result, "Result should not be null.");
            Assert.IsTrue(result.All(f => f.TemperatureMin >= minTemp && f.TemperatureMax <= maxTemp),
                "All returned days should have temperatures within the narrow range.");
        }

        /// <summary>
        /// D60_WeatherAnalysisService: GetDaysWithTemperatureRange - min equals max
        /// Ensures that days with exact temperature match are returned.
        /// </summary>
        [Test]
        [Category("Domain")]
        [Description("D60_WeatherAnalysisService - Verify GetDaysWithTemperatureRange when min equals max.")]
        public void D60_WeatherAnalysisService_GetDaysWithTemperatureRange_MinEqualsMax()
        {
            var weatherService = new WeatherService(_stubClientCustom);
            var analysisService = new WeatherAnalysisService(weatherService);
            string city = "A";
            int minTemp = 2;
            int maxTemp = 2;

            var result = analysisService.GetDaysWithTemperatureRange(city, minTemp, maxTemp).ToList();

            Assert.IsNotNull(result, "Result should not be null.");
            Assert.IsTrue(result.All(f => f.TemperatureMin >= minTemp && f.TemperatureMax <= maxTemp),
                "All returned days should exactly match the min=max value.");
        }

        /// <summary>
        /// D61_WeatherAnalysisService: GetDaysWithTemperatureRange - Invalid range (min > max)
        /// Ensures that an exception is thrown when minTemp is greater than maxTemp.
        /// </summary>
        [Test]
        [Category("Domain")]
        [Description("D61_WeatherAnalysisService - Verify GetDaysWithTemperatureRange throws when min > max.")]
        public void D61_WeatherAnalysisService_GetDaysWithTemperatureRange_MinGreaterThanMax_ShouldThrow()
        {
            var weatherService = new WeatherService(_stubClientCustom);
            var analysisService = new WeatherAnalysisService(weatherService);
            string city = "A";
            int minTemp = 5;
            int maxTemp = 3;

            var ex = Assert.Throws<ArgumentException>(() => analysisService.GetDaysWithTemperatureRange(city, minTemp, maxTemp));
            Assert.That(ex.Message, Does.Contain("minTemp cannot be greater than maxTemp"));
        }

        /// <summary>
        /// D62_WeatherAnalysisService: GetDaysWithTemperatureRange - Invalid close range
        /// Ensures that an exception is thrown even if minTemp is only slightly greater than maxTemp.
        /// </summary>
        [Test]
        [Category("Domain")]
        [Description("D62_WeatherAnalysisService - Verify GetDaysWithTemperatureRange throws when min slightly > max.")]
        public void D62_WeatherAnalysisService_GetDaysWithTemperatureRange_MinSlightlyGreaterThanMax_ShouldThrow()
        {
            var weatherService = new WeatherService(_stubClientCustom);
            var analysisService = new WeatherAnalysisService(weatherService);
            string city = "A";
            double minTemp = 2.01;
            double maxTemp = 2;

            var ex = Assert.Throws<ArgumentException>(() => analysisService.GetDaysWithTemperatureRange(city, (int)Math.Ceiling(minTemp), (int)maxTemp));
            Assert.That(ex.Message, Does.Contain("minTemp cannot be greater than maxTemp"));
        }
    }
}
