

namespace LLExcelConfig
{
    using System;
    using System.Collections.Generic;
         
    public abstract class BaseConfigManager<T1,T2> where T1:new()
    {
        protected static T1 instance = new T1();
        protected string name;
        public Dictionary<int, T2> allDatas;
        public static T1 Instance()
        {
            return instance;
        }
        public BaseConfigManager()
        {
            Init();
            
        }
        public virtual void Init()
        {
           
        }
    }

}


