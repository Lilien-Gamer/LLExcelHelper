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

        protected const string BaseConfigClassName = "BaseConfigManager";

        public bool hasError = false;

        //protected virtual string BaseConfigClassCode { get { return null; } }

        protected const string NameSpaceName = "LLExcelConfig";

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

        //创建 文件夹
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
            Console.WriteLine("\n\n"+content);
            Console.ReadKey();
        }

        //查找定义 类型 和 名称 的行
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
            try
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
            catch (Exception e)
            {

                LoggerError($"有异常: {e.Message} \n {e.StackTrace}");

                return;
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

                        var typeRowData = rowCollections[typeRow];

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
            { return $@"

namespace {NameSpaceName}
{{
    using System;
    using System.Collections.Generic;
         
    public abstract class {BaseConfigClassName}<T1,T2> where T1:new()
    {{
        protected static T1 instance = new T1();
        protected string name;
        public Dictionary<int, T2> allDatas;
        public static T1 Instance()
        {{
            return instance;
        }}
        public {BaseConfigClassName}()
        {{
            Init();
            
        }}
        public virtual void Init()
        {{
           
        }}
    }}

}}


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

                    var firstValue = rowCollections[i][0].ToString();
                    bool needContinue = TryContinueNextRow(firstValue);



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
                        dictionaryCode.Append($"{{ {firstValue} ,config{dataCount} }},");
                    }


                }

                string code = $@"
using System.Collections.Generic;                   
using System;
namespace {NameSpaceName}
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


      
        void GenDataClass(string className)
        {

            string code = $@"
using System;
using System.Collections;
using System.Collections.Generic;

namespace {NameSpaceName}
{{
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
            sb.Remove(0, 1);
            sb.Remove(sb.Length - 1, 1);

            //sb.Replace(" ", "");
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
            

            return $"new int[][] {{ {sb.ToString()} }}";


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
            sb.Remove(0, 1);
            sb.Remove(sb.Length - 1, 1);

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



            return $"new Dictionary<int,int[]>{{ {sb} }}";
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


