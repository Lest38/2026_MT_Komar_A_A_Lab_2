namespace Entities;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("StageTypes")]
public class StageType : BaseEntity<int>
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int StageTypeId { get; set; }

    public override int Id => this.StageTypeId;

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    public virtual ICollection<PipelineStepExecution> PipelineStepExecutions { get; } = [];

    public override string ToLogString(string val = "")
        => base.ToLogString($"{Name} {val}".TrimEnd());
}