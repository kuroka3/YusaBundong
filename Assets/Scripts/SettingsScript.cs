using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


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
        GraphicInit();
        KeySettingsInit();
        AudioInit();
        GamePlayInit();
    }

    void Update() {
        KeySettingsUpdate();
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

        value.text = ToResolutionText(resolutions[SettingsManager.resNum]);
    }

    public void ResolutionChange(bool isUp) {
        if(isUp && SettingsManager.resNum != 0) {
            SettingsManager.resNum--;
            Screen.SetResolution(resolutions[SettingsManager.resNum].width, resolutions[SettingsManager.resNum].height, SettingsManager.fulls);
        } else if (!isUp && SettingsManager.resNum != resolutions.Length - 1) {
            SettingsManager.resNum++;
            Screen.SetResolution(resolutions[SettingsManager.resNum].width, resolutions[SettingsManager.resNum].height, SettingsManager.fulls);
        }

        values[0].text = ToResolutionText(resolutions[SettingsManager.resNum]);
    }

    // ================ Full Screen ================

    void FullScreenInit() {
        TextMeshProUGUI value = values[1];

        if(SettingsManager.fulls == FullScreenMode.ExclusiveFullScreen) value.text = "On";
        else value.text = "Off";
    }

    public void FullScreenBtn() {
        if (SettingsManager.fulls == FullScreenMode.ExclusiveFullScreen) SettingsManager.fulls = FullScreenMode.Windowed;
        else SettingsManager.fulls = FullScreenMode.ExclusiveFullScreen;
        Screen.SetResolution(resolutions[SettingsManager.resNum].width, resolutions[SettingsManager.resNum].height, SettingsManager.fulls);
        FullScreenInit();
    }

    // ================ VSync ================

    void VSyncInit() {
        TextMeshProUGUI value = values[2];

        if(SettingsManager.vsync) value.text = "On";
        else value.text = "Off";
    }

    public void VSyncBtn() {
        if (SettingsManager.vsync) {
            SettingsManager.vsync = false;
            QualitySettings.vSyncCount = 0;
        }
        else {
            SettingsManager.vsync = true;
            QualitySettings.vSyncCount = 1;
        }
        VSyncInit();
    }

    // ================ FrameRateLimit ================

    void FrameRateLimitInit() {
        TextMeshProUGUI value = values[3];
        int Frame = SettingsManager.targetFrames[SettingsManager.targetFrame];
        if (Frame == -1) {
            value.text = "NoLimit";
        } else {
            value.text = Frame.ToString();
        }
    }

    public void FrameLimitChange(bool isUp) {
        if(isUp && SettingsManager.targetFrame != SettingsManager.targetFrames.Length - 1) {
            SettingsManager.targetFrame++;
            Application.targetFrameRate = SettingsManager.targetFrames[SettingsManager.targetFrame];
        } else if (!isUp && SettingsManager.targetFrame != 0) {
            SettingsManager.targetFrame--;
            Application.targetFrameRate = SettingsManager.targetFrames[SettingsManager.targetFrame];
        }

        FrameRateLimitInit();
    }

    private string ToResolutionText(Resolution res) {
        return res.width + " x " + res.height;
    }

    // ======================================= Key Settings =======================================

    void KeySettingsInit() {
        for (int i = 0; i < SettingsManager.keys.Length; i++) {
            keyTexts[i].text = SettingsManager.keys[i].ToString();
        }
    }

    void KeySettingsUpdate() {
        if(KeySetting == -1) return;

        if(Input.anyKeyDown) {
            foreach (KeyCode kcode in System.Enum.GetValues(typeof(KeyCode))) {
                if(Input.GetKeyDown(kcode)) {
                    SettingsManager.keys[KeySetting] = kcode;
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

        value.text = SettingsManager.volume.ToString();
    }

    public void VolumeChange(bool isUp) {
        int changeValue;

        if (Input.GetKey(KeyCode.LeftShift)) changeValue = 1;
        else if (Input.GetKey(KeyCode.LeftControl)) changeValue = 10;
        else changeValue = 5;

        

        if (isUp) {
            if (SettingsManager.volume + changeValue > 100) {
                SettingsManager.volume = 100;
            } else {
                SettingsManager.volume += changeValue;
            }
        }
        else {
            if (SettingsManager.volume - changeValue < 0) {
                SettingsManager.volume = 0;
            } else {
                SettingsManager.volume -= changeValue;
            }
        }

        VolumeInit();
    }

    // ================ Offset ================

    void OffsetInit() {
        TextMeshProUGUI value = values[5];

        value.text = SettingsManager.offset + "ms";
    }

    public void OffsetvalueChange(bool isUp) {
        int changeValue;

        if (Input.GetKey(KeyCode.LeftShift)) changeValue = 1;
        else if (Input.GetKey(KeyCode.LeftControl)) changeValue = 10;
        else changeValue = 5;

        if (isUp) SettingsManager.offset += changeValue;
        else SettingsManager.offset -= changeValue;

        OffsetInit();
    }

    // ======================================= GamePlay Settings =======================================

    void GamePlayInit() {
        HiSpeedInit();
        KeyBeamInit();
    }

    // ================ Hi-Speed ================

    void HiSpeedInit() {
        TextMeshProUGUI value = values[6];

        value.text = (Math.Round(SettingsManager.hispeed*10)*0.1).ToString();
    }

    public void HiSpeedvalueChange(bool isUp) {
        float changeValue;

        if (Input.GetKey(KeyCode.LeftShift)) changeValue = 0.1f;
        else if (Input.GetKey(KeyCode.LeftControl)) changeValue = 1f;
        else changeValue = 0.5f;

        if (isUp) SettingsManager.hispeed += changeValue;
        else SettingsManager.hispeed -= changeValue;

        HiSpeedInit();
    }

    // ================ KeyBeam ================

    void KeyBeamInit() {
        TextMeshProUGUI value = values[7];

        if(SettingsManager.keybeamtoggle) value.text = "On";
        else value.text = "Off";
    }

    public void KeyBeamToggleBtn() {
        if (SettingsManager.keybeamtoggle) {
            SettingsManager.keybeamtoggle = false;
        }
        else {
            SettingsManager.keybeamtoggle = true;
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



public static class SettingsManager {
    public static int[] targetFrames = new int[]{24, 30, 60, 120, 144, 240, 300, -1};

    public static FullScreenMode fulls;
    public static int resNum;
    public static bool vsync;
    public static int targetFrame;

    public static KeyCode[] keys;

    public static int volume;
    public static int offset;
    public static float hispeed;
    public static bool keybeamtoggle;

    private static string settingsFile = GameManager.appdata + "\\settings.cfg";

    private static Dictionary<string, object> values = new();

    public static void ResetSettings() {
        fulls = FullScreenMode.ExclusiveFullScreen;
        resNum = 0;
        vsync = false;
        targetFrame = 7;

        keys = new KeyCode[4]{KeyCode.D, KeyCode.F, KeyCode.J, KeyCode.K};

        volume = 100;
        offset = 0;
        hispeed = 31.3f;
        keybeamtoggle = true;
        Save();
    }

    public static void Save() {
        values["fulls"] = fulls == FullScreenMode.ExclusiveFullScreen;
        values["resNum"] = resNum;
        values["vsync"] = vsync;
        values["targetFrame"] = targetFrame;

        values["keys"] = keys;

        values["volume"] = volume;
        values["offset"] = offset;
        values["hispeed"] = hispeed;
        values["keybeamtoggle"] = keybeamtoggle;

        File.WriteAllText(settingsFile, JsonConvert.SerializeObject(values));
    }

    public static void Load() {
        if (!File.Exists(settingsFile)) ResetSettings();

        values = JsonConvert.DeserializeObject<Dictionary<string, object>>(File.ReadAllText(settingsFile)) ?? new();

        fulls = (bool) (GetorNull("fulls") ?? true) ? FullScreenMode.ExclusiveFullScreen : FullScreenMode.Windowed;
        resNum = (GetorNull("resNum") ?? 0).ToInt();
        vsync = (bool) (GetorNull("vsync") ?? false);
        targetFrame = (GetorNull("targetFrame") ?? 7).ToInt();

        keys = JsonConvert.DeserializeObject<KeyCode[]>((values["keys"] ?? JsonConvert.SerializeObject(new KeyCode[4]{KeyCode.D, KeyCode.F, KeyCode.J, KeyCode.K})).ToString());

        volume = (GetorNull("volume") ?? 100).ToInt();
        offset = (GetorNull("offset") ?? 0).ToInt();
        hispeed = (GetorNull("hispeed") ?? 31.3f).ToFloat();
        keybeamtoggle = (bool) (GetorNull("keybeamtoggle") ?? true);



        if(vsync) QualitySettings.vSyncCount = 1;
        else QualitySettings.vSyncCount = 0;

        Application.targetFrameRate = targetFrames[targetFrame];

        GameManager.OffsetFloat = offset.ToFloat()*0.001f;
    }

    private static object GetorNull(string key) {
        if(values.ContainsKey(key)) return values[key];
        else return null;
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
}