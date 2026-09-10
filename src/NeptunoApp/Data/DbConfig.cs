namespace NeptunoApp.Data;

/// <summary>
/// Cadena de conexion a SQL Server.
///
/// El servidor de base de datos y la aplicacion pueden estar en equipos
/// distintos de la misma red local. Ajustar el valor de Server= con la IP
/// (o nombre) del servidor SQL:
///
///   - misma maquina:    Server=localhost   (o  .\SQLEXPRESS)
///   - equipo en la LAN:  Server=192.168.x.x,1433
///
/// El servidor debe aceptar conexiones TCP en el puerto 1433 y el firewall
/// debe permitirlas.
/// </summary>
public static class DbConfig
{
    public const string ConnectionString =
        @"Server=172.20.10.4,1433;Database=NeptunoDB;User Id=sa;Password=Meolvide2.0;TrustServerCertificate=True;Encrypt=True;Connect Timeout=15;";
}
