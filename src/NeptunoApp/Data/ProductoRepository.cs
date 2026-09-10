using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using NeptunoApp.Models;

namespace NeptunoApp.Data
{
    /// <summary>
    /// Acceso a datos de Productos mediante PROCEDIMIENTOS ALMACENADOS.
    /// Listado en modo CONECTADO (SqlDataReader).
    /// </summary>
    public class ProductoRepository
    {
        public List<Producto> Listar()
        {
            var lista = new List<Producto>();
            using (var cn = new SqlConnection(ConexionBD.CadenaConexion))
            using (var cmd = new SqlCommand("dbo.usp_Productos_Listar", cn) { CommandType = CommandType.StoredProcedure })
            {
                cn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new Producto
                        {
                            IdProducto = (int)reader["IdProducto"],
                            NombreProducto = reader["NombreProducto"].ToString(),
                            IdProveedor = Nid(reader, "IdProveedor"),
                            IdCategoria = Nid(reader, "IdCategoria"),
                            NombreProveedor = Str(reader, "NombreProveedor"),
                            NombreCategoria = Str(reader, "NombreCategoria"),
                            CantidadPorUnidad = Str(reader, "CantidadPorUnidad"),
                            PrecioUnidad = (decimal)reader["PrecioUnidad"],
                            UnidadesEnExistencia = (short)reader["UnidadesEnExistencia"],
                            UnidadesEnPedido = (short)reader["UnidadesEnPedido"],
                            NivelNuevoPedido = (short)reader["NivelNuevoPedido"],
                            Suspendido = (bool)reader["Suspendido"]
                        });
                    }
                }
            }
            return lista;
        }

        public int Insertar(Producto p)
        {
            using (var cn = new SqlConnection(ConexionBD.CadenaConexion))
            using (var cmd = new SqlCommand("dbo.usp_Productos_Insertar", cn) { CommandType = CommandType.StoredProcedure })
            {
                CargarParametros(cmd, p, incluirId: false);
                var pId = new SqlParameter("@IdProducto", SqlDbType.Int) { Direction = ParameterDirection.Output };
                cmd.Parameters.Add(pId);

                cn.Open();
                cmd.ExecuteNonQuery();
                return (int)pId.Value;
            }
        }

        public void Actualizar(Producto p)
        {
            using (var cn = new SqlConnection(ConexionBD.CadenaConexion))
            using (var cmd = new SqlCommand("dbo.usp_Productos_Actualizar", cn) { CommandType = CommandType.StoredProcedure })
            {
                CargarParametros(cmd, p, incluirId: true);
                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Eliminar(int idProducto)
        {
            using (var cn = new SqlConnection(ConexionBD.CadenaConexion))
            using (var cmd = new SqlCommand("dbo.usp_Productos_Eliminar", cn) { CommandType = CommandType.StoredProcedure })
            {
                cmd.Parameters.AddWithValue("@IdProducto", idProducto);
                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // ------------------------------------------------------------------
        private static void CargarParametros(SqlCommand cmd, Producto p, bool incluirId)
        {
            if (incluirId) cmd.Parameters.AddWithValue("@IdProducto", p.IdProducto);
            cmd.Parameters.AddWithValue("@NombreProducto", p.NombreProducto);
            cmd.Parameters.AddWithValue("@IdProveedor", (object)p.IdProveedor ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IdCategoria", (object)p.IdCategoria ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CantidadPorUnidad",
                string.IsNullOrWhiteSpace(p.CantidadPorUnidad) ? (object)DBNull.Value : p.CantidadPorUnidad.Trim());
            cmd.Parameters.AddWithValue("@PrecioUnidad", p.PrecioUnidad);
            cmd.Parameters.AddWithValue("@UnidadesEnExistencia", p.UnidadesEnExistencia);
            cmd.Parameters.AddWithValue("@UnidadesEnPedido", p.UnidadesEnPedido);
            cmd.Parameters.AddWithValue("@NivelNuevoPedido", p.NivelNuevoPedido);
            cmd.Parameters.AddWithValue("@Suspendido", p.Suspendido);
        }

        private static int? Nid(SqlDataReader r, string col)
        {
            int i = r.GetOrdinal(col);
            return r.IsDBNull(i) ? (int?)null : r.GetInt32(i);
        }

        private static string Str(SqlDataReader r, string col)
        {
            int i = r.GetOrdinal(col);
            return r.IsDBNull(i) ? null : r.GetValue(i).ToString();
        }
    }
}
