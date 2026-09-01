//using Microsoft.EntityFrameworkCore.Infrastructure;
//using Microsoft.EntityFrameworkCore.Migrations;
//using Oqtane.Databases.Interfaces;
//using Oqtane.Migrations;

//namespace ToSic.Sxc.Oqt.Server.Installation.Migrations;

//[DbContext(typeof(SxcDbContext))]
//[Migration(SxcMigrationIds.V21_08_01)]
//[ShowApiWhenReleased(ShowApiMode.Never)]
//public sealed class CreateMigrationTestTable(IDatabase database) : MultiDatabaseMigration(database)
//{
//    protected override void Up(MigrationBuilder migrationBuilder)
//        => MigrationTestTable.Create(migrationBuilder, ActiveDatabase, MigrationTestTable.First);

//    protected override void Down(MigrationBuilder migrationBuilder)
//        => MigrationTestTable.Drop(migrationBuilder, ActiveDatabase, MigrationTestTable.First);
//}

//internal static class MigrationTestTable
//{
//    internal const string First = "TsDynDataMigrationTest";
//    internal const string Second = "TsDynDataMigrationTest2";
//    internal const string Third = "TsDynDataMigrationTest3";

//    internal static void Create(MigrationBuilder migrationBuilder, IDatabase database, string name)
//        => migrationBuilder.CreateTable(
//            name: database.RewriteName(name),
//            columns: table => new
//            {
//                Id = table.Column<int>(name: database.RewriteName("Id"), nullable: false)
//            },
//            constraints: table => table.PrimaryKey(database.RewriteName($"PK_{name}"), row => row.Id));

//    internal static void Drop(MigrationBuilder migrationBuilder, IDatabase database, string name)
//        => migrationBuilder.DropTable(database.RewriteName(name));
//}
