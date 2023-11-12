using System;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

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


        switch(judgecode) {
            case 0:
                GameManager.combo++;
                GameManager.score += GameManager.combo/3;
                GameManager.judgeHis.Add(1f);
                break;
            case 1:
                GameManager.combo++;
                GameManager.score += GameManager.combo/3*0.6667f;
                GameManager.judgeHis.Add(0.6667f);
                break;
            case 2:
                GameManager.combo++;
                GameManager.score += GameManager.combo/3*0.3333f;
                GameManager.judgeHis.Add(0.3333f);
                break;
            case 3:
            case 4:
                GameManager.combo = 0;
                GameManager.judgeHis.Add(0f);
                break;
            default:
                break;
        }
        GameManager.judges[judgecode]++;

        float addHis = 0f;
        float count = 0;

        foreach (float accF in GameManager.judgeHis) {
            addHis += accF;
            count++;
        }

        GameManager.acc = addHis/count;
    }
}
