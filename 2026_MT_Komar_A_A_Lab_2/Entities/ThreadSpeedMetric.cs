namespace Entities;

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("ThreadSpeedMetrics")]
public class ThreadSpeedMetric : BaseEntity<int>
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ThreadSpeedMetricId { get; set; }

    public override int Id => this.ThreadSpeedMetricId;

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

    [NotMapped]
    public decimal EfficiencyCoefficient =>
    this.ParallelTimeMs == 0 ? 0 : (decimal)this.SequentialTimeMs / this.ParallelTimeMs;

    [Required]
    public DateTime StartedAt { get; set; }

    [Required]
    public long DurationMs { get; set; }

    [ForeignKey(nameof(PerformanceTestId))]
    public virtual PerformanceTest PerformanceTest { get; set; } = null!;

    [ForeignKey(nameof(HostId))]
    public virtual Host Host { get; set; } = null!;

    [ForeignKey(nameof(PipelineStepExecutionId))]
    public virtual PipelineStepExecution PipelineStepExecution { get; set; } = null!;

    public override string ToLogString(string val = "")
        => base.ToLogString($"Seq={this.SequentialTimeMs}ms Par={this.ParallelTimeMs}ms Eff={this.EfficiencyCoefficient:F4}x {val}".TrimEnd());
}