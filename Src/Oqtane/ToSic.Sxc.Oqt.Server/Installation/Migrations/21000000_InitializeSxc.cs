using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;
using Oqtane.Databases.Interfaces;
using Oqtane.Migrations;
using System.Globalization;
using System.Linq.Expressions;

namespace ToSic.Sxc.Oqt.Server.Installation.Migrations;

[DbContext(typeof(SxcDbContext))]
[Migration(SxcMigrationIds.Initial)]
[ShowApiWhenReleased(ShowApiMode.Never)]
public sealed class InitializeSxc(IDatabase database) : MultiDatabaseMigration(database)
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        CreateTransaction(migrationBuilder);
        CreateZone(migrationBuilder);
        CreateApp(migrationBuilder);
        CreateAttributeType(migrationBuilder);
        CreateTargetType(migrationBuilder);
        CreateContentType(migrationBuilder);
        CreateAttribute(migrationBuilder);
        CreateDimension(migrationBuilder);
        CreateEntity(migrationBuilder);
        CreateHistory(migrationBuilder);
        CreateRelationship(migrationBuilder);
        CreateValue(migrationBuilder);
        CreateValueDimension(migrationBuilder);
        CreateIndexes(migrationBuilder);
        SeedData(migrationBuilder);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        foreach (var table in new[]
                 {
                     "TsDynDataValueDimension",
                     "TsDynDataValue",
                     "TsDynDataRelationship",
                     "TsDynDataHistory",
                     "TsDynDataEntity",
                     "TsDynDataDimension",
                     "TsDynDataAttribute",
                     "TsDynDataContentType",
                     "TsDynDataTargetType",
                     "TsDynDataAttributeType",
                     "TsDynDataApp",
                     "TsDynDataZone",
                     "TsDynDataTransaction"
                 })
            migrationBuilder.DropTable(Name(table));
    }

    private void CreateTransaction(MigrationBuilder migrationBuilder)
        => migrationBuilder.CreateTable(
            name: Name("TsDynDataTransaction"),
            columns: table => new
            {
                TransactionId = Identity(table, "TransactionId"),
                Timestamp = table.Column<DateTime>(name: Name("Timestamp"), nullable: false),
                User = table.Column<string>(name: Name("User"), maxLength: 255, nullable: true)
            },
            constraints: table => table.PrimaryKey(Name("PK_TsDynDataTransaction"), x => x.TransactionId));

    private void CreateZone(MigrationBuilder migrationBuilder)
        => migrationBuilder.CreateTable(
            name: Name("TsDynDataZone"),
            columns: table => new
            {
                ZoneId = Identity(table, "ZoneId"),
                Name = table.Column<string>(name: Name("Name"), maxLength: 255, nullable: false),
                TransCreatedId = table.Column<int>(name: Name("TransCreatedId"), nullable: true),
                TransModifiedId = table.Column<int>(name: Name("TransModifiedId"), nullable: true),
                TransDeletedId = table.Column<int>(name: Name("TransDeletedId"), nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey(Name("PK_TsDynDataZone"), x => x.ZoneId);
                TransactionForeignKeys(table, "TsDynDataZone", x => x.TransCreatedId, x => x.TransModifiedId, x => x.TransDeletedId);
            });

    private void CreateApp(MigrationBuilder migrationBuilder)
        => migrationBuilder.CreateTable(
            name: Name("TsDynDataApp"),
            columns: table => new
            {
                AppId = Identity(table, "AppId"),
                ZoneId = table.Column<int>(name: Name("ZoneId"), nullable: false),
                Name = table.Column<string>(name: Name("Name"), maxLength: 255, nullable: false),
                SysSettings = table.Column<string>(name: Name("SysSettings"), nullable: true),
                TransCreatedId = table.Column<int>(name: Name("TransCreatedId"), nullable: true),
                TransModifiedId = table.Column<int>(name: Name("TransModifiedId"), nullable: true),
                TransDeletedId = table.Column<int>(name: Name("TransDeletedId"), nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey(Name("PK_TsDynDataApp"), x => x.AppId);
                table.ForeignKey(
                    Name("FK_TsDynDataApp_TsDynDataZone"),
                    x => x.ZoneId,
                    Name("TsDynDataZone"),
                    Name("ZoneId"),
                    onDelete: ReferentialAction.Restrict);
                TransactionForeignKeys(table, "TsDynDataApp", x => x.TransCreatedId, x => x.TransModifiedId, x => x.TransDeletedId);
            });

    private void CreateAttributeType(MigrationBuilder migrationBuilder)
        => migrationBuilder.CreateTable(
            name: Name("TsDynDataAttributeType"),
            columns: table => new
            {
                Type = table.Column<string>(name: Name("Type"), maxLength: 50, nullable: false)
            },
            constraints: table => table.PrimaryKey(Name("PK_TsDynDataAttributeType"), x => x.Type));

    private void CreateTargetType(MigrationBuilder migrationBuilder)
        => migrationBuilder.CreateTable(
            name: Name("TsDynDataTargetType"),
            columns: table => new
            {
                TargetTypeId = Identity(table, "TargetTypeId"),
                Name = table.Column<string>(name: Name("Name"), maxLength: 50, nullable: false),
                Description = table.Column<string>(name: Name("Description"), nullable: false)
            },
            constraints: table => table.PrimaryKey(Name("PK_TsDynDataTargetType"), x => x.TargetTypeId));

    private void CreateContentType(MigrationBuilder migrationBuilder)
        => migrationBuilder.CreateTable(
            name: Name("TsDynDataContentType"),
            columns: table => new
            {
                ContentTypeId = Identity(table, "ContentTypeId"),
                StaticName = table.Column<string>(name: Name("StaticName"), maxLength: 150, nullable: false),
                Name = table.Column<string>(name: Name("Name"), maxLength: 150, nullable: false),
                Scope = table.Column<string>(name: Name("Scope"), maxLength: 50, nullable: true),
                TransCreatedId = table.Column<int>(name: Name("TransCreatedId"), nullable: false),
                TransModifiedId = table.Column<int>(name: Name("TransModifiedId"), nullable: true),
                TransDeletedId = table.Column<int>(name: Name("TransDeletedId"), nullable: true),
                AppId = table.Column<int>(name: Name("AppId"), nullable: false),
                InheritContentTypeId = table.Column<int>(name: Name("InheritContentTypeId"), nullable: true),
                IsGlobal = table.Column<bool>(name: Name("IsGlobal"), nullable: false, defaultValue: false),
                SysSettings = table.Column<string>(name: Name("SysSettings"), nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey(Name("PK_TsDynDataContentType"), x => x.ContentTypeId);
                table.ForeignKey(
                    Name("FK_TsDynDataContentType_TsDynDataApp"),
                    x => x.AppId,
                    Name("TsDynDataApp"),
                    Name("AppId"),
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    Name("FK_TsDynDataContentType_TsDynDataContentType"),
                    x => x.InheritContentTypeId,
                    Name("TsDynDataContentType"),
                    Name("ContentTypeId"),
                    onDelete: ReferentialAction.Restrict);
                TransactionForeignKeys(table, "TsDynDataContentType", x => x.TransCreatedId, x => x.TransModifiedId, x => x.TransDeletedId);
            });

    private void CreateAttribute(MigrationBuilder migrationBuilder)
        => migrationBuilder.CreateTable(
            name: Name("TsDynDataAttribute"),
            columns: table => new
            {
                AttributeId = Identity(table, "AttributeId"),
                StaticName = table.Column<string>(name: Name("StaticName"), maxLength: 50, nullable: false),
                Type = table.Column<string>(name: Name("Type"), maxLength: 50, nullable: false),
                Guid = table.Column<Guid>(name: Name("Guid"), nullable: true),
                SysSettings = table.Column<string>(name: Name("SysSettings"), nullable: true),
                ContentTypeId = table.Column<int>(name: Name("ContentTypeId"), nullable: false),
                SortOrder = table.Column<int>(name: Name("SortOrder"), nullable: false, defaultValue: 0),
                IsTitle = table.Column<bool>(name: Name("IsTitle"), nullable: false, defaultValue: false),
                TransCreatedId = table.Column<int>(name: Name("TransCreatedId"), nullable: false),
                TransModifiedId = table.Column<int>(name: Name("TransModifiedId"), nullable: true),
                TransDeletedId = table.Column<int>(name: Name("TransDeletedId"), nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey(Name("PK_TsDynDataAttribute"), x => x.AttributeId);
                table.ForeignKey(
                    Name("FK_TsDynDataAttribute_TsDynDataAttributeType"),
                    x => x.Type,
                    Name("TsDynDataAttributeType"),
                    Name("Type"),
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    Name("FK_TsDynDataAttribute_TsDynDataContentType"),
                    x => x.ContentTypeId,
                    Name("TsDynDataContentType"),
                    Name("ContentTypeId"),
                    onDelete: ReferentialAction.Restrict);
                TransactionForeignKeys(table, "TsDynDataAttribute", x => x.TransCreatedId, x => x.TransModifiedId, x => x.TransDeletedId);
            });

    private void CreateDimension(MigrationBuilder migrationBuilder)
        => migrationBuilder.CreateTable(
            name: Name("TsDynDataDimension"),
            columns: table => new
            {
                DimensionId = Identity(table, "DimensionId"),
                Parent = table.Column<int>(name: Name("Parent"), nullable: true),
                Name = table.Column<string>(name: Name("Name"), maxLength: 100, nullable: false),
                SystemKey = table.Column<string>(name: Name("SystemKey"), maxLength: 100, nullable: true),
                ExternalKey = table.Column<string>(name: Name("ExternalKey"), maxLength: 100, nullable: true),
                Active = table.Column<bool>(name: Name("Active"), nullable: false, defaultValue: true),
                ZoneId = table.Column<int>(name: Name("ZoneId"), nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey(Name("PK_TsDynDataDimension"), x => x.DimensionId);
                table.ForeignKey(
                    Name("FK_TsDynDataDimension_TsDynDataDimension"),
                    x => x.Parent,
                    Name("TsDynDataDimension"),
                    Name("DimensionId"),
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    Name("FK_TsDynDataDimension_TsDynDataZone"),
                    x => x.ZoneId,
                    Name("TsDynDataZone"),
                    Name("ZoneId"),
                    onDelete: ReferentialAction.Restrict);
            });

    private void CreateEntity(MigrationBuilder migrationBuilder)
        => migrationBuilder.CreateTable(
            name: Name("TsDynDataEntity"),
            columns: table => new
            {
                EntityId = Identity(table, "EntityId"),
                EntityGuid = table.Column<Guid>(name: Name("EntityGuid"), nullable: false),
                ContentTypeId = table.Column<int>(name: Name("ContentTypeId"), nullable: false),
                TargetTypeId = table.Column<int>(name: Name("TargetTypeId"), nullable: false),
                KeyNumber = table.Column<int>(name: Name("KeyNumber"), nullable: true),
                KeyGuid = table.Column<Guid>(name: Name("KeyGuid"), nullable: true),
                KeyString = table.Column<string>(name: Name("KeyString"), maxLength: 100, nullable: true),
                IsPublished = table.Column<bool>(name: Name("IsPublished"), nullable: false, defaultValue: true),
                PublishedEntityId = table.Column<int>(name: Name("PublishedEntityId"), nullable: true),
                Owner = table.Column<string>(name: Name("Owner"), maxLength: 250, nullable: true),
                Json = table.Column<string>(name: Name("Json"), nullable: true),
                Version = table.Column<int>(name: Name("Version"), nullable: false, defaultValue: 1),
                AppId = table.Column<int>(name: Name("AppId"), nullable: false),
                ContentType = table.Column<string>(name: Name("ContentType"), maxLength: 250, nullable: true),
                TransCreatedId = table.Column<int>(name: Name("TransCreatedId"), nullable: false),
                TransModifiedId = table.Column<int>(name: Name("TransModifiedId"), nullable: false),
                TransDeletedId = table.Column<int>(name: Name("TransDeletedId"), nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey(Name("PK_TsDynDataEntity"), x => x.EntityId);
                table.ForeignKey(Name("FK_TsDynDataEntity_TsDynDataApp"), x => x.AppId, Name("TsDynDataApp"), Name("AppId"), onDelete: ReferentialAction.Restrict);
                table.ForeignKey(Name("FK_TsDynDataEntity_TsDynDataContentType"), x => x.ContentTypeId, Name("TsDynDataContentType"), Name("ContentTypeId"), onDelete: ReferentialAction.Restrict);
                table.ForeignKey(Name("FK_TsDynDataEntity_TsDynDataTargetType"), x => x.TargetTypeId, Name("TsDynDataTargetType"), Name("TargetTypeId"), onDelete: ReferentialAction.Restrict);
                TransactionForeignKeys(table, "TsDynDataEntity", x => x.TransCreatedId, x => x.TransModifiedId, x => x.TransDeletedId);
            });

    private void CreateHistory(MigrationBuilder migrationBuilder)
        => migrationBuilder.CreateTable(
            name: Name("TsDynDataHistory"),
            columns: table => new
            {
                HistoryId = Identity(table, "HistoryId"),
                SourceTable = table.Column<string>(name: Name("SourceTable"), maxLength: 250, nullable: false),
                SourceId = table.Column<int>(name: Name("SourceId"), nullable: true),
                SourceGuid = table.Column<Guid>(name: Name("SourceGuid"), nullable: true),
                Operation = table.Column<string>(name: Name("Operation"), maxLength: 1, nullable: false, fixedLength: true, defaultValue: "I"),
                Timestamp = table.Column<DateTime>(name: Name("Timestamp"), nullable: false),
                TransactionId = table.Column<int>(name: Name("TransactionId"), nullable: true),
                ParentRef = table.Column<string>(name: Name("ParentRef"), maxLength: 250, nullable: true),
                Json = table.Column<string>(name: Name("Json"), nullable: true),
                CJson = table.Column<byte[]>(name: Name("CJson"), nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey(Name("PK_TsDynDataHistory"), x => x.HistoryId);
                table.ForeignKey(
                    Name("FK_TsDynDataHistory_TsDynDataTransaction"),
                    x => x.TransactionId,
                    Name("TsDynDataTransaction"),
                    Name("TransactionId"),
                    onDelete: ReferentialAction.Restrict);
            });

    private void CreateRelationship(MigrationBuilder migrationBuilder)
        => migrationBuilder.CreateTable(
            name: Name("TsDynDataRelationship"),
            columns: table => new
            {
                AttributeId = table.Column<int>(name: Name("AttributeId"), nullable: false),
                ParentEntityId = table.Column<int>(name: Name("ParentEntityId"), nullable: false),
                ChildEntityId = table.Column<int>(name: Name("ChildEntityId"), nullable: true),
                ChildExternalId = table.Column<Guid>(name: Name("ChildExternalId"), nullable: true),
                SortOrder = table.Column<int>(name: Name("SortOrder"), nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey(Name("PK_TsDynDataRelationship"), x => new { x.AttributeId, x.ParentEntityId, x.SortOrder });
                table.ForeignKey(Name("FK_TsDynDataRelationship_TsDynDataAttribute"), x => x.AttributeId, Name("TsDynDataAttribute"), Name("AttributeId"), onDelete: ReferentialAction.Cascade);
                table.ForeignKey(Name("FK_TsDynDataRelationship_TsDynDataEntityChild"), x => x.ChildEntityId, Name("TsDynDataEntity"), Name("EntityId"), onDelete: ReferentialAction.Restrict);
                table.ForeignKey(Name("FK_TsDynDataRelationship_TsDynDataEntityParent"), x => x.ParentEntityId, Name("TsDynDataEntity"), Name("EntityId"), onDelete: ReferentialAction.Restrict);
            });

    private void CreateValue(MigrationBuilder migrationBuilder)
        => migrationBuilder.CreateTable(
            name: Name("TsDynDataValue"),
            columns: table => new
            {
                ValueId = Identity(table, "ValueId"),
                EntityId = table.Column<int>(name: Name("EntityId"), nullable: false),
                AttributeId = table.Column<int>(name: Name("AttributeId"), nullable: false),
                Value = table.Column<string>(name: Name("Value"), nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey(Name("PK_TsDynDataValue"), x => x.ValueId);
                table.ForeignKey(Name("FK_TsDynDataValue_TsDynDataAttribute"), x => x.AttributeId, Name("TsDynDataAttribute"), Name("AttributeId"), onDelete: ReferentialAction.Cascade);
                table.ForeignKey(Name("FK_TsDynDataValue_TsDynDataEntity"), x => x.EntityId, Name("TsDynDataEntity"), Name("EntityId"), onDelete: ReferentialAction.Restrict);
            });

    private void CreateValueDimension(MigrationBuilder migrationBuilder)
        => migrationBuilder.CreateTable(
            name: Name("TsDynDataValueDimension"),
            columns: table => new
            {
                ValueId = table.Column<int>(name: Name("ValueId"), nullable: false),
                DimensionId = table.Column<int>(name: Name("DimensionId"), nullable: false),
                ReadOnly = table.Column<bool>(name: Name("ReadOnly"), nullable: false, defaultValue: false)
            },
            constraints: table =>
            {
                table.PrimaryKey(Name("PK_TsDynDataValueDimension"), x => new { x.ValueId, x.DimensionId });
                table.ForeignKey(Name("FK_TsDynDataValueDimension_TsDynDataDimension"), x => x.DimensionId, Name("TsDynDataDimension"), Name("DimensionId"), onDelete: ReferentialAction.Cascade);
                table.ForeignKey(Name("FK_TsDynDataValueDimension_TsDynDataValue"), x => x.ValueId, Name("TsDynDataValue"), Name("ValueId"), onDelete: ReferentialAction.Cascade);
            });

    private void CreateIndexes(MigrationBuilder migrationBuilder)
    {
        Index(migrationBuilder, "UQ_TsDynDataApp_Name_ZoneId", "TsDynDataApp", ["Name", "ZoneId"], unique: true);
        TransactionIndexes(migrationBuilder, "TsDynDataApp");
        Index(migrationBuilder, "IX_TsDynDataApp_ZoneId", "TsDynDataApp", ["ZoneId"]);

        Index(migrationBuilder, "IX_TsDynDataAttribute_AttributeId_StaticName", "TsDynDataAttribute", ["AttributeId", "StaticName"]);
        Index(migrationBuilder, "IX_TsDynDataAttribute_ContentTypeId", "TsDynDataAttribute", ["ContentTypeId"]);
        TransactionIndexes(migrationBuilder, "TsDynDataAttribute");

        Index(migrationBuilder, "IX_TsDynDataContentType_AppId", "TsDynDataContentType", ["AppId"]);
        Index(migrationBuilder, "IX_TsDynDataContentType_InheritContentTypeId", "TsDynDataContentType", ["InheritContentTypeId"]);
        TransactionIndexes(migrationBuilder, "TsDynDataContentType");

        Index(migrationBuilder, "IX_TsDynDataDimension_Parent", "TsDynDataDimension", ["Parent"]);
        Index(migrationBuilder, "IX_TsDynDataDimension_ZoneId", "TsDynDataDimension", ["ZoneId"]);

        Index(migrationBuilder, "IX_TsDynDataEntity_AppId", "TsDynDataEntity", ["AppId"]);
        Index(migrationBuilder, "IX_TsDynDataEntity_ContentTypeId", "TsDynDataEntity", ["ContentTypeId"]);
        Index(migrationBuilder, "IX_TsDynDataEntity_KeyNumber", "TsDynDataEntity", ["KeyNumber"]);
        Index(migrationBuilder, "IX_TsDynDataEntity_TargetTypeId", "TsDynDataEntity", ["TargetTypeId"]);
        TransactionIndexes(migrationBuilder, "TsDynDataEntity");

        Index(migrationBuilder, "IX_TsDynDataHistory_SourceGuid", "TsDynDataHistory", ["SourceGuid"]);
        Index(migrationBuilder, "IX_TsDynDataHistory_SourceId", "TsDynDataHistory", ["SourceId"]);
        Index(migrationBuilder, "IX_TsDynDataHistory_ParentRef", "TsDynDataHistory", ["ParentRef"]);
        Index(migrationBuilder, "IX_TsDynDataHistory_TransactionId", "TsDynDataHistory", ["TransactionId"]);

        Index(migrationBuilder, "IX_TsDynDataRelationship_ChildEntityId", "TsDynDataRelationship", ["ChildEntityId"]);
        Index(migrationBuilder, "IX_TsDynDataRelationship_ParentEntityId", "TsDynDataRelationship", ["ParentEntityId"]);
        Index(migrationBuilder, "IX_TsDynDataTargetType_Name", "TsDynDataTargetType", ["Name"]);

        Index(migrationBuilder, "IX_TsDynDataValue_AttributeId", "TsDynDataValue", ["AttributeId"]);
        Index(migrationBuilder, "IX_TsDynDataValue_AttributeId_EntityId", "TsDynDataValue", ["AttributeId", "EntityId"]);
        Index(migrationBuilder, "IX_TsDynDataValue_EntityId", "TsDynDataValue", ["EntityId"]);
        Index(migrationBuilder, "IX_TsDynDataValue_EntityId_AttributeId_ValueId", "TsDynDataValue", ["EntityId", "AttributeId", "ValueId"]);
        Index(migrationBuilder, "IX_TsDynDataValueDimension_DimensionId", "TsDynDataValueDimension", ["DimensionId"]);
        TransactionIndexes(migrationBuilder, "TsDynDataZone");
    }

    private void SeedData(MigrationBuilder migrationBuilder)
    {
        Insert(migrationBuilder, "TsDynDataTransaction", ["TransactionId", "Timestamp", "User"], new object[,]
        {
            { 1, new DateTime(2012, 5, 2, 8, 31, 35, 297, DateTimeKind.Utc), null! },
            { 100, new DateTime(2020, 10, 20, 0, 0, 0, DateTimeKind.Utc), null! }
        }, identity: true);
        Insert(migrationBuilder, "TsDynDataZone", ["ZoneId", "Name", "TransCreatedId", "TransModifiedId", "TransDeletedId"], new object[,]
        {
            { 1, "Default", null!, null!, null! }
        }, identity: true);
        Insert(migrationBuilder, "TsDynDataApp", ["AppId", "ZoneId", "Name", "SysSettings", "TransCreatedId", "TransModifiedId", "TransDeletedId"], new object[,]
        {
            { 1, 1, "Default", null!, null!, null!, null! }
        }, identity: true);
        Insert(migrationBuilder, "TsDynDataContentType", ["ContentTypeId", "StaticName", "Name", "Scope", "TransCreatedId", "TransDeletedId", "AppId", "InheritContentTypeId", "IsGlobal", "SysSettings", "TransModifiedId"], new object[,]
        {
            { 1, "Default", "Default (built in)", "2SexyContent-System", 1, null!, 1, null!, true, null!, null! }
        }, identity: true);
        Insert(migrationBuilder, "TsDynDataDimension", ["DimensionId", "Parent", "Name", "SystemKey", "ExternalKey", "Active", "ZoneId"], new object[,]
        {
            { 1, null!, "Culture Root", "Culture", null!, true, 1 }
        }, identity: true);

        Insert(migrationBuilder, "TsDynDataAttributeType", ["Type"], new object[,]
        {
            { "Boolean" }, { "Custom" }, { "DateTime" }, { "Empty" },
            { "Entity" }, { "Hyperlink" }, { "Number" }, { "String" }
        });

        var targetTypes = new object[100, 3];
        string[] standardNames =
        [
            "Default", "EAV Field Properties", "App", "Entity", "ContentType", "Zone",
            "Scope", "Dimension", "Reserved", "CmsObject", "System", "Site", "SiteVariant",
            "Page", "PageVariant", "Module", "ModuleVariant", "User"
        ];
        for (var id = 1; id <= targetTypes.GetLength(0); id++)
        {
            var name = id <= standardNames.Length
                ? standardNames[id - 1]
                : id is >= 90 and <= 99
                    ? id == 90 ? "Custom" : $"Custom{id - 90}"
                    : "Reserved";
            var description = id switch
            {
                4 => "For Permissions, Data Pipelines with Pipeline Parts and Configurations",
                10 => "References to CMS objects like files and pages",
                _ => name
            };
            targetTypes[id - 1, 0] = id;
            targetTypes[id - 1, 1] = name;
            targetTypes[id - 1, 2] = description;
        }
        Insert(migrationBuilder, "TsDynDataTargetType", ["TargetTypeId", "Name", "Description"], targetTypes, identity: true);
    }

    private OperationBuilder<Microsoft.EntityFrameworkCore.Migrations.Operations.AddColumnOperation> Identity(ColumnsBuilder table, string name)
        => ActiveDatabase.AddAutoIncrementColumn(table, Name(name));

    private string Name(string name) => ActiveDatabase.RewriteName(name);

    private void Index(MigrationBuilder migrationBuilder, string index, string table, string[] columns, bool unique = false)
        => migrationBuilder.CreateIndex(Name(index), Name(table), columns.Select(Name).ToArray(), unique: unique);

    private void Insert(MigrationBuilder migrationBuilder, string table, string[] columns, object[,] values, bool identity = false)
    {
        var tableName = ActiveDatabase.DelimitName(Name(table));
        var columnNames = string.Join(", ", columns.Select(column => ActiveDatabase.DelimitName(Name(column))));
        var rows = Enumerable.Range(0, values.GetLength(0))
            .Select(row => $"({string.Join(", ", Enumerable.Range(0, values.GetLength(1)).Select(column => SqlValue(values[row, column])))})");
        var identityOverride = identity && ActiveDatabase.Name == "PostgreSQL"
            ? " OVERRIDING SYSTEM VALUE"
            : "";
        var sql = $"INSERT INTO {tableName} ({columnNames}){identityOverride} VALUES {string.Join(", ", rows)};";

        if (identity && ActiveDatabase.Name == "SqlServer")
            sql = $"SET IDENTITY_INSERT {tableName} ON;\n{sql}\nSET IDENTITY_INSERT {tableName} OFF;";

        if (identity && ActiveDatabase.Name == "PostgreSQL")
        {
            var nextId = Enumerable.Range(0, values.GetLength(0))
                .Max(row => Convert.ToInt32(values[row, 0], CultureInfo.InvariantCulture)) + 1;
            var identityColumn = ActiveDatabase.DelimitName(Name(columns[0]));
            sql += $"\nALTER TABLE {tableName} ALTER COLUMN {identityColumn} RESTART WITH {nextId};";
        }

        migrationBuilder.Sql(sql);
    }

    private string SqlValue(object value)
        => value switch
        {
            null => "NULL",
            string text => $"'{text.Replace("'", "''")}'",
            DateTime dateTime => $"'{dateTime.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture)}'",
            bool boolean => ActiveDatabase.RewriteValue(boolean),
            IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
            _ => ActiveDatabase.RewriteValue(value)
        };

    private void TransactionIndexes(MigrationBuilder migrationBuilder, string table)
    {
        Index(migrationBuilder, $"IX_{table}_TransCreatedId", table, ["TransCreatedId"]);
        Index(migrationBuilder, $"IX_{table}_TransModifiedId", table, ["TransModifiedId"]);
        Index(migrationBuilder, $"IX_{table}_TransDeletedId", table, ["TransDeletedId"]);
    }

    private void TransactionForeignKeys<TColumns>(
        CreateTableBuilder<TColumns> table,
        string tableName,
        Expression<Func<TColumns, object>> created,
        Expression<Func<TColumns, object>> modified,
        Expression<Func<TColumns, object>> deleted)
    {
        table.ForeignKey(Name($"FK_{tableName}_TsDynDataTransactionCreated"), created, Name("TsDynDataTransaction"), Name("TransactionId"), onDelete: ReferentialAction.Restrict);
        table.ForeignKey(Name($"FK_{tableName}_TsDynDataTransactionModified"), modified, Name("TsDynDataTransaction"), Name("TransactionId"), onDelete: ReferentialAction.Restrict);
        table.ForeignKey(Name($"FK_{tableName}_TsDynDataTransactionDeleted"), deleted, Name("TsDynDataTransaction"), Name("TransactionId"), onDelete: ReferentialAction.Restrict);
    }
}
