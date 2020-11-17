namespace ToSic.Sxc
{
    public partial class Settings
    {
        // Important note: always use static-readonly, NOT constant for .net 451
        // reason is that we must ensure that the static constructor is called 
        // whenever anything is accessed

        /// <summary>
        /// This setting will store what App is to be shown on a module. 
        /// The value must contain the Guid/Name (so the word "Default" or the app guid)
        /// </summary>
#if NETSTANDARD
        public const string ModuleSettingApp = "EavApp";
#else
        public static readonly string ModuleSettingApp = "ToSIC_SexyContent_AppName";
#endif

        /// <summary>
        /// This key is for storing the setting, which content-group (bundle/block) is to be shown in the module.
        /// The value will be a GUID. 
        /// </summary>
#if NETSTANDARD
        public const string ModuleSettingContentGroup = "EavContentGroup";
#else
        public static readonly string ModuleSettingContentGroup = "ToSIC_SexyContent_ContentGroupGuid";
#endif

        /// <summary>
        /// This is used to store the Guid of the Preview-View in the module settings.
        /// The preview is only used till the App has a real content-group attached,
        /// after which the content-group will provide the correct view. 
        /// </summary>
#if NETSTANDARD
        public const string ModuleSettingsPreview = "EavPreview";
#else
        public static readonly string ModuleSettingsPreview = "ToSIC_SexyContent_PreviewTemplateId";
#endif


    }
}
