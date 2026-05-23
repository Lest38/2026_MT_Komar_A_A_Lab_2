using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
    [Table("ExecutionStatuses")]
    public class ExecutionStatus : BaseEntity<int>
    {
        public int ExecutionStatusId { get; set; }

        public override int Id => this.ExecutionStatusId;

        [Required]
        [MaxLength(20)]
        public string Name { get; set; } = string.Empty;
    }
}
