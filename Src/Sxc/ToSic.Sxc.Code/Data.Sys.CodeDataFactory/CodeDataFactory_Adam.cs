﻿using ToSic.Eav.Context;
using ToSic.Razor.Blade;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Adam.Sys;
using ToSic.Sxc.Adam.Sys.Manager;
using ToSic.Sxc.Context.Sys;
using ToSic.Sxc.Data.Sys.Factory;
using ToSic.Sxc.Images;
using ToSic.Sxc.Services;
using ToSic.Sxc.Services.Internal;
using ToSic.Sxc.Services.Tweaks;
using ToSic.Sxc.Sys.ExecutionContext;

namespace ToSic.Sxc.Data.Sys.CodeDataFactory;

partial class CodeDataFactory
{
    [field: AllowNull, MaybeNull]
    public AdamManager AdamManager
    {
        get => field ??= GetAdamManager();
        private set;
    }

    /// <summary>
    /// Special helper - if the DynamicCode is generated by the service or used in a WebApi there is no block, but we can figure out the context.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private AdamManager GetAdamManager()
    {
        // If we don't even have a _DynCodeRoot (like when exporting from a neutral WebAPI)
        if (ExCtxOrNull is null)
            throw new($"Can't create App Context for {nameof(AdamManager)} in {nameof(ICodeDataFactory)} - no block, no App");

        IContextOfApp contextOfApp = ExCtx.GetState<IContextOfBlock>();
        // TODO: @2dm - find out / document why this could even be null
        if (contextOfApp == null)
        {
            var app = ExCtx.GetApp();
            if (app == null)
                throw new("Can't create App Context for ADAM - no block, no App");
            contextOfApp = contextOfAppLazy.Value;
            contextOfApp.ResetApp(app);
        }

        return adamManager.Value.Init(contextOfApp, CompatibilityLevel, this);
    }
    #region ADAM / Folder

    public IFile File(int id)
        => AdamManager.AdamFs.GetFile(AdamAssetIdentifier.Create(id));

    private ExecutionContext ExCtxReal => (ExCtxOrNull as ExecutionContext)!;

    [field: AllowNull, MaybeNull]
    private ServiceKit16 ServiceKit16 => field ??= ExCtxReal.GetKit<ServiceKit16>();

    // TODO: MUST FINISH THIS, NOT WORKING YET
    public IFile? File(IField field)
        => ServiceKit16.Adam.File(field);

    public IFolder Folder(int id)
        => AdamManager.AdamFs.GetFolder(AdamAssetIdentifier.Create(id));

    public IFolder Folder(ICanBeEntity item, string name, IField? field)
        => AdamManager.FolderOfField(item.Entity.EntityGuid, name, field);

    public IFolder Folder(Guid entityGuid, string fieldName, IField? field = default)
        => AdamManager.FolderOfField(entityGuid, fieldName, field);

    [field: AllowNull, MaybeNull]
    private ICmsService CmsSvc => field ??= ExCtxReal.GetService<ICmsService>(reuse: true);

    // TODO: MUST FINISH THIS, NOT WORKING YET
    public IHtmlTag Html(
        object thing,
        NoParamOrder noParamOrder = default,
        object? container = default,
        string? classes = default,
        bool debug = default,
        object? imageSettings = default,
        bool? toolbar = default,
        Func<ITweakInput<string>, ITweakInput<string>>? tweak = default)
        => CmsSvc.Html(
            thing,
            noParamOrder: noParamOrder,
            container: container,
            classes: classes,
            debug: debug,
            imageSettings: imageSettings,
            toolbar: toolbar,
            tweak: tweak
        );

    [field: AllowNull, MaybeNull]
    private IImageService ImgSvc => field ??= ExCtxReal.GetService<IImageService>(reuse: true);


    public IResponsivePicture Picture(
        object? link = null,
        object? settings = default,
        NoParamOrder noParamOrder = default,
        Func<ITweakMedia, ITweakMedia>? tweak = default,
        object? factor = default,
        object? width = default,
        string? imgAlt = default,
        string? imgAltFallback = default,
        string? imgClass = default,
        object? imgAttributes = default,
        string? pictureClass = default,
        object? pictureAttributes = default,
        object? toolbar = default,
        object? recipe = default
    ) => ImgSvc.Picture(link, settings, noParamOrder, tweak, factor, width, imgAlt, imgAltFallback, imgClass,
        imgAttributes, pictureClass, pictureAttributes, toolbar, recipe);

    public IResponsiveImage Img(
        object? link = null,
        object? settings = default,
        NoParamOrder noParamOrder = default,
        Func<ITweakMedia, ITweakMedia>? tweak = default,
        object? factor = default,
        object? width = default,
        string? imgAlt = default,
        string? imgAltFallback = default,
        string? imgClass = default,
        object? imgAttributes = default,
        object? toolbar = default,
        object? recipe = default
    )
        => ImgSvc.Img(link, settings, noParamOrder, tweak, factor, width, imgAlt, imgAltFallback, imgClass,
            imgAttributes, toolbar, recipe);

    #endregion

}