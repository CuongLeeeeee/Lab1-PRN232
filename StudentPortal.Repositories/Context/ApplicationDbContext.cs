using Microsoft.EntityFrameworkCore;
using StudentPortal.Repositories.Configurations;
using StudentPortal.Repositories.Entities;

namespace StudentPortal.Repositories.Context;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Semester> Semesters => Set<Semester>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Subject> Subjects => Set<Subject>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<CourseSubject> CourseSubjects => Set<CourseSubject>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new SemesterConfiguration());
        modelBuilder.ApplyConfiguration(new CourseConfiguration());
        modelBuilder.ApplyConfiguration(new SubjectConfiguration());
        modelBuilder.ApplyConfiguration(new StudentConfiguration());
        modelBuilder.ApplyConfiguration(new CourseSubjectConfiguration());
        modelBuilder.ApplyConfiguration(new EnrollmentConfiguration());

        // Seed data
        modelBuilder.Entity<Semester>().HasData(
            new Semester { SemesterId = 1, SemesterName = "Spring 2025", StartDate = new DateTime(2025, 1, 6), EndDate = new DateTime(2025, 5, 23) },
            new Semester { SemesterId = 2, SemesterName = "Fall 2025",   StartDate = new DateTime(2025, 8, 18), EndDate = new DateTime(2025, 12, 19) }
        );

        modelBuilder.Entity<Subject>().HasData(
            new Subject { SubjectId = 1, SubjectCode = "PRN232",  SubjectName = "ASP.NET Web API",      Credit = 3 },
            new Subject { SubjectId = 2, SubjectCode = "PRJ301",  SubjectName = "Java Web Application", Credit = 3 },
            new Subject { SubjectId = 3, SubjectCode = "SWD392",  SubjectName = "Software Development", Credit = 3 }
        );
    }
}
