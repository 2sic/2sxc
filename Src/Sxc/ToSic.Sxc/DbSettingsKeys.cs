namespace ToSic.Sxc
{
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public class Settings
    {
        // Important note: always use static-readonly, NOT constant
        // This prevents the value from being compiled into other DLLs,
        // So if a value ever changes, it will always be retrieved from here


        /// <summary>
        /// This setting will store what App is to be shown on a module. 
        /// The value must contain the Guid/Name (so the word "Default" or the app guid)
        /// </summary>
#if NETFRAMEWORK
        public static readonly string ModuleSettingApp = "ToSIC_SexyContent_AppName";
#else
        public static readonly string ModuleSettingApp = "EavApp";
#endif

        /// <summary>
        /// This key is for storing the setting, which content-group (bundle/block) is to be shown in the module.
        /// The value will be a GUID. 
        /// </summary>
#if NETFRAMEWORK
        public static readonly string ModuleSettingContentGroup = "ToSIC_SexyContent_ContentGroupGuid";
#else
        public static readonly string ModuleSettingContentGroup = "EavContentGroup";
#endif

        /// <summary>
        /// This is used to store the Guid of the Preview-View in the module settings.
        /// The preview is only used till the App has a real content-group attached,
        /// after which the content-group will provide the correct view. 
        /// </summary>
#if NETFRAMEWORK
        public static readonly string ModuleSettingsPreview = "ToSIC_SexyContent_PreviewTemplateId";

#else
        public static readonly string ModuleSettingsPreview = "EavPreview";
#endif
    }
}
