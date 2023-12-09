using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SettingsScript : MonoBehaviour
{
    public TextMeshProUGUI[] values;

    private Resolution[] resolutions;

    public GameObject garimmak;
    private int KeySetting = -1;
    public TextMeshProUGUI[] keyTexts;

    public static SettingsScript instance;

    void Start()
    {
        instance = this;

        GeneralInit();
        GraphicInit();
        KeySettingsInit();
        AudioInit();
        PerformanceInit();
        GamePlayInit();
    }

    void Update() {
        KeySettingsUpdate();
    }

    // ======================================= General Settings =======================================

    void GeneralInit() {
        LanguageInit();
    }

    // ================ Language ================

    void LanguageInit() {
        TextMeshProUGUI value = values[8];
        value.text = LanguageScript.lang["name"];
    }

    public void LanguageChange(bool isUp) {
        if (isUp) {
            if (SettingsManager.GetAsInt(Settings.Lang) != SettingsManager.langCodes.Length-1) SettingsManager.AddInt(Settings.Lang, 1);
            else SettingsManager.Set(Settings.Lang, 0);
        } else {
            if (SettingsManager.GetAsInt(Settings.Lang) != 0) SettingsManager.AddInt(Settings.Lang, -1);
            else SettingsManager.Set(Settings.Lang, SettingsManager.langCodes.Length-1);
        }

        SettingsManager.Save();
        SceneManager.LoadScene(5);
    }

    // ======================================= Graphic Settings =======================================

    void GraphicInit() {
        ResolutionInit();
        FullScreenInit();
        VSyncInit();
        FrameRateLimitInit();
    }

    // ================ Resolution ================

    void ResolutionInit() {
        TextMeshProUGUI value = values[0];

        Resolution[] tmpRes = Screen.resolutions;
        List<Resolution> tmp = new();
        for (int i = tmpRes.Length-1; i>=0; i--) {
            if(tmpRes[i].refreshRateRatio.Equals(Screen.currentResolution.refreshRateRatio)) {
                tmp.Add(tmpRes[i]);
            }
        }

        resolutions = tmp.ToArray();

        value.text = ToResolutionText(resolutions[SettingsManager.GetAsInt(Settings.Resolution)]);
    }

    public void ResolutionChange(bool isUp) {
        int i = SettingsManager.GetAsInt(Settings.Resolution);

        if(isUp && i != 0) {
            SettingsManager.AddInt(Settings.Resolution, -1);
            i--;

            Screen.SetResolution(resolutions[i].width, resolutions[i].height, SettingsManager.Get<bool>(Settings.FullScreen));
        } else if (!isUp && i != resolutions.Length - 1) {
            SettingsManager.AddInt(Settings.Resolution, 1);
            i++;

            Screen.SetResolution(resolutions[i].width, resolutions[i].height, SettingsManager.Get<bool>(Settings.FullScreen));
        }

        values[0].text = ToResolutionText(resolutions[i]);
    }

    // ================ Full Screen ================

    void FullScreenInit() {
        TextMeshProUGUI value = values[1];

        if(SettingsManager.Get<bool>(Settings.FullScreen)) value.text = LanguageScript.lang["on"];
        else value.text = LanguageScript.lang["off"];
    }

    public void FullScreenBtn() {
        bool v = SettingsManager.Get<bool>(Settings.FullScreen);

        if (v) SettingsManager.Set(Settings.FullScreen, false);
        else SettingsManager.Set(Settings.FullScreen, true);
        Screen.SetResolution(resolutions[SettingsManager.GetAsInt(Settings.Resolution)].width, resolutions[SettingsManager.GetAsInt(Settings.Resolution)].height, SettingsManager.Get<bool>(Settings.FullScreen));
        FullScreenInit();
    }

    // ================ VSync ================

    void VSyncInit() {
        TextMeshProUGUI value = values[2];

        if(SettingsManager.Get<bool>(Settings.VSync)) value.text = LanguageScript.lang["on"];
        else value.text = LanguageScript.lang["off"];
    }

    public void VSyncBtn() {
        if (SettingsManager.Get<bool>(Settings.VSync)) {
            SettingsManager.Set(Settings.VSync, false);
            QualitySettings.vSyncCount = 0;
        }
        else {
            SettingsManager.Set(Settings.VSync, true);
            QualitySettings.vSyncCount = 1;
        }
        VSyncInit();
    }

    // ================ FrameRateLimit ================

    void FrameRateLimitInit() {
        TextMeshProUGUI value = values[3];
        int Frame = SettingsManager.targetFrames[SettingsManager.GetAsInt(Settings.TargetFrame)];
        if (Frame == -1) {
            value.text = "NoLimit";
        } else {
            value.text = Frame.ToString();
        }
    }

    public void FrameLimitChange(bool isUp) {
        if(isUp && SettingsManager.GetAsInt(Settings.TargetFrame) != SettingsManager.targetFrames.Length - 1) {
            SettingsManager.AddInt(Settings.TargetFrame, 1);
            Application.targetFrameRate = SettingsManager.targetFrames[SettingsManager.GetAsInt(Settings.TargetFrame)];
        } else if (!isUp && SettingsManager.GetAsInt(Settings.TargetFrame) != 0) {
            SettingsManager.AddInt(Settings.TargetFrame, -1);
            Application.targetFrameRate = SettingsManager.targetFrames[SettingsManager.GetAsInt(Settings.TargetFrame)];
        }

        FrameRateLimitInit();
    }

    private string ToResolutionText(Resolution res) {
        return res.width + " x " + res.height;
    }

    // ======================================= Key Settings =======================================

    void KeySettingsInit() {
        KeyCode[] value = JsonConvert.DeserializeObject<KeyCode[]>(SettingsManager.Get<object>(Settings.Keys).ToString());

        for (int i = 0; i < value.Length; i++) {
            keyTexts[i].text = value[i].ToString();
        }
    }

    void KeySettingsUpdate() {
        if(KeySetting == -1) return;

        if(Input.anyKeyDown) {
            foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode))) {
                if(Input.GetKeyDown(kcode)) {
                    KeyCode[] temp = JsonConvert.DeserializeObject<KeyCode[]>(SettingsManager.Get<object>(Settings.Keys).ToString());
                    temp[KeySetting] = kcode;
                    SettingsManager.Set(Settings.Keys, JsonConvert.SerializeObject(temp));
                    keyTexts[KeySetting].text = kcode.ToString();
                    KeySetting = -1;
                    garimmak.SetActive(false);
                }
            }
        }
    }

    public void keySet(int keyLine) {
        if(KeySetting != -1) return;
        KeySetting = keyLine;
        garimmak.SetActive(true);
    }

    // ======================================= Audio Settings =======================================

    void AudioInit() {
        VolumeInit();
        OffsetInit();
    }

    // ================ Volume ================

    void VolumeInit() {
        TextMeshProUGUI value = values[4];

        value.text = SettingsManager.GetAsInt(Settings.Volume).ToString();
    }

    public void VolumeChange(bool isUp) {
        int changeValue;

        if (Input.GetKey(KeyCode.LeftShift)) changeValue = 1;
        else if (Input.GetKey(KeyCode.LeftControl)) changeValue = 10;
        else changeValue = 5;

        

        if (isUp) {
            if (SettingsManager.GetAsInt(Settings.Volume) + changeValue > 100) {
                SettingsManager.Set(Settings.Volume, 100);
            } else {
                SettingsManager.AddInt(Settings.Volume, changeValue);
            }
        }
        else {
            if (SettingsManager.GetAsInt(Settings.Volume) - changeValue < 0) {
                SettingsManager.Set(Settings.Volume, 0);
            } else {
                SettingsManager.AddInt(Settings.Volume, -changeValue);
            }
        }

        VolumeInit();
    }

    // ================ Offset ================

    void OffsetInit() {
        TextMeshProUGUI value = values[5];

        value.text = SettingsManager.GetAsInt(Settings.Offset) + "ms";
    }

    public void OffsetvalueChange(bool isUp) {
        int changeValue;

        if (Input.GetKey(KeyCode.LeftShift)) changeValue = 1;
        else if (Input.GetKey(KeyCode.LeftControl)) changeValue = 10;
        else changeValue = 5;

        if (isUp) SettingsManager.AddInt(Settings.Offset, changeValue);
        else SettingsManager.AddInt(Settings.Offset, -changeValue);

        OffsetInit();
    }

    // ======================================= Performance Settings =======================================

    void PerformanceInit() {
        MaxObjectInit();
    }

    // ================ MaxObject ================

    void MaxObjectInit() {
        TextMeshProUGUI value = values[9];

        value.text = SettingsManager.GetAsInt(Settings.MaxObject).ToString();
    }

    public void MaxObjectvalueChange(bool isUp) {
        int changeValue;

        if (Input.GetKey(KeyCode.LeftShift)) changeValue = 1;
        else if (Input.GetKey(KeyCode.LeftControl)) changeValue = 100;
        else changeValue = 10;

        if (isUp) {
            if (SettingsManager.GetAsInt(Settings.MaxObject) + changeValue > 1000) {
                SettingsManager.Set(Settings.MaxObject, 1000);
            } else {
                SettingsManager.AddInt(Settings.MaxObject, changeValue);
            }
        }
        else {
            if (SettingsManager.GetAsInt(Settings.MaxObject) - changeValue < 10) {
                SettingsManager.Set(Settings.MaxObject, 10);
            } else {
                SettingsManager.AddInt(Settings.MaxObject, -changeValue);
            }
        }

        MaxObjectInit();
    }

    // ======================================= GamePlay Settings =======================================

    void GamePlayInit() {
        HiSpeedInit();
        KeyBeamInit();
    }

    // ================ Hi-Speed ================

    void HiSpeedInit() {
        TextMeshProUGUI value = values[6];

        value.text = (Math.Round(SettingsManager.GetAsFloat(Settings.HiSpeed)*10)*0.1).ToString();
    }

    public void HiSpeedvalueChange(bool isUp) {
        float changeValue;

        if (Input.GetKey(KeyCode.LeftShift)) changeValue = 0.1f;
        else if (Input.GetKey(KeyCode.LeftControl)) changeValue = 1f;
        else changeValue = 0.5f;

        if (isUp) SettingsManager.AddFloat(Settings.HiSpeed, changeValue);
        else SettingsManager.AddFloat(Settings.HiSpeed, -changeValue);

        HiSpeedInit();
    }

    // ================ KeyBeam ================

    void KeyBeamInit() {
        TextMeshProUGUI value = values[7];

        if(SettingsManager.Get<bool>(Settings.KeyBeam)) value.text = LanguageScript.lang["on"];
        else value.text = LanguageScript.lang["off"];
    }

    public void KeyBeamToggleBtn() {
        if (SettingsManager.Get<bool>(Settings.KeyBeam)) {
            SettingsManager.Set(Settings.KeyBeam, false);
        }
        else {
            SettingsManager.Set(Settings.KeyBeam, true);
        }
        KeyBeamInit();
    }

    // ======================================= ETC =======================================

    public void gotoHome() {
        SettingsManager.Save();
        SceneManager.LoadScene(1);
    }

    public void import() {
        SettingsManager.Save();
        SceneManager.LoadScene(6);
    }
}


public enum Settings {
        Lang,
        ShowAsUnicode,
        
        FullScreen,
        Resolution,
        VSync,
        TargetFrame,

        Keys,

        Volume,
        Offset,

        MaxObject,

        HiSpeed,
        KeyBeam
    }

public static class SettingsManager {

    private static Dictionary<Settings, object> SettingsDictionary = new();
    private static Dictionary<Settings, object> DefaultValue = new() {
        [Settings.Lang] = 0,
        [Settings.ShowAsUnicode] = true,

        [Settings.FullScreen] = true,
        [Settings.Resolution] = 0,
        [Settings.VSync] = false,
        [Settings.TargetFrame] = 7,

        [Settings.Keys] = new KeyCode[4]{KeyCode.D, KeyCode.F, KeyCode.J, KeyCode.K},

        [Settings.Volume] = 100,
        [Settings.Offset] = 0,

        [Settings.MaxObject] = 50,

        [Settings.HiSpeed] = 31.3f,
        [Settings.KeyBeam] = true,
    };

    public static int[] targetFrames = new int[]{24, 30, 60, 120, 144, 240, 300, -1};
    public static string[] langCodes = new string[]{"en-us", "ko-kr", "ja-jp", "zh-CN"};

    // public static int lang;

    // public static FullScreenMode fulls;
    // public static int resNum;
    // public static bool vsync;
    // public static int targetFrame;

    // public static KeyCode[] keys;

    // public static int volume;
    // public static int offset;

    // public static int maxObject;

    // public static float hispeed;
    // public static bool keybeamtoggle;

    private static string settingsFile = GameManager.appdata + "\\settings.cfg";

    // private static Dictionary<string, object> values = new();

    public static void ResetSettings() {
        // lang = 0;

        // fulls = FullScreenMode.ExclusiveFullScreen;
        // resNum = 0;
        // vsync = false;
        // targetFrame = 7;

        // keys = new KeyCode[4]{KeyCode.D, KeyCode.F, KeyCode.J, KeyCode.K};

        // volume = 100;
        // offset = 0;

        // maxObject = 50;

        // hispeed = 31.3f;
        // keybeamtoggle = true;
        // Save();

        SettingsDictionary = DefaultValue;

        Save();
    }

    public static void Save() {
        // values["lang"] = lang;

        // values["fulls"] = fulls == FullScreenMode.ExclusiveFullScreen;
        // values["resNum"] = resNum;
        // values["vsync"] = vsync;
        // values["targetFrame"] = targetFrame;

        // values["keys"] = keys;

        // values["volume"] = volume;
        // values["offset"] = offset;

        // values["maxObject"] = maxObject;

        // values["hispeed"] = hispeed;
        // values["keybeamtoggle"] = keybeamtoggle;

        // File.WriteAllText(settingsFile, JsonConvert.SerializeObject(values));

        File.WriteAllText(settingsFile, JsonConvert.SerializeObject(SettingsDictionary));
    }

    public static void Load() {
        try {
            if (!File.Exists(settingsFile)) ResetSettings();

            SettingsDictionary = JsonConvert.DeserializeObject<Dictionary<Settings, object>>(File.ReadAllText(settingsFile));

            // lang = (GetorNull("lang") ?? 0).ToInt();

            // fulls = (bool) (GetorNull("fulls") ?? true) ? FullScreenMode.ExclusiveFullScreen : FullScreenMode.Windowed;
            // resNum = (GetorNull("resNum") ?? 0).ToInt();
            // vsync = (bool) (GetorNull("vsync") ?? false);
            // targetFrame = (GetorNull("targetFrame") ?? 7).ToInt();

            // keys = JsonConvert.DeserializeObject<KeyCode[]>((values["keys"] ?? JsonConvert.SerializeObject(new KeyCode[4]{KeyCode.D, KeyCode.F, KeyCode.J, KeyCode.K})).ToString());

            // volume = (GetorNull("volume") ?? 100).ToInt();
            // offset = (GetorNull("offset") ?? 0).ToInt();

            // maxObject = (GetorNull("maxObject") ?? 50).ToInt();

            // hispeed = (GetorNull("hispeed") ?? 31.3f).ToFloat();
            // keybeamtoggle = (bool) (GetorNull("keybeamtoggle") ?? true);



            if(SettingsDictionary[Settings.VSync].As<bool>()) QualitySettings.vSyncCount = 1;
            else QualitySettings.vSyncCount = 0;

            Application.targetFrameRate = targetFrames[GetAsInt(Settings.TargetFrame)];

            GameManager.OffsetFloat = GetAsInt(Settings.Offset).ToFloat()*0.001f;
        } catch (Exception e) {
            Debug.LogError(e);
            ResetSettings();
            Load();
        }
    }

    public static int GetAsInt(Settings key) {
        unchecked { 
            return (int) SettingsDictionary[key].ToLong();
        }
    }

    public static float GetAsFloat(Settings key) {
        unchecked {
            return (float) SettingsDictionary[key].ToDouble();
        }
    }

    public static T Get<T>(Settings key) {
        try {
            return SettingsDictionary[key].As<T>();
        } catch (Exception e) {
            Debug.LogError(e);
            return default;
        }
    }

    public static void Set(Settings key, object value) {
        SettingsDictionary[key] = value;
    }

    public static void AddFloat(Settings key, float value) {
        Set(key, GetAsFloat(key) + value);
    }

    public static void AddInt(Settings key, int value) {
        Set(key, GetAsInt(key) + value);
    }
}

public static class ObjectExtention {
    public static int ToInt(this object o, bool ThrowException = false) {
        try {
            return int.Parse(o.ToString());
        } catch (Exception e) {
            if(ThrowException) {
                throw e;
            } else {
                Debug.LogError(e);
                return default;
            }
        }
    }
    
    public static long ToLong(this object o, bool ThrowException = false) {
        try {
            return long.Parse(o.ToString());
        } catch (Exception e) {
            if(ThrowException) {
                throw e;
            } else {
                Debug.LogError(e);
                return default;
            }
        }
    }

    public static float ToFloat(this object o, bool ThrowException = false) {
        try {
            return float.Parse(o.ToString());
        } catch (Exception e) {
            if(ThrowException) {
                throw e;
            } else {
                Debug.LogError(e);
                return default;
            }
        }
    }

    public static double ToDouble(this object o, bool ThrowException = false) {
        try {
            return double.Parse(o.ToString());
        } catch (Exception e) {
            if(ThrowException) {
                throw e;
            } else {
                Debug.LogError(e);
                return default;
            }
        }
    }

    public static T As<T>(this object o) {
        if (o is T t) {
            return t;
        } else {
            return default;
        }
    }
}