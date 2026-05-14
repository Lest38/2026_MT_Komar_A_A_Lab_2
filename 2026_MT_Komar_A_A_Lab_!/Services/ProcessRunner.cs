using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Models;

namespace Services;

#nullable enable
public class ProcessRunner : IDisposable
{
    private Process? currentProcess;
    private bool disposed;

    public static async Task<ProcessResult> RunCommandAsync(
        string fileName,
        string arguments,
        string workingDirectory,
        bool waitForExit = true,
        int timeoutSeconds = 0)
    {
        var result = new ProcessResult
        {
            Command = fileName,
            Arguments = arguments,
            WorkingDirectory = workingDirectory,
            StartTime = DateTime.Now,
        };

        var psi = new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = arguments,
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        using var process = new Process { StartInfo = psi, EnableRaisingEvents = true };

        var outputBuilder = new StringBuilder();
        var errorBuilder = new StringBuilder();

        process.OutputDataReceived += (s, e) =>
        {
            if (e.Data != null)
            {
                outputBuilder.AppendLine(e.Data);
            }
        };

        process.ErrorDataReceived += (s, e) =>
        {
            if (e.Data != null)
            {
                errorBuilder.AppendLine(e.Data);
            }
        };

        try
        {
            process.Start();
            result.ProcessId = process.Id;

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            if (!waitForExit)
            {
                result.ExitCode = 0;
                result.EndTime = DateTime.Now;
                return result;
            }

            bool exited;

            if (timeoutSeconds > 0)
            {
                using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));
                try
                {
                    await process.WaitForExitAsync(cancellationTokenSource.Token).ConfigureAwait(false);
                    exited = true;
                }
                catch (OperationCanceledException)
                {
                    exited = false;
                }
            }
            else
            {
                await process.WaitForExitAsync().ConfigureAwait(false);
                exited = true;
            }

            if (!exited)
            {
                process.Kill();
                result.IsTimeout = true;
                result.ExitCode = -1;
                result.Errors = $"Timeout after {timeoutSeconds} seconds";
            }
            else
            {
                result.ExitCode = process.ExitCode;
            }

            result.Output = outputBuilder.ToString();
            result.Errors = errorBuilder.ToString();
        }
        finally
        {
            result.EndTime = DateTime.Now;
            result.DurationMs = (long)(result.EndTime - result.StartTime).TotalMilliseconds;
        }

        return result;
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

        if (disposing && this.currentProcess != null && !this.currentProcess.HasExited)
        {
            try
            {
                this.currentProcess.Kill();
                this.currentProcess.Dispose();
            }
            catch (InvalidOperationException)
            {
                // Process already exited
            }
            catch (System.ComponentModel.Win32Exception)
            {
                // Cannot kill the process
            }
            finally
            {
                this.currentProcess = null;
            }
        }

        this.disposed = true;
    }
}
#nullable restore