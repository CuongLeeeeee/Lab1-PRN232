using FluentValidation;
using PRN232.StuPortal.Services.Models.Requests;

namespace PRN232.StuPortal.Services.Validation.Validators
{
    public class CreateEnrollmentRequestValidator : AbstractValidator<CreateEnrollmentRequest>
    {
        private static readonly HashSet<string> AllowedStatuses =
            new(StringComparer.OrdinalIgnoreCase) { "Active", "Completed", "Dropped", "Pending" };

        public CreateEnrollmentRequestValidator()
        {
            RuleFor(x => x.StudentId)
                .GreaterThan(0)
                .WithMessage("StudentId must be greater than 0.");

            RuleFor(x => x.CourseId)
                .GreaterThan(0)
                .WithMessage("CourseId must be greater than 0.");

            RuleFor(x => x.EnrollDate)
                .LessThanOrEqualTo(DateTime.UtcNow.AddDays(1))
                .WithMessage("EnrollDate cannot be in the future.");

            RuleFor(x => x.Status)
                .NotEmpty()
                .Must(status => AllowedStatuses.Contains(status))
                .WithMessage("Status must be one of: Active, Completed, Dropped, Pending.");
        }
    }
}
