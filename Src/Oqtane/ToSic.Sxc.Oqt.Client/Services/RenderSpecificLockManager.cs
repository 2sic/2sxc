using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using AsyncKeyedLock;
using Oqtane.Modules;

namespace ToSic.Sxc.Oqt.Client.Services
{
    public class RenderSpecificLockManager : IService
    {
        private readonly AsyncKeyedLocker<Guid> _locks = new(o =>
        {
            o.PoolSize = 20;
            o.PoolInitialFill = 1;
        });

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async ValueTask<IDisposable> LockAsync(Guid renderId)
        {
            return await _locks.LockAsync(renderId).ConfigureAwait(false);
        }
    }

}
