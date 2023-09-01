using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace ToSic.Sxc.BuildTasks
{
    public class GetBuildConfig : Task
    {
        private const string BuildConfigJsonFileName = "2sxc-build.config.json";
        private const string FallbackBuildConfigJsonFileName = "2sxc-build-fallback.config.json";

        #region json properties
        [Output]
        public string[] JsTargets { get; private set; } = Array.Empty<string>();

        [Output]
        public string[] DnnTargets { get; private set; } = Array.Empty<string>();

        [Output]
        public string[] OqtaneTargets { get; private set; } = Array.Empty<string>();
        [Output]
        public string[] Sources { get; private set; }

        [Output]
        public string OqtaneModuleInstallPackages { get; private set; } = string.Empty;
        #endregion

        #region calculated properties
        [Output]
        public string BuildConfigPath { get; private set; }

        [Output]
        public string JsTarget { get; private set; }

        [Output]
        public string DnnTarget { get; private set; }

        [Output]
        public string OqtaneTarget { get; private set; }

        [Output]
        public string Source { get; private set; }
        #endregion

        public override bool Execute()
        {
            try
            {
                BuildConfigPath = FindJsonFile(BuildConfigJsonFileName) ?? FindJsonFile(FallbackBuildConfigJsonFileName);
                if (string.IsNullOrEmpty(BuildConfigPath))
                {
                    Log.LogError($"Could not find {FallbackBuildConfigJsonFileName} or {FallbackBuildConfigJsonFileName} file.");
                    return false;
                }

                var jsonContent = File.ReadAllText(BuildConfigPath);
                var buildConfig = JsonSerializer.Deserialize<BuildConfig>(jsonContent,
                    new JsonSerializerOptions {
                        ReadCommentHandling = JsonCommentHandling.Skip,
                        AllowTrailingCommas = true
                    });

                Sources = FixAllTargets(buildConfig.Sources)?.ToArray();
                Source = Sources?.FirstOrDefault();

                JsTargets = FixAllTargets(buildConfig.JsTargets)?.ToArray();
                JsTarget = JsTargets?.FirstOrDefault();

                DnnTargets = FixAllTargets(buildConfig.DnnTargets)?.ToArray();
                DnnTarget = DnnTargets?.FirstOrDefault();

                OqtaneTargets = FixAllTargets(buildConfig.OqtaneTargets)?.ToArray();
                OqtaneTarget = OqtaneTargets?.FirstOrDefault();

                OqtaneModuleInstallPackages = FixSingleTarget(buildConfig.OqtaneModuleInstallPackages);

                return true;
            }
            catch (Exception e)
            {
                Log.LogErrorFromException(e);
                return false;
            }
        }


        private static string FindJsonFile(string buildConfigJsonFileName)
        {
            // Start with the current directory
            var currentDirectory = Directory.GetCurrentDirectory();

            // Continue to look in the parent directories until there's no parent
            while (currentDirectory != null)
            {
                // Combine the current directory with the file name to get the full path
                var jsonFilePath = Path.Combine(currentDirectory, buildConfigJsonFileName);

                // Check if the file exists in the current directory
                if (File.Exists(jsonFilePath))
                    return jsonFilePath;

                // Move to the parent directory for the next iteration
                currentDirectory = Directory.GetParent(currentDirectory)?.FullName;
            }

            // Return null if the file was not found in any directory
            return null;
        }

        /// <summary>
        /// Fix all targets, by adding the path to the target
        /// </summary>
        /// <param name="paths">The list of target paths</param>
        /// <param name="addOnPath">The path to add to the target</param>
        /// <returns>The list of fixed target paths</returns>
        public static List<string> FixAllTargets(List<string> paths, string addOnPath = null) 
            => paths?.Select(t => FixSingleTarget(t, addOnPath)).ToList();

        /// <summary>
        /// Fix the target path by adding the path to the target
        /// </summary>
        /// <param name="value">The target path</param>
        /// <param name="addToPath">The path to add to the target</param>
        /// <returns>The fixed target path</returns>
        public static string FixSingleTarget(string value, string addToPath = null)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return string.IsNullOrEmpty(addToPath) 
                ? FixPath(value, false, true) + Path.DirectorySeparatorChar
                : Path.Combine(FixPath(value, false, true) + Path.DirectorySeparatorChar, FixPath(addToPath, false, true) + Path.DirectorySeparatorChar);
        }

        /// <summary>
        /// Fix the path by replacing backslashes with forward slashes and removing double slashes
        /// </summary>
        /// <param name="path">The path to fix</param>
        /// <param name="removeStartingSlash">Remove the starting slash</param>
        /// <param name="removeEndingSlash">Remove the ending slash</param>
        /// <returns>The fixed path</returns>
        public static string FixPath(string path, bool removeStartingSlash = false, bool removeEndingSlash = false)
        {
            if (string.IsNullOrEmpty(path)) return path;

            var clean = path.Trim().Replace("\\", "/").Replace("//", "/");

            if (removeStartingSlash && clean.StartsWith("/")) clean = clean.Substring(1);
            if (removeEndingSlash && clean.EndsWith("/")) clean = clean.Remove(clean.Length - 1);

            return clean.Replace('/', Path.DirectorySeparatorChar);
        }
    }
}
