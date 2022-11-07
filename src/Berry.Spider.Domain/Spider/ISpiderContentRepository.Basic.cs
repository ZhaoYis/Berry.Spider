using System.Linq.Expressions;
using Volo.Abp.Domain.Repositories;

namespace Berry.Spider.Domain;

public interface ISpiderContentRepository : IRepository<SpiderContent, int>
{
    Task<int> MyCountAsync(Expression<Func<SpiderContent, bool>> predicate);
}