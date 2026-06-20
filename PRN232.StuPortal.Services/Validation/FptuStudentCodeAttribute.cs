using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace PRN232.StuPortal.Services.Validation
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class FptuStudentCodeAttribute : ValidationAttribute
    {
        private static readonly Regex Pattern = new(
            @"^(SE|CE|BA|SS|MC|IT|AI|GD|HM|FA|LA|TA|EN|JA|KO|FR|DE|ES|VI)\d{5}$",
            RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

        public FptuStudentCodeAttribute()
            => ErrorMessage = "Student code must follow FPTU format (e.g. SE19886).";

        public override bool IsValid(object? value)
        {
            if (value is not string code || string.IsNullOrWhiteSpace(code))
                return false;

            return Pattern.IsMatch(code.Trim());
        }
    }
}
