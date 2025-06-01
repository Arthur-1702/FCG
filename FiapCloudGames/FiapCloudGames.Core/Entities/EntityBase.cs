using System.ComponentModel.DataAnnotations;

namespace FiapCloudGames.Core.Entities
{
    public class EntityBase
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public DateTime DataCriacao { get; set; }
    }
}
