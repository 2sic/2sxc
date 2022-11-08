using System;

namespace ToSic.Sxc.Dnn.WebApi.HttpJson
{
    /// <summary>
    /// Mark all base classes for custom WebApi controllers which should use the old Newtonsoft.
    /// Important because it otherwise breaks existing code which may be using Newtonsoft objects.
    /// https://github.com/2sic/2sxc/issues/2917
    /// Should only be applied to the base classes up to Api14, but not on newer classes
    /// </summary>
    public class UseOldNewtonsoftForHttpJsonAttribute : Attribute
    { }
}

