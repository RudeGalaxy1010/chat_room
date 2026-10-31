namespace App.Domains.Core.Api
{
    public interface ILogger
    {
        void Log(string message);
        void LogError(string message);
    }
}