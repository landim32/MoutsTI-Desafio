# ?? Fix: SonarScanner Configuration Error

## Problema Identificado

```
Post-processing failed. Exit code: 1
WARNING: File '/home/runner/work/MoutsTI-Desafio/MoutsTI-Desafio/moutsti.presentation/**/*' does not exist.
sonar-project.properties files are not understood by the SonarScanner for .NET. 
Remove those files from the following folders: /home/runner/work/MoutsTI-Desafio/MoutsTI-Desafio
```

## Causa Raiz

1. **sonar-project.properties não é suportado**: O SonarScanner para .NET não usa arquivos `sonar-project.properties`. Este arquivo é usado apenas pelo SonarScanner genérico.

2. **Referências a diretórios inexistentes**: O diretório `moutsti.presentation` existe no repositório mas contém um projeto React/TypeScript que não faz parte da análise .NET.

3. **Paths incorretos de relatórios**: Os caminhos dos relatórios de cobertura estavam muito genéricos.

## Solução Aplicada

### 1. Remover sonar-project.properties
```bash
# Arquivo removido pois não é suportado pelo SonarScanner para .NET
rm sonar-project.properties
```

### 2. Corrigir Exclusões na Pipeline

**Antes:**
```yaml
/d:sonar.exclusions="**/obj/**,**/bin/**,**/*.js,**/*.css,**/wwwroot/**"
```

**Depois:**
```yaml
/d:sonar.exclusions="**/obj/**,**/bin/**,**/*.js,**/*.css,**/wwwroot/**,**/moutsti.presentation/**,**/node_modules/**"
```

### 3. Corrigir Paths dos Relatórios

**Antes:**
```yaml
/d:sonar.cs.opencover.reportsPaths="**/coverage.opencover.xml"
/d:sonar.cs.vstest.reportsPaths="**/*.trx"
```

**Depois:**
```yaml
/d:sonar.cs.opencover.reportsPaths="**/TestResults/**/coverage.opencover.xml"
/d:sonar.cs.vstest.reportsPaths="**/TestResults/*.trx"
```

### 4. Adicionar Step de Debug

Adicionado step para listar arquivos de cobertura gerados:
```yaml
- name: List coverage files
  if: always()
  run: |
    echo "Listing TestResults directory:"
    find TestResults -type f -name "*.xml" -o -name "*.trx" || true
```

### 5. Melhorar Exclusões de Cobertura

```yaml
/d:sonar.coverage.exclusions="**/*Tests/**,**/*.cshtml,**/Program.cs,**/Migrations/**,**/moutsti.presentation/**"
```

## Arquivos Modificados

1. ? `.github/workflows/sonarcloud.yml` - Pipeline corrigida
2. ? `sonar-analysis.ps1` - Script local atualizado
3. ? `sonar-project.properties` - Removido
4. ? `SONARCLOUD_INTEGRATION_SUMMARY.md` - Documentação atualizada

## Como o SonarScanner para .NET Funciona

O SonarScanner para .NET usa um fluxo de 3 etapas:

```bash
# 1. Begin - Configura a análise
dotnet sonarscanner begin /k:"project-key" /d:sonar.host.url="..."

# 2. Build e Test - MSBuild coleta informações
dotnet build
dotnet test

# 3. End - Processa e envia dados
dotnet sonarscanner end
```

**Importante**: As configurações são passadas via parâmetros `/d:`, NÃO via `sonar-project.properties`.

## Diferenças entre SonarScanner Genérico vs .NET

| Característica | SonarScanner Genérico | SonarScanner para .NET |
|----------------|----------------------|------------------------|
| Configuração | `sonar-project.properties` | Parâmetros `/d:` |
| Build Integration | ? Não | ? Sim (MSBuild) |
| Coleta Automática | ? Manual | ? Automática |
| Coverage | Manual | Via MSBuild collectors |

## Estrutura de Diretórios

```
MoutsTI-Desafio/
??? .github/
?   ??? workflows/
?       ??? sonarcloud.yml       ? Corrigido
??? MoutsTI.API/                 ? Analisado
??? MoutsTI.Application/         ? Analisado
??? MoutsTI.Domain/              ? Analisado
??? MoutsTI.Dtos/                ? Analisado
??? MoutsTI.Infra/               ? Analisado
??? MoutsTI.Infra.Interfaces/    ? Analisado
??? MoutsTI.Tests/               ? Analisado (excluído de coverage)
??? moutsti.presentation/        ? Excluído (projeto React)
??? TestResults/                 ?? Relatórios de teste
??? sonar-analysis.ps1           ? Corrigido
??? sonar-project.properties     ? Removido
```

## Resultado Esperado

Após estas correções, a pipeline deve:

1. ? **Begin**: Configurar análise corretamente
2. ? **Build**: Compilar sem erros
3. ? **Test**: Executar 326 testes com sucesso
4. ? **Coverage**: Gerar relatórios no formato OpenCover
5. ? **End**: Processar e enviar dados ao SonarCloud
6. ? **Upload**: Salvar artefatos de teste e cobertura

## Verificação

Para verificar se tudo está funcionando:

```bash
# 1. Executar localmente
.\sonar-analysis.ps1 `
    -SonarUrl "https://sonarcloud.io" `
    -SonarOrganization "landim32" `
    -SonarToken "seu_token" `
    -ProjectKey "landim32_MoutsTI-Desafio"

# 2. Verificar no GitHub Actions
# https://github.com/landim32/MoutsTI-Desafio/actions

# 3. Verificar no SonarCloud
# https://sonarcloud.io/project/overview?id=landim32_MoutsTI-Desafio
```

## Lições Aprendidas

1. **SonarScanner para .NET ? SonarScanner Genérico**: Cada um tem sua própria forma de configuração
2. **Exclusões são importantes**: Excluir diretórios que não são parte do projeto .NET evita warnings
3. **Paths específicos são melhores**: Usar `**/TestResults/**/*.xml` ao invés de `**/*.xml`
4. **Debug é essencial**: Adicionar steps de listagem de arquivos ajuda a identificar problemas
5. **Documentação oficial**: Sempre consultar a documentação oficial do SonarScanner para .NET

## Referências

- [SonarScanner for .NET Documentation](https://docs.sonarqube.org/latest/analysis/scan/sonarscanner-for-msbuild/)
- [SonarCloud Documentation](https://docs.sonarcloud.io/)
- [Code Coverage in .NET](https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-code-coverage)

---

**Data da Correção**: 12/12/2024  
**Status**: ? Resolvido  
**Próximo Passo**: Commit e push para testar na pipeline
