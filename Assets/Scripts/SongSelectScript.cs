using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SongSelectScript : MonoBehaviour
{
    public TextMeshProUGUI nameT;
    public Image jacket;
    public TextMeshProUGUI artist;

    void Awake() {
        updateData();
    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.Space)) {
            startGame();
        } else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            left();
        } else if (Input.GetKeyDown(KeyCode.RightArrow)) {
            right();
        } else if (Input.GetKeyDown(KeyCode.F5)) {
            reload();
        }
    }
    
    private void updateData() {
        int songcode = GameManager.songCode;

        nameT.text = GameManager.datas[songcode][0];
        jacket.sprite = GameManager.jackets[songcode];
        artist.text = GameManager.datas[songcode][1];
    }

    public void left() {
        if (GameManager.songCode > 0) {
            GameManager.songCode--;
            updateData();
        } else {
            GameManager.songCode = GameManager.datas.Length - 1;
            updateData();
        }
    }

    public void right() {
        if (GameManager.songCode < GameManager.datas.Length - 1) {
            GameManager.songCode++;
            updateData();
        } else {
            GameManager.songCode = 0;
            updateData();
        }
    }

    public void reload() {
        SceneManager.LoadScene(1);
    }

    private void startGame() {
        SceneManager.LoadScene(3);
    }

    public void settings() {
        SceneManager.LoadScene(5);
    }
}
