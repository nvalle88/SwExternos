using Microsoft.AspNetCore.Mvc;
using SwExternos.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SwExternos.Interfaces
{
    public interface IApiServicio
    {
        Task<Response> InsertarAsync<T>(T model, Uri baseAddress, string url);
        Task<Response> InsertarAsync<T>(object model, Uri baseAddress, string url);
        Task<T> ObtenerElementoAsync1<T>(object model, Uri baseAddress, string url) where T : class;
        Task<Object> ConsumirServicio(object model, string baseAddress);
    }
}
