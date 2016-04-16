using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using DotNetNuke.Entities.Modules;
using ToSic.Eav;
using ToSic.SexyContent.ImportExport;

namespace ToSic.SexyContent.Installer
{


    internal class V8: VersionBase
    {
        public V8(string version, Logger sharedLogger) : base(version, sharedLogger)  { }

        internal void Version080002()
        {
            logger.LogStep("08.00.02", "Start", false);

            var userName = "System-ModuleUpgrade-080002";

            // Fix AddressMask field in GPS settings content type
            var xmlToImport =
                File.ReadAllText(HttpContext.Current.Server.MapPath("~/DesktopModules/ToSIC_SexyContent/Upgrade/08.00.02.xml"));
            var xmlImport = new XmlImport("en-US", userName, true);
            var success = xmlImport.ImportXml(Constants.DefaultZoneId, Constants.MetaDataAppId, XDocument.Parse(xmlToImport), false); // special note - change existing values

            if (!success)
            {
                var messages = String.Join("\r\n- ", xmlImport.ImportLog.Select(p => p.Message).ToArray());
                throw new Exception("The 2sxc module upgrade to 08.00.02 failed: " + messages);
            }

            logger.LogStep("08.00.02", "Done", false);

        }

        internal void Version080004()
        {
            logger.LogStep("08.00.04", "Start", false);

            var userName = "System-ModuleUpgrade-080004";

            // Fix AddressMask field in GPS settings content type
            var xmlToImport =
                File.ReadAllText(HttpContext.Current.Server.MapPath("~/DesktopModules/ToSIC_SexyContent/Upgrade/08.00.04.xml"));
            var xmlImport = new XmlImport("en-US", userName, true);
            var success = xmlImport.ImportXml(Constants.DefaultZoneId, Constants.MetaDataAppId, XDocument.Parse(xmlToImport), false); // special note - change existing values

            if (!success)
            {
                var messages = String.Join("\r\n- ", xmlImport.ImportLog.Select(p => p.Message).ToArray());
                throw new Exception("The 2sxc module upgrade to 08.00.04 failed: " + messages);
            }

            logger.LogStep("08.00.02", "Done", false);
        }

        internal void Version080007()
        {
            logger.LogStep("08.00.07", "Start", false);

            RemoveModuleControls(new[] { "settings", "settingswrapper" });

            logger.LogStep("08.00.07", "Done", false);
        }

        internal void Version080100()
        {
            logger.LogStep("08.01.00", "Start", false);

            var userName = "System-ModuleUpgrade-080100";

            // Add new content types and entities
            var xmlToImport =
                File.ReadAllText(HttpContext.Current.Server.MapPath("~/DesktopModules/ToSIC_SexyContent/Upgrade/08.01.00.xml"));
            var xmlImport = new XmlImport("en-US", userName, true);
            var success = xmlImport.ImportXml(Constants.DefaultZoneId, Constants.MetaDataAppId, XDocument.Parse(xmlToImport), false); // special note - change existing values

            if (!success)
            {
                var messages = String.Join("\r\n- ", xmlImport.ImportLog.Select(p => p.Message).ToArray());
                throw new Exception("The 2sxc module upgrade to 08.01.00 failed: " + messages);
            }

            // Remove unneeded control key for template file editing
            RemoveModuleControls(new[] { "edittemplatefile" });
        }

        internal void Version080302()
        {
            logger.LogStep("08.03.02", "Start", false);

            var userName = "System-ModuleUpgrade-080302";

            // Add new content types and entities
            var xmlToImport =
                File.ReadAllText(HttpContext.Current.Server.MapPath("~/DesktopModules/ToSIC_SexyContent/Upgrade/08.03.02.xml"));
            var xmlImport = new XmlImport("en-US", userName, true);
            var success = xmlImport.ImportXml(Constants.DefaultZoneId, Constants.MetaDataAppId, XDocument.Parse(xmlToImport), true);

            if (!success)
            {
                var messages = String.Join("\r\n- ", xmlImport.ImportLog.Select(p => p.Message).ToArray());
                throw new Exception("The 2sxc module upgrade to 08.03.02 failed: " + messages);
            }

            // 2016-03-13 2dm: disabled this rename again, because I tested it without and it seems the manifest works, so this could only lead to trouble one day
            //var desktopModuleNames = new[] { "2sxc", "2sxc-app" };
            //// Update BusinessController class name in desktop module info
            //foreach (var d in desktopModuleNames)
            //{
            //    var dmi = DesktopModuleController.GetDesktopModuleByModuleName(d, -1);
            //    dmi.BusinessControllerClass = "ToSic.SexyContent.Environment.Dnn7.DnnBusinessController";
            //    DesktopModuleController.SaveDesktopModule(dmi, false, true);
            //}

        }

        internal void Version080303()
        {
            logger.LogStep("08.03.03", "Start", false);

            var userName = "System-ModuleUpgrade-080303";

            // Change "Author" to "Owner" (permissions content type)
            var xmlToImport =
                File.ReadAllText(HttpContext.Current.Server.MapPath("~/DesktopModules/ToSIC_SexyContent/Upgrade/08.03.03.xml"));
            var xmlImport = new XmlImport("en-US", userName, true);
            var success = xmlImport.ImportXml(Constants.DefaultZoneId, Constants.MetaDataAppId, XDocument.Parse(xmlToImport), false); // Overwrite existing values

            if (!success)
            {
                var messages = String.Join("\r\n- ", xmlImport.ImportLog.Select(p => p.Message).ToArray());
                throw new Exception("The 2sxc module upgrade to 08.03.03 failed: " + messages);
            }
        }

        private void RemoveModuleControls(IEnumerable<string> controls)
        {
            logger.LogStep("08.--.--", "RemoveModuleControlls - Start", false);

            var desktopModuleNames = new[] { "2sxc", "2sxc-app" };

            // Remove settings and settingswrapper control
            foreach (var d in desktopModuleNames)
            {
                var dmi = DesktopModuleController.GetDesktopModuleByModuleName(d, -1);
                if (dmi != null)
                {
                    var mdi = dmi.ModuleDefinitions.FirstOrDefault();

                    if (mdi.Value != null)
                    {
                        foreach (var c in controls)
                        {
                            var settingsControl = ModuleControlController.GetModuleControlByControlKey(c, mdi.Value.ModuleDefID);
                            if (settingsControl != null)
                                ModuleControlController.DeleteModuleControl(settingsControl.ModuleControlID);
                        }
                    }
                }
            }
        }

    }
}