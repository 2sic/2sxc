using System.Collections.Generic;
using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Context
{
    /// <summary>
    /// Collection of url parameters of the current page
    ///
    /// Has a special ToString() implementation, which gives you the parameters for re-use in other scenarios...?
    /// </summary>
    [InternalApi_DoNotUse_MayChangeWithoutNotice]
    public interface IParameters: IReadOnlyDictionary<string, string>
    {
        /// <summary>
        /// ToString is especially implemented, to give you the parameters again as they were originally given on the page.
        /// </summary>
        /// <returns></returns>
        [InternalApi_DoNotUse_MayChangeWithoutNotice("wip")]
        string ToString();

        /// <summary>
        /// Add another URL parameter and return a new <see cref="IParameters"/>.
        /// If the name/key already exists, it will extend it, add a simple 
        /// Otherwise please use <see cref="Set(string,string)"/>
        /// 
        /// _Important: this does not change the current object, it returns a new object._
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        IParameters Add(string name);

        /// <summary>
        /// Add another URL parameter and return a new <see cref="IParameters"/>.
        /// If the name/key already exists, it will extend it, so the parameter will have 2 values.
        /// Otherwise please use <see cref="Set(string,string)"/>
        /// 
        /// _Important: this does not change the current object, it returns a new object._
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        IParameters Add(string name, string value);

        /// <summary>
        /// Add another URL parameter and return a new <see cref="IParameters"/>.
        /// If the name/key already exists, it will just overwrite it.
        /// 
        /// _Important: this does not change the current object, it returns a new object._
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        IParameters Set(string name, string value);

        /// <summary>
        /// Add another URL parameter and return a new <see cref="IParameters"/>.
        /// If the name/key already exists, it will just overwrite it.
        /// 
        /// _Important: this does not change the current object, it returns a new object._
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IParameters Set(string name);

        /// <summary>
        /// Remove a parameter and return a new <see cref="IParameters"/>.
        ///
        /// _Important: this does not change the current object, it returns a new object._
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IParameters Remove(string name);
    }
}
