using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScene : MonoBehaviour
{
    public TextMeshProUGUI info;
    public GameObject button;

    void Awake() {
        ProgressBar.value = 0f;
        button.SetActive(false);

        IEnumerator Init() {
            yield return new AsyncOperation();
            loadInfo("Loading charts...");
            ErrorDisplayer.ClearLog();

            if(!GameManager.loadCharts()) {
                loadInfo("No Songs Found!");
                button.SetActive(true);
            } else {
                ProgressBar.value = 0.5f;

                loadInfo("Loading settings...");
                SettingsManager.Load();

                ProgressBar.value = 1.0f;
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
}
