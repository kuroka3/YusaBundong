using System;
using System.Collections.Generic;
using UnityEngine;

public class LanguageScript : MonoBehaviour
{

    public static LanguageScript lang = null;
    private static IniObject langIni = null;

    void Awake() {
        if (lang != null) {
            DestroyImmediate(gameObject);
            return;
        }

        lang = this;
        DontDestroyOnLoad(gameObject);
    }

    public string this[string key] {
        get {
            return langIni[SettingsManager.langCodes[SettingsManager.lang], key];
        }
    }

    public static void Load() {
        TextAsset textAsset = Resources.Load<TextAsset>("lang");
        langIni = new IniObject(textAsset);
    }
}

public class IniObject {
    private Dictionary<string, Dictionary<string, string>> entries = new Dictionary<string, Dictionary<string, string>>();

    public IniObject(TextAsset iniFile) {

        Dictionary<string, string> currentSection = new Dictionary<string, string>();

        string[] lines = iniFile.text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        
        for (int i = 0; i<lines.Length; i++) {
            string line = lines[i];

            Debug.Log(line);

            if (!string.IsNullOrEmpty(line) && !line.StartsWith(";")) {
                if (line.StartsWith('\u005b') && line.EndsWith('\u005d')) {
                    string sectionName = line.Substring(1, line.Length - 2);

                    currentSection = new Dictionary<string, string>();
                    entries.Add(sectionName, currentSection);
                    Debug.Log(sectionName);
                } else {
                    string[] keyValue = line.Split('=');

                    if (keyValue.Length == 2) {
                        currentSection.Add(keyValue[0].Trim(), keyValue[1].Trim().Replace("\"", ""));
                    }
                }
            }
        }
    }

    public string this[string section, string key] {
        get {
            if (entries.ContainsKey(section)) {
                string value;
                if (entries[section].TryGetValue(key, out value)) {
                    return value;
                }
            }
            return "";
        }
    }
}