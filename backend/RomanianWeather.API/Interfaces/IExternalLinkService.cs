namespace RomanianWeather.API.Interfaces
{
    public interface IExternalLinkService
    {
        string GetCityWeatherLink(string cityName);
    }
}
