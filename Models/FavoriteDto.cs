namespace RepoGateway.Models
{
    public class FavoriteDto
    {
        public string RepoId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Owner { get; set; } = string.Empty;
        public int Stars { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
