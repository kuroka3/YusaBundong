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

    public void Continue() {
        if (NoteScript.instance.audioS.time - 3f >= 0) NoteScript.instance.audioS.time -= 3f;
        else NoteScript.instance.audioS.time = 0;
        NoteScript.time = NoteScript.instance.audioS.time;
        unpause();
    }

    public void unpause() {
        NoteScript.pauseSong(false);
        GameManager.paused = false;
        canvas.gameObject.SetActive(false);
    }

    public void retry() {
        unpause();
        NoteScript.pauseSong(true);
        GameManager.clear();
        SceneAnimation.LoadScene(3);
    }

    public void exit() {
        unpause();
        NoteScript.pauseSong(true);
        GameManager.clear();
        SceneAnimation.LoadScene(2);
    }
}
