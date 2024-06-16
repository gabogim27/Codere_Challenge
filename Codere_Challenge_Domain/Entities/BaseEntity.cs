using System.ComponentModel.DataAnnotations;

namespace Codere_Challenge_Domain.Entities
{
    public abstract class BaseEntity
    {
        [Key]
        [Required]
        public int Id { get; set; }
    }
}
