using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace EmisoraVozDelEste.Models.Api
{
    public class ClimaOnline
    {
        [JsonProperty("list")]
        public List<WeatherItem> List { get; set; }

        [JsonProperty("city")]
        public City City { get; set; }

        // Método auxiliar para deserializar
        public static ClimaOnline FromJson(string json) =>
            JsonConvert.DeserializeObject<ClimaOnline>(json);
    }

    public class WeatherItem
    {
        [JsonProperty("main")]
        public MainInfo Main { get; set; }

        [JsonProperty("weather")]
        public List<WeatherDescription> Weather { get; set; }

        [JsonProperty("dt_txt")]
        public DateTime DtTxt { get; set; }
    }

    public class MainInfo
    {
        [JsonProperty("temp")]
        public double Temp { get; set; }

        [JsonProperty("feels_like")]
        public double FeelsLike { get; set; }
    }

    public class WeatherDescription
    {
        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("icon")]
        public string Icon { get; set; }
    }

    public class City
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}

