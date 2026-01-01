using RomanianWeather.API.Interfaces;
using RomanianWeather.API.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;

namespace RomanianWeather.API.Providers
{
    public class MeteoApiClient : IWeatherApiClient
    {
        private readonly HttpClient _httpClient;

        public MeteoApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public IEnumerable<WeatherSnapshot> GetTodayWeather()
        {
            var response = _httpClient.GetStringAsync("https://www.meteoromania.ro/wp-json/meteoapi/v2/starea-vremii").Result;

            using JsonDocument doc = JsonDocument.Parse(response);
            var features = doc.RootElement.GetProperty("features");

            var weatherList = new List<WeatherSnapshot>();

            foreach (var feature in features.EnumerateArray())
            {
                var props = feature.GetProperty("properties");

                // Temperature can be number or string
                double temp = 0;
                if (props.TryGetProperty("tempe", out var tempProp))
                {
                    if (tempProp.ValueKind == JsonValueKind.Number)
                        temp = tempProp.GetDouble();
                    else if (tempProp.ValueKind == JsonValueKind.String)
                        double.TryParse(tempProp.GetString(), out temp);
                }

                // Humidity can be number or string
                int hum = 0;
                if (props.TryGetProperty("umezeala", out var humProp))
                {
                    if (humProp.ValueKind == JsonValueKind.Number)
                        hum = humProp.GetInt32();
                    else if (humProp.ValueKind == JsonValueKind.String)
                        int.TryParse(humProp.GetString(), out hum);
                }

                string city = props.GetProperty("nume").GetString() ?? "Unknown";
                string description = props.TryGetProperty("nebulozitate", out var nebProp)
                                        ? nebProp.GetString() ?? "indisponibil"
                                        : "indisponibil";

                weatherList.Add(new WeatherSnapshot
                {
                    City = city,
                    Temperature = temp,
                    Humidity = hum,
                    WeatherDescription = description
                });
            }

            return weatherList;
        }

        public IEnumerable<ForecastDay> Get5DayForecast(string city)
        {
            // Download raw bytes (ignore charset problems)
            var bytes = _httpClient.GetByteArrayAsync("https://www.meteoromania.ro/anm/prognoza-orase-xml.php").Result;

            // Let XML parser handle encoding from XML declaration
            var xmlString = Encoding.UTF8.GetString(bytes);

            var xmlDoc = XDocument.Parse(xmlString);

            // Find matching city
            var cityNode = xmlDoc
                .Descendants("localitate")
                .FirstOrDefault(x =>
                    string.Equals(x.Attribute("nume")?.Value, city, StringComparison.OrdinalIgnoreCase)
                );

            if (cityNode == null)
                return new List<ForecastDay>();

            // Parse up to 5 days
            var forecastList = cityNode
                .Elements("prognoza")
                .Take(5)
                .Select(p => new ForecastDay
                {
                    Date = p.Attribute("data")?.Value ?? "",
                    TemperatureMin = double.TryParse(p.Element("temp_min")?.Value, out double tmin) ? tmin : 0,
                    TemperatureMax = double.TryParse(p.Element("temp_max")?.Value, out double tmax) ? tmax : 0,
                    WeatherDescription = p.Element("fenomen_descriere")?.Value ?? ""
                })
                .ToList();

            return forecastList;
        }
    }
}
