namespace Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("PipelineStepExecutions")]
public class PipelineStepExecution : BaseEntity<int>
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int PipelineStepExecutionId { get; set; }

    public override int Id => this.PipelineStepExecutionId;

    [Required]
    public int ProjectId { get; set; }

    [Required]
    public int StageTypeId { get; set; }

    [Required]
    public int ExecutionStatusId { get; set; }

    [Required]
    public DateTime StartedAt { get; set; }

    [Required]
    public long DurationMs { get; set; }

    public int ExitCode { get; set; }

    public int TotalErrors { get; set; }

    public int TotalWarnings { get; set; }

    [ForeignKey(nameof(ProjectId))]
    public virtual Project Project { get; set; } = null!;

    [ForeignKey(nameof(StageTypeId))]
    public virtual StageType StageType { get; set; } = null!;

    [ForeignKey(nameof(ExecutionStatusId))]
    public virtual ExecutionStatus ExecutionStatus { get; set; } = null!;

    public virtual ICollection<IssueLog> IssueLogs { get; } =
        [];

    public virtual ICollection<ThreadSpeedMetric> ThreadSpeedMetrics { get; } =
        [];

    public override string ToLogString(string val = "")
        => base.ToLogString($"Status={this.ExecutionStatus?.Name} Errors={this.TotalErrors} Warnings={this.TotalWarnings} Duration={this.DurationMs}ms {val}".TrimEnd());
}