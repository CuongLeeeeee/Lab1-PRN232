using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentPortal.Repositories.Entities;

namespace StudentPortal.Repositories.Configurations;

public class SubjectConfiguration : IEntityTypeConfiguration<Subject>
{
    public void Configure(EntityTypeBuilder<Subject> builder)
    {
        builder.HasKey(s => s.SubjectId);
        builder.Property(s => s.SubjectCode).IsRequired().HasMaxLength(20);
        builder.HasIndex(s => s.SubjectCode).IsUnique();
        builder.Property(s => s.SubjectName).IsRequired().HasMaxLength(200);
        builder.Property(s => s.Credit).IsRequired();
    }
}
