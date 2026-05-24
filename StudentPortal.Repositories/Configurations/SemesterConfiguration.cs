using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentPortal.Repositories.Entities;

namespace StudentPortal.Repositories.Configurations;

public class SemesterConfiguration : IEntityTypeConfiguration<Semester>
{
    public void Configure(EntityTypeBuilder<Semester> builder)
    {
        builder.HasKey(s => s.SemesterId);
        builder.Property(s => s.SemesterName).IsRequired().HasMaxLength(100);
        builder.Property(s => s.StartDate).IsRequired();
        builder.Property(s => s.EndDate).IsRequired();

        builder.HasMany(s => s.Courses)
               .WithOne(c => c.Semester)
               .HasForeignKey(c => c.SemesterId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
