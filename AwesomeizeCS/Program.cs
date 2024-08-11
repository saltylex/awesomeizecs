using AwesomeizeCS.Data;
using AwesomeizeCS.Repositories;
using AwesomeizeCS.Repositories.Interfaces;
using AwesomeizeCS.Services;
using AwesomeizeCS.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Serilog;
using Microsoft.EntityFrameworkCore;
using AwesomeizeCS.Utils;
using static System.Formats.Asn1.AsnWriter;
using AwesomeizeCS.Utils.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog for logging to a file
Log.Logger = new LoggerConfiguration()
    .WriteTo.File("logfile.txt", rollingInterval: RollingInterval.Day)
    .MinimumLevel.Error().CreateLogger();

builder.Host.UseSerilog();
// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IAssignmentsRepository, AssignmentsRepository>();
builder.Services.AddScoped<IAssignmentsService, AssignmentsService>();
builder.Services.AddScoped<IAttendancesRepository, AttendancesRepository>();
builder.Services.AddScoped<IAttendancesService, AttendancesService>();
builder.Services.AddScoped<ICoursesRepository, CoursesRepository>();
builder.Services.AddScoped<ICoursesService, CoursesService>();
builder.Services.AddScoped<IStudentAssignmentsRepository, StudentAssignmentsRepository>();
builder.Services.AddScoped<IStudentAssignmentsService, StudentAssignmentsService>();
builder.Services.AddScoped<IStudentCoursesRepository, StudentCoursesRepository>();
builder.Services.AddScoped<IStudentCoursesService, StudentCoursesService>();
builder.Services.AddScoped<IStudentsRepository, StudentsRepository>();
builder.Services.AddScoped<IStudentsService, StudentsService>();
builder.Services.AddScoped<ITeacherCoursesRepository, TeacherCoursesRepository>();
builder.Services.AddScoped<ITeacherCoursesService, TeacherCoursesService>();
builder.Services.AddScoped<ITimeTablesRepository, TimeTablesRepository>();
builder.Services.AddScoped<ITimeTablesService, TimeTablesService>();
builder.Services.AddScoped<IExcelManager,  ExcelManager>();
builder.Services.AddScoped<ICodeVersionsRepository,  CodeVersionsRepository>();
builder.Services.AddScoped<ICodeVersionsService,  CodeVersionsService>();
builder.Services.AddScoped<ITestResultsRepository,  TestResultsRepository>();
builder.Services.AddScoped<ITestResultsService,  TestResultsService>();
builder.Services.AddScoped<IIOTestsRepository, IOTestsRepository>();
builder.Services.AddScoped<IIOTestsService,  IOTestsService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

// seeding initial roles and 3 accounts into our application, for testing purposes ONLY.
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var roles = new[] { "Admin", "Student", "Teacher" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }
//
//     var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
//
//     var emailAdmin = "admin@admin.com";
//     var emailStudent = "student@student.com";
//     var emailTeacher1 = "teacher@teacher.com";
//     var emailTeacher2 = "anotherteacher@teacher.com";
//     var password = "123Aa.";
//
//     if (await userManager.FindByEmailAsync(emailAdmin) == null)
//     {
//         var user = new IdentityUser();
//         user.UserName = emailAdmin;
//         user.Email = emailAdmin;
//
//         await userManager.CreateAsync(user, password);
//         await userManager.AddToRoleAsync(user, "Admin");
//     }
//
//     if (await userManager.FindByEmailAsync(emailStudent) == null)
//     {
//         var user = new IdentityUser();
//         user.UserName = emailStudent;
//         user.Email = emailStudent;
//
//         await userManager.CreateAsync(user, password);
//         await userManager.AddToRoleAsync(user, "Student");
//     }
//
//     if (await userManager.FindByEmailAsync(emailTeacher1) == null)
//     {
//         var user = new IdentityUser();
//         user.UserName = emailTeacher1;
//         user.Email = emailTeacher1;
//
//         await userManager.CreateAsync(user, password);
//         await userManager.AddToRoleAsync(user, "Teacher");
//     }
//     if (await userManager.FindByEmailAsync(emailTeacher2) == null)
//     {
//         var user = new IdentityUser();
//         user.UserName = emailTeacher2;
//         user.Email = emailTeacher2;
//
//         await userManager.CreateAsync(user, password);
//         await userManager.AddToRoleAsync(user, "Teacher");
//     }
}
 
async Task LoadTimeTablesAsync(WebApplication app)
{

    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        if (dbContext.TimeTable.Count() <= 0)
        {
            var timeTables = await TimeTableCrawler.GetTimeTableFromWebsiteAsync(dbContext);

            await dbContext.AddRangeAsync(timeTables);
            await dbContext.SaveChangesAsync();
        }
    }
}

// Call LoadTimeTablesAsync during application startup
await LoadTimeTablesAsync(app);


app.Run();
