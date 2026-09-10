using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using NeptunoApp.Models;

namespace NeptunoApp.Data
{
    /// <summary>
    /// Consultas de apoyo para poblar ComboBox (categorias, proveedores,
    /// clientes y empleados). Usa SqlDataReader en modo CONECTADO.
    /// </summary>
    public class CatalogoRepository
    {
        public List<ItemCombo> Categorias() =>
            LeerCombo("SELECT IdCategoria, NombreCategoria FROM dbo.Categorias ORDER BY NombreCategoria");

        public List<ItemCombo> Proveedores() =>
            LeerCombo("SELECT IdProveedor, NombreCompania FROM dbo.Proveedores ORDER BY NombreCompania");

        public List<ItemCombo> Clientes() =>
            LeerCombo("SELECT IdCliente, NombreCompania FROM dbo.Clientes ORDER BY NombreCompania");

        public List<ItemCombo> Empleados() =>
            LeerCombo("SELECT IdEmpleado, Nombre + ' ' + Apellidos FROM dbo.Empleados ORDER BY Apellidos");

        private static List<ItemCombo> LeerCombo(string sql)
        {
            var lista = new List<ItemCombo>();
            using (var cn = new SqlConnection(ConexionBD.CadenaConexion))
            using (var cmd = new SqlCommand(sql, cn))
            {
                cn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        lista.Add(new ItemCombo { Id = reader.GetInt32(0), Texto = reader.GetString(1) });
                }
            }
            return lista;
        }
    }
}
