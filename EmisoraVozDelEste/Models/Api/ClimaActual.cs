using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace EmisoraVozDelEste.Models.Api
{
    public class ClimaActual
    {
        [JsonProperty("main")]
        public MainInfo Main { get; set; }

        [JsonProperty("weather")]
        public List<WeatherDescription> Weather { get; set; }

        [JsonProperty("wind")]
        public Wind Wind { get; set; }

        [JsonProperty("dt")]
        public long UnixTime { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        public DateTime Fecha => DateTimeOffset.FromUnixTimeSeconds(UnixTime).DateTime.ToLocalTime();
    }

    public class Wind
    {
        [JsonProperty("speed")]
        public double Speed { get; set; }
    }
}
