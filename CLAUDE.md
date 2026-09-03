# FinControl — Sistema Web de Controle Financeiro Pessoal

## Contexto
Projeto de portfólio full stack (nível Júnior/Pleno) para demonstrar boas práticas
de mercado, construído por um Analista de Sistemas Júnior que hoje trabalha com
ASP.NET WebForms legado e quer mostrar domínio do stack .NET moderno + React.

## Objetivo do produto
Resolver uma dor real de controle financeiro pessoal:
- Monitoramento de gastos mensais categorizados (Carro, Alimentação, Saídas, etc.)
- Geração de médias de gastos e alertas visuais de onde é possível reduzir custos
- Calculadora inteligente: renda − gastos − reserva de segurança = quanto investir no mês

## Stack definida
- **Frontend:** React + TypeScript, Vite, TanStack Query (chamadas HTTP), Recharts (gráficos)
- **Backend:** C#/.NET 8+, ASP.NET Core Web API
- **Banco:** PostgreSQL (via EF Core / Npgsql)
- **Auth:** JWT com ASP.NET Identity

## Arquitetura (Clean Architecture simplificada, 4 projetos)
```
FinControl.sln
├── FinControl.Api            → Controllers, middlewares, Program.cs (DI), Swagger
├── FinControl.Application    → Services, DTOs, FluentValidation
├── FinControl.Domain         → Entidades, enums, interfaces (contratos)
└── FinControl.Infrastructure → EF Core, DbContext, Repositories, Migrations
```
Regra chave: `Domain` não depende de nada. `Infrastructure` implementa as interfaces
definidas em `Domain`. `Application` orquestra via essas interfaces (inversão de dependência).

## Modelo de dados (DER)
Entidades: `Usuario`, `Categoria`, `Transacao`, `MetaMensal`.
- `Usuario` 1:N `Transacao`, 1:N `Categoria`, 1:N `MetaMensal`
- `Categoria` 1:N `Transacao`, 1:N `MetaMensal`
- `Transacao.Tipo`: 1 = Receita, 2 = Despesa (decimal para `Valor`, nunca float)
- `MetaMensal`: limite por categoria/mês/ano, usado para os alertas de estouro

## Roadmap (ordem de execução)
1. **Modelagem** — DER + script DDL das 4 tabelas
2. **API núcleo** — solution .NET, EF Core + Migrations, CRUD de Categoria e Transacao,
   depois endpoints de inteligência: `/api/dashboard/resumo-mensal`,
   `/api/dashboard/medias-por-categoria`, `/api/dashboard/sugestao-investimento`
3. **Autenticação** — JWT com ASP.NET Identity
4. **Frontend** — Vite + React + TS: login, lançamento de transações, dashboard com
   gráficos (pizza por categoria, linha de evolução mensal), alertas visuais de estouro
5. **Polimento** — README com diagrama de arquitetura, testes unitários (xUnit) nos
   services, Docker Compose (API + Postgres), deploy (Render/Railway + Vercel)

## Próximo passo imediato
Começar pela Fase 1: criar o script DDL do PostgreSQL a partir do DER acima.
