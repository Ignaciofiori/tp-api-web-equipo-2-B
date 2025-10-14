using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using modelo;
using negocio;

namespace WebAPI.Controllers
{
    public class ArticuloController : ApiController
    {
        // GET: api/Articulo
      public HttpResponseMessage Get()
        {
            try
            {
                var negocio = new ArticuloNegocio();
                var lista = negocio.ListarArticulos();

                if (lista == null || lista.Count == 0)
                {
                    var response = new ApiResponse(HttpStatusCode.NotFound, "No se encontraron artículos.");
                    return Request.CreateResponse(HttpStatusCode.NotFound, response);
                }

                var responseSuccess = new ApiResponse(HttpStatusCode.OK, "Artículos recuperados con éxito.", lista);
                return Request.CreateResponse(HttpStatusCode.OK, responseSuccess);
            }
            catch (Exception ex)
            {
                var response = new ApiResponse(HttpStatusCode.InternalServerError, "Momentaneamente Fuera de Servicio.");
                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                    response);
            }
        }


        // GET: api/Articulo/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Articulo
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Articulo/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Articulo/5
        public void Delete(int id)
        {
        }
    }
}
