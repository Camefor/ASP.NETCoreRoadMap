
## MySQL Database Replication | Master-to-Slave MySQL主从架构与读写分离部署

``` 准备环境：
我使用 MySQL 5.7的版本 作为部署环境
主库在windows系统下，从库部署在debian linux系统，均为物理机器
```

### 第一步：修改MySQL数据库配置文件

###### Windows系统
 >
 > 通常配置文件路径为： `C:\ProgramData\MySQL\<具体你的mysql版本>\my.ini`
 </br>
 > Windows文件管理器快速打开配置文件：
    `%PROGRAMDATA%\MySQL\<具体你的mysql版本>\my.ini`

###### Linux系统

 > 通常配置文件路径为： `/etc/my.cnf`
 </br>


#### 配置主库 (Master)

新增/修改内容如下：

在 `[mysqld]` 部分

`server-id=1` #指定ID，主从的两台虚拟机ID必须不同

`log-bin=mysql-bin` #开启二进制日志，这是配置主从同步的关键

`bind-address = 0.0.0.0`  #确保不仅监听 127.0.0.1，允许从库连接

`binlog-do-db=<你要同步的数据库名称>` #(可选)建议指定需要同步的数据库
</br>
**特别注意事项：避免参数重复定义,以上参数在配置文件中可能已经存在，可以根据实际情况修改已有或新增**


##### 重启 MySQL 服务：

windows系统：
    在“服务”管理器中重启 MySQL

linux系统：

 `sudo systemctl restart mysql` # 重启服务

 `sudo systemctl status mysql` # 查看运行状态

docker运行模式下：`docker restart <容器名称>`

##### 在主库(Master库)中添加同步使用的用户

通过MySQL Cli命令行工具进入数据库,顺序执行以下命令:

`CREATE USER 'repl_user'@'192.168.%' IDENTIFIED BY 'Password@123';` --创建同步专用用户  主机地址约束

`GRANT REPLICATION SLAVE ON *.* TO 'repl_user'@'192.168.%';` -- 授予复制权限

`FLUSH TABLES WITH READ LOCK;` --锁定表以获取一致的坐标;主库进入“只读模式”，所有的写入操作都会被暂停。保证你配置从库时，主库的数据绝对没有变动。（生产环境建议）

`SHOW MASTER STATUS;` -- 查看主库状态 获取并记录坐标 操作：把看到的 File 和 Position 记录下来（例如：mysql-bin.000001 和 435）。

<!--输出例如：
mysql> SHOW MASTER STATUS;
+------------------+----------+--------------+------------------+-------------------+
| File             | Position | Binlog_Do_DB | Binlog_Ignore_DB | Executed_Gtid_Set |
+------------------+----------+--------------+------------------+-------------------+
| mysql-bin.000001 |      621 | <你要同步的数据库名称>     |                  |                   |
-->


`UNLOCK TABLES;` -- 释放锁,主库恢复正常读写,因为执行过 FLUSH TABLES WITH READ LOCK 否则数据库无法正常工作

<!--
1. CREATE USER 'repl_user'@'192.168.%' IDENTIFIED BY 'Password123!';
CREATE USER: 【固定】SQL 关键字。
'repl_user': 【自定义】这是用户名。虽然可以叫任何名字，但习惯上带上 repl 前缀，让人一眼看出这是用于同步的。
@: 【固定】连接符，分隔用户名和主机。
'192.168.%': 【自定义】这是最关键的参数（主机地址约束）。
%: 代表允许从任何 IP 登录。
192.168.%: 代表允许 192.168.x.x 整个网段访问。

IDENTIFIED BY: 【固定】SQL 关键字，后面跟密码。
'Password123!':【自定义】同步账号的密码。

2. GRANT REPLICATION SLAVE ON *.* TO ...
GRANT: 【固定】授权关键字。

REPLICATION SLAVE: 【固定】核心参数。这是 MySQL 专门为从库读取 binlog 设计的最小权限。

ON *.*: 【固定】表示对所有数据库的所有表生效。主从同步是基于二进制日志的，必须全局授权。

TO 'repl_user'@'192.168.%': 【自定义】必须与第一步创建的用户完全一致（名字和地址都要匹配）。

3. SHOW MASTER STATUS;
执行后你会得到一个表格，其中两个值是绝对固定不能写错的：

File: 如 mysql-bin.000003。

Position: 如 154。

这两个值在从库执行 CHANGE MASTER TO 时必须手动填入。
-->

#### 配置从库 (Slave)

[mysqld]

`server-id = 2`                # 从库 ID,必须唯一！建议主库设为 1，从库设为 2, 3

`log-bin = mysql-bin`          # 二进制日志 推荐开启。二进制日志虽然从库不直接写，但开启它有助于做“级联同步”或数据恢复。

`relay-log = mysql-relay-bin`  # 必须配置。这是从库存放从主库拉取过来的“中继日志”的地方。

`log-slave-updates = 1`        # 推荐。让从库把同步过来的操作也记录到自己的 binlog 里。

`read_only = 1`                # 只读控制，防止程序误写从库，导致主从数据不一致。

`super_read_only = 1`          # MySQL 5.7.8+ 建议开启，连 root 用户也限制写入（除了同步线程）。

`replicate-do-db = <你要同步的数据库名称>`   # 只同步指定的库

`skip_slave_start = 1`         # 防止重启 MySQL 后立即开始同步。可以手动检查网络连通性后再 START SLAVE

`记得重启 MySQL 服务`

### 第二步：建立同步联系，执行“握手” (CHANGE MASTER)

在 从库 (Slave) 执行：
进入数据库命令行：

```执行同步指令 (MASTER_LOG_FILE和MASTER_LOG_POS 请根据你之前执行 SHOW MASTER STATUS;  记录的 File 和 Position数字 修改)```

 `CHANGE MASTER TO
    MASTER_HOST='<你的master主机ip>',
    MASTER_USER='<repl_user>',
    MASTER_PASSWORD='<你的密码>',
    MASTER_LOG_FILE='mysql-bin.00000X',
    MASTER_LOG_POS=XXX;`

`START SLAVE;` # 启动同步线程

`SHOW SLAVE STATUS\G;` # 查看同步状态

执行完 SHOW SLAVE STATUS\G; 后，你要在满屏的文字中寻找这两行：

✅ Slave_IO_Running: Yes (表示成功连上主库，并正在读取 binlog)

✅ Slave_SQL_Running: Yes (表示成功读取到本地并开始执行SQL)
