namespace Entities;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable enable
[Table("OperatingSystemTypes")]
public class OperatingSystemType : BaseEntity<int>
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int OperatingSystemTypeId { get; set; }

    public override int Id => this.OperatingSystemTypeId;

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Description { get; set; }

    public virtual ICollection<Host> Hosts { get; } =
        [];

    public override string ToLogString(string val = "")
        => base.ToLogString($"{this.Name} {val}".TrimEnd());
}