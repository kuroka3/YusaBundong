using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public enum KeySets
{
    Line1,
    Line2,
    Line3,
    Line4,
}

public class SettingsScript : MonoBehaviour
{
    public TMP_Dropdown resdropdown;
    public GameObject garimmak;
    List<Resolution> resolutions = new();
    private int resNum = 0;
    private FullScreenMode fulls = FullScreenMode.ExclusiveFullScreen;
    public static SettingsScript instance;
    private int KeySetting = -1;
    public TextMeshProUGUI[] keyTexts;

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
                    GameManager.keys[KeySetting] = kcode;
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
        resdropdown.value = resNum;
        
        for (int i = 0; i < GameManager.keys.Length; i++) {
            keyTexts[i].text = GameManager.keys[i].ToString();
        }
    }

    public void DropboxOptionChange(int i) {
        resNum = i;
        Screen.SetResolution(resolutions[resNum].width, resolutions[resNum].height, fulls, resolutions[resNum].refreshRateRatio);
    }

    public void FullScreenBtn(bool isFullS) {
        fulls = isFullS ? FullScreenMode.ExclusiveFullScreen : FullScreenMode.Windowed;
        Screen.SetResolution(resolutions[resNum].width, resolutions[resNum].height, fulls, resolutions[resNum].refreshRateRatio);
    }

    public void keySet(int keyLine) {
        if(KeySetting != -1) return;
        KeySetting = keyLine;
        garimmak.SetActive(true);
    }

    public void gotoHome() {
        File.WriteAllLines(GameManager.appdata + "\\settings.txt", new string[]{((int) GameManager.keys[0]).ToString(), ((int) GameManager.keys[1]).ToString(), ((int) GameManager.keys[2]).ToString(), ((int) GameManager.keys[3]).ToString(), GameManager.offset.ToString(), GameManager.offset.ToString()});
        SceneManager.LoadScene(2);
    }
}
