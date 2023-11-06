using UnityEngine;

public class NoteInstScript : MonoBehaviour
{

    public AudioSource audioS;
    public int[] data;

    private int lineNum = -1;
    private float lineX = 0f;
    private float mytime = 0f;
    private int id = -1;
    private float endTime = -1;
    private float length = -1;

    private int removing = 0;
    private int longJudging = 0;

    void Update() {
        if (removing == 1) {
            GameManager.priority[lineNum].Remove(id);
            removing = 0;
            GameManager.instance.pool.Release(gameObject);
            clear();
            return;
        }

        if (longJudging == 1) {
            transform.position = new Vector3(lineX, -6.7f);
            if (Input.GetKeyUp(GameManager.instance.keys[lineNum])) {
                float namsi2 = endTime - (audioS.time + (float.Parse(GameManager.instance.offset.ToString())/1000));
                JudgementScript.judge(namsi2);
                removing = 1;
                longJudging = 0;
                return;
            } else {
                length = endTime - (audioS.time + (float.Parse(GameManager.instance.offset.ToString())/1000));
                transform.localScale = new Vector3(3, length*168.71257f-0.02170f);
                return;
            }
        }

        float namsi = mytime - (audioS.time + (float.Parse(GameManager.instance.offset.ToString())/1000));
        if(namsi > -GameManager.judgement[3]) {
            transform.position = new Vector3(lineX, namsi * (167/6) - 6.7f);
        } else {
            JudgementScript.judge(namsi);
            GameManager.priority[lineNum].Remove(id);
            GameManager.instance.pool.Release(gameObject);
            clear();
            return;
        }

        if (namsi <= GameManager.judgement[3]) {
            if(!GameManager.priority[lineNum].Contains(id)) {
                GameManager.priority[lineNum].Add(id);
            }

            if(Input.GetKeyDown(GameManager.instance.keys[lineNum]) && GameManager.priority[lineNum][0] == id) {
                JudgementScript.judge(namsi);
                if (length == -1) removing = 1; else longJudging = 1;
            }
        }
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
        mytime = mytime/1000;

        id = data[2];
        endTime = data[3];

        if(endTime != -1f) {
            endTime = endTime/1000;
            length = endTime - mytime;

            transform.localScale = new Vector3(3, length*168.71257f-0.02170f);

            Debug.Log(id + ", " + endTime + ", " + length);
        }
    }

    public void clear() {
        lineNum = -1;
        lineX = 0f;
        mytime = 0f;
        id = -1;
        endTime = -1;
        length = -1;

        removing = 0;
        longJudging = 0;
    }
}
