using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelProject
{
    class Program
    {
        static void Main(string[] args)
        {

            BaseExcelGener ex;


            Console.WriteLine("请选择生成lua 或者 C# 代码。\n\n输入1: lua\n\n输入2: C#\n\n");

            var inputType = Console.ReadLine();

            if(inputType == "1")
            {
                ex = new LuaExcelReader();
            }
            else if(inputType == "2")
            {
                ex = new CSExcelReader();

            }
            else
            {
                Console.WriteLine("\n\n请输入有效数字，不要输入非法数字：" + inputType);
                Console.ReadKey();
                return;
            }

            Console.WriteLine("\n\n请问是 单个 excel 文件生成代码， 还是整个文件夹的 excel 文件生成代码？\n\n请注意：Excel文件需要放到和该应用程序所在目录的'Configs'文件夹下!!! \n\n输入1: 单个 \n\n输入2: 整个文件夹\n\n");

            

            var input1 = Console.ReadLine();

            if(input1 == "1")
            {
                Console.WriteLine("请输入单个文件夹的名称----------------不需要输入后缀，但要求excel格式为：xlsx");

                var singleExcelName = Console.ReadLine();
                ex.GenSingeConfigCode(singleExcelName);

            }
            else if(input1 == "2")
            {
                Console.WriteLine($"\n\n即将处理路径 ：{CSExcelReader.configPath}下的所有excel配置文件\n");
                ex.GenAllConfigCode();
            }
            else
            {
                Console.WriteLine("\n\n请输入有效数字，不要输入非法数字：" + input1);
                Console.ReadKey();
                return;
            }
            if(ex.hasError == false)
            {
                Console.WriteLine("\n\n成功生成 excel 配置代码！输入回车后退出。");
                Console.ReadKey();
            }


            //ExcelReader.GenSingeConfigCode("TestExcel");
        }
    }
}
