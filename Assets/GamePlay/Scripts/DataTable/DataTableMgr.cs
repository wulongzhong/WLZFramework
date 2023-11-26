using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataTableMgr : MonoBehaviour
{
    public static DataTableMgr Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void InitDataTable()
    {
#if ENABLE_LOG
        Debug.Log("开始加载配置");
#endif
        ResLoader resLoader = new ResLoader();
        var data = resLoader.LoadAsset<TextAsset>("Assets/GamePlay/DataTable/all.bytes");
        ConfigPB.Table table = ConfigPB.Table.Parser.ParseFrom(data.bytes);

        DTGlobalCfg dTGlobalCfg = new DTGlobalCfg(table);
        DTLocalization dTLocalization = new DTLocalization(table, SystemLanguage.English);
        DTSound dTSound = new DTSound(table);

        resLoader.Dispose();
    }
}
