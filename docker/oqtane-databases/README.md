# Oqtane database containers

These Compose files run one isolated Linux database server for each Oqtane host:

- `windows` is for the Oqtane installation running directly on Windows.
- `ubuntu` is for the separate Oqtane installation running in WSL2 Ubuntu.

The database data is stored in separate named Docker volumes, so the two Oqtane installations never share a database.

## Prerequisites

1. Start Docker Desktop and use the WSL2 Linux engine.
2. In Docker Desktop, enable **Settings > Resources > WSL Integration > Ubuntu**.
3. Set a development password before running Compose. SQL Server requires at least eight characters from three character groups.

PowerShell:

```powershell
$env:OQTANE_DB_PASSWORD = 'ToSic26!'
```

Ubuntu:

```bash
export OQTANE_DB_PASSWORD='ToSic26!'
```

Use the same password in the Oqtane installer. Do not commit it to a file.

## Start the database servers

From this directory, each command starts both the Windows and Ubuntu database servers for one provider. Run only the provider being tested, or run all three sets together.

```powershell
docker compose -f compose.sqlserver.yml up -d
docker compose -f compose.postgresql.yml up -d
docker compose -f compose.mysql.yml up -d
docker compose -f compose.sqlserver.yml ps
docker compose -f compose.postgresql.yml ps
docker compose -f compose.mysql.yml ps
```

The first start downloads the images and initializes the named volumes. Wait until both services report `healthy` before installing Oqtane.

## Oqtane installer values

Use `127.0.0.1` from both Windows and WSL2 Ubuntu. All ports bind only to the host loopback interface so the development databases are not published to the local network.

| Provider | Oqtane host | Server | Port | Database | User | Installer options |
| --- | --- | --- | ---: | --- | --- | --- |
| SQL Server | Windows | `127.0.0.1,14331` | - | `oqtane` | `sa` | Custom security; encryption `false` |
| SQL Server | Ubuntu | `127.0.0.1,14332` | - | `oqtane` | `sa` | Custom security; encryption `false` |
| PostgreSQL | Windows | `127.0.0.1` | `54321` | `oqtane` | `oqtane` | Custom security |
| PostgreSQL | Ubuntu | `127.0.0.1` | `54322` | `oqtane` | `oqtane` | Custom security |
| MySQL | Windows | `127.0.0.1` | `33061` | `oqtane` | `oqtane` | - |
| MySQL | Ubuntu | `127.0.0.1` | `33062` | `oqtane` | `oqtane` | - |

For SQL Server, Oqtane creates the empty `oqtane` database during installation. PostgreSQL and MySQL create it when their containers initialize.

## SQLite, LocalDB, and Azure SQL

SQLite has no database server, so no container is needed. Select SQLite in each Oqtane installer; each installation creates its own database file in its own `App_Data` directory.

LocalDB is a Windows-only SQL Server development feature, not a Linux database server. Azure SQL is an external managed service and uses Oqtane's SQL Server provider. The SQL Server containers in this folder cover local testing of that provider.

## Stop or reset

Stop a provider and preserve its databases:

```powershell
docker compose -f compose.postgresql.yml down
```

Delete both databases for a provider and start clean:

```powershell
docker compose -f compose.postgresql.yml down --volumes
```

Replace the file name with the provider to stop or reset. `--volumes` permanently deletes that provider's Windows and Ubuntu databases.
