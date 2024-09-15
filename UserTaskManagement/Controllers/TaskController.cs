using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UserTaskManagement.Data;
using UserTaskManagement.Models;

namespace UserTaskManagement.Controllers
{
    // Controllers/TaskController.cs
    [Authorize]
    public class TaskController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TaskController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string priorityFilter, string statusFilter, string searchString)
        {
            var tasks = from t in _context.TaskItems
                        select t;

            if (!string.IsNullOrEmpty(searchString))
            {
                tasks = tasks.Where(t => t.TaskName.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(priorityFilter))
            {
                if (Enum.TryParse(priorityFilter, out Priority priority))
                {
                    tasks = tasks.Where(t => t.Priority == priority);
                }
            }

            if (!string.IsNullOrEmpty(statusFilter))
            {
                if (Enum.TryParse(statusFilter, out Status status))
                {
                    tasks = tasks.Where(t => t.Status == status);
                }
            }
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //var userId = User.Identity.Name;
            tasks = tasks.Where(t => t.UserId == userId);

            return View(await tasks.ToListAsync());
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Users = new SelectList(_context.Users, "Id", "UserName");
            //ViewBag.Users = new SelectList(await _userManager.Users.ToListAsync(), "Id", "UserName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TaskName,Description,DueDate,Priority,Status,UserId")] TaskItem taskItem)
        {
            if (ModelState.IsValid)
            {
                //taskItem.UserId = User.Identity.Name;
                _context.Add(taskItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Users = new SelectList(_context.Users, "Id", "UserName");
            return View(taskItem);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taskItem = await _context.TaskItems.FindAsync(id);
            if (taskItem == null)
            {
                return NotFound();
            }
            ViewBag.Users = new SelectList(_context.Users, "Id", "UserName");
            //ViewBag.Users = new SelectList(await _userManager.Users.ToListAsync(), "Id", "UserName");
            return View(taskItem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TaskName,Description,DueDate,Priority,Status,UserId")] TaskItem taskItem)
        {
            if (id != taskItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(taskItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaskItemExists(taskItem.Id))
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
            ViewBag.Users = new SelectList(_context.Users, "Id", "UserName");
            return View(taskItem);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taskItem = await _context.TaskItems
                .FirstOrDefaultAsync(m => m.Id == id);
            if (taskItem == null)
            {
                return NotFound();
            }

            return View(taskItem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var taskItem = await _context.TaskItems.FindAsync(id);
            _context.TaskItems.Remove(taskItem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TaskItemExists(int id)
        {
            return _context.TaskItems.Any(e => e.Id == id);
        }
    }

}
