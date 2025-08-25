﻿namespace ToSic.Sxc.Sys.Render.PageContext;

[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IChangeQueue
{
    /// <summary>
    /// Once processed clean up, in case the same object (scoped) is used again, and we want to ensure it won't be processed again
    /// </summary>
    /// <returns></returns>
    IList<PagePropertyChange> GetPropertyChangesAndFlush(ILog log);
        
    /// <summary>
    /// Once processed clean up, in case the same object (scoped) is used again, and we want to ensure it won't be processed again
    /// </summary>
    /// <returns></returns>
    IList<HeadChange> GetHeadChangesAndFlush(ILog log);

    /// <summary>
    /// Status code to set (if possible) to the page which loads this block
    /// </summary>
    int? HttpStatusCode { get; set; }

    /// <summary>
    /// Status message to set (if possible) to the page which loads this block
    /// </summary>
    string? HttpStatusMessage { get; set; }

    public PagePropertyChange Queue(PageProperties property, string? value, PageChangeModes change, string? token);
}