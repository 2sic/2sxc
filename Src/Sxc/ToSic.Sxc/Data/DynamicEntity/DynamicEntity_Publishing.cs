namespace ToSic.Sxc.Data
{
    public partial class DynamicEntity
    {
        /// <inheritdoc />
        public bool IsPublished => Entity?.IsPublished ?? true;

        //// <inheritdoc />
        //public dynamic GetDraft() => SubDynEntityOrNull(Entity?.GetDraft()); // 2023-03-27 v15.06 remove GetDraft/GetPublished from Entity

        ///// <inheritdoc />
        // public dynamic GetPublished() => SubDynEntityOrNull(Entity?.GetPublished()); //  2023-03-27 v15.06 remove GetDraft/GetPublished from Entity

    }
}
