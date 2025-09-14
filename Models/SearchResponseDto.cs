namespace RepoGateway.Models
{
    public class SearchResponseDto
    {
        public List<RepoItemDto> Items { get; set; } = new();
    }
}
