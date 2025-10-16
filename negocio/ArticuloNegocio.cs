using modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace negocio
{
    public class ArticuloNegocio
    {
        public List<Articulo> ListarArticulos()
        {
            List<Articulo> lista = new List<Articulo>();
            AccesoDatos datos = new AccesoDatos();
            ImagenNegocio imagenNegocio = new ImagenNegocio();

            try
            {
                datos.setearConsulta("SELECT A.Id, A.Codigo, A.Nombre, A.Descripcion, M.Descripcion as Marca, C.Descripcion as Categoria, A.Precio, A.IdCategoria, A.IdMarca FROM ARTICULOS A, MARCAS M, CATEGORIAS C WHERE A.IdMarca = M.Id AND A.IdCategoria = C.Id\r\n");
                datos.ejecutarLectura();

                while (datos.Lector.Read())
                {
                    Articulo aux = new Articulo();
                    //id
                    aux.Id = (int)datos.Lector["Id"];
                    //Codigo
                    //validacion de null
                    if (!(datos.Lector["Codigo"] is DBNull))
                        aux.CodigoArticulo = (string)datos.Lector["Codigo"];
                    //NOMBRE
                    if (!(datos.Lector["Nombre"] is DBNull))
                        aux.Nombre = (string)datos.Lector["Nombre"];
                    //Descripcion
                    if (!(datos.Lector["Descripcion"] is DBNull))
                        aux.Descripcion = (string)datos.Lector["Descripcion"];
                    //Precio
                    if (!(datos.Lector["Precio"] is DBNull))
                        aux.Precio = (decimal)datos.Lector["Precio"];

                    //Marca
                    aux.Marca = new Marca();
                    if (!(datos.Lector["Marca"] is DBNull))
                        aux.Marca.Descripcion = (string)datos.Lector["Marca"];
                        aux.Marca.Id = (int)datos.Lector["IdMarca"];
                    //Categoria
                    aux.Categoria = new Categoria();
                    if (!(datos.Lector["Categoria"] is DBNull))
                        aux.Categoria.Descripcion = (string)datos.Lector["Categoria"];
                        aux.Categoria.Id = (int)datos.Lector["IdCategoria"];
                    //Imagenes
                    aux.Imagenes = imagenNegocio.ListarImagenesPorArticulo(aux.Id);

                    lista.Add(aux);
                }

                return lista;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public int agregarArticulo(Articulo articulo)
        {
            int idGenerado = 0;
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("INSERT INTO Articulos (Codigo, Nombre, Descripcion, Precio, IdMarca, IdCategoria) " +
                                     "VALUES (@Codigo, @Nombre, @Descripcion, @Precio, @IdMarca, @IdCategoria); " +
                                     "SELECT CAST(SCOPE_IDENTITY() AS INT);");
                datos.setearParametros("@Codigo", articulo.CodigoArticulo);
                datos.setearParametros("@Nombre", articulo.Nombre);
                datos.setearParametros("@Descripcion", articulo.Descripcion);
                datos.setearParametros("@Precio", articulo.Precio);
                datos.setearParametros("@IdMarca", articulo.Marca.Id);
                datos.setearParametros("@IdCategoria", articulo.Categoria.Id);

                datos.command.Connection = datos.connection;
                datos.connection.Open();
                idGenerado = (int)datos.command.ExecuteScalar(); // ya ejecuta el insert y devuelve el id
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (datos.connection.State != System.Data.ConnectionState.Closed)
                    datos.connection.Close();
            }
            return idGenerado;
        }

        public void modificarArticulo(Articulo articulo)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("UPDATE ARTICULOS SET Codigo=@Codigo, Nombre=@Nombre, Descripcion=@Descripcion, Precio=@Precio, IdMarca=@IdMarca, IdCategoria=@IdCategoria WHERE Id=@Id");
                datos.setearParametros("@Codigo", articulo.CodigoArticulo);
                datos.setearParametros("@Nombre", articulo.Nombre);
                datos.setearParametros("@Descripcion", articulo.Descripcion);
                datos.setearParametros("@Precio", articulo.Precio);
                datos.setearParametros("@IdMarca", articulo.Marca.Id);
                datos.setearParametros("@IdCategoria", articulo.Categoria.Id);
                datos.setearParametros("@Id", articulo.Id);
                
                datos.ejecutarAccion();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public List<Articulo> BuscarArticulo(string campo, string criterio, string filtro)
        {
            AccesoDatos datos = new AccesoDatos();
            List<Articulo> lista = new List<Articulo>();
            ImagenNegocio imagenNegocio = new ImagenNegocio();

            try
            {
                string consulta = "SELECT A.Id, A.Codigo, A.Nombre, A.Descripcion, M.Descripcion as Marca, C.Descripcion as Categoria, A.Precio, A.IdCategoria, A.IdMarca FROM ARTICULOS A, MARCAS M, CATEGORIAS C WHERE A.IdMarca = M.Id AND A.IdCategoria = C.Id";

                if (campo == "Precio")
                {
                    decimal valorPrecio = decimal.Parse(filtro.Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);
                    string parametro = "@filtro";
                    switch (criterio)
                    {
                        case "Mayor a":
                            consulta += " AND A.Precio > " + parametro;
                            break;
                        case "Menor a":
                            consulta += " AND A.Precio < " + parametro;
                            break;
                        default: 
                            consulta += " AND A.Precio = " + parametro;
                            break;
                    }
                    datos.setearParametros(parametro, valorPrecio);
                }
                else if (campo == "Nombre")
                {
                    switch (criterio)
                    {
                        case "Comienza con":
                            consulta += " AND A.Nombre like '" + filtro + "%'";
                            break;
                        case "Termina con":
                            consulta += " AND A.Nombre like '%" + filtro + "'";
                            break;
                        default:
                            consulta += " AND A.Nombre like '%" + filtro + "%'";
                            break;
                    }
                }
                else if (campo == "Descripcion")
                {
                    switch (criterio)
                    {
                        case "Comienza con":
                            consulta += " AND A.Descripcion like '" + filtro + "%'";
                            break;
                        case "Termina con":
                            consulta += " AND A.Descripcion like '%" + filtro + "'";
                            break;
                        default:
                            consulta += " AND A.Descripcion like '%" + filtro + "%'";
                            break;
                    }
                }
                else if (campo == "Marca")
                {
                    switch (criterio)
                    {
                        case "Comienza con":
                            consulta += " AND M.Descripcion like '" + filtro + "%'";
                            break;
                        case "Termina con":
                            consulta += " AND M.Descripcion like '%" + filtro + "'";
                            break;
                        default:
                            consulta += " AND M.Descripcion like '%" + filtro + "%'";
                            break;
                    }
                }
                else if(campo == "Id")
                {
                    consulta += " AND A.Id = " + filtro;
                }
                else if(campo == "Codigo")
                {
                    consulta += "AND A.Codigo = '" + filtro + "'";
                }
                else if (campo == "Categoria")
                {
                    switch (criterio)
                    {
                        case "Comienza con":
                            consulta += " AND C.Descripcion like '" + filtro + "%'";
                            break;
                        case "Termina con":
                            consulta += " AND C.Descripcion like '%" + filtro + "'";
                            break;
                        default:
                            consulta += " AND C.Descripcion like '%" + filtro + "%'";
                            break;
                    }
                }

                datos.setearConsulta(consulta);
                datos.ejecutarLectura();

                while(datos.Lector.Read())
                {
                    Articulo aux = new Articulo();
                    aux.Id = (int)datos.Lector["Id"];
                        aux.CodigoArticulo = (string)datos.Lector["Codigo"];
                    if (!(datos.Lector["Nombre"] is DBNull))
                        aux.Nombre = (string)datos.Lector["Nombre"];
                    if (!(datos.Lector["Descripcion"] is DBNull))
                        aux.Descripcion = (string)datos.Lector["Descripcion"];
                    if (!(datos.Lector["Precio"] is DBNull))
                        aux.Precio = (decimal)datos.Lector["Precio"];

                    aux.Marca = new Marca();
                    aux.Marca.Descripcion = (string)datos.Lector["Marca"];

                    aux.Categoria = new Categoria();
                    aux.Categoria.Descripcion = (string)datos.Lector["Categoria"];

                    aux.Imagenes = imagenNegocio.ListarImagenesPorArticulo(aux.Id);

                    lista.Add(aux);
                }
                return lista;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public void eliminarFisico(int d)
        {
            try
            {
                AccesoDatos datos = new AccesoDatos();
                datos.setearConsulta("DELETE FROM ARTICULOS WHERE Id = @Id");
                datos.setearParametros("@Id", d);
                datos.ejecutarAccion();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Articulo BuscarPorId(int id)
        {
            AccesoDatos datos = new AccesoDatos();
            Articulo articulo = null;
            ImagenNegocio imagenNegocio = new ImagenNegocio();

            try
            {
                datos.setearConsulta("SELECT A.Id, A.Codigo, A.Nombre, A.Descripcion, A.Precio, " +
                             "M.Id IdMarca, M.Descripcion Marca, " +
                             "C.Id IdCategoria, C.Descripcion Categoria " +
                             "FROM ARTICULOS A " +
                             "INNER JOIN MARCAS M ON A.IdMarca = M.Id " +
                             "INNER JOIN CATEGORIAS C ON A.IdCategoria = C.Id " +
                             "WHERE A.Id = @id"); // <--- Filtramos por el ID


                datos.setearParametros("@id", id);

                datos.ejecutarLectura();

                if (datos.Lector.Read())
                {
                    Articulo aux = new Articulo();

                    aux.Id = (int)datos.Lector["Id"];

                    if (!(datos.Lector["Codigo"] is DBNull))
                        aux.CodigoArticulo = (string)datos.Lector["Codigo"];

                    if (!(datos.Lector["Nombre"] is DBNull))
                        aux.Nombre = (string)datos.Lector["Nombre"];

                    if (!(datos.Lector["Descripcion"] is DBNull))
                        aux.Descripcion = (string)datos.Lector["Descripcion"];

                    if (!(datos.Lector["Precio"] is DBNull))
                        aux.Precio = (decimal)datos.Lector["Precio"];

                    aux.Marca = new Marca();
                    if (!(datos.Lector["Marca"] is DBNull))
                        aux.Marca.Descripcion = (string)datos.Lector["Marca"];

                    aux.Marca.Id = (int)datos.Lector["IdMarca"];

                    aux.Categoria = new Categoria();
                    if (!(datos.Lector["Categoria"] is DBNull))
                        aux.Categoria.Descripcion = (string)datos.Lector["Categoria"];

                    aux.Categoria.Id = (int)datos.Lector["IdCategoria"];

                    aux.Imagenes = imagenNegocio.ListarImagenesPorArticulo(aux.Id);

                    articulo = aux;
                }

                return articulo;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }
    }
}
