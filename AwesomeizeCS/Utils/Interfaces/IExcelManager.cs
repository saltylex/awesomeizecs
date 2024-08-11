using AwesomeizeCS.Domain;
using AwesomeizeCS.Models;

namespace AwesomeizeCS.Utils.Interfaces
{
    public interface IExcelManager
    {
        public Task GenerateClassSituationExcelAsync(List<StudentSituationViewModel> studentSituation, string courseName);
        public Task<List<StudentCourseViewModel>> GenereateListOfStudentCourses(string filePath);
    }
}
