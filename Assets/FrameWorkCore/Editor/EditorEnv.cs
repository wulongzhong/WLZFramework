using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public static class EditorEnv
{
    #region 用户程序集名字
    private static List<string> sListUserAssemblyName;
    public static List<string> GetUserAssemblyNames()
    {
        if(sListUserAssemblyName == null)
        {
            sListUserAssemblyName = new List<string>();
            //添加默认程序集名字
            sListUserAssemblyName.Add("Assembly-CSharp");
            sListUserAssemblyName.Add("Assembly-CSharp-Editor");
            //搜索程序集文件
            var paths = AssetDatabase.GetAllAssetPaths();
            foreach(var path in paths)
            {
                //跳过Packages里的资源文件
                if(path.IndexOf("Packages/") == 0)
                {
                    continue;
                }
                var assetType = AssetDatabase.GetMainAssetTypeAtPath(path);
                if (assetType == typeof(AssemblyDefinitionAsset))
                {
                    var asset = AssetDatabase.LoadAssetAtPath<TextAsset>(path);

                    var jsData = JsonMapper.ToObject(asset.text);
                    if(!jsData.ContainsKey("name"))
                    {
                        continue;
                    }

                    sListUserAssemblyName.Add(jsData["name"].ToString());
                }
            }
        }
        return sListUserAssemblyName;
    }

    #endregion

}