using modelo;
using negocio;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

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

                // Verificar si la lista esta vacia y retornar 404 si es asi
                if (lista == null || lista.Count == 0)
                {
                    var response = new ApiResponse(HttpStatusCode.NotFound, "No se encontraron artículos.");
                    return Request.CreateResponse(HttpStatusCode.NotFound, response);
                }

                // Retornar 200 OK con la lista de artículos
                var responseSuccess = new ApiResponse(HttpStatusCode.OK, "Artículos recuperados con éxito.", lista);
                return Request.CreateResponse(HttpStatusCode.OK, responseSuccess);
            }
            catch (Exception ex)
            {
                // Manejar excepciones y retornar 500 Internal Server Error
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
        public HttpResponseMessage Post([FromBody]ArticuloDTO nuevoArticuloDto)
        {
            try
            {
                //seteo el negocio
                var articuloNegocio = new ArticuloNegocio();
                var marcaNegocio = new MarcaNegocio();
                var categoriaNegocio = new CategoriaNegocio();

                //Si no viene en el Body un ARTICULODTO
                if (nuevoArticuloDto == null)
                {
                    var response = new ApiResponse(HttpStatusCode.BadRequest, "No se recibieron datos.");
                    return Request.CreateResponse(HttpStatusCode.BadRequest, response);
                }

                //valido marca y categoria Ingresadas
                Marca marcaIngresada = marcaNegocio.BuscarMarca(nuevoArticuloDto.MarcaId);
                Categoria categoriaIngresada = categoriaNegocio.buscarCategoria(nuevoArticuloDto.CategoriaId);

                //si no existen retorno 400 Bad Request
                if(marcaIngresada == null)
                {
                    var response = new ApiResponse(HttpStatusCode.BadRequest, "Marca Ingresada No Existe.");
                    return Request.CreateResponse(HttpStatusCode.BadRequest, response);
                }

                if(categoriaIngresada == null)
                {
                    var response = new ApiResponse(HttpStatusCode.BadRequest, "Categoria Ingresada No Existe.");
                    return Request.CreateResponse(HttpStatusCode.BadRequest, response);
                }


                // Validaciones de formato y tipo
                if (string.IsNullOrWhiteSpace(nuevoArticuloDto.CodigoArticulo))
                    return Request.CreateResponse(HttpStatusCode.BadRequest,
                        new ApiResponse(HttpStatusCode.BadRequest, "El código de artículo es obligatorio."));

                if (string.IsNullOrWhiteSpace(nuevoArticuloDto.Nombre))
                    return Request.CreateResponse(HttpStatusCode.BadRequest,
                        new ApiResponse(HttpStatusCode.BadRequest, "El nombre es obligatorio."));

                if (string.IsNullOrWhiteSpace(nuevoArticuloDto.Descripcion))
                    return Request.CreateResponse(HttpStatusCode.BadRequest,
                        new ApiResponse(HttpStatusCode.BadRequest, "La descripción es obligatoria."));

                //  Validación de precio
                if (nuevoArticuloDto.Precio <= 0)
                    return Request.CreateResponse(HttpStatusCode.BadRequest,
                        new ApiResponse(HttpStatusCode.BadRequest, "El precio debe ser mayor que 0."));



                //creo el articulo nuevo a partir del dto recibido
                Articulo articuloNuevo = new Articulo
                {
                    CodigoArticulo = nuevoArticuloDto.CodigoArticulo,
                    Nombre = nuevoArticuloDto.Nombre,
                    Descripcion = nuevoArticuloDto.Descripcion,
                    Precio = nuevoArticuloDto.Precio,
                    Marca = new Marca { Id = nuevoArticuloDto.MarcaId },
                    Categoria = new Categoria { Id = nuevoArticuloDto.CategoriaId },
                };

                //agrego el articulo
                int idGenerado = articuloNegocio.agregarArticulo(articuloNuevo);

                articuloNuevo.Id = idGenerado; // Asigno el ID generado al artículo nuevo
                articuloNuevo.Marca = marcaIngresada; // Asigno la marca completa
                articuloNuevo.Categoria = categoriaIngresada; // Asigno la categoría completa

                // Retornar 200 OK con el artículo creado
                var responseSuccess = new ApiResponse(HttpStatusCode.OK, "Artículo Creado con Exito.", articuloNuevo);
                return Request.CreateResponse(HttpStatusCode.OK, responseSuccess);
               
            }
            catch (Exception ex)
            {
                // Manejar excepciones y retornar 500 Internal Server Error
                var response = new ApiResponse(HttpStatusCode.InternalServerError, "Error al Crear el Articulo.");
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
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
