//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EmisoraVozDelEste.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Clima
    {
        public int Id { get; set; }
        public Nullable<System.DateTime> Fecha { get; set; }
        public Nullable<decimal> Temperatura { get; set; }
        public string Descripcion { get; set; }
        public string Icono { get; set; }
    }
}
