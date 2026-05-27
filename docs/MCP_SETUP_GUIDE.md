# ✅ MCP Transaction Server - Debug Completo

## Problema Identificado

1. **Arquivo `.mcp.json` faltava** — criado em `.mcp.json`
2. **Porta incorreta** — API em **5274**, MCP procurava por **5273**
3. **Dependência de execução** — ambos os servidores precisam estar rodando

## ✨ Solução

### Opção 1: Usar o script de inicialização (RECOMENDADO)

Execute o script `run_servers.bat`:

```bash
d:/Projetos/projetos-ia/mcp-transaction-server/run_servers.bat
```

Isso vai:
- ✓ Matar todos os processos `dotnet` anteriores
- ✓ Iniciar a API MockTransaction na porta **5274**
- ✓ Aguardar 5 segundos
- ✓ Iniciar o MCP Server com a porta correta

### Opção 2: Manual (Linux/WSL)

```bash
cd d:/Projetos/projetos-ia/mcp-transaction-server

# Terminal 1 - API
dotnet run --project src/TransactionMockApi

# Terminal 2 - MCP Server
export TRANSACTION_API_BASE_URL=http://localhost:5274
dotnet run --project src/TransactionMcpServer
```

## 📝 Arquivos Modificados

- ✅ `.mcp.json` - Criado com configuração correta
- ✅ `src/TransactionMockApi/Program.cs` - Agora aceita porta via `API_PORT` env var
- ✅ `run_servers.bat` - Script Windows para inicializar tudo

## 🧪 Testando

Após os servidores estarem rodando:

```bash
# Claude Code vai conseguir acessar:
claude code

# E usar a ferramenta:
# > Get transaction tx-001
```

## Portas

- **API Mock**: `http://localhost:5274` (TransactionMockApi)
- **MCP Server**: stdio (entrada/saída padrão)
