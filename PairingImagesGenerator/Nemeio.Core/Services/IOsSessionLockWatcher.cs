using System;
using System.Threading.Tasks;

namespace Nemeio.Core.Services
{
    public interface IOsSessionLockWatcher
    {
        void Setup(Func<Task> osUserSessionLockedHandler, Func<Task> osUserSessionUnlockedHandler);
    }
}
