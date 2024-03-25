using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Controllers;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using ToSic.Eav;
using Exception = System.Exception;

namespace ToSic.Sxc.Dnn.Install;

partial class DnnEnvironmentInstaller
{
    internal string UpgradeModule(string version, bool closeWhenDone)
    {
        var l = Log.Fn<string>($"{nameof(version)}: {version}, {nameof(closeWhenDone)}: {closeWhenDone}");

        // Check if table "ToSIC_SexyContent_Templates" exists. 
        // If it's gone, then PROBABLY skip all upgrade-codes incl. 8.11!
        var sql = @"SELECT COUNT(*) FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ToSIC_SexyContent_Templates]') AND TYPE IN(N'U')";
        var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["SiteSqlServer"].ConnectionString);
        sqlConnection.Open();
        var sqlCommand = new SqlCommand(sql, sqlConnection);
        var runDbChangesUntil811 = (int)sqlCommand.ExecuteScalar() == 1; // if there is one result row, this means the templates table still exists, we need to run changes before 08.11
        sqlConnection.Close();

        // if version is 01.00.00, the upgrade has to run because log files should be cleared
        if (!runDbChangesUntil811 && version != "01.00.00" && (new Version(version) <= new Version(8,11,0)))
        {
            _installLogger.LogStep(version, "Upgrade skipped because 00.99.00 install detected (installation of everything until and including 08.11 has been done by 00.99.00.SqlDataProvider)", true);
            _installLogger.LogVersionCompletedToPreventRerunningTheUpgrade(version);
            return version;
        }

        _installLogger.LogStep(version, "UpgradeModule starting", false);

        // Abort upgrade if it's already done - if version is 01.00.00, the module has probably been uninstalled - continue in this case.
        if (version != "01.00.00" && IsUpgradeComplete(version, true, "- Check on Start UpgradeModule"))
        {
            _installLogger.LogStep(version, "Apparently trying to update this version, but this versions upgrade is apparently completed, will abort");
            throw new("2sxc upgrade for version " + version +
                      " started, but it looks like the upgrade for this version is already complete. Aborting upgrade.");
        }
        _installLogger.LogStep(version, "version / upgrade-complete test passed");

        l.A("Will check if IsUpgradeRunning");
        if (IsUpgradeRunning)
        {
            _installLogger.LogStep(version, "Apparently upgrade is running, will abort");
            throw new("2sxc upgrade for version " + version +
                      " started, but the upgrade is already running. Aborting upgrade.");
        }
        _installLogger.LogStep(version, "is-upgrade-running test passed");

        IsUpgradeRunning = true;
        _installLogger.LogStep(version, "----- Upgrade to " + version + " started -----");

        try
        {

            switch (version)
            {
                // All versions before 8.11 should trigger this
                case "08.11.00":
                    throw new("Trying to upgrade a 7 or 8 version - which isn't supported in v9.20+. Please upgrade to the latest 8.12 or 9.15before trying to upgrade to a 9.20+");

                // case "15.00.00": // moved to 15.02 because of we accidentally skipped upgrades in 15.01 - see bug #2997
                // case "15.02.00": // originally moved to here, but the Settings.Installation.LastVersionWithServerChanges had not been upgraded
                //                  this results in no file being created for 15.02, so the "IsUpgradeComplete" always fails
                case "15.02.00":    // Set to 15.04 because otherwise the log file is not created if you already have a 15.02+ (necessary to verify UpgradeComplete)
                    try
                    {
                        // move app.json template from old to new location
                        _appJsonService.Value.MoveAppJsonTemplateFromOldToNewLocation();

                        // migrate old .data-custom folder
                        var oldDataCustomFolder = Path.Combine(Path.Combine(_globalConfiguration.Value.GlobalFolder, ".data-custom"));
                        if (Directory.Exists(oldDataCustomFolder) && !Directory.Exists(_globalConfiguration.Value.DataCustomFolder))
                            Directory.Move(oldDataCustomFolder, _globalConfiguration.Value.DataCustomFolder);

                        // migrate old .databeta folder
                        var oldDataBetaFolder = Path.Combine(Path.Combine(_globalConfiguration.Value.GlobalFolder, ".databeta"));
                        if (Directory.Exists(oldDataBetaFolder) && !Directory.Exists(_globalConfiguration.Value.DataBetaFolder))
                            Directory.Move(oldDataBetaFolder, _globalConfiguration.Value.DataBetaFolder);
                    }
                    catch
                    {
                        /* ignore */
                    }

                    // ToSic_EAV_DataTimeline cleaning data and change schema for CJson
                    const string sql150000 = @"                         
                            -- remove trigger generated data from 'ToSIC_EAV_DataTimeline' in batches
                            WHILE (SELECT COUNT(*) FROM [dbo].[ToSIC_EAV_DataTimeline] WHERE [SourceTable] IN ('ToSIC_EAV_Values', 'ToSIC_EAV_EntityRelationships', 'ToSIC_EAV_ValuesDimensions')) > 0
                            BEGIN
                                ;WITH CTE AS
                                (
	                            SELECT TOP 100000 * 
	                            FROM [dbo].[ToSIC_EAV_DataTimeline] 
	                            WHERE [SourceTable] IN ('ToSIC_EAV_Values', 'ToSIC_EAV_EntityRelationships', 'ToSIC_EAV_ValuesDimensions')
	                            )
                                DELETE FROM CTE;
                            END;

                            -- drop NewData column from 'ToSIC_EAV_DataTimeline'
                            IF EXISTS (SELECT * FROM sys.columns WHERE Name = 'NewData' AND Object_ID = OBJECT_ID('ToSIC_EAV_DataTimeline'))
                            BEGIN
                                ALTER TABLE ToSIC_EAV_DataTimeline DROP COLUMN NewData;
                            END;

                            -- add CJson column to 'ToSIC_EAV_DataTimeline'
                            IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = 'CJson' AND Object_ID = OBJECT_ID('ToSIC_EAV_DataTimeline'))
                            BEGIN
                                ALTER TABLE ToSIC_EAV_DataTimeline ADD CJson varbinary(max) NULL;
                            END;";
                    sqlConnection.Open();
                    var sqlCommand150000 = new SqlCommand(sql150000, sqlConnection);
                    sqlCommand150000.CommandTimeout = 0; // disable sql execution command timeout on sql server
                    sqlCommand150000.ExecuteNonQuery();
                    sqlConnection.Close();

                    break;

                // case "15.xx.xx":
                // IMPORTANT: when you add a new case, make sure you
                // 1. upgrade the version number on Settings.Installation.LastVersionWithServerChanges!!!
                // 2. add it to the Settings.Installation.UpgradeVersionList
                // If you don't do these two things, various problems will appear

            }
            _installLogger.LogStep(version, "version-list check / switch done", false);

            // Increase ClientDependency version upon each upgrade (System and all Portals)
            // prevents browsers caching old JS and CSS files for editing, which could cause several errors
            // only set this on the last upgraded version, to prevent crazy updating the client-resource-cache while upgrading
            if (version == EavSystemInfo.VersionString)
            {
                _installLogger.LogStep(version, "ClientResourceManager- seems to be last item in version-list, will clear");
                    
                HostController.Instance.IncrementCrmVersion(true);
                DataCache.ClearCache();
                    
                _installLogger.LogStep(version, "ClientResourceManager- done clearing");

                // Reset Upgrade complete so it's regenerated
                UpgradeCompleteCache.Reset();
                var getNewStatus = UpgradeComplete(true);
                _installLogger.LogStep(version, $"updated upgrade-complete status to {getNewStatus}");
            }

            _installLogger.LogVersionCompletedToPreventRerunningTheUpgrade(version);
            _installLogger.LogStep(version, "----- Upgrade to " + version + " completed -----");

        }
        catch (Exception e)
        {
            _installLogger.LogStep(version, "Upgrade failed - " + e.Message);
            throw;
        }
        finally
        {
            IsUpgradeRunning = false;
        }
        _installLogger.LogStep(version, "UpgradeModule done / returning");
        if (closeWhenDone)
            _installLogger.CloseLogFiles();

            
        return l.ReturnAndLog(version);
    }
        

}