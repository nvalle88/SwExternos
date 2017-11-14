using System;

namespace SwExternos.Utils
{
    public  class PermisoUsuario
    {
        public Uri Uri  { get; set; }
        public string Usuario { get; set; }
        public string Token { get; set; }
        public object parametros { get; set; }
    }
}
