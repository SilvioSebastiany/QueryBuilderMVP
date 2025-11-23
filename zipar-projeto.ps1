# Script para zipar o projeto QueryBuilder limpo (sem bin/obj/.vs)
# Data: 22/11/2025

Write-Host "🗜️  Preparando projeto para ZIP..." -ForegroundColor Cyan

# Diretório do projeto
$projectRoot = $PSScriptRoot
$projectName = "QueryBuilderMVP"
$zipFileName = "$projectName-$(Get-Date -Format 'yyyy-MM-dd').zip"
$zipPath = Join-Path (Split-Path $projectRoot -Parent) $zipFileName

# Pastas/arquivos a EXCLUIR do ZIP
$excludePatterns = @(
    "*/bin/*",
    "*/obj/*",
    "*/.vs/*",
    "*/.vscode/settings.json",
    "*/.vscode/extensions.json",
    "*/node_modules/*",
    "*/.git/*",
    "*.user",
    "*.suo",
    "*.cache",
    "*.log"
)

Write-Host "📁 Projeto: $projectRoot" -ForegroundColor Yellow
Write-Host "📦 Destino: $zipPath" -ForegroundColor Yellow
Write-Host ""

# Limpar build artifacts antes de zipar
Write-Host "🧹 Limpando builds anteriores..." -ForegroundColor Cyan
dotnet clean --nologo --verbosity quiet

# Criar ZIP excluindo pastas desnecessárias
Write-Host "🗜️  Comprimindo projeto..." -ForegroundColor Cyan

# Opção 1: Usando Compress-Archive (mais simples, mas sem exclusões avançadas)
# Vamos criar uma pasta temp com apenas o que queremos

$tempFolder = Join-Path $env:TEMP "QueryBuilder-Temp"
if (Test-Path $tempFolder) {
    Remove-Item $tempFolder -Recurse -Force
}
New-Item -ItemType Directory -Path $tempFolder | Out-Null

# Copiar tudo
Copy-Item -Path "$projectRoot\*" -Destination $tempFolder -Recurse -Force

# Remover o que não queremos
$foldersToRemove = @("bin", "obj", ".vs", "node_modules")
foreach ($folder in $foldersToRemove) {
    Get-ChildItem -Path $tempFolder -Recurse -Directory -Filter $folder -Force | Remove-Item -Recurse -Force -ErrorAction SilentlyContinue
}

# Remover .git se existir (opcional - comente se quiser manter)
Remove-Item -Path "$tempFolder\.git" -Recurse -Force -ErrorAction SilentlyContinue

# Remover arquivos específicos
Get-ChildItem -Path $tempFolder -Recurse -File -Include "*.user", "*.suo", "*.cache" -Force | Remove-Item -Force -ErrorAction SilentlyContinue

# Criar ZIP
Compress-Archive -Path "$tempFolder\*" -DestinationPath $zipPath -Force

# Limpar pasta temp
Remove-Item $tempFolder -Recurse -Force

# Verificar tamanho
$zipSize = (Get-Item $zipPath).Length / 1MB
Write-Host ""
Write-Host "✅ Projeto zipado com sucesso!" -ForegroundColor Green
Write-Host "📦 Arquivo: $zipPath" -ForegroundColor Green
Write-Host "📊 Tamanho: $($zipSize.ToString('0.00')) MB" -ForegroundColor Green
Write-Host ""

if ($zipSize -gt 25) {
    Write-Host "⚠️  AVISO: Arquivo maior que 25 MB!" -ForegroundColor Yellow
    Write-Host "   Muitos serviços de email limitam anexos a 25 MB." -ForegroundColor Yellow
    Write-Host "   Considere usar Google Drive, OneDrive ou WeTransfer." -ForegroundColor Yellow
} else {
    Write-Host "✅ Tamanho OK para envio por email (< 25 MB)" -ForegroundColor Green
}

Write-Host ""
Write-Host "Localizacao do arquivo:" -ForegroundColor Cyan
Write-Host "   $zipPath" -ForegroundColor White
Write-Host ""

# Abrir pasta do arquivo
explorer.exe (Split-Path $zipPath -Parent)
