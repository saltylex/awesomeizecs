using AwesomeizeCS.Data;
using AwesomeizeCS.Domain;
using AwesomeizeCS.Models;
using AwesomeizeCS.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using TechnologySandbox.Data;

namespace AwesomeizeCS.Repositories;

public class CoursesRepository : ICoursesRepository
{
    private readonly ApplicationDbContext _db;

    public CoursesRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public ValueTask<Course?> GetCourseById(Guid? id)
    {
        return _db.Course.FindAsync(id);
    }

    public Task<Course?> GetCourseOrDefault(Guid? id)
    {
        return _db.Course.FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<List<CoursesTeacherNameViewModel>> GetAllCourses()
    {
        var courseViewModel = await (from c in _db.Course
                                     join user in _db.Users on c.MainTeacherId.ToString() equals user.Id into userGroup
                                     from user in userGroup.DefaultIfEmpty()
                                     select new CoursesTeacherNameViewModel
                                     {
                                         CourseInfo = c,
                                         MainTeacher = user != null ? user.UserName : "No Teacher Assigned Yet"
                                     }).ToListAsync();
        return courseViewModel;
    }

    public Task<List<Course>> GetCoursesByMainTeacher(Guid id)
    {
        return _db.Course.Where(c => c.MainTeacherId == id).Include(c => c.Assignments)
            .Include(c => c.EnrolledStudents)
            .Include(c => c.TimeTable).OrderBy(c=>c.Name).ToListAsync();
    }

    public bool CourseExists(Guid id)
    {
        return _db.Course.Any(a => a.Id == id);
    }

    public async Task CreateCourse(Course course)
    {
        course.Id = Guid.NewGuid();
        _db.Add(course);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateCourse(Course course)
    {
        _db.Update(course);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteCourse(Course course)
    {
        _db.Course.Remove(course);
        await _db.SaveChangesAsync();
    }

    public async Task<List<StudentSituationViewModel>> GetStudentSituationData()
    {
        var studentSituation = await (from sc in _db.StudentCourse
                                      join c in _db.Course on sc.Course.Id equals c.Id
                                      join s in _db.Students on sc.Student.Id equals s.Id orderby c.Name
                                      select new StudentSituationViewModel
                                      {
                                          Id = new Guid(),
                                          Course = c,
                                          Student = s,
                                          StudentCourse = sc,
                                          StudentName = s.FirstName + " " + s.LastName,
                                          CourseName = c.Name,
                                          AssignmentsGrades = new List<string>(), //(from sa in _db.StudentAssignment
                                                               //where sa.Student.Id == s.Id
                                                               //select sa.Grade.ToString()).ToList(),


                                      }
                                      ).ToListAsync();


        foreach (var student in studentSituation)
        {
           var assignmentSituation =  await ( from a in _db.Assignment
                                              join  sa in _db.StudentAssignment on a.Id equals sa.Assignment.Id
                                              where a.Course.Id == student.Course.Id && sa.Student.Id == student.Student.Id
                                              orderby a.Name
                                              select new
                                              {
                                                  Data = a.Name + ": " + (decimal)(sa.Grade + sa.Bonus),
                                              }).ToListAsync();
            foreach(var assignment in assignmentSituation)
            {
                student.AssignmentsGrades.Add(assignment.Data);
            }

            var attendanceSituation = await (from at in _db.Attendance
                                             join t in _db.TimeTable on at.Time.Id equals t.Id
                                             where at.StudentCourse.Id == student.StudentCourse.Id
                                             select new
                                             {
                                                 Type = t.Type,
                                             }
                                             ).ToListAsync();
            foreach(var attendance in attendanceSituation)
            {
                switch (attendance.Type)
                {
                    case InstructionType.Course:
                        student.AttendanceCountCourse++;
                        break;
                    case InstructionType.Seminar:
                        student.AttendanceCountSeminary++;
                        break;
                    case InstructionType.Laboratory:
                        student.AttendanceCountLaboratory++;
                        break;
                }
            }
        }
        return studentSituation;
    }
}