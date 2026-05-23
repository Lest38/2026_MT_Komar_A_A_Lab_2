using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
    [Table("IssueCodes")]
    public class IssueCode : BaseEntity<int>
    {
        public int IssueCodeId { get; set; }

        public override int Id => this.IssueCodeId;

        [Required]
        [MaxLength(50)]
        public string Code { get; set; } = string.Empty;
    }
}
