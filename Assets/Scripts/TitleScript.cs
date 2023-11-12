using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScript : MonoBehaviour
{
    void Start() {
        Application.targetFrameRate = 144;
    }

    void Update() {
        
        if(Input.GetMouseButtonDown(0)) {
            SceneManager.LoadScene(1);
        }
    }
}
