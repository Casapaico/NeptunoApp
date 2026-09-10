using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using NeptunoApp.Models;

namespace NeptunoApp.Data
{
    /// <summary>
    /// Acceso a datos de Categorias mediante PROCEDIMIENTOS ALMACENADOS.
    /// Modo DESCONECTADO para el listado (SqlDataAdapter + DataTable) y
    /// modo CONECTADO para las operaciones puntuales (SqlCommand).
    /// </summary>
    public class CategoriaRepository
    {
        public List<Categoria> Listar()
        {
            var lista = new List<Categoria>();
            var tabla = new DataTable();

            // MODO DESCONECTADO: el adapter abre/cierra la conexion internamente.
            using (var cn = new SqlConnection(ConexionBD.CadenaConexion))
            using (var cmd = new SqlCommand("dbo.usp_Categorias_Listar", cn) { CommandType = CommandType.StoredProcedure })
            using (var adapter = new SqlDataAdapter(cmd))
            {
                adapter.Fill(tabla);
            }

            foreach (DataRow r in tabla.Rows)
            {
                lista.Add(new Categoria
                {
                    IdCategoria = (int)r["IdCategoria"],
                    NombreCategoria = r["NombreCategoria"].ToString(),
                    Descripcion = r["Descripcion"] == System.DBNull.Value ? null : r["Descripcion"].ToString()
                });
            }
            return lista;
        }

        public int Insertar(Categoria c)
        {
            using (var cn = new SqlConnection(ConexionBD.CadenaConexion))
            using (var cmd = new SqlCommand("dbo.usp_Categorias_Insertar", cn) { CommandType = CommandType.StoredProcedure })
            {
                cmd.Parameters.AddWithValue("@NombreCategoria", c.NombreCategoria);
                cmd.Parameters.AddWithValue("@Descripcion", (object)c.Descripcion ?? System.DBNull.Value);
                var pId = new SqlParameter("@IdCategoria", SqlDbType.Int) { Direction = ParameterDirection.Output };
                cmd.Parameters.Add(pId);

                cn.Open();
                cmd.ExecuteNonQuery();
                return (int)pId.Value;
            }
        }

        public void Actualizar(Categoria c)
        {
            using (var cn = new SqlConnection(ConexionBD.CadenaConexion))
            using (var cmd = new SqlCommand("dbo.usp_Categorias_Actualizar", cn) { CommandType = CommandType.StoredProcedure })
            {
                cmd.Parameters.AddWithValue("@IdCategoria", c.IdCategoria);
                cmd.Parameters.AddWithValue("@NombreCategoria", c.NombreCategoria);
                cmd.Parameters.AddWithValue("@Descripcion", (object)c.Descripcion ?? System.DBNull.Value);

                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Eliminar(int idCategoria)
        {
            using (var cn = new SqlConnection(ConexionBD.CadenaConexion))
            using (var cmd = new SqlCommand("dbo.usp_Categorias_Eliminar", cn) { CommandType = CommandType.StoredProcedure })
            {
                cmd.Parameters.AddWithValue("@IdCategoria", idCategoria);
                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
