using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreScript : MonoBehaviour
{
    public TextMeshProUGUI score;
    public TextMeshProUGUI combo;
    public TextMeshProUGUI acc;
    

    void FixedUpdate() {
        score.text = GameManager.score.ToString();
        combo.text = GameManager.combo.ToString();
        acc.text = (Math.Round(GameManager.acc*10000)/100).ToString() + "%";
    }
}
