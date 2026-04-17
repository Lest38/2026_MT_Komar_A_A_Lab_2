namespace Entities;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("CpuModels")]
public class CpuModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string ModelName { get; set; } = string.Empty;

    [Required]
    public int PhysicalCoreCount { get; set; }

    [Required]
    public int LogicalThreadCount { get; set; }

    public virtual ICollection<Host> Hosts { get; } =
        [];
}