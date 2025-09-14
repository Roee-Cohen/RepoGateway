using System.ComponentModel.DataAnnotations;

namespace RepoGateway.Core.Models
{
    public class Favorite
    {
        [Key]
        public string RepoId { get; set; }
        public string Name { get; set; }
        public string Owner { get; set; }
        public int Stars { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
