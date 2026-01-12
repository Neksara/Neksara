using Microsoft.EntityFrameworkCore;
using Neksara.Data;
using Neksara.Models;
using Neksara.Services.Interfaces;

namespace Neksara.Services;

public class AdminTestimoniService : IAdminTestimoniService
{
    private readonly ApplicationDbContext _context;

    public AdminTestimoniService(ApplicationDbContext context)
    {
        _context = context;
    }

    // AMBIL SEMUA TESTIMONI
    public async Task<List<Testimoni>> GetAllAsync()
    {
        return await _context.Testimonis
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    // APPROVE
    public async Task ApproveAsync(int id)
    {
        var testimoni = await _context.Testimonis.FindAsync(id);
        if (testimoni == null) return;

        testimoni.IsApproved = true;
        await _context.SaveChangesAsync();
    }

    // TOGGLE VISIBILITY
    public async Task ToggleVisibilityAsync(int id)
    {
        var testimoni = await _context.Testimonis.FindAsync(id);
        if (testimoni == null) return;

        testimoni.IsVisible = !testimoni.IsVisible;
        await _context.SaveChangesAsync();
    }

    // AMBIL TESTIMONI YANG SUDAH APPROVE DAN VISIBLE (HOMEPAGE)
    public async Task<List<Testimoni>> GetApprovedVisibleAsync(int take = 6)
    {
        return await _context.Testimonis
            .Where(t => t.IsApproved && t.IsVisible)
            .OrderByDescending(t => t.CreatedAt)
            .Take(take)
            .ToListAsync();
    }
}
