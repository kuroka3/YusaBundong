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

    public Sprite[] jackets;
    public string[] names;
    public string[] artists;

    void Awake() {
        updateData();
    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.Space)) {
            startGame();
        }
    }
    
    private void updateData() {
        int songcode = GameManager.songCode;

        nameT.text = names[songcode];
        jacket.sprite = jackets[songcode];
        artist.text = artists[songcode];
    }

    public void left() {
        if (GameManager.songCode > 0) {
            GameManager.songCode--;
            updateData();
        } else {
            GameManager.songCode = names.Length - 1;
            updateData();
        }
    }

    public void right() {
        if (GameManager.songCode < names.Length - 1) {
            GameManager.songCode++;
            updateData();
        } else {
            GameManager.songCode = 0;
            updateData();
        }
    }

    private void startGame() {
        SceneManager.LoadScene("IngameScene");
    }
}
