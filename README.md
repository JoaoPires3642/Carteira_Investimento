# Carteira_Investimento — Guia de Execução

Este guia descreve como rodar o backend (.NET) e o frontend (Next.js) em ambiente local no Windows.

## Pré-requisitos
- .NET SDK 10.0 ou superior
- Node.js 18+ (recomendado 20+) e npm
- PowerShell com permissão para executar scripts na sessão atual

## Estrutura
- Backend: `InvestidorCarteira/3.Infrastructure`
- Frontend: `InvestidorCarteira/4.Presentation/investment-portfolio-management`

## Variáveis de ambiente (opcional)
- Frontend:
  - `NEXT_PUBLIC_API_URL` (default: `http://localhost:5000`)
  - `NEXT_PUBLIC_PORTFOLIO_ID` (se quiser fixar um ID da carteira)
- Crie `InvestidorCarteira/4.Presentation/investment-portfolio-management/.env.local`:
  ```
  NEXT_PUBLIC_API_URL=http://localhost:5000
  # NEXT_PUBLIC_PORTFOLIO_ID=xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx
  ```

## Rodar o Backend
1. Abra um terminal na pasta do backend:
   ```
   cd InvestidorCarteira/3.Infrastructure
   ```
2. Execute:
   ```
   dotnet run
   ```
3. A API sobe em `http://localhost:5000` e o Swagger em `http://localhost:5000/swagger`.

### Dicas de troubleshooting (backend)
- Arquivos bloqueados (erros MSB3026/MSB3027):
  ```
  powershell -NoProfile -Command "Get-Process -Name dotnet -ErrorAction SilentlyContinue | Stop-Process -Force"
  dotnet clean
  dotnet run
  ```
- Porta em uso: encerre processos `dotnet` como acima e rode novamente.
- CORS: se usar outra porta do front, ajuste as origens permitidas em `InvestidorCarteira/3.Infrastructure/Program.cs:34-44`.

## Rodar o Frontend
1. Abra um segundo terminal na pasta do frontend:
   ```
   cd InvestidorCarteira/4.Presentation/investment-portfolio-management
   ```
2. Instale dependências (primeira vez):
   ```
   npm install
   ```
3. No PowerShell, habilite scripts na sessão atual e inicie o dev server:
   ```
   Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass
   npm run dev
   ```
4. A aplicação abre em `http://localhost:3000`.

### Dicas de troubleshooting (frontend)
- Erro de política de execução do PowerShell:
  ```
  Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass
  ```
- ESLint 9 exige `eslint.config.js`. Se `npm run lint` falhar, crie a config ou rode o dev server sem lint.

## Rodar em paralelo
- Use dois terminais:
  - Terminal 1 (backend):
    ```
    cd InvestidorCarteira/3.Infrastructure
    dotnet run
    ```
  - Terminal 2 (frontend):
    ```
    cd InvestidorCarteira/4.Presentation/investment-portfolio-management
    Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass
    npm run dev
    ```

## Endpoints úteis
- Swagger: `http://localhost:5000/swagger`
- Frontend: `http://localhost:3000`

## Notas
- A API usa `pt-BR` e aplica migrations no startup.
- O front cria/persiste `portfolioId` automaticamente se não existir.