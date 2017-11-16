using System;

namespace ServiciosExternos.Utils
{
    public  class PermisoUsuario
    {
        public string NombreServicio { get; set; }
        public string Usuario { get; set; }
        public string Token { get; set; }
        public string Aplicacion { get; set; }
        public object parametros { get; set; }

    }
}
