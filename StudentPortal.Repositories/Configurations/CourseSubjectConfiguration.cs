using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentPortal.Repositories.Entities;

namespace StudentPortal.Repositories.Configurations;

public class CourseSubjectConfiguration : IEntityTypeConfiguration<CourseSubject>
{
    public void Configure(EntityTypeBuilder<CourseSubject> builder)
    {
        builder.HasKey(cs => new { cs.CourseId, cs.SubjectId });

        builder.HasOne(cs => cs.Course)
               .WithMany(c => c.CourseSubjects)
               .HasForeignKey(cs => cs.CourseId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(cs => cs.Subject)
               .WithMany(s => s.CourseSubjects)
               .HasForeignKey(cs => cs.SubjectId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
