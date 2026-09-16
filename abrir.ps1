<#
    abrir.ps1  -  Compila y ejecuta NeptunoApp en Windows.

    Uso (PowerShell, desde la carpeta del proyecto):
        powershell -ExecutionPolicy Bypass -File .\abrir.ps1

    Requisitos: .NET 10 SDK (o abrir src\NeptunoApp.slnx en Visual Studio 2022 y pulsar F5).
#>

$ErrorActionPreference = 'Stop'
$raiz = $PSScriptRoot
$csproj = Join-Path $raiz 'src\NeptunoApp\NeptunoApp.csproj'
$ipServidor = '10.200.171.203'
$puerto = 1433

Write-Host '== NeptunoApp ==' -ForegroundColor Cyan

# 1. Verificar .NET SDK
try {
    $sdk = (dotnet --version)
    Write-Host "SDK .NET detectado: $sdk"
} catch {
    Write-Host 'ERROR: no se encontro el SDK de .NET. Instala .NET 10 SDK o abre src\NeptunoApp.slnx en Visual Studio 2022.' -ForegroundColor Red
    exit 1
}

# 2. Probar la conexion de red con el servidor SQL
Write-Host "`nProbando conexion con SQL Server en $ipServidor`:$puerto ..." -ForegroundColor Cyan
$test = Test-NetConnection -ComputerName $ipServidor -Port $puerto -WarningAction SilentlyContinue
if ($test.TcpTestSucceeded) {
    Write-Host 'Conexion de red OK.' -ForegroundColor Green
} else {
    Write-Host 'AVISO: no se pudo conectar al puerto 1433 del servidor.' -ForegroundColor Yellow
    Write-Host ' - Ambos equipos deben estar en la misma red local.'
    Write-Host ' - Verifica la IP del servidor SQL y ajustala en src\NeptunoApp\Data\DbConfig.cs.'
    Write-Host ' - Continuo de todos modos...'
}

# 3. Compilar y ejecutar
Write-Host "`nCompilando y ejecutando..." -ForegroundColor Cyan
dotnet run --project $csproj -c Debug
