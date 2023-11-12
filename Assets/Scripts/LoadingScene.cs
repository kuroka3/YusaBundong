using System.Collections;
using TMPro;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScene : MonoBehaviour
{
    public TextMeshProUGUI info;

    void Awake() {

        IEnumerator load() {
            yield return new WaitForSeconds(0.5f);
            loadInfo("Loading charts...");
            ErrorDisplayer.ClearLog();

            if(GameManager.loadCharts()) {
                SceneManager.LoadScene(2);
            } else {
                loadInfo("No Songs Found!");
            }            
        }

        StartCoroutine(load());
    }

    private void loadInfo(string infoS) {
        info.text = infoS;
    }
}
