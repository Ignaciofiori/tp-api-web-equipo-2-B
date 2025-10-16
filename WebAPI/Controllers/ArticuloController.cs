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
    [RoutePrefix("api/Articulo")]
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
        [Route("{id}")]
        [HttpPut]
        public HttpResponseMessage Put(int id, [FromBody] ArticuloDTO mArticuloDto)
        {
            try
            {
                if (id <= 0)
                {
                    var response = new ApiResponse(HttpStatusCode.BadRequest, "El ID del artículo a modificar no es válido.");
                    return Request.CreateResponse(HttpStatusCode.BadRequest, response);
                }

                if (mArticuloDto == null)
                {
                    var response = new ApiResponse(HttpStatusCode.BadRequest, "No se recibieron datos para la modificación.");
                    return Request.CreateResponse(HttpStatusCode.BadRequest, response);
                }

                var articuloNegocio = new ArticuloNegocio();
                var marcaNegocio = new MarcaNegocio();
                var categoriaNegocio = new CategoriaNegocio();

                Articulo articuloExistente = articuloNegocio.BuscarPorId(id);
                if (articuloExistente == null)
                {
                    var response = new ApiResponse(HttpStatusCode.NotFound, $"El artículo con ID {id} no fue encontrado para modificar.");
                    return Request.CreateResponse(HttpStatusCode.NotFound, response);
                }

                Marca marcaIngresada = marcaNegocio.BuscarMarca(mArticuloDto.MarcaId);
                Categoria categoriaIngresada = categoriaNegocio.buscarCategoria(mArticuloDto.CategoriaId);

                if (marcaIngresada == null)
                {
                    var response = new ApiResponse(HttpStatusCode.BadRequest, "Marca Ingresada No Existe.");
                    return Request.CreateResponse(HttpStatusCode.BadRequest, response);
                }

                if (categoriaIngresada == null)
                {
                    var response = new ApiResponse(HttpStatusCode.BadRequest, "Categoria Ingresada No Existe.");
                    return Request.CreateResponse(HttpStatusCode.BadRequest, response);
                }

                if (string.IsNullOrWhiteSpace(mArticuloDto.CodigoArticulo) ||
                string.IsNullOrWhiteSpace(mArticuloDto.Nombre) ||
                string.IsNullOrWhiteSpace(mArticuloDto.Descripcion) ||
                mArticuloDto.Precio <= 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest,
                        new ApiResponse(HttpStatusCode.BadRequest, "Verifique que todos los campos obligatorios (código, nombre, descripción y precio > 0) estén completos."));
                }

                Articulo articuloAActualizar = new Articulo
                {
                    Id = id, 
                    CodigoArticulo = mArticuloDto.CodigoArticulo,
                    Nombre = mArticuloDto.Nombre,
                    Descripcion = mArticuloDto.Descripcion,
                    Precio = mArticuloDto.Precio,
                    Marca = new Marca { Id = mArticuloDto.MarcaId },
                    Categoria = new Categoria { Id = mArticuloDto.CategoriaId },
                };

                articuloNegocio.modificarArticulo(articuloAActualizar);

                var responseSuccess = new ApiResponse(HttpStatusCode.Created, "Artículo Modificado con Éxito.", articuloAActualizar);
                return Request.CreateResponse(HttpStatusCode.Created, responseSuccess);
            }
            catch (Exception ex)
            {
                var response = new ApiResponse(HttpStatusCode.InternalServerError, "Error al Modificar el Articulo.");
                return Request.CreateResponse(HttpStatusCode.InternalServerError, response);
            }
        }

        [Route("{id}/Imagenes")]
        [HttpPut]
        public HttpResponseMessage PutImagenes(int id, [FromBody] ImagenDTO imagenDto)
        {
            ImagenNegocio negocio = new ImagenNegocio();
            ArticuloNegocio aNegocio = new ArticuloNegocio();
            
            if(id <= 0)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest,
                    new ApiResponse(HttpStatusCode.BadRequest, "Ingrese un id valido"));
            }

            if (imagenDto == null || imagenDto.ImagenURL == null || imagenDto.ImagenURL.Count == 0)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest,
                    new ApiResponse(HttpStatusCode.BadRequest, "Debe proporcionar al menos una URL de imagen válida."));
            }

            Articulo ArticuloBusqueda = new Articulo();
            ArticuloBusqueda = aNegocio.BuscarPorId(id);

            if (ArticuloBusqueda == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest,
                    new ApiResponse(HttpStatusCode.BadRequest, "El artículo con el Id ingresado no existe"));
            }

            try
            {
                foreach (string url in imagenDto.ImagenURL)
                {
                    if (!string.IsNullOrWhiteSpace(url))
                    {
                        negocio.AgregarImagen(id, url);
                    }
                }
                return Request.CreateResponse(HttpStatusCode.Created,
                    new ApiResponse(HttpStatusCode.Created, "Imagenes Agregadas con Exito"));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                    new ApiResponse(HttpStatusCode.InternalServerError, "Error al agregar las imágenes."));
            }

        }

        // DELETE: api/Articulo/5
        public void Delete(int id)
        {
        }
    }
}
