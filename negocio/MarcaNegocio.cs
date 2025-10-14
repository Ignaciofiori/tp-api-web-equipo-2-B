using modelo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace negocio
{
    public class MarcaNegocio
    {
        public List<Marca> listarMarcas()
        {
            List<Marca> listaMarcas = new List<Marca>();
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("SELECT Id, Descripcion FROM MARCAS");
                datos.ejecutarLectura();
                while (datos.Lector.Read())
                {
                    Marca aux = new Marca();
                    aux.Id = (int)datos.Lector["Id"];
                    if (!(datos.Lector["Descripcion"] is DBNull))
                        aux.Descripcion = (string)datos.Lector["Descripcion"];
                    listaMarcas.Add(aux);
                }
                return listaMarcas;
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


        public Marca BuscarMarca(int id)
        {
            AccesoDatos datos = new AccesoDatos();
            Marca marca = null;

            try
            {
                datos.setearConsulta("SELECT Id, Descripcion FROM MARCAS WHERE Id = @Id");
                datos.setearParametros("@Id", id);
                datos.ejecutarLectura();

                if (datos.Lector.Read())
                {
                    marca = new Marca();
                    marca.Id = (int)datos.Lector["Id"];
                    if (!(datos.Lector["Descripcion"] is DBNull))
                        marca.Descripcion = (string)datos.Lector["Descripcion"];
                }

                return marca; // Si no encontró nada, devuelve null
            }
            catch (Exception ex)
            {
                throw ex; // O podrías loguear el error y devolver null
            }
            finally
            {
                datos.cerrarConexion();
            }
        }
    }

   
}
