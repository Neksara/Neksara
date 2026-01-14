using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Neksara.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Neksara.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminFeedbackController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminFeedbackController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================
        // INDEX
        // =========================
        public async Task<IActionResult> Index(string status, int? rating)
        {
            var query =
                from f in _context.Feedbacks
                join t in _context.Topics on f.TargetId equals t.TopicId
                join c in _context.Categories on t.CategoryId equals c.CategoryId
                where f.TargetType == "Topic" && !t.IsDeleted
                select new AdminFeedbackVM
                {
                    FeedbackId = f.FeedbackId,
                    Name = f.Name ?? "Anon",
                    Role = f.Role,
                    TopicName = t.TopicName,
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                    Rating = f.Rating,
                    Description = f.Description,
                    IsApproved = f.IsApproved,
                    IsVisible = f.IsVisible
                };

            // ===== FILTER STATUS =====
            if (!string.IsNullOrEmpty(status))
            {
                query = status switch
                {
                    "pending" => query.Where(x => !x.IsApproved),
                    "approved" => query.Where(x => x.IsApproved && x.IsVisible),
                    "hidden" => query.Where(x => x.IsApproved && !x.IsVisible),
                    _ => query
                };
            }

            if (rating.HasValue)
                query = query.Where(x => x.Rating == rating.Value);

            return View(await query
                .OrderByDescending(x => x.FeedbackId)
                .ToListAsync());
        }

        // =========================
        // APPROVE
        // =========================
        [HttpPost]
        public async Task<IActionResult> ApproveSelected(List<int> selectedIds)
        {
            var feedbacks = await _context.Feedbacks
                .Where(f => selectedIds.Contains(f.FeedbackId))
                .ToListAsync();

            foreach (var f in feedbacks)
            {
                f.IsApproved = true;
                f.IsVisible = true;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // =========================
        // HIDE (ONLY APPROVED)
        // =========================
        [HttpPost]
        public async Task<IActionResult> HideSelected(List<int> selectedIds)
        {
            var feedbacks = await _context.Feedbacks
                .Where(f => selectedIds.Contains(f.FeedbackId) && f.IsApproved)
                .ToListAsync();

            foreach (var f in feedbacks)
                f.IsVisible = false;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // =========================
        // SHOW (BACK TO APPROVED)
        // =========================
        [HttpPost]
        public async Task<IActionResult> ShowSelected(List<int> selectedIds)
        {
            var feedbacks = await _context.Feedbacks
                .Where(f => selectedIds.Contains(f.FeedbackId) && f.IsApproved)
                .ToListAsync();

            foreach (var f in feedbacks)
                f.IsVisible = true;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
