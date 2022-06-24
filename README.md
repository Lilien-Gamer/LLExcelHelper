# LLExcelHelper


## 简介:
一个用于 解析 excel 配置文件 生成 数据代码 的工具，一键傻瓜式自动生成，配置简单，支持多语言。

第一次知道这个 配置文件生成数据代码的思路 是 19 年实习的时候，然后19年11月自己做游戏的时候写了一个嵌入Unity内的工具，供自己用。
后来20年毕业后到公司minigame，把这个工具拿了出来，基于我们这群校招萌新们的多人使用，做了一些优化和修改。
工作两年后的22年，又基于在项目组的实践经验，以及个人的技术成长，把这个工具彻底的从Unity中脱离，变成了一个exe形式的小工具，并且拓展出了lua版本，也优化了生成后代码的性能等等。

## 详细说明:

###### 支持excel格式:xlsx
###### 支持语言类型:C#, lua

###### C#支持数据类型: int, float, string, string[], int[], int[][], int[,], Dictionary<string,int>, Dictionary<int,int>, Dictionary<int,int[]>

###### lua支持: 支持以上C#类型转换成对应的 lua 类型，包括string, number, 和奇奇怪怪的table.

###### 数据类型示例见:[《格式参考文档》](https://github.com/Lilien-Gamer/LLExcelHelper/blob/main/%E6%A0%BC%E5%BC%8F%E5%8F%82%E8%80%83%E6%96%87%E6%A1%A3.xlsx)

###### 支持机制：
1. 开头标记"#"或者 空白 表示这一行跳过解析。
2. 第一行的任意标记"#",表示这一列跳过解析.
3. 支持复杂数据类型中写入 空格 .

## 如何使用

1. 下载exe:[工具的exe](https://github.com/Lilien-Gamer/LLExcelHelper/blob/main/ExcelHelper.exe)
2. 所有的配置文件放到一个叫"Configs"的文件夹下
3. 把exe放到Configs同目录
4. 编辑excel时一定要注意格式要求（很简单的 看下文档就知道了）
5. 运行exe，根据提示输入
6. 已经在同目录生成代码

##运行时怎么获取到配置数据

C#:  这样的代码: 命名空间.{配置文件名}ConfigManager.Instance().allDatas; 

`var allConfigData = LLExcelConfig.luaCSTestConfigManager.Instance().allDatas;`

lua: `local config = require(文件名)`
