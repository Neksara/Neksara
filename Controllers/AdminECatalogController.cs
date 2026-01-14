using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Neksara.Services.Interfaces;

namespace Neksara.Controllers;

[Authorize(Roles = "Admin")]
public class AdminEcatalogController : Controller
{
    private readonly IAdminEcatalogService _service;
    private const int PageSize = 10;

    public AdminEcatalogController(IAdminEcatalogService service)
    {
        _service = service;
    }

    // =========================
    // INDEX
    // =========================
    public async Task<IActionResult> Index(
        string? search,
        string? sort,
        int page = 1)
    {
        var (data, total) = await _service.GetPagedAsync(
            search,
            sort,
            page,
            PageSize
        );

        ViewBag.Search = search;
        ViewBag.Sort = sort;
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling(total / (double)PageSize);

        return View(data);
    }

    // =========================
    // PUBLISH (BULK)
    // =========================
    [HttpPost]
    public async Task<IActionResult> Publish(int[] topicIds)
    {
        if (topicIds == null || topicIds.Length == 0)
            return RedirectToAction(nameof(Index));

        await _service.PublishAsync(topicIds);

        return RedirectToAction(nameof(Index));
    }

    // =========================
    // DELETE → ARCHIVE
    // =========================
    [HttpPost]
    public async Task<IActionResult> Delete(int[] topicIds)
    {
        if (topicIds == null || topicIds.Length == 0)
            return RedirectToAction(nameof(Index));

        await _service.ArchiveAsync(topicIds);

        return RedirectToAction(nameof(Index));
    }

    // =========================
    // DETAIL (MODAL)
    // =========================
    public async Task<IActionResult> Detail(int id)
    {
        var data = await _service.GetDetailAsync(id);
        if (data == null)
            return NotFound();

        return PartialView("DetailModal", data);
    }
    
}
