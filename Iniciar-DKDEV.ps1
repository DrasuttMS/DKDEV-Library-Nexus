$ErrorActionPreference = "Stop"
$ProjectRoot = $PSScriptRoot
$PublishFolder = Join-Path $ProjectRoot "publish"
$Dll = Join-Path $PublishFolder "BlazorBiblioteca.dll"

if (-not (Test-Path $Dll)) {
    Write-Host "No existe la publicación. Ejecuta primero .\Actualizar-DKDEV.ps1" -ForegroundColor Yellow
    exit 1
}

$port = 5157
$listener = Get-NetTCPConnection -LocalPort $port -State Listen -ErrorAction SilentlyContinue
if ($listener) {
    Write-Host "El puerto $port ya está ocupado. Cierra la instancia anterior o utiliza otro puerto." -ForegroundColor Yellow
    $listener | Select-Object LocalAddress, LocalPort, OwningProcess | Format-Table
    exit 1
}

Set-Location $PublishFolder
Write-Host "Iniciando DKDEV Library Nexus en http://localhost:$port" -ForegroundColor Cyan
Write-Host "No cierres esta ventana mientras uses la aplicación." -ForegroundColor Yellow
dotnet $Dll --urls "http://localhost:$port"
