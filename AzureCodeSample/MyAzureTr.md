#概述

##整体结构：

- 计算服务
- 存储服务
- 网络服务
- 安全服务



##云计算app特征：

- 分布式（适合微服务架构）
- 虚拟化（虚拟机，容器）（hyper-v）
- 弹性计算（算力根据负载改变）

##Azure物理结构：

- 数据中心--机房（自营的-国际版）
- 物理服务器都是Windows server

物理机+os+hyper-v==数据中心





##tips：

- 部在云端，不可以用单点验证，不要用session

## Question

- 密钥保管库在哪？ 就是key vault

# 虚拟机和虚拟网络

### tips：

- vnet就是为了装在里面的应用拥有私有ip地址，使其进行通信，如果两个网络之间的地址重叠，则无法通信

- vnet是vm的管理容器，vm必须位于一个vnet中
- 每个vm至少有有个私有ip，由vnet分配。在一个vnet下，可以通过私有ip和名字通信
- 传统三层结构：web网站--ui层-----》web api----业务逻辑层----》sqlserver------数据访问层

---

微软vm每45天重启一次，所以如果部署一个application，底层一定是两台vm支持，因为如果只用一台vm，它一定会有宕机的时候



如果多个vm部署同一个app，需要一个负载均衡器来接收请求并转发给vm[^此步需要配置负载均衡器]



### 部署app

组件：转发请求的负载均衡器[^公共负载均衡器]+多个vm+连接vm与db的负载均衡器[^内部负载均衡器，使用私有IP地址] +多个DB[^两个：主备模式  多个：多活模式]

但是会面对一个安全问题：一致开放端口会导致不安全，所以需要划分子网[^NSG-网络安全组]（子网1：对外开放80         子网2：对web服务开放1433）





## vnet操作

要有一个IP地址池，至少有1个子网，每个网络要有自己的地址段



## 创建vm

用power shell，Azure CLI，Azure SDK

- 方法1 用market 提供的镜像直接创建

- 方法2 使用客户自定义的镜像
  - 本地使用hyper-v创建hyper-v虚拟机
  - 把包含os的虚拟磁盘的vhd上传到Azure storage



##连接和管理vm

- vm具备公共ip，开放管理端口，rdp3389   ssh22
- vm不具公共ip，开放端口管理，前端有nat服务（负载均衡）
- 在vm网络内，加入堡垒机（bastion）服务，管理员用bastion 连接本网络内的所有vm
- 建立混合连接   企业+Azure



# Azure App service

Azure 托管了web类型的web app运行环境

类型：网页/webapi

app service plan:一组指定区域和os类型，sku的vm

deploy slot--类似于环境的概念

具有自己的实时app

​	

金丝雀部署

灰度部署

身份验证：

- 启用Microsoft身份验证---aad（企业内部验证）
  - configuration--》identify provider--》
- oauth2和openID验证（企业外部验证）



# 容器中部署app



创建ACR

- 准备code和dockerfile 



# Azure blob 存储服务

**数据中心**：数据仓库+数据湖

- 数据仓库只能存储结构化数据
- 不同格式的数据 存储到一起

Azure storage ：存储四类信息的存储服务（有四类子服务）





storage account 三层结构：容器（文件夹，用于放blob的单位）---》

---

**支持的数据信息**：

- blob-------二进制，大对象，非结构化，媒体文件的流式存储
- file---------共享文件存储，与blob不同是协议不同，举个要修改文件的例子（用blob：用code在线修改文件。用file：下载--》修改--》上传）
- 表存储-----NoSQL结构的数据存储，但可以使用CosmosDB代替
- 消息队列存储-----多个应用异步通信

不用担心数据的安全性，保存在微软的磁盘是加密的。[传输加密，存储加密，读写加密]

一个数据上传到微软磁盘是分成三份副本，分别存储，开冗余功能会存6份副本

---

**SKU**：

- 标准：存储四种数据，使用标准ssd存储设备
- 高级：使用高级ssd存储设备，磁盘转的块，所以读写速度更快，分为三种服务
  - 块blob（传输块，读写慢），  单个块最小64k，最大为100M，单个块blob最大190T；追加块blob
  - 页blob（读写块，传输慢），单个页blob最大8T
  - file

---

**复制性**：（为提升存储数据可靠性，storage account有三种复制特性）

- 本地冗余 LRS：一个数据在同一个数据中心三个副本 
- 本区域冗余 ZRS：有可用性区域的数据中心才行，在同一个数据中心的三个区域中有三个副本
- 异地冗余GRS：数据在主数据中心三个副本，灾备中心三个副本

LRS可改成GRS，GRS不能改回LRS

数据中心的类型：

- 早期是配对区域，两个数据中心互为灾备
- 后期是可用性区域数据中心，每个数据中心有三个可用性区域，每个区域有独立电源和通信设施

---

通过Azure resource manager  web api 实现操作storage，微软对webapi封装以后，提供了对应的sdk。所以我有两种方式对资源的操作，直接用web api、用微软的sdk。

storage account都要base url ，操作和访问都是通过baseurl，进行webapi的调用

---

blob生命周期

访问层： 

- 热  读写速度最快，加个最贵 （高频则放热层比较好）
- 冷 性能低 成本低， 有30天保留期
- 存档 脱机，不能直接读写，保留90天  ，将数据加载到该层（冻结数据）移出叫解冻数据

---

使用rest api设置和检索blob资源的属性和元数据

# Azure File Service

通过SMB，NFS协议实现共享的upload 和download

功能是取代或者扩展本地文件服务器

可以建很多的共享，一个共享最多5T



---

# cosmosdb

目的：为了分析、海量数据的存储

分区：逻辑分区和物理分区

分区和时间戳

ts 值是分区键



# Azure SQL DataBase

IaaS :Azure Sql vm

PaaS：Azure SQL，Azure MySQL PostgreSQL，  Oracle sql

完整的sql server包含四个服务：

- 数据引擎
- 集成服务
- 分析服务
- 报表服务

Azure 上面的sql服务有两类：

<u>Azure SQL Database</u>[^Azure托管的数据库服务]和 <u>托管的SQL实例</u>[^具有一个sqk服务器的所以功能，即数据引擎和集成服务]

Azure SQL Database 组件：

1. 逻辑服务器：多个database对外的访问入口点
2. 防火墙
3. 两种模式：
   - 单一库（单个库）
   - 弹性池（多个库）
4. 计费模式：
   - 基于cpu计费：标准层、业务关键层、大规模层
   - 基于DTU计费：100DTU  标准层1CPU（相对便宜）



# 微软标识平台

身份验证服务组成：

用户信息存储；登陆页面；验证逻辑；颁发身份凭证

身份验证服务种类：

- 互联网app：自定义

- 企业内部：集成身份验证(各种不同的服务与域控制器集成)，域服务  

- OAuth2和openID:第三方身份验证服务

Azure AD具有OAuth2特性，因此Azure AD可以：

- 为访问Azure资源提供身份验证
- 为任意App提供身份验证

Azure AD 可以为三类主体提供身份验证：

- 用户：内部user，来宾user，社交账户user
- App(服务主体) ：利用在Azure AD 注册过的application ID 去访问资源
- 托管标识(给资源授权)，把特定的Azure资源经过注册后得到一个主体资格，可以把其他Azure资源的权限授予托管标识

身份验证库(ADAL,MSAL)

Azure AD验证用户功能:

- 普通验证，用户名+密码
- MFA：多重身份验证
- MS Entra：用户账户+绑定硬件
- 条件访问

---

app集成Azure ad 身份验证



向Azure 资源授权



---

# Azure  密钥保管库

Azure托管的机密信息管理服务。存储 密钥；证书；文本等

使用者通过endpoint 向库发送请求，返回机密，用机密方法解密得到password



托管标识：资源在aad注册后，成为一个主体，





# Azure 事件网格&event Hub 事件中心

##event grid事件网格

Azure 资源特定事件发生后，自动触发对应该事件的处理机制

并非所有资源都支持事件网格，只有具备事件属性的资源才支持

事件处理机制：本身拥有代码能力或传递能力，比如自动化账户，function app，logic app等

## event hub 事件中心

海量数据并发传递服务-----------实时数据传输服务

事件中心结合流分析作业服务----------实时数据分析




# 实验

## vnet+vm 部署app

1. 规划网络结构
   - 区域
   - 网络名
   - ip地址范围：192.168 172.16   10          192.168.0.0----192.168.7.255
     - 子网1：wbesubnet--192.168.0.0-192.168.0.255
     - 子网2：dbsubnet----192.168.1.0-192.168.1.255
2. 2
3. 3
4. 4
5. 5
6. 建立从本地到Azure vm的安全连接
   - 在vnet上创建一个网关子网
   - 在网关子网加入vnet网关[^一个网关占用3个ip地址]
   - 建好网管后配置网关：（Address pool，Tunnel type，Authentication type）
   - 本地power shell 生成自签名证书
   - 导出证书，把证书内容配置到站点上面



## Azure Application Service部署web类型的app

slot+app service+repos

application service 环境：独立环境要提供虚拟网络。





openai service 创建

- 地区不同，拥有的gpt版本不同（美国东部可以用大模型davinci-002）
- enter point 和key
- 调用openai服务的方法 ：
  - 自己写code
  - app service
- openai studio--》部署模型--》部署到web 程序--》
