using System.Collections;
using UnityEngine;

public class NoteInstScript : MonoBehaviour
{

    private AudioSource audioS;
    private int[] data;
    private int lineNum = -1;
    private float lineX = 0f;
    private float mytime = 0f;
    private int id = -1;
    private float endTime = -1;
    private float length = -1;
    private float namsi = 0f;

    private bool removing = false;
    private bool longJudging = false;

    void Update() {
        if(GameManager.paused) return;

        namsi = mytime - (audioS.time + (float.Parse(GameManager.instance.offset.ToString())*0.001f));

        if(namsi > -GameManager.judgement[3]) {
                transform.position = new Vector3(lineX, namsi * GameManager.instance.hispeed - 6.6f);
        } else {
            if(longJudging == false) {
                JudgementScript.judge(namsi);
                GameManager.priority[lineNum].Remove(id);
                GameManager.instance.pool.Release(gameObject);
                clear();
            }
        }
    }

    private IEnumerator noteUpdate() {
        while(true) {
            if (removing) {
                GameManager.priority[lineNum].Remove(id);
                removing = false;
                longJudging = false;
                GameManager.instance.pool.Release(gameObject);
                clear();
                break;
            }

            if (longJudging) {
                transform.position = new Vector3(lineX, -6.7f);
                if (Input.GetKeyUp(GameManager.instance.keys[lineNum])) {
                    float namsi2 = endTime - (audioS.time + (float.Parse(GameManager.instance.offset.ToString())*0.001f));
                    JudgementScript.judge(namsi2);
                    removing = true;
                } else {
                    float namsi2 = endTime - (audioS.time + (float.Parse(GameManager.instance.offset.ToString())*0.001f));
                    if(namsi2 <= -GameManager.judgement[3]) {
                        JudgementScript.judge(namsi2);
                        removing = true;
                    } else {
                        length = endTime - (audioS.time + (float.Parse(GameManager.instance.offset.ToString())*0.001f));
                        if(length < 0) length = 0;
                        transform.localScale = new Vector3(3.65f, length * (GameManager.instance.hispeed*6.5831f-12.8649f)+(GameManager.instance.hispeed*0.0044f-0.1436f));
                    }
                }
                
            }

            if (namsi <= GameManager.judgement[3]) {
                if(!GameManager.priority[lineNum].Contains(id)) {
                    GameManager.priority[lineNum].Add(id);
                }

                if(Input.GetKeyDown(GameManager.instance.keys[lineNum]) && GameManager.priority[lineNum][0] == id) {
                    JudgementScript.judge(namsi);
                    if (length == -1) removing = true; else longJudging = true;
                }
            }

            yield return new WaitForSeconds(0.001f);
        }

        yield return null;
    }

    public void sendData(AudioSource one, int[] two) {
        audioS = one;

        data = two;

        lineNum = data[0];

        switch (lineNum)
        {
            case 0: 
                lineX = -3f;
                break;
            case 1: 
                lineX = -1f;
                break;
            case 2: 
                lineX = 1f;
                break;
            case 3: 
                lineX = 3f;
                break;
            default: 
                lineX = -6.0f;
                break;
        }

        mytime = data[1];
        mytime = mytime*0.001f;

        id = data[2];
        endTime = data[3];

        if(endTime != -1f) {
            endTime = endTime*0.001f;
            length = endTime - mytime;

            transform.localScale = new Vector3(3.65f, length * (GameManager.instance.hispeed*6.5831f-12.8649f)+(GameManager.instance.hispeed*0.0044f-0.1436f));
        }

        StartCoroutine(noteUpdate());
    }

    public void clear() {
        lineNum = -1;
        lineX = 0f;
        mytime = 0f;
        id = -1;
        endTime = -1;
        length = -1;
        namsi = 0f;

        removing = false;
        longJudging = false;
    }
}
