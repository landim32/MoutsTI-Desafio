# Exemplo de uso do script sonar-analysis.ps1
# IMPORTANTE: NÃO COMMITE ESTE ARQUIVO COM SUAS CREDENCIAIS REAIS!

# ========================================
# SonarCloud (Recomendado)
# ========================================

# Execute com suas credenciais do SonarCloud:
.\sonar-analysis.ps1 `
    -SonarUrl "https://sonarcloud.io" `
    -SonarOrganization "landim32" `
    -SonarToken "SEU_TOKEN_AQUI" `
    -ProjectKey "landim32_MoutsTI-Desafio"

# ========================================
# SonarQube Local (Docker)
# ========================================

# Para rodar o SonarQube localmente com Docker:
# docker run -d --name sonarqube -p 9000:9000 sonarqube:latest

# Depois execute:
# .\sonar-analysis.ps1 -SonarUrl "http://localhost:9000"

# ========================================
# Onde encontrar as informações
# ========================================

# Organization Key:
# https://sonarcloud.io/ > My Organizations > [Sua Org] > Key

# Token:
# https://sonarcloud.io/account/security/ > Generate Token

# Project Key:
# Formato: organization_repository
# Exemplo: landim32_MoutsTI-Desafio
