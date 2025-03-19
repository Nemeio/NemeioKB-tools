namespace Nemeio.Core.Services
{
    public interface IDocument
    {
        string DocumentPath { get; }

        string GetConfiguratorPath();

        string LogFolderPath { get; }

        string TemporaryFolderPath { get; }
    }
}
