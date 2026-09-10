<#
    abrir.ps1  -  Compila y ejecuta NeptunoApp en Windows.

    Uso (PowerShell, desde la carpeta del proyecto):
        .\abrir.ps1

    Si PowerShell bloquea el script:
        powershell -ExecutionPolicy Bypass -File .\abrir.ps1
#>

$ErrorActionPreference = 'Stop'
$raiz = $PSScriptRoot
$csproj = Join-Path $raiz 'src\NeptunoApp\NeptunoApp.csproj'
$ipServidor = '172.20.10.4'
$puerto = 1433

Write-Host '== NeptunoApp ==' -ForegroundColor Cyan

# 1. Verificar .NET SDK
try {
    $sdk = (dotnet --version)
    Write-Host "SDK .NET detectado: $sdk"
} catch {
    Write-Host 'ERROR: no se encontro el SDK de .NET.' -ForegroundColor Red
    Write-Host 'Instala .NET 8 SDK (o abre src\NeptunoApp.sln en Visual Studio 2022).'
    exit 1
}

# 2. Probar conexion de red con la laptop (SQL Server)
Write-Host "`nProbando conexion con SQL Server en $ipServidor`:$puerto ..." -ForegroundColor Cyan
$test = Test-NetConnection -ComputerName $ipServidor -Port $puerto -WarningAction SilentlyContinue
if ($test.TcpTestSucceeded) {
    Write-Host 'Conexion de red OK.' -ForegroundColor Green
} else {
    Write-Host 'AVISO: no se pudo conectar al puerto 1433 de la laptop.' -ForegroundColor Yellow
    Write-Host ' - Verifica que ambas PC esten en la WiFi "iPhone de Alumno".'
    Write-Host " - Confirma la IP de la laptop (hostname -I) y ajustala en src\NeptunoApp\App.config."
    Write-Host ' - Continuo de todos modos...'
}

# 3. Compilar y ejecutar
Write-Host "`nCompilando y ejecutando..." -ForegroundColor Cyan
dotnet run --project $csproj -c Debug
