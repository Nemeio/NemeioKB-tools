using Nemeio.Core.DataModels.Configurator;

namespace Nemeio.Core.Services.Layouts
{
    public interface ILayoutGenService
    {
        byte[] RenderLayout(ConfiguratorLayout layout);

        byte[] RenderLayout(OsLayoutId layout);
    }
}
