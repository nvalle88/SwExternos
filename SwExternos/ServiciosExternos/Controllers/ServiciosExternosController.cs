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

        #region Métodos
        private async Task<ConsumirServicio> LoginAsync(ConsumirServicio consumirServicio)
        {
            if (string.IsNullOrEmpty(consumirServicio.Usuario) || string.IsNullOrEmpty(consumirServicio.Contrasena))
            {
                return new ConsumirServicio
                {
                    Satisfactorio = false,
                };
            }

            var login = new Login
            {
                Contrasena = consumirServicio.Contrasena,
                Usuario = consumirServicio.Usuario,
            };

            var response = await apiServicio.ObtenerElementoAsync1<Response>(login,
                                                              new Uri(WebApp.BasseAdrees),
                                                              "api/Adscpassws/Login");

            if (!response.IsSuccess)
            {
                return new ConsumirServicio { Satisfactorio = false };
            }

            consumirServicio.Satisfactorio = true;
            return consumirServicio;

        }

        private async Task<ConsumirServicio> SalvarToken(ConsumirServicio consumirServicio)
        {
            consumirServicio.Token = Convert.ToString(Guid.NewGuid());

            var salvarToken = await apiServicio.InsertarAsync<Response>(consumirServicio, new Uri(WebApp.BasseAdrees), "/api/Adscpassws/SalvarTokenSwExternos");

            if (!salvarToken.IsSuccess)
            {
                return new ConsumirServicio { Satisfactorio = false };
            }

            var token = JsonConvert.DeserializeObject<Adsctoken>(salvarToken.Resultado.ToString());
            consumirServicio.Satisfactorio = true;
            consumirServicio.Id = token.AdtoId;
            return consumirServicio;

        }

        private async Task<ConsumirServicio> TieneAcceso(ConsumirServicio consumirServicio)
        {
            consumirServicio.Token = Convert.ToString(Guid.NewGuid());

            var salvarToken = await apiServicio.ObtenerElementoAsync1<Response>(consumirServicio, new Uri(WebApp.BasseAdrees), "api/Adscpassws/TieneAcceso");

            if (!salvarToken.IsSuccess)
            {
                return new ConsumirServicio { Satisfactorio = false };
            }
            consumirServicio.Satisfactorio = true;
            return consumirServicio;

        }

        private async Task<ConsumirServicio> ValidarPermisoToken(ConsumirServicio consumirServicio)
        {

            var respuesta = await apiServicio.ObtenerElementoAsync1<Response>(consumirServicio, new Uri(WebApp.BasseAdrees), "api/Adscpassws/TienePermisoSwExterno");

            if (!respuesta.IsSuccess)
            {
                return new ConsumirServicio { Satisfactorio = false };
            }
            consumirServicio.Satisfactorio = true;
            return consumirServicio;

        }

        private async Task<JsonResult> Consumir(ConsumirServicio consumirServicio)
        {
            var a = Json(await apiServicio.ConsumirServicio(consumirServicio, new Uri(WebApp.BasseAdrees), "/api/Adscpassws/ConsumirSwExterno"));

            if (a != null)
            {
                return a;
            }
            return Json(false);


        }
        #endregion

        #region Servicios

        [HttpPost]
        [Route("ConsumirServicios")]
        public async Task<JsonResult> ConsumirServicios([FromBody]ConsumirServicio consumirServicio)
        {

            if (string.IsNullOrEmpty(consumirServicio.Usuario) || string.IsNullOrEmpty(consumirServicio.Token) ||
                           string.IsNullOrEmpty(consumirServicio.NombreServicio))
            {
                return Json(false);
            }


            consumirServicio = await ValidarPermisoToken(consumirServicio);

            if (!consumirServicio.Satisfactorio)
            {
                return Json(false);
            }

            var json = await Consumir(consumirServicio);

            if (json != null)
            {
                return Json(json.Value);
            }

            return Json(false);

        }

        // POST: api/Credenciales
        [HttpPost]
        [Route("Validar")]
        public async Task<JsonResult> Validar([FromBody]ConsumirServicio consumirServicio)
        {


            try
            {
                if (string.IsNullOrEmpty(consumirServicio.Usuario) || string.IsNullOrEmpty(consumirServicio.Contrasena) ||
                            string.IsNullOrEmpty(consumirServicio.NombreServicio))
                {
                    return Json(false);
                }

                consumirServicio = await LoginAsync(consumirServicio);

                if (!consumirServicio.Satisfactorio)
                {
                    return Json(false);
                }

                consumirServicio = await TieneAcceso(consumirServicio);

                if (!consumirServicio.Satisfactorio)
                {
                    return Json(false);
                }

                consumirServicio = await SalvarToken(consumirServicio);

                if (!consumirServicio.Satisfactorio)
                {
                    return Json(false);
                }

                var respusta = new RespuestaToken
                {
                    Id = consumirServicio.Id,
                    Token = consumirServicio.Token,
                };
                return Json(respusta);
            }
            catch (Exception ex)
            {

                return Json(false);
            }
        } 

        #endregion

    }
}