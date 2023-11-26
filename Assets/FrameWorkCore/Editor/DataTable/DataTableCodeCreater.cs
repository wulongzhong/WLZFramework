using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

public class DataTableCodeCreater : EditorWindow
{
    [MenuItem("FrameworkTools/配置表代码生成器")]
    public static void OpenDTCodeCreater()
    {
        DataTableCodeCreater window = GetWindow<DataTableCodeCreater>();
        window.titleContent = new GUIContent("配置表代码生成器");
        window.Show();
    }

    private string dtName;
    private string indexType = "int";
    private string indexKey = "ID";
    private void OnGUI()
    {
        GUILayout.Label("表名");
        dtName = GUILayout.TextField(dtName);
        GUILayout.Label("索引类型");
        indexType = GUILayout.TextField(indexType);
        GUILayout.Label("索引字段");
        indexKey = GUILayout.TextField(indexKey);
        if (GUILayout.Button("生成"))
        {
            Create();
        }
    }


    private void Create()
    {
        var t =
@"
using System.Collections;
using System.Collections.Generic;
namespace ConfigPB
{
    public sealed partial class #TableName
    {
        public void InitCustom()
        {
        }
    }
}

public class DT#TableName
{
    public static DT#TableName Instance;

    public Dictionary<int, ConfigPB.#TableName> dic#TableNames;

    public DT#TableName(ConfigPB.Table table)
    {
        Instance = this;
        dic#TableNames = new Dictionary<int, ConfigPB.#TableName>();
        foreach (var item in table.#TableName)
        {
            item.InitCustom();
            dic#TableNames.Add(item.Id, item);
        }
    }

    public ConfigPB.#TableName Get#TableNameById(int id)
    {
        return dic#TableNames[id];
    }
}
".Replace("#TableName", dtName).Replace("int", indexType).Replace("Id", indexKey);
        System.IO.File.WriteAllText("Assets/GamePlay/Scripts/DataTable/DT" + dtName + ".cs", t);
        AssetDatabase.Refresh();
    }
}