namespace Utilities.ResourceStrings
{
    public static class ThreadSpeedMetric
    {
        public const string DemoTitle = "ThreadSpeedMetric Demo";
        public const string NoPerformanceTestFound = "No PerformanceTest found, skipping metric demo.";
        public const string NoPipelineStepFound = "No PipelineStepExecution found, skipping metric demo.";
        public const string AlreadyRecorded = "  ThreadSpeedMetric for test \"{0}\" already recorded, skipping.";
        public const string MetricSaved = "  Test: \"{0}\"\n  Sequential: {1} ms  |  Parallel: {2} ms  |  Efficiency: {3:F4}x\n  Metric Id: {4} saved.";
    }
}
