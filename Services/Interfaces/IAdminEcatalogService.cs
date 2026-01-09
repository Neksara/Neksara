using Neksara.Models;
using Neksara.ViewModels;

namespace Neksara.Services.Interfaces;

public interface IAdminEcatalogService
{
    Task<(List<Topic> data, int total)> GetPagedAsync(
        string? search,
        string? sort,
        int page,
        int pageSize);

    Task PublishAsync(int[] topicIds);
    Task WithdrawAsync(int[] topicIds);
    Task ArchiveAsync(int[] topicIds);

    // 🔥 UBAH RETURN TYPE
    Task<TopicDetailVM?> GetDetailAsync(int id);
}
