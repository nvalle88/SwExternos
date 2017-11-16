using Microsoft.AspNetCore.Mvc;
using ServiciosExternos.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServiciosExternos.Interfaces
{
    public interface IApiServicio
    {
        Task<Response> InsertarAsync<T>(T model, Uri baseAddress, string url);
        Task<Response> InsertarAsync<T>(object model, Uri baseAddress, string url);
        Task<T> ObtenerElementoAsync1<T>(object model, Uri baseAddress, string url) where T : class;
        Task<object> ConsumirServicio(PermisoUsuario model, Uri baseAddress, string url);
    }
}
