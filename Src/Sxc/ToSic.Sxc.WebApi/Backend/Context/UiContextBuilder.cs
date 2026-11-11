namespace ToSic.Sxc.Backend.Context;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class UiContextBuilder(Generator<IUiContextParts, IAppReader?> contextPartsGen)
    : ServiceWithSetup<IAppReader?>(SxcLogName + ".UiCtx", connect: [contextPartsGen]), IUiContextBuilder
{
    public IUiContextParts ContextParts => field ??= contextPartsGen.New(MyOptions);

    /// <inheritdoc />
    public ContextDto Get(Ctx flags, CtxEnable enableFlags)
    {
        // logic for activating each part
        // 1. either that switch is on
        // 2. or the null-check: all is on
        // 3. This also means if the switch is off, it's off
        return new()
        {
            App = flags.HasFlag(Ctx.AppBasic) || flags.HasFlag(Ctx.AppAdvanced) ? ContextParts.GetApp(flags) : null,
            Enable = enableFlags != CtxEnable.None ? ContextParts.GetEnable(enableFlags) : null,
            Language = flags.HasFlag(Ctx.Language) ? ContextParts.GetLanguage(flags) : null,
            Page = flags.HasFlag(Ctx.Page) ? ContextParts.GetPage(flags) : null,
            Site = flags.HasFlag(Ctx.Site) ? ContextParts.GetSite(flags) : null,
            System = flags.HasFlag(Ctx.System) ? ContextParts.GetSystem(flags) : null,
            User = flags.HasFlag(Ctx.User) ? ContextParts.GetUser(flags) : null,
            Features = flags.HasFlag(Ctx.Features) || flags.HasFlag(Ctx.FeaturesForSystemTypes) ? ContextParts.GetFeatures(flags) : null
        };

    }
}
