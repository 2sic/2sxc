﻿namespace ToSic.Sxc.Data.Sys.Json;

[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IHasJsonSource
{
    /// <summary>
    /// The inner json source to use.
    /// Will only have an effect if the Attribute [JsonConverter(typeof(DynamicJsonConverter))] is applied.
    /// </summary>
    /// <remarks>
    /// This must be a method - not a property - for safety.
    /// This ensures it doesn't result in being serialized itself. 
    /// </remarks>
    /// <returns></returns>
    object JsonSource();
}