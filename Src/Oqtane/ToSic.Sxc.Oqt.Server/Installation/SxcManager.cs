using Oqtane.Infrastructure;
using Oqtane.Interfaces;
using Oqtane.Models;
using Oqtane.Repository;
using System;
using System.Threading.Tasks;
using Oqtane.Modules;
using ToSic.Lib.DI;
using ToSic.Sxc.Oqt.Server.Search;
using ToSic.Sxc.Search;
using IModule = ToSic.Sxc.Context.IModule;

namespace ToSic.Sxc.Oqt.Server.Installation;

/// <summary>
/// This is probably some kind of installer-class.
/// </summary>
/// <remarks>
/// WARNING: Careful when renaming / moving, the name is listed in the ModuleInfo.cs in the Client.
/// </remarks>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal partial class SxcManager(
    ISqlRepository sql,
    Generator<SearchController> searchControllerGenerator,
    Generator<IModule> moduleGenerator) /*: MigratableModuleBase*/
{ }