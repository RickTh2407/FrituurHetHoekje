using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FrituurHetHoekje.Data;
using FrituurHetHoekje.Models;

namespace FrituurHetHoekje.Controllers
{
    public class OrdersController : Controller
    {
        private readonly FrituurDB _context;

        public OrdersController(FrituurDB context)
        {
            _context = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var frituurDB = _context.Orders.Include(o => o.Account);
            return View(await frituurDB.ToListAsync());
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Account)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            ViewData["AccountId"] = new SelectList(_context.Accounts, "Id", "Email");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,OrderNr,TotalPrice,Date,AccountId")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AccountId"] = new SelectList(_context.Accounts, "Id", "Email", order.AccountId);
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["AccountId"] = new SelectList(_context.Accounts, "Id", "Email", order.AccountId);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,OrderNr,TotalPrice,Date,AccountId")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
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
            ViewData["AccountId"] = new SelectList(_context.Accounts, "Id", "Email", order.AccountId);
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Account)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }

        private async Task<Account> GetLoggedInAccountAsync()
        {
            var email = User.Identity?.Name;
            return await _context.Accounts.FirstOrDefaultAsync(a => a.Email == email);
        }

        [HttpGet]
        public async Task<IActionResult> PreviousOrders()
        {
            var account = await GetLoggedInAccountAsync();
            if (account == null)
            {
                return RedirectToAction("Login", "Accounts");
            }

            var previousOrders = await _context.Orders
                .Where(o => o.AccountId == account.Id)
                .Include(o => o.OrderProducts)
                .ThenInclude(o => o.Product)
                .ToListAsync();

            return View(previousOrders);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReOrder(int orderId)
        {
            var originalOrder = await _context.Orders
                .Include(o => o.OrderProducts)
                .ThenInclude(o => o.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (originalOrder == null) return NotFound();

            // Create a new order based on the original one
            var newOrder = new Order
            {
                AccountId = originalOrder.AccountId,
                OrderNr = GenerateOrderNumber(),
                Date = DateTime.Now,
                OrderProducts = originalOrder.OrderProducts.Select(op => new OrderProduct
                {
                    ProductId = op.ProductId,
                    Amount = op.Amount,
                    Discount = op.Discount
                }).ToList()
            };

            _context.Orders.Add(newOrder);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = newOrder.Id });
        }
        public int GenerateOrderNumber()
        {
            var lastOrder = _context.Orders.OrderByDescending(o => o.OrderNr).FirstOrDefault();
            return lastOrder != null ? lastOrder.OrderNr + 1 : 100000; // Start at 100000 if no orders
        }
    }
}
