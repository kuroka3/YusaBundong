using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class SettingsScript : MonoBehaviour
{
    public TMP_Dropdown resdropdown;
    List<Resolution> resolutions = new();
    public Toggle fullsToggle;

    public GameObject garimmak;
    private int KeySetting = -1;
    public TextMeshProUGUI[] keyTexts;

    public TextMeshProUGUI offsetValueText;
    public TextMeshProUGUI hispeedValueText;

    public static SettingsScript instance;

    void Start()
    {
        instance = this;
        InitUI();
    }

    void Update() {
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

    void InitUI() {
        Resolution[] tmpRes = Screen.resolutions;
        for (int i = tmpRes.Length-1; i>=0; i--) {
            resolutions.Add(tmpRes[i]);
        }
        resdropdown.options.Clear();

        foreach (Resolution item in resolutions) {
            TMP_Dropdown.OptionData option = new()
            {
                text = item.width + " x " + item.height + "  " + item.refreshRateRatio + "hz"
            };
            resdropdown.options.Add(option);
        }
        resdropdown.RefreshShownValue();
        resdropdown.value = SettingsManager.resNum;
        
        for (int i = 0; i < SettingsManager.keys.Length; i++) {
            keyTexts[i].text = SettingsManager.keys[i].ToString();
        }

        offsetValueText.text = SettingsManager.offset + "ms";
        hispeedValueText.text = (Math.Round(SettingsManager.hispeed*10)*0.1).ToString();

        fullsToggle.isOn = SettingsManager.fulls == FullScreenMode.ExclusiveFullScreen;
    }

    public void DropboxOptionChange(int i) {
        SettingsManager.resNum = i;
        Screen.SetResolution(resolutions[SettingsManager.resNum].width, resolutions[SettingsManager.resNum].height, SettingsManager.fulls, resolutions[SettingsManager.resNum].refreshRateRatio);
    }

    public void FullScreenBtn(bool isFullS) {
        SettingsManager.fulls = isFullS ? FullScreenMode.ExclusiveFullScreen : FullScreenMode.Windowed;
        Screen.SetResolution(resolutions[SettingsManager.resNum].width, resolutions[SettingsManager.resNum].height, SettingsManager.fulls, resolutions[SettingsManager.resNum].refreshRateRatio);
    }

    public void keySet(int keyLine) {
        if(KeySetting != -1) return;
        KeySetting = keyLine;
        garimmak.SetActive(true);
    }

    public void OffsetvalueChange(bool isUp) {
        int changeValue;

        if (Input.GetKey(KeyCode.LeftShift)) changeValue = 1;
        else if (Input.GetKey(KeyCode.LeftControl)) changeValue = 10;
        else changeValue = 5;

        if (isUp) SettingsManager.offset += changeValue;
        else SettingsManager.offset -= changeValue;

        offsetValueText.text = SettingsManager.offset + "ms";
    }

    public void HiSpeedvalueChange(bool isUp) {
        float changeValue;

        if (Input.GetKey(KeyCode.LeftShift)) changeValue = 0.1f;
        else if (Input.GetKey(KeyCode.LeftControl)) changeValue = 1f;
        else changeValue = 0.5f;

        if (isUp) SettingsManager.hispeed += changeValue;
        else SettingsManager.hispeed -= changeValue;

        hispeedValueText.text = (Math.Round(SettingsManager.hispeed*10)*0.1).ToString();
    }

    public void gotoHome() {
        SettingsManager.Save();
        SceneManager.LoadScene(1);
    }
}

public static class SettingsManager {
    public static FullScreenMode fulls;
    public static int resNum;

    public static KeyCode[] keys;

    public static int offset;
    public static float hispeed;

    private static string settingsFile = GameManager.appdata + "\\settings.cfg";
    private static Dictionary<string, object> values = new();


    public static void ResetSettings() {
        fulls = FullScreenMode.ExclusiveFullScreen;
        resNum = 0;

        keys = new KeyCode[4]{KeyCode.D, KeyCode.F, KeyCode.J, KeyCode.K};

        offset = 0;
        hispeed = 31.3f;
        Save();
    }

    public static void Save() {
        values["fulls"] = fulls == FullScreenMode.ExclusiveFullScreen;
        values["resNum"] = resNum;
        values["keys"] = keys;
        values["offset"] = offset;
        values["hispeed"] = hispeed;

        File.WriteAllText(settingsFile, JsonConvert.SerializeObject(values));
    }

    public static void Load() {
        if (!File.Exists(settingsFile)) ResetSettings();

        values = JsonConvert.DeserializeObject<Dictionary<string, object>>(File.ReadAllText(settingsFile));

        fulls = (bool) values["fulls"] ? FullScreenMode.ExclusiveFullScreen : FullScreenMode.Windowed;
        resNum = int.Parse(values["resNum"].ToString());

        keys = JsonConvert.DeserializeObject<KeyCode[]>(values["keys"].ToString());

        offset = int.Parse(values["offset"].ToString());
        hispeed = float.Parse(values["hispeed"].ToString());
    }
}