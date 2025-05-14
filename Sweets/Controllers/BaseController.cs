using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Sweets.Models;

namespace Sweets.Controllers
{
    public class BaseController : Controller
    {
        private readonly SweetContext _sweetContext;

        public BaseController(SweetContext context)
        {
            _sweetContext = context;
        }
            
        private async Task<decimal> GetBudgetAsync()
        {
            var budget = await _sweetContext.Budgets.FirstOrDefaultAsync();
            return budget?.TotalAmount ?? 0;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            ViewBag.Budget = await GetBudgetAsync();
            await next(); 
        }
    }
}
