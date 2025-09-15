using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RepoGateway.Models
{
    public class RepoAnalysis
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserId { get; set; } = string.Empty;
        public string RepoId { get; set; } = string.Empty;
        public string Owner { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        public string LicenseSpdxId { get; set; } = string.Empty;
        public List<string> Topics { get; set; } = new();
        public string PrimaryLanguage { get; set; } = string.Empty;
        public int ReadmeLength { get; set; }

        public int OpenIssues { get; set; }
        public int Forks { get; set; }
        public int StarsSnapshot { get; set; }

        public int ActivityDays { get; set; }
        public string DefaultBranch { get; set; } = string.Empty;

        public double HealthScore { get; set; }
        public DateTime AnalyzedAt { get; set; } = DateTime.UtcNow;

        [JsonIgnore]   // prevent circular JSON
        public Favorite Favorite { get; set; }
    }
}
