
using Microsoft.AspNetCore.Mvc;
using EmployeeManagementSystem.Models;
using EmployeeManagementSystem.Repositories;
using EmployeeManagementSystem.ViewModels;
using EmployeeManagementSystem.Services;

namespace EmployeeManagementSystem.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IEmployeeRepository _repository;
        private readonly CountryStateCityService _locationService;

        private readonly ILocationRepository _locationRepository;

        public EmployeeController(
        IEmployeeRepository repository,
        CountryStateCityService locationService,
        ILocationRepository locationRepository)
        {
            _repository = repository;
            _locationService = locationService;
            _locationRepository = locationRepository;
        }

        // Temporary API connection test
        [HttpGet]
        public async Task<IActionResult> TestCountryApi()
        {
            try
            {
                var countries = await _locationService.GetCountriesAsync();

                return Json(new
                {
                    Success = true,
                    Count = countries.Count,
                    SampleCountries = countries.Take(5)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Error = ex.Message
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> TestCitiesApi(
    string countryCode = "IN",
    string stateCode = "MH")
        {
            try
            {
                var cities = await _locationService.GetCitiesAsync(
                    countryCode, stateCode);

                return Json(new
                {
                    Success = true,
                    CountryCode = countryCode,
                    StateCode = stateCode,
                    Count = cities.Count,
                    SampleCities = cities.Take(10)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Error = ex.Message
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> TestStatesApi(string countryCode = "IN")
        {
            try
            {
                var states = await _locationService.GetStatesAsync(countryCode);

                return Json(new
                {
                    Success = true,
                    CountryCode = countryCode,
                    Count = states.Count,
                    SampleStates = states.Take(10)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Error = ex.Message
                });
            }
        }


        // Display and search employees
        [HttpGet]
        public async Task<IActionResult> Index(string searchTerm)
        {
            IEnumerable<Employee> employees;

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                employees = await _repository.GetAllAsync();
            }
            else
            {
                employees = await _repository.SearchAsync(searchTerm.Trim());
            }

            ViewBag.SearchTerm = searchTerm;

            return View(employees);
        }


        // Display employee details
        public async Task<IActionResult> Details(int id)
        {
            var employee = await _repository.GetByIdAsync(id);

            if (employee == null)
                return NotFound();

            return View(employee);
        }

        // Load dropdown data
        private async Task LoadDropdowns(EmployeeFormViewModel model)
        {
            // 1. Fetch countries from external API
            var apiCountries = await _locationService.GetCountriesAsync();

            // 2. Synchronize API countries with SQL Server
            await _locationRepository.SyncCountriesAsync(apiCountries);

            // 3. Load countries from SQL Server
            model.Countries = await _locationRepository.GetCountriesAsync();

            // 4. Load qualifications from SQL Server
            model.Qualifications = await _repository.GetQualificationsAsync();

            // 5. Load states for selected country
            if (model.Employee.CountryId > 0)
            {
                var countryCode =
                    await _locationRepository.GetCountryCodeByIdAsync(
                        model.Employee.CountryId);

                if (!string.IsNullOrWhiteSpace(countryCode))
                {
                    var apiStates =
                        await _locationService.GetStatesAsync(countryCode);

                    await _locationRepository.SyncStatesAsync(
                        model.Employee.CountryId, apiStates);

                    model.States =
                        await _locationRepository.GetStatesAsync(
                            model.Employee.CountryId);
                }
            }

            // 6. Load cities for selected state
            if (model.Employee.StateId > 0)
            {
                model.Cities =
                    await _locationRepository.GetCitiesAsync(
                        model.Employee.StateId);
            }
        }

        // Create - GET
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new EmployeeFormViewModel();

            await LoadDropdowns(model);

            return View(model);
        }

        // Create - POST

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeeFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdowns(model);
                return View(model);
            }

            try
            {
                await _repository.InsertAsync(model.Employee);

                TempData["Success"] = "Employee added successfully!";

                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message.Contains("Email address already exists"))
                {
                    ModelState.AddModelError(
                        "Employee.EmailAddress",
                        "This email address is already registered."
                    );
                }
                else if (ex.Message.Contains("Phone number already exists"))
                {
                    ModelState.AddModelError(
                        "Employee.PhoneNo",
                        "This phone number is already registered."
                    );
                }
                else
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "Unable to save employee. Please try again."
                    );
                }

                await LoadDropdowns(model);
                return View(model);
            }
        }


        // Get states based on selected country
        [HttpGet]
        public async Task<IActionResult> GetStates(int countryId)
        {
            var countryCode =
                await _locationRepository.GetCountryCodeByIdAsync(countryId);

            if (string.IsNullOrWhiteSpace(countryCode))
                return BadRequest("Invalid country selected.");

            var apiStates =
                await _locationService.GetStatesAsync(countryCode);

            await _locationRepository.SyncStatesAsync(countryId, apiStates);

            var states =
                await _locationRepository.GetStatesAsync(countryId);

            return Json(states);
        }
        // Get cities based on selected state
        [HttpGet]
        public async Task<IActionResult> GetCities(int stateId)
        {
            var stateCode =
                await _locationRepository.GetStateCodeByIdAsync(stateId);

            if (string.IsNullOrWhiteSpace(stateCode))
                return BadRequest("Invalid state selected.");

            var countryId = await _repository.GetCountryIdByStateAsync(stateId);

            if (countryId == null)
                return BadRequest("Country not found for selected state.");

            var countryCode =
                await _locationRepository.GetCountryCodeByIdAsync(countryId.Value);

            if (string.IsNullOrWhiteSpace(countryCode))
                return BadRequest("Invalid country code.");

            var apiCities =
                await _locationService.GetCitiesAsync(countryCode, stateCode);

            await _locationRepository.SyncCitiesAsync(stateId, apiCities);

            var cities =
                await _locationRepository.GetCitiesAsync(stateId);

            return Json(cities);
        }

        // Edit - GET
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var employee = await _repository.GetByIdAsync(id);

            if (employee == null)
                return NotFound();

            var model = new EmployeeFormViewModel
            {
                Employee = employee
            };

            await LoadDropdowns(model);

            return View(model);
        }

        // Edit - POST

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EmployeeFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdowns(model);
                return View(model);
            }

            try
            {
                var updated = await _repository.UpdateAsync(model.Employee);

                if (!updated)
                    return NotFound();

                TempData["Success"] = "Employee updated successfully!";

                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message.Contains("Email address already exists"))
                {
                    ModelState.AddModelError(
                        "Employee.EmailAddress",
                        "This email address is already registered."
                    );
                }
                else if (ex.Message.Contains("Phone number already exists"))
                {
                    ModelState.AddModelError(
                        "Employee.PhoneNo",
                        "This phone number is already registered."
                    );
                }
                else
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "Unable to update employee. Please try again."
                    );
                }

                await LoadDropdowns(model);
                return View(model);
            }
        }


        // Delete - GET
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var employee = await _repository.GetByIdAsync(id);

            if (employee == null)
                return NotFound();

            return View(employee);
        }

        // Delete - POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var deleted = await _repository.DeleteAsync(id);

            if (!deleted)
                return NotFound();

            TempData["Success"] = "Employee deleted successfully!";

            return RedirectToAction(nameof(Index));
        }
    }
}
