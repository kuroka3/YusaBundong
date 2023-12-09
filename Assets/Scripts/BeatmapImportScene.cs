using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Windows.Forms;
using Ookii.Dialogs;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BeatmapImportScene : MonoBehaviour
{

    public GameObject afterSelect;
    public TextMeshProUGUI[] values;
    public Image Jacket;
    public TextMeshProUGUI Prefab;
    public GameObject content;

    private OsuBeatmap SelectedBeatmap = null;
    private string SelectedPath = null;
    private string JacketPath = "null";
    
    private readonly List<TextMeshProUGUI> Difficulties = new();
    private bool IsTempFolder = false;

    void Start() {
        afterSelect.SetActive(false);
    }

    public void SelectFolder() {
        VistaFolderBrowserDialog dialog = new() {
            RootFolder = Environment.SpecialFolder.LocalApplicationData
        };

        if (dialog.ShowDialog() == DialogResult.OK) {
            SelectedPath = dialog.SelectedPath;
            SelectedBeatmap = new OsuBeatmap(dialog.SelectedPath);

            afterSelect.SetActive(true);
            LoadValues();

            IsTempFolder = false;
        }
    }

    public void SelectOsz() {
        VistaOpenFileDialog dialog = new() {
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\osu!",
            Filter = ".osz file (*.osz)|*.osz",
            FilterIndex = 0,
            Title = "Select osz file"
        };

        if (dialog.ShowDialog() == DialogResult.OK) {
            if (dialog.CheckFileExists) {
                string osz = dialog.FileName;
                string dir = GameManager.tmpFolder + "\\" + Guid.NewGuid().ToString();
                ZipFile.ExtractToDirectory(osz, dir);

                SelectedPath = dir;
                SelectedBeatmap = new OsuBeatmap(dir);

                afterSelect.SetActive(true);
                LoadValues();

                IsTempFolder = true;
            }
        }
    }

    public void SelectJacket() {
        VistaOpenFileDialog dialog = new() {
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\osu!\\Songs\\",
            Filter = "PNG file (*.png)|*.png|JPG file (*.jpg)|*.jpg|JPEG file (*.jpeg)|*.jpeg",
            FilterIndex = 0,
            Title = "Select Jacket file"
        };

        if (dialog.ShowDialog() == DialogResult.OK) {
            if (dialog.CheckFileExists) {
                JacketPath = dialog.FileName;
                LoadValues();
            }
        }
    }

    private void LoadValues() {
        if (SelectedBeatmap == null) return;

        if (SettingsManager.Get<bool>(Settings.ShowAsUnicode)) {
            values[0].text = SelectedBeatmap.Title_u;
            values[1].text = SelectedBeatmap.Artist_u;
        } else {
            values[0].text = SelectedBeatmap.Title;
            values[1].text = SelectedBeatmap.Artist;
        }

        if (SelectedBeatmap.FromBPM == SelectedBeatmap.ToBPM) {
            values[2].text = SelectedBeatmap.FromBPM + "BPM";
        } else {
            values[2].text = string.Format("{0}-{1}BPM", SelectedBeatmap.FromBPM, SelectedBeatmap.ToBPM);
        }

        if (JacketPath != "null") {
            Jacket.sprite = LoadingScene.LoadJacket(JacketPath);
        }

        foreach (TextMeshProUGUI obj in Difficulties) {
            DestroyImmediate(obj.gameObject);
        }
        Difficulties.Clear();

        foreach (OsuFile osuFile in SelectedBeatmap.osuFiles) {
            TextMeshProUGUI tmp = Instantiate(Prefab, content.transform);
            tmp.text = osuFile["Metadata"]["Version"];
            Difficulties.Add(tmp);
        }
    }

    public void Confirm() {
        SelectedBeatmap.ConvertOsus();
        File.WriteAllText(string.Format("{0}\\Songs\\{1}\\{2}.ini", GameManager.appdata, SelectedBeatmap.BeatmapId, "info"), SelectedBeatmap.Data2Ini(JacketPath.Split("\\")[^1]).ToString());
        File.Copy(string.Format("{0}\\{1}", SelectedPath, SelectedBeatmap.AudioPath), string.Format("{0}\\Songs\\{1}\\{2}", GameManager.appdata, SelectedBeatmap.BeatmapId, SelectedBeatmap.AudioPath));
        File.Copy(JacketPath, string.Format("{0}\\Songs\\{1}\\{2}", GameManager.appdata, SelectedBeatmap.BeatmapId, JacketPath.Split("\\")[^1]));

        if (IsTempFolder) Directory.Delete(SelectedPath, true);

        SceneAnimation.LoadScene(1);
    }

    public void Exit() {
        SceneAnimation.LoadScene(2);
    }
}
