using AwesomeizeCS.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AwesomeizeCS.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<AwesomeizeCS.Domain.Course> Course { get; set; } = default!;
        public DbSet<AwesomeizeCS.Domain.Assignment> Assignment { get; set; } = default!;
        public DbSet<AwesomeizeCS.Domain.Attendance> Attendance { get; set; } = default!;
        public DbSet<AwesomeizeCS.Domain.StudentAssignment> StudentAssignment { get; set; } = default!;
        public DbSet<AwesomeizeCS.Domain.StudentCourse> StudentCourse { get; set; } = default!;
        public DbSet<AwesomeizeCS.Domain.TimeTable> TimeTable { get; set; } = default!;
        public DbSet<AwesomeizeCS.Domain.TeacherCourse> TeacherCourse { get; set; } = default!;
        public DbSet<AwesomeizeCS.Domain.IOTest> IOTest { get; set; } = default!;
        public DbSet<AwesomeizeCS.Domain.TestStep> TestStep { get; set; } = default!;
        public DbSet<AwesomeizeCS.Domain.TestResult> TestResult { get; set; } = default!;
        public DbSet<AwesomeizeCS.Domain.CodeVersion> CodeVersion { get; set; } = default!;
    }
}