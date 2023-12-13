// This file is automatically generated by ABP framework to use MVC Controllers from CSharp
using Berry.Spider.OpenAI.Contracts;
using System;
using System.Collections.Generic;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.ObjectExtending;

// ReSharper disable once CheckNamespace
namespace Berry.Spider.OpenAI.Contracts;

public class TextGenerationInput
{
    public string Keyword { get; set; }

    public int? MaxLength { get; set; }
}
