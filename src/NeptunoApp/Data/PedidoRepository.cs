using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using NeptunoApp.Models;

namespace NeptunoApp.Data
{
    /// <summary>
    /// Acceso a datos de Pedidos (cabecera) mediante PROCEDIMIENTOS ALMACENADOS.
    /// </summary>
    public class PedidoRepository
    {
        public List<Pedido> Listar()
        {
            var lista = new List<Pedido>();
            using (var cn = new SqlConnection(ConexionBD.CadenaConexion))
            using (var cmd = new SqlCommand("dbo.usp_Pedidos_Listar", cn) { CommandType = CommandType.StoredProcedure })
            {
                cn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        lista.Add(MapearCabecera(reader));
                }
            }
            return lista;
        }

        /// <summary>Devuelve los detalles de un pedido (segundo result set del SP).</summary>
        public List<DetallePedido> ObtenerDetalle(int idPedido)
        {
            var detalle = new List<DetallePedido>();
            using (var cn = new SqlConnection(ConexionBD.CadenaConexion))
            using (var cmd = new SqlCommand("dbo.usp_Pedidos_ObtenerPorId", cn) { CommandType = CommandType.StoredProcedure })
            {
                cmd.Parameters.AddWithValue("@IdPedido", idPedido);
                cn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    // Primer result set: la cabecera (se ignora aqui).
                    // Segundo result set: el detalle.
                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            detalle.Add(new DetallePedido
                            {
                                IdProducto = (int)reader["IdProducto"],
                                NombreProducto = reader["NombreProducto"].ToString(),
                                PrecioUnidad = (decimal)reader["PrecioUnidad"],
                                Cantidad = (short)reader["Cantidad"],
                                Descuento = Convert.ToDouble(reader["Descuento"]),
                                Subtotal = (decimal)reader["Subtotal"]
                            });
                        }
                    }
                }
            }
            return detalle;
        }

        public int Insertar(Pedido p)
        {
            using (var cn = new SqlConnection(ConexionBD.CadenaConexion))
            using (var cmd = new SqlCommand("dbo.usp_Pedidos_Insertar", cn) { CommandType = CommandType.StoredProcedure })
            {
                CargarParametros(cmd, p, incluirId: false);
                var pId = new SqlParameter("@IdPedido", SqlDbType.Int) { Direction = ParameterDirection.Output };
                cmd.Parameters.Add(pId);

                cn.Open();
                cmd.ExecuteNonQuery();
                return (int)pId.Value;
            }
        }

        public void Actualizar(Pedido p)
        {
            using (var cn = new SqlConnection(ConexionBD.CadenaConexion))
            using (var cmd = new SqlCommand("dbo.usp_Pedidos_Actualizar", cn) { CommandType = CommandType.StoredProcedure })
            {
                CargarParametros(cmd, p, incluirId: true);
                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Eliminar(int idPedido)
        {
            using (var cn = new SqlConnection(ConexionBD.CadenaConexion))
            using (var cmd = new SqlCommand("dbo.usp_Pedidos_Eliminar", cn) { CommandType = CommandType.StoredProcedure })
            {
                cmd.Parameters.AddWithValue("@IdPedido", idPedido);
                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // ------------------------------------------------------------------
        private static void CargarParametros(SqlCommand cmd, Pedido p, bool incluirId)
        {
            if (incluirId) cmd.Parameters.AddWithValue("@IdPedido", p.IdPedido);
            cmd.Parameters.AddWithValue("@IdCliente", (object)p.IdCliente ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IdEmpleado", (object)p.IdEmpleado ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@FechaPedido", (object)p.FechaPedido ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@FechaEntrega", (object)p.FechaEntrega ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@FechaEnvio", (object)p.FechaEnvio ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Flete", p.Flete);
            cmd.Parameters.AddWithValue("@Destinatario", Val(p.Destinatario));
            cmd.Parameters.AddWithValue("@CiudadDestino", Val(p.CiudadDestino));
            cmd.Parameters.AddWithValue("@PaisDestino", Val(p.PaisDestino));
        }

        private static Pedido MapearCabecera(SqlDataReader r) => new Pedido
        {
            IdPedido = (int)r["IdPedido"],
            IdCliente = Nid(r, "IdCliente"),
            IdEmpleado = Nid(r, "IdEmpleado"),
            NombreCliente = Str(r, "NombreCliente"),
            NombreEmpleado = Str(r, "NombreEmpleado"),
            FechaPedido = Nfecha(r, "FechaPedido"),
            FechaEntrega = Nfecha(r, "FechaEntrega"),
            FechaEnvio = Nfecha(r, "FechaEnvio"),
            Flete = (decimal)r["Flete"],
            Destinatario = Str(r, "Destinatario"),
            CiudadDestino = Str(r, "CiudadDestino"),
            PaisDestino = Str(r, "PaisDestino"),
            TotalPedido = r["TotalPedido"] == DBNull.Value ? 0m : (decimal)r["TotalPedido"]
        };

        private static object Val(string s) => string.IsNullOrWhiteSpace(s) ? (object)DBNull.Value : s.Trim();
        private static int? Nid(SqlDataReader r, string col)
        {
            int i = r.GetOrdinal(col);
            return r.IsDBNull(i) ? (int?)null : r.GetInt32(i);
        }
        private static DateTime? Nfecha(SqlDataReader r, string col)
        {
            int i = r.GetOrdinal(col);
            return r.IsDBNull(i) ? (DateTime?)null : r.GetDateTime(i);
        }
        private static string Str(SqlDataReader r, string col)
        {
            int i = r.GetOrdinal(col);
            return r.IsDBNull(i) ? null : r.GetValue(i).ToString();
        }
    }
}
