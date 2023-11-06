using System;
using TMPro;
using UnityEngine;

public class JudgementScript : MonoBehaviour
{
    public Sprite[] sprites;
    private SpriteRenderer rend;
    public TextMeshProUGUI judgement;
    private static JudgementScript instance;

    void Awake() {
        rend = gameObject.GetComponent<SpriteRenderer>();
        instance = this;
    }

    public static void judge(float milis) {
        int judgecode = 0;

        if (milis <= GameManager.judgement[0] && milis >= -GameManager.judgement[0]) {
            judgecode = 0;
        } else if (milis <= GameManager.judgement[1] && milis >= -GameManager.judgement[1]) {
            judgecode = 1;
        } else if (milis <= GameManager.judgement[2] && milis >= -GameManager.judgement[2]) {
            judgecode = 2;
        } else if (milis <= GameManager.judgement[3] && milis >= -GameManager.judgement[3]) {
            judgecode = 3;
        } else {
            judgecode = 4;
        }
        instance.rend.sprite = instance.sprites[judgecode];
        instance.judgement.text = Math.Round(milis*1000) + "ms";
    }
}
