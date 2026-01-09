using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Neksara.Data;
using Neksara.Models;

namespace Neksara.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminArchiveController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const int PAGE_SIZE = 5;

        public AdminArchiveController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================
        // LIST ARCHIVE
        // =========================
        public async Task<IActionResult> Index(int page = 1, int? categoryId = null)
        {
            var query = _context.ArchiveTopics
                .Include(a => a.Category)
                .AsQueryable();

            if (categoryId.HasValue)
                query = query.Where(a => a.CategoryId == categoryId.Value);

            int totalData = await query.CountAsync();

            var data = await query
                .OrderByDescending(a => a.ArchivedAt)
                .Skip((page - 1) * PAGE_SIZE)
                .Take(PAGE_SIZE)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = Math.Ceiling(totalData / (double)PAGE_SIZE);
            ViewBag.CategoryId = categoryId;

            ViewBag.Categories = await _context.Categories
                .Where(c => !c.IsDeleted)
                .ToListAsync();

            return View(data);
        }

        // =========================
        // RESTORE TOPIC
        // =========================
        [HttpPost]
        public async Task<IActionResult> Restore(int id)
        {
            var archive = await _context.ArchiveTopics.FindAsync(id);
            if (archive == null) return NotFound();

            _context.Topics.Add(new Topic
            {
                TopicName = archive.TopicName,
                Description = archive.Description,
                TopicPicture = archive.TopicPicture,
                VideoUrl = archive.VideoUrl,
                CategoryId = archive.CategoryId,
                CreatedAt = archive.CreatedAt,
                UpdatedAt = archive.UpdatedAt,
                ViewCount = archive.ViewCount,
                IsDeleted = false
            });

            _context.ArchiveTopics.Remove(archive);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // HARD DELETE (PERMANEN)
        // =========================
            [HttpPost]
            public async Task<IActionResult> HardDeleteArchive(int id)
        {
            var archive = await _context.ArchiveTopics.FindAsync(id);
            if (archive == null) return NotFound();

            _context.ArchiveTopics.Remove(archive);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Archive telah berhasil dihapus";

            return RedirectToAction(nameof(Index));
        }
    }
}
