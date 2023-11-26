
using System.Collections;
using System.Collections.Generic;
namespace ConfigPB
{
    public sealed partial class GlobalCfg
    {
        public enum KeyType
        {
            None = 0,
        }
        public void InitCustom()
        {
        }
    }
}

public class DTGlobalCfg
{
    public static DTGlobalCfg Instance;

    public Dictionary<string, ConfigPB.GlobalCfg> dicGlobalCfgs;

    public DTGlobalCfg(ConfigPB.Table table)
    {
        Instance = this;
        dicGlobalCfgs = new Dictionary<string, ConfigPB.GlobalCfg>();
        foreach (var item in table.GlobalCfg)
        {
            item.InitCustom();
            dicGlobalCfgs.Add(item.Key, item);
        }
    }

    public int GetIntByKey(ConfigPB.GlobalCfg.KeyType keyType)
    {
        return dicGlobalCfgs[keyType.ToString()].IntValue;
    }
}