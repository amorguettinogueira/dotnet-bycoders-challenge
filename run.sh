#!/bin/bash

echo "Running unit tests..."
dotnet test bcp.sln --nologo --verbosity minimal

if [ $? -ne 0 ]; then
  echo "Tests failed. Aborting Docker Compose."
  exit 1
fi

echo "Starting Docker Compose..."
docker-compose up --build