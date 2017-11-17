using System;
using System.Collections.Generic;

namespace ServiciosExternos.Utils
{
    public partial class Adsctoken
    {
        public int AdtoId { get; set; }
        public string AdpsLogin { get; set; }
        public string AdtoToken { get; set; }
        public string AdtoNombreServicio { get; set; }
        public DateTime AdtoFecha { get; set; }
    }
}
