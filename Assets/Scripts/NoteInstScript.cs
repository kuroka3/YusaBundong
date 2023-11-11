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

    private bool removing = false;
    private bool longJudging = false;

    private IEnumerator noteUpdate() {
        while(true) {
            if (removing) {
                GameManager.priority[lineNum].Remove(id);
                removing = false;
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
                    longJudging = false;
                    yield return new WaitForSeconds(0.001f);
                    continue;
                } else {
                    length = endTime - (audioS.time + (float.Parse(GameManager.instance.offset.ToString())*0.001f));
                    transform.localScale = new Vector3(3.65f, length*168.71257f-0.02170f);
                    yield return new WaitForSeconds(0.001f);
                    continue;
                }
            }

            float namsi = mytime - (audioS.time + (float.Parse(GameManager.instance.offset.ToString())*0.001f));
            if(namsi > -GameManager.judgement[3]) {
                transform.position = new Vector3(lineX, namsi * 27.8333f - 6.7f);
            } else {
                JudgementScript.judge(namsi);
                GameManager.priority[lineNum].Remove(id);
                GameManager.instance.pool.Release(gameObject);
                clear();
                break;
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

            transform.localScale = new Vector3(3.65f, length*168.71257f-0.02170f);
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

        removing = false;
        longJudging = false;
    }
}
