using System.ComponentModel.DataAnnotations;

namespace FiapCloudGames.Domain.Entities
{
    public class EntityBase
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public DateTime DataCriacao { get; set; }
    }
}
