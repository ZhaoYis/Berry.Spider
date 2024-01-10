using Volo.Abp.Domain.Services;

namespace Berry.Spider.Domain;

public class SpiderAppDomainService : DomainService
{
    private readonly ISpiderAppInfoRepository _spiderAppInfoRepository;

    public SpiderAppDomainService(ISpiderAppInfoRepository spiderAppInfoRepository)
    {
        _spiderAppInfoRepository = spiderAppInfoRepository;
    }

    /// <summary>
    /// 创建一个版本的应用
    /// </summary>
    public async Task CreateAppAsync(long id,
        string name,
        string tagName,
        string targetCommitish,
        DateTime createdAt,
        DateTime publishedAt)
    {
        //包不能重复
        SpiderAppInfo? appInfo = await _spiderAppInfoRepository.FindAsync(app => app.Name == name);
        if (appInfo is null)
        {
            string version = tagName.Split('-').Last();
            appInfo = new SpiderAppInfo
            {
                BizNo = id.ToString(),
                Name = name,
                TagName = tagName,
                TargetCommitish = targetCommitish,
                CreatedAt = createdAt,
                PublishedAt = publishedAt,
                //七牛云对象存储key
                //TODO：后续通过七牛云回调来处理，目前qshell暂时不支持
                OssKey = $"consumer-client/Berry.Spider.Consumers-{version}.zip"
            };

            await _spiderAppInfoRepository.InsertAsync(appInfo);
        }
        else
        {
            //不做操作
        }
    }
}