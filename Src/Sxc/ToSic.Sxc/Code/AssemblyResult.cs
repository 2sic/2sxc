using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ToSic.Sxc.Code
{
    public class AssemblyResult(
        Assembly assembly = null,
        string errorMessages = null,
        string[] assemblyLocations = null,
        string safeClassName = null,
        Type mainType = default)
    {
        public Assembly Assembly { get; } = assembly;
        public string ErrorMessages { get; } = errorMessages;
        public string[] AssemblyLocations { get; } = assemblyLocations;
        public string SafeClassName { get; } = safeClassName;

        public Type MainType => mainType;

        // WIP - should be more functional, this get/set is still hacky
        public IList<string> WatcherFolders { get; set; }

        public AssemblyResult((Assembly Assembly, string ErrorMessages) tuple) : this(tuple.Assembly, tuple.ErrorMessages)
        { }

    }

    public static class AssemblyResultExtensions
    {
        //public static AssemblyResult ToAssemblyResult(this (Assembly Assembly, string ErrorMessages) tuple)
        //    => new(tuple);

        public static (Assembly Assembly, string ErrorMessages) ToTuple(this AssemblyResult assemblyResult) 
            => (assemblyResult.Assembly, assemblyResult.ErrorMessages);
    }


}
