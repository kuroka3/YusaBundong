using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SongListScript : MonoBehaviour
{
    public TextMeshProUGUI[] values;
    public Image Jacket;
    public GameObject Prefab;
    public GameObject parent;
    public GameObject scrollView;

    private List<SongListElementScript> Elements = new();
    private ScrollRect scrollRect;

    public static SongListScript instance;

    void Start() {
        instance = this;
        scrollRect = scrollView.GetComponent<ScrollRect>();

        for (int i = 0; i < GameManager.Beatmaps.Length; i++) {
            YusaBundongBeatmap beatmap = GameManager.Beatmaps[i];
            GameObject obj = Instantiate(Prefab, parent.transform);
            SongListElementScript elementScript = obj.GetComponent<SongListElementScript>();
            elementScript.SendData(i);
            Elements.Add(elementScript);

            TextMeshProUGUI title = obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI artist = obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>();

            if (SettingsManager.Get<bool>(global::Settings.ShowAsUnicode)) {
                title.text = beatmap.metaData["Data", "Title_u"];
                artist.text = beatmap.metaData["Data", "Artist_u"];
            } else {
                title.text = beatmap.metaData["Data", "Title"];
                artist.text = beatmap.metaData["Data", "Artist"];
            }
        }

        RefreshData();
    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.Space)) {
            StartGame();
        }
    }

    public void Select(int code) {
        GameManager.songCode = code;
        GameManager.DifficultyCode = 0;

        scrollRect.verticalNormalizedPosition = 1 - GameManager.songCode.ToFloat()/(Elements.ToArray().Length-1).ToFloat();

        RefreshData();
    }

    public void Reload() {
        SceneManager.LoadScene(1);
    }

    private void StartGame() {
        SceneManager.LoadScene(3);
    }

    public void Settings() {
        SceneManager.LoadScene(5);
    }

    public void Exit() {
        Application.Quit(0);
    }
    
    private void RefreshData() {
        YusaBundongBeatmap SelectedBeatmap = GameManager.Beatmaps[GameManager.songCode];

        if (SelectedBeatmap == null) return;

        Dictionary<string, string> Data = SelectedBeatmap.metaData["Data"];

        if (SettingsManager.Get<bool>(global::Settings.ShowAsUnicode)) {
            values[0].text = Data["Title_u"];
            values[1].text = Data["Artist_u"];
        } else {
            values[0].text = Data["Title"];
            values[1].text = Data["Artist"];
        }

        if (Data["FromBPM"] == Data["ToBPM"]) {
            values[2].text = Data["FromBPM"] + "BPM";
        } else {
            values[2].text = string.Format("{0}-{1}BPM", Data["FromBPM"], Data["ToBPM"]);
        }

        if (SelectedBeatmap.Jacket != null) {
            Jacket.sprite = SelectedBeatmap.Jacket;
        }

        Color color = new(0f, 0f, 0f, 0f);

        foreach (SongListElementScript song in Elements) {
            song.myImage.color = color;
        }

        Elements[GameManager.songCode].myImage.color = color.setA(0.7f);

        RefreshDifficulty();
    }

    public void RefreshDifficulty() {
        TextMeshProUGUI value = values[3];
        value.text = GameManager.Beatmaps[GameManager.songCode].yusaBundongFiles[GameManager.DifficultyCode].version;
    }

    public void ChangeDifficulty(bool isUp) {
        if (isUp) {
            if (GameManager.DifficultyCode == GameManager.Beatmaps[GameManager.songCode].yusaBundongFiles.Length-1) {
                GameManager.DifficultyCode = 0;
            } else {
                GameManager.DifficultyCode++;
            }
        } else {
            if (GameManager.DifficultyCode == 0) {
                GameManager.DifficultyCode = GameManager.Beatmaps[GameManager.songCode].yusaBundongFiles.Length-1;
            } else {
                GameManager.DifficultyCode--;
            }
        }

        RefreshDifficulty();
    }
}
