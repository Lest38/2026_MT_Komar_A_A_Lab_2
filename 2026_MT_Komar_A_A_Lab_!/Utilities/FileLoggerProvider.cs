using System;
using Microsoft.Extensions.Logging;

namespace Utilities
{
    public class FileLoggerProvider(string filePath)
        : ILoggerProvider
    {
        private readonly string filePath = filePath;
        private FileLogger logger;
        private bool disposed;

        public ILogger CreateLogger(string categoryName)
        {
            this.logger = new FileLogger(this.filePath);
            return this.logger;
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
                this.logger?.Dispose();
                this.logger = null;
            }

            this.disposed = true;
        }
    }
}