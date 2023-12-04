using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UIMemberMark))]
public class UIMemberMarkEditor : Editor
{
    public override void OnInspectorGUI()
    {
        UIMemberMark uiMemberMark = target as UIMemberMark;
        var quantityMode = serializedObject.FindProperty(nameof(uiMemberMark.quantityMode));
        quantityMode.intValue = GUILayout.Toolbar(quantityMode.intValue, new string[] { "Single", "Multiple" });

        switch (uiMemberMark.quantityMode)
        {
            case UIMemberMark.QuantityMode.Single:
                DrawSingle(uiMemberMark);
                break;

            case UIMemberMark.QuantityMode.Multiple:
                DrawMultiple(uiMemberMark);
                break;
        }
        serializedObject.ApplyModifiedProperties();
    }

    private void DrawSingle(UIMemberMark uiMemberMark)
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(uiMemberMark.singleMemberInfo)));
    }

    private void DrawMultiple(UIMemberMark uiMemberMark)
    {
        var useGroup = serializedObject.FindProperty(nameof(uiMemberMark.bUseGroup));
        EditorGUILayout.PropertyField(useGroup, new GUIContent("使用组"));
        if (useGroup.boolValue)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(uiMemberMark.groupName)));
        }
        EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(uiMemberMark.multipleMemberInfo)));
    }
}

[CustomPropertyDrawer(typeof(UIMemberMark.MemberInfo))]
public class UIMemberMarkMemberInfoEditor : PropertyDrawer
{
    private const int height = 48;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        UIMemberMark uiMemberMark = property.serializedObject.targetObject as UIMemberMark;

        position.height = height / 2;
        var allBev = uiMemberMark.GetComponents<UnityEngine.Component>().ToList();
        allBev.Remove(uiMemberMark);
        var allName = (from bev in allBev
                       where bev != uiMemberMark
                       select bev.GetType().FullName)
                      .ToArray();
        Object lastSelectObj = property.FindPropertyRelative("memberObj").objectReferenceValue;
        int lastIndex = 0;
        bool bInit = true;
        for (int i = 0; i < allBev.Count; ++i)
        {
            if (allBev[i] == lastSelectObj)
            {
                lastIndex = i;
                bInit = false;
                break;
            }
        }
        if (bInit)
        {
            property.FindPropertyRelative("memberName").stringValue = uiMemberMark.gameObject.name;
            property.FindPropertyRelative("memberObj").objectReferenceValue = allBev[0];
            int priority = 0;
            foreach (var bev in allBev)
            {
                if (bev.GetType().FullName.Contains("UI.Button"))
                {
                    if (priority < 2)
                    {
                        property.FindPropertyRelative("memberObj").objectReferenceValue = bev;
                        priority = 2;
                    }

                }
                if (bev.GetType().FullName.Contains("TextMeshProUGUI"))
                {
                    if (priority < 2)
                    {
                        property.FindPropertyRelative("memberObj").objectReferenceValue = bev;
                        priority = 2;
                    }
                }
                if (bev.GetType().FullName.Contains("UI.Image"))
                {
                    if (priority < 1)
                    {
                        property.FindPropertyRelative("memberObj").objectReferenceValue = bev;
                        priority = 1;
                    }
                }
            }

        }
        int nowIndex = EditorGUI.Popup(position, lastIndex, allName);
        if (nowIndex != lastIndex)
        {
            property.FindPropertyRelative("memberObj").objectReferenceValue = allBev[nowIndex];
        }
        position.y += position.height;
        EditorGUI.PropertyField(position, property.FindPropertyRelative("memberName"), new GUIContent("字段命名"));
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return height;
    }
}