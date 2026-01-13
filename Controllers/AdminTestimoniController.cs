using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Neksara.Services.Interfaces;

namespace Neksara.Controllers;

[Authorize(Roles = "Admin")]
public class AdminTestimoniController : Controller
{
    private readonly IAdminTestimoniService _service;

    public AdminTestimoniController(IAdminTestimoniService service)
    {
        _service = service;
    }

    public async Task<IActionResult> Index()
    {
        var data = await _service.GetAllAsync();
        return View(data);
    }

    // ===== BULK APPROVE =====
    [HttpPost]
    public async Task<IActionResult> ApproveSelected(int[] selectedIds)
    {
        if (selectedIds?.Length > 0)
            await _service.ApproveManyAsync(selectedIds);

        return RedirectToAction(nameof(Index));
    }

    // ===== BULK HIDE =====
    [HttpPost]
    public async Task<IActionResult> HideSelected(int[] selectedIds)
    {
        if (selectedIds?.Length > 0)
            await _service.HideManyAsync(selectedIds);

        return RedirectToAction(nameof(Index));
    }

    // ===== BULK SHOW =====
    [HttpPost]
    public async Task<IActionResult> ShowSelected(int[] selectedIds)
    {
        if (selectedIds?.Length > 0)
            await _service.ShowManyAsync(selectedIds);

        return RedirectToAction(nameof(Index));
    }
}
