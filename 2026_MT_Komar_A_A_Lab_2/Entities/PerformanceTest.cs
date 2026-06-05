namespace Entities;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("PerformanceTests")]
public class PerformanceTest : BaseEntity<int>
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int PerformanceTestId { get; set; }

    public override int Id => this.PerformanceTestId;

    [Required]
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    public virtual ICollection<ThreadSpeedMetric> ThreadSpeedMetrics { get; } =
        [];

    public override string ToLogString(string val = "")
        => base.ToLogString($"{this.Description} {val}".TrimEnd());
}