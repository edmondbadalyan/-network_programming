using Newtonsoft.Json;

namespace Сетевое_программирование
{
    public class WeatherResponse
    {
        [JsonProperty("weather")]
        public Weather[] Weather { get; set; } = new Weather[0];

        [JsonProperty("main")]
        public Main Main { get; set; } = new Main();

        [JsonProperty("name")]
        public string Name { get; set; } = "";
    }

    public class Weather
    {
        [JsonProperty("main")]
        public string Main { get; set; } = "";

        [JsonProperty("description")]
        public string Description { get; set; } = "";

        [JsonProperty("icon")]
        public string Icon { get; set; } = "";
    }

    public class Main
    {
        [JsonProperty("temp")]
        public double Temp { get; set; }

        [JsonProperty("feels_like")]
        public double FeelsLike { get; set; }

        [JsonProperty("humidity")]
        public int Humidity { get; set; }
    }
} 