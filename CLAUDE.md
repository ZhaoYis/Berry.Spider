# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build and test commands

- `dotnet build Berry.Spider.sln` - build the full solution.
- `dotnet build desktop/Berry.Spider.AIGenPlus/Berry.Spider.AIGenPlus.csproj` - build the AI desktop app and validate AgentsV2/AgentsV3 changes.
- `dotnet build host/Berry.Spider.HttpApi.Host/Berry.Spider.HttpApi.Host.csproj` - build the main ASP.NET Core host.
- `dotnet test test/Berry.Spider.Tests/Berry.Spider.Tests.csproj` - run the test project.
- `dotnet test test/Berry.Spider.Tests/Berry.Spider.Tests.csproj --filter FullyQualifiedName~StringExtensions_UnitTest` - run a single test class.
- `dotnet test test/Berry.Spider.Tests/Berry.Spider.Tests.csproj --filter FullyQualifiedName~StringExtensions_UnitTest.SomeTestName` - run a single test method.
- `dotnet run --project host/Berry.Spider.HttpApi.Host/Berry.Spider.HttpApi.Host.csproj` - run the main HTTP API host.
- `dotnet run --project desktop/Berry.Spider.AIGenPlus/Berry.Spider.AIGenPlus.csproj` - run the Avalonia AI desktop app.

## Environment and repo quirks

- `global.json` pins SDK `10.0.0` with `rollForward: latestMajor`; use the repo SDK selection instead of assuming .NET 8/9 from README text.
- The solution mixes `src/`, `host/`, `desktop/`, `admin/`, `tools/`, and `test/`; prefer project-level builds/tests over whole-solution runs when changing a bounded area.
- `test/Berry.Spider.Tests` currently references `src/Berry.Spider.Core`; use it for core helper regression checks, not as full-solution coverage.
- `host/Berry.Spider.HttpApi.Host/Properties/launchSettings.json` uses `ASPNETCORE_ENVIRONMENT=PROD` and opens Swagger on `http://localhost:44306`.

## High-level architecture

- This is a multi-project ABP-based .NET monorepo: `src/` contains the core spider/business modules, `host/` exposes the main HTTP API, `desktop/` contains desktop tools/apps, `admin/` is a separate ABP admin stack, `tools/` contains one-off import/sync utilities, and `test/` targets shared core helpers.
- The main backend follows the standard ABP layering: `Domain.Shared` / `Domain` / `Application.Contracts` / `Application` / `HttpApi` / `HttpApi.Host`, with `EntityFrameworkCore` providing persistence and host projects composing the runtime dependencies.
- Spider capabilities are split into vertical modules under `src/` (for example Baidu, TouTiao, Sogou, proxy, weather, OpenAI, SemanticKernel, real-time, event bus) and are wired together from the host projects via project references and ABP dependency injection.
- `desktop/Berry.Spider.AIGenPlus` is an Avalonia MVVM app focused on AI features; it combines `Microsoft.Agents.AI` workflows, Semantic Kernel integrations, and project plugins from `src/Berry.Spider.SemanticKernel.Plugins`.
- `AgentsV3` in `desktop/Berry.Spider.AIGenPlus/ViewModels/Pages/AgentsV3` is a typed workflow pipeline: `SummarizeWriterExecutor -> MainWriterExecutor -> ReviewerExecutor`, with workflow state stored via `IWorkflowContext` and final UI streaming handled in `ArticleWriterAgentV3ViewModel`.

## AgentsV3 workflow rules

- AgentsV3 uses a typed workflow entry (`RunStreamingAsync(workflow, string)`); do not trigger it with `ChatMessage` + `TrySendMessageAsync(TurnToken)`.
- Every `Executor` in AgentsV3 should implement `ConfigureProtocol(ProtocolBuilder)` explicitly; route handlers with `ConfigureRoutes(...AddHandler<...>...)`.
- If an executor calls `context.SendMessageAsync<T>()` or `context.YieldOutputAsync<T>()`, declare the same types in `ConfigureProtocol` with `SendsMessage<T>()` / `YieldsOutput<T>()`.
- For `ChatClientAgent` in this repo, prefer `RunAsync(input)` / `RunStreamingAsync(input)` without `AgentSession`; session-managed chat history caused invalid conversation-id failures here.
- For AI agent, workflow, or Avalonia MVVM changes, validate first in `desktop/Berry.Spider.AIGenPlus` before escalating to full-solution builds.
