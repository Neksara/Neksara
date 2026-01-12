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

    // LIST SEMUA TESTIMONI
    public async Task<IActionResult> Index()
    {
        var data = await _service.GetAllAsync();
        return View(data);
    }

    // APPROVE TESTIMONI
    [HttpPost]
    public async Task<IActionResult> Approve(int id)
    {
        await _service.ApproveAsync(id);
        return RedirectToAction(nameof(Index));
    }

    // TOGGLE VISIBILITY (HIDE / SHOW)
    [HttpPost]
    public async Task<IActionResult> ToggleVisibility(int id)
    {
        await _service.ToggleVisibilityAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
