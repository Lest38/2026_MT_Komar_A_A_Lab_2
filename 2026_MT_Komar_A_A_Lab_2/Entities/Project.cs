namespace Entities;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("Projects")]
public class Project : BaseEntity<int>
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ProjectId { get; set; }

    public override int Id => this.ProjectId;

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string FolderPath { get; set; } = string.Empty;

    public virtual ICollection<PipelineStepExecution> PipelineStepExecutions { get; } =
        [];

    public override string ToLogString(string val = "")
        => base.ToLogString($"{this.Name} @ {this.FolderPath} {val}".TrimEnd());
}