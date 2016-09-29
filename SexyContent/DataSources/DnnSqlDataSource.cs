using System;
using ToSic.Eav.DataSources;

namespace ToSic.SexyContent.DataSources
{
    [Obsolete("This class was moved / to the Environment.Dnn7.DataSources namespace, use that instead.")]
	public class DnnSqlDataSource : Environment.Dnn7.DataSources.DnnSqlDataSource
	{
        // Todo: leave this helper class/message in till 2sxc 09.00, then either extract into an own DLL
        // - we might also write some SQL to update existing pipelines, but it's not likely to have been used much...
        // - and otherwise im might be in razor-code, which we couldn't auto-update

    }
}