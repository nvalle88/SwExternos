using System;

namespace ServiciosExternos.Utils
{
    public  class ConsumirServicio
    {
        public string NombreServicio { get; set; }
        public string Usuario { get; set; }
        public string Contrasena { get; set; }
        public string Token { get; set; }
        public object parametros { get; set; }
        public bool Satisfactorio { get; set; }

    }
}
