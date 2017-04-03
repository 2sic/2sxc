using ToSic.Eav.Implementations.UserInformation;

namespace ToSic.SexyContent.Environment.Dnn7.EavImplementation
{
    public class DnnUserInformation: IEavUserInformation
    {
        public string IdentityForLog => UserIdentity.CurrentUserIdentityToken;
    }
}