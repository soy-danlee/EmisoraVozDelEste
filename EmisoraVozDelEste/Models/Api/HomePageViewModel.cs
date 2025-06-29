using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmisoraVozDelEste.Models.Api
{
    public class HomePageViewModel
    {
        public Clima ClimaSidebar { get; set; }
        public List<Cotizaciones> Cotizaciones { get; set; }

    }
}