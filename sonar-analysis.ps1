# Script para executar análise do SonarQube com cobertura de testes
# Requisitos:
# - dotnet-sonarscanner instalado globalmente
# - SonarQube server rodando (local ou remoto)

param(
    [string]$SonarUrl = "https://sonarcloud.io",
    [string]$SonarToken = "8a5afa0b227123427cbeb29ce22814d7705a7606",
    [string]$SonarOrganization = "landim32",
    [string]$ProjectKey = "MoutsTI-Desafio",
    [string]$ProjectName = "MoutsTI-Desafio",
    [string]$ProjectVersion = "1.0.0"
)

Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "SonarQube Analysis with Test Coverage" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

# Validação para SonarCloud
if ($SonarUrl -like "*sonarcloud.io*") {
    if (-not $SonarOrganization) {
        Write-Host "ERRO: Para usar o SonarCloud, você precisa especificar a organização!" -ForegroundColor Red
        Write-Host ""
        Write-Host "Para encontrar sua organização:" -ForegroundColor Yellow
        Write-Host "1. Acesse https://sonarcloud.io/" -ForegroundColor Yellow
        Write-Host "2. Clique no seu avatar > My Organizations" -ForegroundColor Yellow
        Write-Host "3. Use a CHAVE (key) da organização, não o nome completo" -ForegroundColor Yellow
        Write-Host "   Exemplo: 'minha-org' ao invés de 'Minha Organização Ltda'" -ForegroundColor Yellow
        Write-Host ""
        Write-Host "Uso correto:" -ForegroundColor Cyan
        Write-Host ".\sonar-analysis.ps1 -SonarUrl 'https://sonarcloud.io' -SonarOrganization 'sua-org-key' -SonarToken 'seu-token'" -ForegroundColor Cyan
        Write-Host ""
        exit 1
    }
    if (-not $SonarToken) {
        Write-Host "ERRO: Para usar o SonarCloud, você precisa especificar um token!" -ForegroundColor Red
        Write-Host ""
        Write-Host "Para gerar um token:" -ForegroundColor Yellow
        Write-Host "1. Acesse https://sonarcloud.io/account/security/" -ForegroundColor Yellow
        Write-Host "2. Gere um novo token" -ForegroundColor Yellow
        Write-Host "3. Use-o no parâmetro -SonarToken" -ForegroundColor Yellow
        Write-Host ""
        exit 1
    }
}

# Verificar se o SonarScanner está instalado
$sonarScannerInstalled = dotnet tool list --global | Select-String "dotnet-sonarscanner"
if (-not $sonarScannerInstalled) {
    Write-Host "SonarScanner não encontrado. Instalando..." -ForegroundColor Yellow
    dotnet tool install --global dotnet-sonarscanner
}

# Verificar se o ReportGenerator está instalado
$reportGeneratorInstalled = dotnet tool list --global | Select-String "dotnet-reportgenerator-globaltool"
if (-not $reportGeneratorInstalled) {
    Write-Host "ReportGenerator não encontrado. Instalando..." -ForegroundColor Yellow
    dotnet tool install --global dotnet-reportgenerator-globaltool
}

# Limpar builds anteriores
Write-Host "Limpando builds anteriores..." -ForegroundColor Yellow
dotnet clean --configuration Release
Remove-Item -Path ".\TestResults" -Recurse -ErrorAction SilentlyContinue
Remove-Item -Path ".\.sonarqube" -Recurse -ErrorAction SilentlyContinue
Remove-Item -Path ".\coverage" -Recurse -ErrorAction SilentlyContinue

# Iniciar análise do SonarQube
Write-Host ""
Write-Host "Iniciando análise do SonarQube..." -ForegroundColor Green
Write-Host "URL: $SonarUrl" -ForegroundColor Gray
Write-Host "Projeto: $ProjectKey" -ForegroundColor Gray

$sonarBeginArgs = @(
    "sonarscanner",
    "begin",
    "/k:`"$ProjectKey`"",
    "/n:`"$ProjectName`"",
    "/v:`"$ProjectVersion`"",
    "/d:sonar.host.url=`"$SonarUrl`""
)

if ($SonarToken) {
    Write-Host "Usando token de autenticação" -ForegroundColor Gray
    $sonarBeginArgs += "/d:sonar.token=`"$SonarToken`""
}

if ($SonarOrganization) {
    Write-Host "Organização: $SonarOrganization" -ForegroundColor Gray
    $sonarBeginArgs += "/o:`"$SonarOrganization`""
}

# Configurações de cobertura de código
$sonarBeginArgs += "/d:sonar.cs.opencover.reportsPaths=`"coverage/coverage.opencover.xml`""
$sonarBeginArgs += "/d:sonar.cs.vstest.reportsPaths=`"TestResults/*.trx`""
$sonarBeginArgs += "/d:sonar.coverage.exclusions=`"**Tests/**,**/*.cshtml,**/Program.cs,**/Migrations/**`""
$sonarBeginArgs += "/d:sonar.exclusions=`"**/obj/**,**/bin/**,**/*.js,**/*.css`""

Write-Host ""
Write-Host "Executando: dotnet $($sonarBeginArgs -join ' ')" -ForegroundColor Cyan
Write-Host ""

& dotnet $sonarBeginArgs

if ($LASTEXITCODE -ne 0) {
    Write-Host ""
    Write-Host "Erro ao iniciar análise do SonarQube" -ForegroundColor Red
    Write-Host ""
    Write-Host "Dicas de solução:" -ForegroundColor Yellow
    Write-Host "1. Verifique se o SonarQube está rodando em: $SonarUrl" -ForegroundColor Yellow
    Write-Host "2. Para SonarQube local:" -ForegroundColor Yellow
    Write-Host "   .\sonar-analysis.ps1 -SonarUrl 'http://localhost:9000'" -ForegroundColor Gray
    Write-Host "3. Para SonarCloud:" -ForegroundColor Yellow
    Write-Host "   .\sonar-analysis.ps1 -SonarUrl 'https://sonarcloud.io' -SonarOrganization 'sua-org-key' -SonarToken 'seu-token'" -ForegroundColor Gray
    Write-Host ""
    Write-Host "IMPORTANTE: Use a CHAVE da organização, não o nome completo!" -ForegroundColor Yellow
    Write-Host "Encontre em: https://sonarcloud.io/ > My Organizations" -ForegroundColor Yellow
    Write-Host ""
    exit 1
}

# Build da solução
Write-Host ""
Write-Host "Compilando solução..." -ForegroundColor Green
dotnet build --configuration Release --no-incremental

if ($LASTEXITCODE -ne 0) {
    Write-Host "Erro ao compilar solução" -ForegroundColor Red
    exit 1
}

# Executar testes com cobertura
Write-Host ""
Write-Host "Executando testes com cobertura..." -ForegroundColor Green
dotnet test `
    --configuration Release `
    --no-build `
    --logger "trx" `
    --collect:"XPlat Code Coverage" `
    --results-directory "./TestResults" `
    -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=opencover

if ($LASTEXITCODE -ne 0) {
    Write-Host "Aviso: Alguns testes falharam, mas continuando com a análise..." -ForegroundColor Yellow
}

# Consolidar relatórios de cobertura
Write-Host ""
Write-Host "Consolidando relatórios de cobertura..." -ForegroundColor Green

# Criar diretório de coverage
New-Item -ItemType Directory -Force -Path ".\coverage" | Out-Null

# Encontrar todos os arquivos de cobertura
$coverageFiles = Get-ChildItem -Path ".\TestResults" -Filter "coverage.opencover.xml" -Recurse
if ($coverageFiles.Count -eq 0) {
    $coverageFiles = Get-ChildItem -Path ".\TestResults" -Filter "*.xml" -Recurse | Where-Object { $_.Directory.Name -match "^[a-f0-9\-]{36}$" }
}

if ($coverageFiles.Count -eq 0) {
    Write-Host "Aviso: Nenhum arquivo de cobertura encontrado!" -ForegroundColor Yellow
    Write-Host "Arquivos no TestResults:" -ForegroundColor Yellow
    Get-ChildItem -Path ".\TestResults" -Recurse | ForEach-Object { Write-Host $_.FullName -ForegroundColor Gray }
} else {
    Write-Host "Encontrados $($coverageFiles.Count) arquivo(s) de cobertura" -ForegroundColor Green
    
    # Se há múltiplos arquivos, usar o ReportGenerator para consolidar
    if ($coverageFiles.Count -gt 1) {
        $coverageFilePaths = ($coverageFiles | ForEach-Object { $_.FullName }) -join ";"
        Write-Host "Consolidando múltiplos arquivos de cobertura..." -ForegroundColor Yellow
        
        reportgenerator `
            "-reports:$coverageFilePaths" `
            "-targetdir:.\coverage" `
            "-reporttypes:OpenCover"
    } else {
        # Copiar o único arquivo para o diretório de coverage
        Write-Host "Copiando arquivo de cobertura..." -ForegroundColor Yellow
        Copy-Item -Path $coverageFiles[0].FullName -Destination ".\coverage\coverage.opencover.xml"
    }
    
    if (Test-Path ".\coverage\coverage.opencover.xml") {
        Write-Host "Relatório de cobertura criado com sucesso!" -ForegroundColor Green
        $fileSize = (Get-Item ".\coverage\coverage.opencover.xml").Length
        Write-Host "Tamanho do arquivo: $fileSize bytes" -ForegroundColor Gray
    } else {
        Write-Host "Aviso: Arquivo de cobertura não foi criado corretamente" -ForegroundColor Yellow
    }
}

# Finalizar análise do SonarQube
Write-Host ""
Write-Host "Finalizando análise do SonarQube..." -ForegroundColor Green

$sonarEndArgs = @("sonarscanner", "end")
if ($SonarToken) {
    $sonarEndArgs += "/d:sonar.token=`"$SonarToken`""
}

& dotnet $sonarEndArgs

if ($LASTEXITCODE -ne 0) {
    Write-Host "Erro ao finalizar análise do SonarQube" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "Análise concluída com sucesso!" -ForegroundColor Green
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Acesse o SonarQube em: $SonarUrl" -ForegroundColor Yellow
Write-Host "Projeto: $ProjectKey" -ForegroundColor Yellow
if ($SonarOrganization) {
    Write-Host "Organização: $SonarOrganization" -ForegroundColor Yellow
}
Write-Host ""
