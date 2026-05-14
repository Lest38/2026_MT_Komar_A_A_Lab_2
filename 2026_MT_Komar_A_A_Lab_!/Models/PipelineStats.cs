namespace Models;

public class PipelineStats
{
    public int StageNumber { get; set; }

    public int TotalStages { get; set; }

    public int SuccessfulStages { get; set; }

    public int FailedStages { get; set; }
}