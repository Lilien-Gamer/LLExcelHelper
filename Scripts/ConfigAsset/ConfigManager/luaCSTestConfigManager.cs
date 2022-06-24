
using System.Collections.Generic;                   
using System;
namespace DataClass
{
    public class luaCSTestConfigManager :BaseConfig<luaCSTestConfigManager,luaCSTestConfig>
    {
        public override void Init( )
        {
            
            name = "luaCSTestConfig";
            
            

            luaCSTestConfig config1 = new luaCSTestConfig();
            config1.id = 100;
            config1.heroId = 26;
            config1.attack = 12.354f;
            config1.heroName = "英雄名字";
            config1.skillNameList = new string[]{"小明","小红","小王"};
            config1.addressIntDic = new Dictionary<string,int>{  {"四川", 15}, {"北京",100}  };
            config1.intArrayDataintArrayData2 = new int[]{15,19,20};
            config1.testName = new int[,]{ {1,2,3},{4,5,6},{7,8,9}  };
            config1.intArrayData3 = new int[][] { new int[]{1,2},new int[]{2,5,9},new int[]{1,7,8,9},new int[]{9} };
            config1.test1 = new Dictionary<int,int>{  {15,89}, {51,48}, {17,20}};
            config1.test2 = new Dictionary<int,int[]>{ {1,new int[]{1,8,9}},{1005,new int[]{5,8,9,45}},{28,new int[]{15,9,15}} };

            luaCSTestConfig config2 = new luaCSTestConfig();
            config2.id = 102;
            config2.heroId = 28;
            config2.attack = 12.354f;
            config2.heroName = "英雄名字";
            config2.skillNameList = new string[]{"小明","小红","小王"};
            config2.addressIntDic = new Dictionary<string,int>{  {"四川", 15}, {"北京",100}  };
            config2.intArrayDataintArrayData2 = new int[]{15,19,20};
            config2.testName = new int[,]{ {1,2,3},{4,5,6},{7,8,9}  };
            config2.intArrayData3 = new int[][] { new int[]{1,2},new int[]{2,5,9},new int[]{1,7,8,9},new int[]{9} };
            config2.test1 = new Dictionary<int,int>{  {15,89}, {51,48}, {17,20}};
            config2.test2 = new Dictionary<int,int[]>{ {1,new int[]{1,8,9}},{1005,new int[]{5,8,9,45}},{28,new int[]{15,9,15}} };

            allDatas = new Dictionary<int, luaCSTestConfig>(2){
            
                { 100 ,config1 },{ 102 ,config2 },

            };

            base.Init();
        }

    }


}




