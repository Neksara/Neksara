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

            // also fetch archived categories to display separately
            ViewBag.ArchiveCategories = await _context.ArchiveCategories
                .OrderByDescending(a => a.ArchivedAt)
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

            // determine category for restore: prefer original CategoryId, otherwise find or create by name
            int restoredCategoryId;
            if (archive.CategoryId.HasValue)
            {
                var originalCat = await _context.Categories.FindAsync(archive.CategoryId.Value);
                if (originalCat != null)
                {
                    if (originalCat.IsDeleted)
                    {
                        originalCat.IsDeleted = false;
                        originalCat.UpdatedAt = DateTime.Now;
                        _context.Categories.Update(originalCat);
                    }
                    restoredCategoryId = originalCat.CategoryId;

                    // If category also exists in ArchiveCategories, remove it (we're restoring it)
                    var archiveCat = await _context.ArchiveCategories.FirstOrDefaultAsync(ac => ac.OriginalCategoryId == originalCat.CategoryId || ac.CategoryName == originalCat.CategoryName);
                    if (archiveCat != null)
                    {
                        _context.ArchiveCategories.Remove(archiveCat);
                    }
                }
                else
                {
                    // original category row doesn't exist anymore; fall back to snapshot name
                    var existingByName = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryName == archive.CategoryName && !c.IsDeleted);
                    if (existingByName == null)
                    {
                        var newCat = new Category
                        {
                            CategoryName = string.IsNullOrWhiteSpace(archive.CategoryName) ? "Uncategorized" : archive.CategoryName,
                            Description = string.Empty,
                            CategoryPicture = archive.CategoryPicture,
                            CreatedAt = DateTime.Now,
                            IsDeleted = false
                        };
                        _context.Categories.Add(newCat);
                        await _context.SaveChangesAsync();
                        restoredCategoryId = newCat.CategoryId;
                        // If an ArchiveCategory exists for this name/original id, remove it
                        var archiveCatByName = await _context.ArchiveCategories.FirstOrDefaultAsync(ac => ac.OriginalCategoryId == archive.CategoryId || ac.CategoryName == archive.CategoryName);
                        if (archiveCatByName != null)
                        {
                            _context.ArchiveCategories.Remove(archiveCatByName);
                        }
                    }
                    else
                    {
                        restoredCategoryId = existingByName.CategoryId;
                    }
                }
            }
            else
            {
                var existing = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryName == archive.CategoryName && !c.IsDeleted);
                if (existing == null)
                {
                    var newCat = new Category
                    {
                        CategoryName = string.IsNullOrWhiteSpace(archive.CategoryName) ? "Uncategorized" : archive.CategoryName,
                        Description = string.Empty,
                        CategoryPicture = archive.CategoryPicture,
                        CreatedAt = DateTime.Now,
                        IsDeleted = false
                    };
                    _context.Categories.Add(newCat);
                    await _context.SaveChangesAsync();
                    restoredCategoryId = newCat.CategoryId;
                    // Remove archive category entry if present
                    var archiveCatByName2 = await _context.ArchiveCategories.FirstOrDefaultAsync(ac => ac.CategoryName == archive.CategoryName || ac.OriginalCategoryId == archive.CategoryId);
                    if (archiveCatByName2 != null)
                    {
                        _context.ArchiveCategories.Remove(archiveCatByName2);
                    }
                }
                else
                {
                    restoredCategoryId = existing.CategoryId;
                }
            }

            _context.Topics.Add(new Topic
            {
                TopicName = archive.TopicName,
                Description = archive.Description,
                TopicPicture = archive.TopicPicture,
                VideoUrl = archive.VideoUrl,
                CategoryId = restoredCategoryId,
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
        // RESTORE CATEGORY
        // =========================
        [HttpPost]
        public async Task<IActionResult> RestoreCategory(int id)
        {
            var archive = await _context.ArchiveCategories.FindAsync(id);
            if (archive == null) return NotFound();

            _context.Categories.Add(new Category
            {
                CategoryName = archive.CategoryName,
                Description = archive.Description,
                CategoryPicture = archive.CategoryPicture,
                CreatedAt = archive.CreatedAt,
                UpdatedAt = archive.UpdatedAt ?? archive.ArchivedAt,
                IsDeleted = false
            });

            _context.ArchiveCategories.Remove(archive);
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

        // =========================
        // HARD DELETE CATEGORY ARCHIVE
        // =========================
        [HttpPost]
        public async Task<IActionResult> HardDeleteArchiveCategory(int id)
        {
            var archive = await _context.ArchiveCategories.FindAsync(id);
            if (archive == null) return NotFound();

            _context.ArchiveCategories.Remove(archive);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Archive kategori telah berhasil dihapus";
            return RedirectToAction(nameof(Index));
        }
    }
}
