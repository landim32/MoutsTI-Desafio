# MoutsTI - Employee Management System

[![SonarCloud](https://sonarcloud.io/images/project_badges/sonarcloud-white.svg)](https://sonarcloud.io/project/overview?id=landim32_MoutsTI-Desafio)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=landim32_MoutsTI-Desafio&metric=alert_status)](https://sonarcloud.io/project/overview?id=landim32_MoutsTI-Desafio)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=landim32_MoutsTI-Desafio&metric=coverage)](https://sonarcloud.io/project/overview?id=landim32_MoutsTI-Desafio)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=landim32_MoutsTI-Desafio&metric=bugs)](https://sonarcloud.io/project/overview?id=landim32_MoutsTI-Desafio)
[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=landim32_MoutsTI-Desafio&metric=code_smells)](https://sonarcloud.io/project/overview?id=landim32_MoutsTI-Desafio)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=landim32_MoutsTI-Desafio&metric=vulnerabilities)](https://sonarcloud.io/project/overview?id=landim32_MoutsTI-Desafio)

Sistema de gerenciamento de funcionários desenvolvido em .NET 8 com análise contínua de qualidade de código via SonarCloud.

## ?? Tecnologias

- **.NET 8** - Framework principal
- **Entity Framework Core** - ORM
- **PostgreSQL** - Banco de dados
- **AutoMapper** - Mapeamento de objetos
- **xUnit** - Testes unitários
- **Moq** - Mock para testes
- **SonarCloud** - Análise de qualidade de código
- **GitHub Actions** - CI/CD

## ?? Qualidade de Código

Este projeto utiliza SonarCloud para análise contínua de qualidade de código. A cada push ou pull request, o código é automaticamente analisado para:

- ? Cobertura de testes
- ? Bugs e vulnerabilidades
- ? Code smells
- ? Duplicação de código
- ? Débito técnico

[Ver análise completa no SonarCloud](https://sonarcloud.io/project/overview?id=landim32_MoutsTI-Desafio)

## ??? Arquitetura

O projeto segue uma arquitetura em camadas:

```
MoutsTI-Desafio/
??? MoutsTI.API/              # Camada de apresentação (Web API)
??? MoutsTI.Application/      # Camada de aplicação
??? MoutsTI.Domain/           # Camada de domínio (regras de negócio)
??? MoutsTI.Dtos/             # Data Transfer Objects
??? MoutsTI.Infra/            # Camada de infraestrutura (EF Core)
??? MoutsTI.Infra.Interfaces/ # Interfaces da camada de infraestrutura
??? MoutsTI.Tests/            # Testes unitários
```

## ?? Configuração e Execução

### Pré-requisitos

- .NET 8 SDK
- PostgreSQL
- Docker (opcional)

### Executar com Docker

```bash
docker-compose up -d
```

### Executar localmente

1. Configure a connection string no `appsettings.json`
2. Execute as migrações:
```bash
dotnet ef database update --project MoutsTI.Infra --startup-project MoutsTI.API
```
3. Execute a API:
```bash
dotnet run --project MoutsTI.API
```

## ?? Testes

Execute os testes com cobertura:

```bash
dotnet test --collect:"XPlat Code Coverage"
```

### Análise SonarCloud Local

Para executar a análise do SonarCloud localmente:

```powershell
.\sonar-analysis.ps1
```

Veja a [documentação completa de configuração do SonarCloud](.github/SONARCLOUD_SETUP.md).

## ?? API Endpoints

### Authentication
- `POST /api/auth/login` - Login de usuário

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

## ?? Documentação Adicional

- [Resumo da Implementação](IMPLEMENTACAO_RESUMO.md)
- [Implementação de Logging](LOGGING_IMPLEMENTATION.md)
- [Configuração SonarCloud](.github/SONARCLOUD_SETUP.md)

## ?? Contribuindo

1. Fork o projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

Todas as contribuições passam por análise automática do SonarCloud.

## ?? Licença

Este projeto está sob a licença MIT.

## ?? Autor

**Rodrigo Landim Carneiro**
- GitHub: [@landim32](https://github.com/landim32)
- Organização SonarCloud: [landim32](https://sonarcloud.io/organizations/landim32)

---

? Se este projeto foi útil, considere dar uma estrela no GitHub!
