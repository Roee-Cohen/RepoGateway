namespace RepoGateway.Models
{
    public class RepoItemDto
    {
        public string RepoId { get; set; }
        public string Name { get; set; }
        public string Owner { get; set; }
        public int Stars { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string HtmlUrl { get; set; }
    }
}
