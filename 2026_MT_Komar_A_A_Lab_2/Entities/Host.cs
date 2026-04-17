namespace Entities;

using Factories;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("Hosts")]
public class Host
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int CpuModelId { get; set; }

    [Required]
    [Column(TypeName = "decimal(5,2)")]
    public decimal RamGb { get; set; }

    [Required]
    [MaxLength(200)]
    public string OperatingSystem { get; set; } = string.Empty;

    [ForeignKey(nameof(CpuModelId))]
    public virtual CpuModel CpuModel { get; set; }

    public virtual ICollection<ThreadSpeedMetric> ThreadSpeedMetrics { get; } =
        [];
}