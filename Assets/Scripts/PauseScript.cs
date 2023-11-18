using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseScript : MonoBehaviour
{

    public Canvas canvas;

    void Awake() {
        canvas.gameObject.SetActive(false);
    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.Escape) && NoteScript.isPlayingSong()) {
            pause();
        }
    }

    public void pause() {
        NoteScript.pauseSong(true);
        GameManager.paused = true;
        canvas.gameObject.SetActive(true);
    }

    public void unpause() {
        NoteScript.pauseSong(false);
        GameManager.paused = false;
        canvas.gameObject.SetActive(false);
    }

    public void retry() {
        unpause();
        GameManager.clear();
        SceneManager.LoadScene(3);
    }

    public void exit() {
        unpause();
        GameManager.clear();
        SceneManager.LoadScene(2);
    }
}
