//using Microsoft.EntityFrameworkCore.Infrastructure;
//using Microsoft.EntityFrameworkCore.Migrations;
//using Oqtane.Databases.Interfaces;
//using Oqtane.Migrations;

//namespace ToSic.Sxc.Oqt.Server.Installation.Migrations;

//[DbContext(typeof(SxcDbContext))]
//[Migration(SxcMigrationIds.V21_08_03)]
//[ShowApiWhenReleased(ShowApiMode.Never)]
//public sealed class ReplaceMigrationTest2Table(IDatabase database) : MultiDatabaseMigration(database)
//{
//    protected override void Up(MigrationBuilder migrationBuilder)
//    {
//        MigrationTestTable.Drop(migrationBuilder, ActiveDatabase, MigrationTestTable.Second);
//        MigrationTestTable.Create(migrationBuilder, ActiveDatabase, MigrationTestTable.Third);
//    }

//    protected override void Down(MigrationBuilder migrationBuilder)
//    {
//        MigrationTestTable.Drop(migrationBuilder, ActiveDatabase, MigrationTestTable.Third);
//        MigrationTestTable.Create(migrationBuilder, ActiveDatabase, MigrationTestTable.Second);
//    }
//}
