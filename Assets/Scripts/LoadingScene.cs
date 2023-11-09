using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScene : MonoBehaviour
{
    public TextMeshProUGUI info;

    void Awake() {
        loadInfo("charts");
        GameManager.loadCharts();

        SceneManager.LoadScene("SongListScene");
    }

    private void loadInfo(string infoS) {
        info.text = "Loading " + infoS + "...";
    }
}
