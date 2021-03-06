﻿using System;
using System.IO;
using ToSic.Eav.Apps.Environment;
using ToSic.Eav.Apps.ImportExport;
using ToSic.Eav.Configuration;
using ToSic.Eav.Context;
using ToSic.Eav.Identity;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;
using ToSic.Eav.WebApi.Dto;

namespace ToSic.Sxc.WebApi.ImportExport
{
    public class ImportApp: HasLog
    {
        #region DI Constructor

        public ImportApp(IZoneMapper zoneMapper, IServerPaths serverPaths, IEnvironmentLogger envLogger, ZipImport zipImport, IGlobalConfiguration globalConfiguration) : base("Bck.Export")
        {
            _zoneMapper = zoneMapper;
            _serverPaths = serverPaths;
            _envLogger = envLogger;
            _zipImport = zipImport;
            _globalConfiguration = globalConfiguration;
        }

        private readonly IZoneMapper _zoneMapper;
        private readonly IServerPaths _serverPaths;
        private readonly IEnvironmentLogger _envLogger;
        private readonly ZipImport _zipImport;
        private readonly IGlobalConfiguration _globalConfiguration;
        private IUser _user;

        public ImportApp Init(IUser user, ILog parentLog)
        {
            Log.LinkTo(parentLog);
            _zoneMapper.Init(Log);
            _user = user;
            return this;
        }

        #endregion

        public ImportResultDto Import(int zoneId, string name, Stream stream)
        {
            Log.Add("import app start");
            var result = new ImportResultDto();

            if (!string.IsNullOrEmpty(name)) Log.Add($"new app name: {name}");

            var zipImport = _zipImport;
            try
            {
                zipImport.Init(zoneId, null, _user.IsSuperUser, Log);
                var temporaryDirectory = Path.Combine(_globalConfiguration.TemporaryFolder, Mapper.GuidCompress(Guid.NewGuid()).Substring(0, 8));

                // Increase script timeout to prevent timeouts
                result.Success = zipImport.ImportZip(stream, temporaryDirectory, name);
                result.Messages.AddRange(zipImport.Messages);
            }
            catch (Exception ex)
            {
                _envLogger.LogException(ex);
                result.Success = false;
                result.Messages.AddRange(zipImport.Messages);
            }
            return result;
        }

    }
}
