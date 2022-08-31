using System.Linq.Expressions;
using Volo.Abp.Domain.Repositories;

namespace Berry.Spider.Domain;

public interface ISpiderTitleContentRepository : IRepository<SpiderTitleContent, int>
{
    Task<int> MyCountAsync(Expression<Func<SpiderTitleContent, bool>> predicate, CancellationToken cancellationToken = default);
}