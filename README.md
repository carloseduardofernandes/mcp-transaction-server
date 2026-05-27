# Servidor MCP .NET - Transações

Projeto .NET 8 com servidor MCP (Model Context Protocol) que expõe a tool **get_transaction_details** e uma API mock de transações para testes com LLM.

Compatível com **VS Code / GitHub Copilot** e **Claude Code**.

## Estrutura

- **TransactionMockApi** – API REST mock em `http://localhost:5274` com `GET /api/transactions/{id}`
- **TransactionMcpServer** – Servidor MCP (stdio) com a tool `get_transaction_details(transactionId)`

## Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

## Como rodar

### 1. Subir a API mock

```bash
cd mcp-transaction-server
dotnet run --project src/TransactionMockApi
```

A API fica em **[http://localhost:5274](http://localhost:5274)**. IDs de exemplo: `tx-001`, `tx-002`, `tx-003`, `tx-004`, `tx-005`.

### 2. Subir o servidor MCP

```bash
cd mcp-transaction-server
dotnet run --project src/TransactionMcpServer
```

Por padrão o MCP usa `http://localhost:5273` como base da API. Para usar a porta correta (5274):

**PowerShell:**

```bash
$env:TRANSACTION_API_BASE_URL="http://localhost:5274"
dotnet run --project src/TransactionMcpServer
```

**CMD:**

```bash
set TRANSACTION_API_BASE_URL=http://localhost:5274
dotnet run --project src/TransactionMcpServer
```

### 3. Configurar no VS Code / GitHub Copilot

O arquivo `.mcp.json` na raiz do workspace já está configurado para o VS Code. Basta abrir a pasta **mcp-transaction-server** como workspace e o Copilot detectará o servidor MCP automaticamente.

### 4. Configurar no Claude Code

Adicione o servidor ao arquivo de configuração do Claude Code (`~/.claude.json` ou `.claude.json` na raiz do projeto):

```json
{
  "mcpServers": {
    "TransactionMcpServer": {
      "command": "dotnet",
      "args": ["run", "--project", "src/TransactionMcpServer"],
      "env": {
        "TRANSACTION_API_BASE_URL": "http://localhost:5274"
      }
    }
  }
}
```

Ou use o comando:

```bash
claude mcp add TransactionMcpServer dotnet run --project src/TransactionMcpServer
```

### 5. Testar na LLM

Com a Mock API e o MCP em execução:

- "Use get_transaction_details para o id tx-001 e me diga o valor e o status."
- "Quais são os detalhes da transação tx-003?"

## Build

```bash
dotnet restore
dotnet build
```
