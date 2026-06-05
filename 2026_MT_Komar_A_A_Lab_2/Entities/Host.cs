namespace Entities;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable enable
[Table("Hosts")]
public class Host : BaseEntity<int>
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int HostId { get; set; }

    public override int Id => this.HostId;

    public int? CpuModelId { get; set; }

    [Required]
    [Column(TypeName = "decimal(5,2)")]
    public decimal RamGb { get; set; }

    [Required]
    public int OperatingSystemTypeId { get; set; }

    [ForeignKey(nameof(CpuModelId))]
    public virtual CpuModel? CpuModel { get; set; }

    [ForeignKey(nameof(OperatingSystemTypeId))]
    public virtual OperatingSystemType OperatingSystemType { get; set; } = null!;

    public virtual ICollection<ThreadSpeedMetric> ThreadSpeedMetrics { get; } =
        [];

    public override string ToLogString(string val = "")
        => base.ToLogString(
            $"{this.OperatingSystemType?.Name} RAM={this.RamGb}GB {val}".TrimEnd());
}