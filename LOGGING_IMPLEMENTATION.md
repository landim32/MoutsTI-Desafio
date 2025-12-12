# Implementação de Logging com Serilog

## Visão Geral

Foi implementado um sistema completo de logging utilizando **Serilog**, uma das melhores bibliotecas de logging para .NET, que oferece logging estruturado, flexível e de alto desempenho.

## Pacotes Instalados

Os seguintes pacotes NuGet foram adicionados ao projeto `MoutsTI.API`:

- **Serilog.AspNetCore** (10.0.0) - Integração do Serilog com ASP.NET Core
- **Serilog.Sinks.File** (7.0.0) - Gravação de logs em arquivos
- **Serilog.Sinks.Console** - Saída de logs no console (incluído no AspNetCore)
- **Serilog.Enrichers.Environment** (3.0.1) - Enriquecimento com informações do ambiente (nome da máquina)
- **Serilog.Enrichers.Thread** (4.0.0) - Enriquecimento com informações de thread

## Configuração

### 1. Program.cs

O Serilog foi configurado no `Program.cs` com:

```csharp
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "MoutsTI.API")
    .Enrich.WithMachineName()
    .Enrich.WithThreadId()
    .WriteTo.Console(...)
    .WriteTo.File(...)
    .CreateLogger();
```

**Características:**
- **Níveis de Log**: Debug para aplicação, Information para Microsoft, Warning para AspNetCore
- **Enriquecimento**: Nome da aplicação, máquina e thread ID
- **Saídas**: Console (formatado) e Arquivo (rotação diária)

### 2. appsettings.Development.json

Configuração adicional no arquivo de settings:

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Debug",
      "Override": {
        "Microsoft": "Information",
        "Microsoft.AspNetCore": "Warning",
        "Microsoft.EntityFrameworkCore": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "Console",
        "Args": {
          "outputTemplate": "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}"
        }
      },
      {
        "Name": "File",
        "Args": {
          "path": "logs/moutsti-.log",
          "rollingInterval": "Day",
          "retainedFileCountLimit": 30
        }
      }
    ]
  }
}
```

## Funcionalidades Implementadas

### 1. Logging de Requisições HTTP

Middleware `UseSerilogRequestLogging` configurado para registrar:
- Método HTTP
- Caminho da requisição
- Status Code da resposta
- Tempo de execução
- Host, Scheme, IP remoto e User-Agent

**Exemplo de log:**
```
HTTP GET /api/employee/1 responded 200 in 45.2345 ms
```

### 2. Logging nos Services

#### AuthService
- ? Tentativas de autenticação (sucesso e falha)
- ? Geração de tokens JWT
- ? Hashing de senhas
- ? Verificação de senhas
- ? Erros e exceções

**Exemplos:**
```
[INFO] Authentication attempt for email: john.doe@example.com
[INFO] Authentication successful for employee: 1 - john.doe@example.com
[INFO] JWT token generated successfully for employee: 1 - Expires at: 2024-01-15 15:30:00
[WARN] Authentication failed: Invalid password for email: john.doe@example.com
```

#### EmployeeService
- ? Operações CRUD (Create, Read, Update, Delete)
- ? Validação de hierarquia de roles
- ? Hashing de senhas
- ? Erros de autorização e validação
- ? Erros e exceções

**Exemplos:**
```
[INFO] Adding new employee. Email: jane.smith@example.com, RoleId: 2, RequestedBy: 1
[INFO] Employee added successfully. EmployeeId: 5, Email: jane.smith@example.com
[WARN] Unauthorized attempt to add employee with RoleId: 3 by employee: 2
[INFO] Updating employee: 5, RequestedBy: 1
[INFO] Employee updated successfully: 5
```

#### EmployeeRoleService
- ? Listagem de roles
- ? Contagem de registros
- ? Erros e exceções

**Exemplos:**
```
[DEBUG] Listing all employee roles
[INFO] Retrieved 6 employee roles
```

### 3. Logging nos Controllers

#### AuthController
- ? Tentativas de login
- ? Login bem-sucedido/falhado
- ? Validação de token
- ? Erros de validação e internos

#### EmployeeController
- ? Todas as operações CRUD
- ? Erros de validação, autorização e não encontrado
- ? Extração de usuário atual do token

#### EmployeeRoleController
- ? Listagem de roles
- ? Erros internos

## Níveis de Log Utilizados

| Nível | Uso | Exemplos |
|-------|-----|----------|
| **Debug** | Informações detalhadas para desenvolvimento | Validações, hash de senha, busca de dados |
| **Information** | Eventos importantes da aplicação | Login bem-sucedido, operações CRUD, início/fim da aplicação |
| **Warning** | Situações inesperadas mas tratáveis | Autenticação falha, erros de validação, acesso não autorizado |
| **Error** | Erros que requerem atenção | Exceções não tratadas, falhas de operação |
| **Fatal** | Falhas críticas que terminam a aplicação | Erro no startup da aplicação |

## Saídas de Log

### 1. Console
- **Formato**: Timestamp, Nível, SourceContext, Mensagem, Exceção
- **Uso**: Desenvolvimento e debugging
- **Template**: 
  ```
  [2024-01-15 14:30:45.123 +00:00] [INF] [MoutsTI.Domain.Services.AuthService] Authentication successful for employee: 1 - john.doe@example.com
  ```

### 2. Arquivo
- **Local**: `logs/moutsti-YYYYMMDD.log`
- **Rotação**: Diária
- **Retenção**: 30 dias
- **Formato**: Timestamp, Nível, SourceContext, ThreadId, Mensagem, Exceção
- **Compartilhado**: Sim (permite múltiplos processos)

## Boas Práticas Implementadas

? **Logging Estruturado**: Uso de templates com placeholders para facilitar queries  
? **Níveis Apropriados**: Diferentes níveis para diferentes situações  
? **Contexto Rico**: Enriquecimento com informações de máquina, thread e aplicação  
? **Informações Sensíveis**: Senhas nunca são logadas (apenas mencionado "hashing")  
? **Performance**: Logging assíncrono e compartilhado  
? **Rotação de Logs**: Arquivos diários com retenção de 30 dias  
? **Filtros**: Redução de ruído de frameworks (Microsoft, System)  
? **Dependency Injection**: ILogger injetado via DI  
? **Testes**: Mocks de ILogger nos testes unitários  

## Exemplo de Uso

### Em um Service
```csharp
private readonly ILogger<EmployeeService> _logger;

public EmployeeService(ILogger<EmployeeService> logger)
{
    _logger = logger;
}

public void Add(EmployeeDto employee)
{
    _logger.LogInformation("Adding new employee. Email: {Email}", employee.Email);
    
    try
    {
        // lógica...
        _logger.LogInformation("Employee added successfully. EmployeeId: {EmployeeId}", employeeId);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error adding employee. Email: {Email}", employee.Email);
        throw;
    }
}
```

### Em um Controller
```csharp
private readonly ILogger<EmployeeController> _logger;

[HttpPost]
public IActionResult Create([FromBody] EmployeeDto employee)
{
    try
    {
        _logger.LogDebug("Creating employee with email: {Email}", employee.Email);
        // lógica...
        return Ok();
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error creating employee");
        return StatusCode(500);
    }
}
```

## Monitoramento

### Verificação de Logs em Tempo Real

**Console (Desenvolvimento):**
```bash
# Os logs aparecem automaticamente no console durante a execução
dotnet run
```

**Arquivo (Produção):**
```bash
# Linux/Mac
tail -f logs/moutsti-20240115.log

# Windows PowerShell
Get-Content logs\moutsti-20240115.log -Wait -Tail 50
```

### Análise de Logs

Os logs podem ser analisados com ferramentas como:
- **Seq** (recomendado para Serilog) - Interface web para logs estruturados
- **ELK Stack** (Elasticsearch, Logstash, Kibana)
- **Splunk**
- **Azure Application Insights**
- **AWS CloudWatch**

## Próximos Passos (Opcional)

Para ambientes de produção, considere:

1. **Adicionar Seq**: 
   ```bash
   dotnet add package Serilog.Sinks.Seq
   ```

2. **Configurar Application Insights**:
   ```bash
   dotnet add package Serilog.Sinks.ApplicationInsights
   ```

3. **Adicionar Alertas**: Configurar alertas para logs de Warning/Error/Fatal

4. **Métricas**: Integrar com métricas de performance (Prometheus, Application Insights)

5. **Correlação de Logs**: Adicionar CorrelationId para rastreamento de requisições

## Arquivos Modificados

- ? `MoutsTI.API/Program.cs` - Configuração do Serilog
- ? `MoutsTI.API/appsettings.Development.json` - Settings do Serilog
- ? `MoutsTI.Domain/Services/AuthService.cs` - Logging adicionado
- ? `MoutsTI.Domain/Services/EmployeeService.cs` - Logging adicionado
- ? `MoutsTI.Domain/Services/EmployeeRoleService.cs` - Logging adicionado
- ? `MoutsTI.API/Controllers/AuthController.cs` - Logging adicionado
- ? `MoutsTI.API/Controllers/EmployeeController.cs` - Já tinha logging
- ? `MoutsTI.API/Controllers/EmployeeRoleController.cs` - Já tinha logging
- ? `MoutsTI.Tests/Domain/Services/AuthServiceTests.cs` - Mock de ILogger
- ? `MoutsTI.Tests/Domain/Services/EmployeeServiceTests.cs` - Mock de ILogger
- ? `MoutsTI.Tests/Domain/Services/EmployeeRoleServiceTests.cs` - Mock de ILogger
- ? `MoutsTI.API/.gitignore` - Ignorar pasta de logs

## Conclusão

O sistema de logging está completamente implementado e pronto para uso. Ele oferece:

? **Visibilidade completa** de todas as operações da aplicação  
? **Rastreamento de autenticação** e autorização  
? **Detecção de erros** e problemas de performance  
? **Auditoria** de operações CRUD  
? **Debugging facilitado** com informações contextuais ricas  
? **Produção-ready** com rotação e retenção de logs  

O logging está agora pronto para auxiliar no desenvolvimento, debugging e monitoramento da aplicação MoutsTI!
