param(
    [string]$Server = "(localdb)\\MSSQLLocalDB",
    [string]$Database = "ShoeStore"
)

$ErrorActionPreference = "Stop"

$root = Split-Path -Parent $PSScriptRoot
$schema = Join-Path $root "db\schema.sql"
$seed = Join-Path $root "db\seed.sql"

if (-not (Get-Command sqlcmd -ErrorAction SilentlyContinue))
{
    throw "sqlcmd not found. Install SQL Server Command Line Utilities."
}

function Invoke-SqlcmdChecked
{
    param([string[]]$Args)
    & sqlcmd @Args
    if ($LASTEXITCODE -ne 0)
    {
        throw "sqlcmd failed with exit code $LASTEXITCODE."
    }
}

$createDb = "IF DB_ID(N'$Database') IS NULL BEGIN CREATE DATABASE [$Database]; END;"

Invoke-SqlcmdChecked @("-S", $Server, "-E", "-C", "-b", "-Q", $createDb)
Invoke-SqlcmdChecked @("-S", $Server, "-E", "-C", "-b", "-d", $Database, "-i", $schema)
Invoke-SqlcmdChecked @("-S", $Server, "-E", "-C", "-b", "-d", $Database, "-i", $seed)

Write-Host "Database '$Database' is ready on $Server."
