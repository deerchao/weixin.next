# weixin.next

.Net 下的微信公众号接口库, 用于调用微信公众平台提供的各种数据接口, 处理微信服务器发来的各种消息.

仅支持 .Net 4.5+ (.Net Core 应该能很方便地移植). 已完成菜单, 用户, 消息, 媒体素材, 网页验证等接口.

## 目标
### 简单易用

- 接口名称和消息类型等遵循 .Net 的命名规范. 
- 异步方法名称后不加 Async 后缀(因为大部分方法都是异步的).
- 提供 access_token, jsapi_ticket 等的自动过期管理机制 
- 提供 Asp.net Mvc 示例项目.

### 简洁高效

- 所有网络 I/O 使用异步机制
- access_token, jsapi_ticket 等能方便地支持多台服务器间的协同工作.
- 可自定义 JSON 库(自带 Json.Net 实现)
- 可自定义消息去重缓存策略(自带内存缓存实现)

### 持续更新

- 附带官方文档下载功能, 可随时更新并 diff

## 使用说明

具体可参见 `Sample` 项目.

### 接口调用

首先使用 `JsonParser`, `AccessTokenManager`, `HttpClient` 构造好 `ApiConfig` 对象, 将其设置为 `ApiHelper` 默认的 `ApiConfig`, 然后就可以直接调用不同的接口方法了. 在调用具体接口时, 如因需要支持多个公众号等原因, 也可以提供一个不同于默认的 `ApiConfig` 对象.

所有接口调用都返回 `Task` 或 `Task<T>` 结果, 应用层应对其进行 `await`. 接口调用有可能抛出 `ApiException`, 对应微信返回的 `{"errcode": xxx, "errmsg": "yyy"}` 的情况. 

### 消息处理

首先从 `MessageCenter`/`MessageHandler` 派生自己的类, 按自己的需求重写部分方法, 然后实例化一个 `MessageCenter` 对象, 调用其 `ProcessMessage` 方法即可.

`MessageCenter` 负责加密/解密, 序列化/反序列化, 消息去重等功能；`MessageHandler` 负责处理单条消息.

## 致谢

本项目部分设计受 JefferySu 的 [WeiXinMPSDK](https://github.com/JeffreySu/WeiXinMPSDK) 影响. 
