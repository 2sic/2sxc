﻿using ToSic.Eav.DataSources;

// ReSharper disable once CheckNamespace
namespace ToSic.SexyContent.Environment.Dnn7.DataSources;

// Todo: leave this helper class/message in till 2sxc 09.00, then either extract into an own DLL
// - we might also write some SQL to update existing pipelines, but it's not likely to have been used much...
// - and otherwise im might be in razor-code, which we couldn't auto-update

[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
[Obsolete("This class was moved / to ToSic.Sxc.Dnn.DataSources.DnnSql, use that instead.")]
public class DnnSqlDataSource(Sql.MyServices services)
    : Sxc.Dnn.DataSources.DnnSql(services);