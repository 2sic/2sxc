using System;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Data;
using static ToSic.Sxc.Compatibility.Obsolete;
using IHtmlString = System.Web.IHtmlString;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Blocks
{
    /// <summary>
    /// Block-Rendering system. It's responsible for taking a Block and delivering HTML for the output. <br/>
    /// It's used for InnerContent, so that Razor-Code can easily render additional content blocks. <br/>
    /// See also [](xref:Basics.Cms.InnerContent.Index)
    /// </summary>
    [PublicApi_Stable_ForUseInYourCode]
    [Obsolete("Deprecated in v12 - please use IRenderService instead - will not work in v12 Base classes like Razor12")]
    public class Render
    {
        /// <summary>
        /// Render one content block
        /// This is accessed through DynamicEntity.Render()
        /// At the moment it MUST stay internal, as it's not clear what API we want to surface
        /// </summary>
        /// <param name="parent">The parent-item containing the content-blocks and providing edit-context</param>
        /// <param name="noParamOrder"></param>
        /// <param name="item">The content-block item to render. Optional, by default the same item is used as the context.</param>
        /// <param name="field">Optional: </param>
        /// <param name="newGuid">Internal: this is the guid given to the item when being created in this block. Important for the inner-content functionality to work. </param>
        /// <returns></returns>
        public static IHtmlString One(DynamicEntity parent,
            string noParamOrder = Eav.Parameters.Protector,
            IDynamicEntity item = null,
            string field = null,
            Guid? newGuid = null)
            => RenderService(parent).One(parent, noParamOrder, item, field, newGuid);

        // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
        private static Services.IRenderService RenderService(DynamicEntity parent)
        {
            // First do version checks -should not be allowed if compatibility is too low
            if (parent._Dependencies.CompatibilityLevel > Constants.MaxLevelForStaticRender)
                throw new Exception(
                    "The static ToSic.Sxc.Blocks.Render can only be used in old Razor components. For v12+ use the ToSic.Sxc.Services.IRenderService instead");


            var block = parent._Dependencies.BlockOrNull;
            Warning13To15(
                "DeprecatedStaticRender",
                $"View:{block?.View?.Id}",
                "https://r.2sxc.org/brc-13-static-render",
                (log) => LogBlockDetails(block, log));


            return parent._Dependencies.RenderService;
        }

        /// <summary>
        /// Render content-blocks into a larger html-block containing placeholders
        /// </summary>
        /// <param name="parent">The parent-item containing the content-blocks and providing edit-context</param>
        /// <param name="noParamOrder"></param>
        /// <param name="field">Required: Field containing the content-blocks. </param>
        /// <param name="max">BETA / WIP</param>
        /// <param name="merge">Optional: html-text containing special placeholders.</param>
        /// <param name="apps">BETA / WIP</param>
        /// <returns></returns>
        public static IHtmlString All(DynamicEntity parent,
            string noParamOrder = Eav.Parameters.Protector,
            string field = null,
            string apps = null,
            int max = 100,
            string merge = null)
            => RenderService(parent).All(parent, noParamOrder, field, apps, max, merge);
    }
}