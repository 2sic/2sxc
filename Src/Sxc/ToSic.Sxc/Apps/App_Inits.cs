using System;
using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;
using ToSic.Sxc.Blocks;
using EavApp = ToSic.Eav.Apps.App;

namespace ToSic.Sxc.Apps;

public partial class App
{
    [PrivateApi]
    public App PreInit(ISite site)
    {
        var l = Log.Fn<App>();
        Site = site;
        return l.Return(this);
    }

    /// <summary>
    /// Main constructor which auto-configures the app-data
    /// </summary>
    [PrivateApi]
    public new App Init(IAppIdentityPure appIdentity, Func<EavApp, IAppDataConfiguration> buildConfig)
    {
        var l = Log.Fn<App>();
        base.Init(appIdentity, buildConfig);
        return buildConfig == null 
            ? l.Return(this, "App only initialized for light use - .Data shouldn't be used") 
            : l.ReturnAsOk(this);
    }


    [PrivateApi]
    public IApp InitWithOptionalBlock(int appId, IBlock optionalBlock = null)
    {
        var l = Log.Fn<IApp>();
        var appStates = _appStates.New();
        var appIdentity = appStates.IdentityOfApp(appId);
        var confProvider = _appConfigDelegate.New();
        var buildConfig = optionalBlock == null
            ? confProvider.Build()
            : confProvider.Build(optionalBlock);
        return l.Return(Init(appIdentity, buildConfig));
    }
}