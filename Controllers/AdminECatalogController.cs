using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Neksara.Services.Interfaces;

namespace Neksara.Controllers;

[Authorize(Roles = "Admin")]
public class AdminEcatalogController : Controller
{
    private readonly IAdminEcatalogService _service;
    private const int PAGE_SIZE = 10;

    public AdminEcatalogController(IAdminEcatalogService service)
    {
        _service = service;
    }

    public async Task<IActionResult> Index(
        string? search,
        string? sort,
        int page = 1)
    {
        var (data, total) = await _service
            .GetPagedAsync(search, sort, page, PAGE_SIZE);

        ViewBag.Search = search;
        ViewBag.Sort = sort;
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = Math.Ceiling(total / (double)PAGE_SIZE);

        return View(data);
    }

    [HttpPost]
    public async Task<IActionResult> Publish(int[] topicIds)
    {
        if (topicIds.Length == 0)
            return RedirectToAction(nameof(Index));

        await _service.PublishAsync(topicIds);
        return RedirectToAction(nameof(Index));
    }

    // 🔥 INI YANG KEMARIN HILANG
    [HttpPost]
    public async Task<IActionResult> Withdraw(int[] topicIds)
    {
        if (topicIds.Length == 0)
            return RedirectToAction(nameof(Index));

        await _service.WithdrawAsync(topicIds);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int[] topicIds)
    {
        if (topicIds.Length == 0)
            return RedirectToAction(nameof(Index));

        await _service.ArchiveAsync(topicIds);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Detail(int id)
    {
        var data = await _service.GetDetailAsync(id);
        if (data == null) return NotFound();

        return PartialView("_DetailModal", data);
    }
}
