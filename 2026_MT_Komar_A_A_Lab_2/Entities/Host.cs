namespace Entities;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("Hosts")]
public class Host : BaseEntity<int>
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int HostId { get; set; }

    public override int Id => this.HostId;

    [Required]
    public int CpuModelId { get; set; }

    [Required]
    [Column(TypeName = "decimal(5,2)")]
    public decimal RamGb { get; set; }

    [Required]
    [MaxLength(200)]
    public string OperatingSystem { get; set; } = string.Empty;

    [ForeignKey(nameof(CpuModelId))]
    public virtual CpuModel CpuModel { get; set; } = null!;

    public virtual ICollection<ThreadSpeedMetric> ThreadSpeedMetrics { get; } =
        [];

    public override string ToLogString(string val = "")
        => base.ToLogString($"{this.OperatingSystem} RAM={this.RamGb}GB {val}".TrimEnd());
}