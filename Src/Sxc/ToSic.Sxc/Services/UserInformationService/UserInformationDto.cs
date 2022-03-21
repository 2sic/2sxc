namespace ToSic.Sxc.Services
{
    public class UserInformationDto /*: IUser*/
    {
        #region IUser

        public int Id { get; set; }
        //public string IdentityToken { get; }
        //public Guid? Guid { get; }
        //public List<int> Roles { get; }
        //public bool IsSuperUser { get; }
        //public bool IsAdmin { get; }
        //public bool IsDesigner { get; }
        //public bool IsAnonymous { get; }

        #endregion

        public string Name { get; set; }
    }
}
