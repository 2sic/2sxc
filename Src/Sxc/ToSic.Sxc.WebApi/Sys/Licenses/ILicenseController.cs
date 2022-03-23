using System;
using System.Collections.Generic;

namespace ToSic.Sxc.WebApi.Sys.Licenses
{
    public interface ILicenseController
    {
        /// <summary>
        /// Gives an array of License (sort by priority)
        /// </summary>
        /// <returns></returns>
        IEnumerable<LicenseDto> Summary();

        /// <summary>
        /// License-upload backend
        /// </summary>
        /// <returns>bool</returns>
        /// <exception cref="ArgumentException"></exception>
        bool Upload();
    }
}