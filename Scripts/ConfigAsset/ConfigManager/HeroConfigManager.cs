
using System.Collections.Generic;                   
using System;
namespace DataClass
{
    public class HeroConfigManager :BaseConfig<HeroConfigManager,HeroConfig>
    {
        public override void Init( )
        {
            
            name = "HeroConfig";
            
            

            HeroConfig config1 = new HeroConfig();
            config1.id = 1;
            config1.name = "罗昕宇";
            config1.attack = 150f;
            config1.hp = 300;
            config1.speed = 10;

            HeroConfig config2 = new HeroConfig();
            config2.id = 15;
            config2.name = "夏实";
            config2.attack = 200f;
            config2.hp = 999;
            config2.speed = 15;

            allDatas = new Dictionary<int, HeroConfig>(2){
            
                { 1 ,config1 },{ 15 ,config2 },

            };

            base.Init();
        }

    }


}




