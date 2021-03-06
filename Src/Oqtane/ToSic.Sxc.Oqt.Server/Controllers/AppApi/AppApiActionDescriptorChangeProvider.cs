﻿using System.Threading;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Primitives;

namespace ToSic.Sxc.Oqt.Server.Controllers.AppApi
{
    public class AppApiActionDescriptorChangeProvider : IActionDescriptorChangeProvider
    {
        public static AppApiActionDescriptorChangeProvider Instance { get; } = new AppApiActionDescriptorChangeProvider();
        public CancellationTokenSource TokenSource { get; private set; }
        public IChangeToken GetChangeToken()
        {
            TokenSource = new CancellationTokenSource();
            return new CancellationChangeToken(TokenSource.Token);
        }

        public bool HasChanged { get; set; }
    }
}
