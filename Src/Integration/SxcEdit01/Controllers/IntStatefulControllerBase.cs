namespace IntegrationSamples.SxcEdit01.Controllers
{
    public abstract class IntStatefulControllerBase: IntStatelessControllerBase
    {
        /// <summary>
        /// The dependencies is for future use.
        /// Later the StatefulController will need dependencies, so it's best to already have this
        /// helper object ready in your code structure
        /// </summary>
        public class Dependencies
        {
        }

        protected IntStatefulControllerBase() : base()
        {
        }

    }
}
