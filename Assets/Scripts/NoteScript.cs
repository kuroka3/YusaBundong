using System;
using System.Collections;
using UnityEngine;

public class NoteScript : MonoBehaviour
{
    public GameObject prefab;

    private AudioSource audioS;

    void Start()
    {
        GameManager.loadCharts();

        startSong(GameManager.songCode);
    }

    private void startSong(int songid) {
        audioS = gameObject.AddComponent<AudioSource>();
        audioS.clip = GameManager.songs[songid];

        IEnumerator startAudio() {
            yield return new WaitForSeconds(1);

            audioS.Play();
        }

        IEnumerator NoteSummon() {
            int id = 0;
            int stack = 0;

            for (int i = 0; i<GameManager.charts[songid].Length; i++) {
                string element = GameManager.charts[songid][i];
                try {
                    string[] testData = element.Split(',');
                    int[] testInts = new int[4]{Int32.Parse(testData[0]), Int32.Parse(testData[1]), id, Int32.Parse(testData[2])};
                } catch(Exception e) {
                    Debug.LogError(e);
                    continue;
                }

                GameObject clone = GameManager.instance.pool.Get(0);
                NoteInstScript cloneScript = clone.GetComponent<NoteInstScript>();

                string[] data = element.Split(',');

                cloneScript.sendData(audioS, new int[4]{Int32.Parse(data[0]), Int32.Parse(data[1]), id, Int32.Parse(data[2])});

                id++;
                stack++;

                if(stack == 50) {
                    stack = 0;
                    yield return new WaitForSeconds(((float.Parse(data[1])*0.001f)-2)-audioS.time);
                }
            }
        }


        StartCoroutine(NoteSummon());
        StartCoroutine(startAudio());
    }
}
