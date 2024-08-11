namespace AwesomeizeCS.Domain
{
    public class StudentAssignment
    {
        public StudentAssignment() { }

        public StudentAssignment(Guid id, Student student, Assignment assignment)
        {
            Id = id;
            Student = student;
            Assignment = assignment;
        }

        public Guid Id { get; set; }
        public decimal? Grade { get; set; }
        public decimal? Bonus { get; set; }

        public Student Student { get; set; }
        public Assignment Assignment { get; set; }
    }
}