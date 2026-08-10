using Microsoft.Data.Sqlite;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Oqtane.Database.MySQL;
using Oqtane.Database.PostgreSQL;
using Oqtane.Database.Sqlite;
using Oqtane.Database.SqlServer;
using Oqtane.Databases.Interfaces;
using Oqtane.Extensions;
using Oqtane.Infrastructure;
using Oqtane.Models;
using Oqtane.Repository;
using ToSic.Eav.Persistence.Efc.Sys.DbContext;
using ToSic.Eav.Persistence.Efc.Sys.DbModels;
using ToSic.Sxc.Oqt.Server.Data;
using ToSic.Sxc.Oqt.Server.Installation.Migrations;
using ToSic.Sys.Configuration;
using ToSic.Sys.Logging;

namespace ToSic.Sxc.Oqt.Server.Tests.Installation.Migrations;

public class InitializeSxcTests
{
    [Theory]
    [InlineData("SqlServer")]
    [InlineData("Sqlite")]
    [InlineData("PostgreSQL")]
    [InlineData("MySQL")]
    public void Up_MatchesEavModel_ForEveryOqtaneDatabase(string databaseName)
    {
        var (database, connectionString) = Database(databaseName);
        using var context = EavContext(database, connectionString);
        var modelTables = context.Model.GetEntityTypes()
            .Select(entityType =>
            {
                var tableName = entityType.GetTableName()!;
                var table = StoreObjectIdentifier.Table(tableName, entityType.GetSchema());
                var columns = entityType.GetProperties()
                    .Select(property => property.GetColumnName(table))
                    .OrderBy(name => name);
                return $"{tableName}:{string.Join(",", columns)}";
            })
            .OrderBy(table => table)
            .ToArray();
        var migrationTables = new InitializeSxc(database).UpOperations
            .OfType<CreateTableOperation>()
            .Select(table => $"{table.Name}:{string.Join(",", table.Columns.Select(column => column.Name).OrderBy(name => name))}")
            .OrderBy(table => table)
            .ToArray();

        Assert.Equal(modelTables, migrationTables);
    }

    [Theory]
    [InlineData("SqlServer", "TsDynDataTargetType")]
    [InlineData("Sqlite", "TsDynDataTargetType")]
    [InlineData("PostgreSQL", "ts_dyn_data_target_type")]
    [InlineData("MySQL", "TsDynDataTargetType")]
    public void EavModel_UsesOqtaneDatabaseNaming(string databaseName, string expectedTableName)
    {
        var (database, connectionString) = Database(databaseName);
        var resolvedDatabase = OqtEavDbContextConfigurator.GetDatabase(database.GetType().AssemblyQualifiedName);
        using var context = EavContext(resolvedDatabase, connectionString);

        var tableName = context.Model
            .FindEntityType(typeof(TsDynDataTargetType))!
            .GetTableName();

        Assert.Equal(expectedTableName, tableName);
    }

    [Fact]
    public void Up_CreatesAndSeedsSqliteDatabase()
    {
        var database = new SqliteDatabase();
        var databaseFile = Path.Combine(Path.GetTempPath(), $"2sxc-{Guid.NewGuid():N}.db");
        try
        {
            var connectionString = $"Data Source={databaseFile}";
            using var migrationContext = MigrationContext(connectionString);
            migrationContext.Database.Migrate();

            var configuration = new GlobalConfiguration();
            configuration.ConnectionString(connectionString);
            using var eavContext = new EavDbContext(
                new DbContextOptionsBuilder<EavDbContext>().Options,
                configuration,
                new LogStoreLive(),
                new TestEavDbContextConfigurator(database));

            Assert.Equal(100, eavContext.TsDynDataTargetTypes.Count());
        }
        finally
        {
            SqliteConnection.ClearAllPools();
            System.IO.File.Delete(databaseFile);
        }
    }

    [Theory]
    [InlineData("SqlServer")]
    [InlineData("Sqlite")]
    [InlineData("PostgreSQL")]
    [InlineData("MySQL")]
    public void Migrations_GenerateSql_ForEveryOqtaneDatabase(string databaseName)
    {
        var (database, connectionString) = Database(databaseName);
        Migration[] migrations =
        [
            new InitializeSxc(database),
            //new CreateMigrationTestTable(database),
            //new ReplaceMigrationTestTable(database),
            //new ReplaceMigrationTest2Table(database),
            //new DropMigrationTest3Table(database)
        ];
        var options = new DbContextOptionsBuilder()
            .UseOqtaneDatabase(database, connectionString)
            .Options;
        using var context = new DbContext(options);

        var commands = context.GetService<IMigrationsSqlGenerator>()
            .Generate(migrations.SelectMany(migration => migration.UpOperations).ToArray());

        Assert.NotEmpty(commands);
    }

    //[Fact]
    //public void MigrationOperations_ReplaceTestTables_WhenAppliedIncrementally()
    //{
    //    var databaseFile = Path.Combine(Path.GetTempPath(), $"2sxc-{Guid.NewGuid():N}.db");
    //    try
    //    {
    //        var connectionString = $"Data Source={databaseFile}";
    //        using var context = MigrationContext(connectionString);
    //        Assert.Equal(
    //            [SxcMigrationIds.Initial, /*SxcMigrationIds.V21_08_01, SxcMigrationIds.V21_08_02, SxcMigrationIds.V21_08_03, SxcMigrationIds.V21_08_04*/],
    //            context.Database.GetMigrations().ToArray());
    //        var generator = context.GetService<IMigrationsSqlGenerator>();
    //        using var connection = new SqliteConnection(connectionString);
    //        connection.Open();

    //        ApplyMigration(generator, new CreateMigrationTestTable(new SqliteDatabase()), connection);
    //        Assert.Equal([MigrationTestTable.First], MigrationTestTables(connection));

    //        ApplyMigration(generator, new ReplaceMigrationTestTable(new SqliteDatabase()), connection);
    //        Assert.Equal([MigrationTestTable.Second], MigrationTestTables(connection));

    //        ApplyMigration(generator, new ReplaceMigrationTest2Table(new SqliteDatabase()), connection);
    //        Assert.Equal([MigrationTestTable.Third], MigrationTestTables(connection));

    //        ApplyMigration(generator, new DropMigrationTest3Table(new SqliteDatabase()), connection);
    //        Assert.Empty(MigrationTestTables(connection));
    //    }
    //    finally
    //    {
    //        SqliteConnection.ClearAllPools();
    //        System.IO.File.Delete(databaseFile);
    //    }
    //}

    private static (IDatabase Database, string ConnectionString) Database(string databaseName)
        => databaseName switch
        {
            "SqlServer" => (new SqlServerDatabase(), "Server=(local);Database=2sxc-test;Trusted_Connection=True;TrustServerCertificate=True"),
            "Sqlite" => (new SqliteDatabase(), "Data Source=:memory:"),
            "PostgreSQL" => (new PostgreSQLDatabase(), "Host=localhost;Database=2sxc_test;Username=test;Password=test"),
            "MySQL" => (new MySQLDatabase(), "Server=localhost;Database=2sxc_test;User=test;Password=test"),
            _ => throw new ArgumentOutOfRangeException(nameof(databaseName), databaseName, null)
        };

    private static EavDbContext EavContext(IDatabase database, string connectionString)
    {
        var configuration = new GlobalConfiguration();
        configuration.ConnectionString(connectionString);
        return new(
            new DbContextOptionsBuilder<EavDbContext>().Options,
            configuration,
            new LogStoreLive(),
            new TestEavDbContextConfigurator(database));
    }

    private static SxcDbContext MigrationContext(string connectionString)
    {
        var database = new SqliteDatabase();
        var tenant = new Tenant
        {
            TenantId = 1,
            DBConnectionString = "Test",
            DBType = database.GetType().AssemblyQualifiedName
        };
        var configurationRoot = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Test"] = connectionString
            })
            .Build();
        var dependencies = new DBContextDependencies(
            new TestTenantManager(tenant),
            new HttpContextAccessor(),
            configurationRoot);
        return new(dependencies);
    }

    private static void ApplyMigration(IMigrationsSqlGenerator generator, Migration migration, SqliteConnection connection)
    {
        foreach (var migrationCommand in generator.Generate(migration.UpOperations))
        {
            using var command = connection.CreateCommand();
            command.CommandText = migrationCommand.CommandText;
            command.ExecuteNonQuery();
        }
    }

    private static string[] MigrationTestTables(SqliteConnection connection)
    {
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT name FROM sqlite_master WHERE type = 'table' AND name LIKE 'TsDynDataMigrationTest%' ORDER BY name";
        using var reader = command.ExecuteReader();
        var tables = new List<string>();
        while (reader.Read())
            tables.Add(reader.GetString(0));
        return tables.ToArray();
    }

    private sealed class TestEavDbContextConfigurator(IDatabase database) : IEavDbContextConfigurator
    {
        public void Configure(DbContextOptionsBuilder optionsBuilder, string connectionString)
            => optionsBuilder.UseOqtaneDatabase(database, connectionString);

        public string RewriteName(string name) => database.RewriteName(name);
    }

    private sealed class TestTenantManager(Tenant tenant) : ITenantManager
    {
        public Alias GetAlias() => null!;

        public Tenant GetTenant() => tenant;

        public void SetAlias(Alias alias) { }

        public void SetAlias(int tenantId, int siteId) { }

        public void SetTenant(int tenantId) { }
    }
}
