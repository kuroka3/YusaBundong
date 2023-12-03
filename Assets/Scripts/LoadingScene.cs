using System;
using System.Collections;
using System.IO;
using System.IO.Compression;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class LoadingScene : MonoBehaviour
{
    public TextMeshProUGUI info;

    void Awake() {
        ProgressBar.value = 0f;

        IEnumerator Init() {
            yield return new AsyncOperation();
            loadInfo("Loading charts...");
            ErrorDisplayer.ClearLog();

            if(!GameManager.loadCharts()) {
                loadInfo("Downloading Charts...");
                downloadDefaultSong();
            } else {
                ProgressBar.value = 0.3333f;

                loadInfo("Loading settings...");
                SettingsManager.Load();

                ProgressBar.value = 0.6666f;

                loadInfo("Loading languages...");
                LanguageScript.Load();

                ProgressBar.value = 1f;

                print(LanguageScript.lang["name"]);
                SceneManager.LoadScene(2);
            }
        }

        StartCoroutine(Init());
    }

    private void loadInfo(string infoS) {
        info.text = infoS;
    }

    public void import() {
        SceneManager.LoadScene(6);
    }

    private void downloadDefaultSong() {
        IEnumerator Confirm(string URL, string path) {
            Download getURL = new(URL);

            yield return StartCoroutine(getURL.Get());

            if(!getURL.isFailed) {
                string downloadURL = System.Text.Encoding.UTF8.GetString(getURL.data);

                Download download = new(downloadURL);

                yield return StartCoroutine(download.Get());

                if(!download.isFailed) {
                    File.WriteAllBytes(path, download.data);

                    string songPath = GameManager.appdata + "\\Songs";

                    if(!Directory.Exists(songPath)) Directory.CreateDirectory(songPath);
                    ZipFile.ExtractToDirectory(path, songPath);
                    
                    File.Delete(path);
                    Reload();
                }
            }
        }

        if(!Directory.Exists(GameManager.tmpFolder)) Directory.CreateDirectory(GameManager.tmpFolder);
        string zipFileName = GameManager.tmpFolder + "\\" + Guid.NewGuid().ToString() + ".tmp";

        StartCoroutine(Confirm("https://pastebin.com/raw/5fXzGBdk", zipFileName));
    }

    private void Reload() {
        Awake();
    }
}

// public static class downloadManager {
    // public static IEnumerator Download(string URL) {
    //         UnityWebRequest req = UnityWebRequest.Get(URL);

    //         yield return req.SendWebRequest();

    //         if (req.result != UnityWebRequest.Result.Success) {
    //             isFailed = true;
    //             loadInfo("Download Failed!");
    //         }
    //         else File.WriteAllBytes(path, req.downloadHandler.data);
    //     }
// }

public class Download {

    public byte[] data;
    public string url;
    public bool isFailed = false;

    public Download(string URL) {
        url = URL;
    }

    public IEnumerator Get() {
        UnityWebRequest req = UnityWebRequest.Get(url);

        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success) {
            data = null;
            isFailed = true;
        }
        else {
            data = req.downloadHandler.data;
        }
    }
}