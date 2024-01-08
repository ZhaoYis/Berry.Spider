// This file is automatically generated by ABP framework to use MVC Controllers from CSharp
using Berry.Spider.TouTiao;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Http.Client;
using Volo.Abp.Http.Client.ClientProxying;
using Volo.Abp.Http.Modeling;

// ReSharper disable once CheckNamespace
namespace Berry.Spider;

[Dependency(ReplaceServices = true)]
[ExposeServices(typeof(ITouTiaoSpiderAppService), typeof(TouTiaoSpiderClientProxy))]
public partial class TouTiaoSpiderClientProxy : ClientProxyBase<ITouTiaoSpiderAppService>, ITouTiaoSpiderAppService
{
    
}
