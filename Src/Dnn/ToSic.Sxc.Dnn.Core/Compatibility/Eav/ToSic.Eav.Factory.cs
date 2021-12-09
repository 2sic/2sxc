using System;
using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Dnn;

// ReSharper disable once CheckNamespace
namespace ToSic.Eav
{
    /// <summary>
    /// The Eav DI Factory, used to construct various objects through [Dependency Injection](xref:NetCode.DependencyInjection.Index).
    ///
    /// If possible avoid using this, as it's a workaround for code which is outside of the normal Dependency Injection and therefor a bad pattern.
    /// </summary>
    [PublicApi("Careful - obsolete!")]
    [Obsolete("Deprecated, please use Dnn 9 DI instead https://r.2sxc.org/brc-13-eav-factory")]
	public class Factory
	{
        [Obsolete("Not used any more, but keep for API consistency in case something calls ActivateNetCoreDi")]
        [PrivateApi]
        public delegate void ServiceConfigurator(IServiceCollection service);

        [PrivateApi]
	    public static void ActivateNetCoreDi(ServiceConfigurator configure)
	    {
            new LogHistory().Add("error", new Log(LogNames.Eav + ".DepInj", null, $"{nameof(ActivateNetCoreDi)} was called, but this won't do anything any more."));
        }

        /// <summary>
        /// Dependency Injection resolver with a known type as a parameter.
        /// </summary>
        /// <typeparam name="T">The type / interface we need.</typeparam>
        [Obsolete("Please use standard Dnn 9.4+ Dnn DI instead https://r.2sxc.org/brc-13-eav-factory")]
        public static T Resolve<T>()
        {
            return DnnStaticDi.GetServiceProvider().Build<T>();

            // Don't throw error yet, would probably cause too much breaks in public code
            throw new NotSupportedException("The Eav.Factory is obsolete. See https://r.2sxc.org/brc-13-eav-factory");
        }
    }
}