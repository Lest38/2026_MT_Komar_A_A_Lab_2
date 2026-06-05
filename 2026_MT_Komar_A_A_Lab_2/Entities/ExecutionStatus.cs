namespace Entities;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable enable

[Table("ExecutionStatuses")]
public class ExecutionStatus : BaseEntity<int>
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ExecutionStatusId { get; set; }

    public override int Id => this.ExecutionStatusId;

    [Required]
    [MaxLength(30)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Description { get; set; }

    public virtual ICollection<PipelineStepExecution> PipelineStepExecutions { get; } =
        [];

    public override string ToLogString(string val = "")
        => base.ToLogString($"{this.Name} {val}".TrimEnd());
}
