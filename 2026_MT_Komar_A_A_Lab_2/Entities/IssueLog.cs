namespace Entities;

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable enable
[Table("IssueLogs")]
public class IssueLog
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int PipelineStepExecutionId { get; set; }

    [Required]
    public DateTime LoggedAt { get; set; }

    [Required]
    [MaxLength(20)]
    public string Severity { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? Code { get; set; }

    [Required]
    public string Message { get; set; } = string.Empty;

    [ForeignKey(nameof(PipelineStepExecutionId))]
    public virtual PipelineStepExecution? PipelineStepExecution { get; set; }
}
#nullable restore