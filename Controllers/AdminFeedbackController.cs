using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Neksara.Data;
using Neksara.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

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
    public async Task<IActionResult> Index(
        int? categoryId,
        int? rating,
        string status,
        string sort,
        int page = 1)
    {
        int pageSize = 10;

        var baseQuery =
            from f in _context.Feedbacks
            join t in _context.Topics on f.TargetId equals t.TopicId
            join c in _context.Categories on t.CategoryId equals c.CategoryId
            where f.TargetType == "Topic" && !t.IsDeleted
            select new { f, t, c };

        // ===== FILTER =====
        if (categoryId.HasValue)
            baseQuery = baseQuery.Where(x => x.c.CategoryId == categoryId.Value);

        if (rating.HasValue)
            baseQuery = baseQuery.Where(x => x.f.Rating == rating.Value);

        if (!string.IsNullOrEmpty(status))
        {
            if (status == "approved")
                baseQuery = baseQuery.Where(x => x.f.IsApproved);
            else if (status == "pending")
                baseQuery = baseQuery.Where(x => !x.f.IsApproved);
        }

        var query = baseQuery.Select(x => new AdminFeedbackVM
        {
            FeedbackId = x.f.FeedbackId,
            Name = x.f.Name ?? "Anon",
            Role = x.f.Role,
            TopicName = x.t.TopicName,
            CategoryId = x.c.CategoryId,
            CategoryName = x.c.CategoryName,
            Rating = x.f.Rating,
            Description = x.f.Description,
            Status = x.f.IsApproved ? "Approved" : "Pending"
        });

        // ===== SORT =====
        query = sort switch
        {
            "name_az" => query.OrderBy(x => x.Name),
            "name_za" => query.OrderByDescending(x => x.Name),
            _ => query.OrderByDescending(x => x.FeedbackId)
        };

        int totalItems = await query.CountAsync();

        var data = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.Categories = await _context.Categories
            .Where(c => !c.IsDeleted)
            .OrderBy(c => c.CategoryName)
            .ToListAsync();

        ViewBag.CategoryId = categoryId;
        ViewBag.SelectedRating = rating;
        ViewBag.SelectedStatus = status;
        ViewBag.Sort = sort;
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        return View(data);
    }

    // =========================
    // APPROVE SELECTED
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
    // HIDE SELECTED
    // =========================
    [HttpPost]
    public async Task<IActionResult> HideSelected(List<int> selectedIds)
    {
        var feedbacks = await _context.Feedbacks
            .Where(f => selectedIds.Contains(f.FeedbackId))
            .ToListAsync();

        foreach (var f in feedbacks)
        {
            f.IsVisible = false;
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
}