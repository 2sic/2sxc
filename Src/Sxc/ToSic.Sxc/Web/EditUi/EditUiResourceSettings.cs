namespace ToSic.Sxc.Web.EditUi
{
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public struct EditUiResourceSettings
    {
        public bool IconsMaterial { get; set; }

        public bool FontRoboto { get; set; }

        public static EditUiResourceSettings EditUi => new EditUiResourceSettings
        {
            IconsMaterial = true,
            FontRoboto = true,
        };

        public static EditUiResourceSettings QuickDialog = new EditUiResourceSettings
        {
            IconsMaterial = true,
            FontRoboto = true,
        };
    }
}
