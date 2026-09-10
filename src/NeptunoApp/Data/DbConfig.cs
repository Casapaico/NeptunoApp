namespace NeptunoApp.Data;

/// <summary>
/// Cadena de conexion a SQL Server. La base corre en la laptop y la app WPF
/// se ejecuta en la PC Windows; ambas se conectan por la red WiFi "iPhone de Alumno".
///
///   Red:      iPhone de Alumno  (rango 172.20.10.0/28)
///   Laptop:   172.20.10.4 , puerto 1433
///   Usuario:  sa
///
/// Si la IP de la laptop cambia, verificarla con "hostname -I" en Ubuntu
/// y actualizar el valor de Server= aqui.
/// </summary>
public static class DbConfig
{
    public const string ConnectionString =
        @"Server=172.20.10.4,1433;Database=NeptunoDB;User Id=sa;Password=Meolvide2.0;TrustServerCertificate=True;Encrypt=True;Connect Timeout=15;";
}
