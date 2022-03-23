using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ToSic.Eav.Configuration;
using ToSic.Eav.Configuration.Licenses;
using ToSic.Eav.Documentation;
using ToSic.Eav.WebApi;
using ToSic.Eav.WebApi.Assets;
using ToSic.Eav.WebApi.Validation;
using ToSic.Sxc.WebApi.Adam;

namespace ToSic.Sxc.WebApi.Sys.Licenses
{
    public class LicenseControllerReal : WebApiBackendBase<LicenseControllerReal>, ILicenseController
    {
        public LicenseControllerReal(IServiceProvider serviceProvider, 
            Lazy<ILicenseService> licenseServiceLazy, 
            Lazy<IFeaturesInternal> featuresLazy,
            Lazy<IGlobalConfiguration> globalConfiguration) : base(serviceProvider, "Bck.Lics")
        {
            _licenseServiceLazy = licenseServiceLazy;
            _featuresLazy = featuresLazy;
            _globalConfiguration = globalConfiguration;
        }
        private readonly Lazy<ILicenseService> _licenseServiceLazy;
        private readonly Lazy<IFeaturesInternal> _featuresLazy;
        private readonly Lazy<IGlobalConfiguration> _globalConfiguration;

        /// <inheritdoc />
        public IEnumerable<LicenseDto> Summary()
        {
            var licSer = _licenseServiceLazy.Value;
            var licenses = licSer.Catalog()
                .OrderBy(l => l.Priority);

            var features = _featuresLazy.Value.All;

            return licenses
                .Select(l => new LicenseDto
                {
                    Name = l.Name,
                    Priority = l.Priority,
                    Guid = l.Guid,
                    Description = l.Description,
                    AutoEnable = l.AutoEnable,
                    IsEnabled = licSer.IsEnabled(l),
                    Features = features
                        .Where(f => f.License == l.Name)
                        .OrderBy(f => f.NameId)
                });
        }



        /// <summary>
        /// Not in use implementation for interface ILicenseController compatibility.
        /// Instead in use is bool Upload(HttpUploadedFile uploadInfo).
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [PrivateApi]
        public bool Upload() => throw new NotImplementedException();


        /// <summary>
        /// License-upload backend
        /// </summary>
        /// <param name="uploadInfo"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public bool Upload(HttpUploadedFile uploadInfo)
        {
            var wrapLog = Log.Call<bool>();

            if (!uploadInfo.HasFiles())
                return wrapLog("no file in upload", false);

            var files = new List<FileUploadDto>();
            for (var i = 0; i < uploadInfo.Count; i++)
            {
                var (fileName, stream) = uploadInfo.GetStream(i);

                files.Add(new FileUploadDto { Name = fileName, Stream = stream });
            }

            var configurationsPath = Path.Combine(_globalConfiguration.Value.GlobalFolder, Eav.Constants.FolderDataCustom, FsDataConstants.ConfigFolder);
            
            // ensure that path to store files already exits
            Directory.CreateDirectory(configurationsPath);

            foreach (var file in files)
            {
                // verify it's json etc.
                if (!Json.IsValidJson(file.Contents))
                    throw new ArgumentException("a file is not json");

                var filePath = Path.Combine(configurationsPath, file.Name);

                //  rename old file before saving new one
                if (File.Exists(filePath)) RenameOldFile(filePath);

                // save file
                File.WriteAllText(filePath, file.Contents);
            }

            // TODO: do we need to reload lic files now?

            return wrapLog("ok", true);
        }

        private static void RenameOldFile(string filePath)
        {
            // rename old file to next free name like filename.001.bak
            var i = 0;
            var fileExists = true;
            do
            {
                i++;
                var newFileName = $"{filePath}.{i:000}.bak";
                fileExists = File.Exists(newFileName);
                if (!fileExists) File.Move(filePath, newFileName);
            } while (fileExists);
        }
    }
}
