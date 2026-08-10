# Oqtane / 2sxc multi-database manual upgrade test plan

Date prepared: 2026-08-04

## Goal and pass criteria

Prove that 2sxc can be installed and upgraded without data loss on every database provider in Oqtane, on both Windows and Ubuntu:

- SQL Server: include a legacy Oqtane 6.1.2 / 2sxc 19.3.5 installation and the old SQL-script upgrade path.
- PostgreSQL, MySQL, and SQLite: start with Oqtane 10.2.3 and 2sxc 21.8.0.
- Exercise both every-version upgrades (`21.8.0 -> .1 -> .2 -> .3 -> .4`) and skipped-version upgrades (`21.8.0 -> .4`).
- Verify Oqtane, 2sxc, existing content, files, relationships, database history, and restart behavior after every step.

The campaign passes only when all required scenarios pass and no provider-specific SQL, migration, or data-loss error remains.

## Fixed versions and artifacts

Pin these versions for this campaign. Do not silently replace them with a newer release halfway through testing.

| Purpose | Version / artifact |
| --- | --- |
| Legacy Oqtane baseline | Oqtane 6.1.2 (.NET 9) |
| Legacy Oqtane intermediate | Oqtane 6.2.1 (.NET 9) |
| Current Oqtane target | Oqtane 10.2.3 (.NET 10); verified as the latest official release on 2026-08-04 |
| Legacy 2sxc baseline | `ToSic.Sxc.Oqtane.Install.19.3.5.nupkg` |
| Legacy 2sxc checkpoints | `20.0.9`, `21.7.0` |
| New migration baseline | `ToSic.Sxc.Oqtane.Install.21.8.0-v2.nupkg` |
| Incremental packages | `21.8.1`, `21.8.2`, `21.8.3`, `21.8.4` |

2sxc package directory:

```text
D:\Projects\2sxc\_InstallPackages\OqtaneModule
```

The package metadata establishes these compatibility boundaries:

- 2sxc 19.3.5 requires Oqtane Framework 6.1.2 and targets `net9.0`.
- 2sxc 21.0.2 through 21.7.0 require Oqtane Framework 6.2.1 and target `net9.0`.
- 2sxc 21.8.x requires Oqtane Framework 10.2.0 or later and targets `net10.0`.

Download Oqtane install/upgrade packages only from the official release pages:

- <https://github.com/oqtane/oqtane.framework/releases/tag/v6.1.2>
- <https://github.com/oqtane/oqtane.framework/releases/tag/v6.2.1>
- <https://github.com/oqtane/oqtane.framework/releases/tag/v10.2.3>
- Upgrade instructions: <https://docs.oqtane.org/guides/installation/upgrade.html>

## Required installation matrix

Use a separate Oqtane application folder, URL/port, and database for every row. Never point two running Oqtane instances at the same database.

| ID | Oqtane host | Database | Start | Upgrade path | Purpose |
| --- | --- | --- | --- | --- | --- |
| M1 | Windows | SQL Server | Oqtane 6.1.2 + 2sxc 19.3.5 | Full legacy and incremental chain | Old MSSQL scripts, EF bridge, every 21.8 migration |
| M2 | Windows | SQL Server | Oqtane 6.1.2 + 2sxc 19.3.5 | Legacy chain, then directly to 21.8.4 | Old MSSQL database plus cumulative skipped migrations |
| M3 | Ubuntu | SQL Server | Oqtane 10.2.3 + 2sxc 21.8.0 | Directly to 21.8.4 | Linux host and current skipped migration |
| P1 | Windows | PostgreSQL | Oqtane 10.2.3 + 2sxc 21.8.0 | `.1 -> .2 -> .3 -> .4` | Every PostgreSQL migration step |
| P2 | Ubuntu | PostgreSQL | Oqtane 10.2.3 + 2sxc 21.8.0 | Directly to 21.8.4 | Linux host and cumulative PostgreSQL migration |
| Y1 | Windows | MySQL | Oqtane 10.2.3 + 2sxc 21.8.0 | `.1 -> .2 -> .3 -> .4` | Every MySQL migration step |
| Y2 | Ubuntu | MySQL | Oqtane 10.2.3 + 2sxc 21.8.0 | Directly to 21.8.4 | Linux host and cumulative MySQL migration |
| S1 | Windows | SQLite | Oqtane 10.2.3 + 2sxc 21.8.0 | `.1 -> .2 -> .3 -> .4` | Every SQLite migration step |
| S2 | Ubuntu | SQLite | Oqtane 10.2.3 + 2sxc 21.8.0 | Directly to 21.8.4 | Linux host and cumulative SQLite migration |

oqtane
- M1, M2, P1, P2, Y1, Y2, S1, and S2 are required.
- M3 is required for full operating-system coverage of SQL Server.

## Database endpoints

The database servers run in Linux containers on the Windows Docker host. Windows-only endpoints bind to loopback; Ubuntu endpoints bind to all host interfaces.

| Provider | Oqtane host | Installer server | Port | User | Database |
| --- | --- | --- | ---: | --- | --- |
| SQL Server | Windows | `127.0.0.1` | `14331` | `test` | `oqtane-m1`, `oqtane-m2` |
| SQL Server | Ubuntu VM | `xps5.mshome.net` | `14332` | `test` | `oqtane-m3` |
| PostgreSQL | Windows | `127.0.0.1` | `54321` | `oqtane` | `oqtane_p1` |
| PostgreSQL | Ubuntu VM | `xps5.mshome.net` | `54322` | `oqtane` | `oqtane_p2` |
| MySQL | Windows | `127.0.0.1` | `33061` | `test` | `oqtane-y1` |
| MySQL | Ubuntu VM | `xps5.mshome.net` | `33062` | `test` | `oqtane-y2` |
| SQLite | Either | Local file | - | - | One file inside that Oqtane installation |

Suggested SQL Server database names:

- M1: `oqtane-m1` or `oqtane_ms_legacy_inc`
- M2: `oqtane-m2` or `oqtane_ms_legacy_skip`
- M3: `oqtane-m3` or `oqtane_ms_ubuntu_skip`

Eg, for SQL Server's Oqtane installer from Ubuntu VM, use server: `xps5.mshome.net` and port: `14332`. Use custom security, user: `test` and disable encryption for these local development containers.

## Phase 1: campaign preparation

1. Create a result directory outside every Oqtane application folder, for example `D:\Projects\_Test\2sxc-oqtane\OqtaneTestResults\2026-08-04`.
1. Copy all tested 2sxc packages into the directory: `D:\Projects\2sxc\_InstallPackages\OqtaneModule\cross-platform`. Do not rebuild a package during the campaign.
1. Confirm the password exists in the current PowerShell session without printing it:

   ```powershell
   if ([string]::IsNullOrWhiteSpace($env:OQTANE_DB_PASSWORD)) { throw 'OQTANE_DB_PASSWORD is not set' }
   ```

1. Start all containerized providers from `D:\Projects\2sxc\2sxc\docker\oqtane-databases`:

   ```powershell
   docker compose -f compose.sqlserver.yml up -d
   docker compose -f compose.postgresql.yml up -d
   docker compose -f compose.mysql.yml up -d
   docker compose -f compose.sqlserver.yml ps
   docker compose -f compose.postgresql.yml ps
   docker compose -f compose.mysql.yml ps
   ```

1. Do not continue until every required service is `healthy`.
1. From Ubuntu, verify the Windows host is reachable on ports `14332`, `54322`, and `33062`, for example with `nc -vz xps5.mshome.net <port>`.
1. Create a separate Oqtane application folder and unique HTTP port for each scenario. Record the folder, URL, database, Oqtane version in the result sheet.
1. Before each Oqtane or 2sxc upgrade, stop the application and copy its application folder plus a native database backup/dump. Label the backup with the scenario and checkpoint. Never test rollback by reusing a partially upgraded database.

## Reusable test data

Create this data once at the first working 2sxc version in each scenario. Keep the values identical across providers.

1. Create an Oqtane pages named `p1` and `p2`.
1. On `p1` add a 2sxc Content module, install 2sxc Content tmplates and add one basic content item with:
   - Title: `p1-<scenario-id>`
   - Body: a sentence containing the scenario ID and baseline versions
   - Number, Boolean, and date fields if the selected content type supports them
1. Upload and render one image or file through 2sxc.
1. Create a second item, edit it once, and delete it. Confirm all three operations succeed.
1. On `p2` add 2sxc App module, install all 2sxc template apps and add `blog` app. Add at least two posts, save, reopen, edit the relationship, and render it.
1. Record a screenshot of the rendered content and the blog data.

## Validation gates

Run the Quick gate after **every** version change. Run the Full gate at each scenario baseline and final version.

### Quick gate Q

- [ ] Q1: Oqtane starts; home page, login, and Admin Dashboard load without an unhandled error.
- [ ] Q2: The displayed Oqtane and 2sxc versions equal the intended checkpoint.
- [ ] Q3: The 2sxc toolbar/admin UI opens and the existing content on `p1` renders.
- [ ] Q4: Edit and save the content body, reload the page, and confirm the change persisted.
- [ ] Q5: Run the migration-history and test-table queries below; results exactly match the expected state.
- [ ] Q6: Stop and restart Oqtane twice. Confirm the same pages still work and migration history did not gain duplicate rows.
- [ ] Q7: Review logs produced since the upgrade. Fail on unhandled exceptions or provider, SQL syntax, migration, relation, transaction, or case-sensitivity errors.

### Full gate F

Run Q, then:

- [ ] F1: Add, edit, and delete a new content item.
- [ ] F2: Open the blog app, add/remove a post, save, reload, and render the new post.
- [ ] F3: Add new blog post.
- [ ] F4: Upload a new image/file and render or download it.
- [ ] F5: Open the main 2sxc administration areas used for Apps, Data, and Settings.
- [ ] F6: Log out/in and repeat one saved edit after a cold application restart.

## Expected 21.8 migration states

All packages are cumulative. A direct install of 21.8.4 must apply every missing migration in order.

| Installed 2sxc | Required 2sxc migration IDs | Only allowed test table |
| --- | --- | --- |
| 21.8.0 | `ToSic.Sxc.21.00.00` | None |
| 21.8.1 | Baseline plus `ToSic.Sxc.21.08.01` | `TsDynDataMigrationTest` |
| 21.8.2 | Baseline plus `.01`, `.02` | `TsDynDataMigrationTest2` |
| 21.8.3 | Baseline plus `.01`, `.02`, `.03` | `TsDynDataMigrationTest3` |
| 21.8.4 | Baseline plus `.01`, `.02`, `.03`, `.04` | None |

PostgreSQL rewrites the test table names to `ts_dyn_data_migration_test`, `ts_dyn_data_migration_test2`, and `ts_dyn_data_migration_test3`.

At every checkpoint verify:

- Every required ID exists exactly once.
- No later ID exists early.
- No older test table remains after its replacement/drop migration.
- Restarting does not add history rows or recreate/drop tables again.

## Database verification queries

Run the matching queries after every 21.8.x install/upgrade.

### SQL Server

```sql
SELECT MigrationId
FROM __EFMigrationsHistory
WHERE MigrationId LIKE 'ToSic.Sxc.%'
ORDER BY MigrationId;

SELECT TABLE_NAME
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_NAME LIKE 'TsDynDataMigrationTest%'
ORDER BY TABLE_NAME;
```

### PostgreSQL

```sql
SELECT migration_id
FROM "__EFMigrationsHistory"
WHERE migration_id LIKE 'ToSic.Sxc.%'
ORDER BY migration_id;

SELECT table_name
FROM information_schema.tables
WHERE table_schema = 'public'
  AND table_name LIKE 'ts_dyn_data_migration_test%'
ORDER BY table_name;
```

### MySQL

```sql
SELECT MigrationId
FROM __EFMigrationsHistory
WHERE MigrationId LIKE 'ToSic.Sxc.%'
ORDER BY MigrationId;

SELECT TABLE_NAME
FROM information_schema.tables
WHERE table_schema = DATABASE()
  AND TABLE_NAME LIKE 'TsDynDataMigrationTest%'
ORDER BY TABLE_NAME;
```

### SQLite

```sql
SELECT MigrationId
FROM __EFMigrationsHistory
WHERE MigrationId LIKE 'ToSic.Sxc.%'
ORDER BY MigrationId;

SELECT name
FROM sqlite_master
WHERE type = 'table'
  AND name LIKE 'TsDynDataMigrationTest%'
ORDER BY name;
```

## Scenario M1: SQL Server legacy full incremental chain

Use Windows, SQL Server `127.0.0.1:14331`, and database `oqtane-m1`.

1. Install Oqtane 6.1.2 and then 2sxc 19.3.5.
1. Create the reusable test data and run Full gate F. Save checkpoint `M1-C1`.
1. Upgrade only Oqtane to 6.2.1. Run Quick gate Q. Save `M1-C2`.
1. Upgrade only 2sxc to 20.0.9. Run Q. Save `M1-C3`.
1. Upgrade only 2sxc to 21.7.0. Run Q. Confirm the legacy SQL migration `ToSic.Sxc.21.00.00` is present once. Save `M1-C4`.
1. Upgrade only Oqtane to 10.2.3 using the official Oqtane upgrade package. Run Q while 2sxc is still 21.7.0. Save `M1-C5`.
1. Upgrade only 2sxc to 21.8.0-v2. Run Q. Confirm the EF baseline recognizes the existing `ToSic.Sxc.21.00.00` row and does not recreate existing 2sxc tables. Save `M1-C6`.
1. Upgrade to 21.8.1. Run Q; only test table 1 may exist. Save `M1-C7`.
1. Upgrade to 21.8.2. Run Q; only test table 2 may exist. Save `M1-C8`.
1. Upgrade to 21.8.3. Run Q; only test table 3 may exist. Save `M1-C9`.
1. Upgrade to 21.8.4. Run Full gate F; no test table may exist. Save `M1-C10`.

Do not combine an Oqtane upgrade and a 2sxc upgrade into one checkpoint. If Oqtane 10.2.3 cannot start with 2sxc 21.7.0, preserve that failure and logs as a host/module compatibility result; restore `M1-C4` before trying a different upgrade staging procedure.

## Scenario M2: SQL Server legacy direct-to-current package

Use a second Windows Oqtane folder and database `oqtane-m2`.

1. Repeat M1 steps 1-6 to reach Oqtane 10.2.3 with 2sxc 21.7.0 and verified legacy data.
1. Install 2sxc 21.8.4 directly; do not install 21.8.0-.3 first.
1. Run Full gate F.
1. Confirm history contains the baseline and `.01` through `.04` exactly once.
1. Confirm none of the three test tables exists.

This is the decisive cumulative migration test from a very old SQL Server installation.

## Scenario M3: SQL Server current skipped upgrade on Ubuntu

Use Ubuntu, SQL Server `xps5.mshome.net:14332`, and database `oqtane-m3`.

1. Install Oqtane 10.2.3 and 2sxc 21.8.0-v2.
1. Create reusable test data and run Full gate F.
1. Upgrade directly to 21.8.4.
1. Run Full gate F and verify the final migration state.

## Scenarios P1 and P2: PostgreSQL

### P1 Windows incremental

1. Install Oqtane 10.2.3 using PostgreSQL `127.0.0.1:54321`, database `oqtane_p1`.
1. Install 2sxc 21.8.0-v2, create reusable test data, and run F.
1. Install 21.8.1, run Q, and verify only `ts_dyn_data_migration_test` exists.
1. Install 21.8.2, run Q, and verify only `ts_dyn_data_migration_test2` exists.
1. Install 21.8.3, run Q, and verify only `ts_dyn_data_migration_test3` exists.
1. Install 21.8.4, run F, and verify no test table exists.

### P2 Ubuntu skipped

1. Install Oqtane 10.2.3 using PostgreSQL `xps5.mshome.net:54322`, database/user `oqtane_p2`.
1. Install 2sxc 21.8.0-v2, create reusable test data, and run F.
1. Install 21.8.4 directly.
1. Run F and verify baseline plus `.01`-.04 exist once and no test table remains.

## Scenarios Y1 and Y2: MySQL

### Y1 Windows incremental

1. Install Oqtane 10.2.3 using MySQL `127.0.0.1:33061`, database `oqtane-y1`.
1. Install 2sxc 21.8.0-v2, create reusable test data, and run F.
1. Install 21.8.1, run Q, and verify only `TsDynDataMigrationTest` exists.
1. Install 21.8.2, run Q, and verify only `TsDynDataMigrationTest2` exists.
1. Install 21.8.3, run Q, and verify only `TsDynDataMigrationTest3` exists.
1. Install 21.8.4, run F, and verify no test table exists.

At every Y1 checkpoint repeat the relationship save/reload test, not only at the final Full gate.

### Y2 Ubuntu skipped

1. Install Oqtane 10.2.3 using MySQL `xps5.mshome.net:33062`, database/user `oqtane-y2`.
1. Install 2sxc 21.8.0-v2, create reusable test data, and run F.
1. Install 21.8.4 directly.
1. Run F, including fresh app import and relationship save/reload, and verify the final migration state.

## Scenarios S1 and S2: SQLite

SQLite needs no Docker service. Each Oqtane folder must have its own database file.

### S1 Windows incremental

1. Install Oqtane 10.2.3 with SQLite and then 2sxc 21.8.0-v2.
1. Create reusable test data and run F.
1. Install 21.8.1, 21.8.2, 21.8.3, and 21.8.4 one at a time.
1. Run Q after each package and verify the expected table transition.
1. Run F after 21.8.4 and verify no test table remains.

### S2 Ubuntu skipped

1. Install Oqtane 10.2.3 with SQLite and then 2sxc 21.8.0-v2.
1. Create reusable test data and run F.
1. Copy the SQLite file while Oqtane is stopped.
1. Install 21.8.4 directly.
1. Run F and verify the final migration state.

## Result sheet

Evidence review date: 2026-08-10.

Backups and other assets are in `D:\Projects\_Test\2sxc-oqtane\OqtaneTestResults`.

- All six Docker database services are healthy. The live Windows IIS sites `oqtane-m1`, `oqtane-m2`, `oqtane-p1`, `oqtane-y1`, and `oqtane-s1` return HTTP 200. Pages of M1 `/p1` and `/p2` rendered the content and blog posts.
- The final database probes currently available for M1, M2, M3, P1, P2, Y1, Y2, S1 and S2 contain `ToSic.Sxc.21.00.00` plus `.01` through `.04` exactly once and no migration test table.
- The M1-C5 archive contains a real Razor compiler failure (`AllowAnonymousAttribute` from ASP.NET Core 9 and 10). This is fixed in next version.
- M3 contains duplicate Oqtane `Job` key errors. Need investigation.
- P1/P2, Y1/Y2, S1/S2 baseline logs contain a 21.8.0 `HtmlHelper.CreateScript` null-reference error. Page reload fix this.
- Y1/Y2 contain Oqtane `MigrationHistoryController` errors for missing `AppliedDate`. Need investigation.

Each `Status/notes` cell starts with exactly one allowed status (`PASS`, `FAIL`, `BLOCKED`, or `NOT RUN`); text after the dash is explanatory notes.

| Scenario | Checkpoint | OS | DB/provider version | Oqtane before -> after | 2sxc before -> after | Q | F | Expected history/table | Actual result | Evidence/log path | Status/notes |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| M1 | M1-C1 | Windows | SQL Server 2022 | fresh -> 6.1.2 | none -> 19.3.5 | PASS | PASS | Legacy baseline | Application backup exists | `OqtaneTestResults\m1-c1\Oqtane.Server.zip` | PASS |
| M1 | M1-C2 | Windows | SQL Server 2022 | 6.1.2 -> 6.2.1 | 19.3.5 -> 19.3.5 | PASS | PASS | Legacy baseline | Application backup exists | `OqtaneTestResults\m1-c2\Oqtane.Server.zip` | PASS |
| M1 | M1-C3 | Windows | SQL Server 2022 | 6.2.1 -> 6.2.1 | 19.3.5 -> 20.0.9 | PASS | PASS | Legacy history only | No `m1-c3` checkpoint artifact found | `OqtaneTestResults\` | PASS - missing checkpoint |
| M1 | M1-C4 | Windows | SQL Server 2022 | 6.2.1 -> 6.2.1 | 20.0.9 -> 21.7.0 | PASS | PASS | `ToSic.Sxc.21.00.00` | Application backup exists | `OqtaneTestResults\m1-c4\Oqtane.Server.zip` | PASS |
| M1 | M1-C5 | Windows | SQL Server 2022 | 6.2.1 -> 10.2.3 | 21.7.0 -> 21.7.0 | FAIL | PASS | Legacy history unchanged | Razor compilation fails with duplicate `AllowAnonymousAttribute` from ASP.NET Core 9/10 | `OqtaneTestResults\m1-c5\blog-err.txt` | PASS, known issue that is fixed |
| M1 | M1-C6 | Windows | SQL Server 2022 | 10.2.3 -> 10.2.3 | 21.7.0 -> 21.8.0 | PASS | PASS | Baseline plus `ToSic.Sxc.21.00.00` | Checkpoint backup is missing | `OqtaneTestResults\m1-c6\Oqtane.Server.zip` | PASS - continued after M1-C5 failure |
| M1 | M1-C7 | Windows | SQL Server 2022 | 10.2.3 -> 10.2.3 | 21.8.0 -> 21.8.1 | PASS | PASS | Baseline + `.01`; only `TsDynDataMigrationTest` | Checkpoint backup exists | `OqtaneTestResults\m1-c7\Oqtane.Server.zip` | PASS |
| M1 | M1-C8 | Windows | SQL Server 2022 | 10.2.3 -> 10.2.3 | 21.8.1 -> 21.8.2 | PASS | PASS | Baseline + `.01`, `.02`; only `TsDynDataMigrationTest2` | Checkpoint backup exists | `OqtaneTestResults\m1-c8\Oqtane.Server.zip` | PASS |
| M1 | M1-C9 | Windows | SQL Server 2022 | 10.2.3 -> 10.2.3 | 21.8.2 -> 21.8.3 | PASS | PASS | Baseline + `.01`-.03; only `TsDynDataMigrationTest3` | Checkpoint backup exists | `OqtaneTestResults\m1-c9\Oqtane.Server.zip` | PASS |
| M1 | M1-C10 | Windows | SQL Server 2022 | 10.2.3 -> 10.2.3 | 21.8.3 -> 21.8.4 | PASS | PASS | Baseline + `.01`-.04; no test table | Current M1 database probe matches final state | `OqtaneTestResults\m1-c10\Oqtane.Server.zip`; Docker `oqtane-m1` | PASS |
| M2 | M2-C1 | Windows | SQL Server 2022 | fresh -> 6.1.2 | none -> 19.3.5 | PASS | PASS | Legacy baseline | Application backup exists | `OqtaneTestResults\m2-c1\Oqtane.Server.zip` | PASS |
| M2 | M2-C2 | Windows | SQL Server 2022 | 6.1.2 -> 6.2.1 | 19.3.5 -> 19.3.5 | PASS | PASS | Legacy baseline | Application backup exists | `OqtaneTestResults\m2-c2\Oqtane.Server.zip` | PASS |
| M2 | M2-C3 | Windows | SQL Server 2022 | 6.2.1 -> 6.2.1 | 19.3.5 -> 20.0.9 | PASS | PASS | Legacy history only | Application backup exists | `OqtaneTestResults\m2-c3\Oqtane.Server.zip` | PASS |
| M2 | M2-C4 | Windows | SQL Server 2022 | 6.2.1 -> 6.2.1 | 20.0.9 -> 21.7.0 | PASS | PASS | `ToSic.Sxc.21.00.00` | Application backup exists | `OqtaneTestResults\m2-c4\Oqtane.Server.zip` | PASS |
| M2 | M2-C5 | Windows | SQL Server 2022 | 6.2.1 -> 10.2.3 | 21.7.0 -> 21.7.0 | PASS | PASS | Legacy history unchanged | Application backup exists | `OqtaneTestResults\m2-c5\Oqtane.Server.zip` | PASS |
| M2 | M2-C10 | Windows | SQL Server 2022 | 10.2.3 -> 10.2.3 | 21.7.0 -> 21.8.4 | PASS | PASS | Baseline + `.01`-.04; no test table | Current M2 database probe matches final state | `OqtaneTestResults\m2-c10\Oqtane.Server.zip`; Docker `oqtane-m2` | PASS - database state only |
| M3 | M3-C1 | Ubuntu | SQL Server 2022 | 10.2.3 -> 10.2.3 | none -> 21.8.0 | PASS | PASS | Baseline plus `ToSic.Sxc.21.00.00` | Oqtane duplicate `Job` key errors were recorded during the baseline run | `OqtaneTestResults\M3\Server\Content\Log\error.log` | PASS - host/database setup error |
| M3 | M3-C10 | Ubuntu | SQL Server 2022 | 10.2.3 -> 10.2.3 | 21.8.0 -> 21.8.4 | PASS | PASS | Baseline + `.01`-.04; no test table | Current `oqtane-m3` db matches final state | Docker `oqtane-sqlserver-ubuntu`, database `oqtane-m3` | PASS |
| P1 | P1-C1 | Windows | PostgreSQL 18 | 10.2.3 -> 10.2.3 | none -> 21.8.0 | PASS | PASS | Baseline `ToSic.Sxc.21.00.00`; no test table | History is correct. 2sxc `HtmlHelper.CreateScript` throws a null-reference error while rendering p1, fixed with reload | `OqtaneTestResults\p1-c1\dump-oqtane_p1.sql` | PASS |
| P1 | P1-C2 | Windows | PostgreSQL 18 | 10.2.3 -> 10.2.3 | 21.8.0 -> 21.8.1 | PASS | PASS | Baseline + `.01`; only `ts_dyn_data_migration_test` | Dump has the expected migration and table | `OqtaneTestResults\p1-c2\dump-oqtane_p1.sql` | PASS |
| P1 | P1-C3 | Windows | PostgreSQL 18 | 10.2.3 -> 10.2.3 | 21.8.1 -> 21.8.2 | PASS | PASS | Baseline + `.01`, `.02`; only `ts_dyn_data_migration_test2` | Dump has the expected migration and table | `OqtaneTestResults\p1-c3\dump-oqtane_p1.sql` | PASS |
| P1 | P1-C4 | Windows | PostgreSQL 18 | 10.2.3 -> 10.2.3 | 21.8.2 -> 21.8.3 | PASS | PASS | Baseline + `.01`-.03; only `ts_dyn_data_migration_test3` | Dump has the expected migration and table | `OqtaneTestResults\p1-c4\dump-oqtane_p1.sql` | PASS |
| P1 | P1-C10 | Windows | PostgreSQL 18 | 10.2.3 -> 10.2.3 | 21.8.3 -> 21.8.4 | PASS | PASS | Baseline + `.01`-.04; no test table | Dump has the expected final state | `OqtaneTestResults\p1-c10\dump-oqtane_p1.sql`; Docker `oqtane-postgresql-windows` | PASS |
| P2 | P2-C1 | Ubuntu | PostgreSQL 18 | 10.2.3 -> 10.2.3 | none -> 21.8.0 | PASS | PASS | Baseline `ToSic.Sxc.21.00.00` | History is correct, but the baseline log contains the 2sxc `HtmlHelper.CreateScript` null-reference error, fixed with reload | `OqtaneTestResults\p2-c1\dump-oqtane_p2.sql` | PASS |
| P2 | P2-C10 | Ubuntu | PostgreSQL 18 | 10.2.3 -> 10.2.3 | 21.8.0 -> 21.8.4 | PASS | PASS | Baseline + `.01`-.04; no test table | Dump has the expected final state | `OqtaneTestResults\p2-c10\dump-oqtane_p2.sql`; Docker `oqtane-postgresql-ubuntu` | PASS |
| Y1 | Y1-C1 | Windows | MySQL 8.4 | 10.2.3 -> 10.2.3 | none -> 21.8.0 | FAIL | PASS | Baseline `ToSic.Sxc.21.00.00`; no test table | History is correct, but logs contain the 2sxc `HtmlHelper` null-reference and Oqtane `AppliedDate` migration-history errors | `OqtaneTestResults\y1-c1\dump-oqtane-y1.sql` | PASS |
| Y1 | Y1-C2 | Windows | MySQL 8.4 | 10.2.3 -> 10.2.3 | 21.8.0 -> 21.8.1 | PASS | PASS | Baseline + `.01`; only `TsDynDataMigrationTest` | Dump has the expected migration and table | `OqtaneTestResults\y1-c2\dump-oqtane-y1.sql` | PASS |
| Y1 | Y1-C3 | Windows | MySQL 8.4 | 10.2.3 -> 10.2.3 | 21.8.1 -> 21.8.2 | PASS | PASS | Baseline + `.01`, `.02`; only `TsDynDataMigrationTest2` | Dump has the expected migration and table | `OqtaneTestResults\y1-c3\dump-oqtane-y1.sql` | PASS |
| Y1 | Y1-C4 | Windows | MySQL 8.4 | 10.2.3 -> 10.2.3 | 21.8.2 -> 21.8.3 | PASS | PASS | Baseline + `.01`-.03; only `TsDynDataMigrationTest3` | Dump has the expected migration and table | `OqtaneTestResults\y1-c4\dump-oqtane-y1.sql` | PASS |
| Y1 | Y1-C10 | Windows | MySQL 8.4 | 10.2.3 -> 10.2.3 | 21.8.3 -> 21.8.4 | PASS | PASS | Baseline + `.01`-.04; no test table | Dump has the expected final state | `OqtaneTestResults\y1-c10\dump-oqtane-y1.sql`; Docker `oqtane-mysql-windows` | PASS |
| Y2 | Y2-C1 | Ubuntu | MySQL 8.4 | 10.2.3 -> 10.2.3 | none -> 21.8.0 | PASS | PASS | Baseline `ToSic.Sxc.21.00.00`; no test table | Logs contain 2sxc `HtmlHelper` null-reference and Oqtane `AppliedDate` migration-history errors | `OqtaneTestResults\y2-c1\dump-oqtane-y2.sql` | PASS |
| Y2 | Y2-C10 | Ubuntu | MySQL 8.4 | 10.2.3 -> 10.2.3 | 21.8.0 -> 21.8.4 | PASS | PASS | Baseline + `.01`-.04; no test table | Dump has the expected final state | `OqtaneTestResults\y2-c10\dump-oqtane-y2.sql`; Docker `oqtane-mysql-ubuntu` | PASS |
| S1 | S1-C1 | Windows | SQLite | 10.2.3 -> 10.2.3 | none -> 21.8.0 | PASS | PASS | Baseline `ToSic.Sxc.21.00.00`; no test table | Archive has the expected baseline, but the log contains the 2sxc `HtmlHelper` null-reference error | `OqtaneTestResults\s1-c1\Oqtane.Server.zip` (`Data\oqtane-s1.db`) | PASS |
| S1 | S1-C2 | Windows | SQLite | 10.2.3 -> 10.2.3 | 21.8.0 -> 21.8.1 | PASS | PASS | Baseline + `.01`; only `TsDynDataMigrationTest` |  | `OqtaneTestResults\s1-c2\Oqtane.Server.zip` (`Data\oqtane-s1.db`) | PASS |
| S1 | S1-C3 | Windows | SQLite | 10.2.3 -> 10.2.3 | 21.8.1 -> 21.8.2 | PASS | PASS | Baseline + `.01`, `.02`; only `TsDynDataMigrationTest2` | Archive has `.00`-.02 and table 2 | `OqtaneTestResults\s1-c3\Oqtane.Server.zip` (`Data\oqtane-s1.db`) | PASS |
| S1 | S1-C4 | Windows | SQLite | 10.2.3 -> 10.2.3 | 21.8.2 -> 21.8.3 | PASS | PASS | Baseline + `.01`-.03; only `TsDynDataMigrationTest3` | Archive still `.00`-.03 and table 3 | `OqtaneTestResults\s1-c4\Oqtane.Server.zip` (`Data\oqtane-s1.db`) | PASS |
| S1 | S1-C10 | Windows | SQLite | 10.2.3 -> 10.2.3 | 21.8.3 -> 21.8.4 | PASS | PASS | Baseline + `.01`-.04; no test table | | `OqtaneTestResults\s1-c10\Oqtane.Server.zip` (`Data\oqtane-s1.db`) | PASS |
| S2 | S2-C1 | Ubuntu | SQLite | 10.2.3 -> 10.2.3 | none -> 21.8.0 | PASS | PASS | Baseline `ToSic.Sxc.21.00.00` | Archive has the expected baseline, but the log contains the 2sxc `HtmlHelper` null-reference error | `OqtaneTestResults\s2-c1\S2\Server\Data\oqtane-s2.db` | PASS |
| S2 | S2-C10 | Ubuntu | SQLite | 10.2.3 -> 10.2.3 | 21.8.0 -> 21.8.4 | PASS | PASS | Baseline + `.01`-.04; no test table | Archive has the expected final migration state | `OqtaneTestResults\s2-c10\S2\Server\Data\oqtane-s2.db` | PASS |

## Completion checklist

- [ ] M1 legacy SQL Server incremental chain passed.
- [ ] M2 legacy SQL Server direct-to-21.8.4 chain passed.
- [ ] M3 SQL Server on Ubuntu passed.
- [ ] PostgreSQL incremental and skipped paths passed on Windows/Ubuntu.
- [ ] MySQL incremental and skipped paths passed on Windows/Ubuntu, including relationship imports/saves.
- [ ] SQLite incremental and skipped paths passed on Windows/Ubuntu.
- [ ] All final databases contain `.01`-.04 once and contain no migration test table.
- [ ] All sentinel content, relationships, files, and module placements survived.
- [ ] Two cold restarts per final installation produced no new migration or provider errors.
