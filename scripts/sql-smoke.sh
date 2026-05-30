#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
CONFIG="Release"
SOLUTION="$ROOT/Core.SQL.sln"
TESTS="$ROOT/tests/Core.SQL.Tests/Core.SQL.Tests.csproj"
LIB="$ROOT/src/PolyhydraGames.Data.Sql.csproj"
SAMPLE="$ROOT/samples/Core.SQL.Smoke/Core.SQL.Smoke.csproj"
ARTIFACTS="$ROOT/artifacts/sql-smoke"

rm -rf "$ARTIFACTS"
mkdir -p "$ARTIFACTS/packages"

dotnet restore "$SOLUTION"
dotnet build "$SOLUTION" -c "$CONFIG" --no-restore
dotnet test "$TESTS" -c "$CONFIG" --no-build
dotnet pack "$LIB" -c "$CONFIG" --no-build -o "$ARTIFACTS/packages"
dotnet run --project "$SAMPLE" -c "$CONFIG" --no-restore --no-build
