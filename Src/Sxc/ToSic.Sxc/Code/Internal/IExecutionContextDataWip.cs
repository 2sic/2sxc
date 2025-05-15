using ToSic.Sxc.Apps;

namespace ToSic.Sxc.Code.Internal;

/// <summary>
/// WIP
/// </summary>
public interface IExecutionContextDataWip 
{

    #region Content, Header, App, Data, Resources, Settings

    ///// <inheritdoc cref="Eav.DataSources.App" />
    //IApp App { get; }

    /// <summary>
    /// Almost every use 
    /// </summary>
    /*IDynamicStack*/
    object Resources { get; }
    /*IDynamicStack*/
    object Settings { get; }


    #endregion



}