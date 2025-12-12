# GitHub Actions - SonarCloud Integration

Este repositório está configurado para executar análise automática de código no SonarCloud usando GitHub Actions.

## ?? Configuração Inicial

### 1. Configurar o Token do SonarCloud

Para que a pipeline funcione, você precisa adicionar o token do SonarCloud como um secret no GitHub:

1. Acesse **SonarCloud** em https://sonarcloud.io/
2. Faça login com sua conta do GitHub
3. Vá para **Account Security**: https://sonarcloud.io/account/security/
4. Gere um novo token (ou use o existente: `8a5afa0b227123427cbeb29ce22814d7705a7606`)
5. Copie o token gerado

### 2. Adicionar Secret no GitHub

1. Acesse seu repositório no GitHub: https://github.com/landim32/MoutsTI-Desafio
2. Vá em **Settings** > **Secrets and variables** > **Actions**
3. Clique em **New repository secret**
4. Nome: `SONAR_TOKEN`
5. Value: Cole o token do SonarCloud
6. Clique em **Add secret**

### 3. Configurar o Projeto no SonarCloud

Se o projeto ainda não existe no SonarCloud:

1. Acesse https://sonarcloud.io/projects/create
2. Selecione **GitHub**
3. Escolha o repositório `landim32/MoutsTI-Desafio`
4. Configure as seguintes informações:
   - **Organization**: `landim32`
   - **Project Key**: `landim32_MoutsTI-Desafio`
   - **Display Name**: `MoutsTI-Desafio`

### 4. Verificar a Configuração

A pipeline está configurada para rodar automaticamente em:
- ? Push para as branches `main` e `develop`
- ? Pull Requests (opened, synchronize, reopened)

## ?? O que a Pipeline Faz

1. **Checkout do código** - Clona o repositório
2. **Setup .NET 8** - Configura o ambiente .NET
3. **Cache** - Otimiza builds subsequentes usando cache
4. **Instala ferramentas** - SonarScanner e ReportGenerator
5. **Restaura dependências** - `dotnet restore`
6. **Inicia análise SonarCloud** - Configura parâmetros de análise
7. **Build** - Compila a solução em Release
8. **Executa testes** - Roda testes com cobertura de código
9. **Finaliza análise** - Envia resultados para o SonarCloud
10. **Upload de artefatos** - Salva resultados de testes e cobertura

## ?? Visualizar Resultados

Após a execução da pipeline:

1. **GitHub Actions**: https://github.com/landim32/MoutsTI-Desafio/actions
2. **SonarCloud Dashboard**: https://sonarcloud.io/project/overview?id=landim32_MoutsTI-Desafio

## ?? Métricas Analisadas

O SonarCloud analisa:
- ? **Code Coverage** - Cobertura de testes
- ? **Bugs** - Erros no código
- ? **Vulnerabilities** - Problemas de segurança
- ? **Code Smells** - Problemas de qualidade
- ? **Duplications** - Código duplicado
- ? **Technical Debt** - Débito técnico

## ?? Exclusões Configuradas

A análise **exclui**:
- Projetos de teste (`**Tests/**`)
- Views Razor (`**/*.cshtml`)
- Arquivo `Program.cs`
- Migrações do EF Core (`**/Migrations/**`)
- Arquivos compilados (`**/obj/**`, `**/bin/**`)
- Assets estáticos (`**/*.js`, `**/*.css`)

## ??? Executar Análise Localmente

Para executar a análise localmente, use o script PowerShell:

```powershell
.\sonar-analysis.ps1
```

O script usa os parâmetros padrão já configurados no arquivo.

## ?? Personalização

Para alterar as configurações da pipeline, edite o arquivo:
```
.github/workflows/sonarcloud.yml
```

### Alterar branches monitoradas

```yaml
on:
  push:
    branches:
      - main
      - develop
      - feature/*  # Adicione suas branches aqui
```

### Alterar exclusões de cobertura

```yaml
/d:sonar.coverage.exclusions="**Tests/**,**/*.cshtml,**/Program.cs,**/Migrations/**,**/SuaPasta/**"
```

## ?? Documentação Adicional

- [SonarCloud Documentation](https://docs.sonarcloud.io/)
- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [.NET Testing with Coverage](https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-code-coverage)

## ?? Troubleshooting

### Pipeline falha com "Organization not found"
- Verifique se a organização `landim32` está correta no SonarCloud
- Confirme que o projeto foi criado no SonarCloud

### Cobertura não aparece no SonarCloud
- Verifique se os testes estão sendo executados corretamente
- Confirme que o formato OpenCover está sendo gerado
- Verifique os logs da pipeline na seção "Run tests with coverage"

### Token inválido
- Gere um novo token no SonarCloud
- Atualize o secret `SONAR_TOKEN` no GitHub

## ?? Suporte

Para questões relacionadas ao projeto, abra uma issue no GitHub:
https://github.com/landim32/MoutsTI-Desafio/issues
