using Newtonsoft.Json;
using UnityEngine;

public class NoteInstScript : MonoBehaviour
{

    private SpriteRenderer rend = null;

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
    
    void Awake() {
        if (rend == null) rend = GetComponent<SpriteRenderer>();
    }

    void Update() {
        if (!gameObject.activeSelf || id == -1) return;

        namsi = mytime - (NoteScript.time + GameManager.OffsetFloat);

        if (MoveNote()) JudgeNote();
    }

    bool MoveNote() {
        if(GameManager.paused) return false;

        if(namsi > -GameManager.judgement[3]) {
            transform.position = new Vector3(lineX, namsi * SettingsManager.GetAsFloat(Settings.HiSpeed) - 7f);
        } else {
            if(longJudging == false) {
                JudgementScript.judge(namsi);
                GameManager.priority[lineNum].Remove(id);
                GameManager.instance.pool.Release(gameObject);
                clear();
                return false;
            }
        }

        if (removing) {
            GameManager.priority[lineNum].Remove(id);
            removing = false;
            longJudging = false;
            GameManager.instance.pool.Release(gameObject);
            clear();
            return false;
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

        return true;
    }

    void JudgeNote() {
        if (namsi <= GameManager.judgement[3]) {
            if(!GameManager.priority[lineNum].Contains(id)) {
                GameManager.priority[lineNum].Add(id);
            }

            if(Input.GetKeyDown(JsonConvert.DeserializeObject<KeyCode[]>(SettingsManager.Get<object>(Settings.Keys).ToString())[lineNum]) && GameManager.priority[lineNum][0] == id) {
                JudgementScript.judge(namsi);
                if (length == -1) removing = true; else longJudging = true;
            }

            if (longJudging) {
                float namsi2 = endTime - (NoteScript.time + GameManager.OffsetFloat);

                if (Input.GetKeyUp(JsonConvert.DeserializeObject<KeyCode[]>(SettingsManager.Get<object>(Settings.Keys).ToString())[lineNum])) {
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

        if (lineNum == 1 || lineNum == 2) {
            rend.color = new Color(0f, 0.7f, 9f, 1f);
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

        rend.color = Color.white;
    }

    private float GetLongnoteLength(float length) {
        return length * SettingsManager.GetAsFloat(Settings.HiSpeed);
    }
}
