namespace RepoGateway.Models
{
    public class RepoRo
    {
        public string RepoId { get; set; }
        public string Name { get; set; }
        public string Owner { get; set; }
        public int Stars { get; set; }
        public string HtmlUrl { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
