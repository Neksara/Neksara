using Neksara.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Neksara.Services.Interfaces
{
    public interface IAdminTestimoniService
    {
        // ===== GET ALL TESTIMONI (ADMIN) =====
        Task<List<Testimoni>> GetAllAsync(int? rating = null, string status = null);

        // ===== BULK ACTION =====
        Task ApproveManyAsync(int[] ids);
        Task HideManyAsync(int[] ids);
        Task ShowManyAsync(int[] ids);

        // ===== PUBLIC / HOMEPAGE =====
        Task<List<Testimoni>> GetApprovedVisibleAsync(int take = 6);
    }
}
