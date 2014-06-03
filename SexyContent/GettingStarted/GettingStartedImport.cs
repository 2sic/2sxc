using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Xml.Linq;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;

namespace ToSic.SexyContent.ImportExport
{
    public class GettingStartedImport
    {

        private int _zoneId;
        private int _appId;

        public GettingStartedImport(int zoneId, int appId)
        {
            _zoneId = zoneId;
            _appId = appId;
        }

        private XDocument _releases;
        public XDocument Releases
        {
            get
            {
                if (_releases == null)
                {
                    var releaseXmlFileUrl = "http://autoinstall.2sexycontent.org/2SexyContent-Core/Releases.xml";
                    var releaseXmlRequest = (HttpWebRequest)WebRequest.Create(releaseXmlFileUrl);
                    var releaseXmlFileContent = new StreamReader(releaseXmlRequest.GetResponse().GetResponseStream()).ReadToEnd();
                    _releases = XDocument.Parse(releaseXmlFileContent);
                }
                return _releases;
            }
        }

        public bool ImportGettingStartedTemplates(UserInfo user, List<ExportImportMessage> messages)
        {
            var release = Releases.Element("SexyContentReleases").Elements("Release").Last(p => new Version(p.Attribute("Version").Value) <= new Version(SexyContent.ModuleVersion));
            var starterPackageUrl = release.Elements("RecommendedPackages").Elements("Package").First().Attribute("PackageUrl").Value;

            return new ZipImport(_zoneId, _appId, user.IsSuperUser).ImportZipFromUrl(starterPackageUrl, messages, false);
        }

    }
}