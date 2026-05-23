namespace Entities;

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable enable
[Table("IssueLogs")]
public class IssueLog : BaseEntity<int>
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IssueLogId { get; set; }

    public override int Id => this.IssueLogId;

    [Required]
    public int PipelineStepExecutionId { get; set; }

    [Required]
    public DateTime LoggedAt { get; set; }

    [Required]
    [MaxLength(20)]
    public string Severity { get; set; } = string.Empty;

    public int? IssueCodeId { get; set; }

    [ForeignKey(nameof(IssueCodeId))]
    public virtual IssueCode? IssueCode { get; set; }

    [Required]
    public string Message { get; set; } = string.Empty;

    [ForeignKey(nameof(PipelineStepExecutionId))]
    public virtual PipelineStepExecution PipelineStepExecution { get; set; } = null!;

    public override string ToLogString(string val = "")
        => base.ToLogString($"[{this.Severity}] {this.IssueCode?.Code}: {this.Message} {val}".TrimEnd());
}