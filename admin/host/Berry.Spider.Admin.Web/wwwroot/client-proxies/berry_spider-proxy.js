/* This file is automatically generated by ABP framework to use MVC Controllers from javascript. */


// module berry_spider

(function(){

  // controller berry.spider.baiduSpider

  (function(){

    abp.utils.createNamespace(window, 'berry.spider.baiduSpider');

    berry.spider.baiduSpider.push = function(push, ajaxParams) {
      return abp.ajax($.extend(true, {
        url: abp.appPath + 'api/services/spider/baidu/push-from-file',
        type: 'POST',
        dataType: null,
        data: JSON.stringify(push)
      }, ajaxParams));
    };

  })();

  // controller berry.spider.common

  (function(){

    abp.utils.createNamespace(window, 'berry.spider.common');

    berry.spider.common.txtDeDuplication = function(push, ajaxParams) {
      return abp.ajax($.extend(true, {
        url: abp.appPath + 'api/services/common/de-duplication',
        type: 'POST',
        data: JSON.stringify(push)
      }, ajaxParams));
    };

  })();

  // controller berry.spider.sogouSpider

  (function(){

    abp.utils.createNamespace(window, 'berry.spider.sogouSpider');

    berry.spider.sogouSpider.push = function(push, ajaxParams) {
      return abp.ajax($.extend(true, {
        url: abp.appPath + 'api/services/spider/sogou/push-from-file',
        type: 'POST',
        dataType: null,
        data: JSON.stringify(push)
      }, ajaxParams));
    };

  })();

  // controller berry.spider.spiderLifetime

  (function(){

    abp.utils.createNamespace(window, 'berry.spider.spiderLifetime');

    berry.spider.spiderLifetime.getSpiderStatus = function(ajaxParams) {
      return abp.ajax($.extend(true, {
        url: abp.appPath + 'api/services/spider-lifetime/get-status',
        type: 'GET'
      }, ajaxParams));
    };

  })();

  // controller berry.spider.spiderPubAndRec

  (function(){

    abp.utils.createNamespace(window, 'berry.spider.spiderPubAndRec');

    berry.spider.spiderPubAndRec.clearTodoTask = function(from, ajaxParams) {
      return abp.ajax($.extend(true, {
        url: abp.appPath + 'api/services/spiderPubAndRec/clearTodoTask' + abp.utils.buildQueryString([{ name: 'from', value: from }]) + '',
        type: 'POST',
        dataType: null
      }, ajaxParams));
    };

  })();

  // controller berry.spider.touTiaoSpider

  (function(){

    abp.utils.createNamespace(window, 'berry.spider.touTiaoSpider');

    berry.spider.touTiaoSpider.push = function(push, ajaxParams) {
      return abp.ajax($.extend(true, {
        url: abp.appPath + 'api/services/spider/toutiao/push-from-file',
        type: 'POST',
        dataType: null,
        data: JSON.stringify(push)
      }, ajaxParams));
    };

  })();

  // controller berry.spider.openAI.spiderOpenAI

  (function(){

    abp.utils.createNamespace(window, 'berry.spider.openAI.spiderOpenAI');

    berry.spider.openAI.spiderOpenAI.textGeneration = function(input, ajaxParams) {
      return abp.ajax($.extend(true, {
        url: abp.appPath + 'api/services/spider/openai/text-generation',
        type: 'POST',
        data: JSON.stringify(input)
      }, ajaxParams));
    };

  })();

  // controller berry.spider.biz.servMachine

  (function(){

    abp.utils.createNamespace(window, 'berry.spider.biz.servMachine');

    berry.spider.biz.servMachine.getByMachineName = function(machineName, ajaxParams) {
      return abp.ajax($.extend(true, {
        url: abp.appPath + 'api/services/serv-machine/getByMachineName' + abp.utils.buildQueryString([{ name: 'machineName', value: machineName }]) + '',
        type: 'GET'
      }, ajaxParams));
    };

    berry.spider.biz.servMachine.getByConnectionId = function(connectionId, ajaxParams) {
      return abp.ajax($.extend(true, {
        url: abp.appPath + 'api/services/serv-machine/getByConnectionId' + abp.utils.buildQueryString([{ name: 'connectionId', value: connectionId }]) + '',
        type: 'GET'
      }, ajaxParams));
    };

    berry.spider.biz.servMachine.online = function(online, ajaxParams) {
      return abp.ajax($.extend(true, {
        url: abp.appPath + 'api/services/serv-machine/online',
        type: 'POST',
        data: JSON.stringify(online)
      }, ajaxParams));
    };

    berry.spider.biz.servMachine.offline = function(offline, ajaxParams) {
      return abp.ajax($.extend(true, {
        url: abp.appPath + 'api/services/serv-machine/offline',
        type: 'POST',
        data: JSON.stringify(offline)
      }, ajaxParams));
    };

    berry.spider.biz.servMachine.get = function(id, ajaxParams) {
      return abp.ajax($.extend(true, {
        url: abp.appPath + 'api/services/serv-machine/get' + abp.utils.buildQueryString([{ name: 'id', value: id }]) + '',
        type: 'GET'
      }, ajaxParams));
    };

    berry.spider.biz.servMachine.getList = function(input, ajaxParams) {
      return abp.ajax($.extend(true, {
        url: abp.appPath + 'api/services/serv-machine/getList' + abp.utils.buildQueryString([{ name: 'skipCount', value: input.skipCount }, { name: 'maxResultCount', value: input.maxResultCount }]) + '',
        type: 'GET'
      }, ajaxParams));
    };

    berry.spider.biz.servMachine.create = function(input, ajaxParams) {
      return abp.ajax($.extend(true, {
        url: abp.appPath + 'api/services/serv-machine/create',
        type: 'POST',
        data: JSON.stringify(input)
      }, ajaxParams));
    };

    berry.spider.biz.servMachine.update = function(id, input, ajaxParams) {
      return abp.ajax($.extend(true, {
        url: abp.appPath + 'api/services/serv-machine/update' + abp.utils.buildQueryString([{ name: 'id', value: id }]) + '',
        type: 'PUT',
        data: JSON.stringify(input)
      }, ajaxParams));
    };

    berry.spider.biz.servMachine['delete'] = function(id, ajaxParams) {
      return abp.ajax($.extend(true, {
        url: abp.appPath + 'api/services/serv-machine/delete' + abp.utils.buildQueryString([{ name: 'id', value: id }]) + '',
        type: 'DELETE',
        dataType: null
      }, ajaxParams));
    };

  })();

})();

