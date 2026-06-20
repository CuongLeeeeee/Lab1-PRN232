using PRN232.StuPortal.Repositories.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PRN232.StuPortal.Repositories.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Student> Students => Set<Student>();

        public DbSet<Course> Courses => Set<Course>();

        public DbSet<Semester> Semesters => Set<Semester>();

        public DbSet<Subject> Subjects => Set<Subject>();

        public DbSet<Enrollment> Enrollments => Set<Enrollment>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // -- Constraints ----------------------------------------------
            modelBuilder.Entity<Student>()
                .HasIndex(x => x.Email)
                .IsUnique();

            modelBuilder.Entity<Enrollment>()
                .HasOne(x => x.Student)
                .WithMany(x => x.Enrollments)
                .HasForeignKey(x => x.StudentId);

            modelBuilder.Entity<Enrollment>()
                .HasOne(x => x.Course)
                .WithMany(x => x.Enrollments)
                .HasForeignKey(x => x.CourseId);

            modelBuilder.Entity<Course>()
                .HasOne(x => x.Semester)
                .WithMany(x => x.Courses)
                .HasForeignKey(x => x.SemesterId);

            // -- Seed: 5 Semesters -----------------------------------------
            modelBuilder.Entity<Semester>().HasData(
                new Semester { SemesterId = 1, SemesterName = "Spring 2023", StartDate = new DateTime(2023, 1, 10), EndDate = new DateTime(2023, 5, 20) },
                new Semester { SemesterId = 2, SemesterName = "Summer 2023", StartDate = new DateTime(2023, 6, 1), EndDate = new DateTime(2023, 8, 31) },
                new Semester { SemesterId = 3, SemesterName = "Fall 2023", StartDate = new DateTime(2023, 9, 5), EndDate = new DateTime(2024, 1, 15) },
                new Semester { SemesterId = 4, SemesterName = "Spring 2024", StartDate = new DateTime(2024, 1, 20), EndDate = new DateTime(2024, 5, 25) },
                new Semester { SemesterId = 5, SemesterName = "Fall 2024", StartDate = new DateTime(2024, 9, 2), EndDate = new DateTime(2025, 1, 10) }
            );

            // -- Seed: 10 Subjects -----------------------------------------
            modelBuilder.Entity<Subject>().HasData(
                new Subject { SubjectId = 1, SubjectCode = "CS101", SubjectName = "Introduction to Programming", Credit = 3 },
                new Subject { SubjectId = 2, SubjectCode = "CS102", SubjectName = "Data Structures", Credit = 3 },
                new Subject { SubjectId = 3, SubjectCode = "CS201", SubjectName = "Algorithms", Credit = 3 },
                new Subject { SubjectId = 4, SubjectCode = "CS202", SubjectName = "Database Systems", Credit = 3 },
                new Subject { SubjectId = 5, SubjectCode = "CS301", SubjectName = "Web Development", Credit = 3 },
                new Subject { SubjectId = 6, SubjectCode = "CS302", SubjectName = "Software Engineering", Credit = 3 },
                new Subject { SubjectId = 7, SubjectCode = "CS401", SubjectName = "Machine Learning", Credit = 3 },
                new Subject { SubjectId = 8, SubjectCode = "CS402", SubjectName = "Computer Networks", Credit = 3 },
                new Subject { SubjectId = 9, SubjectCode = "CS403", SubjectName = "Operating Systems", Credit = 3 },
                new Subject { SubjectId = 10, SubjectCode = "CS404", SubjectName = "Cloud Computing", Credit = 3 }
            );

            // -- Seed: 20 Courses (4 per semester) ------------------------
            modelBuilder.Entity<Course>().HasData(
                new Course { CourseId = 1, CourseName = "Intro to Programming – S23", SemesterId = 1 },
                new Course { CourseId = 2, CourseName = "Data Structures – S23", SemesterId = 1 },
                new Course { CourseId = 3, CourseName = "Database Systems – S23", SemesterId = 1 },
                new Course { CourseId = 4, CourseName = "Web Development – S23", SemesterId = 1 },
                new Course { CourseId = 5, CourseName = "Algorithms – SU23", SemesterId = 2 },
                new Course { CourseId = 6, CourseName = "Software Engineering – SU23", SemesterId = 2 },
                new Course { CourseId = 7, CourseName = "Machine Learning – SU23", SemesterId = 2 },
                new Course { CourseId = 8, CourseName = "Computer Networks – SU23", SemesterId = 2 },
                new Course { CourseId = 9, CourseName = "Operating Systems – F23", SemesterId = 3 },
                new Course { CourseId = 10, CourseName = "Cloud Computing – F23", SemesterId = 3 },
                new Course { CourseId = 11, CourseName = "Intro to Programming – F23", SemesterId = 3 },
                new Course { CourseId = 12, CourseName = "Web Development – F23", SemesterId = 3 },
                new Course { CourseId = 13, CourseName = "Data Structures – SP24", SemesterId = 4 },
                new Course { CourseId = 14, CourseName = "Algorithms – SP24", SemesterId = 4 },
                new Course { CourseId = 15, CourseName = "Database Systems – SP24", SemesterId = 4 },
                new Course { CourseId = 16, CourseName = "Machine Learning – SP24", SemesterId = 4 },
                new Course { CourseId = 17, CourseName = "Software Engineering – F24", SemesterId = 5 },
                new Course { CourseId = 18, CourseName = "Computer Networks – F24", SemesterId = 5 },
                new Course { CourseId = 19, CourseName = "Operating Systems – F24", SemesterId = 5 },
                new Course { CourseId = 20, CourseName = "Cloud Computing – F24", SemesterId = 5 }
            );

            // -- Seed: 50 Students -----------------------------------------
            var students = new List<Student>();
            var firstNames = new[] { "An", "Bình", "Chi", "Dung", "Em", "Phong", "Giang", "Hoa", "Khánh", "Lan" };
            var lastNames = new[] { "Nguy?n", "Tr?n", "Lê", "Ph?m", "Hoàng", "Vu", "Ð?ng", "Bùi", "Ð?", "H?" };
            for (int i = 1; i <= 50; i++)
            {
                var fn = firstNames[(i - 1) % 10];
                var ln = lastNames[(i - 1) % 10];
                students.Add(new Student
                {
                    StudentId = i,
                    FullName = $"{ln} {fn} {i}",
                    Email = $"student{i}@lms.edu.vn",
                    DateOfBirth = new DateTime(2000 + (i % 5), (i % 12) + 1, (i % 28) + 1)
                });
            }
            modelBuilder.Entity<Student>().HasData(students);

            // -- Seed: 500 Enrollments -------------------------------------
            // M?i student dang ký 10 courses (phân b? d?u)
            var statuses = new[] { "Active", "Completed", "Dropped" };
            var enrollments = new List<Enrollment>();
            int enrollId = 1;
            for (int s = 1; s <= 50; s++)
            {
                for (int c = 1; c <= 10; c++)
                {
                    var courseId = ((s + c - 2) % 20) + 1;
                    enrollments.Add(new Enrollment
                    {
                        EnrollmentId = enrollId++,
                        StudentId = s,
                        CourseId = courseId,
                        EnrollDate = new DateTime(2023, 1, 1).AddDays((enrollId % 365)),
                        Status = statuses[enrollId % 3]
                    });
                }
            }
            modelBuilder.Entity<Enrollment>().HasData(enrollments);

            base.OnModelCreating(modelBuilder);
        }
    }
}
