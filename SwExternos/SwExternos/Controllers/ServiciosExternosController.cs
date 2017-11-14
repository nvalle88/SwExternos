using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using System.Security.Cryptography;
using SwExternos.Utils;
using SwExternos.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace SwExternos.Controllers
{
    [Produces("application/json")]
    [Route("api/Credenciales")]
    public class ServiciosExternosController : Controller
    {

        private readonly IApiServicio apiServicio;
        private IConfiguration Configuration;

        public ServiciosExternosController(IApiServicio apiServicio)
        {
            this.apiServicio = apiServicio;
        }
        // GET: api/Adscpassws
      

        [HttpPost]
        [Route("Login")]
        public async Task<Response> Login([FromBody] Login login)
        {
            if (string.IsNullOrEmpty(login.Usuario) || string.IsNullOrEmpty(login.Contrasena))
            {
                return new Utils.Response
                {
                    IsSuccess = false,
                    Message = Mensaje.UsuarioIncorrecto,
                    Resultado=null,

                };
            }


            var response = await apiServicio.ObtenerElementoAsync1<Response>(login,
                                                              new Uri(Configuration.GetSection("HostServicioSeguridad").Value),
                                                              "/api/Adscpassws/Login");

            if (!response.IsSuccess)
            {
                return new Utils.Response
                {
                    IsSuccess=false,
                    Message=Mensaje.UsuarioIncorrecto,
                    Resultado=null,

                };
            }

            var usuario = JsonConvert.DeserializeObject<Adscpassw>(response.Resultado.ToString());

            Guid guidUsuario;
            guidUsuario = Guid.NewGuid();

            var permisoUsuario = new PermisoUsuario
            {
                Usuario = usuario.AdpsLogin,
                Token = Convert.ToString(guidUsuario),
            };
            var salvarToken = await apiServicio.InsertarAsync<Response>(permisoUsuario, new Uri(Configuration.GetSection("HostServicioSeguridad").Value), "/api/Adscpassws/SalvarToken");


            return new Utils.Response
            {
                IsSuccess=true,
                Message=Mensaje.Satisfactorio,
                Resultado=permisoUsuario,
            };
        }

        public async Task<Response> ConsumirServicios(PermisoUsuario permisoUsuario)
        {
            var respuesta = apiServicio.ObtenerElementoAsync1<Response>(permisoUsuario, new Uri(Configuration.GetSection("HostServicioSeguridad").Value), "/api/Adscpassws/TienePermiso");

            if (respuesta.Result.IsSuccess)
            {
                return null;
            }
            return null;
        }
    }
}