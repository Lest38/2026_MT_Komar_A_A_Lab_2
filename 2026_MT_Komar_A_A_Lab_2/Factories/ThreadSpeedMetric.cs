namespace Factories;

using Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable enable
[Table("ThreadSpeedMetrics")]
public class ThreadSpeedMetric
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int PerformanceTestId { get; set; }

    [Required]
    public int HostId { get; set; }

    [Required]
    public int PipelineStepExecutionId { get; set; }

    [Required]
    public long SequentialTimeMs { get; set; }

    [Required]
    public long ParallelTimeMs { get; set; }

    [Required]
    [Column(TypeName = "decimal(10,4)")]
    public decimal EfficiencyCoefficient { get; set; }

    [Required]
    public DateTime StartedAt { get; set; }

    [Required]
    public long DurationMs { get; set; }

    [ForeignKey(nameof(PerformanceTestId))]
    public virtual PerformanceTest? PerformanceTest { get; set; }

    [ForeignKey(nameof(HostId))]
    public virtual Host? Host { get; set; }

    [ForeignKey(nameof(PipelineStepExecutionId))]
    public virtual PipelineStepExecution? PipelineStepExecution { get; set; }
}
#nullable restore