using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;
using System.IO;
using System;
using UnityEngine.UI;

public class UICSCodeGenerate
{
    const string pathUIMemberCodeTemplate = "Assets/FrameWorkCore/Editor/UI/UICodeTemplate0.cs";
    const string pathUIFucCodeTemplate = "Assets/FrameWorkCore/Editor/UI/UICodeTemplate1.cs";

    const string strRegion = "#region";
    const string strEndRegion = "#endregion";

    const string strMultipleObject = "MultipleObject";
    const string strObject = "Object";
    const string strMemberName = "memberName";

    const string strCombi = "Combi";

    string strMultipleMemberClass;
    string strMultipleMember;

    string strSingleMember;

    const string strTargetGBName = "CodeGenerateTargetGBName";

    public void Generate(GameObject targetGB)
    {
        UIRootMark uiRootMark = targetGB.GetComponent<UIRootMark>();

        Dictionary<string, UnityEngine.Object> dicMember2Object = new Dictionary<string, UnityEngine.Object>();
        Dictionary<string, Dictionary<string, UnityEngine.Object>> dicCombi2Object = new Dictionary<string, Dictionary<string, UnityEngine.Object>>();
        List<TMPro.TextMeshProUGUI> listStaticLocalizationText = new List<TMPro.TextMeshProUGUI>();
        Dictionary<Button, string> dicBtnClickSoundId = new();
        CollectMarkGB(dicMember2Object, dicCombi2Object, targetGB, listStaticLocalizationText, dicBtnClickSoundId, false);
        bool bFirstCreate = true;
        if (Directory.Exists(UIGlobalCfg.Instance.uiCodeRootPath))
        {
            if (File.Exists(Path.Combine(UIGlobalCfg.Instance.uiCodeRootPath, $"{targetGB.name}.cs")))
            {
                bFirstCreate = false;
            }
        }
        else
        {
            Directory.CreateDirectory(UIGlobalCfg.Instance.uiCodeRootPath);
        }

        {
            //create uiMemberCode
            string template = File.ReadAllText(pathUIMemberCodeTemplate);
            StringBuilder sb = new StringBuilder(template, 1024 * 16);

            var tempStrMultipleMemberClass = new StringBuilder(HandleRegionStr(sb, "#region multiple member class", nameof(strMultipleMemberClass)));
            strMultipleMember = HandleRegionStr(tempStrMultipleMemberClass, "#region member", nameof(strMultipleMember));
            strMultipleMemberClass = tempStrMultipleMemberClass.ToString();

            strSingleMember = HandleRegionStr(sb, "#region single member", nameof(strSingleMember));

            RefreshMainBody(sb, uiRootMark);
            foreach (var kv in dicCombi2Object)
            {
                RefreshMultipleBody(sb, kv.Key, kv.Value);
            }
            foreach (var kv in dicMember2Object)
            {
                RefreshMemberBody(sb, kv.Key, kv.Value.GetType().FullName);
            }
            sb.Replace(nameof(strMultipleMemberClass), "");
            sb.Replace(nameof(strSingleMember), "");
            ClearMark(sb);
            ClearEmptyLine(sb);
            string path = Path.Combine(UIGlobalCfg.Instance.uiCodeRootPath, $"{targetGB.name}.Member.cs");
            if (File.Exists(path))
            {
                if (File.ReadAllText(path).Equals(sb.ToString()))
                {
                    Debug.Log("与上一次相同的CS导出代码");
                }
                File.Delete(path);
            }
            File.WriteAllText(path, sb.ToString());
        }

        if (bFirstCreate)
        {
            //create uiFuncCode
            string template = File.ReadAllText(pathUIFucCodeTemplate);
            StringBuilder sb = new StringBuilder(template, 1024 * 16);
            RefreshMainBody(sb, uiRootMark);
            ClearMark(sb);
            ClearEmptyLine(sb);
            File.WriteAllText(Path.Combine(UIGlobalCfg.Instance.uiCodeRootPath, $"{targetGB.name}.cs"), sb.ToString());
        }

        AssetDatabase.Refresh();

        EditorPrefs.SetString(strTargetGBName, targetGB.name);
    }

    private void RefreshMainBody(StringBuilder stringBuilder, UIRootMark uiRootMark)
    {
        stringBuilder.Replace("UICodeTemplate", uiRootMark.gameObject.name);
    }

    private void RefreshMultipleBody(StringBuilder stringBuilder, string memberClassName, Dictionary<string, UnityEngine.Object> dicMembers)
    {
        StringBuilder sbMultipleMemberClass = new StringBuilder(strMultipleMemberClass);
        string strObjectTypeName = strCombi + memberClassName;
        sbMultipleMemberClass.Replace(strMultipleObject, strObjectTypeName);
        foreach (var kv in dicMembers)
        {
            StringBuilder sbMember = new StringBuilder(strMultipleMember);
            sbMember.AppendLine(nameof(strMultipleMember));
            sbMember.Replace(strObject, kv.Value.GetType().FullName);
            sbMember.Replace(strMemberName, kv.Key);
            sbMultipleMemberClass.Replace(nameof(strMultipleMember), sbMember.ToString());
        }
        sbMultipleMemberClass.Replace(nameof(strMultipleMember), "");
        sbMultipleMemberClass.AppendLine(nameof(strMultipleMemberClass));
        stringBuilder.Replace(nameof(strMultipleMemberClass), sbMultipleMemberClass.ToString());
        RefreshMemberBody(stringBuilder, memberClassName, strObjectTypeName);
    }

    private void RefreshMemberBody(StringBuilder stringBuilder, string memberName, string typeName)
    {
        StringBuilder sbSingleMember = new StringBuilder(strSingleMember);
        sbSingleMember.AppendLine(nameof(strSingleMember));
        sbSingleMember.Replace(strObject, typeName);
        sbSingleMember.Replace(strMemberName, memberName);
        stringBuilder.Replace(nameof(strSingleMember), sbSingleMember.ToString());
    }

    private string HandleRegionStr(StringBuilder stringBuilder, string reginHead, string lineMark)
    {
        int startIndex = 0;
        int endIndex = 0;
        int intervalCount = 0;

        int lineBeginIndex = 0;
        int index = 0;

        while (index < stringBuilder.Length)
        {
            if (stringBuilder[index] == '\r' || stringBuilder[index] == '\n')
            {
                string strTemp = stringBuilder.ToString(lineBeginIndex, index - lineBeginIndex);
                if (strTemp.Contains(strRegion))
                {
                    if (startIndex == 0)
                    {
                        if (strTemp.Contains(reginHead))
                        {
                            startIndex = lineBeginIndex;
                        }
                    }
                    else
                    {
                        ++intervalCount;
                    }
                }

                if (strTemp.Contains(strEndRegion))
                {
                    if (intervalCount == 0)
                    {
                        endIndex = index;
                        break;
                    }
                    else
                    {
                        --intervalCount;
                    }
                }
                lineBeginIndex = index;
            }
            ++index;
        }
        string res = stringBuilder.ToString(startIndex, endIndex - startIndex);
        stringBuilder.Replace(res, lineMark);
        return res;
    }

    private void ClearMark(StringBuilder stringBuilder)
    {
        int index = 0;
        int lineBeginIndex = 0;
        while (index < stringBuilder.Length)
        {
            if (stringBuilder[index] == '\r' || stringBuilder[index] == '\n')
            {
                string strTemp = stringBuilder.ToString(lineBeginIndex, index - lineBeginIndex);
                if (strTemp.Contains(strRegion) || strTemp.Contains(strEndRegion))
                {
                    stringBuilder.Remove(lineBeginIndex, index - lineBeginIndex);
                    index = lineBeginIndex;
                }
                else
                {
                    lineBeginIndex = index;
                }
            }
            ++index;
        }
    }

    private void ClearEmptyLine(StringBuilder stringBuilder)
    {
        for (int i = 0; i < 16; ++i)
            stringBuilder.Replace("\r\n\r\n", "\r\n");
    }

    public static void DoCreateUIPf(GameObject targetGB)
    {
        var name = targetGB.name;
        targetGB = UnityEngine.GameObject.Instantiate(targetGB);
        targetGB.name = name;
        int modeTypeIndex = 0;
        string btnDefaultClickSoundId = "";
        if (targetGB.GetComponent<UIBase>() != null)
        {
            modeTypeIndex = (int)targetGB.GetComponent<UIBase>().localizationModuleType;
            btnDefaultClickSoundId = targetGB.GetComponent<UIBase>().btnDefaultClickSoundId;
            GameObject.DestroyImmediate(targetGB.GetComponent<UIBase>());
        }

        Dictionary<string, UnityEngine.Object> dicMember2Object = new Dictionary<string, UnityEngine.Object>();
        Dictionary<string, Dictionary<string, UnityEngine.Object>> dicCombi2Object = new Dictionary<string, Dictionary<string, UnityEngine.Object>>();
        Dictionary<Button, string> dicBtnClickSoundId = new();
        List<TMPro.TextMeshProUGUI> listStaticLocalizationText = new List<TMPro.TextMeshProUGUI>();
        CollectMarkGB(dicMember2Object, dicCombi2Object, targetGB, listStaticLocalizationText, dicBtnClickSoundId, true);

        var uiRootMark = targetGB.GetComponent<UIRootMark>();
        var scriptPath = Path.Combine(UIGlobalCfg.Instance.uiCodeRootPath, $"{targetGB.name}.cs");
        MonoScript monoScript = AssetDatabase.LoadAssetAtPath<MonoScript>(scriptPath);
        Type uiType = monoScript.GetClass();
        var uiComp = targetGB.GetComponent(uiType);
        if (uiComp == null)
        {
            uiComp = targetGB.AddComponent(uiType);
        }

        SerializedObject serializedObject = new SerializedObject(uiComp, targetGB);

        {
            SerializedProperty arrStaticLocalizationTextProperty = serializedObject.FindProperty("arrStaticLocalizationText");
            arrStaticLocalizationTextProperty.arraySize = listStaticLocalizationText.Count;
            for (int i = 0; i < listStaticLocalizationText.Count; ++i)
            {
                arrStaticLocalizationTextProperty.GetArrayElementAtIndex(i).objectReferenceValue = listStaticLocalizationText[i];
            }
        }

        {
            SerializedProperty arrPlaySoundBtnProperty = serializedObject.FindProperty("arrPlaySoundBtn");
            arrPlaySoundBtnProperty.arraySize = dicBtnClickSoundId.Count;

            SerializedProperty arrBtnSoundIdProperty = serializedObject.FindProperty("arrBtnSoundId");
            arrBtnSoundIdProperty.arraySize = dicBtnClickSoundId.Count;

            int tempIndex = 0;
            foreach (var kv in dicBtnClickSoundId)
            {
                arrPlaySoundBtnProperty.GetArrayElementAtIndex(tempIndex).objectReferenceValue = kv.Key;
                arrBtnSoundIdProperty.GetArrayElementAtIndex(tempIndex).stringValue = kv.Value;
                ++tempIndex;
            }
        }

        foreach (var kv in dicCombi2Object)
        {
            var combiProperty = serializedObject.FindProperty(kv.Key);
            foreach (var name2obj in kv.Value)
            {
                combiProperty.FindPropertyRelative(name2obj.Key).objectReferenceValue = name2obj.Value;
            }
        }
        foreach (var kv in dicMember2Object)
        {
            serializedObject.FindProperty(kv.Key).objectReferenceValue = kv.Value;
        }
        serializedObject.FindProperty("localizationModuleType").enumValueIndex = modeTypeIndex;
        serializedObject.FindProperty("btnDefaultClickSoundId").stringValue = btnDefaultClickSoundId;
        serializedObject.ApplyModifiedProperties();

        var gb = PrefabUtility.SaveAsPrefabAsset(targetGB, Path.Combine(uiRootMark.prefabFolderPath, $"{targetGB.name}.prefab"));
        //AssetDatabase.Refresh();
        //ClearPrefabMark(AssetDatabase.LoadAssetAtPath<GameObject>(Path.Combine(uiRootMark.prefabDir, $"{targetGB.name}.prefab")));
        ClearPrefabMark(gb);
        GameObject.DestroyImmediate(targetGB);
    }

    public static void CollectMarkGB(Dictionary<string, UnityEngine.Object> dicMember2Object, Dictionary<string, Dictionary<string, UnityEngine.Object>> dicCombi2Object, GameObject targetGB, List<TMPro.TextMeshProUGUI> listStaticLocalizationText, Dictionary<Button, string> dicBtnSoundId, bool bUnpackPrefab)
    {
        List<GameObject> listMemberCheckGB = new List<GameObject>(256);
        listMemberCheckGB.Add(targetGB);
        int index = 0;
        while (index < listMemberCheckGB.Count)
        {
            GameObject check = listMemberCheckGB[index];
            for (int i = 0; i < check.transform.childCount; i++)
            {
                listMemberCheckGB.Add(check.transform.GetChild(i).gameObject);
            }
            if (bUnpackPrefab && UnityEditor.PrefabUtility.IsAnyPrefabInstanceRoot(check))
            {
                Debug.Log(check.name);
                UnityEditor.PrefabUtility.UnpackPrefabInstance(check, UnityEditor.PrefabUnpackMode.Completely, UnityEditor.InteractionMode.AutomatedAction);
            }
            if (check.GetComponent<TMPro.TextMeshProUGUI>() != null)
            {
                if (check.GetComponent<TextNotLocalizationMark>() == null)
                {
                    listStaticLocalizationText.Add(check.GetComponent<TMPro.TextMeshProUGUI>());
                }
            }

            var btn = check.GetComponent<Button>();
            if (btn != null)
            {
                ButtonSpecialClickSound buttonSpecialClickSound = check.GetComponent<ButtonSpecialClickSound>();
                if (buttonSpecialClickSound != null)
                {
                    if (!string.IsNullOrEmpty(buttonSpecialClickSound.soundId))
                    {
                        dicBtnSoundId.Add(btn, buttonSpecialClickSound.soundId);
                    }
                }
                else
                {
                    dicBtnSoundId.Add(btn, "default");
                }
            }

            UIMemberMark mark = check.GetComponent<UIMemberMark>();
            if (mark != null)
            {
                switch (mark.quantityMode)
                {
                    case UIMemberMark.QuantityMode.Single:
                        if (!mark.singleMemberInfo.memberObj)
                        {
                            Debug.LogError($"info.memberObj is null, {mark.singleMemberInfo.memberName} {check.name}", check);
                            return;
                        }
                        dicMember2Object.Add(mark.singleMemberInfo.memberName, mark.singleMemberInfo.memberObj);
                        break;
                    case UIMemberMark.QuantityMode.Multiple:
                        if (mark.bUseGroup)
                        {
                            Dictionary<string, UnityEngine.Object> tempMember2Obj = new Dictionary<string, UnityEngine.Object>();
                            foreach (var info in mark.multipleMemberInfo)
                            {
                                if (!info.memberObj)
                                {
                                    Debug.LogError("info.memberObj is null", mark);
                                    return;
                                }
                                tempMember2Obj.Add(info.memberName, info.memberObj);
                            }
                            dicCombi2Object.Add(mark.groupName, tempMember2Obj);
                        }
                        else
                        {
                            foreach (var info in mark.multipleMemberInfo)
                            {
                                if (!info.memberObj)
                                {
                                    Debug.LogError("info.memberObj is null", mark);
                                    return;
                                }
                                dicMember2Object.Add(info.memberName, info.memberObj);
                            }
                        }
                        break;
                }
            }

            ++index;
        }
    }

    public static void ClearPrefabMark(GameObject targetGB)
    {
        UnityEngine.Object.DestroyImmediate(targetGB.GetComponent<UIRootMark>(), true);
        List<GameObject> listMemberCheckGB = new List<GameObject>(256);
        listMemberCheckGB.Add(targetGB);
        int index = 0;
        while (index < listMemberCheckGB.Count)
        {
            GameObject check = listMemberCheckGB[index];
            for (int i = 0; i < check.transform.childCount; i++)
            {
                listMemberCheckGB.Add(check.transform.GetChild(i).gameObject);
            }

            UIMemberMark mark = check.GetComponent<UIMemberMark>();
            if (mark != null)
            {
                UnityEngine.Object.DestroyImmediate(mark, true);
            }
            TextNotLocalizationMark textNotLocalizationMark = check.GetComponent<TextNotLocalizationMark>();
            if (textNotLocalizationMark != null)
            {
                UnityEngine.Object.DestroyImmediate(textNotLocalizationMark, true);
            }
            ++index;
        }
    }
}