using ToSic.Sxc.Cms.Notes.Sys;

namespace ToSic.Sxc.Cms.Notes;

/// <summary>
/// Note on anything - such as a site, page, entity, etc.
/// </summary>
/// <remarks>
/// History
/// 
/// * Introduced in v21.02
/// </remarks>
[ModelSpecs(Use = typeof(NoteModelOfEntity))]
[InternalApi_DoNotUse_MayChangeWithoutNotice("WIP v21.02")]
public interface INoteModel : IModelOfData
{
    /// <summary>
    /// The note ID.
    /// </summary>
    int Id { get; }

    /// <summary>
    /// The note GUID.
    /// </summary>
    Guid Guid { get; }

    /// <summary>
    /// The note creation date/time.
    /// </summary>
    DateTime Created { get; }

    /// <summary>
    /// The note modification date/time.
    /// </summary>
    DateTime Modified { get; }

    /// <summary>
    /// The note.
    /// </summary>
    string? Note { get; }

    string? NoteType { get; }
}