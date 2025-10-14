using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using modelo;

namespace negocio
{
    public class ImagenNegocio
    {
        public List<Imagen> ListarImagenesPorArticulo(int IdArticulo)
        {
            List<Imagen> lista = new List<Imagen>();
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta("SELECT Id, IdArticulo, ImagenURL FROM IMAGENES WHERE IdArticulo = " + IdArticulo);
                datos.ejecutarLectura();

                while (datos.Lector.Read())
                {
                    Imagen aux = new Imagen();
                    aux.Id = (int)datos.Lector["Id"];
                    aux.IdArticulo = (int)datos.Lector["IdArticulo"];
                    aux.ImagenURL = (string)datos.Lector["ImagenURL"];
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

        public void AgregarImagen(int articuloId, string urlImagen)
            {
                AccesoDatos datos = new AccesoDatos();

                try
                {
                    datos.setearConsulta("INSERT INTO Imagenes (IdArticulo, ImagenUrl) VALUES (@IdArticulo, @ImagenUrl)");
                    datos.setearParametros("@IdArticulo", articuloId);
                    datos.setearParametros("@ImagenUrl", urlImagen);

                    datos.ejecutarAccion();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al insertar la imagen: " + ex.Message);
                }
                finally
                {
                    datos.cerrarConexion();
                }
            }
        
    }
    }
