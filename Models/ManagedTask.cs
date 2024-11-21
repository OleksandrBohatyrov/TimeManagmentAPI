namespace TimeManagmentAPI.Models
{
    public class ManagedTask
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int ProjectId { get; set; }
        public int UserId { get; set; } 
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsConfirmed { get; set; }
    }
}
