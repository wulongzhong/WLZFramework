using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEditor.SceneManagement;

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
        foreach (var assetPath in refPath)
        {
            Debug.Log(assetPath, AssetDatabase.LoadAssetAtPath<Object>(assetPath));
        }

        Debug.Log("查找资源引用结束");
    }

    [MenuItem("Assets/在当前场景中的引用", false, 1)]
    static private void FindScriptRefInScene()
    {
        EditorSettings.serializationMode = SerializationMode.ForceText;
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        var instanceId = Selection.activeInstanceID;
        var guid = AssetDatabase.AssetPathToGUID(path);


        Debug.Log($"查找资源在场景中的{path}引用开始");

        List<GameObject> listWaitCheckObj = new List<GameObject>();

        EditorSceneManager.GetActiveScene().GetRootGameObjects(listWaitCheckObj);
        while (listWaitCheckObj.Count > 0)
        {
            var lastObj = listWaitCheckObj[^1];
            listWaitCheckObj.RemoveAt(listWaitCheckObj.Count - 1);
            for (int i = 0; i < lastObj.transform.childCount; i++)
            {
                listWaitCheckObj.Add(lastObj.transform.GetChild(i).gameObject);
            }
            foreach (var bev in lastObj.GetComponents<UnityEngine.Component>())
            {
                if (bev == null)
                {
                    continue;
                }
                SerializedObject serialized = new SerializedObject(bev);
                var iterator = serialized.GetIterator();
                iterator.Reset();
                while (iterator.NextVisible(true))
                {
                    if (iterator.propertyType == SerializedPropertyType.ObjectReference && iterator.objectReferenceInstanceIDValue == instanceId)
                    {
                        Debug.Log($"{GetGameObjectPath(lastObj)} : {iterator.propertyPath}", lastObj);
                    }
                }
            }
        }

        Debug.Log("查找资源在场景中的引用结束");
    }

    [MenuItem("GameObject/在场景中的引用", false, 1)]
    static private void FindGameObjectRefInScene()
    {
        EditorSettings.serializationMode = SerializationMode.ForceText;
        var instanceId = Selection.activeInstanceID;

        var listInstanceId = new List<int>();
        listInstanceId.Add(Selection.activeInstanceID);
        foreach (var bev in Selection.activeGameObject.GetComponents<UnityEngine.Component>())
        {
            listInstanceId.Add(bev.GetInstanceID());
        }

        Debug.Log($"查找资源在场景中的{Selection.activeGameObject.name}引用开始");

        List<GameObject> listWaitCheckObj = new List<GameObject>();
        EditorSceneManager.GetActiveScene().GetRootGameObjects(listWaitCheckObj);
        while (listWaitCheckObj.Count > 0)
        {
            var lastObj = listWaitCheckObj[^1];
            listWaitCheckObj.RemoveAt(listWaitCheckObj.Count - 1);
            for (int i = 0; i < lastObj.transform.childCount; i++)
            {
                listWaitCheckObj.Add(lastObj.transform.GetChild(i).gameObject);
            }
            foreach (var bev in lastObj.GetComponents<UnityEngine.Component>())
            {
                if (bev == null)
                {
                    continue;
                }
                SerializedObject serialized = new SerializedObject(bev);
                var iterator = serialized.GetIterator();
                iterator.Reset();
                while (iterator.NextVisible(true))
                {
                    if (iterator.propertyType == SerializedPropertyType.ObjectReference && listInstanceId.Contains(iterator.objectReferenceInstanceIDValue))
                    {
                        Debug.Log($"{GetGameObjectPath(lastObj)} : {iterator.propertyPath}", lastObj);
                    }
                }
            }
        }
        Debug.Log("查找资源在场景中的引用结束");
    }

    private static string GetGameObjectPath(GameObject go)
    {
        StringBuilder goPath = new StringBuilder(go.name);
        var parent = go.transform.parent;
        while (parent != null)
        {
            goPath.Insert(0, "/").Insert(0, parent.gameObject.name);
            parent = parent.parent;
        }
        return goPath.ToString();
    }
}
