using EmployeeManagementSystem.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EmployeeManagementSystem.ViewModels
{
    public class EmployeeFormViewModel
    {
        public Employee Employee { get; set; } = new Employee();

        public string? SelectedCountryCode { get; set; }

        public string? SelectedStateCode { get; set; }

        public IEnumerable<LocationDropdownItem> Countries { get; set; }
            = new List<LocationDropdownItem>();

        public IEnumerable<LocationDropdownItem> States { get; set; }
            = new List<LocationDropdownItem>();

        public IEnumerable<SelectListItem> Cities { get; set; }
            = new List<SelectListItem>();

        public IEnumerable<SelectListItem> Qualifications { get; set; }
            = new List<SelectListItem>();
    }
}