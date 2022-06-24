using System.Collections;
using System.Collections.Generic;
using System.Data;
using Excel;
using System.IO;
using System;
using Object = System.Object;
using System.Text;

namespace ExcelProject
{

    public abstract class BaseExcelGener: IHasConvertHelper
    {
        protected BaseConvertHelper convertHelper;

        protected const string BaseConfigClassName = "BaseConfig";

        public bool hasError = false;

        //protected virtual string BaseConfigClassCode { get { return null; } }


        protected List<string> configNames;

        protected Dictionary<int, string> memberType = new Dictionary<int, string>();
        protected Dictionary<int, string> names = new Dictionary<int, string>();

        public const string configPath = "./Configs/";

        protected const string dataClassPath = "./Scripts/ConfigAsset/ConfigBaseClass/";
        protected const string dataManager = "./Scripts/ConfigAsset/ConfigManager/";

        protected const string baseConfigPath = "./Scripts/BaseConfig.cs";
        protected const string TypeRowName = "#Type#";
        protected const string NameRowName = "#Name#";



        protected List<int> ignoreColIndex = new List<int>(); 

        protected void InitIgnoreColList(DataRow firstRow, int length)
        {
            for(int i = 0; i< length;i ++)
            {
                if(  (firstRow[i].ToString() ).StartsWith("#")  ) 
                {
                    ignoreColIndex.Add(i);
                }
            }
        }
        protected virtual  void TryCreateFolder()
        {
            bool exist = Directory.Exists(configPath);

            if (exist == false)
            {
                Directory.CreateDirectory(configPath);
            }

            bool exist2 = Directory.Exists(dataClassPath);

            if (exist2 == false)
            {
                Directory.CreateDirectory(dataClassPath);
            }

            bool exist3 = Directory.Exists(dataManager);

            if (exist3 == false)
            {
                Directory.CreateDirectory(dataManager);
            }

  

        }


        //尝试跳过当前列
        protected bool TryContinueNextCol(int currentCol)
        {
            if (ignoreColIndex.Contains(currentCol))
                return true;
            else
                return false;
        }
        //尝试跳过当前行
        protected bool TryContinueNextRow(string content)
        {
            // # 开头表示该行跳过
            if (content.StartsWith("#") || string.IsNullOrEmpty(content))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected  void LoggerError(string content)
        {
            hasError = true;
            Console.WriteLine(content);
            Console.ReadKey();
        }

        protected bool FindTypeAndNameRow(DataRowCollection rows, out int typeRow, out int nameRow)
        {
            typeRow = -1;
            nameRow = -1;

            bool res = true;

            int count = rows.Count;
            for (int i = 0; i < count; i++)
            {
                if (typeRow != -1 && nameRow != -1)
                {
                    break;
                }

                var row = rows[i];
                var content = row[0].ToString();


                if (typeRow == -1)
                    if (content.StartsWith(TypeRowName))
                    {
                        typeRow = i;
                        row[0] = content.Replace(TypeRowName, "");
                    }

                if (nameRow == -1)
                {
                    if (content.StartsWith(NameRowName))
                    {
                        nameRow = i;
                        row[0] = content.Replace(NameRowName, "");
                    }
                }

            }


            if (typeRow == -1)
            {
                LoggerError($" 请 excel 中 配置某一行的开头第一列的内容为:'{TypeRowName}' 开头 ，来表示 数据的类型");
                res = false;
            }
            if (nameRow == -1)
            {
                LoggerError($" 请 excel 中 配置某一行的开头第一列的内容为:'{NameRowName}' 开头 ，来表示 数据字段的名称");
                res = false;
            }

            return res;
        }

        public  void GenAllConfigCode()
        {
            DirectoryInfo folder = new DirectoryInfo(configPath);

            List<string> fileNames = new List<string>();

            foreach (FileInfo file in folder.GetFiles())
            {
                fileNames.Add(file.Name);
            }

            foreach (var v in fileNames)
            {
                GenSingeConfigCode(v.Replace(".xlsx", ""));
            }
        }

        public void GenSingeConfigCode(string fileName)
        {
            try
            {
                TryCreateFolder();

                

                memberType.Clear();
                names.Clear();
                string path = configPath + fileName + ".xlsx";
                FileStream fileStream = File.Open(path, FileMode.Open, FileAccess.Read);
                IExcelDataReader excelDataReader = ExcelReaderFactory.CreateOpenXmlReader(fileStream);

                // 表格数据全部读取到result里
                DataSet result = excelDataReader.AsDataSet();

                if (result.Tables.Count == 0)
                {
                    //表里没有数据
                    LoggerError("配置表里没有数据 是个空表 :" + fileName);



                   
                }
                else
                {
                    var table = result.Tables[0];
                    // 获取表格有多少列
                    int columns = table.Columns.Count;
                    // 获取表格有多少行
                    int rows = table.Rows.Count;


                    //取得表格中的数据 
                    //取得table中所有的行
                    var rowCollections = table.Rows;



                    int columnLength = table.Columns.Count; //列数
                    int rowLength = rowCollections.Count;


                    bool res = FindTypeAndNameRow(rowCollections, out var typeRow, out int nameRow);

                    if (res)
                    {
                        InitIgnoreColList(rowCollections[0], columnLength);

                        var typeRowData = rowCollections[typeRow];//返回了第0行的集合

                        var nameRowData = rowCollections[nameRow];


                        for (int i = 0; i < columnLength; i++)
                        {
                            if (TryContinueNextCol(i)) 
                                continue;
                            else
                            {

                                var s = typeRowData[i].ToString();
                                if (string.IsNullOrEmpty(s))
                                {
                                    LoggerError($"这儿有列 :{i} 没有定义 数据类型 ");
                                }

                                memberType.Add(i, s);
                            }
                               


                        }

                        for (int i = 0; i < columnLength; i++)
                        {
                            if (TryContinueNextCol(i))
                                continue;
                            else
                            {
                                var s = nameRowData[i].ToString();
                                if(string.IsNullOrEmpty(s))
                                {
                                    LoggerError($"这儿有列 :{i} 没有定义 字段名 ");
                                }

                                names.Add(i, s);
                            }
                                
                        }


                        StartGen(fileName, rowCollections, typeRow, nameRow, columnLength);
                    }
                }


                excelDataReader.Close();
                fileStream.Close();


            }
            catch (Exception e)
            {
                LoggerError($"有异常: {e.Message} \n {e.StackTrace}" );
               
            }
        }

        protected abstract void StartGen(string fileName, DataRowCollection rowCollections, int typeRow, int nameRow, int columnLength);
        public abstract BaseConvertHelper GetConvertHelper();
    }

    public class CSExcelReader: BaseExcelGener
    {


        string BaseConfigClassCode 
        {
            get 
            { return @"

namespace DataClass
{
    using System;
    using System.Collections.Generic;
         
    public abstract class BaseConfig<T1,T2> where T1:new()
    {
        protected static T1 instance = new T1();
        protected string name;
        public Dictionary<int, T2> allDatas;
        public static T1 Instance()
        {
            return instance;
        }
        public ConfigDataManager()
        {
            Init();
            
        }
        public virtual void Init()
        {
           
        }
    }

}


"; 
            } 
        }




        protected override void TryCreateFolder()
        {
            base.TryCreateFolder();
            var s = File.CreateText(baseConfigPath);
            s.Write(BaseConfigClassCode);

            s.Close();
        }

        protected override  void StartGen(string fileName, DataRowCollection rowCollections, int typeRow, int nameRow, int columnLength)
        {
           
            {
                GenDataClass(fileName);

                int rowLength = rowCollections.Count;

                var typeRowData = rowCollections[typeRow];

                StringBuilder dataCode = new StringBuilder();
                StringBuilder dictionaryCode = new StringBuilder();
                int dataCount = 0;
                for (int i = 0; i < rowLength; i++)
                {
                    if (i == typeRow || i == nameRow)
                        continue;

                    bool needContinue = TryContinueNextRow(rowCollections[i][0].ToString());



                    if (needContinue == false)
                    {

                        dataCount++;

                        dataCode.Append($"\n\n            {fileName} config{dataCount} = new {fileName}();");
                        for (int j = 0; j < columnLength; j++)
                        {
                            bool needContinueCol = TryContinueNextCol(j);
                            if (needContinueCol)
                                continue;
                            else
                            {
                                var typeRowDataItem = typeRowData[j].ToString();

                                var rowDataItem = rowCollections[i][j]?.ToString();

                                dataCode.Append($"\n            config{dataCount}.{names[j]} = {GetConvertHelper().ConvertString(typeRowDataItem, rowDataItem)};");
                            }

                        }
                        //dataCode += $"\n    allDatas.Add( config.{names[0]}, config)\n;";
                        dictionaryCode.Append($"config{dataCount},");
                    }


                }

                string code = $@"
using System.Collections.Generic;                   
using System;
namespace DataClass
{{
    public class {fileName}Manager :{BaseConfigClassName}<{fileName}Manager,{fileName}>
    {{
        public override void Init( )
        {{
            
            name = ""{fileName}"";
            
            {dataCode}

            allDatas = new Dictionary<int, {fileName}>({dataCount}){{
            
                {dictionaryCode}

            }};

            base.Init();
        }}

    }}


}}




";

                FileStream fs = new FileStream(dataManager + fileName + "Manager.cs", FileMode.Create);//创建文件
                StreamWriter w = new StreamWriter(fs);

                w.Write(code);//追加数据
                w.Close();//释放资源,关闭文件  
                fs.Close();
            }

          
        }


        //public static void GenDataAsset(string fileName)
        //{
        //    memberType.Clear();
        //    names.Clear();
        //    string path =  configPath + fileName + ".xlsx";
        //    FileStream fileStream = File.Open(path, FileMode.Open, FileAccess.Read);
        //    IExcelDataReader excelDataReader = ExcelReaderFactory.CreateOpenXmlReader(fileStream);

        //    // 表格数据全部读取到result里
        //    DataSet result = excelDataReader.AsDataSet();
        //    var table = result.Tables[0];
        //    // 获取表格有多少列
        //    int columns = result.Tables[0].Columns.Count;
        //    // 获取表格有多少行
        //    int rows = result.Tables[0].Rows.Count;


        //    //取得表格中的数据
        //    //取得table中所有的行
        //    var rowCollections = table.Rows;

        //    // 类型
        //    var rowCollection1 = rowCollections[2];//返回了第1行的集合
        //                                           // 变量名
        //    var rowCollection2 = rowCollections[3];

        //    int columnLength = table.Columns.Count; //列数
        //    int rowLength = rowCollections.Count; //行

        //    for (int i = 0; i < columnLength; i++)
        //    {
        //        names.Add(rowCollection2[i].ToString());
        //    }

        //    string dataCode = "";

        //    for (int i = 4; i < rowLength; i++)
        //    {
        //        dataCode += $"\n    config = new {fileName}();";
        //        for (int j = 0; j < columnLength; j++)
        //        {
        //            dataCode += $"\n    config.{names[j]} = {ConvertMyExcel.ConvertString(rowCollection1[j].ToString(), rowCollections[i][j]?.ToString())};";
        //        }
        //        dataCode += $"\n    allDatas.Add( config.{names[0]}, config)\n;";
        //    }
        //    string code = "using System.Collections.Generic;\nusing System;\nnamespace DataClass\n{\n\n" +
        //                  $"    public class {fileName}Manager : ConfigDataManager<{fileName}Manager,{fileName}>\n\n" +
        //                  "    {\n    " +//$"private static {fileName}Manager instance = new {fileName}Manager( );\n"+
        //                                 //$"    public static {fileName}Manager Instance()"+"{ return instance; } "+
        //                  "\n    public override void Init( )\n    {\n    name = " + $"\"{fileName}\";\n" +
        //                  $"    {fileName} config = null;\n    {dataCode}\n"
        //                  + "    base.Init();\n    }\n}\n}";
        //    FileStream fs = new FileStream(dataManager + fileName + "Manager.cs", FileMode.Create);//创建文件
        //    StreamWriter w = new StreamWriter(fs);

        //    w.Write(code);//追加数据
        //    w.Close();//释放资源,关闭文件  
        //    fs.Close();

          

        //}

        //C# 需要自己先生成类
        void GenDataClass(string className)
        {

            string code = @"
using System;
using System.Collections;
using System.Collections.Generic;

namespace DataClass
{
    public class " + className + "{\n";

            StringBuilder codeField = new StringBuilder(); ;

            foreach(var v in memberType.Keys)
            {
                codeField.Append($"        public {memberType[v]} {names[v]};\n");
            }


            code += codeField + "    }\n}";

            FileStream fs = new FileStream(dataClassPath + className + ".cs", FileMode.Create);//创建文件
            StreamWriter w = new StreamWriter(fs);

            w.Write(code);//追加数据
            w.Close();//释放资源,关闭文件  
            fs.Close();

          
            return;


        }

        
        public override BaseConvertHelper GetConvertHelper()
        {
            if(convertHelper == null)
            {
                convertHelper = new ConvertMyExcel();
            }
            return convertHelper;
        }

        class ConvertMyExcel:BaseConvertHelper
        {


        }
    }





    public interface IHasConvertHelper
    {
        BaseConvertHelper GetConvertHelper();
    }


    public abstract class BaseConvertHelper
    {
        public virtual string ConvertString(string stringType, string content)
        {
            string re = null;

            //说明没有写内容
            if (content == "")
            {
                return "null";
            }

            switch (stringType)
            {
                case "int":
                    re = ToInt(content);
                    break;
                case "string":

                    re = "\"" + content + "\"";
                    break;
                case "int[]":

                    re = ToArrayInt(content);

                    break;
                case "int[][]":
                    re = ToArrayArrayInt(content);
                    break;
                case "int[,]":
                    re = ToArrayArrayIntSameLenghth(content);
                    break;

                case "Dictionary<int,int[]>":
                    re = ToDicKeyIntValueArrayInt(content);
                    break;
                case "Dictionary<int,int>":
                    re = ToDicKeyIntValueInt(content);
                    break;
                case "float":
                    re = ToFloat(content);
                    break;
                case "Dictionary<string,int>":
                    re = ToDicKeyStringValueInt(content);
                    break;
                case "string[]":
                    re = ToArrayString(content);
                    break;
            }
            return re;
        }
        protected virtual string ToArrayString(string content)
        {

            return "new string[]" + content;
        }
        protected virtual string ToDicKeyStringValueInt(string content)
        {


            
            return "new Dictionary<string,int>"+content;

        }
        protected virtual string ToFloat(string str)
        {
            return str + "f";
        }

        protected virtual string ToInt(string str)
        {

            return str;
        }

        protected virtual string ToArrayInt(string str)
        {


            return $"new int[]{str}";
        }
        //数组， 数组的元素是int 数组， 就是 二维数组的意思
        protected virtual string ToArrayArrayInt(string str)
        {
            StringBuilder sb = new StringBuilder(str);
            sb.Replace(" ", "");
            List<int> index = new List<int>();
            for (int i = 0; i < sb.Length; i++)
            {
                if (sb[i] == '{')
                    index.Add(i);
            }
            for (int i = 0; i < index.Count; i++)
            {
                sb.Insert(index[i] + i * (9), "new int[]");
            }
            sb.Insert(0, '{');
            sb.Insert(sb.Length - 1, '}');

            return $"new int[][]{sb.ToString()}";


        }

        protected virtual string ToArrayArrayIntSameLenghth(string str)
        {
            //定长二维数组
            return "new int[,]" + str;
        }
        //解析 dic<int,int[]>
        protected virtual string ToDicKeyIntValueArrayInt(string str)
        {
            StringBuilder sb = new StringBuilder(str);
            sb.Replace(" ", "");
            List<int> index = new List<int>();
            for (int i = 0; i < sb.Length; i++)
            {
                if (sb[i] == '{')
                    index.Add(i);
            }
            for (int i = 1; i < index.Count; i = i + 2)
            {
                sb.Insert(index[i] + i / 2 * (9), "new int[]");
            }



            return "new Dictionary<int,int[]>" + sb.ToString() ;
        }

        protected virtual string ToDicKeyIntValueInt(string str)
        {
            StringBuilder sb = new StringBuilder(str);
           
            return "new Dictionary<int,int>" + sb.ToString() ;
        }
    }





    public class LuaExcelReader : CSExcelReader
    {
        public override BaseConvertHelper GetConvertHelper()
        {
            if (convertHelper == null)
            {
                convertHelper = new ConvertMyExcel();
            }
            return convertHelper;
        }

        class ConvertMyExcel : BaseConvertHelper
        {

            public override string ConvertString(string stringType, string content)
            {
                return base.ConvertString(stringType, content);
            }

            protected override string ToFloat(string str)
            {
                return str;
            }


            protected override string ToArrayString(string content)
            {
                return content;
            }

            protected override string ToDicKeyIntValueInt(string str)
            {
                return str;
            }

            protected override string ToDicKeyStringValueInt(string content)
            {
                return content;
            }

            protected override string ToArrayInt(string str)
            {
                return str;
            }

            protected override string ToArrayArrayInt(string str)
            {
                return (str);
            }

            protected override string ToArrayArrayIntSameLenghth(string str)
            {
                return str;
            }

            protected override string ToDicKeyIntValueArrayInt(string str)
            {
                return (str);
            }

            
        }

        protected override void StartGen(string fileName, DataRowCollection rowCollections, int typeRow, int nameRow, int columnLength)
        {
            try
            {
                var rowLength = rowCollections.Count;

                var typeRowData = rowCollections[typeRow];

                StringBuilder dataCode = new StringBuilder();
                StringBuilder dictionaryCode = new StringBuilder();
                int dataCount = 0;
                for (int i = 0; i < rowLength; i++)
                {
                    if (i == typeRow || i == nameRow)
                        continue;

                    bool needContinue = TryContinueNextRow(rowCollections[i][0].ToString());



                    if (needContinue == false)
                    {

                        dataCount++;

                        dataCode.Append($"\n    [ {rowCollections[i][0] }] = {{");
                        for (int j = 0; j < columnLength; j++)
                        {
                            bool needContinueCol = TryContinueNextCol(j);
                            if (needContinueCol)
                                continue;
                            else
                                dataCode.Append($"\n        {names[j]} = {GetConvertHelper().ConvertString(typeRowData[j].ToString(), rowCollections[i][j]?.ToString())}, ");
                        }
                        dataCode.Append(" },");
                       
                    }


                }

                string code = $@"
local configMap = {{ 

{dataCode}

}}
return configMap


";

                FileStream fs = new FileStream(dataManager + fileName + "Manager.lua", FileMode.Create);//创建文件
                StreamWriter w = new StreamWriter(fs);

                w.Write(code);//追加数据
                w.Close();//释放资源,关闭文件  
                fs.Close();


            }
            catch (Exception e)
            {
                LoggerError("有异常: " + e.Message);

                return;
            }

        }

    }
}


