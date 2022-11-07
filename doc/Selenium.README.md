安装Java：
yum install -y java-1.8.0-openjdk.x86_64
//验证完成安装
java -version

安装谷歌浏览器：
sudo yum -y install  redhat-lsb
sudo yum -y install libvulkan*
sudo yum -y install libXss*
sudo yum -y install libappindicator3*
sudo yum -y install liberation-fonts
sudo rpm -ivh google-chrome-stable_current_x86_64.rpm

设置webdriver的PATH地址：
sudo vi /etc/profile
添加以下代码：
PATH=$PATH:/usr/local/software/webdriver
export PATH

启动selenium：
cd /usr/local/software/selenium
java -jar selenium-server-4.1.2.jar standalone --port 4445 --driver-configuration display-name="Chrome" webdriver-path="/usr/local/software/webdriver" max-sessions=4 stereotype='{"browserName":"chrome","gsg:customcap":true}'

以服务的方式运行：
cd /usr/local/software/selenium
nohup java -jar /usr/local/software/selenium/selenium-server-4.1.2.jar standalone --port 4445 --driver-configuration display-name="Chrome" webdriver-path="/usr/local/software/webdriver" max-sessions 4 stereotype='{"browserName":"chrome","gsg:customcap":true}' >/dev/null 2>&1 &
nohup java -jar selenium-server-4.1.2.jar standalone --port 4445 --max-sessions 4 >/dev/null 2>&1 &

查处进程：
lsof -i:4445
结束进程：
kill -9 进程号

hub：124.223.62.114:4445
node-1:121.199.12.249:4445

查找大于100M的文件：
ls -lh  $(find / -type f -size +100M)


https://www.selenium.dev/zh-cn/documentation/grid/configuration/cli_options/

webdriver下载地址
https://chromedriver.storage.googleapis.com/index.html

测试工具：
https://bot.sannysoft.com/

擦出webdriver指纹：
https://zhuanlan.zhihu.com/p/328768200

清除mysql缓存
mysqladmin -u root -p flush-host