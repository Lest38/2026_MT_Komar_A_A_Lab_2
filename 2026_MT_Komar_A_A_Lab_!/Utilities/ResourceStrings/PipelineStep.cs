namespace Utilities.ResourceStrings
{
    public static class PipelineStep
    {
        public const string StageNotFound = "StageType 'Build' not found, skipping pipeline demo.";
        public const string BuildStepAlreadyExists = "  [Pipeline] Build step already exists for this project - skipping demo insert.";
        public const string StepInfo = "  [Step] Id: {0} Stage: {1} Status: {2} Duration: {3} ms";
        public const string IssueLogsCommitted = "  [IssueLogs] {0} entries committed (transaction).";
        public const string ReadBackInfo = "\n  Read-back: Step {0} has {1} log(s):";
        public const string LogEntry = "    [{0}] {1}: {2}";
        public const string TransactionRolledBack = "  Transaction rolled back: {0}";
    }
}