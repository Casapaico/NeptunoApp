using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using NeptunoApp.Models;

namespace NeptunoApp.Data
{
    /// <summary>
    /// Acceso a datos de Proveedores mediante PROCEDIMIENTOS ALMACENADOS.
    /// El listado y la busqueda usan modo CONECTADO (SqlDataReader fila por fila).
    /// </summary>
    public class ProveedorRepository
    {
        public List<Proveedor> Listar()
        {
            using (var cn = new SqlConnection(ConexionBD.CadenaConexion))
            using (var cmd = new SqlCommand("dbo.usp_Proveedores_Listar", cn) { CommandType = CommandType.StoredProcedure })
            {
                cn.Open();
                using (var reader = cmd.ExecuteReader())
                    return LeerLista(reader);
            }
        }

        /// <summary>
        /// Listado de proveedores buscando por nombreContacto y ciudad
        /// (ambos filtros opcionales). Ejecuta usp_Proveedores_Buscar.
        /// </summary>
        public List<Proveedor> Buscar(string nombreContacto, string ciudad)
        {
            using (var cn = new SqlConnection(ConexionBD.CadenaConexion))
            using (var cmd = new SqlCommand("dbo.usp_Proveedores_Buscar", cn) { CommandType = CommandType.StoredProcedure })
            {
                cmd.Parameters.AddWithValue("@NombreContacto", (object)NuloSiVacio(nombreContacto) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Ciudad", (object)NuloSiVacio(ciudad) ?? DBNull.Value);

                cn.Open();
                using (var reader = cmd.ExecuteReader())
                    return LeerLista(reader);
            }
        }

        public int Insertar(Proveedor p)
        {
            using (var cn = new SqlConnection(ConexionBD.CadenaConexion))
            using (var cmd = new SqlCommand("dbo.usp_Proveedores_Insertar", cn) { CommandType = CommandType.StoredProcedure })
            {
                CargarParametros(cmd, p, incluirId: false);
                var pId = new SqlParameter("@IdProveedor", SqlDbType.Int) { Direction = ParameterDirection.Output };
                cmd.Parameters.Add(pId);

                cn.Open();
                cmd.ExecuteNonQuery();
                return (int)pId.Value;
            }
        }

        public void Actualizar(Proveedor p)
        {
            using (var cn = new SqlConnection(ConexionBD.CadenaConexion))
            using (var cmd = new SqlCommand("dbo.usp_Proveedores_Actualizar", cn) { CommandType = CommandType.StoredProcedure })
            {
                CargarParametros(cmd, p, incluirId: true);
                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Eliminar(int idProveedor)
        {
            using (var cn = new SqlConnection(ConexionBD.CadenaConexion))
            using (var cmd = new SqlCommand("dbo.usp_Proveedores_Eliminar", cn) { CommandType = CommandType.StoredProcedure })
            {
                cmd.Parameters.AddWithValue("@IdProveedor", idProveedor);
                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // ------------------------------------------------------------------
        private static void CargarParametros(SqlCommand cmd, Proveedor p, bool incluirId)
        {
            if (incluirId) cmd.Parameters.AddWithValue("@IdProveedor", p.IdProveedor);
            cmd.Parameters.AddWithValue("@NombreCompania", p.NombreCompania);
            cmd.Parameters.AddWithValue("@NombreContacto", Val(p.NombreContacto));
            cmd.Parameters.AddWithValue("@CargoContacto", Val(p.CargoContacto));
            cmd.Parameters.AddWithValue("@Direccion", Val(p.Direccion));
            cmd.Parameters.AddWithValue("@Ciudad", Val(p.Ciudad));
            cmd.Parameters.AddWithValue("@Region", Val(p.Region));
            cmd.Parameters.AddWithValue("@CodPostal", Val(p.CodPostal));
            cmd.Parameters.AddWithValue("@Pais", Val(p.Pais));
            cmd.Parameters.AddWithValue("@Telefono", Val(p.Telefono));
            cmd.Parameters.AddWithValue("@Fax", Val(p.Fax));
        }

        private static List<Proveedor> LeerLista(SqlDataReader reader)
        {
            var lista = new List<Proveedor>();
            while (reader.Read())
            {
                lista.Add(new Proveedor
                {
                    IdProveedor = reader.GetInt32(reader.GetOrdinal("IdProveedor")),
                    NombreCompania = Str(reader, "NombreCompania"),
                    NombreContacto = Str(reader, "NombreContacto"),
                    CargoContacto = Str(reader, "CargoContacto"),
                    Direccion = Str(reader, "Direccion"),
                    Ciudad = Str(reader, "Ciudad"),
                    Region = Str(reader, "Region"),
                    CodPostal = Str(reader, "CodPostal"),
                    Pais = Str(reader, "Pais"),
                    Telefono = Str(reader, "Telefono"),
                    Fax = Str(reader, "Fax")
                });
            }
            return lista;
        }

        private static object Val(string s) => (object)NuloSiVacio(s) ?? DBNull.Value;
        private static string NuloSiVacio(string s) => string.IsNullOrWhiteSpace(s) ? null : s.Trim();
        private static string Str(SqlDataReader r, string col)
        {
            int i = r.GetOrdinal(col);
            return r.IsDBNull(i) ? null : r.GetString(i);
        }
    }
}
