namespace ToSic.Sxc.Data.Sys.Typed;

internal interface IValueOverrider
{
    #region Experiment - but decided for now that it's too much compute for something which is extremely rarely used, and can be done with an if-statement in the code

    //object? OverrideRaw(string name);

    //public T? OverrideRaw<T>(string name);

    #endregion

    /// <summary>
    /// Replace the raw value before any parsing/conversion happens.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="originalValue"></param>
    /// <returns></returns>
    string? ProcessString(string name, string? originalValue);
}