# Docker Setup - MoutsTI

Este projeto inclui configuração Docker completa para API .NET 8, Frontend React e PostgreSQL.

## Estrutura

- **api**: Aplicação .NET 8 Web API
- **frontend**: Aplicação React com Vite e shadcn/ui
- **postgres**: Banco de dados PostgreSQL 15

## Pré-requisitos

- Docker
- Docker Compose

## Configuração

1. Copie o arquivo `.env.example` para `.env`:
```bash
cp .env.example .env
```

2. Edite o arquivo `.env` e configure as variáveis de ambiente necessárias:
   - `POSTGRES_PASSWORD`: Senha do banco de dados
   - `JWT_SECRET_KEY`: Chave secreta para JWT (mínimo 32 caracteres)

## Execução

### Modo Produção

Para executar todos os serviços em modo produção:

```bash
docker-compose up -d
```

Acesse:
- **Frontend**: http://localhost:3000
- **API**: http://localhost:5000
- **PostgreSQL**: localhost:5432

### Modo Desenvolvimento

Para executar com hot-reload no frontend:

```bash
docker-compose -f docker-compose.dev.yml up -d
```

Acesse:
- **Frontend (dev)**: http://localhost:3000
- **API**: http://localhost:5000
- **PostgreSQL**: localhost:5432

## Comandos Úteis

### Ver logs dos serviços
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

### Parar os serviços
```bash
docker-compose down
```

### Parar e remover volumes (ATENÇÃO: apaga o banco de dados)
```bash
docker-compose down -v
```

### Reconstruir imagens
```bash
docker-compose build
```

### Reconstruir e executar
```bash
docker-compose up -d --build
```

## Volumes

- **postgres-data**: Persiste os dados do PostgreSQL

## Network

Todos os serviços estão na mesma rede (`moutsti-network`) e podem se comunicar entre si usando os nomes dos serviços como hostname.

## Desenvolvimento

### Frontend
O Dockerfile de desenvolvimento (`Dockerfile.dev`) monta o código fonte como volume, permitindo hot-reload das alterações.

### API
Para desenvolvimento da API, recomenda-se executar localmente fora do Docker para melhor experiência de debugging.

### Banco de Dados
O PostgreSQL sempre roda no Docker, tanto em desenvolvimento quanto em produção.

## Troubleshooting

### Porta já em uso
Se alguma porta estiver em uso, edite os mapeamentos no `docker-compose.yml` ou `docker-compose.dev.yml`.

### Problemas de permissão no volume do PostgreSQL
```bash
docker-compose down -v
docker-compose up -d
```

### Rebuild completo
```bash
docker-compose down -v
docker-compose build --no-cache
docker-compose up -d
```
