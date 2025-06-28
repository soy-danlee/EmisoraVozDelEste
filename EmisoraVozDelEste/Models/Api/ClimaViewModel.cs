using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmisoraVozDelEste.Models.Api
{
    public class ClimaViewModel
    {
        public ClimaActual Actual { get; set; }
        public List<WeatherItem> Pronostico { get; set; }
        public List<WeatherItem> BloquesHoy { get; set; } 
        public string Ciudad { get; set; }
    }
}