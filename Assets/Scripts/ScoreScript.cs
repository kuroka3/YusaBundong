using System;
using TMPro;
using UnityEngine;

public class ScoreScript : MonoBehaviour
{
    public TextMeshProUGUI score;
    public TextMeshProUGUI combo;
    public TextMeshProUGUI acc;
    

    void FixedUpdate() {
        score.text = Math.Round(GameManager.score).ToString();
        combo.text = GameManager.combo.ToString();
        acc.text = (Math.Round(GameManager.acc*10000)*0.01).ToString() + "%";
    }
}
