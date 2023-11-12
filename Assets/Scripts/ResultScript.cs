using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultScript : MonoBehaviour
{

    public TextMeshProUGUI[] judgeValues;
    public TextMeshProUGUI rank;
    public TextMeshProUGUI score;
    public TextMeshProUGUI acc;


    void Awake() {
        for (int i = 0; i<judgeValues.Length; i++) {
            TextMeshProUGUI judge = judgeValues[i];
            judge.text = GameManager.judges[i].ToString();
        }

        score.text = Math.Round(GameManager.score).ToString();
        acc.text = (Math.Round(GameManager.acc*10000)*0.01).ToString() + "%";

        if(GameManager.judges[1] + GameManager.judges[2] + GameManager.judges[3] + GameManager.judges[4] == 0) {
            rank.text = "PF";
        } else if (GameManager.judges[3] + GameManager.judges[4] == 0) {
            rank.text = "EX";
        } else if (GameManager.acc >= 0.95f) {
            rank.text = "S";
        } else if (GameManager.acc >= 0.9f) {
            rank.text = "A";
        } else if (GameManager.acc >= 0.8f) {
            rank.text = "B";
        } else if (GameManager.acc >= 0.7f) {
            rank.text = "C";
        } else if (GameManager.acc >= 0.6f) {
            rank.text = "D";
        } else {
            rank.text = "F";
        }
    }

    public void end() {
        GameManager.clear();
        SceneManager.LoadScene(2);
    }
}
