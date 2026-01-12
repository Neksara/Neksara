using Microsoft.AspNetCore.Mvc;
using Neksara.Models;
using Neksara.Services.Interfaces;

namespace Neksara.Controllers
{
    public class TestimoniController : Controller
    {
        private readonly ITestimoniService _service;

        public TestimoniController(ITestimoniService service)
        {
            _service = service;
        }

        // Tampil semua testimoni (opsional filter rating)
        public async Task<IActionResult> Index(int? rating)
        {
            var data = await _service.GetPublishedAsync(rating);
            ViewBag.SelectedRating = rating;
            return View(data);
        }

        // Submit testimoni tanpa batasan session
        [HttpPost]
        public async Task<IActionResult> Create(Testimoni model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Data testimoni tidak valid 😢";
                return RedirectToAction("Index", "Home");
            }

            // Simpan ke DB
            await _service.CreateAsync(model);

            TempData["Success"] = "Terima kasih! Testimoni kamu berhasil dikirim 💙";

            return RedirectToAction("Index", "Home");
        }
    }
}
