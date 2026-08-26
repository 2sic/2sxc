// #CleanUp2sxcEditInformation 2026-08-26 v22 2dm
// Removing this completely.
// Here's what I checked:
// - There is a bit of old code in 2sxc-ui, mentioning compatibility with V9
// - eav-ui does not use it
// - no apps use it as of 2026-08 and no apps probably used this for at least 3 years
// - IMHO previously there was still some use of this in the timelineJS a long time ago,
//   so there may still be installed usages, which would then need some guidance for upgrading.
//   
//
// But I hope that we can just kill this
// Keep an eye on this.

//namespace ToSic.Sxc.Edit.Sys;

//[ShowApiWhenReleased(ShowApiMode.Never)]
//public class SxcEditSharedConstants
//{


//    /// <summary>
//    /// Additional json-node for metadata in serialized entities, if user has edit rights
//    /// </summary>
//    public const string JsonEntityEditNodeName = "_2sxcEditInformation";
//}