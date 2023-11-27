using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class DTLocalization
{
    public static DTLocalization Instance;

    private Dictionary<string, string>[] arrModuleLocalization;

    public DTLocalization(ConfigPB.Table table, UnityEngine.SystemLanguage systemLanguage)
    {
        Instance = this;
        arrModuleLocalization = new Dictionary<string, string>[(int)ConfigPB.LocalizationModuleType.Max];
        for (int i = 0; i < arrModuleLocalization.Length; ++i)
        {
            arrModuleLocalization[i] = new Dictionary<string, string>();
        }
        foreach (var localization in table.Localization)
        {
            var dic = arrModuleLocalization[(int)localization.ModuleType];
            switch (systemLanguage)
            {
                case SystemLanguage.ChineseSimplified:
                    dic.Add(localization.Key, localization.ChineseSimplified);
                    break;
                case SystemLanguage.English:
                    dic.Add(localization.Key, localization.English);
                    break;
                case SystemLanguage.German:
                    dic.Add(localization.Key, localization.German);
                    break;
                case SystemLanguage.French:
                    dic.Add(localization.Key, localization.French);
                    break;
                case SystemLanguage.Italian:
                    dic.Add(localization.Key, localization.Italian);
                    break;
                case SystemLanguage.ChineseTraditional:
                    dic.Add(localization.Key, localization.ChineseTraditional);
                    break;
                case SystemLanguage.Korean:
                    dic.Add(localization.Key, localization.Korean);
                    break;
                case SystemLanguage.Japanese:
                    dic.Add(localization.Key, localization.Japanese);
                    break;
                case SystemLanguage.Dutch:
                    dic.Add(localization.Key, localization.Dutch);
                    break;
                case SystemLanguage.Spanish:
                    dic.Add(localization.Key, localization.Spanish);
                    break;
                case SystemLanguage.Portuguese:
                    dic.Add(localization.Key, localization.Portuguese);
                    break;
                case SystemLanguage.Polish:
                    dic.Add(localization.Key, localization.Polish);
                    break;
                case SystemLanguage.Ukrainian:
                    dic.Add(localization.Key, localization.Ukrainian);
                    break;
                case SystemLanguage.Russian:
                    dic.Add(localization.Key, localization.Russian);
                    break;
                case SystemLanguage.Thai:
                    dic.Add(localization.Key, localization.Thai);
                    break;
                case SystemLanguage.Vietnamese:
                    dic.Add(localization.Key, localization.Vietnamese);
                    break;
                case SystemLanguage.Indonesian:
                    dic.Add(localization.Key, localization.Indonesian);
                    break;
            }

#if !TEST
            if (string.IsNullOrEmpty(dic[localization.Key]))
            {
                dic[localization.Key] = localization.English;
            }
#endif
        }
    }

    public string GetString(ConfigPB.LocalizationModuleType localizationModuleType, string key)
    {
        var dic = arrModuleLocalization[(int)localizationModuleType];
        string v;
        dic.TryGetValue(key, out v);
        if (string.IsNullOrEmpty(v))
        {
#if UNITY_EDITOR
            return $"Not Config {localizationModuleType}:{key}";
#else
            return "";
#endif
        }
        return v;
    }

    public string GetStringFormat(ConfigPB.LocalizationModuleType localizationModuleType, string key, params object[] args)
    {
        var dic = arrModuleLocalization[(int)localizationModuleType];
        string v;
        dic.TryGetValue(key, out v);
        if (string.IsNullOrEmpty(v))
        {
            return $"Not Config {localizationModuleType}:{key}";
        }
        return string.Format(v, args);
    }
    public string GetStringFormat(ConfigPB.LocalizationModuleType localizationModuleType, string key, object arg0, object arg1, object arg2)
    {
        var dic = arrModuleLocalization[(int)localizationModuleType];
        string v;
        dic.TryGetValue(key, out v);
        if (string.IsNullOrEmpty(v))
        {
            return $"Not Config {localizationModuleType}:{key}";
        }
        return string.Format(v, arg0, arg1, arg2);
    }
    public string GetStringFormat(ConfigPB.LocalizationModuleType localizationModuleType, string key, object arg0, object arg1)
    {
        var dic = arrModuleLocalization[(int)localizationModuleType];
        string v;
        dic.TryGetValue(key, out v);
        if (string.IsNullOrEmpty(v))
        {
            return $"Not Config {localizationModuleType}:{key}";
        }
        return string.Format(v, arg0, arg1);
    }
    public string GetStringFormat(ConfigPB.LocalizationModuleType localizationModuleType, string key, object arg0)
    {
        var dic = arrModuleLocalization[(int)localizationModuleType];
        string v;
        dic.TryGetValue(key, out v);
        if (string.IsNullOrEmpty(v))
        {
            return $"Not Config {localizationModuleType}:{key}";
        }
        return string.Format(v, arg0);
    }

    public Dictionary<string, string> GetList_ByModule(ConfigPB.LocalizationModuleType localizationModuleType)
    {
        return arrModuleLocalization[(int)localizationModuleType];
    }

}
