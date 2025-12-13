# MoutsTI - Sistema de Gerenciamento de Funcionários

![.NET](https://img.shields.io/badge/.NET-8.0-blue)
![React Version](https://img.shields.io/badge/React-18.2.0-61DAFB?logo=react)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=landim32_MoutsTI-Desafio&metric=alert_status)](https://sonarcloud.io/project/overview?id=landim32_MoutsTI-Desafio)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=landim32_MoutsTI-Desafio&metric=coverage)](https://sonarcloud.io/project/overview?id=landim32_MoutsTI-Desafio)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=landim32_MoutsTI-Desafio&metric=bugs)](https://sonarcloud.io/project/overview?id=landim32_MoutsTI-Desafio)
[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=landim32_MoutsTI-Desafio&metric=code_smells)](https://sonarcloud.io/project/overview?id=landim32_MoutsTI-Desafio)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=landim32_MoutsTI-Desafio&metric=vulnerabilities)](https://sonarcloud.io/project/overview?id=landim32_MoutsTI-Desafio)

## 🚀 Quick Start com Docker

### Pré-requisitos

- Docker
- Docker Compose

### Estrutura dos Serviços

- **api**: Aplicação .NET 8 Web API
- **frontend**: Aplicação React com Vite e shadcn/ui
- **postgres**: Banco de dados PostgreSQL 15

### Configuração Inicial

1. **Clone o repositório:**
```bash
git clone https://github.com/landim32/MoutsTI-Desafio.git
cd MoutsTI-Desafio
```

2. **Configure as variáveis de ambiente:**

Renomeie o arquivo `.env.example` para `.env`:

```bash
# No Windows (PowerShell)
Copy-Item .env.example .env 

# No Linux/Mac
cp .env.example .env
```

O arquivo `.env` já está configurado com as credenciais necessárias. **IMPORTANTE:** Mantenha a `JWT_SECRET_KEY` como está para que as senhas funcionem corretamente:

```env
JWT_SECRET_KEY=7K9mP2vN8xQ4wE6rT1yU3iO5pA7sD9fG2hJ4kL6zX8cV0bN3mQ5wE7rT9yU1iO3pA
```

3. **Inicie os containers:**
```bash
docker-compose up -d
```

### 🔐 Credenciais de Primeiro Acesso

Após iniciar os containers, acesse o sistema com as seguintes credenciais:

**Email:** `rodrigo@emagine.com.br`  
**Senha:** `teste123`

### Acesso aos Serviços

- **Frontend**: http://localhost:3000
- **API**: http://localhost:5000
- **Swagger**: http://localhost:5000/swagger
- **PostgreSQL**: localhost:5432

### Modo Desenvolvimento

Para executar com hot-reload no frontend:

```bash
docker-compose -f docker-compose.yml up -d
```

Acesse:
- **Frontend (dev)**: http://localhost:3000
- **API**: http://localhost:5000
- **PostgreSQL**: localhost:5432

### Comandos Úteis

#### Ver logs dos serviços
```bash
# Todos os serviços
docker-compose logs -f

# Apenas API
docker-compose logs -f api

# Apenas Frontend
docker-compose logs -f frontend

# Apenas PostgreSQL
docker-compose logs -f postgres
```

#### Parar os serviços
```bash
docker-compose down
```

#### Parar e remover volumes (ATENÇÃO: apaga o banco de dados)
```bash
docker-compose down -v
```

#### Reconstruir imagens
```bash
docker-compose build
```

#### Reconstruir e executar
```bash
docker-compose up -d --build
```

### Volumes e Network

- **postgres-data**: Persiste os dados do PostgreSQL
- **moutsti-network**: Rede interna para comunicação entre serviços

### Troubleshooting

#### Porta já em uso
Se alguma porta estiver em uso, edite os mapeamentos no `docker-compose.yml`.

#### Problemas de permissão no volume do PostgreSQL
```bash
docker-compose down -v
docker-compose up -d
```

#### Rebuild completo
```bash
docker-compose down -v
docker-compose build --no-cache
docker-compose up -d
```

---

## 📋 Sobre o Projeto

Sistema completo de gerenciamento de funcionários desenvolvido como solução para teste técnico. O projeto implementa um CRUD completo com autenticação, relacionamentos hierárquicos entre funcionários e gestores, validações robustas e interface moderna em React.

O sistema permite:
- Cadastro, edição, listagem e exclusão de funcionários
- Gerenciamento de cargos (Employee Roles)
- Hierarquia de gestores e subordinados
- Autenticação JWT
- Validação completa de dados (CPF, CNPJ, email, telefone)
- Interface responsiva e intuitiva

## 🎯 Objetivo do Teste Técnico

Desenvolver uma API RESTful em .NET 8 com as seguintes funcionalidades:
- CRUD completo de funcionários
- Sistema de autenticação
- Relacionamento entre funcionários (gestor/subordinado)
- Validações de negócio
- Testes unitários com alta cobertura
- Documentação clara
- Boas práticas de desenvolvimento

## 🏗️ Arquitetura

### Domain-Driven Design (DDD)

O projeto segue os princípios do DDD, organizando o código em camadas bem definidas:

```
MoutsTI-Desafio/
├── MoutsTI.API/              # Camada de Apresentação (Controllers, Middlewares)
├── MoutsTI.Application/      # Camada de Aplicação (Casos de uso, Inicialização)
├── MoutsTI.Domain/           # Camada de Domínio (Entidades, Serviços, Regras de Negócio)
├── MoutsTI.Dtos/             # Data Transfer Objects (Contratos de API)
├── MoutsTI.Infra/            # Camada de Infraestrutura (EF Core, Repositories)
├── MoutsTI.Infra.Interfaces/ # Interfaces da camada de infraestrutura
├── MoutsTI.Tests/            # Testes Unitários (xUnit + Moq)
└── moutsti.presentation/     # Frontend React + TypeScript
```

#### Camadas do DDD:

**1. Domain Layer (MoutsTI.Domain)**
- **Entities**: `EmployeeModel`, `EmployeeRoleModel`, `EmployeePhoneModel`
- **Value Objects**: Validações encapsuladas (CPF, CNPJ, Email, Phone)
- **Domain Services**: `EmployeeService`, `EmployeeRoleService`, `AuthService`
- **Interfaces**: Contratos de serviços de domínio
- **Business Rules**: Validações complexas (idade mínima, formato de documentos, hierarquia de gestores)

**2. Application Layer (MoutsTI.Application)**
- Orquestração dos casos de uso
- Inicialização da aplicação
- Configuração de dependências

**3. Infrastructure Layer (MoutsTI.Infra)**
- **Repositories**: Implementação do padrão Repository
- **Context**: `MoutsTIContext` (Entity Framework Core)
- **Mappings**: AutoMapper profiles para conversão de entidades
- **Persistence**: Configuração de banco de dados PostgreSQL

**4. Presentation Layer (MoutsTI.API)**
- **Controllers**: Endpoints REST
- **Middlewares**: Tratamento de exceções, logging
- **Authentication**: Configuração JWT

### Clean Architecture

O projeto segue os princípios da Clean Architecture:
- **Independência de Frameworks**: O domínio não depende de bibliotecas externas
- **Testabilidade**: Todas as camadas podem ser testadas isoladamente
- **Independência de UI**: A lógica de negócio não conhece a interface
- **Independência de Banco de Dados**: O domínio não depende do EF Core
- **Regras de Negócio Centralizadas**: Toda a lógica está no domínio

## 🎨 Padrões de Projeto Implementados

### 1. Repository Pattern
Abstração do acesso a dados, permitindo trocar a implementação sem afetar o domínio.

```csharp
public interface IEmployeeRepository
{
    Task<EmployeeModel?> GetByIdAsync(long id);
    Task<IEnumerable<EmployeeModel>> GetAllAsync();
    Task<EmployeeModel> AddAsync(EmployeeModel employee);
    Task UpdateAsync(EmployeeModel employee);
    Task DeleteAsync(long id);
}
```

### 2. Dependency Injection (DI)
Inversão de controle para gerenciar dependências e promover baixo acoplamento.

```csharp
services.AddScoped<IEmployeeRepository, EmployeeRepository>();
services.AddScoped<IEmployeeService, EmployeeService>();
```

### 3. Data Transfer Object (DTO)
Separação entre modelos de domínio e contratos de API.

```csharp
public class EmployeeDto : PersonDto
{
    public long EmployeeId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public EmployeeRoleDto? Role { get; set; }
    public ManagerDto? Manager { get; set; }
}
```

### 4. Factory Method
Criação controlada de entidades com validações.

```csharp
public static EmployeeModel Create(
    string firstName, 
    string lastName, 
    string docNumber, 
    string email, 
    string password, 
    DateTime birthday, 
    long roleId, 
    long? managerId = null)
{
    // Validações e criação da instância
}
```

### 5. Service Layer
Encapsulamento da lógica de negócio em serviços de domínio.

```csharp
public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _repository;
    
    public async Task<EmployeeModel> CreateEmployeeAsync(EmployeeModel employee)
    {
        // Validações de negócio
        // Persistência via repository
    }
}
```

### 6. Unit of Work (Implicit)
O Entity Framework Core gerencia transações automaticamente através do DbContext.

### 7. Builder Pattern (AutoMapper)
Construção complexa de objetos através de profiles de mapeamento.

```csharp
public class EmployeeProfile : Profile
{
    public EmployeeProfile()
    {
        CreateMap<Employee, EmployeeModel>()
            .ForMember(dest => dest.Phones, opt => opt.MapFrom(src => src.Phones));
    }
}
```

### 8. Singleton Pattern
Configurações e serviços de aplicação únicos.

```csharp
services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
```

### 9. Strategy Pattern (Validações)
Diferentes estratégias de validação para CPF, CNPJ, Email, etc.

### 10. Specification Pattern
Validações complexas encapsuladas em métodos específicos.

```csharp
private static bool IsValidCPF(string cpf) { }
private static bool IsValidCNPJ(string cnpj) { }
```

## 🧹 Princípios de Clean Code

### SOLID Principles

**S - Single Responsibility Principle**
- Cada classe tem uma única responsabilidade
- `EmployeeModel` - Representa um funcionário
- `EmployeeService` - Lógica de negócio de funcionários
- `EmployeeRepository` - Acesso a dados de funcionários

**O - Open/Closed Principle**
- Classes abertas para extensão, fechadas para modificação
- Uso de interfaces permite adicionar novas implementações

**L - Liskov Substitution Principle**
- Interfaces podem ser substituídas por suas implementações
- `IEmployeeRepository` pode ser implementado de diferentes formas

**I - Interface Segregation Principle**
- Interfaces específicas e coesas
- `IEmployeeService`, `IEmployeeRepository`, `IAuthService`

**D - Dependency Inversion Principle**
- Dependência de abstrações, não de implementações
- Controllers dependem de interfaces, não de classes concretas

### Outras Práticas

**DRY (Don't Repeat Yourself)**
- Validações reutilizáveis
- Métodos auxiliares compartilhados

**KISS (Keep It Simple, Stupid)**
- Código simples e direto
- Evita complexidade desnecessária

**YAGNI (You Aren't Gonna Need It)**
- Implementa apenas o necessário
- Sem over-engineering

**Naming Conventions**
- Nomes descritivos e auto-explicativos
- Padrão consistente em todo o código

**Error Handling**
- Tratamento adequado de exceções
- Mensagens de erro claras

**Immutability**
- Propriedades somente leitura onde apropriado
- Value Objects imutáveis

## 🧪 Testes Unitários

### Cobertura de Testes

O projeto possui **alta cobertura de testes** (>80%):
- Testes de entidades (Domain Models)
- Testes de serviços (Business Logic)
- Testes de validações
- Testes de integração com mocks

### Tecnologias de Teste

- **xUnit**: Framework de testes
- **Moq**: Biblioteca de mocking
- **FluentAssertions**: Asserções fluentes e legíveis
- **Coverlet**: Cobertura de código

### Exemplos de Testes

```csharp
[Fact]
public void Create_WithValidParameters_ShouldCreateInstance()
{
    // Arrange
    var firstName = "John";
    var lastName = "Doe";
    
    // Act
    var employee = EmployeeModel.Create(firstName, lastName, ...);
    
    // Assert
    employee.Should().NotBeNull();
    employee.FirstName.Should().Be("John");
}
```

## 💻 Tecnologias

### Backend (.NET 8)
- **.NET 8** - Framework principal
- **C# 12** - Linguagem de programação
- **ASP.NET Core Web API** - REST API
- **Entity Framework Core 8** - ORM
- **PostgreSQL** - Banco de dados relacional
- **AutoMapper** - Mapeamento de objetos
- **JWT Bearer** - Autenticação
- **Serilog** - Logging estruturado
- **xUnit** - Framework de testes
- **Moq** - Mocking para testes
- **FluentAssertions** - Asserções de testes

### Frontend (React)
- **React 18** - Biblioteca UI
- **TypeScript** - Tipagem estática
- **Vite** - Build tool
- **Tailwind CSS** - Estilização
- **shadcn/ui** - Componentes UI
- **React Router** - Navegação
- **Axios** - Cliente HTTP
- **Zod** - Validação de schemas
- **React Hook Form** - Gerenciamento de formulários

### DevOps & Qualidade
- **Docker & Docker Compose** - Containerização
- **GitHub Actions** - CI/CD
- **SonarCloud** - Análise de qualidade de código
- **Coverlet** - Cobertura de testes

## 🔧 Configuração e Execução

### Pré-requisitos

- .NET 8 SDK
- Node.js 18+ (para o frontend)
- PostgreSQL 15+
- Docker e Docker Compose (opcional)

### Executar com Docker (Recomendado)

```bash
# Clone o repositório
git clone https://github.com/landim32/MoutsTI-Desafio.git
cd MoutsTI-Desafio

# Inicie os containers
docker-compose up -d

# A API estará disponível em http://localhost:5000
# O frontend estará disponível em http://localhost:5173
```

### Executar localmente

#### Backend

```bash
# Configure a connection string no appsettings.json
# Execute as migrações
dotnet ef database update --project MoutsTI.Infra --startup-project MoutsTI.API

# Execute a API
dotnet run --project MoutsTI.API

# A API estará disponível em http://localhost:5000
# Swagger: http://localhost:5000/swagger
```

#### Frontend

```bash
cd moutsti.presentation

# Instale as dependências
npm install

# Execute em modo de desenvolvimento
npm run dev

# O frontend estará disponível em http://localhost:5173
```

### Executar Testes

```bash
# Executar todos os testes
dotnet test

# Executar com cobertura
dotnet test --collect:"XPlat Code Coverage"

# Gerar relatório de cobertura
dotnet tool install -g dotnet-reportgenerator-globaltool
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html
```

## 📚 API Endpoints

### Authentication
- `POST /api/auth/login` - Autenticação de usuário
  - Request: `{ "email": "rodrigo@emagine.com.br", "password": "teste123" }`
  - Response: `{ "token": "JWT_TOKEN", "employee": { ... } }`

### Employees
- `GET /api/employees` - Lista todos os funcionários
- `GET /api/employees/{id}` - Busca funcionário por ID
- `POST /api/employees` - Cria novo funcionário
- `PUT /api/employees/{id}` - Atualiza funcionário
- `DELETE /api/employees/{id}` - Remove funcionário

### Employee Roles
- `GET /api/employeeroles` - Lista todos os cargos
- `GET /api/employeeroles/{id}` - Busca cargo por ID
- `POST /api/employeeroles` - Cria novo cargo
- `PUT /api/employeeroles/{id}` - Atualiza cargo
- `DELETE /api/employeeroles/{id}` - Remove cargo

## 📊 Qualidade de Código

### SonarCloud

Este projeto utiliza **SonarCloud** para análise contínua de qualidade de código. A cada push ou pull request, o código é automaticamente analisado para:

- ✅ **Cobertura de testes** - Mínimo de 80%
- 🐛 **Bugs e vulnerabilidades** - Zero tolerância
- 🔍 **Code smells** - Manutenção da qualidade
- 📋 **Duplicação de código** - Minimização
- 💰 **Débito técnico** - Controle rigoroso

[Ver análise completa no SonarCloud](https://sonarcloud.io/project/overview?id=landim32_MoutsTI-Desafio)

### Métricas de Qualidade

- **Cobertura de Testes**: >80%
- **Linhas de Código**: ~5000
- **Testes Unitários**: 200+
- **Zero Bugs Críticos**
- **Zero Vulnerabilidades**
- **Manutenibilidade**: A (Excelente)

## 🚀 Funcionalidades Principais

### Gerenciamento de Funcionários
- ✅ CRUD completo
- ✅ Validação de CPF e CNPJ
- ✅ Validação de email
- ✅ Gerenciamento de telefones
- ✅ Relacionamento gestor/subordinado
- ✅ Validação de idade (18+)
- ✅ Foto de perfil

### Gerenciamento de Cargos
- ✅ CRUD completo
- ✅ Descrição e hierarquia

### Autenticação e Segurança
- ✅ JWT Bearer Authentication
- ✅ Hash de senhas (BCrypt)
- ✅ Autorização por roles
- ✅ Proteção de rotas

### Interface do Usuário
- ✅ Design responsivo
- ✅ Tema dark/light
- ✅ Componentes reutilizáveis
- ✅ Validação de formulários
- ✅ Feedback visual (toasts)
- ✅ Loading states
- ✅ Tratamento de erros

## 🌟 Diferenciais do Projeto

### Técnicos
- ✨ Arquitetura DDD completa
- ✨ Alta cobertura de testes (>80%)
- ✨ Validações robustas de negócio
- ✨ Análise contínua de código (SonarCloud)
- ✨ CI/CD com GitHub Actions
- ✨ Documentação completa
- ✨ Código limpo e bem estruturado
- ✨ Containerização com Docker

### Negócio
- ✨ Interface moderna e intuitiva
- ✨ Experiência de usuário fluida
- ✨ Feedback visual claro
- ✨ Validações em tempo real
- ✨ Hierarquia de gestores
- ✨ Múltiplos telefones por funcionário

## 🤝 Contribuindo

1. Fork o projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

Todas as contribuições passam por:
- ✅ Revisão de código
- ✅ Análise automática do SonarCloud
- ✅ Testes unitários obrigatórios
- ✅ Cobertura de código mantida

## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.

## 👨‍💻 Autor

**Rodrigo Landim Carneiro**
- 🌐 GitHub: [@landim32](https://github.com/landim32)
- 🏢 Organização SonarCloud: [landim32](https://sonarcloud.io/organizations/landim32)
- 💼 LinkedIn: [rodrigo-landim](https://www.linkedin.com/in/rodrigo-landim)

---

⭐ Se este projeto foi útil para você, considere dar uma estrela no GitHub!

**Desenvolvido com ❤️ usando .NET 8, React e boas práticas de desenvolvimento**
