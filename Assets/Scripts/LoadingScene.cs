using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScene : MonoBehaviour
{
    public TextMeshProUGUI info;

    void Awake() {

        IEnumerator load() {
            yield return new WaitForSeconds(1.0f);
            loadInfo("charts");

            GameManager.loadCharts();

            SceneManager.LoadScene("SongListScene");
        }

        StartCoroutine(load());
    }

    private void loadInfo(string infoS) {
        info.text = "Loading " + infoS + "...";
    }
}
