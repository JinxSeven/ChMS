## Monorepo Orchestration

**Turbo** is a high-performance task orchestrator for monorepos that runs, caches, and parallelizes build/dev scripts across multiple projects.

**Nx** is a full-featured monorepo framework that provides task orchestration, dependency graph analysis, code generators, and architectural enforcement.

### Turbo Over Nx

 **Turborepo (Turbo)** was chosen as this monorepo task orchestrator.

### Reasoning

The project consists of:

- ASP.NET API
- React frontend
- Docker-based deployment (probably)

Turbo was chosen because:

- Focuses on task orchestration
- Does not impose architectural opinions
- Integrates cleanly with non-JavaScript tools (like .NET)
- Minimal configuration overhead

For this project’s size and scope, Turbo provides:

- Simpler setup
- Faster onboarding
- Clearer mental model