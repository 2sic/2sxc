namespace ToSic.Sxc.Dnn.Install
{
    public partial class DnnEnvironmentInstaller
    {
        #region Static Constructors to ensure installation

        /// <summary>
        /// This static initializer will do a one-time check to see if everything is ready,
        /// so subsequent access to this property will not need to do anything any more
        /// </summary>
        
        // TODO: THIS IS AN INIT-CODE, SHOULD NOT RUN RANDOMLY ON FIRST ACCESS, BUT ON DNN START CODE
        // THAT WOULD ALSO REMOVE THE NEED FOR A PAGE-SCOPED SERVICE PROVIDER BECAUSE WE ALREADY HAVE ONE THERE
        // but we need to be careful about timing, I'm not sure when exactly this is triggered
        static DnnEnvironmentInstaller()
        {
            // When this is created, update the status
            //DnnBusinessController.UpdateUpgradeCompleteStatus();
            VerifyUpgradeComplete();
        }

        public static void VerifyUpgradeComplete()
        {
            // When this is created, update the status
#pragma warning disable CS0618
            UpgradeComplete = DnnStaticDi.StaticBuild<DnnEnvironmentInstaller>()
                .IsUpgradeComplete(Settings.Installation.LastVersionWithServerChanges, "- static check");
#pragma warning restore CS0618
        }

        internal static bool UpgradeComplete;

        #endregion
    }
}
