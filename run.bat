@echo off

echo Running unit tests...
dotnet test Bcp.sln --nologo --verbosity minimal

IF %ERRORLEVEL% NEQ 0 (
    echo Tests failed. Aborting Docker Compose.
    exit /b %ERRORLEVEL%
)

echo Starting Docker Compose...
docker-compose up --build