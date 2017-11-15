using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using System.Security.Cryptography;
using ServiciosExternos.Utils;
using ServiciosExternos.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ServiciosExternos.Servicios;

namespace ServiciosExternos.Controllers
{
    [Produces("application/json")]
    [Route("api/Credenciales")]
    public class ServiciosExternosController : Controller
    {

     
        private IConfiguration Configuration;
        private IApiServicio apiServicio;

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
                                                              new Uri(WebApp.BasseAdrees),
                                                              "api/Adscpassws/Login");

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
            var salvarToken = await apiServicio.InsertarAsync<Response>(permisoUsuario, new Uri(WebApp.BasseAdrees), "api/Adscpassws/SalvarTokenSwExternos");


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


                var respuesta =await apiServicio.ObtenerElementoAsync1<Response>(permisoUsuario, new Uri(WebApp.BasseAdrees), "/api/Adscpassws/ConsumirSwExterno");

                if (respuesta.IsSuccess)
                {
                  return Json(await apiServicio.ConsumirServicio(permisoUsuario.parametros, permisoUsuario.Uri));
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