#!/usr/bin/env bash
trap 'kill 0' EXIT
echo "Iniciando TelemetryService..."
dotnet run --project src/TechLogistics.TelemetryService/TechLogistics.TelemetryService.csproj &
echo "Iniciando Web Host..."
dotnet run --project src/TechLogistics.Web/TechLogistics.Web.csproj
