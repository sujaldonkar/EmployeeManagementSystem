
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EmployeeManagementSystem.Models.Validation;

namespace EmployeeManagementSystem.Models
{
    public class Employee : IValidatableObject
    {
        public int EmployeeId { get; set; }

        // First Name
        [Required(ErrorMessage = "First name is required.")]
        [StringLength(50, MinimumLength = 2,
            ErrorMessage = "First name must contain 2 to 50 characters.")]
        [RegularExpression(@"^[A-Za-z]+(?:[ '-][A-Za-z]+)*$",
            ErrorMessage = "First name can contain only letters, spaces, hyphens and apostrophes.")]
        public string FirstName { get; set; } = string.Empty;

        // Last Name
        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(50, MinimumLength = 2,
            ErrorMessage = "Last name must contain 2 to 50 characters.")]
        [RegularExpression(@"^[A-Za-z]+(?:[ '-][A-Za-z]+)*$",
            ErrorMessage = "Last name can contain only letters, spaces, hyphens and apostrophes.")]
        public string LastName { get; set; } = string.Empty;

        // Date of Birth
        [Required(ErrorMessage = "Date of birth is required.")]
        [DataType(DataType.Date)]
        [NotFutureDate]
        public DateTime DateOfBirth { get; set; }

        // Country
        [Range(1, int.MaxValue,
            ErrorMessage = "Please select a valid country.")]
        public int CountryId { get; set; }

        public string? CountryName { get; set; }

        // State
        [Range(1, int.MaxValue,
            ErrorMessage = "Please select a valid state.")]
        public int StateId { get; set; }

        public string? StateName { get; set; }

        // City
        [Range(1, int.MaxValue,
            ErrorMessage = "Please select a valid city.")]
        public int CityId { get; set; }

        public string? CityName { get; set; }

        // Qualification
        [Range(1, int.MaxValue,
            ErrorMessage = "Please select a valid qualification.")]
        public int QualificationId { get; set; }

        public string? QualificationName { get; set; }

        // Email Address
        [Required(ErrorMessage = "Email address is required.")]
        [StringLength(150,
    ErrorMessage = "Email cannot exceed 150 characters.")]
        [RegularExpression(
    @"^[a-z0-9]+(?:[._%+-][a-z0-9]+)*@[a-z0-9]+(?:-[a-z0-9]+)*(?:\.[a-z]{2,})+$",
    ErrorMessage = "Email must contain lowercase letters only, for example: name@gmail.com.")]
        public string EmailAddress { get; set; } = string.Empty;


        // Phone Number
        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^[6-9][0-9]{9}$",
            ErrorMessage = "Enter a valid 10-digit Indian mobile number.")]
        public string PhoneNo { get; set; } = string.Empty;

        // Additional Date of Birth validation
        public IEnumerable<ValidationResult> Validate(
            ValidationContext validationContext)
        {
            if (DateOfBirth == DateTime.MinValue)
            {
                yield return new ValidationResult(
                    "Please enter a valid date of birth.",
                    new[] { nameof(DateOfBirth) });
            }
            else if (DateOfBirth.Year < 1900)
            {
                yield return new ValidationResult(
                    "Date of birth must be after 1900.",
                    new[] { nameof(DateOfBirth) });
            }
        }
    }
}
