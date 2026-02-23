namespace ToSic.Sxc.Oqt.Shared
{
    public class ModuleInfoConstants
    {
        /// <summary>
        /// Cleans up specific 2sxc/eav old assemblies in v20.00.00,
        /// which are no longer needed and may cause conflicts.
        /// Also to execute migration SQL script for 20.00.00
        /// </summary>
        public const string V20_00_00 = "20-00-00";

        /// <summary>
        /// Performs 2sxc folder reorganization for multi tenants support in v21.00.00
        /// and cleans up 1 net9 assembly in net10 runtime, which is no longer needed and may cause conflicts.
        /// Also to execute migration SQL script for 21.00.00
        /// </summary>
        public const string V21_00_00 = "21-00-00";
    }
}
