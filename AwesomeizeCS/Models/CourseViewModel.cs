using AwesomeizeCS.Domain;
using Microsoft.AspNetCore.Identity;

namespace AwesomeizeCS.Models;

public class CourseViewModel
{
    public List<Course> CourseInformation { get; set; }
    public List<List<string?>> TeachersPerCourse { get; set; }

    public CourseViewModel(List<Course> courses, List<List<string?>> teachers)
    {
        CourseInformation = courses;
        TeachersPerCourse = teachers;
    }
}