namespace Entities;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable enable

[Table("SeverityTypes")]
public class SeverityType : BaseEntity<int>
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int SeverityTypeId { get; set; }

    public override int Id => this.SeverityTypeId;

    [Required]
    [MaxLength(20)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Description { get; set; }

    public virtual ICollection<IssueLog> IssueLogs { get; } =
        [];

    public override string ToLogString(string val = "")
        => base.ToLogString($"{this.Name} {val}".TrimEnd());
}
