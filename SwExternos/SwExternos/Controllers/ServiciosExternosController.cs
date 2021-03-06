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
using SwExternos.Servicios;

namespace SwExternos.Controllers
{
    [Produces("application/json")]
    [Route("api/Credenciales")]
    public class ServiciosExternosController : Controller
    {

     
        private IConfiguration Configuration;

        public ServiciosExternosController(IApiServicio apiServicio)
        {


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


            var response = await ApiServicio.ObtenerElementoAsync1<Response>(login,
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
                Aplicacion=login.Aplicacion,
                Token = Convert.ToString(guidUsuario),
            };
            var salvarToken = await ApiServicio.InsertarAsync<Response>(permisoUsuario, new Uri(Configuration.GetSection("HostServicioSeguridad").Value), "/api/Adscpassws/SalvarTokenSwExternos");


            return new Utils.Response
            {
                IsSuccess=true,
                Message=Mensaje.Satisfactorio,
                Resultado=permisoUsuario,
            };
        }

        // POST: api/Credenciales
        [HttpPost]
        [Route("ConsumirServicios")]
        public async Task<JsonResult> ConsumirServicios([FromBody]PermisoUsuario permisoUsuario)
        {
            try
            {

                if (string.IsNullOrEmpty(permisoUsuario.Usuario) || string.IsNullOrEmpty(permisoUsuario.Token) || string.IsNullOrEmpty(permisoUsuario.Uri) || permisoUsuario.parametros==null)
                {
                    Json(false);
                }


                var respuesta =await ApiServicio.ObtenerElementoAsync1<Response>(permisoUsuario, new Uri(Configuration.GetSection("HostServicioSeguridad").Value), "/api/Adscpassws/ConsumirSwExterno");

                if (respuesta.IsSuccess)
                {
                  return Json(await ApiServicio.ConsumirServicio(permisoUsuario.parametros, permisoUsuario.Uri));
                }

                
            }
            catch (Exception ex)
            {
                Json(false);

            }
            return Json(false);
        }
        
    }
}