# 项目 上下文

## 目的
`Berry.Spider` 是一个基于 ABP vNext 和 Selenium 的分布式爬虫系统，主要用于自动化抓取主流内容平台的搜索结果与文章内容（目前包括百度、头条、搜狗等），并结合消息队列、Redis、分词、敏感词过滤以及大模型能力，对抓取数据进行清洗、分析和后续处理。  
本项目同时作为个人学习与演示用示例，用于探索 .NET 8、分布式架构、事件驱动、AI 能力（Semantic Kernel / OpenAI / Ollama 等）在爬虫场景下的综合实践。

## 技术栈

- **后端与基础架构**
  - .NET 8 / C#
  - ABP vNext 框架（模块化、DDD、权限、多租户等基础能力）
  - Berry.Spider 多模块解决方案（`src/`、`host/`、`tools/`、`test/` 等）

- **爬虫与自动化**
  - Selenium WebDriver（Chrome & WebDriver）
  - 自定义爬虫调度与任务管理（基于 CAP / Redis / RabbitMQ 等）

- **配置与分布式协调**
  - AgileConfig 分布式配置中心
  - Redis（FreeRedis）作为缓存与分布式组件

- **消息与事件**
  - CAP（基于消息队列的事务型事件总线）
  - RabbitMQ 作为主要消息中间件
  - 自定义 Redis 事件总线模块（`Berry.Spider.EventBus.Redis`）

- **自然语言处理与文本处理**
  - jieba.NET 分词
  - ToolGood.Words 敏感词/文本过滤

- **HTTP 与外部服务集成**
  - Refit（类型安全的 HTTP API Client）
  - Useragents.me（User-Agent 列表获取）

- **AI / LLM 能力**
  - Betalgo.OpenAI（OpenAI 客户端）
  - SemanticKernel（语义内核与推理编排）
  - Ollama + OllamaSharp（本地大模型推理）

- **前端与桌面**
  - `admin/`：后台管理前端（基于 JavaScript/TypeScript + 前端框架构建，用于管理爬虫任务、配置和数据）
  - `desktop/`：桌面客户端（C# + Avalonia / WPF 风格 XAML，提供图形化管理和操作入口）

- **运维与部署**
  - Docker（多份 Dockerfile：Admin、AuthServer、SpiderApi 等）
  - Kubernetes 部署清单（`deploy/` 下的 `*.yaml`）
  - Shell 脚本（例如 `deploy/app_start.sh`）用于容器/服务启动

## 项目约定

### 代码风格

- **语言与注释**
  - 代码注释、文档、提交信息优先使用中文，必要时附带英文说明。
  - 类、方法、接口、属性命名使用标准 C# PascalCase / camelCase 约定，保持语义清晰、领域化。

- **C# / .NET**
  - 使用 .NET 8 标准，遵循 ABP 与官方 C# 编码规范：
    - 统一使用 `async/await` 异步模式。
    - 尽量使用依赖注入和接口编程，避免静态单例。
    - 业务逻辑集中在应用服务 / Domain Service / 领域对象中，控制器薄而简单。
  - 每个模块通过独立的 `*.csproj` 与 `*Module.cs` 注册 ABP 模块依赖。

- **前端**
  - 管理后台前端遵循统一组件化风格（以当前前端框架的官方最佳实践为准），CSS/样式集中管理，避免内联样式滥用。
  - 前端与后端接口使用明确的 DTO / ViewModel，避免直接暴露领域实体。

- **格式化与工具**
  - 后端统一使用 IDE/EditorConfig 配置（如 ReSharper / Rider / VSCode + C# 扩展）自动格式化。
  - 遵循现有 `.editorconfig` / 解决方案既有风格，避免在无必要时大面积格式化提交。

### 架构模式

- **整体架构**
  - 基于 ABP vNext 的模块化 + 分层架构：Domain、Application、HttpApi、EntityFrameworkCore、BackgroundWorkers 等。
  - 使用事件驱动架构（CAP/RabbitMQ/Redis EventBus）解耦爬虫任务生产、调度、执行与结果处理。
  - 多进程/多服务部署：AuthServer、AdminApi、SpiderApi、前端 Admin Web、桌面应用等通过配置中心与消息中间件协作。

- **领域与边界**
  - 爬虫任务、抓取策略、站点适配器、解析规则等作为独立领域概念，逐步抽象为可复用模块。
  - 与外部服务（OpenAI/Ollama/Useragents/第三方 API）的集成封装为独立的基础设施服务，避免直接散落在业务代码中。

- **持久化与缓存**
  - 使用 EF Core（通过 ABP）进行持久化，遵循仓储模式（Repository）。
  - 使用 Redis 做缓存、分布式锁以及轻量级队列/标记，以降低数据库压力。

- **配置与特性开关**
  - 所有重要配置（连接串、队列、第三方服务 Key 等）统一通过 AgileConfig / appsettings 管理，不在代码中硬编码。
  - 对新特性或风险功能可采用配置开关或 Feature Management 控制。

### 测试策略

- **单元测试**
  - 在 `test/` 目录中为关键领域逻辑（如爬虫调度、解析规则、文本处理）提供单元测试。
  - 使用内存实现或测试专用配置替代真实外部依赖（Redis、RabbitMQ、OpenAI 等）。

- **集成/端到端**
  - 对关键 API 和爬虫流程提供最小可行的集成测试，验证服务之间的协作和消息流转。
  - 在本地或 CI 环境中尽量模拟真实依赖（Docker Compose / 本地服务）进行端到端验证。

- **人工验证**
  - 对重要站点和新接入数据源，保留一套手工回归步骤（例如在 Admin / Desktop 中进行任务创建、执行、结果查看）的简单说明。

### Git 工作流

- **分支策略**
  - `main`/`master`：稳定可发布分支。
  - 功能分支：使用短横线命名，建议与 OpenSpec `change-id` 对齐，例如：`add-new-site-adapter`、`update-crawler-scheduler`。
  - Bugfix 分支：`fix-*` 命名，例如：`fix-duplicate-url-handling`。

- **提交约定**
  - 提交信息首行简短（建议中文），动词开头：`feat:`、`fix:`、`refactor:`、`chore:` 等可选前缀。
  - 当提交是对应某个 OpenSpec 变更时，在提交描述中引用 `change-id`（例如：`feat: 支持新站点抓取 (change: add-new-site-x)`）。

- **PR 与代码评审**
  - 功能变更优先先有 OpenSpec 提案与任务清单，再进行实现与 PR。
  - PR 描述中关联对应的 `changes/[change-id]` 目录，并说明完成了哪些 `tasks.md` 项目。

## 领域上下文

- **核心领域**
  - 搜索引擎 / 内容平台爬虫（百度、头条、搜狗等），以关键字、时间范围、站点等为主要输入条件。
  - 爬取后的数据包括：标题、摘要、正文、作者、时间、来源等，用于后续分析、展示或 AI 处理。
  - 支持多节点/多实例并发执行，依赖消息队列和 Redis 保证任务分发与负载均衡。

- **文本与内容处理**
  - 使用分词（jieba.NET）和敏感词过滤（ToolGood.Words）对抓取内容进行清洗和标记。
  - 通过 OpenAI / SemanticKernel / Ollama 等对文本进行分类、摘要或结构化抽取（视具体配置与实现而定）。

- **管理与可视化**
  - Admin Web 提供任务管理、配置管理、监控等界面。
  - Desktop 应用为桌面端使用场景（如快捷启动、配置、监控）提供图形界面。

- **学习/演示属性**
  - 项目主要用于个人学习与演示，默认不针对生产环境的高安全/高可靠做极致优化，但仍然尽量遵循良好架构与工程实践。

## 重要约束

- **用途限制**
  - 项目仅限个人学习、研究和演示使用，**禁止**用于任何违反法律法规、目标网站服务条款或道德规范的用途。

- **技术约束**
  - 依赖 .NET 8 运行时与对应工具链。
  - 爬虫实现依赖浏览器驱动（ChromeDriver 等）以及目标站点当前页面结构，站点改版可能导致部分功能失效，需要定期维护。

- **资源与性能**
  - 高并发、大规模数据抓取需谨慎配置（队列、线程数、重试策略等），避免对目标站点造成压力。
  - 默认配置偏向安全/保守，必要时通过配置中心（AgileConfig）进行调整。

- **隐私与合规**
  - 对抓取的内容应尊重版权和隐私，不存储或传播敏感个人信息。
  - 与外部 AI 服务交互时，应注意 API Key 安全与数据合规性。

## 外部依赖

- **消息与缓存**
  - RabbitMQ：作为 CAP 消息队列后端，用于事件驱动与任务分发。
  - Redis（FreeRedis）：缓存、分布式锁、状态存储等。

- **配置与服务治理**
  - AgileConfig：集中配置管理，统一管理连接字符串、功能开关、第三方服务配置。

- **浏览器与爬虫基础设施**
  - Chrome 浏览器与 ChromeDriver：Selenium 爬虫执行环境。
  - NSSM 等工具用于将服务以 Windows 服务形式运行（视部署方式而定）。

- **AI / LLM 服务**
  - OpenAI（通过 Betalgo.OpenAI）
  - SemanticKernel（微软语义内核）
  - Ollama + OllamaSharp（本地大模型）

- **文本与工具库**
  - jieba.NET、ToolGood.Words、Refit、Useragents.me 等第三方库与服务。

- **运维与部署**
  - Docker（多个服务的镜像构建）
  - Kubernetes 集群及其相关组件（Ingress、cert-manager、dashboard 等）