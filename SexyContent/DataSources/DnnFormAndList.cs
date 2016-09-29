using System;
using ToSic.Eav.DataSources;

namespace ToSic.SexyContent.DataSources
{
    /// <summary>
    /// Delivers UDT-data (now known as Form and List) to the templating engine
    /// </summary>
    [Obsolete("This class was moved to the Environment.Dnn7.DataSources Namespace")]
    public class DnnFormAndList : Environment.Dnn7.DataSources.DnnFormAndList
    {
        // Todo: leave this helper class/message in till 2sxc 09.00, then either extract into an own DLL
        // - we might also write some SQL to update existing pipelines, but it's not likely to have been used much
        // - and otherwise im might be in razor-code, which we couldn't auto-update
    }
}
