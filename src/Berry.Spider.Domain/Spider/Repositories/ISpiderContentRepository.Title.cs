using System.Linq.Expressions;
using Volo.Abp.Domain.Repositories;

namespace Berry.Spider.Domain;

public interface ISpiderContentTitleRepository : IRepository<SpiderContent_Title, int>
{
    Task<int> MyCountAsync(Expression<Func<SpiderContent_Title, bool>> predicate, CancellationToken cancellationToken = default);
}