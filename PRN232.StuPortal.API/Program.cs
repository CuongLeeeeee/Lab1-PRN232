using Microsoft.EntityFrameworkCore;
using PRN232.StuPortal.Repositories.Data;
using PRN232.StuPortal.Repositories.Interfaces;
using PRN232.StuPortal.Repositories.Repositories;
using PRN232.StuPortal.Services.Interfaces;
using PRN232.StuPortal.Services.Services;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// -- Repositories -------------------------------------------------------------
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<ISemesterRepository, SemesterRepository>();
builder.Services.AddScoped<ISubjectRepository, SubjectRepository>();
builder.Services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();

// -- Services ------------------------------------------------------------------
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<ISemesterService, SemesterService>();
builder.Services.AddScoped<ISubjectService, SubjectService>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();

// -- Controllers + Swagger ----------------------------------------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "PRN232 StuPortal API",
        Version = "v1",
        Description = "Learning Management System REST API"
    });
});

var app = builder.Build();

// -- Auto migrate + seed on startup -------------------------------------------
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

// -- Swagger UI ---------------------------------------------------------------
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "PRN232 StuPortal API v1");
    c.RoutePrefix = string.Empty; // Swagger ? http://localhost/
});

app.MapControllers();
app.Run();