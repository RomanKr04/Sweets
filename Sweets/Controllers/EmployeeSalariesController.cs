using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using Sweets.Models;

namespace Sweets.Controllers
{
    public class EmployeeSalariesController : BaseController
    {
        private readonly SweetContext _context;
        private readonly ILogger<EmployeeSalariesController> _logger;

        public EmployeeSalariesController(SweetContext context, ILogger<EmployeeSalariesController> logger) : base(context)
        {
            _context = context;
            _logger = logger;
        }

        // GET: EmployeeSalaries
        [HttpGet]
        public async Task<IActionResult> Index(DateTime? selectedDate)
        {
            
            if (!selectedDate.HasValue)
            {
                selectedDate = DateTime.Now;
            }

            
            var year = selectedDate.Value.Year;
            var month = selectedDate.Value.Month;

            var existingCalculation = await _context.EmployeeSalaries
                .Where(es => es.Year == year && es.Month == month)
                .FirstOrDefaultAsync();

            
            if (existingCalculation != null)
            {
                ViewBag.Message = "Зарплата за выбранный месяц уже была рассчитана.";
            }
            else
            {
               
                await CalculateEmployeeSalariesAsync(year, month);
                ViewBag.Message = "Зарплата успешно рассчитана и сохранена.";
            }

            
            var salaries = await _context.EmployeeSalaries
                .Where(es => es.Year == year && es.Month == month)
                .Include(es => es.Employee)
                .ToListAsync();

            ViewBag.SelectedDate = selectedDate;
            var totalSalarySum = salaries.Sum(s => s.TotalSalary);
            ViewBag.TotalSalarySum = totalSalarySum;
            return View(salaries);
        }
      

        // GET: EmployeeSalaries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var employeeSalary = await _context.EmployeeSalaries
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employeeSalary == null)
                return NotFound();

            return View(employeeSalary);
        }

        // GET: EmployeeSalaries/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: EmployeeSalaries/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,EmployeeId,Year,Month,TotalPurchases,TotalManufacturing,TotalSales,TotalAllParticipations,BonusTotal,TotalSalary,Status")] EmployeeSalary employeeSalary)
        {
            if (ModelState.IsValid)
            {
                _context.Add(employeeSalary);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(employeeSalary);
        }

        // GET: EmployeeSalaries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var employeeSalary = await _context.EmployeeSalaries.FindAsync(id);
            if (employeeSalary == null)
                return NotFound();

            return View(employeeSalary);
        }

        // POST: EmployeeSalaries/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EmployeeSalary employeeSalary)
        {
           
            if (id != employeeSalary.Id)
            {
                return NotFound();
            }

            try
            {
               
                var existingSalary = await _context.EmployeeSalaries.FindAsync(id);

                if (existingSalary == null)
                {
                    return NotFound();
                }

                
                var success = await UpdateEmployeeTotalSalaryAsync(
                    employeeSalary.EmployeeId,
                    employeeSalary.Year,
                    employeeSalary.Month,
                    employeeSalary.TotalSalary
                );

                if (!success)
                {
                    TempData["ErrorMessage"] = "Ошибка при обновлении зарплаты.";
                    return RedirectToAction(nameof(Index));
                }
                existingSalary.TotalSalary = employeeSalary.TotalSalary;
                _context.Update(existingSalary);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Зарплата успешно обновлена.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при редактировании зарплаты.");
                TempData["ErrorMessage"] = $"Ошибка при редактировании зарплаты: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }


        // GET: EmployeeSalaries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var employeeSalary = await _context.EmployeeSalaries
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employeeSalary == null)
                return NotFound();

            return View(employeeSalary);
        }

        // POST: EmployeeSalaries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employeeSalary = await _context.EmployeeSalaries.FindAsync(id);
            if (employeeSalary != null)
                _context.EmployeeSalaries.Remove(employeeSalary);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeSalaryExists(int id)
        {
            return _context.EmployeeSalaries.Any(e => e.Id == id);
        }

        // POST: EmployeeSalaries/ToggleStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var employeeSalary = await _context.EmployeeSalaries.FindAsync(id);
            if (employeeSalary == null)
                return NotFound();

            if (!employeeSalary.Status)
            {
                var budget = await _context.Budgets.FirstOrDefaultAsync();
                if (budget.TotalAmount < employeeSalary.TotalSalary)
                {
                    TempData["ErrorMessage"] = "Не хватает баланса бюджета";
                    return RedirectToAction(nameof(Index));
                }

                budget.TotalAmount -= employeeSalary.TotalSalary;
                employeeSalary.Status = true;

                _context.Update(budget);
                _context.Update(employeeSalary);
                await _context.SaveChangesAsync();
            }
            else
            {
                TempData["ErrorMessage"] = "Зарплата уже выдана";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: EmployeeSalaries/PayAllSalaries
       [HttpPost]
public async Task<IActionResult> PayAllSalaries(int year, int month)
{
    if (year == 0 || month == 0)
    {
        TempData["ErrorMessage"] = "Дата не выбрана.";
        return RedirectToAction(nameof(Index));
    }
    var allSalaries = await _context.EmployeeSalaries
        .Where(s => s.Year == year && s.Month == month)
        .ToListAsync();

    if (!allSalaries.Any())
    {
        TempData["ErrorMessage"] = "Нет зарплат для выбранного месяца.";
        return RedirectToAction(nameof(Index), new { selectedDate = $"{year}-{month:D2}" });
    }
    var unpaidSalaries = allSalaries.Where(s => !s.Status).ToList();

    if (!unpaidSalaries.Any())
    {
        TempData["ErrorMessage"] = "Зарплата за выбранный месяц уже полностью выплачена.";
        return RedirectToAction(nameof(Index), new { selectedDate = $"{year}-{month:D2}" });
    }
    using (var transaction = await _context.Database.BeginTransactionAsync())
    {
        try
        {
            
            foreach (var salary in unpaidSalaries)
            {
                salary.Status = true;
            }

                    await PayAllEmployeeSalariesAsync(year, month);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            TempData["SuccessMessage"] = "Зарплата за месяц успешно выплачена всем сотрудникам.";
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Ошибка при выплате зарплат.");
            TempData["ErrorMessage"] = $"Ошибка при выплате зарплат: {ex.Message}";
        }
    }

    return RedirectToAction(nameof(Index), new { selectedDate = $"{year}-{month:D2}" });
}

        // ===== Вызов хранимых процедур =====

        /// <summary>
        /// Процедура расчета зарплат сотрудников за год и месяц.
        /// </summary>
        private async Task CalculateEmployeeSalariesAsync(int year, int month)
        {
            var sql = "CALL public.calculate_employee_salaries(@p0, @p1)";
            try
            {
                await _context.Database.ExecuteSqlRawAsync(sql, year, month);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при расчете зарплат.");
                TempData["ErrorMessage"] = $"Ошибка при расчете зарплат: {ex.Message}";
            }
        }

        /// <summary>
        /// Процедура массовой выплаты зарплат за месяц.
        /// </summary>
        private async Task PayAllEmployeeSalariesAsync(int year, int month)
        {
            var sql = "CALL public.payout_salaries(@p0, @p1)";
            try
            {
                await _context.Database.ExecuteSqlRawAsync(sql, year, month);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при выплате зарплат.");
                TempData["ErrorMessage"] = $"Ошибка при выплате зарплат: {ex.Message}";
            }
        }

        /// <summary>
        /// Процедура обновления итоговой суммы выплаты сотрудника.
        /// </summary>

        private async Task<bool> UpdateEmployeeTotalSalaryAsync(int employeeId, int year, int month, decimal newTotalSalary)
        {
            try
            {
                var sql = "CALL update_employee_total_salary(@p_employee_id, @p_year, @p_month, @p_new_total_salary)";

                var parameters = new object[]
                {
            new Npgsql.NpgsqlParameter("p_employee_id", employeeId),
            new Npgsql.NpgsqlParameter("p_year", year),
            new Npgsql.NpgsqlParameter("p_month", month),
            new Npgsql.NpgsqlParameter("p_new_total_salary", newTotalSalary)
                };

                await _context.Database.ExecuteSqlRawAsync(sql, parameters);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении зарплаты.");
                return false;
            }
        }


    }
}
