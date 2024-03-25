using System.Threading;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Primitives;

namespace ToSic.Sxc.Oqt.Server.Controllers.AppApi;


/// <summary>
/// Provides change notifications for action descriptors in the AppApi.
/// 2sxc supports adding, modifying, or removing endpoints at runtime, and this is essential that 
/// the rest of the asp.net core is aware of these changes to adjust its behavior accordingly.
/// This could include updating routing tables, clearing caches, or other refreshing to match the new set of available actions.
/// </summary>
/// <remarks>
/// IActionDescriptorChangeProvider interface is part of the ASP.NET Core MVC framework, 
/// specifically designed for providing mechanisms to notify about changes in action descriptors.
/// Action descriptors represent actions in MVC controllers that can be executed as a result of incoming HTTP requests.
/// Knowing when these descriptors change is crucial for dynamic applications that may modify their available actions at runtime.
/// </remarks>
internal class AppApiActionDescriptorChangeProvider : IActionDescriptorChangeProvider
{
    /// <summary>
    /// Gets the singleton instance of the AppApiActionDescriptorChangeProvider class.
    /// </summary>
    public static AppApiActionDescriptorChangeProvider Instance { get; } = new();

    /// <summary>
    /// Gets or sets the cancellation token source.
    /// </summary>
    public CancellationTokenSource TokenSource { get; private set; }

    /// <summary>
    /// Gets a change token for monitoring changes in the action descriptors.
    /// </summary>
    /// <returns>The change token.</returns>
    public IChangeToken GetChangeToken()
    {
        TokenSource = new();
        return new CancellationChangeToken(TokenSource.Token);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the action descriptors have changed.
    /// </summary>
    public bool HasChanged { get; set; }
}
