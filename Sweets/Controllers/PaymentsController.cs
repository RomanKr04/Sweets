using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Sweets.Models;

namespace Sweets.Controllers
{
    public class PaymentsController : BaseController
    {
        private readonly SweetContext _context;

        public PaymentsController(SweetContext context) : base(context)
        {
            _context = context;
        }


        // GET: Payments
        public async Task<IActionResult> Index()
        {
            ViewBag.CreditList = new SelectList(await _context.Credits.ToListAsync(), "Id", "Id");
            return View(await _context.Payments.ToListAsync());
        }

        // GET: Payments/Details/5/1 (creditId/paymentNumber)
        public async Task<IActionResult> Details(int creditId, int paymentNumber)
        {
            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.CreditId == creditId && p.PaymentNumber == paymentNumber);

            if (payment == null)
            {
                return NotFound();
            }

            return View(payment);
        }

        // GET: Payments/Create
        public IActionResult Create()
        {
            ViewData["CreditId"] = new SelectList(_context.Credits, "Id", "Id");
            return View();
        }

        // POST: Payments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CreditId,PaymentNumber,PaymentDate,CreditPart,Interest,TotalPayment,RemainingCredit,OverdueDays,PenaltyAmount,TotalWithPenalty")] Payment payment)
        {
            if (ModelState.IsValid)
            {
                // Автоматически определяем следующий номер платежа для данного кредита
                if (payment.PaymentNumber == 0)
                {
                    var lastPayment = await _context.Payments
                        .Where(p => p.CreditId == payment.CreditId)
                        .OrderByDescending(p => p.PaymentNumber)
                        .FirstOrDefaultAsync();

                    payment.PaymentNumber = lastPayment != null ? lastPayment.PaymentNumber + 1 : 1;
                }

                _context.Add(payment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CreditId"] = new SelectList(_context.Credits, "Id", "Id", payment.CreditId);
            return View(payment);
        }

        // GET: Payments/Edit/5/1 (creditId/paymentNumber)
        public async Task<IActionResult> Edit(int creditId, int paymentNumber)
        {
            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.CreditId == creditId && p.PaymentNumber == paymentNumber);

            if (payment == null)
            {
                return NotFound();
            }

            ViewData["CreditId"] = new SelectList(_context.Credits, "Id", "Id", payment.CreditId);
            return View(payment);
        }

        // POST: Payments/Edit/5/1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int creditId, int paymentNumber, [Bind("CreditId,PaymentNumber,PaymentDate,CreditPart,Interest,TotalPayment,RemainingCredit,OverdueDays,PenaltyAmount,TotalWithPenalty")] Payment payment)
        {
            if (creditId != payment.CreditId || paymentNumber != payment.PaymentNumber)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(payment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PaymentExists(payment.CreditId, payment.PaymentNumber))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CreditId"] = new SelectList(_context.Credits, "Id", "Id", payment.CreditId);
            return View(payment);
        }

        // GET: Payments/Delete/5/1
        public async Task<IActionResult> Delete(int creditId, int paymentNumber)
        {
            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.CreditId == creditId && p.PaymentNumber == paymentNumber);

            if (payment == null)
            {
                return NotFound();
            }

            return View(payment);
        }

        // POST: Payments/Delete/5/1
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int creditId, int paymentNumber)
        {
            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.CreditId == creditId && p.PaymentNumber == paymentNumber);

            if (payment != null)
            {
                _context.Payments.Remove(payment);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool PaymentExists(int creditId, int paymentNumber)
        {
            return _context.Payments.Any(p => p.CreditId == creditId && p.PaymentNumber == paymentNumber);
        }

        // POST: Payments/MakePayment/5
        [HttpPost]
        public async Task<IActionResult> MakePayment(int creditId, DateTime paymentDate)
        {
            try
            {
                var creditExists = await _context.Credits
                    .AsNoTracking()
                    .AnyAsync(c => c.Id == creditId);

                if (!creditExists)
                {
                    TempData["Error"] = $"Кредит с ID {creditId} не найден";
                    return RedirectToAction(nameof(Index));
                }

                await _context.MakePaymentAsync(creditId, paymentDate);

                var payment = await _context.Payments
                    .Where(p => p.CreditId == creditId)
                    .OrderByDescending(p => p.PaymentNumber)
                    .FirstOrDefaultAsync();

                TempData["Message"] = payment != null
                    ? $"Платёж №{payment.PaymentNumber} успешно создан на сумму {payment.TotalWithPenalty} сом."
                    : "Платёж успешно создан.";
            }
            catch (PostgresException ex)
            {
                if (ex.MessageText.Contains("Недостаточно средств в бюджете"))
                {
                    TempData["Error"] = ex.MessageText;
                }
                else
                {
                    TempData["Error"] = $"Ошибка платежа: {ex.MessageText}";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Непредвиденная ошибка: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}