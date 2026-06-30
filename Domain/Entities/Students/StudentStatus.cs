namespace Domain.Entities.Students
{
    public class StudentStatus
    {
        public int StatusId { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }
        public ICollection<Student>Students { get; set; } = new List<Student>();
    }
}
