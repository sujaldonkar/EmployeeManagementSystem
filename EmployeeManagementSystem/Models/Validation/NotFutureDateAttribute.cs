
using System;
using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementSystem.Models.Validation
{
    public class NotFutureDateAttribute : ValidationAttribute
    {
        public NotFutureDateAttribute()
        {
            ErrorMessage = "Date of birth cannot be in the future.";
        }

        public override bool IsValid(object? value)
        {
            if (value == null)
                return true;

            if (value is DateTime date)
                return date.Date <= DateTime.Today;

            return false;
        }
    }
}
