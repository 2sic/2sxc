
// ReSharper disable UnusedMember.Global
namespace System.Web.Http
{
    /// <summary>
    /// This is a Helper Attribute to make sure Code created in DNN will work in Oqtane.
    /// It basically mimics the type & namespace of System.Web (which should never exist in .net core & Oqtane)
    /// So that any code which has this set will continue to work.
    /// </summary>
    public class AllowAnonymousAttribute: Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute
    {
    }
}
