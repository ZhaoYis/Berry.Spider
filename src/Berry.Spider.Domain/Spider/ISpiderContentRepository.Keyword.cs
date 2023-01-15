using System.Linq.Expressions;
using Volo.Abp.Domain.Repositories;

namespace Berry.Spider.Domain;

public interface ISpiderContentKeywordRepository : IRepository<SpiderContent_Keyword, int>
{
    Task<int> MyCountAsync(Expression<Func<SpiderContent_Keyword, bool>> predicate);
}