using ToSic.Eav.DataSources;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    public abstract class DataSource15: CustomDataSourceLight
    {
        /// <summary>
        /// Default constructor.
        /// You need to call this constructor in your code to ensure that
        /// </summary>
        /// <param name="services"></param>
        /// <param name="logName"></param>
        protected DataSource15(MyServices services, string logName = null) : base(services, logName: "Cus.HybDs")
        {

        }

    }
}
