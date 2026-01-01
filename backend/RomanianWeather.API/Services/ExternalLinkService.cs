using RomanianWeather.API.Interfaces;

namespace RomanianWeather.API.Services
{
    public class ExternalLinkService : IExternalLinkService
    {
        public string GetCityWeatherLink(string cityName)
        {
            // Replace spaces with dashes, Romanian style URL maybe
            var cityFormatted = cityName.Replace(" ", "-").ToLower();
            return $"https://www.meteoromania.ro/{cityFormatted}";
        }
    }
}
