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
    public class CategoriaNegocio
    {
        public List<Categoria> listarCategorias()
        {
            List<Categoria> listaCategorias = new List<Categoria>();
           AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta("SELECT Id, Descripcion FROM CATEGORIAS");
                datos.ejecutarLectura();
                while (datos.Lector.Read())
                {
                    Categoria aux = new Categoria();
                    aux.Id = (int)datos.Lector["Id"];
                    if (!(datos.Lector["Descripcion"] is DBNull))
                        aux.Descripcion = (string)datos.Lector["Descripcion"];
                    listaCategorias.Add(aux);
                }

                return listaCategorias;
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
        public Categoria buscarCategoria(int id)
        {
            AccesoDatos datos = new AccesoDatos();
            Categoria categoria = null;

            try
            {
                datos.setearConsulta("SELECT Id, Descripcion FROM CATEGORIAS WHERE Id = @Id");
                datos.setearParametros("@Id", id);
                datos.ejecutarLectura();

                if (datos.Lector.Read())
                {
                    categoria = new Categoria();
                    categoria.Id = (int)datos.Lector["Id"];
                    if (!(datos.Lector["Descripcion"] is DBNull))
                        categoria.Descripcion = (string)datos.Lector["Descripcion"];
                }

                return categoria; // si no encuentra nada, devuelve null
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