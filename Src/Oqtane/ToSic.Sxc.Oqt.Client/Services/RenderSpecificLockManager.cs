using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Oqtane.Modules;

namespace ToSic.Sxc.Oqt.Client.Services
{
    public class RenderSpecificLockManager : IService
    {
        private readonly ConcurrentDictionary<Guid, SemaphoreSlim> _locks = new();

        public async Task<IDisposable> LockAsync(Guid renderId)
        {
            var semaphore = _locks.GetOrAdd(renderId, _ => new SemaphoreSlim(1, 1));
            await semaphore.WaitAsync();
            return new ReleaseHandle(() => Release(renderId));
        }

        private void Release(Guid requestId)
        {
            if (_locks.TryGetValue(requestId, out var semaphore))
            {
                semaphore.Release();

                // Consider removing the semaphore from the dictionary if no longer needed,
                // but be cautious of race conditions and ensure proper synchronization.
            }
        }

        private class ReleaseHandle(Action releaseAction) : IDisposable
        {
            public void Dispose()
            {
                releaseAction();
            }
        }
    }

}
