// This file is automatically generated by ABP framework to use MVC Controllers from CSharp
using Berry.Spider;
using Berry.Spider.Core;
using System;
using System.Collections.Generic;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.ObjectExtending;

// ReSharper disable once CheckNamespace
namespace Berry.Spider;

public class SpiderUpdateInput : EntityDto<Int32>
{
    public string Name { get; set; }

    public SpiderSourceFrom From { get; set; }

    public string Remark { get; set; }
}
