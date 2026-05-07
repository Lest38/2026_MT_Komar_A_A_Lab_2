using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities;

[Table("Projects")]
public class Project
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string FolderPath { get; set; } = string.Empty;

    public DateTime? CreatedAt { get; set; } = DateTime.Now;

    [MaxLength(500)]
    public string? Description { get; set; }

    public virtual ICollection<PipelineStepExecution> PipelineStepExecutions { get; set; } = new List<PipelineStepExecution>();
}