using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentPortal.Repositories.Entities;

namespace StudentPortal.Repositories.Configurations;

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.HasKey(c => c.CourseId);
        builder.Property(c => c.CourseName).IsRequired().HasMaxLength(200);
        builder.Property(c => c.SemesterId).IsRequired();
    }
}
