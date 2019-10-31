using FluentValidation;

namespace $safeprojectname$.Dto.Validators
{
    public class PersonDTOValidator : AbstractValidator<PersonDTO>
    {
        public PersonDTOValidator()
        {
            RuleFor(o => o.FirstName).NotEmpty().WithMessage("First Name is required").WithErrorCode("1001");

            RuleFor(o => o.LastName).NotEmpty();

            RuleFor(o => o.Type).NotEmpty().Must(IsValidType).WithMessage("Type incorrect");

            RuleFor(o => o.Active).NotEmpty();

            RuleFor(x => x.FirstName).Length(0, 10);

            RuleFor(x => x.Email).EmailAddress();

            RuleFor(x => x.Age).InclusiveBetween(18, 60);

            RuleFor(o => o.Register).GreaterThan(1);
        }

        private static bool IsValidType(string type)
        {
            var isValid = type.ToUpper().Equals("EMPLOYEE") || type.ToUpper().Equals("TRAINEE");
            return isValid;
        }
    }
}
