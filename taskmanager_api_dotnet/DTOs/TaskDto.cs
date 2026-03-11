namespace taskmanager_api_dotnet.DTOs
{
    public class TaskRequestDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsCompleted { get; set; }
        public int UserId { get; set; }
    }

    public class TaskResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsCompleted { get; set; }
        public int UserId { get; set; }
    }
}
