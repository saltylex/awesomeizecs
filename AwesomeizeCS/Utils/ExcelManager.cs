using AwesomeizeCS.Models;
using AwesomeizeCS.Utils.Interfaces;
using System.Text;


namespace AwesomeizeCS.Utils
{
    public class ExcelManager : IExcelManager
    {
        public async Task GenerateClassSituationExcelAsync(List<StudentSituationViewModel> studentSituation, string courseName)
        {
            //TODO: Change viewmodel and stuff to send list of grades and list of assignments
            await Task.Run(() =>
            {
                string filePath = courseName + "_Class_Situation.csv";
                StringBuilder csvContent = new StringBuilder();

                csvContent.AppendLine("Student Name,Attendance Laboratory,Attendance Seminary,Attendance Course,Grades");

                var selectedCourseStudentSituation = studentSituation.Where(record => record.CourseName.Equals(courseName));


                foreach (var record in selectedCourseStudentSituation)
                {
                    string grades = string.Join(", ", record.AssignmentsGrades ?? new List<string>());

                    csvContent.AppendLine($"{record.StudentName},{record.AttendanceCountLaboratory},{record.AttendanceCountSeminary},{record.AttendanceCountCourse},{grades}");
                }

                File.WriteAllText(filePath, csvContent.ToString());
            });
        }
    

        public async Task<List<StudentCourseViewModel>> GenereateListOfStudentCourses(string filePath)
        {
            var studentCourseList = new List<StudentCourseViewModel>();

            try
            {
                string[] lines = await File.ReadAllLinesAsync(filePath);

                for (int i = 6; i < lines.Length; i++)
                {
                    string[] columns = lines[i].Split(',');

                    if (columns.Length >= 16) 
                    {
                        string studentEmail = columns[15].Trim();
                        string courseName = columns[8].Trim();
                        string attendingGroup = columns[6].Trim();
                        string firstName = columns[12].Trim();
                        string lastName = columns[11].Trim(); 
                        Guid guid = Guid.NewGuid();

                        if (!string.IsNullOrEmpty(studentEmail) && !string.IsNullOrEmpty(courseName) && !string.IsNullOrEmpty(attendingGroup))
                        {
                            studentCourseList.Add(new StudentCourseViewModel
                            {
                                StudentEmail = studentEmail,
                                CourseName = courseName,
                                AttendingGroup = attendingGroup,
                                StudentFirstName = firstName,
                                StudentLastName = lastName,
                                Id = guid
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error parsing CSV file: " + ex.Message);
            }

            return studentCourseList;
        }
    }
}
