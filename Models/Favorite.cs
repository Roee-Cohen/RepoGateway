using System.ComponentModel.DataAnnotations;

namespace RepoGateway.Models
{
    public class Favorite
    {
        [Key]
        public int Id { get; set; }
        public string RepoId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Owner { get; set; } = string.Empty;
        public int Stars { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string UserId { get; set; } = string.Empty;

        // Repo Analysis
        public string AnalysisState { get; set; }
        public RepoAnalysis Analysis { get; set; }
        public Guid? AnalysisId { get; set; }
    }
}
