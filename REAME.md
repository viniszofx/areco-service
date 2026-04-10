# Areco - Gestão de Produtos

Aplicação full stack para gestão de produtos, com:

- Backend em ASP.NET Core Web API
- Frontend em React + Vite
- Banco PostgreSQL

## Pré-requisitos

- .NET SDK 10 (ou compatível com o projeto)
- Node.js 18+
- pnpm
- Docker (para subir o PostgreSQL)

## Estrutura do projeto

- `backend`: API ASP.NET Core
- `frontend`: interface React

## 1. Subir banco de dados

No diretório `backend`:

```bash
docker compose up -d
```

Isso cria o PostgreSQL com:

- host: `localhost`
- porta: `5432`
- database: `arecodb`
- user: `areco`
- password: `areco`

## 2. Rodar o backend

No diretório `backend`:

```bash
dotnet restore
dotnet ef database update
dotnet run
```

Para rodar com hot reload:

```bash
dotnet watch run
```

Se precisar instalar os pacotes do Entity Framework manualmente:

```bash
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
```

API disponível em:

- `http://localhost:5000`

## 3. Rodar o frontend

No diretório `frontend`:

```bash
pnpm install
pnpm dev
```

Frontend disponível em:

- `http://localhost:5173`

Observação:

- O frontend já está configurado com proxy para `/product` apontando para `http://localhost:5000`.

## Endpoints principais

Base: `http://localhost:5000/product`

- `GET /product` -> lista produtos
- `GET /product/{id}` -> detalhe de produto
- `POST /product` -> cria produto
- `PUT /product/{id}` -> atualiza produto
- `DELETE /product/{id}` -> remove produto

## Exemplo de payload (POST/PUT)

```json
{
  "sku": "ABC123",
  "name": "Mouse Gamer",
  "category": "Eletrônicos",
  "description": "Mouse com 6 botões",
  "price": 99.9,
  "stock": 10
}
```

## Regras de negócio implementadas no backend

- SKU obrigatório e único
- Nome obrigatório
- Categoria obrigatória
- Estoque não pode ser menor que 0
- Se categoria for Eletrônicos/Eletronicos, preço mínimo é 50

## Comandos úteis

No `backend`:

```bash
dotnet ef migrations add NomeDaMigration
dotnet ef database update
```

No `frontend`:

```bash
pnpm build
pnpm preview
```
