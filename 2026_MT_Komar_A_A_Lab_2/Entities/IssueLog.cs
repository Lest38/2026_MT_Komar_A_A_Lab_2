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
    public int SeverityTypeId { get; set; }

    [ForeignKey(nameof(SeverityTypeId))]
    public virtual SeverityType SeverityType { get; set; } = null!;

    public int? IssueCodeId { get; set; }

    [ForeignKey(nameof(IssueCodeId))]
    public virtual IssueCode? IssueCode { get; set; }

    [Required]
    public string Message { get; set; } = string.Empty;

    [ForeignKey(nameof(PipelineStepExecutionId))]
    public virtual PipelineStepExecution PipelineStepExecution { get; set; } = null!;

    public override string ToLogString(string val = "")
        => base.ToLogString($"[{this.SeverityType?.Name}] {this.IssueCode?.Code}: {this.Message} {val}".TrimEnd());
}
