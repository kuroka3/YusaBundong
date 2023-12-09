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
            return langIni[SettingsManager.langCodes[SettingsManager.GetAsInt(Settings.Lang)], key];
        }
    }

    public static void Load() {
        TextAsset textAsset = Resources.Load<TextAsset>("lang");
        langIni = new IniObject(textAsset);
    }
}