using StudentPortal.Repositories.Implementations;
using StudentPortal.Repositories.Interfaces;
using StudentPortal.Services.Implementations;
using StudentPortal.Services.Interfaces;

namespace StudentPortal.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<ISemesterRepository, SemesterRepository>();
        services.AddScoped<ICourseRepository,   CourseRepository>();
        services.AddScoped<ISubjectRepository,  SubjectRepository>();
        services.AddScoped<IStudentRepository,  StudentRepository>();
        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ISemesterService, SemesterService>();
        services.AddScoped<ICourseService,   CourseService>();
        services.AddScoped<ISubjectService,  SubjectService>();
        services.AddScoped<IStudentService,  StudentService>();
        return services;
    }
}
