
using System.Collections.Generic;                   
using System;
namespace DataClass
{
    public class TestExcelManager :BaseConfig<TestExcelManager,TestExcel>
    {
        public override void Init( )
        {
            
            name = "TestExcel";
            
            

            TestExcel config1 = new TestExcel();
            config1.id = 1;
            config1.name = "罗昕宇";
            config1.hp = 300;

            allDatas = new Dictionary<int, TestExcel>(1){
            
                { 1 ,config1 },

            };

            base.Init();
        }

    }


}




