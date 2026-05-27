@echo off
REM Kill all dotnet processes
for /f "tokens=2" %%a in ('tasklist ^| find /i "dotnet"') do taskkill /PID %%a /F 2>nul

timeout /t 3 /nobreak

REM Start Mock API on port 5274
cd /d "d:\Projetos\projetos-ia\mcp-transaction-server"
start "Mock API" cmd /k "dotnet run --project src/TransactionMockApi"

REM Wait for API to start
timeout /t 5 /nobreak

REM Start MCP Server
set TRANSACTION_API_BASE_URL=http://localhost:5274
start "MCP Server" cmd /k "dotnet run --project src/TransactionMcpServer"
