using ToSic.Eav.Data.Sys.Entities;
using ToSic.Sxc.Adam.Sys.Security;
using ToSic.Sxc.Edit.Toolbar;
using ToSic.Sxc.Edit.Toolbar.Sys;
using ToSic.Sxc.Images.Sys;
using ToSic.Sxc.Images.Sys.ResizeSettings;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Images;

internal class ResponsiveToolbarBuilder(ILog parentLog) : HelperBase(parentLog, $"{SxcLogName}.ImgTlb")
{
    // note: it's a method but ATM always returns the cached toolbar
    // still implemented as a method, so we could add future parameters if necessary
    public IToolbarBuilder? Toolbar(ImageService imgService, ResponsiveSpecsOfTarget target, TweakMedia tweaker, ResizeSettings? settings, string src)
    {
        var l = Log.Fn<IToolbarBuilder?>();
        switch (tweaker.ToolbarObj)
        {
            case false: return l.ReturnNull("false");
            case IToolbarBuilder customToolbar: return l.Return(customToolbar, "already set");
        }

        // If we're creating an image for a string value, it won't have a field or parent.
        if (target.FieldOrNull?.Parent == null)
            return l.ReturnNull("no field");

        // If the HasMd is null, or the Metadata is null (e.g. image from another website)
        if (target.HasMdOrNull?.Metadata == null)
            return l.ReturnNull("no metadata");

        // Determine if this is an "own" adam file, because only field-owned files should allow config
        var isInSameEntity = AdamSecurity.PathIsInItemAdam(target.FieldOrNull.Parent.Guid, "", src);

        // Construct the toolbar; in edge cases the toolbar service could be missing
        var imgTlb = imgService.ToolbarOrNull?.Empty().Settings(
            hover: "right-middle",
            // Delay show of toolbar if it's a shared image, as it shouldn't be used much
            ui: isInSameEntity ? null : "delayShow=1000"
        );

        // Try to add the metadata button (or just null if not available)
        imgTlb = imgTlb?.Metadata(target.HasMdOrNull,
            tweak: t =>
            {
                // Add note only for the ImageDecorator Metadata, not for other buttons
                // Note: Using experimental AddNamed feature which doesn't exist on the ITweakButton interface
                var modified = t.AddNamed(ImageDecorator.TypeNameId, btn =>
                {
                    // add label like "Image Settings and Cropping" - i18n
                    btn = btn.Tooltip($"{ToolbarConstants.ToolbarLabelPrefix}MetadataImage");

                    // Check if we have special resize metadata
                    var md = target.HasMdOrNull.Metadata
                        .OneOfType(ImageDecorator.NiceTypeName)
                        .NullOrGetWith(imgDeco => new ImageDecorator(imgDeco, []));

                    // Try to add note
                    var note = settings?.ToHtmlInfo(md);
                    if (note.HasValue())
                        btn = btn.Note(note, format: "html", background: "#DFC2F2", delay: 1000);

                    // if image is from elsewhere, show warning
                    btn = isInSameEntity
                        ? btn
                        : btn.FormParameters(ImageDecorator.ShowWarningGlobalFile, true);
                    return btn;
                });

                // Add note for Copyright - if there is Metadata for that
                target.HasMdOrNull.Metadata
                    .OfType(CopyrightDecorator.NiceTypeName)
                    .FirstOrDefault()
                    .DoIfNotNull(cpEntity =>
                    {
                        var copyright = new CopyrightDecorator(cpEntity);
                        modified = modified.AddNamed(
                            CopyrightDecorator.TypeNameId,
                            btn => btn
                                .Tooltip("Copyright")
                                .Note(copyright.CopyrightMessage.NullIfNoValue() ??
                                      copyright.Copyrights?.FirstOrDefault()?.GetBestTitle() ?? ""
                                )
                        );
                    });


                return modified;
            });

        return l.ReturnAsOk(imgTlb);
    }
}