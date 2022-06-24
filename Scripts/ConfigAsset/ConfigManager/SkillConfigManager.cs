
using System.Collections.Generic;                   
using System;
namespace DataClass
{
    public class SkillConfigManager :BaseConfig<SkillConfigManager,SkillConfig>
    {
        public override void Init( )
        {
            
            name = "SkillConfig";
            
            

            SkillConfig config1 = new SkillConfig();
            config1.id = 1;
            config1.name = "冲击波";
            config1.sp = 15;

            SkillConfig config2 = new SkillConfig();
            config2.id = 2;
            config2.name = "狂风战";
            config2.sp = 18;

            SkillConfig config3 = new SkillConfig();
            config3.id = 3;
            config3.name = "超级风暴";
            config3.sp = 50;

            allDatas = new Dictionary<int, SkillConfig>(3){
            
                { 1 ,config1 },{ 2 ,config2 },{ 3 ,config3 },

            };

            base.Init();
        }

    }


}




