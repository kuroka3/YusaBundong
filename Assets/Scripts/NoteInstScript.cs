using System.Collections;
using UnityEngine;

public class NoteInstScript : MonoBehaviour
{

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
        namsi = mytime - (NoteScript.time + GameManager.OffsetFloat);

        MoveNote();
        JudgeNote();
    }

    void MoveNote() {
        if(GameManager.paused) return;

        if(namsi > -GameManager.judgement[3]) {
            transform.position = new Vector3(lineX, namsi * SettingsManager.hispeed - 7f);
        } else {
            if(longJudging == false) {
                JudgementScript.judge(namsi);
                GameManager.priority[lineNum].Remove(id);
                GameManager.instance.pool.Release(gameObject);
                clear();
            }
        }

        if (removing) {
                GameManager.priority[lineNum].Remove(id);
                removing = false;
                longJudging = false;
                GameManager.instance.pool.Release(gameObject);
                clear();
                return;
            }

        if (longJudging) {
            transform.position = new Vector3(lineX, -7f);
            float namsi2 = endTime - (NoteScript.time + GameManager.OffsetFloat);
            
            if(namsi2 > -GameManager.judgement[3]) {
                length = endTime - (NoteScript.time + GameManager.OffsetFloat);
                if(length < 0) length = 0;
                transform.localScale = new Vector3(2f, GetLongnoteLength(length));
            }
        }
    }

    void JudgeNote() {
        if (GameManager.paused) return;

        if (namsi <= GameManager.judgement[3]) {
            if(!GameManager.priority[lineNum].Contains(id)) {
                GameManager.priority[lineNum].Add(id);
            }

            if(Input.GetKeyDown(SettingsManager.keys[lineNum]) && GameManager.priority[lineNum][0] == id) {
                JudgementScript.judge(namsi);
                if (length == -1) removing = true; else longJudging = true;
            }

            if (longJudging) {
                float namsi2 = endTime - (NoteScript.time + GameManager.OffsetFloat);

                if (Input.GetKeyUp(SettingsManager.keys[lineNum])) {
                    JudgementScript.judge(namsi2);
                    removing = true;
                } else {
                    if(namsi2 <= -GameManager.judgement[3]) {
                        JudgementScript.judge(namsi2);
                        removing = true;
                    }
                }
            }
        }
    }

    public void sendData(int[] two) {
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

            transform.localScale = new Vector3(2f, GetLongnoteLength(length));
        }
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

    private float GetLongnoteLength(float length) {
        return length * SettingsManager.hispeed;
    }
}
