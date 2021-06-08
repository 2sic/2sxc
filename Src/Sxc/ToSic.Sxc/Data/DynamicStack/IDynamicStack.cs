using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Data
{
    /// <summary>
    /// This is a dynamic object which contains multiple dynamic objects (Sources).
    /// It will try to find a value inside each source in the order the Sources are managed. 
    /// </summary>
    /// <remarks>New in 12.02</remarks>
    [PublicApi("Careful - still WIP in 12.02")]
    public interface IDynamicStack: IDynamicEntityGet
    {
        /// <summary>
        /// Get a dynamic object
        /// </summary>
        /// <param name="name"></param>
        /// <returns>A dynamic object like a <see cref="IDynamicEntity"/> or similar, can also be null if the source isn't found. </returns>
        dynamic GetSource(string name);

        /// <inheritdoc/>
        dynamic Get(string name);

        /// <inheritdoc/>
        dynamic Get(string name,
            // ReSharper disable once MethodOverloadWithOptionalParameter
            string dontRelyOnParameterOrder = Eav.Parameters.Protector,
            string language = null,
            bool convertLinks = true);
    }
}