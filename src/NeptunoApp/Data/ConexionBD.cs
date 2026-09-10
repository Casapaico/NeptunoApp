using System.Configuration;

namespace NeptunoApp.Data
{
    /// <summary>Provee la cadena de conexion definida en App.config.</summary>
    public static class ConexionBD
    {
        public static string CadenaConexion =>
            ConfigurationManager.ConnectionStrings["NeptunoDB"].ConnectionString;
    }
}
