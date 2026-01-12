using Neksara.Models;

namespace Neksara.Services.Interfaces
{
    public interface ITestimoniService
    {
        Task<List<Testimoni>> GetPublishedAsync(int? rating);
        Task CreateAsync(Testimoni model);
    }
}
