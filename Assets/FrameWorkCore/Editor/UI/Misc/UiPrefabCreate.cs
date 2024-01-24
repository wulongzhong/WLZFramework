using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;
using UnityEngine.UI;

public class UiPrefabCreate
{
    static UiDevToolObject toolObject;
    int currSelectIndex = 0;
    CreateInfo[] arrCreateInfo;
    Vector2 scrollPos = Vector2.zero;
    public void Init(UiDevToolObject toolObject)
    {
        UiPrefabCreate.toolObject = toolObject;
        arrCreateInfo = new CreateInfo[]
        {
            new CreatButtonAndChangeSprite(),
            new CreateButtonAndToDark(),
            new CreateImgNor(),
            new CreateImgSliced(),
            new CreateHorList(),
            new CreateVerList(),
            new CreateGridList(),
            new CreateCheckBox(),
        };
    }

    public void Draw()
    {
        EditorGUILayout.BeginHorizontal();
        DrawPrefabList();
        EditorGUILayout.BeginVertical();
        arrCreateInfo[currSelectIndex].Draw();
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
    }

    private void DrawPrefabList()
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(160));
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        for(int i = 0; i < arrCreateInfo.Length; ++i)
        {
            string prefabName = arrCreateInfo[i].name;
            if(i == currSelectIndex)
            {
                GUI.color = Color.green;
            }
            if (GUILayout.Button(prefabName))
            {
                currSelectIndex = i;
            }
            if (i == currSelectIndex)
            {
                GUI.color = Color.white;
            }
        }
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    private abstract class CreateInfo
    {
        public string name;
        protected string strTempletePath;
        protected string strSavePath = "Assets/GamePlay/Prefabs/UIItem/";

        public CreateInfo()
        {
            Init();
        }

        public abstract void Init();

        public void Draw()
        {
            DrawParmter();
            DrawSave();
        }

        public abstract void DrawParmter();

        protected abstract void ApplyParamter();

        private void DrawSave()
        {
            strSavePath = EditorGUILayout.TextField(strSavePath);
            if (GUILayout.Button("创建"))
            {
                AssetDatabase.CopyAsset($"{strTempletePath}.prefab", $"{strSavePath}.prefab");
                AssetDatabase.Refresh();
                ApplyParamter();
            }
        }
    }

    private class CreatButtonAndChangeSprite : CreateInfo
    {
        private Sprite norSprite;
        private Sprite downSprite;
        public override void Init()
        {
            name = "按钮_按下替换图片";
            strTempletePath = $"{toolObject.uiTempletePath}/btn_sprite";
        }

        public override void DrawParmter()
        {
            EditorGUILayout.LabelField("正常状态时图片:");
            norSprite = (Sprite)EditorGUILayout.ObjectField(norSprite, typeof(Sprite), false, GUILayout.ExpandHeight(true));
            EditorGUILayout.LabelField("按住时的图片");
            downSprite = (Sprite)EditorGUILayout.ObjectField(downSprite, typeof(Sprite), false, GUILayout.ExpandHeight(true));
        }

        protected override void ApplyParamter()
        {
            GameObject gb = AssetDatabase.LoadAssetAtPath<GameObject>($"{strSavePath}.prefab");

            gb.GetComponent<RectTransform>().sizeDelta = norSprite.textureRect.size;

            gb.GetComponent<Image>().sprite = norSprite;
            var spriteState = gb.GetComponent<Button>().spriteState;
            spriteState.pressedSprite = downSprite;
            gb.GetComponent<Button>().spriteState = spriteState;
            PrefabUtility.SavePrefabAsset(gb);
        }
    }

    private class CreateButtonAndToDark : CreateInfo
    {
        private Sprite norSprite;
        public override void Init()
        {
            name = "按钮_按下变暗";
            strTempletePath = $"{toolObject.uiTempletePath}/btn_dark";
        }

        public override void DrawParmter()
        {
            EditorGUILayout.LabelField("正常状态时图片:");
            norSprite = (Sprite)EditorGUILayout.ObjectField(norSprite, typeof(Sprite), false, GUILayout.ExpandHeight(true));
        }

        protected override void ApplyParamter()
        {
            GameObject gb = AssetDatabase.LoadAssetAtPath<GameObject>($"{strSavePath}.prefab");

            gb.GetComponent<RectTransform>().sizeDelta = norSprite.textureRect.size;
            gb.GetComponent<Image>().sprite = norSprite;
            PrefabUtility.SavePrefabAsset(gb);
        }
    }

    private class CreateImgNor : CreateInfo
    {
        private Sprite sprite;
        public override void Init()
        {
            name = "UI图片_普通";
            strTempletePath = $"{toolObject.uiTempletePath}/img_simple";
        }

        public override void DrawParmter()
        {
            EditorGUILayout.LabelField("图片:");
            sprite = (Sprite)EditorGUILayout.ObjectField(sprite, typeof(Sprite), false, GUILayout.ExpandHeight(true));
        }

        protected override void ApplyParamter()
        {
            GameObject gb = AssetDatabase.LoadAssetAtPath<GameObject>($"{strSavePath}.prefab");

            gb.GetComponent<RectTransform>().sizeDelta = sprite.textureRect.size;
            gb.GetComponent<Image>().sprite = sprite;
            PrefabUtility.SavePrefabAsset(gb);
        }
    }

    private class CreateImgSliced : CreateInfo
    {
        private Sprite sprite;
        public override void Init()
        {
            name = "UI图片_九宫格";
            strTempletePath = $"{toolObject.uiTempletePath}/img_sliced";
        }

        public override void DrawParmter()
        {
            EditorGUILayout.LabelField("图片:");
            sprite = (Sprite)EditorGUILayout.ObjectField(sprite, typeof(Sprite), false, GUILayout.ExpandHeight(true));
        }

        protected override void ApplyParamter()
        {
            GameObject gb = AssetDatabase.LoadAssetAtPath<GameObject>($"{strSavePath}.prefab");

            gb.GetComponent<RectTransform>().sizeDelta = sprite.textureRect.size;
            gb.GetComponent<Image>().sprite = sprite;
            PrefabUtility.SavePrefabAsset(gb);
        }
    }

    private class CreateHorList : CreateInfo
    {
        Sprite bgSprite;
        RectOffset padding = new RectOffset();
        int childSpace;
        public override void Init()
        {
            name = "水平列表";
            strTempletePath = $"{toolObject.uiTempletePath}/list_hor";
        }

        public override void DrawParmter()
        {
            EditorGUILayout.LabelField("背景图片");
            bgSprite = (Sprite)EditorGUILayout.ObjectField(bgSprite, typeof(Sprite), false, GUILayout.ExpandHeight(true));
            EditorGUILayout.LabelField("与左边空隙");
            padding.left = EditorGUILayout.IntField(padding.left);
            EditorGUILayout.LabelField("与右边空隙");
            padding.right = EditorGUILayout.IntField(padding.right);
            EditorGUILayout.LabelField("子物体之间空隙");
            childSpace = EditorGUILayout.IntField(childSpace);
        }

        protected override void ApplyParamter()
        {
            GameObject gb = AssetDatabase.LoadAssetAtPath<GameObject>($"{strSavePath}.prefab");

            gb.GetComponent<RectTransform>().sizeDelta = bgSprite.textureRect.size;
            gb.GetComponent<Image>().sprite = bgSprite;
            HorizontalLayoutGroup group = gb.transform.GetChild(0).GetChild(0).GetComponent<HorizontalLayoutGroup>();
            group.padding = padding;
            group.spacing = childSpace;
            PrefabUtility.SavePrefabAsset(gb);
        }
    }

    private class CreateVerList : CreateInfo
    {
        Sprite bgSprite;
        RectOffset padding = new RectOffset();
        int childSpace;
        public override void Init()
        {
            name = "垂直列表";
            strTempletePath = $"{toolObject.uiTempletePath}/list_ver";
        }

        public override void DrawParmter()
        {
            EditorGUILayout.LabelField("背景图片");
            bgSprite = (Sprite)EditorGUILayout.ObjectField(bgSprite, typeof(Sprite), false, GUILayout.ExpandHeight(true));

            EditorGUILayout.LabelField("与顶部空隙");
            padding.top = EditorGUILayout.IntField(padding.top);
            EditorGUILayout.LabelField("与底部空隙");
            padding.bottom = EditorGUILayout.IntField(padding.bottom);

            EditorGUILayout.LabelField("子物体之间空隙");
            childSpace = EditorGUILayout.IntField(childSpace);
        }

        protected override void ApplyParamter()
        {
            GameObject gb = AssetDatabase.LoadAssetAtPath<GameObject>($"{strSavePath}.prefab");

            gb.GetComponent<RectTransform>().sizeDelta = bgSprite.textureRect.size;
            gb.GetComponent<Image>().sprite = bgSprite;
            VerticalLayoutGroup group = gb.transform.GetChild(0).GetChild(0).GetComponent<VerticalLayoutGroup>();
            group.padding = padding;
            group.spacing = childSpace;
            PrefabUtility.SavePrefabAsset(gb);
        }
    }

    private class CreateGridList : CreateInfo
    {
        Sprite bgSprite;
        RectOffset padding = new RectOffset();
        Vector2Int childSize;
        Vector2 childSpace;
        public override void Init()
        {
            name = "格子列表";
            strTempletePath = $"{toolObject.uiTempletePath}/list_grid";
        }

        public override void DrawParmter()
        {
            EditorGUILayout.LabelField("背景图片");
            bgSprite = (Sprite)EditorGUILayout.ObjectField(bgSprite, typeof(Sprite), false, GUILayout.ExpandHeight(true));
            EditorGUILayout.LabelField("与顶部空隙");
            padding.top = EditorGUILayout.IntField(padding.top);
            EditorGUILayout.LabelField("与底部空隙");
            padding.bottom = EditorGUILayout.IntField(padding.bottom);
            EditorGUILayout.LabelField("与左边空隙");
            padding.left = EditorGUILayout.IntField(padding.left);
            EditorGUILayout.LabelField("与右边空隙");
            padding.right = EditorGUILayout.IntField(padding.right);
            childSize = EditorGUILayout.Vector2IntField("子物体大小", childSize);
            childSpace = EditorGUILayout.Vector2Field("子物体之间空隙", childSpace);
        }

        protected override void ApplyParamter()
        {
            GameObject gb = AssetDatabase.LoadAssetAtPath<GameObject>($"{strSavePath}.prefab");

            gb.GetComponent<RectTransform>().sizeDelta = bgSprite.textureRect.size;
            gb.GetComponent<Image>().sprite = bgSprite;
            GridLayoutGroup group = gb.transform.GetChild(0).GetChild(0).GetComponent<GridLayoutGroup>();
            group.padding = padding;
            group.cellSize = childSize;
            group.spacing = childSpace;
            PrefabUtility.SavePrefabAsset(gb);
        }
    }

    private class CreateCheckBox : CreateInfo
    {
        Sprite bgSprite;
        Sprite selectSprite;
        public override void Init()
        {
            name = "点选框";
            strTempletePath = $"{toolObject.uiTempletePath}/check_box";
        }

        public override void DrawParmter()
        {
            EditorGUILayout.LabelField("背景图片");
            bgSprite = (Sprite)EditorGUILayout.ObjectField(bgSprite, typeof(Sprite), false, GUILayout.ExpandHeight(true));
            EditorGUILayout.LabelField("选中状态图片");
            selectSprite = (Sprite)EditorGUILayout.ObjectField(selectSprite, typeof(Sprite), false, GUILayout.ExpandHeight(true));
        }

        protected override void ApplyParamter()
        {
            GameObject gb = AssetDatabase.LoadAssetAtPath<GameObject>($"{strSavePath}.prefab");

            gb.GetComponent<RectTransform>().sizeDelta = bgSprite.textureRect.size;
            gb.GetComponent<Image>().sprite = bgSprite;
            gb.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = selectSprite.textureRect.size;
            gb.transform.GetChild(0).GetComponent<Image>().sprite = selectSprite;
            PrefabUtility.SavePrefabAsset(gb);
        }
    }
}