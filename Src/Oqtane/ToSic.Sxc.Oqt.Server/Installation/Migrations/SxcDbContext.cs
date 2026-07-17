using Oqtane.Repository;
using Oqtane.Repository.Databases.Interfaces;

namespace ToSic.Sxc.Oqt.Server.Installation.Migrations;

[ShowApiWhenReleased(ShowApiMode.Never)]
public sealed class SxcDbContext(IDBContextDependencies dependencies)
    : DBContextBase(dependencies), IMultiDatabase;
