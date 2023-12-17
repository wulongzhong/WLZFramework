using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public class FindScriptRefTool
{
    [MenuItem("Assets/查找资源引用", false, 1)]
    static private void FindScriptRef()
    {
        EditorSettings.serializationMode = SerializationMode.ForceText;
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        var allAssetPath = AssetDatabase.GetAllAssetPaths();
        Debug.Log($"查找资源{path}引用开始");
        List<string> refPath = new List<string>();
        foreach (var assetPath in allAssetPath)
        {
            if (assetPath.Equals(path))
            {
                continue;
            }
            var allDependencies = AssetDatabase.GetDependencies(assetPath);
            foreach (var dependencie in allDependencies)
            {
                if (dependencie.Equals(path))
                {
                    refPath.Add(assetPath);
                    break;
                }
            }
        }

        refPath.Sort();
        foreach(var assetPath in refPath)
        {
            Debug.Log(assetPath, AssetDatabase.LoadAssetAtPath<Object>(assetPath));
        }

        Debug.Log("查找资源引用结束");
    }
}
