using UnityEngine;
using ILogger = App.Domains.Core.Api.ILogger;

namespace App.Domains.Core.Impl
{
    public class ConsoleLogger : ILogger
    {
        public void Log(string message) =>
            Debug.Log(message);

        public void LogError(string message) =>
            Debug.LogError(message);
    }
}