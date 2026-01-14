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

    public async Task<List<Testimoni>> GetAllAsync(int? rating = null, string? status = null)
    {
        return await _context.Testimonis
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task ApproveManyAsync(int[] ids)
    {
        var items = await _context.Testimonis
            .Where(t => ids.Contains(t.TestimoniId))
            .ToListAsync();

        foreach (var t in items)
        {
            t.IsApproved = true;
            t.IsVisible = true;
        }

        await _context.SaveChangesAsync();
    }

    public async Task HideManyAsync(int[] ids)
    {
        var items = await _context.Testimonis
            .Where(t => ids.Contains(t.TestimoniId))
            .ToListAsync();

        foreach (var t in items)
            t.IsVisible = false;

        await _context.SaveChangesAsync();
    }

    public async Task ShowManyAsync(int[] ids)
    {
        var items = await _context.Testimonis
            .Where(t => ids.Contains(t.TestimoniId))
            .ToListAsync();

        foreach (var t in items)
            t.IsVisible = true;

        await _context.SaveChangesAsync();
    }

    public async Task<List<Testimoni>> GetApprovedVisibleAsync(int take = 6)
    {
        return await _context.Testimonis
            .Where(t => t.IsApproved && t.IsVisible)
            .OrderByDescending(t => t.CreatedAt)
            .Take(take)
            .ToListAsync();
    }
}
