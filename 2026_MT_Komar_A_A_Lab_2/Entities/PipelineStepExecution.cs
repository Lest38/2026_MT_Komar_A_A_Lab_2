namespace Entities;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

#nullable enable
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

    [Required]
    public int ExitCode { get; set; }

    [NotMapped]
    public int TotalErrors => this.IssueLogs?.Count(l => l.SeverityType?.Name == "Error") ?? 0;

    [NotMapped]
    public int TotalWarnings => this.IssueLogs?.Count(l => l.SeverityType?.Name == "Warning") ?? 0;

    [ForeignKey(nameof(ProjectId))]
    public virtual Project Project { get; set; } = null!;

    [ForeignKey(nameof(StageTypeId))]
    public virtual StageType StageType { get; set; } = null!;

    [ForeignKey(nameof(ExecutionStatusId))]
    public virtual ExecutionStatus ExecutionStatus { get; set; } = null!;

    public virtual ICollection<IssueLog> IssueLogs { get; } =
        [];

    public override string ToLogString(string val = "")
        => base.ToLogString($"[{this.ExecutionStatus?.Name}] Project={this.ProjectId} Stage={this.StageTypeId} {val}".TrimEnd());
}
