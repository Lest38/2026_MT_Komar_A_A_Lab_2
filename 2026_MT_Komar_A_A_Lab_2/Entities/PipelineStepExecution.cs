namespace Entities;

using Factories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("PipelineStepExecutions")]
public class PipelineStepExecution
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int ProjectId { get; set; }

    [Required]
    public int StageTypeId { get; set; }

    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = string.Empty;

    [Required]
    public DateTime StartedAt { get; set; }

    [Required]
    public long DurationMs { get; set; }

    public int ExitCode { get; set; }

    public int TotalErrors { get; set; }

    public int TotalWarnings { get; set; }

    [ForeignKey(nameof(ProjectId))]
    public virtual Project Project { get; set; }

    [ForeignKey(nameof(StageTypeId))]
    public virtual StageType StageType { get; set; }

    public virtual ICollection<IssueLog> IssueLogs { get; } =
        [];

    public virtual ICollection<ThreadSpeedMetric> ThreadSpeedMetrics { get; } =
        [];
}