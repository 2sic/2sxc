using System.Collections.Generic;
using ToSic.Eav.Documentation;

// ReSharper disable once CheckNamespace
namespace ToSic.Eav.Context
{
    // ReSharper disable once PossibleInterfaceMemberAmbiguity
    public interface IPage : IPageLight
    {
        /// <summary>
        /// These parameters can reconfigure what view is used or change
        /// </summary>
        [PrivateApi("wip")] List<KeyValuePair<string, string>> Parameters { get; set; }

        IPage Init(int id);


        // unsure if used
        /// <summary>
        /// The resource specific url, like the one to this page or portal
        /// </summary>
        string Url { get; }

    }
}
