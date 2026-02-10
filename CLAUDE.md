# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build & Run Commands

```bash
# Restore, build, test (from repo root)
dotnet restore
dotnet build
dotnet test --configuration Release --collect:"XPlat Code Coverage"

# Run a single test class
dotnet test --filter "FullyQualifiedName~NTools.Tests.Domain.Utils.SlugHelperTest"

# Run a single test method
dotnet test --filter "FullyQualifiedName~NTools.Tests.Domain.Utils.SlugHelperTest.TestGenerateSlug"

# Run the API locally (launches at https://localhost:9001/swagger)
dotnet run --project NTools.API

# Docker
docker compose build
docker compose up -d          # API at http://localhost:5001/swagger
docker compose logs -f ntools-api
```

## Architecture

This is a .NET 8.0 REST API following **clean architecture** with four projects:

- **NTools.API** — Controllers, HTTP configuration, Startup/Program. Controllers: `ChatGPTController`, `DocumentController`, `FileController`, `MailController`, `StringController`.
- **NTools.Application** — DI wiring only. `Initializer.Configure()` registers all services. HttpClient factory pattern is used for HTTP-based services (`MailerSendService`, `ChatGPTService`); `FileService` is registered as scoped.
- **NTools.Domain** — Business logic. `Services/` contains interface+implementation pairs (e.g., `IChatGPTService`/`ChatGPTService`). `Utils/` contains static utility classes (`DocumentUtils`, `EmailValidator`, `SlugHelper`, `StringUtils`, `ShuffleEx`).
- **NTools.Tests** — xUnit tests with Moq. Mirrors domain structure under `Domain/Services/` and `Domain/Utils/`. Uses `Mock<HttpMessageHandler>` with `RichardSzalay.MockHttp` for HTTP service tests and `Mock<IOptions<T>>` for settings.

External NuGet packages **NTools.DTO** and **NTools.ACL** provide shared DTOs and anti-corruption layer clients respectively.

## Key Patterns

- **Configuration**: Settings classes bound via `IOptions<T>` (e.g., `ChatGPTSettings`, `MailerSendSettings`, `S3Settings`). Environment variables mapped in `docker-compose.yml` and `appsettings.*.json`.
- **Controller conventions**: `[Route("[controller]")]` attribute routing, async actions, try-catch returning 500 on failure, `ILogger<T>` for logging.
- **Test conventions**: `[Fact]` for single cases, `[Theory]` with `[InlineData]` for parameterized tests. Arrange-Act-Assert pattern. `Record.Exception` for exception assertions.

## CI/CD

GitHub Actions workflows in `.github/workflows/`:
- **sonarcloud.yml** — Builds, runs tests with coverage, uploads to SonarCloud. Triggers on push to `main`/`develop` and PRs.
- **version-tag.yml** — Runs GitVersion on `main` pushes to create semantic version tags.

## Environment Setup

Copy `.env.example` to `.env` and fill in credentials for: MailerSend, OpenAI (ChatGPT + DALL-E), DigitalOcean Spaces (S3-compatible storage).
