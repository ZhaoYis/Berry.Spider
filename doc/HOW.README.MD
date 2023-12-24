1、下载最新版谷歌浏览器（如果电脑上已经安装，可根据实际情况进行更新），正版谷歌浏览器下载地址：https://www.google.com/intl/zh-CN/chrome/

2、下载和浏览器主版本保持一致的webdriver驱动程序（切记，主版本一定要一致），下载地址：https://chromedriver.storage.googleapis.com/index.html

![Xnip2022-07-05_09-24-04.jpg](https://s2.loli.net/2022/07/05/EWauoRrk9fCSU2T.jpg)

3、解压后将chromedriver.exe文件拷贝到`C:/berry_spider/webdriver`目录下，如果目录不存在，则创建目录即可。双击chromedriver.exe文件，出现下面消息表示安装成功：

![Xnip2022-07-05_09-34-03.jpg](https://s2.loli.net/2022/07/05/hr6EHKfuWSzAoZ4.jpg)

4、从以下位置下载最新程序包，并将程序包放在任意位置（不过建议还是放在D盘下，建议位置：D:/BerrySpider_Full文件夹下）

链接: https://pan.baidu.com/s/1nohlmHxaY0FU8S3FrgJzQA

提取码: afjm

![Xnip2022-07-05_09-43-08.jpg](https://s2.loli.net/2022/07/05/gUBKNPxpcIMTkFz.jpg)

5、启动相关程序即可（一台机器上理论上可以运行N个程序，在电脑资源充足的情况下）

6、配置系统（建议不要直接修改，有不清楚的可以先问下我）

​ 地址：http://124.223.62.114:15000/ui#/user/login

​ 账号：admin

​ 密码：1qaz2wsx.com

![Xnip2022-07-05_09-48-41.jpg](https://s2.loli.net/2022/07/05/1t2JaqlX7uMT6bg.jpg)

注意事项：本地应用程序配置文件切勿修改，所有默认配置格式如下

```json
{
  "AgileConfig": {
    "appId": "berry_spider",
    "secret": "1q2w3E*.com",
    "nodes": "http://124.223.62.114:15000",
    "name": "具体应用名称",
    "env": "PROD"
  }
}
```

7、消息队列管理面板

​ 地址：http://47.114.177.24:15672/#/queues

​ 账号：spider

​ 密码：1q2w3E*.com