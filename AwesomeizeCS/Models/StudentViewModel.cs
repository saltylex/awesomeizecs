namespace AwesomeizeCS.Models
{
    public class StudentViewModel
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public string Subgroup { get; set; } = string.Empty;
    }
}
