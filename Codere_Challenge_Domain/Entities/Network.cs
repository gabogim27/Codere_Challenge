using Codere_Challenge_Domain.Entities;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Codere_Challenge_Core.Entities
{
    [Table("Networks")]
    public class Network : BaseEntity
    {
        public string Name { get; set; }
        public Country Country { get; set; }
    }
}
