using Microsoft.EntityFrameworkCore;
using Neksara.Data;
using Neksara.Models;
using Neksara.Services.Interfaces;

namespace Neksara.Services
{
    public class TestimoniService : ITestimoniService
    {
        private readonly ApplicationDbContext _context;

        public TestimoniService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Testimoni>> GetPublishedAsync(int? rating)
        {
            var query = _context.Testimonis
                .Where(t => t.IsApproved && t.IsVisible);

            if (rating.HasValue)
            {
                query = query.Where(t => t.TestimoniRating == rating.Value);
            }

            return await query
                .OrderByDescending(t => t.TestimoniRating)
                .ThenByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task CreateAsync(Testimoni model)
        {
            model.TestimoniRating = Math.Clamp(model.TestimoniRating, 1, 5);
            model.IsApproved = false; // langsung approved untuk dummy
            model.IsVisible = true;
            model.CreatedAt = DateTime.Now;

            _context.Testimonis.Add(model);
            await _context.SaveChangesAsync();
        }
    }
}
