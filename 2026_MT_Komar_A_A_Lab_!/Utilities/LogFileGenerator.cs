using System;
using System.Globalization;
using System.IO;

namespace Utilities;

public static class LogFileGenerator
{
    public static string GenerateLogFilePath(string targetDir)
    {
        var dirName = new DirectoryInfo(targetDir).Name;

        var timestamp = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss", CultureInfo.InvariantCulture);

        var fileName = $"CICD_{dirName}_{timestamp}.log";

        var parentDir = Directory.GetParent(targetDir)?.FullName ?? Directory.GetCurrentDirectory();

        return Path.Combine(parentDir, fileName);
    }
}