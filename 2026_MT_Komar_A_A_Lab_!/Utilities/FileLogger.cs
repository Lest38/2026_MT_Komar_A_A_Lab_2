using System;
using System.IO;
using Microsoft.Extensions.Logging;

#nullable enable
namespace Utilities
{
    public class FileLogger : ILogger, IDisposable
    {
        private static readonly object Lock = new
            ();

        private readonly string filePath;
        private bool disposed;

        public FileLogger(string filePath)
        {
            this.filePath = filePath;
            this.EnsureDirectoryExists();
        }

        public IDisposable? BeginScope<TState>(TState state)
            where TState : notnull
            => default!;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            if (!this.IsEnabled(logLevel) || this.disposed)
            {
                return;
            }

            var message = formatter != null ? formatter(state, exception) : string.Empty;
            var logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{logLevel}] {message}";

            if (exception != null)
            {
                logEntry += $"{Environment.NewLine}{exception}";
            }

            lock (Lock)
            {
                if (!this.disposed)
                {
                    File.AppendAllText(this.filePath, logEntry + Environment.NewLine);
                }
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                // Nothing to dispose, file is closed automatically
            }

            this.disposed = true;
        }

        private void EnsureDirectoryExists()
        {
            var directory = Path.GetDirectoryName(this.filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }
    }
}