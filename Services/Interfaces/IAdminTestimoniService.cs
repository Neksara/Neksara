using Neksara.Models;

namespace Neksara.Services.Interfaces;

public interface IAdminTestimoniService
{
    // Ambil semua testimoni (untuk Admin Panel)
    Task<List<Testimoni>> GetAllAsync();

    // Ambil testimoni yang visible & approved (untuk Homepage)
    Task<List<Testimoni>> GetApprovedVisibleAsync(int take = 6);

    // Approve testimoni tertentu
    Task ApproveAsync(int id);

    // Toggle visibility testimoni (hide <-> show)
    Task ToggleVisibilityAsync(int id);
}
