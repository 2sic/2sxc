using ToSic.Eav.Apps.Sys.FileSystemState;

namespace ToSic.Sxc.Backend.App;

/// <summary>
/// Shared validation results for extension zip operations.
/// </summary>
internal record ValidationResult(bool Success, string? Error);

internal record LockValidationResult(bool Success, string? Error, HashSet<string>? AllowedFiles) : ValidationResult(Success, Error);

internal record ManifestValidationResult(bool Success, string? Error, bool EditionsSupported, ExtensionManifest? Manifest) : ValidationResult(Success, Error);

internal record ExtensionExtractionResult(
    bool Success,
    string? Error,
    string TempDir,
    string AppRoot,
    string[] Editions,
    Dictionary<string, LockValidationResult> LockResults,
    Dictionary<string, ManifestValidationResult> ManifestResults);
