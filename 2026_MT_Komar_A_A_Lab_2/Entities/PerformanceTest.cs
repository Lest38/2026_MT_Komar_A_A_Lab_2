namespace Entities;

using Factories;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("PerformanceTests")]
public class PerformanceTest
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public string Description { get; set; } = string.Empty;

    public virtual ICollection<ThreadSpeedMetric> ThreadSpeedMetrics { get; } =
        [];
}