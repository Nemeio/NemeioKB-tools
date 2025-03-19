using Nemeio.Core.JsonModels;
using System.Threading.Tasks;

namespace Nemeio.Core.Services
{
    public interface INemeioHttpService
    {
        Task<UpdateModel> CheckForUpdates();
        void ListenToRequests();
        void StopListeningToRequests();
    }
}
