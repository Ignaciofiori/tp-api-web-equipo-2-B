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
        public List<Articulo> Get()
        {
            ArticuloNegocio negocio = new ArticuloNegocio();
            return negocio.ListarArticulos();
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
