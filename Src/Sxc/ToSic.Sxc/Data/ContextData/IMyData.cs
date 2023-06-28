using System.Collections.Generic;
using ToSic.Eav.DataSource;
using ToSic.Lib.Documentation;
using static ToSic.Eav.Parameters;

namespace ToSic.Sxc.Data
{
    [WorkInProgressApi("WIP 16.02")]
    public interface IMyData: IContextData
    {
        ITypedItem FakeItem { get; }

        IEnumerable<ITypedItem> FakeItems { get; }

        /// <summary>
        /// Get the first item in the specified stream. If not found:
        /// By default it will return `null` or the `fallback` if the stream was empty, 
        /// but will throw an error if the stream with that name is not found.
        /// </summary>
        /// <param name="streamName">The stream name to use, will use `Default` if not provided.</param>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="fallback">Fallback value to return if not found / empty.</param>
        /// <param name="required">
        /// * `null` (default): stream must exist, but it can be empty
        /// * `false`: will return `null` if stream-not-found
        /// * `true`: throw errors even if stream exists but is empty
        /// </param>
        /// <returns></returns>
        ITypedItem Item(
            string streamName = default,
            string noParamOrder = Protector,
            ITypedItem fallback = default,
            bool? required = default
        );

        /// <summary>
        /// Get a list of items in the specified stream.
        /// * It will return an empty list if the stream exists
        /// * It will throw an error if the stream doesn't exist, unless `required: false` in which case it returns the `<see cref="fallback"/>` or `null`
        /// * If you prefer an empty list on `required: false`, set `preferNull: false`
        /// * If you prefer null on empty lists, set `preferNull: true`
        /// </summary>
        /// <param name="streamName">The stream name to use, will use `Default` if not provided.</param>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="fallback"></param>
        /// <param name="required">
        /// * `null` (default): stream must exist, but it can be empty
        /// * `false`: will return `null` if stream-not-found
        /// * `true`: throw errors even if stream exists but is empty
        /// </param>
        /// <param name="preferNull">
        /// Determines preference for not-found-stream or empty-list
        ///
        /// * `null` (default): Return null on not-found (if 'required: false', empty-list on empty-list
        /// * `true`: return a null if the stream was found and list is empty
        /// * `false`: return empty list if 'required: false'
        /// </param>
        /// <returns></returns>
        IEnumerable<ITypedItem> Items(
            string streamName = default,
            string noParamOrder = Protector,
            IEnumerable<ITypedItem> fallback = default,
            bool? required = default,
            bool? preferNull = default);

        IDataSource DataSource { get; }
    }
}
