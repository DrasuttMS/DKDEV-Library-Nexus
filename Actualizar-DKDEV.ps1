$ErrorActionPreference = "Stop"

$ProjectRoot = $PSScriptRoot
$PayloadRoot = Join-Path $ProjectRoot "Actualizacion"
$BackupRoot = Join-Path $ProjectRoot ("Respaldos\AntesActualizacion_" + (Get-Date -Format "yyyyMMdd_HHmmss"))

Write-Host "`nDKDEV Library Nexus - Actualizador" -ForegroundColor Cyan
Write-Host "Proyecto: $ProjectRoot"

$files = @(
    "BlazorBiblioteca\BlazorBiblioteca\Controller\LibroController.cs",
    "BlazorBiblioteca\BlazorBiblioteca.Client\Pages\Home.razor",
    "BlazorBiblioteca\BlazorBiblioteca\Components\App.razor",
    "BlazorBiblioteca\BlazorBiblioteca\wwwroot\app.css"
)

foreach ($relative in $files) {
    $source = Join-Path $PayloadRoot $relative
    $destination = Join-Path $ProjectRoot $relative

    if (-not (Test-Path $source)) {
        throw "No se encuentra el archivo de actualización: $source"
    }

    if (Test-Path $destination) {
        $backup = Join-Path $BackupRoot $relative
        New-Item -ItemType Directory -Force -Path (Split-Path $backup) | Out-Null
        Copy-Item $destination $backup -Force
    }

    New-Item -ItemType Directory -Force -Path (Split-Path $destination) | Out-Null
    Copy-Item $source $destination -Force
    Write-Host "Actualizado: $relative" -ForegroundColor Green
}

$serverProject = Join-Path $ProjectRoot "BlazorBiblioteca\BlazorBiblioteca\BlazorBiblioteca.csproj"
$publishFolder = Join-Path $ProjectRoot "publish"

Write-Host "`nRestaurando y compilando..." -ForegroundColor Cyan
dotnet restore $serverProject
if ($LASTEXITCODE -ne 0) { throw "Falló dotnet restore." }

dotnet build $serverProject --no-restore
if ($LASTEXITCODE -ne 0) { throw "Falló la compilación. Revisa los errores anteriores." }

Write-Host "`nPublicando versión Release..." -ForegroundColor Cyan
dotnet publish $serverProject -c Release -o $publishFolder
if ($LASTEXITCODE -ne 0) { throw "Falló la publicación." }

Write-Host "`nActualización completada correctamente." -ForegroundColor Green
Write-Host "Respaldo anterior: $BackupRoot"
Write-Host "Para iniciar, ejecuta: .\Iniciar-DKDEV.ps1"
