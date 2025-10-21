# Developer Guide

This file documents project conventions and mandatory rules to follow when making changes to this repository. These rules are intentionally strict to ensure consistency, maintainability and test coverage.

## Mandatory rules (always follow)
1. Prefer primary constructors for classes where appropriate (use concise record/class primary constructor syntax).
2. Always document API controllers and public controller endpoints with XML comments so Swagger includes useful docs.
3. Always use braces (`{ }`) after `if`, `for`, `foreach`, `while`, etc., even for single-line bodies.
4. Preserve and follow the existing clean structure and patterns already implemented in the project.
5. Abide by SOLID principles when making changes.
6. Always create unit tests for any new code introduced.
7. Always fix and run unit tests for any modified code; do not ship code that breaks tests.

## Testing and CI expectations
- Unit tests must be added to the appropriate test project under `tests/`.
- Tests should be fast and deterministic. Use in-memory providers or mocks for external dependencies.
- Run `dotnet test` and ensure all tests pass before pushing changes.

## Database / Migrations
- If schema changes are required, create EF Core migrations and include migration files.
- Run a build after adding migrations to ensure compilation.

## SignalR and cross-project interactions
- Host SignalR hubs in the API project (host concerns belong to API).
- Use an `INotificationPublisher` abstraction in `bcp.Application` if services outside the API need to trigger notifications.

## Code style
- Follow existing code layout, naming, and spacing.
- Keep methods small and single-responsibility.