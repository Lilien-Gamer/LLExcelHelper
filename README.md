# LLExcelHelper


## 简介:
一个用于 解析 excel 配置文件 生成 数据代码 的工具，一键傻瓜式自动生成，配置简单，支持多语言。

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
4. 运行exe，根据提示输入
