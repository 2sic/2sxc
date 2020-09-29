using System;
using ToSic.Eav.Apps;
using ToSic.Eav.Security.Permissions;
using ToSic.Sxc.Blocks.Edit;

namespace ToSic.Sxc.WebApi.InPage
{
    internal class AppViewPickerBackend: BlockWebApiBackendBase<AppViewPickerBackend>
    {
        public AppViewPickerBackend() : base("Bck.ViwApp") { }

        public void SetAppId(int? appId) => BlockEditorBase.GetEditor(_block).SetAppId(appId);


        public Guid? SaveTemplateId(int templateId, bool forceCreateContentGroup)
        {
            var callLog = Log.Call<Guid?>($"{templateId}, {forceCreateContentGroup}");
            ThrowIfNotAllowedInApp(GrantSets.WriteSomething);
            return callLog("ok", BlockEditorBase.GetEditor(_block).SaveTemplateId(templateId, forceCreateContentGroup));
        }

        public bool Publish(int id)
        {
            var callLog = Log.Call<bool>($"{id}");
            ThrowIfNotAllowedInApp(GrantSets.WritePublished);
            new AppManager(_block.App, Log).Entities.Publish(id);
            return callLog("ok", true);
        }


        public string Render(int templateId, string lang)
        {
            var callLog = Log.Call<string>($"{nameof(templateId)}:{templateId}, {nameof(lang)}:{lang}");
            SetThreadCulture(lang);

            // if a preview templateId was specified, swap to that
            if (templateId > 0)
            {
                var template = _cmsManager.Read.Views.Get(templateId);
                _block.View = template;
            }

            var rendered = _block.BlockBuilder.Render();
            return callLog("ok", rendered);
        }

        /// <summary>
        /// Try setting thread language to enable 2sxc to render the template in this language
        /// </summary>
        /// <param name="lang"></param>
        private static void SetThreadCulture(string lang)
        {
            if (string.IsNullOrEmpty(lang)) return;
            try
            {
                System.Threading.Thread.CurrentThread.CurrentCulture =
                    System.Globalization.CultureInfo.GetCultureInfo(lang);
            }
            // Fallback / ignore if the language specified has not been found
            catch (System.Globalization.CultureNotFoundException) { /* ignore */ }
        }
    }
}
