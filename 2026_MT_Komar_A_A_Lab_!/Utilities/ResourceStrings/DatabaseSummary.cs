namespace Utilities.ResourceStrings
{
    public static class DatabaseSummary
    {
        public const string Header = "Database Summary (read-back)";
        public const string Projects = "Projects";
        public const string ProjectEntry = "  Id: {0}  Name: {1}";
        public const string PipelineSteps = "\nPipelineStepExecutions: {0}";
        public const string StepEntry = "  Id={0}  Status={1}  Errors={2}  Warnings={3}  Duration={4}ms";
        public const string IssueLogs = "\nIssueLogs: {0}";
        public const string IssueEntry = "  [{0}] {1}: {2}";
        public const string ThreadSpeedMetrics = "\nThreadSpeedMetrics: {0}";
        public const string MetricEntry = "  Id: {0}  Seq: {1}ms  Par: {2}ms  Eff: {3:F4}x";
        public const string StageTypes = "\nStageTypes (seeded): {0}";
        public const string StageTypeEntry = "  Id: {0}  Name: {1}";
        public const string CpuModels = "\nCpuModels (seeded): {0}";
        public const string CpuModelEntry = "  Id: {0}  {1}  Cores: {2}  Threads: {3}";
        public const string Hosts = "\nHosts: {0}";
        public const string HostEntry = "  Id: {0}  CpuModelId: {1}  RAM: {2} GB  OS: {3}";
        public const string PerformanceTests = "\nPerformanceTests: {0}";
        public const string PerformanceTestEntry = "  Id: {0}  \"{1}\"";
    }
}
