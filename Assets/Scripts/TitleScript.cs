using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScript : MonoBehaviour
{

    void Update() {
        if(Input.GetMouseButtonDown(0)) {
            SceneManager.LoadScene(1);
        }
    }
}
