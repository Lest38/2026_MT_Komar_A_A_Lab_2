namespace Entities;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("CpuModels")]
public class CpuModel : BaseEntity<int>
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int CpuModelId { get; set; }

    public override int Id => this.CpuModelId;

    [Required]
    [MaxLength(200)]
    public string ModelName { get; set; } = string.Empty;

    [Required]
    public int PhysicalCoreCount { get; set; }

    [Required]
    public int LogicalThreadCount { get; set; }

    public virtual ICollection<Host> Hosts { get; } = [];

    public override string ToLogString(string val = "")
        => base.ToLogString($"{this.ModelName} Cores={this.PhysicalCoreCount} Threads={this.LogicalThreadCount} {val}".TrimEnd());
}