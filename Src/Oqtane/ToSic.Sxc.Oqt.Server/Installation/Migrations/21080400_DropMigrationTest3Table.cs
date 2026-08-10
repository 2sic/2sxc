//using Microsoft.EntityFrameworkCore.Infrastructure;
//using Microsoft.EntityFrameworkCore.Migrations;
//using Oqtane.Databases.Interfaces;
//using Oqtane.Migrations;

//namespace ToSic.Sxc.Oqt.Server.Installation.Migrations;

//[DbContext(typeof(SxcDbContext))]
//[Migration(SxcMigrationIds.V21_08_04)]
//[ShowApiWhenReleased(ShowApiMode.Never)]
//public sealed class DropMigrationTest3Table(IDatabase database) : MultiDatabaseMigration(database)
//{
//    protected override void Up(MigrationBuilder migrationBuilder)
//        => MigrationTestTable.Drop(migrationBuilder, ActiveDatabase, MigrationTestTable.Third);

//    protected override void Down(MigrationBuilder migrationBuilder)
//        => MigrationTestTable.Create(migrationBuilder, ActiveDatabase, MigrationTestTable.Third);
//}
