using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RomanianWeather.API.Models
{
    public class MeteoApiResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("features")]
        public List<MeteoFeature> Features { get; set; } = new List<MeteoFeature>();
    }

    public class MeteoFeature
    {
        [JsonPropertyName("properties")]
        public MeteoProperties Properties { get; set; } = new MeteoProperties();
    }

    public class MeteoProperties
    {
        [JsonPropertyName("nume")]
        public string Nume { get; set; } = string.Empty;

        [JsonPropertyName("tempe")]
        public double Tempe { get; set; } = 0; 

        [JsonPropertyName("umezeala")]
        public int Hum { get; set; } = 0; 

        [JsonPropertyName("nebulozitate")]
        public string Nebulozitate { get; set; } = string.Empty;

        [JsonPropertyName("fenomen_e")]
        public string FenomenE { get; set; } = string.Empty;
    }
}
